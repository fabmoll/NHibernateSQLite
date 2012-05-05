using System.Linq;
using NHibernateSQLite.Core;
using NHibernateSQLite.Data.Repository;
using NUnit.Framework;

namespace NHibernateSQLite.Data
{
	[TestFixture]
	public class CustomerRepositoryFixture
	{
		private ICustomerRepository Repository { get; set; }
		private SqLiteSessionFactory SqLiteSessionFactory { get; set; }

		[SetUp]
		public void SetUp()
		{
			SqLiteSessionFactory = new SqLiteSessionFactory();
			Repository = new CustomerRepository { Session = SqLiteSessionFactory.Session };
		}

		[Test]
		public void SaveTest()
		{
			var customer = new Customer { Code = "Code1", Name = "Name1" };

			Assert.That(customer.Id, Is.EqualTo(0));

			Repository.Save(customer);

			SqLiteSessionFactory.Session.Flush();

			var customer2 = from c in Repository.CreateQuery()
								 select c;

			Assert.That(customer2.First().Id, Is.GreaterThan(0));
		}

		[TearDown]
		public void TearDown()
		{
			SqLiteSessionFactory.Dispose();
		}
	}
}