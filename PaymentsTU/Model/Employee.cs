using System;
using System.ComponentModel;

namespace PaymentsTU.Model
{
	public class Employee : ICloneable//, IDataErrorInfo
	{
		public long? Id { get; set; }
		public string Surname { get; set; }
		public string Name { get; set; }
		public string Patronymic { get; set; }
		public bool IsFired { get; set; }
		public string Note { get; set; }
		public long? DepartmentId { get; set; }
		public string FullName => string.Join(" ", new[] { Surname, Name, Patronymic }).Trim();
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		//public string this[string columnName]
		//{
		//	get
		//	{
		//		string message = null;
		//		switch (columnName)
		//		{
		//			case nameof(Surname):
		//				if (string.IsNullOrEmpty(this.Surname))
		//					message = "Должно быть заполнено";
		//				break;
		//			default:
		//				message = null;
		//				break;
		//		}
		//		return message;
		//	}
		//}

		//public string Error { get; }
	}
}