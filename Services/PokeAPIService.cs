using kotas_desafio_back_end.Models;
using kotas_desafio_back_end.Models.PokeAPI;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace kotas_desafio_back_end.Services
{
    public class PokeAPIService
    {
        //MemoryCache was added here to fetch the total number of pokemons only every 24 hours
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
            {
                return numberOfPokemons;
            }

            numberOfPokemons = await RefreshAndCacheNumberOfPokemonsAsync();
            return numberOfPokemons;
        }

        private async Task<int> RefreshAndCacheNumberOfPokemonsAsync()
        {
            try
            {
                string url = "https://pokeapi.co/api/v2/pokemon-species/?limit=0";
                string response = await _httpClient.GetStringAsync(url);
                var jsonResponse = JObject.Parse(response);
                int numberOfPokemons = jsonResponse.Value<int>("count");

                MemoryCacheEntryOptions cacheEntryOptions = new()
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
            int numberOfPokemons = await GetNumberOfPokemonsAsync();

            if (numberOfPokemons == 0)
            {
                return new List<int>();
            }

            Random random = new();
            List<int> randomIds = new();

            while (randomIds.Count < count)
            {
                //+1 because the next function that generates a random value within the range includes the minimum value and excludes the maximum
                int randomId = random.Next(1, numberOfPokemons + 1);

                if (!randomIds.Contains(randomId))
                {
                    randomIds.Add(randomId);
                }
            }

            return randomIds;
        }

        public async Task<Pokemon?> GetPokemonAsync(string idOrName)
        {
            try
            {
                string url = $"https://pokeapi.co/api/v2/pokemon/{idOrName}/";
                string response = await _httpClient.GetStringAsync(url);
                RawPokemonData? rawPokemonData = JsonConvert.DeserializeObject<RawPokemonData>(response);

                if (rawPokemonData is null)
                {
                    return null;
                }

                List<string> evolutions = await GetEvolutionChainAsync(rawPokemonData.Species.Url);
                Pokemon pokemon = await RawPokemonDataToPokemonAsync(rawPokemonData);

                bool evolutionsHasBasePokemon = evolutions.Any(e => e == pokemon.Name);
                if (evolutionsHasBasePokemon)
                {
                    evolutions.Remove(pokemon.Name);
                }

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

            string Base64Sprite = string.Empty;
            if (!string.IsNullOrEmpty(rawPokemonData.Sprites.FrontDefault))
            {
                string spriteUrl = rawPokemonData.Sprites.FrontDefault;

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
            string response = await _httpClient.GetStringAsync(url);

            if (string.IsNullOrEmpty(response))
            {
                return new List<string>();
            }

            PokemonSpecies? pokemonSpecies = JsonConvert.DeserializeObject<PokemonSpecies>(response);

            if (pokemonSpecies is null || pokemonSpecies.EvolutionChain is null)
            {
                return new List<string>();
            }

            response = await _httpClient.GetStringAsync(pokemonSpecies.EvolutionChain.Url);

            if (string.IsNullOrEmpty(response))
            {
                return new List<string>();
            }

            EvolutionChain? evolutionChain = JsonConvert.DeserializeObject<EvolutionChain>(response);

            if (evolutionChain is null || evolutionChain.Chain is null)
            {
                return new List<string>();
            }

            List<string> species = GetSpeciesNames(evolutionChain);
            return species;
        }
        public List<string> GetSpeciesNames(EvolutionChain evolutionChain)
        {
            List<string> speciesNames = new();
            CollectSpeciesNamesRecursively(evolutionChain?.Chain?.EvolvesTo, speciesNames);
            return speciesNames;
        }

        private void CollectSpeciesNamesRecursively(IEnumerable<EvolutionChainEvolvesTo>? evolvesToItems, List<string> speciesNames)
        {
            if (evolvesToItems is null)
            {
                return;
            }

            foreach (var evolvesToItem in evolvesToItems)
            {
                if (evolvesToItem.Species is not null)
                {
                    speciesNames.Add(evolvesToItem.Species.Name);
                }

                CollectSpeciesNamesRecursively(evolvesToItem.EvolvesTo, speciesNames);
            }
        }

        public async Task<List<Pokemon>> GetRandomPokemonsAsync()
        {
            try
            {
                List<int> randomIds = await GetRandomIdsAsync(10);

                if (!randomIds.Any())
                {
                    return new List<Pokemon>();
                }

                List<Pokemon> pokemons = new();

                // Fetch Pokemon data in parallel
                ParallelOptions options = new(){ MaxDegreeOfParallelism = 10 }; //configuring to allow 10 tasks in parallel
                var tasks = randomIds.Select(async randomId =>
                {
                    Pokemon? pokemon = await GetPokemonAsync(randomId.ToString());
                    if (pokemon is not null)
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
