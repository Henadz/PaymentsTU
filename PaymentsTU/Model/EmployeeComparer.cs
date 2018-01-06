using System;
using System.Collections;
using System.Collections.Generic;

namespace PaymentsTU.Model
{
	internal class EmployeeComparer : IComparer<Employee>, IComparer
	{
		public int Compare(Employee x, Employee y)
		{
			return string.Compare(x?.FullName, y?.FullName, StringComparison.CurrentCulture);
		}

		int IComparer.Compare(object x, object y)
		{
			return Compare((Employee) x, (Employee) y);
		}
	}
}
