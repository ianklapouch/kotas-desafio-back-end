using Newtonsoft.Json;

namespace kotas_desafio_back_end.Models.PokeAPI
{
    public class RawPokemonData
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required RawPokemonDataSpecies Species { get; set; }
        public required RawPokemonDataSprites Sprites { get; set; }
        public required List<RawPokemonDataTypes> Types { get; set; }
    }

    public class RawPokemonDataSpecies
    {
        public required string Url { get; set; }
    }

    public class RawPokemonDataSprites
    {
        [JsonProperty("front_default")]
        public required string FrontDefault { get; set; }
    }

    public class RawPokemonDataTypes
    {
        public int Slot { get; set; }
        public required RawPokemonDataType Type { get; set; }
    }

    public class RawPokemonDataType
    {
        public required string Name { get; set; }
        public required string Url { get; set; }
    }
}
