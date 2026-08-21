using System.Collections.ObjectModel;
using StormFellowship.Models;

namespace StormFellowship.Services;

public class SearchService
{
    private static SearchService? _instance;
    public static SearchService Instance => _instance ??= new SearchService();

    public ObservableCollection<ChatMessage> SearchMessages(string query, Fellowship? currentFellowship)
    {
        var results = new ObservableCollection<ChatMessage>();
        if (string.IsNullOrWhiteSpace(query) || currentFellowship == null) return results;

        string q = query.Trim().ToLowerInvariant();

        foreach (var cat in currentFellowship.Categories)
        {
            foreach (var chan in cat.Channels)
            {
                foreach (var msg in chan.Messages)
                {
                    if (msg.Content.ToLowerInvariant().Contains(q) ||
                        msg.Author.DisplayName.ToLowerInvariant().Contains(q) ||
                        msg.TranscriptionText.ToLowerInvariant().Contains(q))
                    {
                        results.Add(msg);
                    }
                }
            }
        }

        return results;
    }
}
