using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketingApp.Domain.Entities;

namespace TicketingApp.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {}

    public DbSet<Ticket> Tickets => Set<Ticket>();
}

