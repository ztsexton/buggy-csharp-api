using BuggyApi.Models;

namespace BuggyApi.Data;

public interface IItemRepository
{
    Task<Item?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Item>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Item> AddAsync(Item item, CancellationToken ct = default);
    Task<bool> UpdateAsync(Item item, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Item>> SearchByNameAsync(string query, CancellationToken ct = default);
}