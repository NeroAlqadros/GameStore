using System;
using GameStore.api.Data;
using GameStore.api.Dtos;
using GameStore.api.Entities;
using GameStore.api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.api.EndPoints;

public static class GamesEndPonts
{
    const string GetGameEndpointName = "GetGame";

    private static readonly List<GameSummaryDto> games = [
    new (1,"Street Fighter II","Fighting",19.99M, new DateOnly(1992,7,15)),
    new (2,"FinalFantasy XIV", "Roleplaying", 59.99M, new DateOnly(2010,9,30)),
    new (3,"FIFA 23", "Sports", 69.99m,new DateOnly(2022,9,27))
    ];

    public static RouteGroupBuilder MapGamesEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
                        .WithParameterValidation();

        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
        {
            return await dbContext.Games
                            .Include(game => game.Genre)
                            .Select(game => game.ToGameSummaryDto())
                            .AsNoTracking()
                            .ToListAsync();
        });

        //GET /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);

            return game is null ?
                Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        })
            .WithName(GetGameEndpointName);

        //POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();
            game.Genre = await dbContext.Genres.FindAsync(newGame.GenreId);

            await dbContext.Games.AddAsync(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(
                GetGameEndpointName,
                new { id = game.Id },
                game.ToGameDetailsDto());
        });

        //PUT /games/1
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingGame)
                        .CurrentValues
                        .SetValues(updatedGame.ToEntity(id));

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        //DELETE /games/4
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
                         .Where(game => game.Id == id)
                         .ExecuteDeleteAsync();

            return Results.NoContent();
        });

        return group;
    }
}
