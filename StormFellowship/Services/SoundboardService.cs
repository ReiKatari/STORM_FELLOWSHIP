using System.Collections.ObjectModel;

namespace StormFellowship.Services;

public record SoundboardTrack(string Id, string Title, string Icon, string Category, double DurationSec);

public class SoundboardService
{
    private static SoundboardService? _instance;
    public static SoundboardService Instance => _instance ??= new SoundboardService();

    public ObservableCollection<SoundboardTrack> Tracks { get; } = new()
    {
        new("airhorn", "Airhorn Blast 📢", "📢", "Мемы и Звуки", 1.8),
        new("gg", "GG WP Victory 🏆", "🏆", "Игры", 2.2),
        new("applause", "Аплодисменты 👏", "👏", "Реакции", 3.5),
        new("thunder", "STORM Гром ⚡", "⚡", "STORM FX", 2.8),
        new("drumroll", "Барабанная дробь 🥁", "🥁", "Реакции", 3.0),
        new("laser", "Cyber Laser 🔫", "🔫", "Игры", 1.2),
        new("badumtss", "Ba-Dum-Tss 🥁", "🥁", "Мемы и Звуки", 1.5),
        new("victory", "Победный фанфар 🎺", "🎺", "STORM FX", 4.0),
        new("zap", "Электро-разряд ⚡", "⚡", "STORM FX", 1.0),
        new("laugh", "Командный смех 😂", "😂", "Реакции", 2.5)
    };

    public event Action<SoundboardTrack>? TrackPlayed;

    public void PlayTrack(SoundboardTrack track)
    {
        if (track == null) return;
        AudioService.Instance.PlaySoundCue(SoundCueType.UserJoin);
        TrackPlayed?.Invoke(track);
    }
}
