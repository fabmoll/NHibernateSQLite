using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace NHibernateSQLite.Data.Repository
{
	public class SqLiteRepository<T> : IRepository<T> where T : class
	{
		public ISession Session { get; set; }

		#region IRepository members

		public IQueryable<T> CreateQuery()
		{
			return Session.Query<T>();
		}

		public T Save(T entity)
		{
			Session.Save(entity);

			return entity;
		} 

		#endregion
	}
}