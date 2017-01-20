using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SQLite;
using System.Text;
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

		public IEnumerable<Employee> Employees(bool onlyActive = false)
		{
			var resultset = new List<Employee>();
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				var statement = new StringBuilder("SELECT Id, Surname, Name, Patronimic, IsFired, Note FROM Employee");
				if (onlyActive)
					statement.AppendLine("WHERE IsFired = 1");

				using (var command = new SQLiteCommand(statement.ToString(), connection))
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
						? "UPDATE Employee SET Surname = @Surname, Name = @Name, Patronimic = @Patronimic, IsFired = @IsFired, Note = @Note, DepartmentId = @DepartmentId WHERE Id = @Id"
						: "INSERT INTO Employee(Surname, Name, Patronimic, IsFired, Note, DepartmentId) VALUES(@Surname, @Name, @Patronimic, @IsFired, @Note, @DepartmentId)";
					command.Prepare();

					if (employee.Id.HasValue)
						command.Parameters.AddWithValue("@Id", employee.Id.Value);
					command.Parameters.AddWithValue("@Surname", employee.Surname.Trim());
					command.Parameters.AddWithValue("@Name", employee.Name.Trim());
					command.Parameters.AddWithValue("@Patronimic", employee.Patronymic?.Trim());
					command.Parameters.AddWithValue("@IsFired", Convert.ToInt32(employee.IsFired));
					command.Parameters.AddWithValue("@Note", employee.Note);
					command.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
					result = command.ExecuteNonQuery();

					if (!employee.Id.HasValue)
						employee.Id = connection.LastInsertRowId;
				}

				connection.Close();
			}

			return result > 0;
		}

		public bool DeleteEmployee(Employee employee)
		{
			int result;
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText = "DELETE FROM Employee WHERE Id = @Id";
					command.Prepare();

					if (employee.Id.HasValue)
					{
						command.Parameters.AddWithValue("@Id", employee.Id.Value);
						result = command.ExecuteNonQuery();
					}
					else
					{
						result = 1;
					}
				}

				connection.Close();
			}

			return result > 0;
		}

		#region PaymentType
		public IList<PaymentType> PaymentTypes()
		{
			var resultset = new List<PaymentType>();
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				const string statement = "SELECT Id, Name FROM PaymentType";

				using (var command = new SQLiteCommand(statement, connection))
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							resultset.Add(new PaymentType
							{
								Id = reader.GetInt64(0),
								Name = DataReaderExtensions.SafeGetString(reader, 1)
							});
						}
					}
				}

				connection.Close();
			}

			return resultset;
		}

		internal bool SavePaymentType(PaymentType record)
		{
			int result;
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText = record.Id.HasValue
						? "UPDATE PaymentType SET Name = @Name WHERE Id = @Id"
						: "INSERT INTO PaymentType(Name) VALUES(@Name)";
					command.Prepare();

					if (record.Id.HasValue)
						command.Parameters.AddWithValue("@Id", record.Id.Value);
					command.Parameters.AddWithValue("@Name", record.Name);
					result = command.ExecuteNonQuery();

					if (!record.Id.HasValue)
						record.Id = connection.LastInsertRowId;
				}

				connection.Close();
			}

			return result > 0;
		}

		public bool DeletePaymentType(PaymentType record)
		{
			int result;
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText = "DELETE FROM PaymentType WHERE Id = @Id";
					command.Prepare();

					if (record.Id.HasValue)
					{
						command.Parameters.AddWithValue("@Id", record.Id.Value);
						result = command.ExecuteNonQuery();
					}
					else
					{
						result = 1;
					}
				}

				connection.Close();
			}

			return result > 0;
		}
		#endregion

		#region Department
		public IList<Department> Departments()
		{
			var resultset = new List<Department>();
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				const string statement = "SELECT Id, Name FROM Department";

				using (var command = new SQLiteCommand(statement, connection))
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							resultset.Add(new Department
							{
								Id = reader.GetInt64(0),
								Name = DataReaderExtensions.SafeGetString(reader, 1)
							});
						}
					}
				}

				connection.Close();
			}

			return resultset;
		}

		internal bool SaveDepartment(Department record)
		{
			int result;
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText = record.Id.HasValue
						? "UPDATE Department SET Name = @Name WHERE Id = @Id"
						: "INSERT INTO Department(Name) VALUES(@Name)";
					command.Prepare();

					if (record.Id.HasValue)
						command.Parameters.AddWithValue("@Id", record.Id.Value);
					command.Parameters.AddWithValue("@Name", record.Name);
					result = command.ExecuteNonQuery();

					if (!record.Id.HasValue)
						record.Id = connection.LastInsertRowId;
				}

				connection.Close();
			}

			return result > 0;
		}

		public bool DeleteDepartment(Department record)
		{
			int result;
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText = "DELETE FROM Department WHERE Id = @Id";
					command.Prepare();

					if (record.Id.HasValue)
					{
						command.Parameters.AddWithValue("@Id", record.Id.Value);
						result = command.ExecuteNonQuery();
					}
					else
					{
						result = 1;
					}
				}

				connection.Close();
			}

			return result > 0;
		}
		#endregion

		#region Payments
		internal List<Payment> Payments()
		{
			var resultset = new List<Payment>();
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				const string statement =
					"SELECT p.Id, p.EmployeeId, e.Name, e.Surname, e.Patronimic, p.PaymentTypeId, pt.Name as PaymentType, p.DepartmentId, d.Name as Department, p.DatePayment, p.Value, p.CurrencyCode, c.Name as Currency "
					+ "FROM Payment p "
					+ "INNER JOIN Employee e ON p.EmployeeId = e.Id "
					+ "INNER JOIN Department d ON p.DepartmentId = d.Id "
					+ "INNER JOIN PaymentType pt ON p.PaymentTypeId = pt.Id "
					+ "INNER JOIN Currency c ON p.CurrencyCode = c.Code";

				using (var command = new SQLiteCommand(statement, connection))
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							resultset.Add(new Payment
							{
								Id = reader.GetInt64(0),
								EmployeeId = reader.GetInt32(1),
								Name = DataReaderExtensions.SafeGetString(reader, 2),
								Surname = DataReaderExtensions.SafeGetString(reader, 3),
								Patronimic = DataReaderExtensions.SafeGetString(reader, 4),
								PaymentTypeId = reader.GetInt32(5),
								PaymentType = DataReaderExtensions.SafeGetString(reader, 6),
								DepartmentId = reader.GetInt32(7),
								Department = DataReaderExtensions.SafeGetString(reader, 8),
								DatePayment = DateTime.Today,
								Value = reader.GetDecimal(10),
								CurrencyId = reader.GetInt32(11),
								Currency = DataReaderExtensions.SafeGetString(reader, 12)
							});
						}
					}
				}

				connection.Close();
			}

			return resultset;
		}

		internal bool DeletePayment(Payment record)
		{
			int result;
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText = "DELETE FROM Payment WHERE Id = @Id";
					command.Prepare();

					if (record.Id.HasValue)
					{
						command.Parameters.AddWithValue("@Id", record.Id.Value);
						result = command.ExecuteNonQuery();
					}
					else
					{
						result = 1;
					}
				}

				connection.Close();
			}

			return result > 0;
		}
		#endregion
	}
}