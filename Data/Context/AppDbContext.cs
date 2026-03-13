using joker_api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace joker_api.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<JokeEntity> Jokes { get; set; }
}