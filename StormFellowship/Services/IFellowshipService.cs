using System.Collections.ObjectModel;
using StormFellowship.Models;

namespace StormFellowship.Services;

public interface IFellowshipService
{
    ObservableCollection<Fellowship> Fellowships { get; }
    ObservableCollection<User> DirectMessageUsers { get; }
    Fellowship? CurrentFellowship { get; set; }
    Channel? CurrentChannel { get; set; }
    User? CurrentDmUser { get; set; }
    bool IsDirectMessagesSelected { get; set; }
    User CurrentUser { get; }

    event Action<Fellowship?>? CurrentFellowshipChanged;
    event Action<Channel?>? CurrentChannelChanged;
    event Action<User?>? CurrentDmUserChanged;

    Fellowship CreateFellowship(string name);
    Fellowship CreateFellowship(string name, string description, string iconUrl = "");
    Fellowship? JoinFellowship(string inviteCode);
    void DeleteFellowship(string fellowshipId);
    void RenameFellowship(string fellowshipId, string newName, string newDescription);
    Channel AddChannel(string fellowshipId, string? categoryId, string name, string topic, ChannelType type, int bitrateKbps = 128);
    Channel AddChannel(string fellowshipId, string? categoryId, string name, ChannelType type, int bitrateKbps = 128);
    void UpdateChannel(string fellowshipId, string channelId, string newName, string newTopic, int bitrateKbps);
    void DeleteChannel(string fellowshipId, string channelId);
    void AddCategory(string fellowshipId, string name);
    void SelectFellowship(Fellowship? fellowship);
    void SelectChannel(Channel? channel);
    void SelectDirectMessage(User? user);
}
