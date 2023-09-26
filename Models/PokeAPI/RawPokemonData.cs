using Newtonsoft.Json;

namespace kotas_desafio_back_end.Models.PokeAPI
{
    public class RawPokemonData
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public Species Species { get; set; }
        public required Sprites Sprites { get; set; }
        public required List<Types> Types { get; set; }
    }

    public class Species
    {
        public string Url { get; set; }
    }

    public class Sprites
    {
        [JsonProperty("front_default")]
        public required string FrontDefault { get; set; }
    }

    public class Types
    {
        public int Slot { get; set; }
        public Type Type { get; set; }
    }

    public class Type
    {
        public required string Name { get; set; }
        public required string Url { get; set; }

    }

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
        public string Url { get; set; }
    }
}
