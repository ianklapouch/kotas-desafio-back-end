using kotas_desafio_back_end.Data;
using kotas_desafio_back_end.Models;
using Microsoft.AspNetCore.Mvc;

namespace kotas_desafio_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonMasterController : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        public PokemonMasterController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        [HttpPost]
        public async Task<ActionResult> CreatePokemonMaster(PokemonMasterCreate newPokemonMaster)
        {

            try
            {

                bool cpfAlreadyRegistered = appDbContext.PokemonMasters.Any(pm => pm.Cpf == newPokemonMaster.Cpf);
                if (cpfAlreadyRegistered)
                {
                    return Conflict("There is already a pokemon master registered with this CPF!");
                }


                PokemonMaster pokemonMaster = new()
                {
                    Nome = newPokemonMaster.Nome,
                    Idade = newPokemonMaster.Idade,
                    Cpf = newPokemonMaster.Cpf
                };

                appDbContext.PokemonMasters.Add(pokemonMaster);
                await appDbContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPokemonMasterById), new { id = pokemonMaster.Id }, pokemonMaster);

            }
            catch (Exception ex)
            {
                // Trate a exceção aqui, dependendo do tipo de erro.
                // Por exemplo, você pode logar o erro ou retornar uma resposta de erro personalizada.
                return StatusCode(500, "Ocorreu um erro ao criar o PokemonMaster: " + ex.Message);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<PokemonMaster>> GetPokemonMasterById(Guid id)
        {
            var pokemonMaster = await appDbContext.PokemonMasters.FindAsync(id);
            if (pokemonMaster == null)
            {
                return NotFound();
            }

            return pokemonMaster;
        }
    }
}
