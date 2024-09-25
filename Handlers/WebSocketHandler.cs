using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Backend.Models;
using Backend.Services;

namespace Backend.Handlers
{
  public class WebSocketHandler
  {
    private readonly CipherService _cipherService;

    public WebSocketHandler(CipherService cipherService)
    {
      _cipherService = cipherService;
    }

    public async Task HandleAsync(HttpContext context)
    {
      WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
      var buffer = new byte[1024 * 4];
      WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

      while (!result.CloseStatus.HasValue)
      {
        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine($"Received message: {receivedMessage}");  
        var request = JsonSerializer.Deserialize<RequestMessage>(receivedMessage);
        Console.WriteLine($"Received action: {request.Action}");  

        string responseMessage = request.Action switch
        {
          "encrypt" => _cipherService.Encrypt(request.Text, request.Key),
          "decrypt" => _cipherService.Decrypt(request.Text, request.Key),
          "hack" => _cipherService.HackCipher(request.Text),
          "generateKey" => _cipherService.GenerateKey(),
          _ => "Неверное действие"
        };

        byte[] responseBuffer = Encoding.UTF8.GetBytes(responseMessage);
        await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
      }

      await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
  }
}
