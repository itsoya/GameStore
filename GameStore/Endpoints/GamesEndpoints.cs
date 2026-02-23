using System;
using GameStore.Data;
using GameStore.Dtos;
using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    private static readonly List<GameSummaryDto> games = [
        new (1, "The Legend of Zelda: Breath of the Wild", "Action-adventure", 59.99m, new DateOnly(2017, 3, 3)),
        new (2, "Super Mario Odyssey", "Platformer", 59.99m, new DateOnly(2017, 10, 27)),
        new (3, "Red Dead Redemption 2", "Action-adventure", 59.99m, new DateOnly(2018, 10, 26)),
        new (4, "The Witcher 3: Wild Hunt", "Action RPG", 39.99m, new DateOnly(2015, 5, 19)),
        new (5, "Minecraft", "Sandbox", 26.95m, new DateOnly(2011, 11, 18))
    ];

    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games").WithTags("Games");

        // GET  /games http://localhost:5022
        group.MapGet("/", async (GameStoreContext dbContext) => await dbContext.Games.Include(game => game.Genre).Select(
            game => new GameSummaryDto(game.Id, game.Name, game.Genre!.Name, game.Price, game.ReleaseDate)).AsNoTracking().ToListAsync());

        // Get /games/{id} http://localhost:5022/games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);
            return game is not null
                ? Results.Ok(new GameDetailsDto(game.Id, game.Name, game.GenreId, game.Price, game.ReleaseDate))
                : Results.NotFound();
        }).WithName(GetGameEndpointName);

        // POST /games http://localhost:5022/games
        group.MapPost("/", async (CreateGameDto game, GameStoreContext dbContext) =>
        {
            Game newGame = new()
            {
                Name = game.Name,
                GenreId = game.GenreId,
                Price = game.Price,
                ReleaseDate = game.ReleaseDate
            };


            if (string.IsNullOrEmpty(game.Name) || game.GenreId <= 0 || game.Price <= 0)
            {
                return Results.BadRequest("Invalid game data. Name, Genre must be provided and Price must be greater than 0.");
            }
            dbContext.Games.Add(newGame);
            dbContext.SaveChangesAsync().Wait();
            GameDetailsDto gameDetailsDto = new(newGame.Id, newGame.Name, newGame.GenreId, newGame.Price, newGame.ReleaseDate);
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = gameDetailsDto.Id }, gameDetailsDto);
        });

        //put /games/{id} http://localhost:5022/games/1
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);
            
            if (game is null)
            {
                return Results.NotFound();
            }
            game.Name = updatedGame.Name;
            game.GenreId = updatedGame.GenreId;
            game.Price = updatedGame.Price;
            game.ReleaseDate = updatedGame.ReleaseDate;

            dbContext.Games.Update(game);
            dbContext.SaveChangesAsync().Wait();
            return Results.Ok(new GameDetailsDto(game.Id, game.Name, game.GenreId, game.Price, game.ReleaseDate));
        });

        // DELETE /games/{id} http://localhost:5022/games/1
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);
            if (game is null)
            {
                return Results.NotFound();
            }
            dbContext.Games.Remove(game);
            dbContext.SaveChangesAsync().Wait();
            return Results.NoContent();
        });
    }
}