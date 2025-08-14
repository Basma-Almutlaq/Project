using System.Security.Claims;
using MediatR;
using TicketingApp.Application.Common.Models;
using TicketingApp.Application.DTOs;
using TicketingApp.Domain.Interfaces;

namespace TicketingApp.Application.Tickets.Queries;

public class GetAllTicketsQueryHandler : IRequestHandler<GetAllTicketsQuery, PagedResult<TicketDto>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAllTicketsQueryHandler(ITicketRepository ticketRepository, IHttpContextAccessor httpContextAccessor)
    {
        _ticketRepository = ticketRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<TicketDto>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
{
    var httpContext = _httpContextAccessor.HttpContext!;
    var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var isAdmin = httpContext.User.IsInRole("Admin");

    var allTickets = await _ticketRepository.GetAllTicketsAsync();

    if (!isAdmin)
    {
        allTickets = allTickets.Where(t => t.CreatedBy == userId).ToList();
    }

    // --- pagination ---
    var count = allTickets.Count;
    var totalPages = (int)Math.Ceiling(count / (double)request.PageSize);
    var items = allTickets
        .Skip((request.PageNumber - 1) * request.PageSize)
        .Take(request.PageSize)
        .Select(t => new TicketDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status.ToString(),
            CreatedAt = t.CreatedAt,
            CreatedBy = t.CreatedBy?? string.Empty
        })
        .ToList();

    return new PagedResult<TicketDto>
    {
        CurrentPage = request.PageNumber,
        TotalPages  = totalPages,
        PageSize    = request.PageSize,
        TotalCount  = count,
        Items       = items
    };
}

}
