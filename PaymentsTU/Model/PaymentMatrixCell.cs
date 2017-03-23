using System;
using System.Collections.Generic;

namespace PaymentsTU.Model
{
	internal sealed class Cell
	{
		public int ColumnId { get; set; }
		public Type ValueType { get; private set; }
		public object Value { get; set; }

		public Cell(Type valueType)
		{
			ValueType = valueType;
		}
	}

	internal sealed class Row
	{
		public int RowId { get; set; }
		public IList<Cell> Cells { get; set; }
	}
}
