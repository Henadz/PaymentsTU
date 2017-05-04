using PaymentsTU.Database;
using System;
using System.ComponentModel;

namespace PaymentsTU.Model
{
	public class Employee : ICloneable
	{
		[DBMetadata(TableName = "Employee")]
		public long? Id { get; set; }

		[DBMetadata(TableName = "Employee")]
		public string Surname { get; set; }

		[DBMetadata(TableName = "Employee")]
		public string Name { get; set; }

		[DBMetadata(TableName = "Employee", ColumnName = "Patronimic")]
		public string Patronymic { get; set; }

		[DBMetadata(TableName = "Employee")]
		public bool IsFired { get; set; }

		[DBMetadata(TableName = "Employee")]
		public string Note { get; set; }

		[DBMetadata(TableName = "Employee")]
		public long? DepartmentId { get; set; }

		public string FullName => string.Join(" ", new[] { Surname, Name, Patronymic }).Trim();

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}