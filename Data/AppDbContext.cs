using kotas_desafio_back_end.Models;
using Microsoft.EntityFrameworkCore;

namespace kotas_desafio_back_end.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}
        public DbSet<User> Users { get; set; }
    }
}
