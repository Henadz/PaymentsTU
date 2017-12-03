using FrameworkExtend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace DocumentModel
{
    public sealed class Cell
    {
        /// <summary>
        /// Background color of cell
        /// </summary>
        public Color BackColor { get; set; }
        /// <summary>
        /// Count columns with cell is spanned
        /// default value is 1
        /// </summary>
        public int ColSpan { get; set; }
        /// <summary>
        /// Count rows with cell is spanned
        /// default value is 1
        /// </summary>
        public int RowSpan { get; set; }
        /// <summary>
        /// Content
        /// </summary>
        public Paragraph Text { get; private set; }
        /// <summary>
        /// set wrapping text in cell
        /// default value is true
        /// </summary>
        public bool IsTextWrapped { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsHeaderCell { get; set; }
        /// <summary>
        /// Defines how preferrably Cell content shall be treated
        /// </summary>
        public CellDisplayStyle DisplayStyle { get; set; }
        /// <summary>
        /// Format of cell data
        /// </summary>
        public string CellFormat { get; set; }

        internal Cell()
        {
            DisplayStyle = CellDisplayStyle.Text;
            IsTextWrapped = true;
            ColSpan = 1;
            RowSpan = 1;
        }

        public Cell(int text)
        {
            DisplayStyle = CellDisplayStyle.Integer;
            Text = new Paragraph(text.ToString(CultureInfo.InvariantCulture));
        }

        public Cell([Localizable(false)] string text) : this()
        {
            Text = new Paragraph(text ?? "");
        }

        public Cell([Localizable(false)] string text, bool wrap)
            : this()
        {
            IsTextWrapped = wrap;
            Text = new Paragraph(text ?? "");
        }

        public Cell([Localizable(false)] string text, string format, CellDisplayStyle displayStyle)
            :this()
        {
            Text = new Paragraph(text);
            DisplayStyle = displayStyle;
            CellFormat = format;
        }


        public Cell(FormattedText text) : this()
        {
            Text = new Paragraph(Enumerable.Repeat(text, 1));
        }

        public Cell(IEnumerable<FormattedText> text) : this()
        {
            Text = new Paragraph(text);
        }

        public Cell(Paragraph paragraph) : this()
        {
            Text = paragraph;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public Cell([Localizable(false)] string text, Uri url, bool isWrapped)
        {
            if (url == null)
                throw new ArgumentException(@"Correct URL shall be specified for cell", "url");

            var urlBlock = new UrlBlock(url, text);

            Text = new Paragraph(urlBlock);
            IsTextWrapped = isWrapped;
        }

        public Cell([Localizable(false)] string text, Image image, int width, int? height)
            : this()
        {
            if (width < 0) throw new ArgumentException(@"Image width shall not be negative", "width");
            if (height < 0) throw new ArgumentException(@"Image height shall not be negative", "height");

            var imageBlock = new ImageBlock(image, text);
            Text = new Paragraph(imageBlock);
            imageBlock.Width = width;
            imageBlock.Height = height;
        }
    }

    /// <summary>
    /// Defines how preferrably Cell content shall be treated
    /// </summary>
    public enum CellDisplayStyle
    {
        // cell content is text
        Text,
        // cell content is double number
        Double,
        // cell content is integer number
        Integer,
        Date
    }
}