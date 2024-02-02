using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.DTO;
using MyBGList.Models;

namespace MyBGList.Controllers;

[Route("[controller]")]
[ApiController]
public class BoardGamesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BoardGamesController> _logger;

    public BoardGamesController(
        ApplicationDbContext context,
        ILogger<BoardGamesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet(Name = "GetBoardGames")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<RestDTO<BoardGame[]>> Get()
    {
        var query = _context.BoardGames;

        return new RestDTO<BoardGame[]>
        {
            Data = await query.ToArrayAsync(),
            Links = new List<LinkDTO>
            {
                new(
                    Url.Action(null, "BoardGames", null, Request.Scheme)!,
                    "self",
                    "GET")
            }
        };
    }
}