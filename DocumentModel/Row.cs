using FrameworkExtend;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DocumentModel
{
	/// <summary>
	/// Defines several items arranged in horizontal line
	/// </summary>
	public sealed class Row
    {
        internal Row()
        {}

        public ReadOnlyCollection<Cell> Items { get; private set; }

        public Row(params Cell[] cells) : this((IEnumerable<Cell>)cells) { }

        public Row(IEnumerable<Cell> cells)
        {
            if (cells == null) throw new ArgumentNullException("cells");
            Items = new ReadOnlyCollection<Cell>(cells.ToList());
        }
    }
}