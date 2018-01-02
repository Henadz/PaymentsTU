using FrameworkExtend;
using PaymentsTU.Model;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using DocumentModel.Processor;
using System.IO;
using PaymentsTU.Document;
using System.Collections.Generic;
using System.IO.Packaging;
using System;
using System.Windows.Xps.Packaging;
using DocumentModel;
using PaymentsTU.Reports;
using Page = DocumentModel.Processor.Page;

namespace PaymentsTU.ViewModel
{
	internal sealed class ReportPaymentForYearViewModel : ViewModelBase, IReport
	{
		public string Title => "Выплачено за период";

		public ObservableCollection<ReportRow> Rows { get; private set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }
		public bool IsPrintable => true;
		public bool CanPrint => IsPrintable && Rows.Count > 0;

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

				printDialog.UserPageRangeEnabled = true;

				var pageSettings = new Page
				{
					PageSize = new System.Drawing.Size(794, 1123),
					PageMargin = new Margin(Utility.CentimeterToPixel(1.5), Utility.CentimeterToPixel(1), Utility.CentimeterToPixel(1), Utility.CentimeterToPixel(1.5))
				};
				var docBuilder = new DocumentBuilder(DocumentType.Xps, pageSettings, memoryStream);
				docBuilder.Build(PreparePrintDocumentModel, DocumentRendererFabric.GetDocumentRender);
				docBuilder.DocumentStream.Position = 0;

				using (var package = Package.Open(docBuilder.DocumentStream, FileMode.Open, FileAccess.Read))
				{
					string inMemoryPackageName = $"memorystream://{Guid.NewGuid()}.xps";
					var packageUri = new Uri(inMemoryPackageName);

					PackageStore.AddPackage(packageUri, package);

					var xpsDoc = new XpsDocument(package, CompressionOption.Fast, inMemoryPackageName);
					var fixedDocumentSequence = xpsDoc.GetFixedDocumentSequence();

					if (printDialog.ShowDialog() == true && fixedDocumentSequence != null)
					{
						printDialog.PrintDocument(fixedDocumentSequence.DocumentPaginator, Title);
					}

					PackageStore.RemovePackage(packageUri);
					xpsDoc.Close();
				}
			}
		}

		private IEnumerable<object> PreparePrintDocumentModel()
		{
			var title = new FormattedText(Title)
			{
				FontSize = 14,
				Bold = true,
				LineBreak = true
			};
			var period = new FormattedText($"с {From:dd.MM.yyyy} по {To:dd.MM.yyyy}");
			yield return new Paragraph(new[] { title, period })
			{
				Alignment = ParagraphAlignment.Center
			};

			var table = new Table(2, SizeUnit.Pixels) {LayoutMode = TableLayoutMode.UseAllWidth};
			table.SetColumnsWidth(180d, 80d);

			var header = new List<DocumentModel.Cell>
			{
				new DocumentModel.Cell(new FormattedText("Ф.И.О. работника"){ Bold = true, FontSize = 12 }),
				new DocumentModel.Cell(new FormattedText("Сумма, руб."){ Bold = true, FontSize = 12 }){Alignment = ParagraphAlignment.Right}
			};

			table.SetHeader(new[] { new DocumentModel.Row(header) });

			var rows = new List<DocumentModel.Row>();
			foreach (var row in Rows)
			{
				var r = new []
				{
					new DocumentModel.Cell(row.FullName),
					new DocumentModel.Cell(row.Total.ToString("#,###.00")){Alignment = ParagraphAlignment.Right}
				};
				rows.Add(new DocumentModel.Row(r));
			}

			table.SetRows(rows);

			yield return table;
		}

		public sealed class ReportRow
		{
			public long EmployeeId { get; set; }
			public string FullName { get; set; }
			public decimal Total { get; set; }
			public bool Warning { get; set; }
		}
	}
}
