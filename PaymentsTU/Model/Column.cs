using System;

namespace PaymentsTU.Model
{
    public sealed class Column
    {
		public int Ordinal { get; set; }
		public string ColumnName { get; set; }
		public Type DataType { get; set; }
		public string Caption { get; set; }
        public string DataFormat { get; set; }
		public bool IsVisible { get; set; } = true;
    }
}