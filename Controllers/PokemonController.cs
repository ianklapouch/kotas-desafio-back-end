using kotas_desafio_back_end.Data;
using kotas_desafio_back_end.Models;
using kotas_desafio_back_end.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
            // Inicialize o cronômetro
            Stopwatch stopwatch = new Stopwatch();

            // Comece a medir o tempo
            stopwatch.Start();
            List<Pokemon> randomPokemons = await _pokeAPIService.Get10RandomPokemonsAsync();
            // Pare o cronômetro
            stopwatch.Stop();

            // Obtenha o tempo decorrido em milissegundos
            long tempoDecorridoMs = stopwatch.ElapsedMilliseconds;

            // Exiba o tempo decorrido
            Console.WriteLine("Tempo decorrido: " + tempoDecorridoMs + " ms");
            return Ok(randomPokemons);

        }

        [HttpGet("{idOrName?}")]
        public async Task<IActionResult> Get(string? idOrName = null)
        {
            if (string.IsNullOrEmpty(idOrName))
            {
                return BadRequest();
            }

            Pokemon? pokemon = await _pokeAPIService.GetPokemonAsync(idOrName);

            if (pokemon is not null)
            {
                return Ok(pokemon);
            }

            return NotFound();
        }

        [HttpPost("CatchPokemon")]
        public async Task<IActionResult> CatchPokemon(CatchPokemon catchPokemon)
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
                return NotFound("Pokemon Master not found!");
            }

            CapturedPokemon capturedPokemon = new()
            {
                PokemonId = pokemon.Id
            };

            _appDbContext.CapturedPokemons.Add(capturedPokemon);


            await _appDbContext.SaveChangesAsync();

            return Ok();
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
                return NotFound("Pokemon Master not found!");
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
 