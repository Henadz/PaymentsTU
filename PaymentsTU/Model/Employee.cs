using System;

namespace PaymentsTU.Model
{
	public class Employee : ICloneable
	{
		public long? Id { get; set; }
		public string Surname { get; set; }
		public string Name { get; set; }
		public string Patronymic { get; set; }
		public bool IsFired { get; set; }
		public string Note { get; set; }
		public long? DepartmentId { get; set; }
		public string FullName => string.Join(" ", new[] { Surname, Name, Patronymic }).Trim();
		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}