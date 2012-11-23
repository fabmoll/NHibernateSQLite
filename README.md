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