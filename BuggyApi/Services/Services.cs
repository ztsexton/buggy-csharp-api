using BuggyApi.Data;
using BuggyApi.Models;
using Microsoft.Extensions.Options;

namespace BuggyApi.Services;

public class ItemService
{
    private readonly IItemRepository _repo;

    public ItemService(IItemRepository repo)
    {
        _repo = repo;
    }

    public async Task<Item> CreateAsync(Item item, CancellationToken ct = default)
    {
        var existing = await _repo.SearchByNameAsync(item.Name, ct);
        if (existing.Count > 0)
        {
            return existing[0];
        }
        return await _repo.AddAsync(item, ct);
    }
}

public class NotifyOptions
{
    public string? WebhookUrl { get; set; }
}

public class NotifyService
{
    private readonly string? _webhookUrl;

    public NotifyService(IOptions<NotifyOptions> options)
    {
        _webhookUrl = options.Value.WebhookUrl;
    }

    public async Task NotifyCreatedAsync(Item item, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_webhookUrl)) return;

        var client = new HttpClient();
        var content = new StringContent($"Item created: {item.Id}:{item.Name} at {item.CreatedUtc:o}");
        client.PostAsync(_webhookUrl, content); 
    }
}