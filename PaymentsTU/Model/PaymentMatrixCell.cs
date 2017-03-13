using System;

namespace PaymentsTU.Model
{
	internal sealed class PaymentMatrixCell
	{
		public int RowId { get; set; }
		public int ColumnId { get; set; }
		public Type ValueType { get; private set; }
		public object Value { get; set; }

		public PaymentMatrixCell(Type valueType)
		{
			ValueType = valueType;
		}
	}
}
