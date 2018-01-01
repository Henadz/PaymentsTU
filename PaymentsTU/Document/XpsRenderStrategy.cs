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
using System.Windows.Markup;

namespace PaymentsTU.Document
{
	internal class XpsRenderStrategy : IRenderStrategy
	{
		private FixedDocument _document;
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

			var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);

			var documentUri = new Uri($"memorystream://{Guid.NewGuid()}.xps");
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
					else if (artifact is DocumentModel.Table)
					{
						_page = _page ?? CreateNewPage();

						RenderTableContent((DocumentModel.Table)artifact, pages);
					}
				}
			}

			if (_page != null)
				pages.Add(_page.GetPageContent());

			return pages;
		}

		private void RenderTableContent(DocumentModel.Table artifact, List<PageContent> pages)
		{
			var grid = new Grid();

			var columns = SetColumnDefinition(grid, artifact);

			//_page.SetContentColumns(2);

			ArrangeTableHeader(columns, artifact.Header, pages);
			ArrangeTableRows(columns, artifact, pages);
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

		private IList<string> SetColumnDefinition(Grid table, DocumentModel.Table source)
		{
			var totalColumnWidth = 0d;
			foreach (var w in source.ColumnsWidth)
				totalColumnWidth += w;

			var totalColumns = source.ColumnsWidth.Count;

			var columnDefinitions = new List<string>(totalColumns);
			for (var i = 0; i < source.ColumnsWidth.Count; i++)
			{
				var cd = AddTableColumn(table, totalColumnWidth, source.ColumnsWidth[i]);
				columnDefinitions.Add(XamlWriter.Save(cd));
			}

			return columnDefinitions;
		}

		private Grid RenderTableRow(IEnumerable<string> columnDefinitions, Row row)
		{
			var table = new Grid();
			foreach (var columnDefinition in columnDefinitions)
			{
				var copy = XamlReader.Parse(columnDefinition) as ColumnDefinition;
				table.ColumnDefinitions.Add(copy);
			}

			table.RowDefinitions.Add(new RowDefinition());
			for (var i = 0; i < table.ColumnDefinitions.Count; i++)
			{
				AddTableCell(table, row.Items[i], i, 0);
			}

			return table;
		}

		private void ArrangeTableHeader(IList<string> columnDefinitions, IList<Row> rows, IList<PageContent> pages)
		{
			_page.SetContentColumns(2);

			foreach (var row in rows)
			{
				var tr = RenderTableRow(columnDefinitions, row);

				if (_page.AddContent(tr)) continue;

				pages.Add(_page.GetPageContent());

				_page = CreateNewPage();
				_page.AddContent(tr);
			}
		}

		private void ArrangeTableRows(IList<string> columnDefinitions, DocumentModel.Table table, IList<PageContent> pages)
		{
			foreach (var row in table.Body)
			{
				var tr = RenderTableRow(columnDefinitions, row);

				if (_page.AddContent(tr)) continue;

				pages.Add(_page.GetPageContent());
				_page = CreateNewPage();
				if (table.RepeatHeader)
					ArrangeTableHeader(columnDefinitions, table.Header, pages);
				_page.AddContent(tr);
			}
		}

		private ColumnDefinition AddTableColumn(Grid grid, double totalColumnWidth, double columnWidth)
		{
			//var proportion = columnWidth / (_pageSize.Width - (_pageMargin.Left + _pageMargin.Right));

			var colDefinition = new ColumnDefinition {Width = new GridLength(columnWidth, GridUnitType.Pixel)};

			grid.ColumnDefinitions.Add(colDefinition);

			return colDefinition;
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
