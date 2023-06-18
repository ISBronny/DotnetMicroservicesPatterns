using AutoMapper;
using Outbox.Application.DTOs;
using Outbox.Domain.Handlers;

namespace Outbox.Application.AutoMapper;


public class OrdersProfile : Profile
{
	public OrdersProfile()
	{
		CreateMap<CreateOrderRequestDto, CreateOrder.CreateOrderRequest>();
	}
}