using kotas_desafio_back_end.Models;
using Microsoft.EntityFrameworkCore;

namespace kotas_desafio_back_end.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}
        public DbSet<PokemonMaster> PokemonMasters { get; set; }
        public DbSet<CapturedPokemon> CapturedPokemons { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PokemonMaster>().HasMany(pm => pm.CapturedPokemons);
            modelBuilder.Entity<PokemonMaster>().HasIndex(pm => pm.Cpf).IsUnique();
        }
    }
}
