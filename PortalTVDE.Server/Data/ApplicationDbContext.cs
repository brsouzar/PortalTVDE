
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalTVDE.Server.Models;








namespace PortalTVDE.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        // DbSets (Tabelas) para as Entidades de Domínio
        public DbSet<Mediator> Mediators { get; set; }
        public DbSet<Clientt> Clients { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<CoverageItem> CoverageItems { get; set; }
        public DbSet<Policy> Policies { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Mediator)
            .WithMany(m => m.Users)
            .HasForeignKey(u => u.MediatorId) // A FK é o MediatorId
            .IsRequired(false) // Permite que MediatorId seja nulo
            .OnDelete(DeleteBehavior.Restrict); // Evita a deleção em cascata (opcional, mas boa prática)



            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property("RowVersion")
                        .IsRowVersion()
                        .ValueGeneratedOnAddOrUpdate();
                }
            }

            // Configurações e Índices Únicos (Unique Indexes)

            // Client.NIF (Índice Único)
            modelBuilder.Entity<Clientt>()
                .HasIndex(c => c.NIF)
                .IsUnique();

            // Mediator.Email (Índice Único)
            modelBuilder.Entity<Mediator>()
                .HasIndex(m => m.Email)
                .IsUnique();

            // Vehicle.LicensePlate (Índice Único)
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.LicensePlate)
                .IsUnique();

            // Quote.Number (Índice Único)
            modelBuilder.Entity<Quote>()
                .HasIndex(q => q.Number)
                .IsUnique();

            // Policy.PolicyNumber (Índice Único)
            modelBuilder.Entity<Policy>()
                .HasIndex(p => p.PolicyNumber)
                .IsUnique();

            modelBuilder.Entity<Quote>()
                .Property(q => q.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Policy>()
                .Property(p => p.IssuedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
