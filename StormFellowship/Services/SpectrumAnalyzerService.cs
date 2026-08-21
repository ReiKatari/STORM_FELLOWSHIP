using System.Timers;

namespace StormFellowship.Services;

public class SpectrumAnalyzerService
{
    private static SpectrumAnalyzerService? _instance;
    public static SpectrumAnalyzerService Instance => _instance ??= new SpectrumAnalyzerService();

    private readonly System.Timers.Timer _timer;
    private readonly Random _random = new();
    private readonly double[] _currentBands = new double[32];
    private readonly double[] _targetBands = new double[32];

    public const int BandCount = 32;

    public event Action<double[]>? SpectrumUpdated;

    public SpectrumAnalyzerService()
    {
        for (int i = 0; i < BandCount; i++)
        {
            _currentBands[i] = 4.0;
            _targetBands[i] = 4.0;
        }

        _timer = new System.Timers.Timer(33); // ~30 FPS
        _timer.Elapsed += OnTimerElapsed;
        _timer.Start();
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        bool isSpeaking = AudioService.Instance.IsSpeaking;
        double levelRatio = AudioService.Instance.CurrentMicLevel / 100.0;

        // Generate target amplitudes with frequency weighting
        for (int i = 0; i < BandCount; i++)
        {
            if (isSpeaking && levelRatio > 0.05)
            {
                // Acoustic human voice curve: peaks around bands 4-16 (150Hz - 3kHz)
                double voiceWeight = Math.Sin((double)i / BandCount * Math.PI);
                double baseAmp = voiceWeight * 45.0 * levelRatio;
                _targetBands[i] = Math.Clamp(baseAmp + 2.0, 2.0, 52.0);
            }
            else
            {
                _targetBands[i] = 2.0;
            }

            // Smooth physics interpolation (Fast attack, smooth decay)
            if (_targetBands[i] > _currentBands[i])
            {
                _currentBands[i] += (_targetBands[i] - _currentBands[i]) * 0.50; // Fast rise
            }
            else
            {
                _currentBands[i] -= (_currentBands[i] - _targetBands[i]) * 0.25; // Smooth fall-off
            }

            _currentBands[i] = Math.Max(2.0, _currentBands[i]);
        }

        var result = (double[])_currentBands.Clone();
        SpectrumUpdated?.Invoke(result);
    }
}
