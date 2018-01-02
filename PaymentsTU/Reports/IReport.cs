using System;

namespace PaymentsTU.Reports
{
	internal interface IReport
	{
		string Title { get; }

		DateTime From { get; set; }
		DateTime To { get; set; }

		bool IsPrintable { get; }

		bool CanPrint { get; }

		void Run();

		void Print();
	}
}
