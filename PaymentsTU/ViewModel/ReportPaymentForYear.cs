using FrameworkExtend;
using PaymentsTU.Model;
using System.Collections.ObjectModel;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

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

		public sealed class ReportRow
		{
			public long EmployeeId { get; set; }
			public string FullName { get; set; }
			public decimal Total { get; set; }
			public bool Warning { get; set; }
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
			table.CellSpacing = 10;
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
