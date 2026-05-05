using BlogApi.Models.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaginationController : ControllerBase
{
    private PaginationSettings _settings;

    public PaginationController(IOptions<PaginationSettings> options)
    {
        _settings = options.Value;
    }

    [HttpGet("settings")]
    public IActionResult GetSettings()
    {
        return Ok(new { PageSize = _settings.PageSize, MaxPageSize = _settings.MaxPageSize });
    }
}