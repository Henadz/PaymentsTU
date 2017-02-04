using System;
using System.Collections;

namespace PaymentsTU.Model
{
	public class Payment : ICloneable
	{
		public long? Id { get; set; }
		public long EmployeeId { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Patronimic { get; set; }

		public string FullName
			=> string.Join(" ", new[] {Surname, Name, Patronimic}).Trim();
		public long PaymentTypeId { get; set; }
		public string PaymentType { get; set; }
		public long DepartmentId { get; set; }
		public string Department { get; set; }
		public DateTime DatePayment { get; set; }
		public int CurrencyId { get; set; }
		public string Currency { get; set; }
		public decimal Value { get; set; }
		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}