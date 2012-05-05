namespace NHibernateSQLite.Core
{
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
}