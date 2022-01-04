using api.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Reflection;

namespace api.Data
{
    public class Context : DbContext
    {
        public DbSet<Trainer> Trainers;
        public DbSet<CapturedPokemon> Pokemons;

        public Context()
        {
            StartConnection();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Data.db", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trainer>().ToTable("Trainers");
            modelBuilder.Entity<Trainer>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.HasMany<CapturedPokemon>(e => e.CapturedPokemons).WithOne();
                entity.Property(t => t.CreatAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Navigation(e => e.CapturedPokemons)
                    .UsePropertyAccessMode(PropertyAccessMode.Property);
                
            });

            modelBuilder.Entity<CapturedPokemon>(entity =>
            {
                entity.HasOne(e => e.Trainer)
                    .WithMany(t => t.CapturedPokemons)
                    .HasForeignKey(e => e.TrainerId);
            });


            base.OnModelCreating(modelBuilder);
        }

        private void StartConnection()
        {
            try
            {
                if (!File.Exists("Data.db"))
                    File.Create("Data.db");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
