using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kotas_desafio_back_end.Models
{
    public class Pokemon
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Base64Sprite { get; set; } 
        public required List<string> Types { get; set; }
        public List<string>? Evolutions { get; set; }
    }
}
