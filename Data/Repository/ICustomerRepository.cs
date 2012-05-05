using System.Collections.Generic;
using NHibernateSQLite.Core;

namespace NHibernateSQLite.Data.Repository
{
	public interface ICustomerRepository : IRepository<Customer>
	{
		IEnumerable<Customer> GetAll();
	}
}