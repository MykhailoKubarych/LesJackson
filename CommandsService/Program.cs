using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.EventProcessing;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var services = builder.Services;
services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMen"));
services.AddScoped<ICommandRepo, CommandRepo>();
services.AddControllers();

services.AddHostedService<MessageBusSubscriber>();

services.AddSingleton<IEventProcessor, EventProcessor>();
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddScoped<IPlatformDataClient, PlatformDataClient>();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CommandsService", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CommandsService v1"));;
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

PrepDb.PrepPopulation(app);
await app.RunAsync();