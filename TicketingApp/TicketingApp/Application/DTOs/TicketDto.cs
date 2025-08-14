namespace TicketingApp.Application.DTOs;

public class TicketDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}