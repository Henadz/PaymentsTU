using FrameworkExtend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace DocumentModel
{
	public sealed class Paragraph
	{
		const ParagraphAlignment DefaultAlignment = ParagraphAlignment.Left;

		private Paragraph()
		{
			Alignment = DefaultAlignment;
		}

		public string FontName { get; set; }
		public Color Color { get; set; }
		public IEnumerable<IBlock> TextBlocks { get; private set; }
		public ParagraphAlignment Alignment { get; set; }

		public Paragraph([Localizable(false)] string text)
		{
			if (text == null) throw new ArgumentNullException("text");

			Alignment = DefaultAlignment;
			TextBlocks = new [] { new TextBlock(new FormattedText(text)) };
		}

		public Paragraph(FormattedText text)
		{
			if (text == null) 
				throw new ArgumentNullException("text");

			Alignment = DefaultAlignment;
			TextBlocks = new [] { new TextBlock(text) };
		}

		public Paragraph(IEnumerable<FormattedText> text)
		{
			if (text == null) throw new ArgumentNullException("text");

			Alignment = DefaultAlignment;
			TextBlocks = text.Select(t => new TextBlock(t)).ToArray();
		}

		public Paragraph(params IBlock[] blocks)
			: this((IEnumerable<IBlock>)blocks)
		{
		}

		public Paragraph(IEnumerable<IBlock> blocks)
		{
			Alignment = DefaultAlignment;
			TextBlocks = blocks ?? throw new ArgumentNullException("blocks");
		}
	}

	public enum ParagraphAlignment
	{
		Left,
		Right,
		Justify,
		Center
	}
}