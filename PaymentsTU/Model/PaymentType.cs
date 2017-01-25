using System;
using System.Collections;

namespace PaymentsTU.Model
{
	public sealed class PaymentType : ICloneable
	{
		public long? Id { get; set; }
		public string Name { get; set; }
		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	internal class PaymentTypeComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			return string.Compare(((PaymentType)x).Name, ((PaymentType)y).Name, StringComparison.CurrentCulture);
		}
	}
}