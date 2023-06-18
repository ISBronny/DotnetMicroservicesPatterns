using System.Net;
using FluentAssertions;
using Outbox.Application.DTOs;
using Outbox.Domain.Handlers;
using Outbox.Domain.Models;

namespace Outbox.AcceptanceTests;

public class OrderTests : TestBase
{
	private static Guid CustomerId { get; } = Guid.NewGuid();
	private static Guid MangerId { get; } = Guid.NewGuid();
	
	[Fact]
	public void CreateOrderTest()
	{
		var request = new CreateOrderRequestDto()
		{
			Price = 123.00,
			CustomerId = CustomerId,
			MangerId = MangerId
		};

		HttpHost.Post
			.Url("orders")
			.Json(request)
			.Send()
			.Response
				.AssertStatusCode(HttpStatusCode.OK)
				.AsJson
					.AssertThat<CreateOrder.CreateOrderResponse>(r => r.Order.Should().NotBeNull())
					.CopyResponseTo(out CreateOrder.CreateOrderResponse response);
		
		HttpHost.Get
			.Url($"orders/{response.Order!.Id}")
			.Send()
			.Response
				.AssertStatusCode(HttpStatusCode.OK)
				.AsJson
					.AssertThat<Order>(o => o.Should().NotBeNull());
	}
}