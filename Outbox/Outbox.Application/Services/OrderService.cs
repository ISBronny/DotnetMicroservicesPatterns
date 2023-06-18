using AutoMapper;
using MediatR;
using Outbox.Application.DTOs;
using Outbox.Domain.Handlers;
using Outbox.Domain.Interfaces;
using Outbox.Domain.Models;

namespace Outbox.Application.Services;

public class OrderService : IOrderService
{
	private IMediator _mediator;
	private IMapper _mapper;
	private IOrderRepository _repository;

	public OrderService(IMediator mediator, IMapper mapper, IOrderRepository repository)
	{
		_mediator = mediator;
		_mapper = mapper;
		_repository = repository;
	}

	public Task<CreateOrder.CreateOrderResponse> CreateOrder(CreateOrderRequestDto requestDto)
	{
		var command = _mapper.Map<CreateOrder.CreateOrderRequest>(requestDto);
		return _mediator.Send(command);
	}

	public Task<Order?> GetOrderById(Guid guid)
	{
		return _repository.GetById(guid);
	}
}