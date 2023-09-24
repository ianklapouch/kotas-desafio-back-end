using System.ComponentModel.DataAnnotations;

namespace kotas_desafio_back_end.Models
{
    public class CatchPokemon
    {
        [Required]
        public Guid PokemonMasterId { get; set; }
        [Required]
        public required string PokemonIdOrName { get; set; }
    }
}
