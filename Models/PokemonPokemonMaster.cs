namespace kotas_desafio_back_end.Models
{
        public class PokemonPokemonMaster
        {
            public Guid PokemonMasterId { get; set; }
            public PokemonMaster PokemonMaster { get; set; }

            public int PokemonId { get; set; }
            public Pokemon Pokemon { get; set; }
        }
}
