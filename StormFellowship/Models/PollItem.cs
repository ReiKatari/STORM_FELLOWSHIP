using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace StormFellowship.Models;

public partial class PollOption : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _text = string.Empty;

    [ObservableProperty]
    private string _imageUrl = string.Empty;

    public bool HasImage => !string.IsNullOrWhiteSpace(ImageUrl);

    [ObservableProperty]
    private int _votesCount = 0;

    [ObservableProperty]
    private double _percentage = 0.0;

    [ObservableProperty]
    private bool _hasVoted = false;

    public ObservableCollection<string> VotedUserNames { get; } = new();
}

public partial class PollItem : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _question = string.Empty;

    [ObservableProperty]
    private string _questionImageUrl = string.Empty;

    public bool HasQuestionImage => !string.IsNullOrWhiteSpace(QuestionImageUrl);

    [ObservableProperty]
    private int _totalVotes = 0;

    [ObservableProperty]
    private bool _isClosed = false;

    [ObservableProperty]
    private bool _allowMultipleAnswers = false;

    [ObservableProperty]
    private bool _isAnonymous = false;

    [ObservableProperty]
    private DateTime _createdAt = DateTime.Now;

    [ObservableProperty]
    private string _authorName = string.Empty;

    public ObservableCollection<PollOption> Options { get; } = new();

    public void RecalculatePercentages()
    {
        TotalVotes = Options.Sum(o => o.VotesCount);
        foreach (var opt in Options)
        {
            opt.Percentage = TotalVotes > 0 ? ((double)opt.VotesCount / TotalVotes) * 100.0 : 0.0;
        }
    }

    [RelayCommand]
    public void Vote(PollOption option)
    {
        if (IsClosed || option == null) return;

        if (!AllowMultipleAnswers)
        {
            foreach (var opt in Options)
            {
                if (opt != option && opt.HasVoted)
                {
                    opt.HasVoted = false;
                    opt.VotesCount = Math.Max(0, opt.VotesCount - 1);
                }
            }
        }

        option.HasVoted = !option.HasVoted;
        if (option.HasVoted)
        {
            option.VotesCount++;
        }
        else
        {
            option.VotesCount = Math.Max(0, option.VotesCount - 1);
        }

        RecalculatePercentages();
    }

    [RelayCommand]
    public void ToggleClose()
    {
        IsClosed = !IsClosed;
    }
}
