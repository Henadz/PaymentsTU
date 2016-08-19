using System.Collections.Generic;
using System.Windows.Documents;

namespace PaymentsTU.Model
{
    internal class PaymentInfoRow
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public List<Payment> Payments { get; set; }
    }
}