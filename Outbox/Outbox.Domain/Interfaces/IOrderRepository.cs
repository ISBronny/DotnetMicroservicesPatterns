using Domain.Core.Interfaces;
using Outbox.Domain.Models;

namespace Outbox.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
	
}

public interface IOutboxRepository : IRepository<Models.Outbox>
{
	
}