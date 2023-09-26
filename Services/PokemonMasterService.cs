using kotas_desafio_back_end.Data;
using kotas_desafio_back_end.Models;
using Microsoft.EntityFrameworkCore;

namespace kotas_desafio_back_end.Services
{
    public class PokemonMasterService : IPokemonMasterService
    {
        private readonly AppDbContext _appDbContext;
        private readonly PokeAPIService _pokeAPIService;

        public PokemonMasterService(AppDbContext appDbContext, PokeAPIService pokeAPIService)
        {
            _appDbContext = appDbContext;
            _pokeAPIService = pokeAPIService;
        }


        public async Task<PokemonMaster> CreatePokemonMaster(PokemonMasterCreate pokemonMasterCreate)
        {
            bool cpfAlreadyRegistered = _appDbContext.PokemonMasters.Any(pm => pm.Cpf == pokemonMasterCreate.Cpf);
            if (cpfAlreadyRegistered)
            {
                throw new PokemonMasterServiceException("There is already a pokemon master registered with this CPF!", StatusCodes.Status409Conflict);
            }


            PokemonMaster pokemonMaster = new()
            {
                Nome = pokemonMasterCreate.Nome,
                Idade = pokemonMasterCreate.Idade,
                Cpf = pokemonMasterCreate.Cpf
            };

            _appDbContext.PokemonMasters.Add(pokemonMaster);
            await _appDbContext.SaveChangesAsync();
            return pokemonMaster;
        }

        public async Task<PokemonMaster?> GetPokemonMaster(Guid id)
        {
            PokemonMaster? pokemonMaster = await _appDbContext.PokemonMasters
                                                       .Include(pm => pm.CapturedPokemons)
                                                       .FirstOrDefaultAsync(pm => pm.Id == id);
            return pokemonMaster;
        }
    }
}
