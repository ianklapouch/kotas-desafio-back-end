using kotas_desafio_back_end.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace kotas_desafio_back_end.Services
{
    public class PokeAPIService
    {
        /*
            Cache here was added because we don't need to fetch the total number of pokemons all the time, 
            it's a heavy request and doesn't change constantly, so it was implemented to store the value 
            in the memory cache for 24 hours before fetching it again.

            Average time of request to fetch random top 10 after implementation dropped by around 4 seconds.
            Going from 14 seconds to 10.
        */
        private readonly IMemoryCache cache;
        public PokeAPIService(IMemoryCache cache)
        {
            this.cache = cache;
        }
        public async Task<int> GetNumberOfPokemons()
        {
            if (cache.TryGetValue<int>("NumberOfPokemons", out var numberOfPokemons))
            {
                return numberOfPokemons;
            }
            else
            {
                return await RefreshAndCacheNumberOfPokemons();
            }
        }
        private async Task<int> RefreshAndCacheNumberOfPokemons()
        {
            try
            {
                string url = "https://pokeapi.co/api/v2/pokemon-species/?limit=0";
                using HttpClient httpClient = new();
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject jsonResponse = JObject.Parse(responseBody);
                    int numberOfPokemons = (int)jsonResponse["count"];

                    //Storing the value in memory cache in for 24 hours
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                    };
                    cache.Set("NumberOfPokemons", numberOfPokemons, cacheEntryOptions);

                    return numberOfPokemons;
                }

                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        private async Task<List<int>> GetRandomIds()
        {
            int numberOfPokemons = await GetNumberOfPokemons();

            if (numberOfPokemons == 0)
            {
                return new List<int>();
            }

            Random random = new();
            List<int> randomIds = new();

            while (randomIds.Count < 10)
            {
                int randomId = random.Next(1, numberOfPokemons);

                if (!randomIds.Contains(randomId))
                {
                    randomIds.Add(randomId);
                }
            }

            return randomIds;
        }
        public async Task<List<RawPokemonData>> Get10RandomPokemons()
        {
            try
            {
                List<int> randomIds = await GetRandomIds();

                if (randomIds.Count == 0)
                {
                    return new List<RawPokemonData>();
                }

                List<RawPokemonData> pokemons = new();
                HttpClient httpClient = new();

                /*
                    This implementation was added to make requests in parallel instead of waiting for each request, iterating one by one, deserialize and add to the list.
                    
                    Average time of request to fetch random top 10 after implementation dropped by around 9 seconds.
                    Going from 11 seconds to 2.
                */
                var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };

                var tasks = randomIds.Select(async randomId =>
                {
                    string url = $"https://pokeapi.co/api/v2/pokemon/{randomId}/";

                    try
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();
                            RawPokemonData pokemon = JsonConvert.DeserializeObject<RawPokemonData>(responseBody);
                            if (pokemon is not null)
                            {
                                lock (pokemons)
                                {
                                    pokemons.Add(pokemon);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

                await Task.WhenAll(tasks);

                return pokemons;
            }
            catch (Exception)
            {
                return new List<RawPokemonData>();
            }
        }
        public async Task<RawPokemonData?> GetPokemon(string idOrName)
        {
            try
            {

                string url = $"https://pokeapi.co/api/v2/pokemon/{idOrName}/";
                using HttpClient httpClient = new();
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    RawPokemonData pokemon = JsonConvert.DeserializeObject<RawPokemonData>(responseBody);
                    if (pokemon is not null)
                    {
                        return pokemon;
                    }
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
