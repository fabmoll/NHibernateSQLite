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

