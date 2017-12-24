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
using FrameworkExtend;
using System.Windows.Controls;

namespace PaymentsTU.Document
{
	internal class XpsRenderStrategy : IRenderStrategy
	{
		private FixedDocument _document;
		private Size _pageSize;
		private Thickness _pageMargin;
		private double _freeHeight = 0;
		private FixedPage _page;

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
							pages.Add(ToPageContent(_page));
							_page = CreateNewPage();
						}
					}
					else if (artifact is DocumentModel.Paragraph)
					{
						_page = _page ?? CreateNewPage();

						var p = RenderParagraph((DocumentModel.Paragraph)artifact);

						var height = MeasureHeight(p);

						if (height <= _freeHeight)
						{
							_page.Children.Add(p);
						}
						else
						{
							pages.Add(ToPageContent(_page));
							_page = CreateNewPage();
							_page.Children.Add(p);
						}
					}
					else if (artifact is DocumentModel.Table)
					{
						_page = _page ?? CreateNewPage();

						RenderTableContent((DocumentModel.Table)artifact);
					}
				}
			}

			if (_page != null)
				pages.Add(ToPageContent(_page));

			return pages;
		}

		private void RenderTableContent(DocumentModel.Table artifact)
		{
			var grid = CreateTableControl(artifact);

			_page.Children.Add(grid);
		}

		private System.Windows.Controls.TextBlock RenderParagraph(DocumentModel.Paragraph p, double? desiredWidth = null)
		{
			var tb = new System.Windows.Controls.TextBlock()
			{
				TextWrapping = TextWrapping.Wrap,
				TextAlignment = ToWindowsAligment(p.Alignment)
			};
			tb.Width = desiredWidth ?? _pageSize.Width - (_pageMargin.Left + _pageMargin.Right);

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

			//Build the page inc header and footer
			//var pageGrid = new Grid();

			////Header row
			//AddGridRow(pageGrid, GridLength.Auto);

			////Content row
			//AddGridRow(pageGrid, new GridLength(1.0d, GridUnitType.Star));

			////Footer row
			//AddGridRow(pageGrid, GridLength.Auto);

			//ContentControl pageHeader = new ContentControl();
			//pageHeader.Content = this.CreateDocumentHeader();
			//pageGrid.Children.Add(pageHeader);

			//if (content != null)
			//{
			//	content.SetValue(Grid.RowProperty, 1);
			//	pageGrid.Children.Add(content);
			//}

			//ContentControl pageFooter = new ContentControl();
			//pageFooter.Content = CreateDocumentFooter(pageNumber + 1);
			//pageFooter.SetValue(Grid.RowProperty, 2);

			//pageGrid.Children.Add(pageFooter);

			//double width = this.PageSize.Width - (this.PageMargin.Left + this.PageMargin.Right);
			//double height = this.PageSize.Height - (this.PageMargin.Top + this.PageMargin.Bottom);

			//pageGrid.Measure(new Size(width, height));
			//pageGrid.Arrange(new Rect(this.PageMargin.Left, this.PageMargin.Top, width, height));

			return page;
		}

		private PageContent ToPageContent(FixedPage page)
		{
			//page.Measure(_pageSize);
			//page.Arrange(new Rect(_pageMargin.Left, _pageMargin.Top, _pageSize.Width, _pageSize.Height));
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

		private Grid CreateTableControl(DocumentModel.Table source)
		{
			Grid table = new Grid();

			var totalColumnWidth = 0d;
			foreach (var w in source.ColumnsWidth)
				totalColumnWidth += w;

			var totalColumns = source.ColumnsWidth.Count;

			var columnDefinitions = new List<ColumnDefinition>(source.ColumnsWidth.Count);
			for (var i = 0; i < source.ColumnsWidth.Count; i++)
			{
				columnDefinitions.Add(AddTableColumn(table, totalColumnWidth, source.ColumnsWidth[i]));
			}

			int rowIndex = 0;
			foreach (var row in source.Header)
			{
				for(var i = 0; i < totalColumns; i++)
				{
					AddTableCell(table, row.Items[i], i, rowIndex);
				}
				rowIndex++;
			}

			foreach(var row in source.Body)
			{
				table.RowDefinitions.Add(new RowDefinition());
				for (var i = 0; i < totalColumns; i++)
				{
					AddTableCell(table, row.Items[i], i, rowIndex);
				}
				rowIndex++;
			}

			//var height = MeasureHeight(table);

			//table.Measure(new Size(_pageSize.Width, _pageSize.Height));

			//	foreach (DataGridColumn column in _documentSource.Columns)
			//	{
			//		if (column.Visibility == Visibility.Visible)
			//		{
			//			AddTableColumn(table, totalColumnWidth, columnIndex, column);
			//			columnIndex++;
			//		}
			//	}

			//if (this.TableHeaderBorderStyle != null)
			//{
			//	Border headerBackground = new Border();
			//	headerBackground.Style = this.TableHeaderBorderStyle;
			//	headerBackground.SetValue(Grid.ColumnSpanProperty, columnIndex);
			//	headerBackground.SetValue(Grid.ZIndexProperty, -1);

			//	table.Children.Add(headerBackground);
			//}

			//if (createRowDefinitions)
			//{
			//	for (int i = 0; i <= _rowsPerPage; i++)
			//		table.RowDefinitions.Add(new RowDefinition());
			//}

			return table;

		}

		private ColumnDefinition AddTableColumn(Grid grid, double totalColumnWidth, double columnWidth)
		{
			//var proportion = columnWidth / (_pageSize.Width - (_pageMargin.Left + _pageMargin.Right));

			var colDefinition = new ColumnDefinition();
			colDefinition.Width = new GridLength(columnWidth, GridUnitType.Pixel);

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
