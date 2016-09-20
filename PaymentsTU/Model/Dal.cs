using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using PaymentsTU.Database;

namespace PaymentsTU.Model
{
	public sealed class Dal
	{
		private Dal()
		{
			_connectionString = ConfigurationManager.ConnectionStrings["Payments"].ConnectionString;
		}

		public static Dal Instance => Singleton.SingletonInstance;

		// ReSharper disable once ClassNeverInstantiated.Local
		private class Singleton
		{
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			static Singleton()
			{
			}

			internal static readonly Dal SingletonInstance = new Dal();
		}

		private readonly string _connectionString;

		public static IList<FinancialPeriod> FinancialPeriods()
		{
			return new[]
			{
				new FinancialPeriod
				{
					Id = 0,
					StartDate = new DateTime(2016, 1, 1),
					EndDate = new DateTime(2016, 12, 31),
					IsClosed = false,
					PaymentLimit = (decimal) 260.00
				}
			};
		}

		public bool ClosePeriod(int id)
		{
			return id >= 0;
		}

		public int UpdatePeriod(FinancialPeriod period)
		{
			return period.Id.HasValue ? 1 : 0;
		}

		public PaymentMatrix PaymentsInformation(DateTime startDate, DateTime endDate)
		{
			return new PaymentMatrix();
		}

		internal IList<Department> Departments()
		{
			return new[]
			{
				new Department
				{
					Id = 0,
					Name = "Viktoria 1"
				},
				new Department
				{
					Id = 1,
					Name = "Henadz 2"
				},
				new Department
				{
					Id = 3,
					Name = "Department 3"
				}
			};
		}

		public IEnumerable<Employee> Employees()
		{
			var resultset = new List<Employee>();
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				//var statement = "SELECT * FROM Cars LIMIT 5";
				const string statement = "SELECT Id, Surname, Name, Patronimic, IsFired, Note FROM Employee";

				using (var command = new SQLiteCommand(statement, connection))
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							resultset.Add(new Employee
							{
								Id = reader.GetInt64(0),
								Surname = DataReaderExtensions.SafeGetString(reader, 1),
								Name = DataReaderExtensions.SafeGetString(reader, 2),
								Patronymic = DataReaderExtensions.SafeGetString(reader, 3),
								IsFired = reader.GetBoolean(4),
								Note = DataReaderExtensions.SafeGetString(reader, 5)
							});
						}
					}
				}

				connection.Close();
			}

			return resultset;
		}

		public bool SaveEmployee(Employee employee)
		{
			int result;
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText = employee.Id.HasValue 
						? "UPDATE Employee SET Surname = @Surname, Name = @Name, Patronimic = @Patronimic, IsFired = @IsFired, Note = @Note WHERE Id = @Id"
						: "INSERT INTO Employee(Surname, Name, Patronimic, IsFired, Note) VALUES(@Surname, @Name, @Patronimic, @IsFired, @Note)";
					command.Prepare();

					if (employee.Id.HasValue)
						command.Parameters.AddWithValue("@Id", employee.Id.Value);
					command.Parameters.AddWithValue("@Surname", employee.Surname);
					command.Parameters.AddWithValue("@Name", employee.Name);
					command.Parameters.AddWithValue("@Patronimic", employee.Patronymic);
					command.Parameters.AddWithValue("@IsFired", Convert.ToInt32(employee.IsFired));
					command.Parameters.AddWithValue("@Note", employee.Note);
					result = command.ExecuteNonQuery();

					if (!employee.Id.HasValue)
						employee.Id = connection.LastInsertRowId;
				}

				connection.Close();
			}

			return result > 0;
		}

		public IList<PaymentType> PaymentTypes()
		{
			return new[]
			{
				new PaymentType
				{
					Id = 0,
					Name = "Viktoria"
				},
				new PaymentType
				{
					Id = 1,
					Name = "Henadz"
				}
			};
		}

		
	}
}