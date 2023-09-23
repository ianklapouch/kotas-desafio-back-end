using Newtonsoft.Json;

namespace kotas_desafio_back_end.Models
{
    public class PokemonSpecies
    {
        [JsonProperty("evolution_chain")]
        public PokemonSpeciesEvolutionChain EvolutionChain { get; set; }
    }

    public class PokemonSpeciesEvolutionChain
    {
        public string Url { get; set; }
    }
}
