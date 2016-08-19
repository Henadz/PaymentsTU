using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentsTU.Model
{
    public sealed class FinancialPeriod
    {
        public int? Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsClosed { get; set; }
        public decimal PaymentLimit { get; set; }
    }
}
