namespace Gurko.Persistence;

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Subscriber> Subscribers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=../gurko.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscriber>().HasData(new Subscriber
        {
            Id = 1,
            Name = "subscriber",
            SubscriberId = new Guid("ba5c7671-8a7b-4eed-a4bd-8e33d1f724dd")
        });
        
        base.OnModelCreating(modelBuilder);
    }
}