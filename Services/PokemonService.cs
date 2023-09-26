using kotas_desafio_back_end.Data;
using kotas_desafio_back_end.Models;
using Microsoft.EntityFrameworkCore;

namespace kotas_desafio_back_end.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly AppDbContext _appDbContext;
        private readonly PokeAPIService _pokeAPIService;

        public PokemonService(AppDbContext appDbContext, PokeAPIService pokeAPIService)
        {
            _appDbContext = appDbContext;
            _pokeAPIService = pokeAPIService;
        }

        public async Task<List<Pokemon>> GetRandomPokemonsAsync()
        {
            List<Pokemon> pokemons = await _pokeAPIService.GetRandomPokemonsAsync();
            return pokemons;
        }

        public async Task<Pokemon?> GetPokemonAsync(string idOrName)
        {
            Pokemon? pokemon = await _pokeAPIService.GetPokemonAsync(idOrName);
            if (pokemon is not null)
            {
                return pokemon;
            }

            return null;
        }

        public async Task CatchPokemonAsync(CatchPokemon catchPokemon)
        {
            PokemonMaster? pokemonMaster = await _appDbContext.PokemonMasters
                                                                 .Include(pm => pm.CapturedPokemons)
                                                                 .FirstOrDefaultAsync(pm => pm.Id == catchPokemon.PokemonMasterId);
            if (pokemonMaster is null)
            {
                throw new PokemonServiceException("Pokemon Master not found!", StatusCodes.Status404NotFound);
            }


            Pokemon? pokemon = await _pokeAPIService.GetPokemonAsync(catchPokemon.PokemonIdOrName);
            if (pokemon is null)
            {
                throw new PokemonServiceException("Pokemon not found!", StatusCodes.Status404NotFound);
            }


            bool pokemonAlreadyCaught = pokemonMaster.CapturedPokemons.Any(p => p.PokemonId == pokemon.Id);
            if (pokemonAlreadyCaught)
            {
                throw new PokemonServiceException($"Pokemon already captured by: {pokemonMaster.Nome}", StatusCodes.Status409Conflict);
            }

            CapturedPokemon capturedPokemon = new()
            {
                PokemonId = pokemon.Id,
            };

            pokemonMaster.CapturedPokemons.Add(capturedPokemon);
            _appDbContext.CapturedPokemons.Add(capturedPokemon);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<List<Pokemon>> GetCapturedPokemonsAsync(Guid pokemonMasterId)
        {
            PokemonMaster? pokemonMaster = await _appDbContext.PokemonMasters
                                                            .Include(pm => pm.CapturedPokemons)
                                                            .FirstOrDefaultAsync(pm => pm.Id == pokemonMasterId);
            if (pokemonMaster is null)
            {
                throw new PokemonServiceException("Pokemon Master not found!", StatusCodes.Status404NotFound);
            }

            if (pokemonMaster.CapturedPokemons is null || pokemonMaster.CapturedPokemons.Count == 0)
            {
                return new List<Pokemon>();
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

            return pokemons;
        }
    }
}
