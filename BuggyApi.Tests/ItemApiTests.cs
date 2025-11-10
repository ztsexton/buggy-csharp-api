using System.Net;
using System.Net.Http.Json;
using BuggyApi;
using BuggyApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace BuggyApi.Tests;

public class ItemApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ItemApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetById_NotFound_ShouldReturn404()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/items/9999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Pagination_Page1_DefaultSize_ShouldContainSeed()
    {
        var client = _factory.CreateClient();
        var items = await client.GetFromJsonAsync<List<Item>>("/api/items?page=1&pageSize=2");
        items.Should().NotBeNull();
        items!.Count.Should().Be(2);
        items[0].Id.Should().Be(1);
    }

    [Fact]
    public async Task Create_ShouldSetUtcTime_AndIgnoreClientId()
    {
        var client = _factory.CreateClient();
        var dto = new Item { Id = 42, Name = "NewOne", CreatedUtc = DateTime.MinValue };
        var resp = await client.PostAsJsonAsync("/api/items", dto);
        resp.EnsureSuccessStatusCode();
        var created = await resp.Content.ReadFromJsonAsync<Item>();
        created!.Id.Should().NotBe(42);
        created.CreatedUtc.Kind.Should().Be(DateTimeKind.Utc);
    }
}