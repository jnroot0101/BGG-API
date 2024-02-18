using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.Constants;
using MyBGList.DTO;
using MyBGList.Models;

namespace MyBGList.Controllers;

[Route("[controller]")]
[ApiController]
public class MechanicsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MechanicsController> _logger;

    public MechanicsController(
        ApplicationDbContext context,
        ILogger<MechanicsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet(Name = "GetMechanics")]
    [ResponseCache(CacheProfileName = "Any-60")]
    public async Task<RestDTO<Mechanic[]>> Get(
        [FromQuery] RequestDTO<MechanicDTO> input)
    {
        var query = _context.Mechanics.AsQueryable();
        if (!string.IsNullOrEmpty(input.FilterQuery)) query = query.Where(m => m.Name.Contains(input.FilterQuery));

        var recordCount = await query.CountAsync();
        query = query.OrderBy($"{input.SortColumn} {input.SortOrder}")
            .Skip(input.PageIndex * input.PageSize)
            .Take(input.PageSize);

        return new RestDTO<Mechanic[]>
        {
            Data = await query.ToArrayAsync(),
            PageIndex = input.PageIndex,
            PageSize = input.PageSize,
            RecordCount = recordCount,
            Links = new List<LinkDTO>
            {
                new(Url.Action(
                        null,
                        "Mechanics",
                        new { input.PageIndex, input.PageSize },
                        Request.Scheme)!,
                    "self",
                    "GET")
            }
        };
    }

    [Authorize(Roles = RoleNames.Moderator)]
    [HttpPost(Name = "UpdateMechanic")]
    [ResponseCache(CacheProfileName = "no-cache")]
    public async Task<RestDTO<Mechanic>> Post(MechanicDTO model)
    {
        var mechanic = await _context.Mechanics
            .Where(m => m.Id == model.Id)
            .FirstOrDefaultAsync();

        if (mechanic != null)
        {
            if (!string.IsNullOrEmpty(model.Name)) mechanic.Name = model.Name;

            _context.Update(mechanic);
            await _context.SaveChangesAsync();
        }

        return new RestDTO<Mechanic>
        {
            Data = mechanic,
            Links = new List<LinkDTO>
            {
                new(
                    Url.Action(
                        null,
                        "Mechanics",
                        model,
                        Request.Scheme)!,
                    "self",
                    "POST")
            }
        };
    }

    [Authorize(Roles = RoleNames.Administrator)]
    [HttpDelete(Name = "DeleteMechanic")]
    [ResponseCache(CacheProfileName = "no-cache")]
    public async Task<RestDTO<Mechanic?>> Delete(int id)
    {
        var mechanic = await _context.Mechanics
            .Where(m => m.Id == id)
            .FirstOrDefaultAsync();

        if (mechanic != null)
        {
            _context.Mechanics.Remove(mechanic);
            await _context.SaveChangesAsync();
        }

        return new RestDTO<Mechanic?>
        {
            Data = mechanic,
            Links = new List<LinkDTO>
            {
                new(
                    Url.Action(
                        null,
                        "Mechanics",
                        id,
                        Request.Scheme)!,
                    "self",
                    "DELETE")
            }
        };
    }
}