namespace Outbox.Domain.Models;

public enum OrderState
{
	Created,
	Confirmed,
	InDelivery,
	Done,
}