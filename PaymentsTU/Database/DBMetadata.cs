using System;

namespace PaymentsTU.Database
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	class DBMetadata : Attribute
	{
		public string TableName { get; set; }
		public string ColumnName { get; set; }

	}
}
