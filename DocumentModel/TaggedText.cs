using FrameworkExtend;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace DocumentModel
{
	/// <summary>
	/// Describes tagged text
	/// </summary>
	[DebuggerDisplay("{Text}")]
    public class FormattedText
    {
        internal FormattedText()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FormattedText([Localizable(false)] string text)
        {
            Text = text;
            TextColor = Color.Empty;
            BackgroundColor = Color.Empty;
            ListItem = null;
        }

        /// <summary>
        /// Tagged text string
        /// </summary>
        public string Text { get; private set; }

        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        /// <summary>
        /// Font size in points
        /// </summary>
        public float? FontSize { get; set; }
        public string FontFamilyName { get; set; }
        public bool Bold { get; set; }
        /// <summary>
        /// Next text will be started from new line
        /// </summary>
        public bool LineBreak { get; set; }
        public bool Underline { get; set; }
        public bool Italic { get; set; }
        /// <summary>
        /// Text contains CJK symbols
        /// </summary>
        public bool HasCjk { get; private set; }

        private bool _superscript;

        public bool Superscript
        {
            get { return _superscript; }
            set
            {
                _superscript = value;
                if (value)
                    _subscript = false;
            }
        }

        private bool _subscript;
        public bool Subscript
        {
            get { return _subscript; }
            set
            {
                _subscript = value;
                if (value)
                    _superscript = false;
            }
        }
        public string ListItem { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public string Link { get; set; } //todo: remove

        public bool HasTextColor
        {
            get { return TextColor != Color.Empty; }
        }

        public bool HasBackgroundColor
        {
            get { return BackgroundColor != Color.Empty; }
        }

        public FormattedText CreateCopy([Localizable(false)] string text)
        {
            var newValue = new FormattedText(text);
            newValue.BackgroundColor = BackgroundColor;
            newValue.TextColor = TextColor;
            newValue.FontSize = FontSize;
            newValue.FontFamilyName = FontFamilyName;
            newValue.Bold = Bold;
            newValue.LineBreak = LineBreak;
            newValue.Underline = Underline;
            newValue.Italic = Italic;
            newValue.HasCjk = HasCjk;
            newValue.Superscript = Superscript;
            newValue.Subscript = Subscript;
            newValue.ListItem = ListItem;

            return newValue;
        }

        internal FormattedText CreateCjkCopy([Localizable(false)] string text)
        {
            var newValue = CreateCopy(text);
            newValue.HasCjk = true;

            return newValue;
        }

        public static bool IsPlainEmpty(IEnumerable<FormattedText> source)
        {
            if (source == null)
                return true;

            return source.All(x => string.IsNullOrEmpty(x.Text));
        }

        public static string ToPlaintext(IEnumerable<FormattedText> source)
        {
            if (source == null)
                return string.Empty;

            return string.Join(string.Empty, source.Select(x => x.Text).ToArray());
        }
    }
}
