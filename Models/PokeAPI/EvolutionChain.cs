using Newtonsoft.Json;

namespace kotas_desafio_back_end.Models.PokeAPI
{
    public class EvolutionChain
    {
        public required EvolutionChainChain Chain { get; set; }
    }

    public class EvolutionChainChain
    {
        [JsonProperty("evolves_to")]
        public List<EvolutionChainEvolvesTo>? EvolvesTo { get; set; }
    }

    public class EvolutionChainEvolvesTo
    {
        public required EvolutionChainSpecies Species { get; set; }
        [JsonProperty("evolves_to")]
        public List<EvolutionChainEvolvesTo>? EvolvesTo { get; set; }
    }

    public class EvolutionChainSpecies
    {
        public required string Name { get; set; }
        public required string Url { get; set; }
    }
}
