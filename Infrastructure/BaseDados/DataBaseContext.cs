using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.BaseDados;
using System.Reflection.Emit;

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
            // Configurações do Usuario
            modelBuilder.Entity<UsuarioDomain>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Cpf).IsUnique();

                entity.HasOne(u => u.Preferencias)
                      .WithOne(p => p.Usuario)
                      .HasForeignKey<PreferenciasUsuario>(p => p.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurações do PreferenciasUsuario
            modelBuilder.Entity<PreferenciasUsuarioDomain>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.UsuarioId).IsUnique();
            });

            // Configurações do LocalEncontro
            modelBuilder.Entity<LocalEncontroDomain>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.HasIndex(l => l.Nome);
            });

            // Configurações do Encontro
            modelBuilder.Entity<EncontroDomain>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Local)
                      .WithMany(l => l.Encontros)
                      .HasForeignKey(e => e.LocalId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Participantes)
                      .WithMany(u => u.Encontros)
                      .UsingEntity(j => j.ToTable("EncontroParticipantes"));
            });

            // Configurações do FeedbackEncontro
            modelBuilder.Entity<FeedbackEncontroDomain>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.HasOne(f => f.Encontro)
                      .WithMany(e => e.Feedbacks)
                      .HasForeignKey(f => f.EncontroId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(f => f.Usuario)
                      .WithMany(u => u.Feedbacks)
                      .HasForeignKey(f => f.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
