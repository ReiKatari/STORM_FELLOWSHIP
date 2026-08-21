namespace StormFellowship.Services;

public class WhisperTranscriptionService
{
    private static WhisperTranscriptionService? _instance;
    public static WhisperTranscriptionService Instance => _instance ??= new WhisperTranscriptionService();

    private readonly string[] _sampleTranscriptions = new[]
    {
        "[00:01] Всем привет! Подключайтесь в голосовой канал, сейчас начинаем сбор команды.",
        "[00:02] Принято, выхожу на точку. Готовьте тактическую карту к разбору.",
        "[00:01] Звук отличный, никаких задержек! Запустил трансляцию в 1080p 60 FPS.",
        "[00:03] Скиньте файл с настройками в текстовый чат, я закреплю в канале.",
        "[00:02] Всем хорошей игры и отличной связи в STORM FELLOWSHIP!"
    };

    public async Task<string> TranscribeAudioAsync(string audioPathOrId, int durationSec = 5)
    {
        // Simulate high-speed local AI inference (Whisper Turbo engine)
        await Task.Delay(800);
        int idx = Math.Abs(audioPathOrId.GetHashCode()) % _sampleTranscriptions.Length;
        return _sampleTranscriptions[idx];
    }
}
