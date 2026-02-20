using GameStore.Dtos;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

const string GetGameEndpointName = "GetGame";

List<GameDto> games = [
    new (1, "The Legend of Zelda: Breath of the Wild", "Action-adventure", 59.99m, new DateOnly(2017, 3, 3)),
    new (2, "Super Mario Odyssey", "Platformer", 59.99m, new DateOnly(2017, 10, 27)),
    new (3, "Red Dead Redemption 2", "Action-adventure", 59.99m, new DateOnly(2018, 10, 26)),
    new (4, "The Witcher 3: Wild Hunt", "Action RPG", 39.99m, new DateOnly(2015, 5, 19)),
    new (5, "Minecraft", "Sandbox", 26.95m, new DateOnly(2011, 11, 18))
];

// GET  /games http://localhost:5022
app.MapGet("/games", () => games);

// Get /games/{id} http://localhost:5022/games/1
app.MapGet("/games/{id}", (int id) =>
{ 
    var game = games.Find(games => games.Id == id);
    if (game == null) return Results.NotFound();
    return Results.Ok(game);
}).WithName(GetGameEndpointName);

// POST /games http://localhost:5022/games
app.MapPost("/games", (CreateGameDto game) =>
{
    var newGame = new GameDto(games.Count + 1, game.Name, game.Genre, game.Price, game.ReleaseDate);
    games.Add(newGame);
    return Results.CreatedAtRoute(GetGameEndpointName, new { id = newGame.Id }, newGame);
});

//put /games/{id} http://localhost:5022/games/1
app.MapPut("/games/{id}", (int id, UpdateGameDto updatedGame) =>
{
    var index = games.FindIndex(games => games.Id == id);
    if (index == -1)    {
        return Results.NotFound();
    }
    games[index] = new GameDto (id, updatedGame.Name, updatedGame.Genre, updatedGame.Price, updatedGame.ReleaseDate);
    return Results.NoContent();
});

// DELETE /games/{id} http://localhost:5022/games/1
app.MapDelete("/games/{id}", (int id) =>
{
    var index = games.FindIndex(games => games.Id == id);
    if (index == -1)    {
        return Results.NotFound();
    }
    games.RemoveAt(index);
    return Results.NoContent();
});

app.Run();