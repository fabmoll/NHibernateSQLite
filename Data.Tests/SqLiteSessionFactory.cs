using System;
using System.IO;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace NHibernateSQLite.Data
{
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
			//export.SetOutputFile(@"..\..\..\database\initial.database\create.objects.sql").Execute(true, true, false, Session.Connection, null);

			using (var file = new FileStream(@"..\..\..\database\initial.database\create.objects.sql", FileMode.Create, FileAccess.Write))
			{
				using (var sw = new StreamWriter(file))
				{
					export.Execute(true, true, false, Session.Connection, sw);
					sw.Close();
				}
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
}