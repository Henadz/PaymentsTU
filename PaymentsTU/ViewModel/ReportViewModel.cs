using PaymentsTU.Reports;
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

		private DateTime _periodStart;
		public DateTime PeriodStart
		{
			get { return _periodStart; }
			set { _periodStart = value; OnPropertyChanged(nameof(PeriodStart)); }
		}

		private DateTime _periodEnd;
		public DateTime PeriodEnd
		{
			get { return _periodEnd; }
			set { _periodEnd = value; OnPropertyChanged(nameof(PeriodEnd)); }
		}

		public ICommand RunCommand { get; set; } = null;
		public ICommand PrintCommand { get; set; } = null;

		private IReport _report;
		public IReport Report
		{
			get { return _report; }
			set
			{
				if (value == null)
					return;
				_report = value;
				OnPropertyChanged(nameof(Report));
				var now = DateTime.Now;
				if (_report is ReportPaymentForYearViewModel)
				{
					PeriodStart = new DateTime(now.Year, 1, 1);
					PeriodEnd = new DateTime(now.Year, 12, 31);
				}
				else
				{
					PeriodStart = new DateTime(now.Year, now.Month, 1);
					PeriodEnd = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
				}
			}
		}

		public ObservableCollection<IReport> Reports { get; private set; }

		public ReportViewModel()
		{
			var now = DateTime.Now;
			PeriodStart = new DateTime(now.Year, now.Month, 1);
			PeriodEnd = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

			RunCommand = new RelayCommand(OnRunCommand, _ => PeriodStart <= PeriodEnd);
			PrintCommand = new RelayCommand(OnPrintCommand, _ => PeriodStart <= PeriodEnd);

			Reports = new ObservableCollection<IReport>
				(
					new List<IReport>
					{
						new PaymentReportViewModel(),
						new ReportPaymentForYearViewModel()
					}
				);

			_report = Reports[0];
		}

		private void OnRunCommand()
		{
			_report.From = PeriodStart;
			_report.To = PeriodEnd;
			_report.Run();
			//OnPropertyChanged(nameof(Report));
		}

		private void OnPrintCommand()
		{
			_report.Print();
		}
	}
}
