using System.ComponentModel.DataAnnotations;

namespace kotas_desafio_back_end.Models
{
    public class PokemonMaster
    {
        [Key]
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public int Idade { get; set; }
        [StringLength(11)]
        public required string Cpf { get; set; }
        public List<CapturedPokemon> CapturedPokemons { get; set; } = new();
    }
    public class CapturedPokemon
    {
        [Key]
        public Guid Id { get; set; }
        public int PokemonId { get; set; }
    }
}
