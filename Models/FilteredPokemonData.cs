namespace kotas_desafio_back_end.Models
{
    //This object is made to process RawPokemonData data and return only what is necessary.
    public class FilteredPokemonData
    {
        public required string Name { get; set; }
        public required string Base64Sprite { get; set; }
    }
}
