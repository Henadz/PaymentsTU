using System.Collections.Generic;

namespace DocumentModel
{
	/// <summary>
	/// Defines a set of lines (horizontal or vertical) to be rendered as a table.
	/// Table header is just another line.
	/// </summary>
	public sealed class Table
	{
		public SizeUnit Units { get; set; }
		public Paragraph Caption { get; set; }
		public bool RepeatHeader { get; set; }
		public IList<Row> Header { get; private set; }
		public IList<Row> Body { get; private set; }
		public bool IsBorderless { get; set; }
		public int ColumnCount { get; }
		public IList<double> ColumnsWidth { get; private set; }
		public TableLayoutMode LayoutMode { get; set; }

		public Table(int columnCount)
		{
			Units = SizeUnit.AverageCharacter;
			RepeatHeader = true;
			LayoutMode = TableLayoutMode.None;
			ColumnCount = columnCount;
		}

		public Table(int columnCount, SizeUnit units)
		:this(columnCount)
		{
			Units = units;
		}

		public void SetHeader(IList<Row> header)
		{
			Header = header;
		}

		public void SetRows(IList<Row> body)
		{
			Body = body;
		}

		public void SetColumnsWidth(params double[] widths)
		{
			ColumnsWidth = widths;
		}

		public double? Width
		{
			get
			{
				if ((ColumnsWidth?.Count ?? 0) == 0)
					return null;

				var totalColumnWidth = 0d;
				foreach (var w in ColumnsWidth)
					totalColumnWidth += w;

				return totalColumnWidth;
			}
		}
	}
}