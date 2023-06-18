namespace Outbox.Domain.Models;

public class Order
{
	public Guid Id { get; set; }
	public Guid CustomerId { get; set; }
	public Guid MangerId { get; set; }
	public DateTime CreatedAt { get; set; }
	public double Price { get; set; }
	public OrderState State { get; set; }
}