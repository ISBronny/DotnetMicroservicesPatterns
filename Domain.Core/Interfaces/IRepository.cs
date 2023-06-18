namespace Domain.Core.Interfaces;

public interface IRepository<TEntity>  where TEntity : class
{
	void Add(TEntity obj);
	Task<TEntity?> GetById(Guid id);
	Task<List<TEntity>> GetAll();
	void Update(TEntity obj);
	void Remove(Guid id);
	Task SaveChangesAsync();
}