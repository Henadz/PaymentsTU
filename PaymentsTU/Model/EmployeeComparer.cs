using System;
using System.Collections;

namespace PaymentsTU.Model
{
	internal class EmployeeComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			return string.Compare(((Employee)x).FullName, ((Employee)y).FullName, StringComparison.CurrentCulture);
		}
	}
}
