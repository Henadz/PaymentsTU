using FrameworkExtend;
using PaymentsTU.Model;
using System.Collections.ObjectModel;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using DocumentModel.Processor;
using System.IO;
using PaymentsTU.Document;
using System.Collections.Generic;

namespace PaymentsTU.ViewModel
{
	internal sealed class ReportPaymentForYearViewModel : ViewModelBase, IReport
	{
		public string Title => "Выплачено за год";

		public ObservableCollection<ReportRow> Rows { get; private set; }

		public ReportPaymentForYearViewModel()
		{
			Rows = new ObservableCollection<ReportRow>();
		}

		public void Run<T>(T parameters) where T : IPeriodReportParams
		{
			var rows = Dal.Instance.PaymentByEmployeeReport(parameters.StartDate, parameters.EndDate);
			Rows = new ObservableCollection<ReportRow>
				(
					rows
					.Select(x => new ReportRow { EmployeeId = x.EmployeeId, FullName = x.FullName, Total = x.Total, Warning = x.Total >= 270 })
					//.OrderByDescending(x => x.Total)
				);
			OnPropertyChanged(nameof(Rows));
		}

		public void Print()
		{
			using (var memoryStream = new MemoryStream())
			{
				var docBuilder = new DocumentBuilder(DocumentType.Xps, memoryStream);
				docBuilder.Build(PreparePrintDocumentModel, DocumentRendererFabric.GetDocumentRender);


				var pageSize = new PageMediaSize(PageMediaSizeName.ISOA4);
				var document = PrepareDocument();
				var printDialog = new PrintDialog();
				if (printDialog.ShowDialog() == true)
				{
					document.PageHeight = printDialog.PrintableAreaHeight;
					document.PageWidth = printDialog.PrintableAreaWidth;
					document.PagePadding = new Thickness(50, 15, 15, 15);
					document.ColumnGap = 0;
					document.ColumnWidth = printDialog.PrintableAreaWidth;

					printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, Title);
				}
			}
		}

		private IEnumerable<object> PreparePrintDocumentModel()
		{
			var title = new DocumentModel.FormattedText(Title);
			title.FontSize = 14;
			title.Bold = true;
			yield return new DocumentModel.Paragraph(title)
			{
				Alignment = DocumentModel.ParagraphAlignment.Center
			};

			var table = new DocumentModel.Table();
			var rows = new List<DocumentModel.Row>();

			var header = new List<DocumentModel.Cell>
			{
				new DocumentModel.Cell(new DocumentModel.FormattedText("Ф.И.О. работника"){ Bold = true }){IsHeaderCell = true},
				new DocumentModel.Cell(new DocumentModel.FormattedText("Всего выплачено, рублей"){ Bold = true }){IsHeaderCell = true}
			};

			rows.Add(new DocumentModel.Row(header));

			foreach (var row in Rows)
			{
				var r = new DocumentModel.Cell[]
				{
					new DocumentModel.Cell(row.FullName){IsTextWrapped = true},
					new DocumentModel.Cell(row.Total.ToString("#,###.00")){IsTextWrapped = true}
				};
				rows.Add(new DocumentModel.Row(r));
			}

			table.SetRows(rows);

			yield return table;

			yield break;
		}

		public sealed class ReportRow
		{
			public long EmployeeId { get; set; }
			public string FullName { get; set; }
			public decimal Total { get; set; }
			public bool Warning { get; set; }
		}

		private FixedDocument PrepareFixedDocument()
		{
			var document = new FixedDocument();
			var page = new FixedPage();
			var tb = new TextBlock() { Text = "Test" };
			tb.TextAlignment = TextAlignment.Left;
			page.Children.Add(tb);
			var grid = new Grid();
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300, GridUnitType.Pixel) });
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			page.Children.Add(grid);
			var content = new PageContent();
			((System.Windows.Markup.IAddChild)content).AddChild(page);
			document.Pages.Add(content);

			return document;
		}

		private FlowDocument PrepareDocument()
		{
			var document = new FlowDocument();

			var section = new Section();
			var title = new Paragraph(new Run(Title));
			title.TextAlignment = TextAlignment.Center;
			title.FontSize = 14;
			title.FontStyle = FontStyles.Normal;
			title.FontWeight = FontWeights.Bold;

			section.Blocks.Add(title);
			document.Blocks.Add(section);

			var table = new Table();
			table.CellSpacing = 2;
			table.Background = Brushes.White;
			table.Columns.Add(new TableColumn());
			table.Columns.Add(new TableColumn());

			var rowGroup = new TableRowGroup();
			table.RowGroups.Add(rowGroup);
			var headerRow = new TableRow();

			headerRow.FontSize = 14;
			headerRow.FontWeight = FontWeights.Bold;

			headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Ф.И.О. работника"))));
			headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Всего выплачено, рублей"))));

			rowGroup.Rows.Add(headerRow);

			foreach(var row in Rows)
			{
				var tableRow = new TableRow();
				tableRow.FontSize = 12;
				tableRow.FontWeight = FontWeights.Normal;
				tableRow.Cells.Add(new TableCell(new Paragraph(new Run(row.FullName))));
				tableRow.Cells.Add(new TableCell(new Paragraph(new Run(row.Total.ToString("#,###.00")))));
				rowGroup.Rows.Add(tableRow);
			}

			document.Blocks.Add(table);

			return document;
		}
	}
}
