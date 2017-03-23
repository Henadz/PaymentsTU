using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace PaymentsTU.ViewModel
{
	internal sealed class ReportViewModel : ViewModelBase, IPageBase
	{
		public string Title => "Отчеты";

		public DateTime PeriodStart { get; set; }

		private DateTime _periodEnd;
		public DateTime PeriodEnd
		{
			get { return _periodEnd; }
			set { _periodEnd = value; OnPropertyChanged(nameof(PeriodEnd)); }
		}

		public ICommand RunCommand { get; set; } = null;

		public PaymentReportViewModel Report { get; private set; }

		public ReportViewModel()
		{
			var now = DateTime.Now;
			PeriodStart = new DateTime(now.Year, now.Month, 1);
			PeriodEnd = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

			RunCommand = new RelayCommand(OnRunCommand, _ => PeriodStart <= PeriodEnd);

			Report = new PaymentReportViewModel();
		}

		private void OnRunCommand()
		{
			Report.Run(new PaymentReportParams { StartDate = PeriodStart, EndDate = PeriodEnd });
			OnPropertyChanged(nameof(Report));
		}
	}
}
