using GameStore.api.Data;
using GameStore.api.Dtos;
using GameStore.api.EndPoints;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

var app = builder.Build();

app.MapGamesEndPoints();
app.MapGenresEndpoints();

await app.MigrateDbAsync();

app.MapGet("/", () => "Hello");

app.Run();
