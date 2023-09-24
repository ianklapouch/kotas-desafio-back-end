using System.ComponentModel.DataAnnotations;

namespace kotas_desafio_back_end.Models
{
    public class PokemonMasterCreate 
    {
        [Required]
        public required string Nome { get; set; }
        [Required]
        public int Idade { get; set; }
        [Required]
        public required string Cpf { get; set; }
    }
}
