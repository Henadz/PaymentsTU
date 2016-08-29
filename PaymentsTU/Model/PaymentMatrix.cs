using System.Collections;
using System.Collections.Generic;

namespace PaymentsTU.Model
{
    public sealed class PaymentMatrix : IEnumerable
    {
        public List<Column> Columns { get; set; }
        public List<object[]> Rows { get; set; }

        public IEnumerator GetEnumerator()
        {
            return Rows.GetEnumerator();
        }
    }
}