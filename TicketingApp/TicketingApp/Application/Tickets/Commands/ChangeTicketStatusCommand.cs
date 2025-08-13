using MediatR;
using TicketingApp.Domain.Enums;

namespace TicketingApp.Application.Tickets.Commands;

public class ChangeTicketStatusCommand : IRequest<Unit>
{
    public int TicketId { get; set; }
    public TicketStatus NewStatus { get; set; }
}