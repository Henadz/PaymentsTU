using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentsTU.ViewModel
{
	internal interface IReport
	{
		string Title { get; }
		void Run<T>(T parameters) where T: IPeriodReportParams;
	}
}
