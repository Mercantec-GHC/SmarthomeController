using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class WebSocketService
{
    private static readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    public static void AddSocket(string id, WebSocket socket)
    {
        _sockets[id] = socket;
    }

    public static void RemoveSocket(string id)
    {
        _sockets.TryRemove(id, out _);
    }

    public static async Task SendMessageToAllAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(buffer);

        foreach (var socket in _sockets.Values)
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}