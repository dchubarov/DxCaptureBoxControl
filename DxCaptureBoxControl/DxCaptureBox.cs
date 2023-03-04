using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
// DirectShowLib
using DirectShowLib;

namespace Gyrosoft.WinForms
{
    public partial class DxCaptureBox : UserControl, ISampleGrabberCB
    {
        #region Private Fields

        private DxCaptureBoxState _state = DxCaptureBoxState.Inactive;
        private Color _inactiveBorderColor = SystemColors.Control;
        private Color _playBorderColor = Color.Red;
        private Color _pauseBorderColor = Color.Yellow;
        private Color _stillBorderColor = Color.Green;

        private IFilterGraph2 _graphBuilder = null;
        private IPin _pinStill = null;
        private ISampleGrabber _sampleGrabber = null;
        private ManualResetEvent _pictureReadyEvent = new ManualResetEvent(false);
        private bool _wantSnapshot = false;
        private IntPtr _snapshotBuffer = IntPtr.Zero;

        private int _videoWidth = 0;
        private int _videoHeight = 0;
        private int _stride = 0;
        private bool _interlaced = false;

#if DEBUG
        private DsROTEntry _rot = null;
#endif

        #endregion

        #region Public Constructors

        public DxCaptureBox()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Properties

        public DxCaptureBoxState State
        {
            get
            {
                return _state;
            }
        }

        public int VideoWidth
        {
            get
            {
                return _videoWidth;
            }
        }

        public int VideoHeight
        {
            get
            {
                return _videoHeight;
            }
        }

        public Size PreferredBoxSize
        {
            get
            {
                return new Size(
                    _videoWidth + this.Padding.Left + this.Padding.Right, 
                    _videoHeight + this.Padding.Top + this.Padding.Bottom);
            }
        }

        public Size VideoSize
        {
            get
            {
                return new Size(_videoWidth, _videoHeight);
            }
        }

        public bool Interlaced
        {
            get
            {
                return _interlaced;
            }
        }

        [Description("Inactive mode border color.")]
        public Color InactiveBorderColor
        {
            get
            {
                return _inactiveBorderColor;
            }

            set
            {
                _inactiveBorderColor = value;
                SetDisplayState();
            }
        }

        [Description("Video preview mode border color.")]
        public Color PlayBorderColor
        {
            get
            {
                return _playBorderColor;
            }

            set
            {
                _playBorderColor = value;
                SetDisplayState();
            }
        }

        [Description("Video pause mode border color.")]
        public Color PauseBorderColor
        {
            get
            {
                return _pauseBorderColor;
            }

            set
            {
                _pauseBorderColor = value;
                SetDisplayState();
            }
        }

        [Description("Still image display mode border color.")]
        public Color StillBorderColor
        {
            get
            {
                return _stillBorderColor;
            }

            set
            {
                _stillBorderColor = value;
                SetDisplayState();
            }
        }

        #endregion

        #region Public Methods

        public String[] GetDeviceNames()
        {
            DsDevice[] captureDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            if (captureDevices.Length < 1)
                return null;
            else {
                String[] names = new String[captureDevices.Length];
                int index = 0;
                foreach (DsDevice dev in captureDevices) {
                    names[index++] = dev.Name;
                }
                return names;
            }
        }

        public void ShowStillImage(Image img)
        {
            if (_state == DxCaptureBoxState.Paused || _state == DxCaptureBoxState.Playing) {
                Stop();
            }

            videoBox.Image = img;
            _state = DxCaptureBoxState.Still;
            SetDisplayState();
        }

        public void Play(int deviceIndex)
        {
            Play(deviceIndex, 0, 0, 0, false);
        }

        public void Play(int deviceIndex, int width, int height, short bpp, bool interlaced)
        {
            int hr = 0;

            if (_state == DxCaptureBoxState.Inactive || _state == DxCaptureBoxState.Still) {
                videoBox.Image = null;
                _state = DxCaptureBoxState.Inactive;

                DsDevice[] captureDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
                if (deviceIndex + 1 > captureDevices.Length) {
                    throw new Exception("Invalid video capture device.");
                }

                SetupGraph(captureDevices[deviceIndex], width, height, bpp, interlaced);
            }

            if (_state == DxCaptureBoxState.Inactive || _state == DxCaptureBoxState.Paused) {
                IMediaControl mediaCtl = _graphBuilder as IMediaControl;
                hr = mediaCtl.Run();
                DsError.ThrowExceptionForHR(hr);
            }

            _state = DxCaptureBoxState.Playing;
            SetDisplayState();
        }

        public void Stop()
        {
            if (_graphBuilder != null) {
                IMediaControl mediaCtl = _graphBuilder as IMediaControl;
                mediaCtl.StopWhenReady();

                IVideoWindow videoWindow = _graphBuilder as IVideoWindow;
                videoWindow.put_Visible(OABool.False);
                videoWindow.put_Owner(IntPtr.Zero);
                Marshal.ReleaseComObject(videoWindow);

#if DEBUG
                if (_rot != null) {
                    _rot.Dispose();
                    _rot = null;
                }
#endif

                ReleaseInterfaces();
            }

            _state = DxCaptureBoxState.Inactive;
            SetDisplayState();
        }

        public void Pause()
        {
            int hr = 0;

            if (_state == DxCaptureBoxState.Playing) {
                if (_graphBuilder != null) {
                    IMediaControl mediaCtl = _graphBuilder as IMediaControl;
                    hr = mediaCtl.Pause();
                    DsError.ThrowExceptionForHR(hr);
                }

                _state = DxCaptureBoxState.Paused;
                SetDisplayState();
            }
        }

        public Bitmap Snap()
        {
            int hr = 0;
            Bitmap bmp = null;

            if (_state == DxCaptureBoxState.Playing) {
                _pictureReadyEvent.Reset();
                _wantSnapshot = true;

                try {
                    /*
                    hr = _sampleGrabber.SetCallback(this, 1);
                    DsError.ThrowExceptionForHR(hr);
                    */

                    int bufferSize = 0;
                    hr = _sampleGrabber.GetCurrentBuffer(ref bufferSize, IntPtr.Zero);
                    DsError.ThrowExceptionForHR(hr);
                    _snapshotBuffer = Marshal.AllocCoTaskMem(bufferSize);

                    hr = _sampleGrabber.GetCurrentBuffer(ref bufferSize, _snapshotBuffer);
                    DsError.ThrowExceptionForHR(hr);

                    /*
                    if (!_pictureReadyEvent.WaitOne(1000, true)) {
                        throw new Exception("Cannot grab picture. Timeout expired.");
                    }
                     * */

                    bmp = new Bitmap(
                        this.VideoWidth, 
                        _interlaced ? this.VideoHeight >> 1 : this.VideoHeight, 
                        _stride, 
                        PixelFormat.Format24bppRgb, 
                        _snapshotBuffer
                        );

                    bmp.RotateFlip(RotateFlipType.Rotate180FlipX);

                    if (_interlaced) {
                        using (Graphics g = Graphics.FromImage(bmp)) {
                            Bitmap bmp2 = new Bitmap(
                                bmp,
                                this.VideoWidth,
                                this.VideoHeight
                                );

                            bmp.Dispose();
                            bmp = bmp2;                                
                        }
                    }
                }
                finally {
                    _sampleGrabber.SetCallback(null, 0);

                    if (_snapshotBuffer != IntPtr.Zero) {
                        Marshal.FreeCoTaskMem(_snapshotBuffer);
                        _snapshotBuffer = IntPtr.Zero;
                    }
                }
            }

            return bmp;
        }

        #endregion

        #region Private Methods

        private void SetupGraph(DsDevice device, int width, int height, short bpp, bool interlaced)
        {
            int hr = 0;
            IBaseFilter capFilter = null;

            _interlaced = false;
            _graphBuilder = new FilterGraph() as IFilterGraph2;

#if DEBUG
            _rot = new DsROTEntry(_graphBuilder);
#endif

            // add video input device
            hr = _graphBuilder.AddSourceFilterForMoniker(device.Mon, null, device.Name, out capFilter);
            DsError.ThrowExceptionForHR(hr);

            // get still pin
            _pinStill = DsFindPin.ByCategory(capFilter, PinCategory.Still, 0);

            if (_pinStill == null) {
                _pinStill = DsFindPin.ByCategory(capFilter, PinCategory.Preview, 0);
            }

            if (_pinStill != null) {
                if (width > 0 && height > 0 && bpp > 0) {
                    SetupStream(_pinStill, width, height, bpp, interlaced);
                }
            }

            _sampleGrabber = new SampleGrabber() as ISampleGrabber;
            IBaseFilter baseGrabFilter = _sampleGrabber as IBaseFilter;
            SetupSampleGrabber(_sampleGrabber);

            IBaseFilter renderFilter = new VideoRendererDefault() as IBaseFilter;
            hr = _graphBuilder.AddFilter(renderFilter, "DxCaptureBoxControl");
            DsError.ThrowExceptionForHR(hr);

            hr = _graphBuilder.AddFilter(baseGrabFilter, "DxCaptureBoxControl.Grabber");
            DsError.ThrowExceptionForHR(hr);

            IPin samplePin = DsFindPin.ByDirection(baseGrabFilter, PinDirection.Input, 0);
            IPin capturePin = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);
            IPin renderPin = DsFindPin.ByDirection(renderFilter, PinDirection.Input, 0);

            hr = _graphBuilder.Connect(capturePin, renderPin);
            DsError.ThrowExceptionForHR(hr);

            hr = _graphBuilder.Connect(_pinStill, samplePin);
            DsError.ThrowExceptionForHR(hr);

            GetSizeInfo(_sampleGrabber);
            SetupVideoWindow();
        }

        private void SetupStream(IPin pin, int width, int height, short bpp, bool interlaced)
        {
            IAMStreamConfig streamConfig = pin as IAMStreamConfig;
            VideoInfoHeader videoInfo;
            AMMediaType media;
            int hr = 0;

            hr = streamConfig.GetFormat(out media);
            DsError.ThrowExceptionForHR(hr);

            try {
                videoInfo = new VideoInfoHeader();
                Marshal.PtrToStructure(media.formatPtr, videoInfo);

                if (width > 0) {
                    videoInfo.BmiHeader.Width = width;
                }

                if (height > 0) {
                    videoInfo.BmiHeader.Height = height;
                    if (interlaced) videoInfo.BmiHeader.Height >>= 1;
                    _interlaced = interlaced;
                }

                if (bpp > 0) {
                    videoInfo.BmiHeader.BitCount = bpp;
                }

                Marshal.StructureToPtr(videoInfo, media.formatPtr, false);
                hr = streamConfig.SetFormat(media);
                DsError.ThrowExceptionForHR(hr);
            }
            finally {
                DsUtils.FreeAMMediaType(media);
            }
        }

        private void SetupSampleGrabber(ISampleGrabber sampleGrabber)
        {
            AMMediaType media = new AMMediaType();
            int hr = 0;

            media.majorType = MediaType.Video;
            media.subType = MediaSubType.RGB24;
            media.formatType = FormatType.VideoInfo;

            hr = sampleGrabber.SetMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            DsUtils.FreeAMMediaType(media);
            media = null;

            hr = sampleGrabber.SetBufferSamples(true);
            DsError.ThrowExceptionForHR(hr);

            hr = sampleGrabber.SetOneShot(false);
            DsError.ThrowExceptionForHR(hr);
        }

        private void SetupVideoWindow()
        {
            IVideoWindow videoWindow = _graphBuilder as IVideoWindow;
            int hr = 0;

            hr = videoWindow.put_Owner(videoBox.Handle);
            DsError.ThrowExceptionForHR(hr);

            hr = videoWindow.put_WindowStyle(WindowStyle.Child | 
                WindowStyle.ClipChildren | WindowStyle.ClipSiblings);
            DsError.ThrowExceptionForHR(hr);

            hr = videoWindow.put_Visible(OABool.True);
            DsError.ThrowExceptionForHR(hr);

            ResizeVideoWindow();
        }

        private void ResizeVideoWindow()
        {
            IVideoWindow videoWindow = _graphBuilder as IVideoWindow;
            videoWindow.SetWindowPosition(0, 0, videoBox.Width, videoBox.Height);
        }

        private void ReleaseInterfaces()
        {
            try {
                if (_graphBuilder != null) {
                    IMediaControl mediaCtl = _graphBuilder as IMediaControl;
                    mediaCtl.Stop();
                }
            }
            catch (Exception) {
                // do nothing
            }

            if (_sampleGrabber != null) {
                Marshal.ReleaseComObject(_sampleGrabber);
                _sampleGrabber = null;
            }

            if (_graphBuilder != null) {
                Marshal.ReleaseComObject(_graphBuilder);
                _graphBuilder = null;
            }

            if (_pinStill != null) {
                Marshal.ReleaseComObject(_pinStill);
                _pinStill = null;
            }

            GC.Collect();
        }

        private void SetDisplayState()
        {
            switch (_state) {
                case DxCaptureBoxState.Paused:
                    this.BackColor = _pauseBorderColor;
                    break;
                case DxCaptureBoxState.Playing:
                    this.BackColor = _playBorderColor;
                    break;
                case DxCaptureBoxState.Still:
                    this.BackColor = _stillBorderColor;
                    break;
                default:
                    this.BackColor = _inactiveBorderColor;
                    break;
            }
        }

        private void GetSizeInfo(ISampleGrabber sampleGrabber)
        {
            int hr;

            AMMediaType media = new AMMediaType();

            hr = sampleGrabber.GetConnectedMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero)) {
                throw new NotSupportedException("Unknown Grabber Media Format");
            }

            VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            _videoWidth = videoInfoHeader.BmiHeader.Width;
            _videoHeight = videoInfoHeader.BmiHeader.Height;
            _stride = _videoWidth * (videoInfoHeader.BmiHeader.BitCount / 8);
            if (_interlaced) _videoHeight <<= 1;

            DsUtils.FreeAMMediaType(media);
            media = null;
        }

        #endregion

        #region DxCaptureBox Event Handlers (Control)

        private void DxCaptureBox_Load(object sender, EventArgs e)
        {
            SetDisplayState();
        }

        private void DxCaptureBox_Resize(object sender, EventArgs e)
        {
            if (_graphBuilder != null) {
                ResizeVideoWindow();
            }
        }

        #endregion

        #region ISampleGrabberCB Members

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] int Length);

        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            if (_wantSnapshot) {
                _wantSnapshot = false;
                CopyMemory(_snapshotBuffer, pBuffer, BufferLen);
                _pictureReadyEvent.Set();
            }

            return 0;
        }

        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        #endregion
    }
}
