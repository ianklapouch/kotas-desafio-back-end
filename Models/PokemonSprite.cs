using Newtonsoft.Json;

namespace kotas_desafio_back_end.Models
{
    public class PokemonSprites
    {
        [JsonProperty("front_default")]
        public required string FrontDefault { get; set; }
    }
}
