using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using NAudio.Wave;

namespace StormFellowship.Services;

public record SoundboardTrack(string Id, string Title, string IconGeo, string Category, double DurationSec, string ColorHex = "#00D2FF");

public class SoundboardService
{
    private static SoundboardService? _instance;
    public static SoundboardService Instance => _instance ??= new SoundboardService();

    public ObservableCollection<SoundboardTrack> Tracks { get; } = new()
    {
        // 1. STORM FX
        new("thunder", "STORM Гром", "GeoLightning", "STORM FX", 2.8, "#00D2FF"),
        new("zap", "Электро-разряд", "GeoLightning", "STORM FX", 1.0, "#06B6D4"),
        new("boom", "Cyber Impact", "GeoFire", "STORM FX", 2.0, "#EF4444"),
        new("laser", "Neon Laser", "GeoCrosshair", "STORM FX", 1.2, "#E11D48"),
        new("glitch", "Cyber Glitch", "GeoServices", "STORM FX", 1.4, "#A855F7"),
        new("bassdrop", "Bass Drop Sub", "GeoSpeaker", "STORM FX", 2.5, "#6366F1"),

        // 2. Игры
        new("gg", "GG WP Victory", "GeoTrophy", "Игры", 2.2, "#10B981"),
        new("levelup", "Level Up 1-Up", "GeoStar", "Игры", 1.6, "#34D399"),
        new("coin", "Gold Coin Pickup", "GeoCrown", "Игры", 0.9, "#FBBF24"),
        new("sword", "Blade Clash", "GeoSwords", "Игры", 1.1, "#38BDF8"),
        new("gameover", "Game Over Fail", "GeoShield", "Игры", 2.0, "#F43F5E"),
        new("nuke", "Nuclear Alert", "GeoTarget", "Игры", 2.5, "#FB923C"),

        // 3. Мемы & Звуки
        new("airhorn", "Airhorn Blast", "GeoSoundboard", "Мемы", 1.8, "#F59E0B"),
        new("badumtss", "Ba-Dum-Tss!", "GeoPopcorn", "Мемы", 1.5, "#F97316"),
        new("fanfare", "Victory Fanfare", "GeoCrown", "Мемы", 3.2, "#EAB308"),
        new("robot", "Robot Bleep", "GeoBot", "Мемы", 1.3, "#00D2FF"),
        new("pizza", "Snack Time", "GeoPizza", "Мемы", 1.2, "#FB923C"),
        new("cheer", "Crowd Cheer", "GeoUsers", "Мемы", 2.8, "#10B981"),

        // 4. Реакции
        new("applause", "Аплодисменты", "GeoUsers", "Реакции", 3.0, "#00D2FF"),
        new("magic", "Магия / Sparkle", "GeoDiamond", "Реакции", 2.4, "#C084FC"),
        new("chime", "Динь-динь Chime", "GeoSmile", "Реакции", 1.5, "#38BDF8"),
        new("heartbeat", "Pulse Heartbeat", "GeoShield", "Реакции", 2.0, "#EC4899"),
        new("rocket", "Rocket Launch", "GeoRocket", "Реакции", 2.2, "#0284C7"),
        new("alert", "Direct Ping", "GeoCommand", "Реакции", 1.0, "#F59E0B")
    };

    public event Action<SoundboardTrack>? TrackPlayed;

    public void PlayTrack(SoundboardTrack track)
    {
        if (track == null) return;
        TrackPlayed?.Invoke(track);

        Task.Run(() =>
        {
            try
            {
                int sampleRate = 44100;
                int ms = (int)(track.DurationSec * 1000);
                if (ms > 3500) ms = 3500;
                int totalSamples = (sampleRate * ms) / 1000;
                byte[] buffer = new byte[totalSamples * 2];
                var random = new Random();

                for (int i = 0; i < totalSamples; i++)
                {
                    double t = (double)i / sampleRate;
                    double sample = 0.0;

                    switch (track.Id)
                    {
                        case "airhorn":
                            double tRepeat = t % 0.45;
                            if (tRepeat < 0.35)
                            {
                                double h1 = Math.Sin(2.0 * Math.PI * 466.16 * tRepeat);
                                double h2 = Math.Sin(2.0 * Math.PI * 622.25 * tRepeat);
                                double h3 = Math.Sin(2.0 * Math.PI * 932.33 * tRepeat);
                                sample = (h1 + h2 * 0.8 + h3 * 0.6) * 0.35;
                            }
                            break;

                        case "gg":
                        case "fanfare":
                            double seg = t / (track.DurationSec / 4.0);
                            double f = seg switch
                            {
                                < 1.0 => 523.25,
                                < 2.0 => 659.25,
                                < 3.0 => 783.99,
                                _ => 1046.50
                            };
                            sample = Math.Sin(2.0 * Math.PI * f * t) * 0.4;
                            break;

                        case "applause":
                        case "cheer":
                            double env = Math.Exp(-0.6 * t);
                            double noise = (random.NextDouble() * 2.0 - 1.0);
                            double pulse = (0.5 + 0.5 * Math.Sin(2.0 * Math.PI * 12.0 * t));
                            sample = noise * pulse * env * 0.35;
                            break;

                        case "thunder":
                        case "boom":
                        case "bassdrop":
                            double snap = (t < 0.1) ? (random.NextDouble() * 2.0 - 1.0) * 0.8 : 0.0;
                            double rumble = Math.Sin(2.0 * Math.PI * (60.0 - t * 10.0) * t) * Math.Exp(-0.7 * t);
                            sample = (snap + rumble) * 0.5;
                            break;

                        case "laser":
                        case "glitch":
                            double lf = Math.Max(200.0, 2600.0 * Math.Exp(-6.0 * t));
                            sample = Math.Sin(2.0 * Math.PI * lf * t) * Math.Exp(-2.5 * t) * 0.45;
                            break;

                        case "levelup":
                        case "coin":
                            double cf = (t < 0.12) ? 987.77 : 1318.51;
                            sample = Math.Sin(2.0 * Math.PI * cf * t) * Math.Exp(-1.5 * t) * 0.4;
                            break;

                        default:
                            sample = Math.Sin(2.0 * Math.PI * (440.0 + Math.Sin(20.0 * t) * 120.0) * t) * Math.Exp(-1.2 * t) * 0.4;
                            break;
                    }

                    short intSample = (short)Math.Clamp((int)(sample * 32767.0), -32767, 32767);
                    buffer[i * 2] = (byte)(intSample & 0xFF);
                    buffer[i * 2 + 1] = (byte)((intSample >> 8) & 0xFF);
                }

                using var msStream = new MemoryStream(buffer);
                using var rawStream = new RawSourceWaveStream(msStream, new WaveFormat(sampleRate, 16, 1));
                using var waveOut = new WaveOutEvent();
                waveOut.Init(rawStream);
                waveOut.Play();
                while (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(50);
                }
            }
            catch { }
        });
    }
}
