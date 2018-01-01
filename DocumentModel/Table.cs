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
        public IList<double> ColumnsWidth { get; private set; }
        public bool ScaleToPage { get; set; }

        public Table()
        {
            Units = SizeUnit.AverageCharacter;
	        RepeatHeader = true;

        }

        public Table(SizeUnit units)
		:this()
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
    }
}