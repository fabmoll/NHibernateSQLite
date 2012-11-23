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

>public interface IEntity
>{
>  int Id { get; set; }
>}