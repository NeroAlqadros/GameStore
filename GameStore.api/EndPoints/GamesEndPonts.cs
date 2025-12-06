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
        group.MapGet("/", (GameStoreContext dbContext) =>
        {
            return dbContext.Games
                            .Include(game => game.Genre)
                            .Select(game => game.ToGameSummaryDto())
                            .AsNoTracking();
        });

        //GET /games/1
        group.MapGet("/{id}", (int id, GameStoreContext dbContext) =>
        {
            var game = dbContext.Games.Find(id);

            return game is null ?
                Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        })
            .WithName(GetGameEndpointName);

        //POST /games
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();
            game.Genre = dbContext.Genres.Find(newGame.GenreId);

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute(
                GetGameEndpointName,
                new { id = game.Id },
                game.ToGameDetailsDto());
        });

        //PUT /games/1
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = dbContext.Games.Find(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingGame)
                        .CurrentValues
                        .SetValues(updatedGame.ToEntity(id));

            dbContext.SaveChanges();

            return Results.NoContent();
        });

        //DELETE /games/4
        group.MapDelete("/{id}", (int id, GameStoreContext dbContext) =>
        {
            dbContext.Games
                         .Where(game => game.Id == id)
                         .ExecuteDelete();



            return Results.NoContent();
        });

        return group;
    }
}
