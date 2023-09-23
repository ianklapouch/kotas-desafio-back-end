using Newtonsoft.Json;

namespace kotas_desafio_back_end.Models
{
    public class EvolutionChain
    {
        public EvolutionChainChain Chain { get; set; }
    }

    public class EvolutionChainChain
    {
        [JsonProperty("evolves_to")]
        public List<EvolutionChainEvolvesToItems> EvolvesTo { get; set; }
    }

    public class EvolutionChainEvolvesToItems
    {
        public EvolutionChainSpecies Species { get; set; }
        [JsonProperty("evolves_to")]
        public List<EvolutionChainEvolvesToItems> EvolvesTo { get; set; }
    }

    public class EvolutionChainSpecies
    {
        public string Name { get; set; }
        public string Url{ get; set; }
    }
}
