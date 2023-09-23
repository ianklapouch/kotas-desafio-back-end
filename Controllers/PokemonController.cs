using kotas_desafio_back_end.Models;
using kotas_desafio_back_end.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace kotas_desafio_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly PokeAPIService pokeAPIService;
        public PokemonController(PokeAPIService pokeAPIService)
        {
            this.pokeAPIService = pokeAPIService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Inicialize o cronômetro
            Stopwatch stopwatch = new Stopwatch();

            // Comece a medir o tempo
            stopwatch.Start();
            List<Pokemon> randomPokemons = await pokeAPIService.Get10RandomPokemonsAsync();
            // Pare o cronômetro
            stopwatch.Stop();

            // Obtenha o tempo decorrido em milissegundos
            long tempoDecorridoMs = stopwatch.ElapsedMilliseconds;

            // Exiba o tempo decorrido
            Console.WriteLine("Tempo decorrido: " + tempoDecorridoMs + " ms");
            return Ok(randomPokemons);

        }

        [HttpGet("{idOrName?}")]
        public async Task<IActionResult> Get(string idOrName = null)
        {
            if (string.IsNullOrEmpty(idOrName))
            {
                return BadRequest();
            }

            Pokemon? pokemon = await pokeAPIService.GetPokemonAsync(idOrName);

            if (pokemon is not null) { 
                return Ok(pokemon);
            }

            return NotFound();
        }
    }
}
