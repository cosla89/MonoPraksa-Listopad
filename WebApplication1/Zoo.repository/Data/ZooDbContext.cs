using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zoo.model;

namespace Zoo.repository.Data
{
    public class ZooDbContext : DbContext
    {
        public ZooDbContext(DbContextOptions<ZooDbContext> options)
            : base(options)
        {
        }

        public DbSet<Animal> Animals => Set<Animal>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var animal = modelBuilder.Entity<Animal>();

            animal.ToTable("animals", "public");

            animal.HasKey(a => a.Id);

            animal.Property(a => a.Id)
                .HasColumnName("id")
                .UseIdentityAlwaysColumn();

            animal.Property(a => a.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            animal.Property(a => a.Species)
                .HasColumnName("species")
                .HasMaxLength(100)
                .IsRequired();

            animal.Property(a => a.Age)
                .HasColumnName("age")
                .IsRequired();
        }
    }
}
