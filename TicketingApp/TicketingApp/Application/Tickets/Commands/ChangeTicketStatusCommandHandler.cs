using MediatR;
using TicketingApp.Domain.Interfaces;

namespace TicketingApp.Application.Tickets.Commands;

public class ChangeTicketStatusCommandHandler : IRequestHandler<ChangeTicketStatusCommand, Unit>
{
    private readonly ITicketRepository _ticketRepository;

    public ChangeTicketStatusCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Unit> Handle(ChangeTicketStatusCommand request, CancellationToken cancellationToken)
    {
        await _ticketRepository.UpdateTicketStatusAsync(request.TicketId, request.NewStatus);
        return Unit.Value;
    }
}