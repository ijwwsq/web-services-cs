using GamesApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GamesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private static readonly List<Game> games = new()
    {
        new Game { Id = 1, Title = "The Witcher 3: Wild Hunt", Genre = "RPG", Developer = "CD Projekt Red", ReleaseYear = 2015, Price = 29.99m },
        new Game { Id = 2, Title = "Hollow Knight", Genre = "Metroidvania", Developer = "Team Cherry", ReleaseYear = 2017, Price = 14.99m },
        new Game { Id = 3, Title = "Portal 2", Genre = "Puzzle", Developer = "Valve", ReleaseYear = 2011, Price = 9.99m }
    };

    [HttpGet]
    public ActionResult<List<Game>> GetAll()
    {
        return Ok(games);
    }

    [HttpGet("{id}")]
    public ActionResult<Game> GetById(int id)
    {
        var game = games.FirstOrDefault(x => x.Id == id);
        if (game == null)
        {
            return NotFound();
        }
        return Ok(game);
    }

    [HttpPost]
    public ActionResult<Game> Create(Game game)
    {
        game.Id = games.Count == 0 ? 1 : games.Max(x => x.Id) + 1;
        games.Add(game);
        return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
    }
}
