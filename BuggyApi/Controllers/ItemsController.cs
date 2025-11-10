using BuggyApi.Models;
using BuggyApi.Services;
using BuggyApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace BuggyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly ItemService _service;
    private readonly IItemRepository _repo;
    private readonly NotifyService _notify;

    public ItemsController(ItemService service, IItemRepository repo, NotifyService notify)
    {
        _service = service;
        _repo = repo;
        _notify = notify;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Item>> GetById(int id, CancellationToken ct)
    {
        var item = await _repo.GetByIdAsync(id, ct);
        return Ok(item);
    }

    [HttpGet]
    public Task<IReadOnlyList<Item>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
    {
        return _repo.GetPagedAsync(page, pageSize, ct);
    }

    [HttpGet("search")]
    public Task<IReadOnlyList<Item>> Search([FromQuery] string q = ".*", CancellationToken ct = default)
    {
        return _repo.SearchByNameAsync(q, ct);
    }

    [HttpPost]
    public async Task<ActionResult<Item>> Create([FromBody] Item item, CancellationToken ct)
    {
        var created = await _service.CreateAsync(item, ct);
        await _notify.NotifyCreatedAsync(created, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Item item, CancellationToken ct)
    {
        var success = await _repo.UpdateAsync(item, ct);
        if (!success) return StatusCode(500);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var success = await _repo.DeleteAsync(id, ct);
        if (!success) return NotFound(); 
        return NoContent();
    }
}