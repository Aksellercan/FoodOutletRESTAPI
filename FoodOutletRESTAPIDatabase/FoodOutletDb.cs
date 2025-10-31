using FoodOutletRESTAPIDatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOutletRESTAPIDatabase
{
    public class FoodOutletDb(DbContextOptions<FoodOutletDb> options) : DbContext(options)
    {
        public DbSet<FoodOutlet> FoodOutlets => Set<FoodOutlet>();
        public DbSet<Review> Reviews => Set<Review>();

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure FoodOutlet entity
            modelBuilder.Entity<FoodOutlet>(entity =>
            {
                entity.HasKey(f => f.Id); // Primary key
                entity.Property(f => f.Name).IsRequired().HasMaxLength(64);
                entity.Property(f => f.Location).IsRequired().HasMaxLength(256);

                // Define one-to-many relationship with Review
                entity.HasMany(f => f.Reviews)
                    .WithOne(r => r.FoodOutlet)
                    .HasForeignKey(r => r.FoodOutletId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Review entity
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id); // Primary key
                entity.Property(r => r.Comment).HasColumnType("TEXT").HasMaxLength(512);
                entity.Property(r => r.Score).IsRequired(); // Rating must be provided
                entity.Property(r => r.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Default value for timestamps
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id); // Primary key
                entity.Property(u => u.Username).IsRequired().HasMaxLength(64);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(256);
                // Define one-to-many relationship with Review
                entity.HasMany(u => u.Reviews)
                    .WithOne(r => r.User)
                    .IsRequired()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
