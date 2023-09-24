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
            //modelBuilder.Entity<PokemonPokemonMaster>()
            //    .HasKey(ppm => new { ppm.PokemonMasterId, ppm.PokemonId });

            //modelBuilder.Entity<Pokemon>().HasMany(pm => pm.PokemonMasters);
            modelBuilder.Entity<PokemonMaster>().HasMany(p => p.CapturedPokemons);


            //modelBuilder.Entity<PokemonPokemonMaster>()
            //    .HasOne(ppm => ppm.PokemonMaster)
            //    .WithMany()
            //    .HasForeignKey(ppm => ppm.PokemonMasterId);

            //modelBuilder.Entity<PokemonPokemonMaster>()
            //    .HasOne(ppm => ppm.Pokemon)
            //    .WithMany()
            //    .HasForeignKey(ppm => ppm.PokemonId);
        }
    }
}
