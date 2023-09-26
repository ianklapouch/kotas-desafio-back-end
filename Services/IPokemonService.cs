using kotas_desafio_back_end.Models;

namespace kotas_desafio_back_end.Services
{
    public interface IPokemonService
    {
        Task<List<Pokemon>> GetRandomPokemonsAsync();
        Task<Pokemon?> GetPokemonAsync(string idOrName);
        Task CatchPokemonAsync(CatchPokemon catchPokemon);
        Task<List<Pokemon>> GetCapturedPokemonsAsync(Guid id);
    }
}
