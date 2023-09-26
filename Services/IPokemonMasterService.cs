using kotas_desafio_back_end.Models;

namespace kotas_desafio_back_end.Services
{
    public interface IPokemonMasterService
    {
        Task<PokemonMaster> CreatePokemonMaster(PokemonMasterCreate pokemonMasterCreate);
        Task<PokemonMaster?> GetPokemonMaster(Guid id);
    }
}
