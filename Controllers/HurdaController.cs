using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HurdaApi.Data;
using HurdaApi.Dtos;
using HurdaApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HurdaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HurdaController : ControllerBase
{
    private readonly AppDbContext _context;

    public HurdaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _context.HurdaItems.ToListAsync();
        return Ok(items);
    }

    /// <summary>Yeni eşya ekle (Yeni eşya ekle paneli — ayrı rota).</summary>
    [HttpPost("items")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddItem([FromBody] AddHurdaItemDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "Malzeme tanımı zorunludur." });

        var item = new HurdaItem
        {
            Name = dto.Name.Trim(),
            Description = dto.Description ?? string.Empty,
            Price = 0,
            ScrapAmount = dto.ScrapAmount,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = User.Identity?.Name ?? "admin",
        };

        _context.HurdaItems.Add(item);
        await _context.SaveChangesAsync();
        return Ok(item);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCount(int id, [FromBody] UpdateHurdaItemDto dto)
    {
        var item = await _context.HurdaItems.FindAsync(id);
        if (item == null)
            return NotFound();

        item.ScrapAmount = dto.ScrapAmount;
        item.UpdatedDate = DateTime.UtcNow;
        item.UpdatedBy = User.Identity!.Name;

        await _context.SaveChangesAsync();
        return Ok(item);
    }
}
