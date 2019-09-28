namespace SamuraiDotCore.Data
{
    using Microsoft.EntityFrameworkCore;

    using SamuraiDotCore.Domain;

    public class SamuraiContext : DbContext
    {
        public DbSet<Samurai> Samurais { get; set; }

        public DbSet<Quote> Quotes { get; set; }

        public DbSet<Battle> Battles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = SamuraiDb; Trusted_Connection = True; ");
           //NO OP base.OnConfiguring(optionsBuilder);
        }
    }


}
