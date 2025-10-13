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
                entity.Property(u => u.Id).HasMaxLength(36);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Cpf).IsUnique();

                // Relacionamento 1:1 com Preferencias
                entity.HasOne(u => u.Preferencias)
                      .WithOne(p => p.Usuario)
                      .HasForeignKey<PreferenciasUsuarioDomain>(p => p.UsuarioId);
            });

            // Configuração para PreferenciasUsuarioDomain
            modelBuilder.Entity<PreferenciasUsuarioDomain>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).HasMaxLength(36);
                entity.Property(p => p.UsuarioId).HasMaxLength(36);

                // Restrições para campos numéricos
                entity.Property(p => p.NivelEstresse)
                      .HasAnnotation("Range", new[] { 1, 10 });

                entity.Property(p => p.ImportanciaEspiritualidade)
                      .HasAnnotation("Range", new[] { 1, 10 });
            });

            // Configuração para LocalEncontroDomain
            modelBuilder.Entity<LocalEncontroDomain>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Id).HasMaxLength(36);
            });

            // Configuração para EncontroDomain
            modelBuilder.Entity<EncontroDomain>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(36);
                entity.Property(e => e.LocalId).HasMaxLength(36);

                // Relacionamento com Local
                entity.HasOne(e => e.Local)
                      .WithMany(l => l.Encontros)
                      .HasForeignKey(e => e.LocalId);

                // Relacionamento muitos-para-muitos com Usuarios
                entity.HasMany(e => e.Participantes)
                      .WithMany(u => u.Encontros)
                      .UsingEntity<Dictionary<string, object>>(
                          "UsuarioEncontro",
                          j => j.HasOne<UsuarioDomain>().WithMany().HasForeignKey("UsuarioId"),
                          j => j.HasOne<EncontroDomain>().WithMany().HasForeignKey("EncontroId"),
                          j =>
                          {
                              j.HasKey("UsuarioId", "EncontroId");
                          });
            });

            // Configuração para FeedbackEncontroDomain
            modelBuilder.Entity<FeedbackEncontroDomain>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Id).HasMaxLength(36);
                entity.Property(f => f.EncontroId).HasMaxLength(36);
                entity.Property(f => f.UsuarioId).HasMaxLength(36);

                // Relacionamentos
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