namespace Outbox.Domain.Events;

public class OrderCreatedEvent
{
	public Guid Id { get; set; }
	public Guid CustomerId { get; set; }
	public Guid MangerId { get; set; }
	public DateTime CreatedAt { get; set; }
}