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
using System.Windows.Controls;
using Table = DocumentModel.Table;

namespace PaymentsTU.Document
{
	internal class XpsRenderStrategy : IRenderStrategy
	{
		private readonly FixedDocument _document;
		private Size _pageSize;
		private Thickness _pageMargin;
		private PageLayout _page;

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

			using (var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite))
			{

				var documentUri = new Uri($"memorystream://{Guid.NewGuid()}.xps");
				PackageStore.AddPackage(documentUri, package);

				var xpsDocument = new XpsDocument(package, CompressionOption.Maximum, documentUri.AbsoluteUri);
				var writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
				writer.Write(_document);
				xpsDocument.Close();

				PackageStore.RemovePackage(documentUri);
			}
		}

		private IEnumerable<PageContent> GetPages(IEnumerable<ProcessingChunk> chunks)
		{
			var pages = new List<PageContent>();

			_page = null;

			foreach (var chunk in chunks)
			{
				foreach(var artifact in chunk.Artifacts)
				{
					if (artifact is NewPage)
					{
						if (_page == null)
						{
							_page = CreateNewPage();
						}
						else
						{
							pages.Add(_page.GetPageContent());
							_page = CreateNewPage();
						}
					}
					else if (artifact is DocumentModel.Paragraph)
					{
						_page = _page ?? CreateNewPage();

						var p = RenderParagraph((DocumentModel.Paragraph)artifact);

						if (_page.AddContent(p)) continue;

						pages.Add(_page.GetPageContent());
						_page = CreateNewPage();
						_page.AddContent(p);
					}
					else if (artifact is Table)
					{
						_page = _page ?? CreateNewPage();

						RenderTableContent((Table)artifact, pages);
					}
				}
			}

			if (_page != null)
				pages.Add(_page.GetPageContent());

			return pages;
		}

		private void RenderTableContent(Table artifact, List<PageContent> pages)
		{
			if (artifact.RepeatHeader)
				_page.OnNewColumn += (sender, args) => { ArrangeTableHeader(artifact, pages); };
			else
			{
				ArrangeTableHeader(artifact, pages);
			}

			SetTableContentLayout(artifact);

			ArrangeTableRows(artifact, pages);
		}

		private System.Windows.Controls.TextBlock RenderParagraph(DocumentModel.Paragraph p, double? desiredWidth = null)
		{
			var tb = new System.Windows.Controls.TextBlock
			{
				TextWrapping = TextWrapping.Wrap,
				TextAlignment = ToWindowsAligment(p.Alignment),
				Width = desiredWidth ?? _pageSize.Width - (_pageMargin.Left + _pageMargin.Right)
			};

			if (p.Color != System.Drawing.Color.Empty)
				tb.Foreground = new SolidColorBrush(Color.FromArgb(p.Color.A, p.Color.R, p.Color.G, p.Color.B));
			if (!string.IsNullOrEmpty(p.FontFamilyName))
				tb.FontFamily = new FontFamily(p.FontFamilyName);
			if (p.FontSize.HasValue)
				tb.FontSize = p.FontSize.Value;

			foreach (var block in p.TextBlocks)
			{
				var text = (block as DocumentModel.TextBlock)?.Text;
				if (text != null)
				{
					tb.Inlines.AddRange(RenderTextBlock(text));
				}

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

		private PageLayout CreateNewPage()
		{
			var page = new PageLayout(_pageSize, _pageMargin);
			return page;
		}

		private Grid RenderTableRow(IEnumerable<ColumnDefinition> columnDefinitions, Row row)
		{
			var table = new Grid();
			foreach (var columnDefinition in columnDefinitions)
			{
				table.ColumnDefinitions.Add(new ColumnDefinition{Width = columnDefinition.Width});
			}

			table.RowDefinitions.Add(new RowDefinition());
			for (var i = 0; i < table.ColumnDefinitions.Count; i++)
			{
				AddTableCell(table, row.Items[i], i, 0);
			}

			return table;
		}

		private void ArrangeTableHeader(Table table, IList<PageContent> pages)
		{
			var columns = GetColumnDefinitions(table, _page.ContentWidth);

			foreach (var row in table.Header)
			{
				var tr = RenderTableRow(columns, row);

				if (_page.AddContent(tr)) continue;

				pages.Add(_page.GetPageContent());

				_page = CreateNewPage();
				//TODO: validate this case
				SetTableContentLayout(table);
				_page.AddContent(tr);
			}
		}

		private void SetTableContentLayout(Table table)
		{
			var layoutColumns = 1;
			if (table.LayoutMode == TableLayoutMode.UseAllWidth)
			{
				var totalColumnWidth = 0d;
				foreach (var w in table.ColumnsWidth)
					totalColumnWidth += w;

				if (totalColumnWidth > 0)
				{
					var columns = (_pageSize.Width - (_pageMargin.Left + _pageMargin.Right)) / totalColumnWidth;

					layoutColumns = (int) Math.Round(columns, MidpointRounding.AwayFromZero);
				}
			}


			_page.SetContentColumns(layoutColumns);
		}

		private void ArrangeTableRows(Table table, IList<PageContent> pages)
		{
			var columns = GetColumnDefinitions(table, _page.ContentWidth);

			foreach (var row in table.Body)
			{
				var tr = RenderTableRow(columns, row);

				if (_page.AddContent(tr)) continue;

				pages.Add(_page.GetPageContent());
				_page = CreateNewPage();
				if (table.RepeatHeader)
					_page.OnNewColumn += (sender, args) => { ArrangeTableHeader(table, pages); };

				SetTableContentLayout(table);
				_page.AddContent(tr);
			}
		}

		private IList<ColumnDefinition> GetColumnDefinitions(Table table, double width)
		{
			var tw = table.Width ?? width;
			var proportion = 1d;
			if (tw > width)
				proportion = width / tw;

			var definitions = new List<ColumnDefinition>(table.ColumnCount);

			foreach (var d in table.ColumnsWidth)
			{
				var cd = new ColumnDefinition { Width = new GridLength(d * proportion, GridUnitType.Pixel) };
				definitions.Add(cd);
			}

			return definitions;
		}

		private void AddTableCell(Grid grid, Cell cell, int columnIndex, int rowIndex)
		{
			var text = RenderParagraph(cell.Text, grid.ColumnDefinitions[columnIndex].Width.Value);
			//text.Style = this.TableHeaderTextStyle;
			if (!cell.IsTextWrapped)
			{
				text.TextTrimming = TextTrimming.WordEllipsis;//.CharacterEllipsis;
				text.TextWrapping = TextWrapping.NoWrap;
			}

			text.SetValue(Grid.ColumnProperty, columnIndex);
			text.SetValue(Grid.RowProperty, rowIndex);

			grid.Children.Add(text);
		}

		public void Setup(ProcessingModel model)
		{
			throw new NotImplementedException();
		}
	}
}
