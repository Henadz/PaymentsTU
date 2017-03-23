using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentsTU.ViewModel
{
	interface IPeriodReportParams
	{
		DateTime StartDate { get; set; }
		DateTime EndDate { get; set; }
	}
}
