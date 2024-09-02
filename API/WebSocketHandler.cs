using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public static class WebSocketHandler
{
    public static async Task HandleWebSocket(HttpContext context)
    {
        var id = Guid.NewGuid().ToString();
        using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
        {
            WebSocketService.AddSocket(id, webSocket);
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    // Optionally handle incoming messages
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the server", CancellationToken.None);
                }

            } while (!result.CloseStatus.HasValue);

            WebSocketService.RemoveSocket(id);
        }
    }
}