NHibernateSQLite
================

In the solution you'll find three projects:

* "Core" will contain the model
* "Data" will contain the mapping and the repository
* "Data.Tests" will contain unit test for my Data layer

The Core project
----------------

I really like the Code First development and to do so I need to have a project that will contain my model class.

Since all my classes need to have an id (I’ll only use one class for a test purpose), I’ll create the IEntity interface to store the Id property:

```c#
public interface IEntity
{
  int Id { get; set; }
}
```

And our model class will look like this:

```c#
public class Customer : IEntity
{
  private int _id;
	private string _code;
	private string _name;

	public virtual int Id
	{
		get { return _id; }
		set { _id = value; }
	}

	public virtual string Code
	{
		get { return _code; }
		set { _code = value; }
	}

	public virtual string Name
	{
		get { return _name; }
		set { _name = value; }
	}
}
```

The Data project
----------------

Before to start you need to add a reference to NHibernate and FluentNHibernate.  NHibernate will be used by the repository and FluentNHibernate will be used for the mapping.

NHibernate and FluentNHibernate are already in the project but you can find these files on [NuGet](http://nuget.org/)

Like I said , I’m using FluentNHibernate for the mapping.  If you don’t know how it works you should check the [Fluent NHibernate wiki](https://github.com/jagregory/fluent-nhibernate/wiki/Getting-started)

```c#
public sealed class CustomerMap : ClassMap<Customer>
{
	public CustomerMap()
	{
		Id(x => x.Id).GeneratedBy.Identity();
		Map(x => x.Code);
		Map(x => x.Name);
	}
}
```

The mapping will be used by NHibernate to map the model with the database and you’ll see later that the mapping is used to create the database schema.

Most of the time (every time I need to access a database) I try to use the Repository Pattern.  I’ll show you how you can use a Generic Repository Pattern to make an abstraction layer on top of SQLite.


First, we need to create an interface which will be our data access facade:

```c#
public interface IRepository<T> where T : class
{
	IQueryable<T> CreateQuery();
	T Save(T entity);
}
```

It’s enough for our test but to complete the interface you should add more functionalities like: Delete, GetById, GetAll, …

The _IRepository_ interface is a common interface which can be implemented by different repositories and it will only contain basic functionalities.

To execute those functionalities, we’ll create the following implementation of the _IRepository_ interface:

```c#
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
```

Like you can see, I added the _ISession_ property to use the NHibernate persistence service.

But how can we use the _SqLiteRepository_ class to get our _Customer_ entity from the database? For that job, we’ll inherit from the _SqLiteRepository_ class to create the _CustomerRepository_ class.

```c#
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
```

That’s in this final repository that you’ll add specific functionalities like: GetByCode, GeByName, …

Did you see the type of the _SqLiteRepository_ contained object class? Yes this is the type of our _Customer_ model.  For example, if you want to create the address repository for the address model, you’ll use something like this:

```c#
public class AddressRepository : SqLiteRepository<Address>, IAddressRepository
{
...
}
```

Now you’ll probably ask me why do you use the _ICustomerRepository_ interface.  Well it’s a habit and I’ll not use it here but I need that kind of interface when it’s time to mock in my unit test.

```c#
public interface ICustomerRepository : IRepository<Customer>
{
	IEnumerable<Customer> GetAll();
}
```

The Data.Tests project
----------------------

To test the data access layer I wanted to have an in memory database created when needed in my unit test then disposed when the unit test is over.

The _SqLiteSessionFactory_ class (below) will generate an in memory SQLite database based on the model.

```c#
public class SqLiteSessionFactory : IDisposable
{
	private Configuration _configuration;
	private ISessionFactory _sessionFactory;

	public ISession Session { get; set; }

	public SqLiteSessionFactory()
	{
		_sessionFactory = CreateSessionFactory();
		Session = _sessionFactory.OpenSession();
		ExportSchema();
	}

	private void ExportSchema()
	{
		var export = new SchemaExport(_configuration);
		//export.SetOutputFile(@"..\..\..\database\initial.database\create.objects.sql").Execute(true, true, false,
		//                                                                                       Session.Connection, null);

		using (var file = new FileStream(@"..\..\..\database\initial.database\create.objects.sql", FileMode.Create, FileAccess.Write))
		using (var sw = new StreamWriter(file))
		{
			export.Execute(true, true, false, Session.Connection, sw);
			sw.Close();
		}
	}

	private ISessionFactory CreateSessionFactory()
	{
		return Fluently.Configure()
					.Database(SQLiteConfiguration.Standard.InMemory().ShowSql())
					.Mappings(m =>
					{
						m.FluentMappings.Conventions.Setup(c => c.Add(AutoImport.Never()));
						m.FluentMappings.Conventions.AddAssembly(Assembly.GetExecutingAssembly());
						m.HbmMappings.AddFromAssembly(Assembly.GetExecutingAssembly());

						var assembly = Assembly.Load("NHibernateSQLite.Data");
						m.FluentMappings.Conventions.AddAssembly(assembly);
						m.FluentMappings.AddFromAssembly(assembly);
						m.HbmMappings.AddFromAssembly(assembly);

					})
					.ExposeConfiguration(cfg => _configuration = cfg)
					.BuildSessionFactory();
	}

	public void Dispose()
	{
		Session.Dispose();
	}
}
```

When our test will run, we’ll create an instance of the _SqLiteSessionFactory_ class.  The default constructor will call two methods:

* ExportSchema
* CreateSessionFactory

The _ExportSchema_ method is used to create the in memory database based on the mapping and to create the SQL script file in a specific folder ("..\..\..\database\initial.database\create.objects.sql").

The following line worked only once:

```c#
export.SetOutputFile(@"..\..\..\database\initial.database\create.objects.sql").Execute(true, true, false, Session.Connection, null);
```

And I still don’t know why it doesn’t work anymore… So I used a StreamWriter to create the SQL Script file:

```c#
using (var file = new FileStream(@"..\..\..\database\initial.database\create.objects.sql", FileMode.Create, FileAccess.Write))
{
	using (var sw = new StreamWriter(file))
	{
		export.Execute(true, true, false, Session.Connection, sw);
		sw.Close();
	}
}
```

The _CreateSessionFactory_ method is used to look in the assembly where the mapping is located.  With this method NHibernate will be able to generate the database schema.


Now we need to create the unit test to check if it works! Here is the _CustomerRepositoryFixture_ class:

```c#
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
```

The _SaveTest_ method will create an instance of our model and we’ll save it in the database.  Before to save the _Id_ property must be equal to 0 because it’s not assigned and after the save the _Id_ property must be different to 0 (because we specified in the mapping that the Id property is an identity).