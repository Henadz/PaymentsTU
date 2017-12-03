using System;
using System.ComponentModel;

namespace DocumentModel
{
	public class TextBlock: IBlock
    {
        /// <summary>
        /// Used for serialization
        /// </summary>
        internal TextBlock()
        {
        }

        public FormattedText Text { get; protected set; }

        public TextBlock(FormattedText text)
        {
			Text = text ?? throw new ArgumentNullException("text");
        }

        public TextBlock([Localizable(false)] string text)
        {
            Text = new FormattedText(text);
        }
    }
}