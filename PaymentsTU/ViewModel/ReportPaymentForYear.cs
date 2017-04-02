using FrameworkExtend;
using PaymentsTU.Model;
using PaymentsTU.Properties;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;

namespace PaymentsTU.ViewModel
{
	internal sealed class ReportPaymentForYearViewModel : ViewModelBase, IReport
	{
		private readonly decimal _warningLimit;
		public string Title => "Выплачено за период";

		public ObservableCollection<ReportRow> Rows { get; private set; }

		private IList<FinancialPeriod> _periods;

		public ReportPaymentForYearViewModel()
		{
			_warningLimit = Settings.Default.WarningLimitPercent;
			_periods = Dal.Instance.FinancialPeriods();

			Rows = new ObservableCollection<ReportRow>();
		}

		public void Run<T>(T parameters) where T : IPeriodReportParams
		{
			var start = parameters.StartDate;
			var end = parameters.EndDate;

			var rows = Dal.Instance.PaymentByEmployeeReport(start, end);
			var limit = _periods.FirstOrDefault(x => x.StartDate == start && x.EndDate == end)?.PaymentLimit;
			limit = limit.HasValue ? limit * (_warningLimit / 100) : null;

			Rows = new ObservableCollection<ReportRow>
				(
					rows
					.Select(x => new ReportRow { EmployeeId = x.EmployeeId, FullName = x.FullName, Total = x.Total, Warning = limit.HasValue ? x.Total >= limit : false })
					.OrderBy(x => x.FullName)
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
