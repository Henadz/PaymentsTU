using FrameworkExtend;
using PaymentsTU.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PaymentsTU.ViewModel
{
	internal sealed class PaymentReportViewModel : ViewModelBase, IReport
	{
		public string Title => "Отчет по платежам";

		public ObservableCollection<Row> Cells { get; private set; }

		public int RowsCount { get; private set; }
		public int ColumnsCount { get; private set; }

		public ObservableCollection<ColumnDescriptor> Columns { get; private set; }
	

		public PaymentReportViewModel()
		{
			Cells = new ObservableCollection<Row>();
			Columns = new ObservableCollection<ColumnDescriptor>();
			//Items = new ObservableCollection<Payment>(Dal.Instance.Payments().OrderByDescending(x => x.DatePayment).ThenBy(x => x.FullName));
			//ItemsDataView.Culture = CultureInfo.CurrentCulture;
			//ItemsDataView.MoveCurrentToPosition(Items.Count > 0 ? 0 : -1);
			////TODO: refactoring viewmodel for navigation bar
			//NavigationBar = new DataNavigationBarViewModel<Payment>(ItemsDataView, OnAdd, OnDelete, OnEdit, () =>
			//{
			//	Items.Clear();
			//	foreach (var payment in Dal.Instance.Payments())
			//	{
			//		Items.Add(payment);
			//	}
			//	ItemsDataView.Refresh();
			//	ItemsDataView.MoveCurrentToPosition(Items.Count > 0 ? 0 : -1);
			//});
		}

		public void Run<T>(T parameters) where T: IPeriodReportParams
		{
			var reportData = Dal.Instance.PaymentReport(parameters.StartDate, parameters.EndDate);

			var cols = new List<ColumnDescriptor>();

			foreach (var col in reportData.Columns)
			{
				if (col.IsVisible)
					cols.Add(new ColumnDescriptor { HeaderText = col.Caption, DisplayMember = "Cells[" + col.Ordinal + "].Value" });
			}

			Columns = new ObservableCollection<ColumnDescriptor>(cols);

			RowsCount = reportData.Rows.Count;
			ColumnsCount = reportData.Columns.Count;
			OnPropertyChanged(nameof(RowsCount));
			OnPropertyChanged(nameof(ColumnsCount));

			//var cells = new List<PaymentMatrixCell>();
			//foreach (var r in reportData.Rows)
			//{
			//	cells.AddRange(r);
			//}
			Cells = new ObservableCollection<Row>(reportData.Rows);
			
		}

		
	}

	public class PaymentReportParams: IPeriodReportParams
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}

	public class ColumnDescriptor
	{
		public string HeaderText { get; set; }
		public string DisplayMember { get; set; }
	}
}
