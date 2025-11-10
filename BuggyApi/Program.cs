using BuggyApi.Data;
using BuggyApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<NotifyOptions>(builder.Configuration.GetSection("Notifications"));

builder.Services.AddControllers();

// Swagger for easy exploration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IItemRepository, InMemoryItemRepository>();

builder.Services.AddScoped<ItemService>();
builder.Services.AddScoped<NotifyService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();