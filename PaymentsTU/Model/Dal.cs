using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SQLite;
using System.Text;
using PaymentsTU.Database;
using FrameworkExtend;

namespace PaymentsTU.Model
{
	internal sealed class Dal
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

				var statement = new StringBuilder();
				statement.AppendLine("SELECT e.Id, e.Surname, e.Name, e.Patronimic, e.IsFired, e.Note, e.DepartmentId, d.Name  as Department ");
				statement.AppendLine("FROM Employee e ");
				statement.AppendLine("LEFT JOIN Department d ON e.DepartmentId = d.Id ");
				if (onlyActive)
					statement.AppendLine("WHERE IsFired = 0");

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
								Note = DataReaderExtensions.SafeGetString(reader, 5),
								DepartmentId = DataReaderExtensions.SafeGetLong(reader, 6),
								
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

		#region Currency
		public IList<Currency> Currencies()
		{
			var resultset = new List<Currency>();
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				const string statement = "SELECT * FROM Currency";

				using (var command = new SQLiteCommand(statement, connection))
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							resultset.Add(new Currency
							{
								DigitalCode = reader.GetInt32(0),
								Code = DataReaderExtensions.SafeGetString(reader, 1),
								Name = DataReaderExtensions.SafeGetString(reader, 2)
							});
						}
					}
				}

				connection.Close();
			}

			return resultset;
		}
		#endregion

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
		internal List<Payment> Payments(long? id = null)
		{
			var resultset = new List<Payment>();
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				var statement = new StringBuilder();
				statement.AppendLine(
					"SELECT p.Id, p.EmployeeId, e.Name, e.Surname, e.Patronimic, p.PaymentTypeId, pt.Name as PaymentType, p.DepartmentId, d.Name as Department, p.DatePayment, p.Value, p.CurrencyCode, c.Name as Currency ");
				statement.AppendLine("FROM Payment p ");
				statement.AppendLine("INNER JOIN Employee e ON p.EmployeeId = e.Id ");
				statement.AppendLine("INNER JOIN Department d ON p.DepartmentId = d.Id ");
				statement.AppendLine("INNER JOIN PaymentType pt ON p.PaymentTypeId = pt.Id ");
				statement.AppendLine("INNER JOIN Currency c ON p.CurrencyCode = c.DigitalCode");

				if (id.HasValue)
					statement.AppendLine("WHERE p.Id = @Id");

				using (var command = new SQLiteCommand(statement.ToString(), connection))
				{
					if (id.HasValue)
						command.Parameters.AddWithValue("@Id", id.Value);
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
								DatePayment = reader.GetDateTime(9),
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

		internal bool SavePayment(Payment record)
		{
			int result;
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText = record.Id.HasValue
						? "UPDATE Payment SET EmployeeId = @EmployeeId, PaymentTypeId = @PaymentTypeId, DepartmentId = @DepartmentId, DatePayment = @DatePayment, Value = @Value, CurrencyCode = @CurrencyCode WHERE Id = @Id"
						: "INSERT INTO Payment(EmployeeId, PaymentTypeId, DepartmentId, DatePayment, Value, CurrencyCode) VALUES(@EmployeeId, @PaymentTypeId, @DepartmentId, @DatePayment, @Value, @CurrencyCode)";
					command.Prepare();

					if (record.Id.HasValue)
						command.Parameters.AddWithValue("@Id", record.Id.Value);
					command.Parameters.AddWithValue("@EmployeeId", record.EmployeeId);
					command.Parameters.AddWithValue("@PaymentTypeId", record.PaymentTypeId);
					command.Parameters.AddWithValue("@DepartmentId", record.DepartmentId);
					command.Parameters.AddWithValue("@DatePayment", record.DatePayment);
					command.Parameters.AddWithValue("@Value", record.Value);
					command.Parameters.AddWithValue("@CurrencyCode", record.CurrencyId);
					result = command.ExecuteNonQuery();

					if (!record.Id.HasValue)
						record.Id = connection.LastInsertRowId;

					var recordData = Payments(record.Id)[0];
					record.Currency = recordData.Currency;
					record.Department = recordData.Department;
					record.PaymentType = recordData.PaymentType;
					record.Name = recordData.Name;
					record.Patronimic = recordData.Patronimic;
					record.Surname = recordData.Surname;
				}

				connection.Close();
			}

			return result > 0;
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

		#region Reports

		public PaymentMatrix PaymentReport(DateTime start, DateTime end)
		{
			//
			var reportColumns = new HashSet<Column>();// new Dictionary<int, Column>();
			var reportRows = new List<PaymentMatrixCell[]>();
			var crossTabColumn = new List<PaymentType>();
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
							crossTabColumn.Add(new PaymentType
							{
								Id = reader.GetInt64(0),
								Name = DataReaderExtensions.SafeGetString(reader, 1)
							});
						}
					}
				}

				var reportStatement = new StringBuilder("SELECT");
				var columnID = 0;
				reportStatement.AppendLine("EmployeeId");
				reportColumns.Add(new Column { ColumnName = "EmployeeId", DataType = typeof(long), Ordinal = ++columnID, IsVisible = false });
				reportStatement.AppendLine(",e.Surname");
				reportColumns.Add(new Column { ColumnName = "Surname", DataType = typeof(string), Ordinal = ++columnID, IsVisible = false });
				reportStatement.AppendLine(",e.Name");
				reportColumns.Add(new Column { ColumnName = "Name", DataType = typeof(string), Ordinal = ++columnID, IsVisible = false });
				reportStatement.AppendLine(",e.Patronimic");
				reportColumns.Add(new Column { ColumnName = "Patronimic", DataType = typeof(string), Ordinal = ++columnID, IsVisible = false });
				reportStatement.AppendLine(",DepartmentId");
				reportColumns.Add(new Column { ColumnName = "DepartmentId", DataType = typeof(long), Ordinal = ++columnID, IsVisible = false });
				reportStatement.AppendLine(",d.Name as 'Department'");
				reportColumns.Add(new Column { ColumnName = "Department", DataType = typeof(string), Ordinal = ++columnID });
				foreach (var t in crossTabColumn)
				{
					var columnName = $",PaymentType_{t.Id}";
					reportStatement.AppendLine(columnName);
					reportColumns.Add(new Column { ColumnName = columnName, Caption = t.Name, DataType = typeof(object), Ordinal = ++columnID });
				}
				reportStatement.AppendLine("FROM");
				reportStatement.AppendLine("(SELECT");
				reportStatement.AppendLine("p.EmployeeId");
				reportStatement.AppendLine(",p.DepartmentId");
				foreach (var t in crossTabColumn)
				{
					reportStatement.AppendLine($",sum(CASE WHEN p.PaymentTypeId = {t.Id} THEN p.Value END) as PaymentType_{t.Id}");
				}
				reportStatement.AppendLine("FROM Payment p");
				reportStatement.AppendLine("GROUP BY p.EmployeeId, p.DepartmentId");
				reportStatement.AppendLine(") x");
				reportStatement.AppendLine("INNER JOIN Employee e ON x.EmployeeId = e.Id");
				reportStatement.AppendLine("INNER JOIN Department d ON x.DepartmentId = d.Id");
				reportStatement.AppendLine("ORDER BY Department, Surname, e.Name, Patronimic");

				using (var command = new SQLiteCommand(reportStatement.ToString(), connection))
				{
					using (var reader = command.ExecuteReader())
					{
						bool setColumnsType = true;
						var rowid = 0;
						while (reader.Read())
						{
							var row = new List<PaymentMatrixCell>(reportColumns.Count);
							for (var c = 0; c < reader.FieldCount; c++)
							{
								var column = reportColumns.FirstOrDefault(x => x.Ordinal == c);
								if (column == null)
									continue;

								var type = reader.GetFieldType(c);
								if (setColumnsType)
								{
									column.DataType = type;
								}

								row.Add(new PaymentMatrixCell(type) {
									RowId = ++rowid,
									ColumnId = column.Ordinal,
									Value = reader.IsDBNull(c) ? null : reader.GetValue(c)
								});
								setColumnsType = false;
							}
							reportRows.Add(row.ToArray());
						}
					}
				}
				/*
				SELECT
    EmployeeId,
    e.Surname,
    e.Name ,
    e.Patronimic,
    DepartmentId,
    d.Name as "Department",
    PaymentTypeId1,
    PaymentTypeId2,
    PaymentTypeId3,
    PaymentTypeId4,
    PaymentTypeId5,
    PaymentTypeId6,
    PaymentTypeId7,
    PaymentTypeId8,
    PaymentTypeId9,
    "Продукты питания",
    PaymentTypeId11
FROM
    (
    SELECT 
        p.EmployeeId,
        p.DepartmentId,
        sum(CASE WHEN p.PaymentTypeId = 1 THEN p.Value END) as PaymentTypeId1,
        sum(CASE WHEN p.PaymentTypeId = 2 THEN p.Value END) as PaymentTypeId2,
        sum(CASE WHEN p.PaymentTypeId = 3 THEN p.Value END) as PaymentTypeId3,
        sum(CASE WHEN p.PaymentTypeId = 4 THEN p.Value END) as PaymentTypeId4,
        sum(CASE WHEN p.PaymentTypeId = 5 THEN p.Value END) as PaymentTypeId5,
        sum(CASE WHEN p.PaymentTypeId = 6 THEN p.Value END) as PaymentTypeId6,
        sum(CASE WHEN p.PaymentTypeId = 7 THEN p.Value END) as PaymentTypeId7,
        sum(CASE WHEN p.PaymentTypeId = 8 THEN p.Value END) as PaymentTypeId8,
        sum(CASE WHEN p.PaymentTypeId = 9 THEN p.Value END) as PaymentTypeId9,
        sum(CASE WHEN p.PaymentTypeId = 10 THEN p.Value END) as "Продукты питания",
        sum(CASE WHEN p.PaymentTypeId = 11 THEN p.Value END) as PaymentTypeId11
    FROM Payment p
    GROUP BY EmployeeId
    ) x
INNER JOIN Employee e
ON x.EmployeeId = e.Id
INNER JOIN Department d
ON x.DepartmentId = d.Id
ORDER BY Department, Surname, e.Name, Patronimic
				 */

				connection.Close();
			}

			return new PaymentMatrix
			{
				Columns = reportColumns.ToList(),
				Rows = reportRows
			};
		}
		#endregion
	}
}