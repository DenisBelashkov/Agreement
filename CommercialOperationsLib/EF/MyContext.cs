using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.EF
{
    public class MyContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Street> Streets { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryAgreement> CategoryAgreements { get; set; }
        
        public MyContext(DbContextOptions<MyContext> options):base(options)
        { 
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies()
              //  .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=mydb;Trusted_Connection=True;")
                ;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().HasAlternateKey(c => new { c.Name });
            modelBuilder.Entity<City>().HasAlternateKey(city => new { city.CountryId, city.Name });
            modelBuilder.Entity<Street>().HasAlternateKey(st => new { st.CityId, st.Name });
            modelBuilder.Entity<Building>().HasAlternateKey(b => new { b.StreetId, b.Name });

            modelBuilder.Entity<Category>().HasIndex(i => i.Name).IsUnique();
            modelBuilder.Entity<UserType>().HasIndex(i => i.Type).IsUnique();

            modelBuilder.Entity<Agreement>()
                .HasMany(agr => agr.Categories)
                .WithMany(cat => cat.Agreements)
                .UsingEntity<CategoryAgreement>(
                    i => i
                        .HasOne(catit => catit.Category)
                        .WithMany(cat => cat.CategoryAgreement)
                        .HasForeignKey(catit => catit.CategoryId),
                    i => i
                        .HasOne(catit => catit.Agreement)
                        .WithMany(agr => agr.CategoryAgreement)
                        .HasForeignKey(catit => catit.AgreementId),
                    i => i.HasKey(t => new { t.CategoryId, t.AgreementId})
                );

            modelBuilder.Entity<Agreement>()
                .HasOne(op => op.CustomerUser)
                .WithMany(t => t.OrderAgreements)
                .HasForeignKey(t => t.CustomerUserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Agreement>()
                .HasOne(op => op.ContractorUser)
                .WithMany(t => t.ContractAgreements)
                .HasForeignKey(t => t.ContractorUserId)
                .OnDelete(DeleteBehavior.ClientCascade);
            

        }
    }
}
