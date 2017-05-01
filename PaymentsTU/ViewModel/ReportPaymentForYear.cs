using FrameworkExtend;
using PaymentsTU.Model;
using System.Collections.ObjectModel;

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

		public sealed class ReportRow
		{
			public long EmployeeId { get; set; }
			public string FullName { get; set; }
			public decimal Total { get; set; }
			public bool Warning { get; set; }
		}
	}
}
