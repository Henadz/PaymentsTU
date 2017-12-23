using DocumentModel.Processor;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Documents;
using DocumentModel;
using System.Windows;
using System.Windows.Media;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Printing;

namespace PaymentsTU.Document
{
	internal class XpsRenderStrategy : IRenderStrategy
	{
		private FixedDocument _document;
		private Size _pageSize;
		private Thickness _pageMargin;
		private double _freeHeight = 0;

		public XpsRenderStrategy(Size pageSize, Thickness pageMargin)
		{
			_document = new FixedDocument();
			_pageSize = pageSize;
			_pageMargin = pageMargin;
		}

		public void Render(IEnumerable<ProcessingChunk> items, Stream stream, Encoding encoding)
		{
			foreach (var content in GetPages(items))
			{
				_document.Pages.Add(content);
			}

			var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);

			var documentUri = new Uri("pack://filename.xps");
			PackageStore.AddPackage(documentUri, package);

			var xpsDocument = new XpsDocument(package, CompressionOption.NotCompressed, documentUri.AbsoluteUri);
			var writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
			writer.Write(_document);

			PackageStore.RemovePackage(documentUri);
			xpsDocument.Close();
		}

		private IEnumerable<PageContent> GetPages(IEnumerable<ProcessingChunk> chunks)
		{
			var pages = new List<PageContent>();

			FixedPage page = null;

			foreach (var chunk in chunks)
			{
				foreach(var artifact in chunk.Artifacts)
				{
					if (artifact is NewPage)
					{
						if (page == null)
						{
							page = CreateNewPage();
						}
						else
						{
							pages.Add(ToPageContent(page));
							page = CreateNewPage();
						}
					}
					else if (artifact is DocumentModel.Paragraph)
					{
						page = page ?? CreateNewPage();

						var p = RenderParagraph((DocumentModel.Paragraph)artifact);

						var height = MeasureHeight(p);

						if (height <= _freeHeight)
						{
							page.Children.Add(p);
						}
						else
						{
							pages.Add(ToPageContent(page));
							page = CreateNewPage();
							page.Children.Add(p);
						}
					}
				}
			}

			if (page != null)
				pages.Add(ToPageContent(page));

			return pages;
		}

		private System.Windows.Controls.TextBlock RenderParagraph(DocumentModel.Paragraph p)
		{
			var tb = new System.Windows.Controls.TextBlock()
			{
				TextWrapping = TextWrapping.Wrap,
				TextAlignment = ToWindowsAligment(p.Alignment)
			};
			tb.Width = _pageSize.Width - (_pageMargin.Left + _pageMargin.Right);

			if (p.Color != System.Drawing.Color.Empty)
				tb.Foreground = new SolidColorBrush(Color.FromArgb(p.Color.A, p.Color.R, p.Color.G, p.Color.B));
			if (!string.IsNullOrEmpty(p.FontFamilyName))
				tb.FontFamily = new FontFamily(p.FontFamilyName);
			if (p.FontSize.HasValue)
				tb.FontSize = p.FontSize.Value;

			foreach (var block in p.TextBlocks)
			{
				var text = (block as TextBlock)?.Text;
				if (text != null)
				{
					tb.Inlines.AddRange(RenderTextBlock(text));
				};

				//TODO: add another blocks
			}

			return tb;
		}

		private static IEnumerable<Run> RenderTextBlock(DocumentModel.FormattedText text)
		{
			var runs = new List<Run>(2);

			var run = new Run(text.Text);
			if (text.Bold)
				run.FontWeight = FontWeights.Bold;
			if (text.Italic)
				run.FontStyle = FontStyles.Italic;
			if (text.Underline)
				run.TextDecorations = TextDecorations.Underline;
			if (text.Superscript)
				run.BaselineAlignment = BaselineAlignment.Superscript;
			if (text.Subscript)
				run.BaselineAlignment = BaselineAlignment.Subscript;
			if (text.HasTextColor)
				run.Foreground = new SolidColorBrush(Color.FromArgb(text.TextColor.A, text.TextColor.R, text.TextColor.G, text.TextColor.B));
			if (text.HasBackgroundColor)
				run.Background = new SolidColorBrush(Color.FromArgb(text.BackgroundColor.A, text.BackgroundColor.R, text.BackgroundColor.G, text.BackgroundColor.B));
			runs.Add(run);

			if (text.LineBreak)
				runs.Add(new Run(Environment.NewLine));

			return runs;
		}

		private TextAlignment ToWindowsAligment(ParagraphAlignment alignment)
		{
			switch (alignment)
			{
				case ParagraphAlignment.Right:
					return TextAlignment.Right;
				case ParagraphAlignment.Justify:
					return TextAlignment.Justify;
				case ParagraphAlignment.Center:
					return TextAlignment.Center;
				case ParagraphAlignment.Left:
				default:
					return TextAlignment.Left;
			}
		}

		private FixedPage CreateNewPage()
		{
			var page = new FixedPage();
			page.RenderSize = _pageSize;
			page.Margin = _pageMargin;
			_freeHeight = _pageSize.Height - (_pageMargin.Top + _pageMargin.Bottom);
			return page;
		}

		private PageContent ToPageContent(FixedPage page)
		{
			var content = new PageContent();
			((System.Windows.Markup.IAddChild)content).AddChild(page);
			return content;
		}

		private double MeasureHeight(FrameworkElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			element.Measure(new Size(_pageSize.Width - (_pageMargin.Left + _pageMargin.Right), _pageSize.Height));
			return element.DesiredSize.Height;
		}

		public void Setup(ProcessingModel model)
		{
			throw new NotImplementedException();
		}
	}
}
