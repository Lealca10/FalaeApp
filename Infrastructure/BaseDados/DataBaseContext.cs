using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using System.Reflection;

namespace Infrastructure.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<UsuarioDomain> Usuarios { get; set; }
        public DbSet<PreferenciasUsuarioDomain> PreferenciasUsuarios { get; set; }
        public DbSet<LocalEncontroDomain> LocaisEncontro { get; set; }
        public DbSet<EncontroDomain> Encontros { get; set; }
        public DbSet<FeedbackEncontroDomain> FeedbacksEncontro { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica todas as configurações automaticamente
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configuração básica para Usuario
            modelBuilder.Entity<UsuarioDomain>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Nome).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Senha).IsRequired().HasMaxLength(255);
                entity.Property(u => u.DataCriacao).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configuração básica para PreferenciasUsuario
            modelBuilder.Entity<PreferenciasUsuarioDomain>(entity =>
            {
                entity.HasKey(p => p.Id);
                // Se tiver relação com Usuario, descomente:
                // entity.HasOne(p => p.Usuario)
                //       .WithMany()
                //       .HasForeignKey(p => p.UsuarioId)
                //       .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração básica para LocalEncontro
            modelBuilder.Entity<LocalEncontroDomain>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Nome).IsRequired().HasMaxLength(100);
                entity.Property(l => l.Endereco).IsRequired().HasMaxLength(200);
            });

            // Configuração básica para Encontro - APENAS CAMPOS QUE EXISTEM
            modelBuilder.Entity<EncontroDomain>(entity =>
            {
                entity.HasKey(e => e.Id);

                // APENAS se sua entidade EncontroDomain tiver esses campos:
                // entity.HasOne(e => e.Local)
                //       .WithMany()
                //       .HasForeignKey(e => e.LocalId)
                //       .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuração básica para FeedbackEncontro - APENAS CAMPOS QUE EXISTEM
            modelBuilder.Entity<FeedbackEncontroDomain>(entity =>
            {
                entity.HasKey(f => f.Id);

                // APENAS se sua entidade FeedbackEncontroDomain tiver esses campos:
                // entity.HasOne(f => f.Encontro)
                //       .WithMany()
                //       .HasForeignKey(f => f.EncontroId)
                //       .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Atualiza datas automaticamente para entidades que herdam de BaseEntity
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var entity = (BaseEntity)entityEntry.Entity;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.DataCriacao = DateTime.UtcNow;
                }
                entity.DataAtualizacao = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}