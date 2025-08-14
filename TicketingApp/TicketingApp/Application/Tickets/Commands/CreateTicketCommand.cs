using MediatR;
using TicketingApp.Application.DTOs;

namespace TicketingApp.Application.Tickets.Commands;

public class CreateTicketCommand : IRequest<TicketDto>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

}