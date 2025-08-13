using System.Security.Claims;
using MediatR;
using TicketingApp.Domain.Entities;
using TicketingApp.Domain.Enums;
using TicketingApp.Domain.Interfaces;
using TicketingApp.Application.DTOs;

namespace TicketingApp.Application.Tickets.Commands;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketDto>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _ticketRepository = ticketRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var ticket = new Ticket
        {
            Title = request.Title,
            Description = request.Description,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        await _ticketRepository.AddTicketAsync(ticket);

        return new TicketDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status.ToString(),
            CreatedAt = ticket.CreatedAt,
            CreatedBy = ticket.CreatedBy ?? string.Empty
        };
    }
}
