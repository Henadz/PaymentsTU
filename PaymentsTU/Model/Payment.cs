using System;

namespace PaymentsTU.Model
{
    internal class Payment
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int PaymentTypeId { get; set; }
        public DateTime DatePayment { get; set; }
        public int CurrencyId { get; set; }
        public decimal Value { get; set; }
    }
}