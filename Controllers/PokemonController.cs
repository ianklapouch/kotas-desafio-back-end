using kotas_desafio_back_end.Models;
using kotas_desafio_back_end.Services;
using Microsoft.AspNetCore.Mvc;

namespace kotas_desafio_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService _pokemonService;

        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet("GetRandomPokemons")]
        public async Task<IActionResult> GetRandomPokemons()
        {
            try
            {
                List<Pokemon> pokemons = await _pokemonService.GetRandomPokemonsAsync();
                return Ok(pokemons);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet("{pokemonIdOrName?}")]
        public async Task<IActionResult> Get(string pokemonIdOrName)
        {
            try
            {
                Pokemon? pokemon = await _pokemonService.GetPokemonAsync(pokemonIdOrName);
                if (pokemon is null)
                {
                    return NotFound();
                }

                return Ok(pokemon);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost("CatchPokemon")]
        public async Task<IActionResult> CatchPokemon(CatchPokemon catchPokemon)
        {
            try
            {
                await _pokemonService.CatchPokemonAsync(catchPokemon);
                return Ok();
            }
            catch (PokemonServiceException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("CapturedPokemons")]
        public async Task<IActionResult> GetCapturedPokemons(Guid pokemonMasterId)
        {
            try
            {
                List<Pokemon> capturedPokemons = await _pokemonService.GetCapturedPokemonsAsync(pokemonMasterId);
                return Ok(capturedPokemons);
            }
            catch (PokemonServiceException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }
    }
}
