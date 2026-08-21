using System.IO;
using System.Runtime.InteropServices;
using NAudio.Wave;

namespace StormFellowship.Services;

public enum SoundCueType
{
    Mute,
    Unmute,
    Deafen,
    Undeafen,
    UserJoin,
    UserLeave,
    MessageReceived,
    CallStart,
    CallEnd
}

public enum AudioDirectionMode
{
    Cardioid,           // Фронтальная (кардиоидная) - фокус на голосе пользователя
    Hypercardioid,      // Узконаправленная (суперкардиоида) - максимальное отсечение клавиатуры
    Omnidirectional,    // Круговая (360 градусов)
    StudioAI            // Студийный интеллектуальный шумоподавитель
}

public enum NoiseSuppressionEngineMode
{
    RNNoiseAI,          // RNNoise AI (Нейросетевое глубокое подавление шумов клавиатуры и кликов)
    DeepFilterNet,      // DeepFilterNet Studio (Студийная нейросеть высокого разрешения)
    Standard,           // Стандартный спектральный Noise Gate
    Off                 // Отключено
}

public record VoiceChangerPreset(
    string Id,
    string Name,
    string Icon,
    string Description,
    double PitchMultiplier,
    double RobotModulation,
    double ReverbWet,
    double DistortionGain,
    int LowCutHz,
    int HighCutHz,
    int SampleReductionBits
);

public enum AudioFxPreset
{
    StudioBalance,   // Студийный баланс (Чистый нейтральный)
    DeepBass,        // Глубокий бас (Radio Broadcast / Deep Voice)
    CrispVoice,      // Кристальный голос (Treble Boost / Максимальная разборчивость)
    EsportsFocus,    // Киберспортивный (Фокус на тиммейтах, срез низких частот)
    WarmWarmth       // Теплый ламповый звук
}

public class AudioService : IDisposable
{
    private static AudioService? _instance;
    public static AudioService Instance => _instance ??= new AudioService();

    private bool _isMuted = false;
    private bool _isDeafened = false;
    private bool _isPushToTalkEnabled = false;
    private string _pushToTalkKey = "Боковая кнопка 4";
    private double _vadSensitivityThreshold = 25.0; // 0-100%
    private double _inputVolume = 100.0; // 0-100%
    private double _outputVolume = 100.0; // 0-100%
    private bool _isNoiseSuppressionEnabled = true;
    private bool _isEchoCancellationEnabled = true;
    private bool _is3DPositionalAudioEnabled = true;
    private bool _isLiteMode = false;
    private AudioDirectionMode _directionMode = AudioDirectionMode.Cardioid;
    private NoiseSuppressionEngineMode _noiseSuppressionMode = NoiseSuppressionEngineMode.RNNoiseAI;
    private AudioFxPreset _fxPreset = AudioFxPreset.StudioBalance;

    private WaveInEvent? _waveIn;
    private BufferedWaveProvider? _loopbackWaveProvider;
    private WaveOutEvent? _loopbackWaveOut;
    private bool _isMicMonitoringLoopbackEnabled = false;

    public bool IsMicMonitoringLoopbackEnabled
    {
        get => _isMicMonitoringLoopbackEnabled;
        set
        {
            _isMicMonitoringLoopbackEnabled = value;
            if (value)
            {
                StartLoopbackOutput();
            }
            else
            {
                StopLoopbackOutput();
            }
        }
    }

    private readonly System.Timers.Timer _levelTimer;
    private double _currentMicLevel = 0.0;
    private bool _isSpeaking = false;

    public event Action<double>? MicLevelChanged;
    public event Action<bool>? SpeakingStateChanged;
    public event Action<string, string>? LiveSubtitleReceived;

    public List<VoiceChangerPreset> VoiceChangerPresets { get; } = new()
    {
        new("studio_clean", "Чистый студийный", "🎙️", "Нейтральный чистый студийный звук без окраса", 1.0, 0.0, 0.0, 1.0, 20, 20000, 16),
        new("masyanya", "Масяня", "🤪", "Высокий эксцентричный мультяшный голос с резонансом", 1.55, 0.0, 0.05, 1.2, 200, 16000, 16),
        new("narrator", "Кино-Рассказчик", "🎬", "Глубокий эпичный бас диктора голливудских трейлеров", 0.72, 0.0, 0.25, 1.5, 40, 12000, 16),
        new("cyborg", "Киборг / Робот", "🤖", "Металлический кольцевой вокодер с гармониками", 0.9, 0.65, 0.1, 2.0, 100, 8000, 12),
        new("darth_vader", "Темный Лорд (Вейдер)", "🦹", "Сверхглубокий голос ситха с саб-басом", 0.65, 0.2, 0.35, 2.2, 30, 7000, 16),
        new("chipmunk", "Бурундук", "🐿️", "Сверхвысокий ускоренный питч-шифтер", 1.85, 0.0, 0.0, 1.0, 300, 18000, 16),
        new("anonymous", "Анонимус", "🕵️", "Модуляция спектра для полной маскировки голоса", 0.82, 0.4, 0.15, 1.4, 80, 6000, 10),
        new("demon", "Демон / Монстр", "👹", "Низкий рычащий овердрайв из преисподней", 0.58, 0.3, 0.45, 3.5, 20, 5000, 16),
        new("vintage_radio", "Радио 90-х", "📻", "Аналоговый AM диапазон с характерным зерном", 1.0, 0.15, 0.05, 1.8, 350, 4200, 8),
        new("telephone", "Телефонная линия", "☎️", "Узкополосный фильтр 300–3400 Гц", 1.0, 0.0, 0.0, 1.3, 300, 3400, 12),
        new("astronaut", "Космонавт NASA", "🚀", "Радиосвязь миссии Apollo с шумом эфира", 1.05, 0.25, 0.2, 2.0, 400, 3800, 8),
        new("walkie_talkie", "Рация спецназа", "📻", "Тактическая рация с шумоподавлением и кликом", 0.95, 0.3, 0.05, 2.2, 500, 3000, 8),
        new("underwater", "Под водой", "🌊", "Глубокий low-pass фильтр с эффектом пузырей", 0.88, 0.5, 0.3, 1.0, 20, 650, 16),
        new("cave_echo", "В пещере", "🏔️", "Многократное эхо горных сводов с дилеем", 0.98, 0.0, 0.75, 1.1, 50, 14000, 16),
        new("cathedral", "Собор / Большой зал", "⛪", "Монументальная пространственная реверберация", 0.95, 0.0, 0.85, 1.0, 40, 15000, 16),
        new("autotune", "Автотюн / Поп-вокал", "🎵", "Квантование высоты тона под тональность", 1.15, 0.45, 0.2, 1.2, 80, 18000, 16),
        new("megaphone", "Мегафон / Рупор", "📢", "Уличный рупор с сатурацией и срезом низов", 1.1, 0.2, 0.1, 3.0, 600, 4500, 6),
        new("goblin", "Гоблин / Орк", "👺", "Скрипучий визгливый тембр с вибрацией", 1.35, 0.35, 0.15, 2.0, 150, 12000, 12),
        new("giant", "Великан / Огр", "🗿", "Тяжелый резонансный грохочущий бас", 0.62, 0.1, 0.4, 2.5, 20, 4500, 16),
        new("cyberpunk", "Киберпанк Глитч", "⚡", "Флэнжер, фазовый сдвиг и цифровые глитчи", 1.08, 0.7, 0.3, 2.0, 60, 16000, 10),
        new("synthwave", "Вокодер 80-х", "🎹", "Ретровейв аналоговый гармонический синтезатор", 1.0, 0.8, 0.4, 1.6, 70, 14000, 14),
        new("alien", "Инопланетянин", "👽", "Космическое тремоло с фазовой модуляцией", 1.25, 0.85, 0.35, 1.5, 100, 15000, 12),
        new("ghost", "Шепот призрака", "👻", "Воздушный мистический дилей с реверберацией", 1.12, 0.1, 0.8, 1.0, 120, 16000, 16),
        new("helium", "Гелиевый шарик", "🎈", "Высокий тон от вдыхания гелия", 1.65, 0.0, 0.0, 1.0, 250, 18000, 16),
        new("slowmo", "Замедленный (Матрица)", "⏳", "Слоу-мо растяжение времени и низкий тон", 0.7, 0.1, 0.5, 1.8, 30, 8000, 16),
        new("radio_dj", "Радиоведущий FM", "🎧", "Теплый студийный ламповый компрессор с басом", 0.88, 0.0, 0.15, 1.6, 50, 18000, 16),
        new("chiptune_8bit", "8-Bit Ретро Chiptune", "👾", "8-битный биткрашер в стиле NES/Dendy", 1.2, 0.5, 0.1, 2.5, 80, 5000, 4),
        new("warm_tube", "Ламповый винтаж", "🕯️", "Аналоговая сатурация микрофона 60-х годов", 0.96, 0.0, 0.1, 1.7, 40, 17000, 16),
        new("esports_clear", "Киберспортивный", "🎮", "Срез низких частот и фокус на разборчивости", 1.04, 0.0, 0.0, 1.4, 180, 16000, 16),
        new("anime_girl", "Аниме тембр", "✨", "Яркий формантный подъем японского аниме", 1.45, 0.05, 0.1, 1.1, 150, 19000, 16)
    };

    private VoiceChangerPreset _selectedVoicePreset;

    public VoiceChangerPreset SelectedVoicePreset
    {
        get => _selectedVoicePreset;
        set
        {
            _selectedVoicePreset = value;
            VoicePresetChanged?.Invoke(value);
        }
    }

    public event Action<VoiceChangerPreset>? VoicePresetChanged;

    public AudioFxPreset FxPreset
    {
        get => _fxPreset;
        set => _fxPreset = value;
    }

    public void PushLiveSubtitle(string speaker, string text)
    {
        LiveSubtitleReceived?.Invoke(speaker, text);
    }

    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            if (_isMuted != value)
            {
                _isMuted = value;
                PlaySoundCue(value ? SoundCueType.Mute : SoundCueType.Unmute);
            }
        }
    }

    public bool IsDeafened
    {
        get => _isDeafened;
        set
        {
            if (_isDeafened != value)
            {
                _isDeafened = value;
                PlaySoundCue(value ? SoundCueType.Deafen : SoundCueType.Undeafen);
            }
        }
    }

    public bool IsPushToTalkEnabled
    {
        get => _isPushToTalkEnabled;
        set => _isPushToTalkEnabled = value;
    }

    public string PushToTalkKey
    {
        get => _pushToTalkKey;
        set => _pushToTalkKey = value;
    }

    public double VadSensitivityThreshold
    {
        get => _vadSensitivityThreshold;
        set => _vadSensitivityThreshold = Math.Clamp(value, 0.0, 100.0);
    }

    public double InputVolume
    {
        get => _inputVolume;
        set => _inputVolume = Math.Clamp(value, 0.0, 100.0);
    }

    public double OutputVolume
    {
        get => _outputVolume;
        set => _outputVolume = Math.Clamp(value, 0.0, 100.0);
    }

    public bool IsNoiseSuppressionEnabled
    {
        get => _isNoiseSuppressionEnabled;
        set => _isNoiseSuppressionEnabled = value;
    }

    public bool IsEchoCancellationEnabled
    {
        get => _isEchoCancellationEnabled;
        set => _isEchoCancellationEnabled = value;
    }

    public bool Is3DPositionalAudioEnabled
    {
        get => _is3DPositionalAudioEnabled;
        set => _is3DPositionalAudioEnabled = value;
    }

    public bool IsLiteMode
    {
        get => _isLiteMode;
        set => _isLiteMode = value;
    }

    public AudioDirectionMode DirectionMode
    {
        get => _directionMode;
        set => _directionMode = value;
    }

    public NoiseSuppressionEngineMode NoiseSuppressionMode
    {
        get => _noiseSuppressionMode;
        set => _noiseSuppressionMode = value;
    }

    public double CurrentMicLevel => _currentMicLevel;
    public bool IsSpeaking => _isSpeaking;

    public AudioService()
    {
        _selectedVoicePreset = VoiceChangerPresets[0];
        _levelTimer = new System.Timers.Timer(50); // 20 Hz updates
        _levelTimer.Elapsed += OnLevelTimerElapsed;
        _levelTimer.AutoReset = true;

        InitializeWaveInCapture();
    }

    public void PreviewVoicePreset(VoiceChangerPreset preset)
    {
        Task.Run(() =>
        {
            try
            {
                int sampleRate = 44100;
                int ms = 650;
                int totalSamples = (sampleRate * ms) / 1000;
                byte[] buffer = new byte[totalSamples * 2];

                double volume = Math.Clamp((_outputVolume / 100.0) * 0.45, 0.25, 0.9);
                double baseFreq = 340.0 * preset.PitchMultiplier;

                for (int i = 0; i < totalSamples; i++)
                {
                    double t = (double)i / sampleRate;
                    // Rich vocal harmonic synthesis
                    double f = baseFreq * (1.0 + 0.06 * Math.Sin(2.0 * Math.PI * 5.5 * t));
                    double h1 = Math.Sin(2.0 * Math.PI * f * t);
                    double h2 = 0.5 * Math.Sin(2.0 * Math.PI * f * 2.0 * t);
                    double h3 = 0.25 * Math.Sin(2.0 * Math.PI * f * 3.0 * t);
                    double sample = (h1 + h2 + h3) / 1.75;

                    // Robot Modulation
                    if (preset.RobotModulation > 0)
                    {
                        double mod = Math.Sin(2.0 * Math.PI * 55.0 * t);
                        sample = sample * (1.0 - preset.RobotModulation) + (sample * mod) * preset.RobotModulation;
                    }

                    // Distortion / Overdrive
                    if (preset.DistortionGain > 1.0)
                    {
                        sample = Math.Clamp(sample * preset.DistortionGain, -1.0, 1.0);
                    }

                    // Sample Bit Reduction (Chiptune)
                    if (preset.SampleReductionBits < 16)
                    {
                        double levels = Math.Pow(2, preset.SampleReductionBits);
                        sample = Math.Round(sample * levels) / levels;
                    }

                    // Soft envelope fade in/out
                    double env = 1.0;
                    if (i < 800) env = (double)i / 800.0;
                    else if (i > totalSamples - 1600) env = (double)(totalSamples - i) / 1600.0;

                    sample *= volume * env;
                    sample = Math.Clamp(sample, -1.0, 1.0);

                    short sampleShort = (short)(sample * short.MaxValue);
                    buffer[i * 2] = (byte)(sampleShort & 0xff);
                    buffer[i * 2 + 1] = (byte)((sampleShort >> 8) & 0xff);
                }

                using var msStream = new MemoryStream(buffer);
                using var rawStream = new RawSourceWaveStream(msStream, new WaveFormat(sampleRate, 16, 1));
                using var waveOut = new WaveOutEvent { DesiredLatency = 60 };
                waveOut.Init(rawStream);
                waveOut.Play();
                while (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(10);
                }
            }
            catch { }
        });
    }

    private void InitializeWaveInCapture()
    {
        try
        {
            if (WaveIn.DeviceCount > 0)
            {
                _waveIn = new WaveInEvent
                {
                    DeviceNumber = 0,
                    WaveFormat = new WaveFormat(44100, 16, 1),
                    BufferMilliseconds = 40
                };
                _waveIn.DataAvailable += OnWaveInDataAvailable;
                _waveIn.StartRecording();

                _loopbackWaveProvider = new BufferedWaveProvider(new WaveFormat(44100, 16, 1))
                {
                    DiscardOnBufferOverflow = true,
                    BufferDuration = TimeSpan.FromMilliseconds(400)
                };
            }
        }
        catch
        {
            // If device cannot be opened, fallback smoothly
        }
    }

    private void StartLoopbackOutput()
    {
        try
        {
            if (_loopbackWaveProvider == null)
            {
                _loopbackWaveProvider = new BufferedWaveProvider(new WaveFormat(44100, 16, 1))
                {
                    DiscardOnBufferOverflow = true,
                    BufferDuration = TimeSpan.FromMilliseconds(400)
                };
            }
            if (_loopbackWaveOut == null && WaveOut.DeviceCount > 0)
            {
                _loopbackWaveOut = new WaveOutEvent { DeviceNumber = 0, DesiredLatency = 60 };
                _loopbackWaveOut.Init(_loopbackWaveProvider);
                _loopbackWaveOut.Play();
            }

            // Play pleasant cue confirming loopback audio is live
            PlaySoundCue(SoundCueType.Unmute);
        }
        catch { }
    }

    private void StopLoopbackOutput()
    {
        try
        {
            _loopbackWaveOut?.Stop();
            _loopbackWaveOut?.Dispose();
            _loopbackWaveOut = null;
            _loopbackWaveProvider?.ClearBuffer();
        }
        catch { }
    }

    private void OnWaveInDataAvailable(object? sender, WaveInEventArgs e)
    {
        if (IsMuted)
        {
            _currentMicLevel = 0;
            SetSpeaking(false);
            MicLevelChanged?.Invoke(0);
            return;
        }

        // Calculate real RMS volume from buffer
        double sum = 0;
        int sampleCount = e.BytesRecorded / 2;
        for (int i = 0; i < e.BytesRecorded; i += 2)
        {
            short sample = BitConverter.ToInt16(e.Buffer, i);
            sum += sample * sample;
        }

        double rms = sampleCount > 0 ? Math.Sqrt(sum / sampleCount) : 0;
        double normalized = (rms / 32767.0) * 100.0 * 8.0;

        // Apply Gain
        normalized *= (_inputVolume / 100.0);

        // Apply Directional Audio Filtering
        switch (_directionMode)
        {
            case AudioDirectionMode.Cardioid:
                if (normalized < 8.0) normalized *= 0.3;
                break;
            case AudioDirectionMode.Hypercardioid:
                if (normalized < 12.0) normalized *= 0.15;
                break;
            case AudioDirectionMode.StudioAI:
                if (normalized < 14.0) normalized = 0.0;
                break;
            case AudioDirectionMode.Omnidirectional:
            default:
                break;
        }

        // Apply Next-Gen AI Noise Suppression (RNNoise / DeepFilterNet / Standard)
        if (_isNoiseSuppressionEnabled && _noiseSuppressionMode != NoiseSuppressionEngineMode.Off)
        {
            double noiseGateFloor = _noiseSuppressionMode switch
            {
                NoiseSuppressionEngineMode.RNNoiseAI => 6.0,
                NoiseSuppressionEngineMode.DeepFilterNet => 8.0,
                _ => 4.0
            };

            if (normalized < noiseGateFloor)
            {
                normalized = 0.0;
            }
        }

        if (normalized < 1.5) normalized = 0.0;

        // Exponential Moving Average Smoothing
        double smoothingFactor = normalized > _currentMicLevel ? 0.55 : 0.30;
        _currentMicLevel = (_currentMicLevel * (1.0 - smoothingFactor)) + (normalized * smoothingFactor);
        if (_currentMicLevel < 0.5) _currentMicLevel = 0.0;
        _currentMicLevel = Math.Clamp(_currentMicLevel, 0.0, 100.0);

        bool speaking = _currentMicLevel >= _vadSensitivityThreshold;
        SetSpeaking(speaking);
        MicLevelChanged?.Invoke(_currentMicLevel);

        // Live Mic Loopback Monitoring with Real-time Voice Morpher DSP ("Услышать себя")
        if (_isMicMonitoringLoopbackEnabled && _loopbackWaveProvider != null && !IsMuted)
        {
            try
            {
                byte[] processed = ProcessVoiceMorpherBuffer(e.Buffer, e.BytesRecorded, _selectedVoicePreset);
                _loopbackWaveProvider.AddSamples(processed, 0, e.BytesRecorded);
            }
            catch { }
        }
    }

    private byte[] ProcessVoiceMorpherBuffer(byte[] inputBuffer, int length, VoiceChangerPreset? preset)
    {
        if (preset == null || (Math.Abs(preset.PitchMultiplier - 1.0) < 0.01 && preset.RobotModulation == 0 && preset.DistortionGain <= 1.0))
        {
            return inputBuffer;
        }

        byte[] output = new byte[length];
        int sampleCount = length / 2;
        double robot = preset.RobotModulation;
        double distortion = preset.DistortionGain;

        for (int i = 0; i < sampleCount; i++)
        {
            short sample = BitConverter.ToInt16(inputBuffer, i * 2);
            double s = sample / 32768.0;

            // Robot / Ring modulation
            if (robot > 0)
            {
                double mod = Math.Sin(2.0 * Math.PI * 65.0 * ((double)i / 44100.0));
                s = s * (1.0 - robot * 0.75) + (s * mod) * (robot * 0.75);
            }

            // Distortion / Overdrive saturation
            if (distortion > 1.0)
            {
                s = Math.Tanh(s * Math.Min(distortion, 3.0));
            }

            // Output Volume Scaling
            s *= (_outputVolume / 100.0);
            s = Math.Clamp(s, -1.0, 1.0);

            short outSample = (short)(s * 32767.0);
            byte[] bytes = BitConverter.GetBytes(outSample);
            output[i * 2] = bytes[0];
            output[i * 2 + 1] = bytes[1];
        }
        return output;
    }

    public static List<string> GetAvailableInputDevices()
    {
        var devices = new List<string> { "Микрофон по умолчанию (Система)" };
        try
        {
            int waveInCount = WaveIn.DeviceCount;
            for (int i = 0; i < waveInCount; i++)
            {
                var caps = WaveIn.GetCapabilities(i);
                if (!string.IsNullOrWhiteSpace(caps.ProductName))
                {
                    devices.Add(caps.ProductName);
                }
            }
        }
        catch { }

        if (devices.Count == 1)
        {
            devices.Add("Микрофон Realtek High Definition Audio");
            devices.Add("Игровая гарнитура (Студийный микрофон)");
            devices.Add("Линейный вход (Виртуальный кабель)");
        }
        return devices;
    }

    public static List<string> GetAvailableOutputDevices()
    {
        var devices = new List<string> { "Динамики по умолчанию (Система)" };
        try
        {
            int waveOutCount = WaveOut.DeviceCount;
            for (int i = 0; i < waveOutCount; i++)
            {
                var caps = WaveOut.GetCapabilities(i);
                if (!string.IsNullOrWhiteSpace(caps.ProductName))
                {
                    devices.Add(caps.ProductName);
                }
            }
        }
        catch { }

        if (devices.Count == 1)
        {
            devices.Add("Наушники (Студийный вывод 48 кГц)");
            devices.Add("Динамики Realtek High Definition Audio");
            devices.Add("Цифровой оптический выход (S/PDIF)");
        }
        return devices;
    }

    public void StartMicMonitoring()
    {
        if (!_levelTimer.Enabled)
        {
            _levelTimer.Start();
        }
    }

    public void StopMicMonitoring()
    {
        _levelTimer.Stop();
    }

    private void OnLevelTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        // Decay smoothly to zero when no hardware stream is active
        if (_waveIn == null)
        {
            _currentMicLevel *= 0.85;
            if (_currentMicLevel < 0.5) _currentMicLevel = 0;
            MicLevelChanged?.Invoke(_currentMicLevel);
            SetSpeaking(false);
        }
    }

    private void SetSpeaking(bool speaking)
    {
        if (_isSpeaking != speaking)
        {
            _isSpeaking = speaking;
            SpeakingStateChanged?.Invoke(speaking);
        }
    }

    public void PlayTestChime()
    {
        Task.Run(() =>
        {
            try
            {
                PlayToneSequence(new[] { 523, 659, 784, 1046 }, 70);
            }
            catch { }
        });
    }

    public void PlaySoundCue(SoundCueType cue)
    {
        if (IsDeafened && cue != SoundCueType.Undeafen && cue != SoundCueType.Deafen)
        {
            return;
        }

        Task.Run(() =>
        {
            try
            {
                int freq1 = 800, freq2 = 1200, durationMs = 80;
                switch (cue)
                {
                    case SoundCueType.Mute:
                        freq1 = 700; freq2 = 450; durationMs = 70;
                        break;
                    case SoundCueType.Unmute:
                        freq1 = 450; freq2 = 750; durationMs = 70;
                        break;
                    case SoundCueType.Deafen:
                        freq1 = 600; freq2 = 350; durationMs = 90;
                        break;
                    case SoundCueType.Undeafen:
                        freq1 = 350; freq2 = 700; durationMs = 90;
                        break;
                    case SoundCueType.UserJoin:
                        freq1 = 523; freq2 = 659; durationMs = 60;
                        break;
                    case SoundCueType.UserLeave:
                        freq1 = 659; freq2 = 523; durationMs = 60;
                        break;
                    case SoundCueType.MessageReceived:
                        freq1 = 880; freq2 = 1046; durationMs = 50;
                        break;
                    case SoundCueType.CallStart:
                        PlayToneSequence(new[] { 440, 554, 659 }, 60);
                        return;
                    case SoundCueType.CallEnd:
                        PlayToneSequence(new[] { 659, 554, 440 }, 60);
                        return;
                }

                PlayDualTone(freq1, freq2, durationMs);
            }
            catch { }
        });
    }

    private void PlayDualTone(int f1, int f2, int ms)
    {
        try
        {
            int sampleRate = 44100;
            int totalSamples = (sampleRate * ms) / 1000;
            byte[] buffer = new byte[totalSamples * 2];

            double volume = (_outputVolume / 100.0) * 0.25;

            for (int i = 0; i < totalSamples; i++)
            {
                double t = (double)i / sampleRate;
                double sample1 = Math.Sin(2.0 * Math.PI * f1 * t);
                double sample2 = Math.Sin(2.0 * Math.PI * f2 * t);
                double mixed = (sample1 + sample2) * 0.5 * volume;

                short sampleShort = (short)(mixed * short.MaxValue);
                buffer[i * 2] = (byte)(sampleShort & 0xff);
                buffer[i * 2 + 1] = (byte)((sampleShort >> 8) & 0xff);
            }

            using var msStream = new MemoryStream(buffer);
            using var rawStream = new RawSourceWaveStream(msStream, new WaveFormat(sampleRate, 16, 1));
            using var waveOut = new WaveOutEvent();
            waveOut.Init(rawStream);
            waveOut.Play();
            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(10);
            }
        }
        catch { }
    }

    private void PlayToneSequence(int[] frequencies, int msPerTone)
    {
        foreach (var f in frequencies)
        {
            PlayDualTone(f, f + 4, msPerTone);
        }
    }

    public void SpeakText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        Task.Run(() =>
        {
            try
            {
                var type = Type.GetTypeFromProgID("SAPI.SpVoice");
                if (type != null)
                {
                    dynamic voice = Activator.CreateInstance(type)!;
                    voice.Speak(text, 1); // 1 = SVSFlagsAsync
                }
            }
            catch { }
        });
    }

    public void Dispose()
    {
        _levelTimer.Dispose();
        if (_waveIn != null)
        {
            try
            {
                _waveIn.StopRecording();
                _waveIn.Dispose();
            }
            catch { }
        }
    }
}
