using Newtonsoft.Json;

namespace kotas_desafio_back_end.Models.PokeAPI
{
    public class PokemonSpecies
    {
        [JsonProperty("evolution_chain")]
        public required PokemonSpeciesEvolutionChain EvolutionChain { get; set; }
    }

    public class PokemonSpeciesEvolutionChain
    {
        public required string Url { get; set; }
    }
}
