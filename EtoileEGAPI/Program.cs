using Application.ApplicationDI;
using Domain.Entities;
using EtoileEGAPI.Middlewares;
using Infrastructure.InfraStructureDI;
using Infrastructure.Logging;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOpenApi();

#region Infrastructure DI Registration
builder.Services.AddInfraStructureDIRegister(builder.Configuration);


//builder.Host.UseSerilogLogging(builder.Configuration);
#endregion
#region Application DI Registration
builder.Services.AddApplicationDIRegister();
#endregion

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await IdentityDataSeeder.SeedRolesAsync(roleManager);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
//app.UseSerilogRequestLogging();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
