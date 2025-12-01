using GameStore.api.Dtos;
using GameStore.api.EndPoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGamesEndPoints();

app.MapGet("/", () => "Hello");

app.Run();
