using System.ComponentModel.DataAnnotations;

namespace kotas_desafio_back_end.Models
{
    public class PokemonMaster
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public int Idade { get; set; }
        public required string Cpf { get; set; }
        public List<CapturedPokemon>? CapturedPokemons { get; set; } 
    }

    public class CapturedPokemon
    {
        [Key]
        public Guid Id { get; set; }
        public int PokemonId { get; set; }
    }
}
