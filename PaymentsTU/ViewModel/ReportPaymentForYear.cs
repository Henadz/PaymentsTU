using FrameworkExtend;
using PaymentsTU.Model;
using PaymentsTU.Properties;
using System.Collections.ObjectModel;
using System.Configuration;

namespace PaymentsTU.ViewModel
{
	internal sealed class ReportPaymentForYearViewModel : ViewModelBase, IReport
	{
		private readonly decimal _warningLimit;
		public string Title => "Выплачено за период";

		public ObservableCollection<ReportRow> Rows { get; private set; }

		public ReportPaymentForYearViewModel()
		{
			_warningLimit = 90;//Settings.Default.Properties.
			Rows = new ObservableCollection<ReportRow>();
		}

		public void Run<T>(T parameters) where T : IPeriodReportParams
		{
			var rows = Dal.Instance.PaymentByEmployeeReport(parameters.StartDate, parameters.EndDate);
			Rows = new ObservableCollection<ReportRow>
				(
					rows
					.Select(x => new ReportRow { EmployeeId = x.EmployeeId, FullName = x.FullName, Total = x.Total, Warning = x.Total >= _warningLimit })
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
