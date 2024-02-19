using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using MyBGList.gRPC;

namespace MyBGList.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class GrpcController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<BoardGameResponse> GetBoardGame(int id)
    {
        using var channel = GrpcChannel
            .ForAddress("https://localhost:40443");
        var client = new gRPC.Grpc.GrpcClient(channel);
        var response = await client.GetBoardGameAsync(
            new BoardGameRequest { Id = id });
        return response;
    }
}