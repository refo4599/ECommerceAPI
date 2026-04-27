using ECommerceAPI.API.Extensions;
using ECommerceAPI.API.Middlewares;
using ECommerceAPI.Infrastructure.Data;
using ECommerceAPI.Infrastructure.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddSwaggerWithJwt();
builder.Services.AddCorsPolicy();
builder.Services.AddSignalRService();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Angular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// SignalR endpoint
app.MapHub<StockHub>("/hubs/stock");

app.Run();