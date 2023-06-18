namespace Outbox.Domain.Models;

public class Outbox
{
	public string EventType { get; set; }
	public DateTime CreatedAt { get; set; }
}