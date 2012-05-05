using FluentNHibernate.Mapping;
using NHibernateSQLite.Core;

namespace NHibernateSQLite.Data.Mapping
{
	public sealed class CustomerMap : ClassMap<Customer>
	{
		public CustomerMap()
		{
			Id(x => x.Id).GeneratedBy.Identity();
			Map(x => x.Code);
			Map(x => x.Name);
		}
	}
}