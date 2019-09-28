namespace Transit.Data
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Runtime.Serialization;

    using Core.Common.Contracts;

    using Transit.Business.Entities;

    public class TransitDbContext : DbContext
    {
        static TransitDbContext()
        {
            Database.SetInitializer<TransitDbContext>(null);
        }
        public TransitDbContext()
            : base("Data Source=localhost\\SQLEXPRESS;Initial Catalog=TransitDB;Integrated Security=SSPI;")
        {
            Database.SetInitializer<TransitDbContext>(new CreateDatabaseIfNotExists<TransitDbContext>());
        }

        public DbSet<Account> AccountSet { get; set; }

        public DbSet<Sign> SignSet { get; set; }

        public DbSet<Stop> StopSet { get; set; }

        public DbSet<Route> RouteSet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Ignore<ExtensionDataObject>();
            modelBuilder.Ignore<IIdentifiableEntity>();

            modelBuilder.Entity<Account>().HasKey(e => e.AccountId).Ignore(e => e.EntityId);
            modelBuilder.Entity<Sign>().HasKey(e => e.SignId).Ignore(e => e.EntityId);
            modelBuilder.Entity<Stop>().HasKey(e => e.StopId).Ignore(e => e.EntityId);
            modelBuilder.Entity<Route>().HasKey(e => e.RouteId).Ignore(e => e.EntityId);
        }
    }
}