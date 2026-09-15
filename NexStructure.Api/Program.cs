using Scalar.AspNetCore;
using NexStructure.Application.Core;
using NexStructure.Application.Realtime.Hubs;
using NexStructure.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure();
var app = builder.Build();
app.MapScalarApiReference(options =>
{
  options.WithTitle("NexStructure API");
});

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.AddApplication();
app.UseHttpsRedirection();
app.MapHub<BaseHub>("hubs/baseHub");


app.Run();

