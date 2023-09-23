namespace kotas_desafio_back_end.Models
{
    public class Pokemon
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Base64Sprite { get; set; }
        public required List<string> Types { get; set; }
        public List<string>? Evolutions { get; set; } 
    }
}
