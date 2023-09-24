using kotas_desafio_back_end.Models;
using kotas_desafio_back_end.Models.PokeAPI;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace kotas_desafio_back_end.Services
{
    public class PokeAPIService
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;

        public PokeAPIService(IMemoryCache cache)
        {
            _cache = cache;
            _httpClient = new HttpClient();
        }

        private async Task<int> GetNumberOfPokemonsAsync()
        {
            if (_cache.TryGetValue<int>("NumberOfPokemons", out var numberOfPokemons))
                return numberOfPokemons;

            numberOfPokemons = await RefreshAndCacheNumberOfPokemonsAsync();
            return numberOfPokemons;
        }

        private async Task<int> RefreshAndCacheNumberOfPokemonsAsync()
        {
            try
            {
                var url = "https://pokeapi.co/api/v2/pokemon-species/?limit=0";
                var response = await _httpClient.GetStringAsync(url);
                var jsonResponse = JObject.Parse(response);
                var numberOfPokemons = jsonResponse.Value<int>("count");

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                };

                _cache.Set("NumberOfPokemons", numberOfPokemons, cacheEntryOptions);
                return numberOfPokemons;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private async Task<List<int>> GetRandomIdsAsync(int count)
        {
            var numberOfPokemons = await GetNumberOfPokemonsAsync();

            if (numberOfPokemons == 0)
                return new List<int>();

            var random = new Random();
            var randomIds = new List<int>();

            while (randomIds.Count < count)
            {
                var randomId = random.Next(1, numberOfPokemons + 1);

                if (!randomIds.Contains(randomId))
                    randomIds.Add(randomId);
            }

            return randomIds;
        }

        public async Task<Pokemon?> GetPokemonAsync(string idOrName)
        {
            try
            {
                var url = $"https://pokeapi.co/api/v2/pokemon/{idOrName}/";
                var response = await _httpClient.GetStringAsync(url);
                var rawPokemonData = JsonConvert.DeserializeObject<RawPokemonData>(response);

                if (rawPokemonData == null)
                    return null;

                var evolutions = await GetEvolutionChainAsync(rawPokemonData.Species.Url);
                var pokemon = await RawPokemonDataToPokemonAsync(rawPokemonData);

                pokemon.Evolutions = evolutions;
                return pokemon;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<Pokemon> RawPokemonDataToPokemonAsync(RawPokemonData rawPokemonData)
        {
            //var pokemon = new Pokemon
            //{
            //    Id = rawPokemonData.Id,
            //    Name = rawPokemonData.Name
            //};

            string Base64Sprite = string.Empty;
            if (!string.IsNullOrEmpty(rawPokemonData.Sprites.FrontDefault))
            {
                var spriteUrl = rawPokemonData.Sprites.FrontDefault;

                try
                {
                    var imageBytes = await _httpClient.GetByteArrayAsync(spriteUrl);
                    Base64Sprite = Convert.ToBase64String(imageBytes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            List<string> types = rawPokemonData.Types
                .Where(types => !string.IsNullOrEmpty(types.Type.Name))
                .Select(types => types.Type.Name)
                .ToList();

            Pokemon pokemon = new()
            {
                Id = rawPokemonData.Id,
                Name = rawPokemonData.Name,
                Base64Sprite = Base64Sprite,
                Types = types
            };


            return pokemon;
        }

        private async Task<List<string>> GetEvolutionChainAsync(string url)
        {
            var response = await _httpClient.GetStringAsync(url);

            if (string.IsNullOrEmpty(response))
                return new List<string>();

            var pokemonSpecies = JsonConvert.DeserializeObject<PokemonSpecies>(response);

            if (pokemonSpecies == null || pokemonSpecies.EvolutionChain == null)
                return new List<string>();

            response = await _httpClient.GetStringAsync(pokemonSpecies.EvolutionChain.Url);

            if (string.IsNullOrEmpty(response))
                return new List<string>();

            var evolutionChain = JsonConvert.DeserializeObject<EvolutionChain>(response);

            if (evolutionChain == null || evolutionChain.Chain == null)
                return new List<string>();

            var species = GetSpeciesNames(evolutionChain);
            return species;
        }

        private static List<string> GetSpeciesNames(EvolutionChain evolutionChain)
        {
            List<string> speciesNames = new();
            if (evolutionChain != null && evolutionChain.Chain != null)
            {
                foreach (var evolvesToItem in evolutionChain.Chain.EvolvesTo)
                {
                    if (evolvesToItem.Species != null)
                    {
                        speciesNames.Add(evolvesToItem.Species.Name);
                    }

                    if (evolvesToItem.EvolvesTo != null)
                    {
                        GetSpeciesNamesRecursive(evolvesToItem, speciesNames);
                    }
                }
            }

            return speciesNames;
        }
        private static void GetSpeciesNamesRecursive(EvolutionChainEvolvesToItems evolvesToItem, List<string> speciesNames)
        {
            foreach (var nextEvolvesToItem in evolvesToItem.EvolvesTo)
            {
                if (nextEvolvesToItem.Species != null)
                {
                    speciesNames.Add(nextEvolvesToItem.Species.Name);
                }

                if (nextEvolvesToItem.EvolvesTo != null)
                {
                    GetSpeciesNamesRecursive(nextEvolvesToItem, speciesNames);
                }
            }
        }

        public async Task<List<Pokemon>> Get10RandomPokemonsAsync()
        {
            try
            {
                var randomIds = await GetRandomIdsAsync(10);

                if (!randomIds.Any())
                    return new List<Pokemon>();

                var pokemons = new List<Pokemon>();

                // Fetch Pokemon data in parallel
                var tasks = randomIds.Select(async randomId =>
                {
                    var pokemon = await GetPokemonAsync(randomId.ToString());
                    if (pokemon != null)
                    {
                        lock (pokemons)
                        {
                            pokemons.Add(pokemon);
                        }
                    }
                });

                await Task.WhenAll(tasks);

                return pokemons;
            }
            catch (Exception)
            {
                return new List<Pokemon>();
            }
        }
    }
}
