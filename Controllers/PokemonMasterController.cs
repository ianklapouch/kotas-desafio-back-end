using kotas_desafio_back_end.Models;
using kotas_desafio_back_end.Services;
using Microsoft.AspNetCore.Mvc;

namespace kotas_desafio_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonMasterController : ControllerBase
    {
        private readonly IPokemonMasterService _pokemonMasterService;

        public PokemonMasterController(IPokemonMasterService pokemonMasterService)
        {
            _pokemonMasterService = pokemonMasterService;
        }

        [HttpPost]
        public async Task<ActionResult> CreatePokemonMaster(PokemonMasterCreate pokemonMasterCreate)
        {

            try
            {
                PokemonMaster pokemonMaster = await _pokemonMasterService.CreatePokemonMaster(pokemonMasterCreate);
                return CreatedAtAction(nameof(GetPokemonMasterById), new { id = pokemonMaster.Id }, pokemonMaster);
            }
            catch(PokemonMasterServiceException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PokemonMaster>> GetPokemonMasterById(Guid id)
        {
            try
            {
                PokemonMaster? pokemonMaster = await _pokemonMasterService.GetPokemonMaster(id);
                if (pokemonMaster is null)
                {
                    return NotFound();
                }

                return pokemonMaster;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ex.Message);

            }
        }
    }
}
