using Microsoft.AspNetCore.Mvc;
using MediatR;
using TicketingApp.Application.Tickets.Commands;
using TicketingApp.Application.Tickets.Queries;
using TicketingApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace TicketingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/tickets
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateTicketCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(result);
}

    // GET: api/tickets
   [HttpGet]
public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
{
    if (!ModelState.IsValid)
{
    return BadRequest(ModelState);
}

    var query = new GetAllTicketsQuery
    {
        PageNumber = pageNumber,
        PageSize = pageSize
    };

    var result = await _mediator.Send(query);
    return Ok(result);
}

    // PUT: api/tickets/{id}/status
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] TicketStatus newStatus)
    {
        var command = new ChangeTicketStatusCommand
        {
            TicketId = id,
            NewStatus = newStatus
        };

        await _mediator.Send(command);
        return NoContent();
    }
}