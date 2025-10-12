using Microsoft.EntityFrameworkCore;
using Domain.Entities;

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

            // Configuração para UsuarioDomain
            modelBuilder.Entity<UsuarioDomain>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Nome).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Senha).IsRequired().HasMaxLength(255);

                // Adicione outras propriedades que existem na sua classe UsuarioDomain
            });

            // Configuração para PreferenciasUsuarioDomain
            modelBuilder.Entity<PreferenciasUsuarioDomain>(entity =>
            {
                entity.HasKey(p => p.Id);

                // Se tiver relação com usuário, descomente:
                // entity.HasOne(p => p.Usuario)
                //       .WithOne()
                //       .HasForeignKey<PreferenciasUsuarioDomain>(p => p.UsuarioId);
            });

            // Configuração para LocalEncontroDomain
            modelBuilder.Entity<LocalEncontroDomain>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Nome).IsRequired().HasMaxLength(100);
                entity.Property(l => l.Endereco).IsRequired().HasMaxLength(200);
                entity.Property(l => l.DataCriacao).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configuração para EncontroDomain
            modelBuilder.Entity<EncontroDomain>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DataCriacao).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure APENAS as propriedades que existem na sua classe EncontroDomain
                // Exemplo:
                // entity.Property(e => e.DataEncontro).IsRequired();
                // entity.Property(e => e.Status).HasMaxLength(50);
            });

            // Configuração para FeedbackEncontroDomain
            modelBuilder.Entity<FeedbackEncontroDomain>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.DataCriacao).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure APENAS as propriedades que existem na sua classe FeedbackEncontroDomain
                // Exemplo:
                // entity.Property(f => f.Comentario).HasMaxLength(500);
                // entity.Property(f => f.Nota).IsRequired();
            });
        }
    }
}