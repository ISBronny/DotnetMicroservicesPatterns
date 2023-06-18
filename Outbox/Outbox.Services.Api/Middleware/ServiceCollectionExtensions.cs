using Outbox.Domain.Interfaces;
using Outbox.Infra.Data.Repositories;

namespace Outbox.Services.Api.Middleware;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection RegisterRepositoriesServices(this IServiceCollection services)
	{
		return services.AddScoped<IOrderRepository, OrderRepository>();
	}
}