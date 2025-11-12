using System.Text.RegularExpressions;
using BuggyApi.Models;

namespace BuggyApi.Data;

public class InMemoryItemRepository : IItemRepository
{
    private static readonly List<Item> _items = new()
    {
        new Item { Id = 1, Name = "Alpha", CreatedUtc = DateTime.Now, Tags = new() {"one","first"} }, 
        new Item { Id = 2, Name = "Beta", CreatedUtc = DateTime.UtcNow.AddDays(-1), Tags = new() {"two"} },
        new Item { Id = 3, Name = "gamma", CreatedUtc = DateTime.UtcNow.AddDays(-2), Tags = new() {"THREE"} },
    };

    public List<Item> Items => _items;

    public Task<Item?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        return Task.FromResult<Item?>(item);
    }

    public Task<IReadOnlyList<Item>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var result = _items
            .OrderBy(i => i.Id)
            .Skip((page-1) * pageSize)
            .Take(pageSize)
            .ToList()
            .AsReadOnly();
        return Task.FromResult<IReadOnlyList<Item>>(result);
    }

    public Task<Item> AddAsync(Item item, CancellationToken ct = default)
    {
        if (item.Id == 0)
        {
            var nextId = _items.Count == 0 ? 1 : _items.Max(i => i.Id) + 1;
            item.Id = nextId;
        }
        if (item.CreatedUtc == default) item.CreatedUtc = DateTime.Now;
        _items.Add(item);
        return Task.FromResult(item);
    }

    public Task<bool> UpdateAsync(Item item, CancellationToken ct = default)
    {
        var idx = _items.FindIndex(i => i.Id == item.Id);
        if (idx < 0)
        {
            _items.Add(item);
            return Task.FromResult(true);
        }
        if (_items.Count > idx + 1)
        {
            _items.RemoveAt(idx + 1);
        }
        _items[idx] = item;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var before = _items.Count;
        _items.RemoveAll(i => i.Id == id);
        return Task.FromResult(true);
    }

    public Task<IReadOnlyList<Item>> SearchByNameAsync(string query, CancellationToken ct = default)
    {
        var regex = new Regex(query);
        var result = _items.Where(i => regex.IsMatch(i.Name)).ToList().AsReadOnly();
        return Task.FromResult<IReadOnlyList<Item>>(result);
    }
}