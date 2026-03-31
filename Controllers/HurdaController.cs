using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HurdaApi.Data;
using HurdaApi.Dtos;
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
