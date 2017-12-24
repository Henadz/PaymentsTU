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
using System.IO.Packaging;
using System;
using System.Windows.Xps.Packaging;
using PaymentsTU.Reports;

namespace PaymentsTU.ViewModel
{
	internal sealed class ReportPaymentForYearViewModel : ViewModelBase, IReport
	{
		public string Title => "Выплачено за период";

		public ObservableCollection<ReportRow> Rows { get; private set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }

		public ReportPaymentForYearViewModel()
		{
			Rows = new ObservableCollection<ReportRow>();
		}

		public void Run()
		{
			var rows = Dal.Instance.PaymentByEmployeeReport(From, To);
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
				var printDialog = new PrintDialog();
				//PageMediaSize pageSize = null;
				//bool bA4 = false;

				//if (bA4)
				//{
				//	pageSize = new PageMediaSize(PageMediaSizeName.NorthAmericaLetter);
				//}
				//else
				//{
				//	pageSize = new PageMediaSize(PageMediaSizeName.ISOA4);
				//}

				//printDialog.PrintTicket.PageMediaSize = pageSize;

				//printDialog.ShowDialog();

				//PrintTicket pt = printDialog.PrintTicket;
				//Double printableWidth = pt.PageMediaSize.Width.Value;
				//Double printableHeight = pt.PageMediaSize.Height.Value;

				//var pageSize2 = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

				var docBuilder = new DocumentBuilder(DocumentType.Xps, memoryStream);
				docBuilder.Build(PreparePrintDocumentModel, DocumentRendererFabric.GetDocumentRender);
				docBuilder.DocumentStream.Position = 0;

				var package = Package.Open(docBuilder.DocumentStream, FileMode.Open, FileAccess.Read);

				//Create URI for Xps Package
				//Any Uri will actually be fine here. It acts as a place holder for the
				//Uri of the package inside of the PackageStore
				string inMemoryPackageName = string.Format("memorystream://{0}.xps", Guid.NewGuid());
				var packageUri = new Uri(inMemoryPackageName);

				//Add package to PackageStore
				PackageStore.AddPackage(packageUri, package);

				var xpsDoc = new XpsDocument(package, CompressionOption.Fast, inMemoryPackageName);
				var fixedDocumentSequence = xpsDoc.GetFixedDocumentSequence();

				//var pageSize = new PageMediaSize(PageMediaSizeName.ISOA4);
				//var document = PrepareDocument();
				
				if (printDialog.ShowDialog() == true)
				{
					printDialog.PrintDocument(fixedDocumentSequence.DocumentPaginator, Title);
				}

				PackageStore.RemovePackage(packageUri);
				xpsDoc.Close();
			}
		}

		private IEnumerable<object> PreparePrintDocumentModel()
		{
			var title = new DocumentModel.FormattedText(Title);
			title.FontSize = 14;
			title.Bold = true;
			title.LineBreak = true;
			var period = new DocumentModel.FormattedText($"с {From.ToString("dd.MM.yyyy")} по {To.ToString("dd.MM.yyyy")}");
			yield return new DocumentModel.Paragraph(new[] { title, period })
			{
				Alignment = DocumentModel.ParagraphAlignment.Center
			};

			var table = new DocumentModel.Table(DocumentModel.SizeUnit.Pixels);
			table.SetColumnsWidth(new[] { 200d, 80d });
			

			var header = new List<DocumentModel.Cell>
			{
				new DocumentModel.Cell(new DocumentModel.FormattedText("Ф.И.О. работника"){ Bold = true, FontSize = 12 }),
				new DocumentModel.Cell(new DocumentModel.FormattedText("Сумма, руб."){ Bold = true, FontSize = 12 }){IsTextWrapped = false, Alignment = DocumentModel.ParagraphAlignment.Right}
			};

			table.SetHeader(new[] { new DocumentModel.Row(header) });

			var rows = new List<DocumentModel.Row>();
			foreach (var row in Rows)
			{
				var r = new DocumentModel.Cell[]
				{
					new DocumentModel.Cell(row.FullName),
					new DocumentModel.Cell(row.Total.ToString("#,###.00")){Alignment = DocumentModel.ParagraphAlignment.Right}
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
				tableRow.Cells.Add(new TableCell(new Paragraph(new Run(row.Total.ToString("#,###.00"))) { TextAlignment = TextAlignment.Right }));
				rowGroup.Rows.Add(tableRow);
			}

			document.Blocks.Add(table);

			return document;
		}
	}
}
