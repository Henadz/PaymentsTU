using System;
using System.Collections;

namespace PaymentsTU.Model
{
	public sealed class Department : ICloneable
	{
		public long? Id { get; set; }
		public string Name { get; set; }
		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	internal class DepartmentComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			return string.Compare(((Department)x).Name, ((Department)y).Name, StringComparison.CurrentCulture);
		}
	}
}