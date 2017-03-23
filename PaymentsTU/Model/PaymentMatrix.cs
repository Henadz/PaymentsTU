using System.Collections.Generic;

namespace PaymentsTU.Model
{
    internal sealed class PaymentMatrix
    {
        public List<Column> Columns { get; set; }
        public List<Row> Rows { get; set; }
    }
}