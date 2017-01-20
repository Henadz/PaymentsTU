using System;
using System.Collections;

namespace PaymentsTU.Model
{
	public sealed class PaymentType
	{
		public long? Id { get; set; }
		public string Name { get; set; }
	}

	internal class PaymentTypeComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			return string.Compare(((PaymentType)x).Name, ((PaymentType)y).Name, StringComparison.CurrentCulture);
		}
	}
}