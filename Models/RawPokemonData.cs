namespace kotas_desafio_back_end.Models
{
    //This object is made to receive responses from the PokéAPI.
    public class RawPokemonData
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public required PokemonSprites Sprites { get; set; }
        public required List<PokemonTypes> Types { get; set; }
    }
}
