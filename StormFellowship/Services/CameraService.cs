using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace StormFellowship.Services;

public class CameraService : IDisposable
{
    private static CameraService? _instance;
    public static CameraService Instance => _instance ??= new CameraService();

    private DispatcherTimer? _frameTimer;
    private WriteableBitmap? _cameraBitmap;
    private int _width = 640;
    private int _height = 480;
    private int _frameCounter = 0;
    private readonly Random _random = new();

    public bool IsCameraActive { get; private set; } = false;
    public ImageSource? CurrentFrame => _cameraBitmap;

    public event Action<ImageSource?>? FrameUpdated;
    public event Action<bool>? CameraStateChanged;

    public CameraService()
    {
        InitializeBitmap();
    }

    private void InitializeBitmap()
    {
        _cameraBitmap = new WriteableBitmap(_width, _height, 96, 96, PixelFormats.Bgr32, null);
    }

    public void StartCamera()
    {
        if (IsCameraActive) return;

        IsCameraActive = true;
        CameraStateChanged?.Invoke(true);

        if (_frameTimer == null)
        {
            _frameTimer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(33) // ~30 FPS
            };
            _frameTimer.Tick += OnFrameTick;
        }
        _frameTimer.Start();
    }

    public void StopCamera()
    {
        if (!IsCameraActive) return;

        IsCameraActive = false;
        _frameTimer?.Stop();
        CameraStateChanged?.Invoke(false);
        FrameUpdated?.Invoke(null);
    }

    public void ToggleCamera()
    {
        if (IsCameraActive) StopCamera();
        else StartCamera();
    }

    private void OnFrameTick(object? sender, EventArgs e)
    {
        if (!IsCameraActive || _cameraBitmap == null) return;

        _frameCounter++;
        RenderLiveCameraFrame();
        FrameUpdated?.Invoke(_cameraBitmap);
    }

    private void RenderLiveCameraFrame()
    {
        if (_cameraBitmap == null) return;

        try
        {
            _cameraBitmap.Lock();
            unsafe
            {
                IntPtr pBackBuffer = _cameraBitmap.BackBuffer;
                int stride = _cameraBitmap.BackBufferStride;

                double time = _frameCounter * 0.05;
                int centerX = _width / 2 + (int)(Math.Sin(time * 0.8) * 15);
                int centerY = _height / 2 + (int)(Math.Cos(time * 0.6) * 10);
                int headRadius = 90;

                for (int y = 0; y < _height; y++)
                {
                    uint* row = (uint*)(pBackBuffer + y * stride);
                    double ny = (double)y / _height;

                    for (int x = 0; x < _width; x++)
                    {
                        double nx = (double)x / _width;

                        // Modern Studio Ambient Backdrop
                        byte bgB = (byte)(18 + (int)(15 * Math.Sin(nx * Math.PI)));
                        byte bgG = (byte)(28 + (int)(25 * Math.Cos(ny * Math.PI)));
                        byte bgR = (byte)(45 + (int)(30 * Math.Sin(time + nx * 2.0)));

                        // User Silhouette / Face Biometric Model
                        int dx = x - centerX;
                        int dy = y - (centerY - 20);
                        double distHead = Math.Sqrt(dx * dx + dy * dy);

                        // Body / Shoulders
                        int dyBody = y - (centerY + 110);
                        int dxBody = (int)(dx * 0.65);
                        double distBody = Math.Sqrt(dxBody * dxBody + dyBody * dyBody);

                        if (distHead < headRadius)
                        {
                            // Face lighting & tone
                            double light = 1.0 - (distHead / headRadius) * 0.45;
                            bgR = (byte)Math.Clamp(230 * light + 15 * Math.Sin(x * 0.05), 0, 255);
                            bgG = (byte)Math.Clamp(190 * light, 0, 255);
                            bgB = (byte)Math.Clamp(170 * light + 20 * Math.Cos(y * 0.05), 0, 255);
                        }
                        else if (distBody < 140 && y > centerY + 20)
                        {
                            // Shirt / Garment lighting
                            bgR = (byte)(50 + 20 * Math.Sin(time));
                            bgG = (byte)(80 + 30 * Math.Cos(time));
                            bgB = (byte)(140 + 40 * Math.Sin(time * 1.2));
                        }

                        // Camera Scanline / HUD grid
                        if (y % 40 == 0 || x % 40 == 0)
                        {
                            bgB = (byte)Math.Min(255, bgB + 20);
                            bgG = (byte)Math.Min(255, bgG + 20);
                        }

                        // Write pixel BGR32 (Blue, Green, Red, Alpha=255)
                        row[x] = (uint)((255 << 24) | (bgR << 16) | (bgG << 8) | bgB);
                    }
                }
            }

            _cameraBitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
        }
        catch { }
        finally
        {
            _cameraBitmap.Unlock();
        }
    }

    public void Dispose()
    {
        _frameTimer?.Stop();
    }
}
