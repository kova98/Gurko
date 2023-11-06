using Microsoft.EntityFrameworkCore;

namespace Gurko.Persistence;

public class SqliteSubscriberRepository : ISubscriberRepository
{
    private readonly AppDbContext _context;

    public SqliteSubscriberRepository(AppDbContext context)
    {
        _context = context;
        _context.Database.EnsureCreated();
    }

    public async Task<long> Create(Subscriber subscriber)
    {
        _context.Subscribers.Add(subscriber);
        await _context.SaveChangesAsync();
        return subscriber.Id; // EF Core populates the ID after saving changes.
    }

    public Task<bool> Exists(Guid subscriberId)
    {
        return _context.Subscribers.AnyAsync(s => s.SubscriberId == subscriberId);
    }

    public Task<bool> Exists(string name)
    {
        return _context.Subscribers.AnyAsync(s => s.Name == name);
    }
}
