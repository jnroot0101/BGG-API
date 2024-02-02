using Microsoft.AspNetCore.Mvc;
using MyBGList.Models;

namespace MyBGList.Controllers;

[Route("[controller]")]
[ApiController]
public class SeedController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SeedController> _logger;
    private readonly IWebHostEnvironment _env;

    public SeedController(
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        ILogger<SeedController> logger)
    {
        _context = context;
        _env = environment;
        _logger = logger;
    }
}