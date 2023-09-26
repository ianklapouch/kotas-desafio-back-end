using kotas_desafio_back_end.Data;
using kotas_desafio_back_end.Models;
using kotas_desafio_back_end.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace kotas_desafio_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly PokeAPIService _pokeAPIService;
        public PokemonController(AppDbContext appDbContext, PokeAPIService pokeAPIService)
        {
            _pokeAPIService = pokeAPIService;
            _appDbContext = appDbContext;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Pokemon> randomPokemons = await _pokeAPIService.Get10RandomPokemonsAsync();
            return Ok(randomPokemons);
        }


        [HttpGet("{idOrName?}")]
        public async Task<IActionResult> Get(string idOrName)
        {
            Pokemon? pokemon = await _pokeAPIService.GetPokemonAsync(idOrName);
            if (pokemon is null)
            {
                return NotFound();
            }

            return Ok(pokemon);
        }

        [HttpPost("CatchPokemon")]
        public async Task<IActionResult> CatchPokemon(CatchPokemon catchPokemon)
        {
            try
            {


                PokemonMaster? pokemonMaster = await _appDbContext.PokemonMasters
                                                                  .Include(pm => pm.CapturedPokemons)
                                                                  .FirstOrDefaultAsync(pm => pm.Id == catchPokemon.PokemonMasterId);
                if (pokemonMaster is null)
                {
                    return NotFound("Pokemon Master not found!");
                }


                Pokemon? pokemon = await _pokeAPIService.GetPokemonAsync(catchPokemon.PokemonIdOrName);
                if (pokemon is null)
                {
                    return NotFound("Pokemon not found!");
                }


                bool pokemonAlreadyCaught = pokemonMaster.CapturedPokemons.Any(p => p.PokemonId == pokemon.Id);
                if (pokemonAlreadyCaught)
                {
                    return Conflict($"Pokemon already captured by: {pokemonMaster.Nome}");
                }

                CapturedPokemon capturedPokemon = new()
                {
                    PokemonId = pokemon.Id,
                };


                pokemonMaster.CapturedPokemons.Add(capturedPokemon);
                _appDbContext.CapturedPokemons.Add(capturedPokemon);
                await _appDbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocorreu um erro ao criar o PokemonMaster: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("CapturedPokemons")]
        public async Task<IActionResult> GetCapturedPokemons(Guid id)
        {
            PokemonMaster? pokemonMaster = await _appDbContext.PokemonMasters
                                                            .Include(pm => pm.CapturedPokemons)
                                                            .FirstOrDefaultAsync(pm => pm.Id == id);
            if (pokemonMaster is null)
            {
                return NotFound("Pokemon Master not found!");
            }

            if (pokemonMaster.CapturedPokemons is null)
            {
                return NotFound("Pokemon not found!");
            }

            List<Pokemon> pokemons = new();
            foreach (CapturedPokemon capturedPokemon in pokemonMaster.CapturedPokemons)
            {
                Pokemon? pokemon = await _pokeAPIService.GetPokemonAsync(capturedPokemon.PokemonId.ToString());
                if (pokemon is not null)
                {
                    pokemons.Add(pokemon);
                }
            }

            return Ok(pokemons);
        }
    }
}
