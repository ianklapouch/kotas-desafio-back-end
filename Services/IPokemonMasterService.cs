using kotas_desafio_back_end.Models;

namespace kotas_desafio_back_end.Services
{
    public interface IPokemonMasterService
    {
        Task<PokemonMaster> CreatePokemonMasterAsync(PokemonMasterCreate pokemonMasterCreate);
        Task<PokemonMaster?> GetPokemonMasterAsync(Guid id);
        Task<List<PokemonMaster>> GetPokemonMastersAsync();
    }
}
