using System;

namespace PaymentsTU.Reports
{
	internal interface IReport
	{
		string Title { get; }

		DateTime From { get; set; }
		DateTime To { get; set; }

		void Run();

		void Print();
	}
}
