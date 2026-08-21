using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using StormFellowship.Models;

namespace StormFellowship.Services;

public record LanPeer(string DisplayName, string IpAddress, int Port, string AvatarGlyph, DateTime LastSeen);

public class CloudSyncService : IDisposable
{
    private static CloudSyncService? _instance;
    public static CloudSyncService Instance => _instance ??= new CloudSyncService();

    private UdpClient? _udpBeacon;
    private bool _isListening = false;
    private const int DiscoveryPort = 48152;

    public ObservableCollection<LanPeer> DiscoveredPeers { get; } = new();
    public DateTime? LastSyncTime { get; private set; } = DateTime.Now;
    public bool IsSyncing { get; private set; } = false;

    public event Action<DateTime?>? SyncCompleted;

    public CloudSyncService()
    {
        StartLanDiscovery();
    }

    public async Task<bool> SyncNowAsync()
    {
        if (IsSyncing) return false;

        IsSyncing = true;
        try
        {
            // Simulate encrypted E2EE synchronization with Supabase Realtime DB
            await Task.Delay(500);

            FellowshipService.Instance.SaveUserProfile();
            LastSyncTime = DateTime.Now;
            SyncCompleted?.Invoke(LastSyncTime);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            IsSyncing = false;
        }
    }

    public void BroadcastLanPresence()
    {
        Task.Run(() =>
        {
            try
            {
                using var client = new UdpClient();
                client.EnableBroadcast = true;
                var user = FellowshipService.Instance.CurrentUser;

                var payload = new
                {
                    Type = "STORM_LAN_ANNOUNCE",
                    user.DisplayName,
                    user.AvatarGlyph,
                    Port = 48150
                };

                string json = JsonSerializer.Serialize(payload);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                var endpoint = new IPEndPoint(IPAddress.Broadcast, DiscoveryPort);
                client.Send(bytes, bytes.Length, endpoint);
            }
            catch { }
        });
    }

    public void ConnectDirectP2P(string ipAddress, int port)
    {
        if (string.IsNullOrWhiteSpace(ipAddress)) return;

        var peerUser = new User
        {
            DisplayName = $"P2P Узел ({ipAddress})",
            CustomStatus = $"Прямое соединение {ipAddress}:{port}",
            AvatarGlyph = "🌐"
        };

        CallService.Instance.StartDirectCall(peerUser);
    }

    private void StartLanDiscovery()
    {
        if (_isListening) return;
        _isListening = true;

        Task.Run(async () =>
        {
            try
            {
                _udpBeacon = new UdpClient(DiscoveryPort);
                while (_isListening)
                {
                    var result = await _udpBeacon.ReceiveAsync();
                    string json = Encoding.UTF8.GetString(result.Buffer);

                    if (json.Contains("STORM_LAN_ANNOUNCE"))
                    {
                        var doc = JsonDocument.Parse(json);
                        string name = doc.RootElement.GetProperty("DisplayName").GetString() ?? "Участник сети";
                        string avatar = doc.RootElement.GetProperty("AvatarGlyph").GetString() ?? "⚡";
                        string ip = result.RemoteEndPoint.Address.ToString();

                        App.Current?.Dispatcher.Invoke(() =>
                        {
                            var existing = DiscoveredPeers.FirstOrDefault(p => p.IpAddress == ip);
                            if (existing != null)
                            {
                                DiscoveredPeers.Remove(existing);
                            }
                            DiscoveredPeers.Add(new LanPeer(name, ip, 48150, avatar, DateTime.Now));
                        });
                    }
                }
            }
            catch { }
        });
    }

    public void Dispose()
    {
        _isListening = false;
        _udpBeacon?.Dispose();
    }
}
