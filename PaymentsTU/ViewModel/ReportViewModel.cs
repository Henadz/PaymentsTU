using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		private IReport _report;
		public IReport Report
		{
			get { return _report; }
			set { _report = value; OnPropertyChanged(nameof(Report)); }
		}

		public ObservableCollection<IReport> Reports { get; private set; }

		public ReportViewModel()
		{
			var now = DateTime.Now;
			PeriodStart = new DateTime(now.Year, now.Month, 1);
			PeriodEnd = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

			RunCommand = new RelayCommand(OnRunCommand, _ => PeriodStart <= PeriodEnd);

			Reports = new ObservableCollection<IReport>
				(
					new List<IReport>
					{
						new PaymentReportViewModel()
					}
				);

			_report = Reports[0];
		}

		private void OnRunCommand()
		{
			_report.Run(new PaymentReportParams { StartDate = PeriodStart, EndDate = PeriodEnd });
			//OnPropertyChanged(nameof(Report));
		}
	}
}
