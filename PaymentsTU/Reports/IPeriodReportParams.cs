using System;

namespace PaymentsTU.Reports
{
	interface IPeriodReportParams
	{
		DateTime StartDate { get; set; }
		DateTime EndDate { get; set; }
	}
}
