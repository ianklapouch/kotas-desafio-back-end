using System.ComponentModel.DataAnnotations;

namespace kotas_desafio_back_end.Models
{
    public class PokemonMasterCreate 
    {
        [Required]
        public required string Nome { get; set; }
        public int Idade { get; set; }
        [Required]
        [MaxLength(11)]
        public required string Cpf { get; set; }
    }
}
