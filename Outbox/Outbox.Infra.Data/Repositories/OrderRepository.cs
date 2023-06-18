using Infra.Data.Core;
using Outbox.Domain.Interfaces;
using Outbox.Domain.Models;

namespace Outbox.Infra.Data.Repositories;

public class OrderRepository :  Repository<Order>, IOrderRepository
{
	public OrderRepository(OutboxDbContext context) : base(context)
	{
	}
}
