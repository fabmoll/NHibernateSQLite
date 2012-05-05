using System.Linq;

namespace NHibernateSQLite.Data.Repository
{
	public interface IRepository<T> where T : class
	{
		IQueryable<T> CreateQuery();
		T Save(T entity);
	}
}