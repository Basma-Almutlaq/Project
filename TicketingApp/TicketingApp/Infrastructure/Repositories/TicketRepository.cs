using TicketingApp.Domain.Entities;
using TicketingApp.Domain.Enums;
using TicketingApp.Domain.Interfaces;
using TicketingApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TicketingApp.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _context;

    public TicketRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddTicketAsync(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Ticket>> GetAllTicketsAsync()
    {
        return await _context.Tickets.ToListAsync();
    }

    public async Task<Ticket?> GetTicketByIdAsync(int id)
    {
        return await _context.Tickets.FindAsync(id);
    }

    public async Task UpdateTicketStatusAsync(int id, TicketStatus status)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket != null)
        {
            ticket.Status = status;
            await _context.SaveChangesAsync();
        }
    }
}