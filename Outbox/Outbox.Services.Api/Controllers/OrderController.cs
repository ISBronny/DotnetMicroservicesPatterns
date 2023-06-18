using Microsoft.AspNetCore.Mvc;
using Outbox.Application.DTOs;
using Outbox.Application.Services;

namespace Outbox.Services.Api.Controllers;

[ApiController]
[Route("orders")]
public class OrderController : ControllerBase
{
	private readonly ILogger<OrderController> _logger;
	private readonly IOrderService _orderService;

	public OrderController(ILogger<OrderController> logger, IOrderService orderService)
	{
		_logger = logger;
		_orderService = orderService;
	}
	
	[HttpGet("{guid:guid}")]
	public async Task<IActionResult> GetById([FromRoute] Guid guid)
	{
		return Ok(await _orderService.GetOrderById(guid));
	}

	[HttpPost("")]
	public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto requestDto)
	{
		var result = await _orderService.CreateOrder(requestDto);
		
		if(result.IsValid)
			return Ok(result);
		
		return BadRequest(result);
	}
}