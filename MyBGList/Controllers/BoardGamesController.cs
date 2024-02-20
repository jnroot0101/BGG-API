using System.Linq.Dynamic.Core;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyBGList.Constants;
using MyBGList.DTO;
using MyBGList.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MyBGList.Controllers;

[Route("[controller]")]
[ApiController]
public class BoardGamesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BoardGamesController> _logger;
    private readonly IMemoryCache _memoryCache;

    public BoardGamesController(
        ApplicationDbContext context,
        ILogger<BoardGamesController> logger,
        IMemoryCache memoryCache)
    {
        _context = context;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    [HttpGet(Name = "GetBoardGames")]
    [ResponseCache(CacheProfileName = "Any-60")]
    [SwaggerOperation(
        Summary = "Get a list of board games.",
        Description = "Retrieves a list of board games with custom paging, sorting and filtering rules.")]
    public async Task<RestDTO<BoardGame[]>> Get(
        [FromQuery] [SwaggerParameter("A DTO object that can be used to customize data-retrieval parameters.")]
        RequestDTO<BoardGameDto> input)
    {
        _logger.LogInformation(CustomLogEvents.BoardGamesController_Get, "Get method started.");

        var query = _context.BoardGames.AsQueryable();
        if (!string.IsNullOrEmpty(input.FilterQuery))
            query = query.Where(b => b.Name.Contains(input.FilterQuery));

        var recordCount = await query.CountAsync();

        BoardGame[]? result = null;
        var cacheKey = $"{input.GetType()}-{JsonSerializer.Serialize(input)}";
        if (!_memoryCache.TryGetValue<BoardGame[]>(cacheKey, out result))
        {
            query = query.OrderBy($"{input.SortColumn} {input.SortOrder}")
                .Skip(input.PageIndex * input.PageSize)
                .Take(input.PageSize);
            result = await query.ToArrayAsync();
            _memoryCache.Set(cacheKey, result, new TimeSpan(0, 0, 30));
        }

        return new RestDTO<BoardGame[]>
        {
            Data = result,
            PageIndex = input.PageIndex,
            PageSize = input.PageSize,
            RecordCount = recordCount,
            Links = new List<LinkDTO>
            {
                new(
                    Url.Action(
                        null,
                        "BoardGames",
                        new { input.PageIndex, input.PageSize },
                        Request.Scheme)!,
                    "self",
                    "GET")
            }
        };
    }

    [HttpGet("{id}")]
    [ResponseCache(CacheProfileName = "Any-60")]
    [SwaggerOperation(
        Summary = "Get a single board game.",
        Description = "Retrieves a single board game with a given Id.")]
    public async Task<RestDTO<BoardGame?>> GetBoardGame(int id)
    {
        _logger.LogInformation(CustomLogEvents.BoardGamesController_Get,
            "GetBoardGame method started.");

        BoardGame? result = null;
        var cacheKey = $"GetBoardGame-{id}";
        if (!_memoryCache.TryGetValue<BoardGame>(cacheKey, out result))
        {
            result = await _context.BoardGames
                .FirstOrDefaultAsync(bg => bg.Id == id);
            _memoryCache.Set(cacheKey, result, new TimeSpan(0, 0, 30));
        }

        return new RestDTO<BoardGame?>
        {
            Data = result,
            PageIndex = 0,
            PageSize = 1,
            RecordCount = result != null ? 1 : 0,
            Links = new List<LinkDTO>
            {
                new(
                    Url.Action(
                        null,
                        "BoardGames",
                        new { id },
                        Request.Scheme)!,
                    "self",
                    "GET")
            }
        };
    }

    [Authorize(Roles = RoleNames.Moderator)]
    [HttpPost(Name = "UpdateBoardGame")]
    [ResponseCache(CacheProfileName = "no-cache")]
    [SwaggerOperation(
        Summary = "Updates a board game.",
        Description = "Updates the board game's data.")]
    public async Task<RestDTO<BoardGame?>> Post(BoardGameDto model)
    {
        var boardgame = await _context.BoardGames
            .Where(b => b.Id == model.Id)
            .FirstOrDefaultAsync();
        if (boardgame != null)
        {
            if (!string.IsNullOrEmpty(model.Name))
                boardgame.Name = model.Name;
            if (model.Year.HasValue && model.Year.Value > 0)
                boardgame.Year = model.Year.Value;
            boardgame.LastModifiedDate = DateTime.Now;
            _context.BoardGames.Update(boardgame);
            await _context.SaveChangesAsync();
        }

        return new RestDTO<BoardGame?>
        {
            Data = boardgame,
            Links = new List<LinkDTO>
            {
                new(
                    Url.Action(
                        null,
                        "BoardGames",
                        model,
                        Request.Scheme)!,
                    "self",
                    "POST")
            }
        };
    }

    [Authorize(Roles = RoleNames.Administrator)]
    [HttpDelete(Name = "DeleteBoardGame")]
    [ResponseCache(CacheProfileName = "no-cache")]
    [SwaggerOperation(Summary = "Deletes a board game.",
        Description = "Deletes a board game from the database.")]
    public async Task<RestDTO<BoardGame?>> Delete(int id)
    {
        var boardgame = await _context.BoardGames
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();
        if (boardgame != null)
        {
            _context.BoardGames.Remove(boardgame);
            await _context.SaveChangesAsync();
        }

        return new RestDTO<BoardGame?>
        {
            Data = boardgame,
            Links = new List<LinkDTO>
            {
                new(
                    Url.Action(
                        null,
                        "BoardGames",
                        id,
                        Request.Scheme)!,
                    "self",
                    "Delete")
            }
        };
    }
}