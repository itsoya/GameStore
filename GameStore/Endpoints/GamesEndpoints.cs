using System;
using GameStore.Dtos;

namespace GameStore.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";
    
    private static readonly List<GameDto> games = [
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
            group.MapGet("/", () => games);

            // Get /games/{id} http://localhost:5022/games/1
            group.MapGet("/{id}", (int id) =>
            { 
                var game = games.Find(games => games.Id == id);
                if (game == null) return Results.NotFound();
                return Results.Ok(game);
            }).WithName(GetGameEndpointName);

            // POST /games http://localhost:5022/games
            group.MapPost("/", (CreateGameDto game) =>
            {
                if (string.IsNullOrEmpty(game.Name) || string.IsNullOrEmpty(game.Genre) || game.Price <= 0)
                {
                    return Results.BadRequest("Invalid game data. Name, Genre must be provided and Price must be greater than 0.");
                }
                ddwdwad
                var newGame = new GameDto(games.Count + 1, game.Name, game.Genre, game.Price, game.ReleaseDate);
                games.Add(newGame);
                return Results.CreatedAtRoute(GetGameEndpointName, new { id = newGame.Id }, newGame);
            });

            //put /games/{id} http://localhost:5022/games/1
            group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
            {
                var index = games.FindIndex(games => games.Id == id);
                if (index == -1)    {
                    return Results.NotFound();
                }
                games[index] = new GameDto (id, updatedGame.Name, updatedGame.Genre, updatedGame.Price, updatedGame.ReleaseDate);
                return Results.NoContent();
            });

            // DELETE /games/{id} http://localhost:5022/games/1
            group.MapDelete("/{id}", (int id) =>
            {
                var index = games.FindIndex(games => games.Id == id);
                if (index == -1)    {
                    return Results.NotFound();
                }
                games.RemoveAt(index);
                return Results.NoContent();
            });
        }
    }