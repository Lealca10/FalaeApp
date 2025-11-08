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

            // UsuarioDomain
            modelBuilder.Entity<UsuarioDomain>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).HasMaxLength(36);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Cpf).IsUnique();
                entity.Property(u => u.Senha)
                   .IsRequired() 
                   .HasMaxLength(255);

                // 1:1 com Preferencias
                entity.HasOne(u => u.Preferencias)
                      .WithOne(p => p.Usuario)
                      .HasForeignKey<PreferenciasUsuarioDomain>(p => p.UsuarioId);

                // M:N com Encontros
                entity.HasMany(u => u.Encontros)
                      .WithMany(e => e.Participantes)
                      .UsingEntity<Dictionary<string, object>>(
                          "UsuarioEncontro",
                          j => j.HasOne<EncontroDomain>().WithMany().HasForeignKey("EncontroId"),
                          j => j.HasOne<UsuarioDomain>().WithMany().HasForeignKey("UsuarioId")
                      );
            });

            // PreferenciasUsuarioDomain
            modelBuilder.Entity<PreferenciasUsuarioDomain>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).HasMaxLength(36);
                entity.Property(p => p.UsuarioId).HasMaxLength(36);
                entity.Property(p => p.IdiomaPreferido).HasMaxLength(20);
                entity.Property(p => p.InvestimentoEncontro).HasMaxLength(20);
            });

            // LocalEncontroDomain
            modelBuilder.Entity<LocalEncontroDomain>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Id).HasMaxLength(36);

                entity.Property(l => l.ImagemUrl).HasMaxLength(500);
            });

            // EncontroDomain
            modelBuilder.Entity<EncontroDomain>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(36);
                entity.Property(e => e.LocalId).HasMaxLength(36);

                entity.HasOne(e => e.Local)
                      .WithMany(l => l.Encontros)
                      .HasForeignKey(e => e.LocalId);
            });

            // FeedbackEncontroDomain
            modelBuilder.Entity<FeedbackEncontroDomain>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Id).HasMaxLength(36);
                entity.Property(f => f.EncontroId).HasMaxLength(36);
                entity.Property(f => f.UsuarioId).HasMaxLength(36);

                entity.HasOne(f => f.Encontro)
                      .WithMany(e => e.Feedbacks)
                      .HasForeignKey(f => f.EncontroId);

                entity.HasOne(f => f.Usuario)
                      .WithMany(u => u.Feedbacks)
                      .HasForeignKey(f => f.UsuarioId);
            });
        }
    }
}