using System;

namespace PaymentsTU.Model
{
    public class Payment
    {
        public long? Id { get; set; }
        public int EmployeeId { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Patronimic { get; set; }

	    public string FullName
		    => string.Join(" ", new[] {Surname.Trim(), (Name ?? "").Trim(), (Patronimic ?? "").Trim()}).Trim();
        public int PaymentTypeId { get; set; }
		public string PaymentType { get; set; }
		public int DepartmentId { get; set; }
		public string Department { get; set; }
		public DateTime DatePayment { get; set; }
        public int CurrencyId { get; set; }
		public string Currency { get; set; }
        public decimal Value { get; set; }
    }
}