﻿namespace PaymentsTU.Model
{
	public class Employee
	{
		public long? Id { get; set; } 
		public string Surname { get; set; }
		public string Name { get; set; }
		public string Patronymic { get; set; }
		public bool IsFired { get; set; }
		public string Note { get; set; }

		public string FullName => string.Join(" ", new[] { Surname, Name, Patronymic });
	}
}