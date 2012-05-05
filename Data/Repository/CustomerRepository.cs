using System.Collections.Generic;
using System.Linq;
using NHibernateSQLite.Core;

namespace NHibernateSQLite.Data.Repository
{
	public class CustomerRepository : SqLiteRepository<Customer>, ICustomerRepository
	{
		#region ICustomerRepository members

		public IEnumerable<Customer> GetAll()
		{
			var query = from c in CreateQuery()
							select c;

			return query;
		} 

		#endregion
	}
}