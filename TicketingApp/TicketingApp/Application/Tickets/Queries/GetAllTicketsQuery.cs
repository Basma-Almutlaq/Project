using MediatR;
using TicketingApp.Application.Common.Models;
using TicketingApp.Application.DTOs;

namespace TicketingApp.Application.Tickets.Queries;

public class GetAllTicketsQuery : IRequest<PagedResult<TicketDto>>
{ 
    public int PageNumber { get; set; } = 1;
    public int PageSize   { get; set; } = 10;
}