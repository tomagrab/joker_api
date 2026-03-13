using joker_api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace joker_api.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<JokeEntity> Jokes { get; set; }
    public DbSet<UserPreferencesEntity> UserPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserPreferencesEntity>(entity =>
        {
            entity.HasKey(item => item.UserId);
            entity.Property(item => item.UserId)
                .HasMaxLength(450);
            entity.Property(item => item.PreferencesJson)
                .IsRequired();
            entity.Property(item => item.UpdatedAtUtc)
                .HasColumnType("datetimeoffset");
        });
    }
}