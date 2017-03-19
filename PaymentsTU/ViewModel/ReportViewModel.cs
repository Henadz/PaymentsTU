using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentsTU.ViewModel
{
	public sealed class ReportViewModel : ViewModelBase, IPageBase
	{
		public string Title => "Отчеты";

		public DateTime PeriodStart { get; set; }

		//private DateTime _periodEnd;
		public DateTime PeriodEnd
		{
			get { return PeriodEnd; }
			set { OnPropertyChanged(nameof(PeriodEnd)); }
		}

		public ReportViewModel()
		{
			var now = DateTime.Now;
			PeriodStart = new DateTime(now.Year, now.Month, 1);
			PeriodEnd = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
		}

	}
}
