using Microsoft.Extensions.DependencyInjection;
using Outbox.Application.Services;
using Outbox.Domain.Handlers;

namespace Outbox.Application.Persistent;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
	{
		return services.AddScoped<IOrderService, OrderService>()
			.AddScoped<CreateOrder.CreateOrderRequestValidator>();
	}
}