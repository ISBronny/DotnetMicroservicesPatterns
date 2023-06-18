using Outbox.Application.DTOs;
using Outbox.Domain.Handlers;
using Outbox.Domain.Models;

namespace Outbox.Application.Services;

public interface IOrderService
{
	public Task<CreateOrder.CreateOrderResponse> CreateOrder(CreateOrderRequestDto requestDto);
	
	public Task<Order?> GetOrderById(Guid guid);
}