using TicketingApp.Domain.Entities;
using TicketingApp.Domain.Enums;

namespace TicketingApp.Domain.Interfaces;

public interface ITicketRepository
{
    Task<List<Ticket>> GetAllTicketsAsync();
    Task<Ticket?> GetTicketByIdAsync(int id);
    Task AddTicketAsync(Ticket ticket);
    Task UpdateTicketStatusAsync(int id, TicketStatus status);
}