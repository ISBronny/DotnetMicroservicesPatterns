namespace Outbox.Application.DTOs;

public class CreateOrderRequestDto
{
	public Guid CustomerId { get; set; }
	public Guid MangerId { get; set; }
	public double Price { get; set; }
}