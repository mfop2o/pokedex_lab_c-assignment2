using Microsoft.AspNetCore.Mvc;
using pokedex.Models;
using pokedex.Services;

namespace pokedex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService _pokemonService;

        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Pokemon>>> GetAll()
        {
            var pokemons = await _pokemonService.GetAllAsync();
            return Ok(pokemons);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Pokemon>> GetById(int id)
        {
            var pokemon = await _pokemonService.GetByIdAsync(id);
            if (pokemon == null)
            {
                return NotFound($"Pokemon with ID {id} not found.");
            }
            return Ok(pokemon);
        }

        [HttpPost]
        public async Task<ActionResult<Pokemon>> AddPokemon([FromBody] Pokemon newPokemon)
        {
            if (newPokemon == null)
            {
                return BadRequest("The Pokemon data is empty. Retry!");
            }
            var pokemon = await _pokemonService.AddAsync(newPokemon);
            return CreatedAtAction(nameof(GetById), new { id = pokemon.Id }, pokemon);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Pokemon>> UpdatePokemon(int id, [FromBody] Pokemon updatedPokemon)
        {
            var updated = await _pokemonService.UpdateAsync(id, updatedPokemon);
            if (updated == null)
            {
                return NotFound($"Pokemon with ID {id} not found.");
            }
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeletePokemon(int id)
        {
            var deleted = await _pokemonService.DeleteAsync(id);
            if (deleted)
            {
                return Ok($"Pokemon with ID {id} deleted successfully.");
            }
            return NotFound($"Pokemon with ID {id} not found.");
        }
    }
}
