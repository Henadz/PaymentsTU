﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Text;
using PaymentsTU.Database;
using FrameworkExtend;
using System.Data;

namespace PaymentsTU.Model
{
	internal sealed partial class Dal
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
					StartDate = new DateTime(2017, 1, 1),
					EndDate = new DateTime(2017, 12, 31),
					IsClosed = false,
					PaymentLimit = (decimal) 300.00
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
		internal List<Payment> Payments(DateTime? from = null, DateTime? to = null, long? id = null)
		{
			var withPeriod = from.HasValue && to.HasValue;

			if (from.HasValue && !to.HasValue)
				throw new ArgumentOutOfRangeException(nameof(to), @"Parametr should be specified");
			if (!from.HasValue && to.HasValue)
				throw new ArgumentOutOfRangeException(nameof(from), @"Parametr should be specified");

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
				statement.AppendLine("WHERE 1 = 1");

				if (id.HasValue)
					statement.AppendLine("AND p.Id = @Id");
				if (withPeriod)
					statement.AppendLine("AND (p.DatePayment >= @DateStart AND p.DatePayment <= @DateEnd)");

				using (var command = new SQLiteCommand(statement.ToString(), connection))
				{
					if (id.HasValue)
						command.Parameters.AddWithValue("@Id", id.Value);

					if (withPeriod)
					{
						command.Parameters.Add("@DateStart", DbType.DateTime).Value = from;
						command.Parameters.Add("@DateEnd", DbType.DateTime).Value = to;
					}

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

		internal List<Payment> Payments(long id)
		{
			return Payments(null, null, id);
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

					var recordData = Payments(record.Id.Value)[0];
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
			var reportColumns = new HashSet<Column>();
			var reportRows = new List<Row>();
			var crossTabColumn = new List<PaymentType>();
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				const string statement = "SELECT Id, Name FROM PaymentType ORDER BY Name";

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

				var reportStatement = new StringBuilder("SELECT ");
				var columnId = 0;
				reportStatement.AppendLine("x.EmployeeId");
				reportColumns.Add(new Column { ColumnName = "EmployeeId", DataType = typeof(long), Ordinal = columnId++, IsVisible = true });
				reportStatement.AppendLine(",IFNULL(e.Surname,'') || ' ' || IFNULL(e.Name,'') || ' ' || IFNULL(e.Patronimic,'')");
				reportColumns.Add(new Column { ColumnName = "Surname", Caption = "Ф.И.О.", DataType = typeof(string), Ordinal = columnId++, IsVisible = true });
				reportStatement.AppendLine(",x.DepartmentId");
				reportColumns.Add(new Column { ColumnName = "DepartmentId", DataType = typeof(long), Ordinal = columnId++, IsVisible = false });
				reportStatement.AppendLine(",d.Name as 'Department'");
				reportColumns.Add(new Column { ColumnName = "Department", DataType = typeof(string), Ordinal = columnId++ });
				foreach (var t in crossTabColumn)
				{
					var columnName = $",x.PaymentType_{t.Id}";
					reportStatement.AppendLine(columnName);
					reportColumns.Add(new Column { ColumnName = columnName.TrimStart(',', 'x','.'), Caption = t.Name, DataType = typeof(object), Ordinal = columnId++ });
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
				reportStatement.AppendLine("WHERE p.DatePayment >= @DateStart");
				reportStatement.AppendLine("AND p.DatePayment <= @DateEnd");
				reportStatement.AppendLine("GROUP BY p.EmployeeId, p.DepartmentId");
				reportStatement.AppendLine(") x");
				reportStatement.AppendLine("INNER JOIN Employee e ON x.EmployeeId = e.Id");
				reportStatement.AppendLine("INNER JOIN Department d ON x.DepartmentId = d.Id");
				reportStatement.AppendLine("ORDER BY Department, Surname, e.Name, Patronimic");

				using (var command = new SQLiteCommand(reportStatement.ToString(), connection))
				{
					command.Parameters.Add("@DateStart", DbType.DateTime).Value = start;
					command.Parameters.Add("@DateEnd", DbType.DateTime).Value = end;

					using (var reader = command.ExecuteReader())
					{
						var rowid = 0;
						while (reader.Read())
						{
							var row = new List<Cell>(reportColumns.Count);
							for (var c = 0; c < reader.FieldCount; c++)
							{
								var current = c;
								var column = reportColumns.FirstOrDefault(x => x.Ordinal == current);
								if (column == null)
									continue;

								var type = reader.GetFieldType(c);
								if (column.DataType is object && type != typeof(object))
								{
									column.DataType = type;
								}

								row.Add(new Cell(type) {
									ColumnId = column.Ordinal,
									Value = reader.IsDBNull(current) ? null : reader.GetValue(current)
								});
							}
							reportRows.Add(new Row {RowId = rowid, Cells = row });
							rowid++;
						}
					}
				}
				connection.Close();
			}

			return new PaymentMatrix
			{
				Columns = reportColumns.ToList(),
				Rows = reportRows
			};
		}

		public IEnumerable<EmployeeTotalRow> PaymentByEmployeeReport(DateTime start, DateTime end)
		{
			var reportRows = new List<EmployeeTotalRow>();
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				var reportStatement = new StringBuilder("SELECT ");
				reportStatement.AppendLine("p.EmployeeId");
				reportStatement.AppendLine(",IFNULL(e.Surname,'') || ' ' || IFNULL(e.Name,'') || ' ' || IFNULL(e.Patronimic,'') AS 'FullName'");
				reportStatement.AppendLine(",SUM(p.Value) AS 'Total'");
				reportStatement.AppendLine("FROM Payment p");
				reportStatement.AppendLine("INNER JOIN Employee e ON p.EmployeeId = e.Id");
				reportStatement.AppendLine("WHERE p.DatePayment >= @DateStart");
				reportStatement.AppendLine("AND p.DatePayment <= @DateEnd");
				reportStatement.AppendLine("AND p.Value IS NOT NULL");
				reportStatement.AppendLine("GROUP BY p.EmployeeId");
				reportStatement.AppendLine("ORDER BY FullName");

				using (var command = new SQLiteCommand(reportStatement.ToString(), connection))
				{
					command.Parameters.Add("@DateStart", DbType.DateTime).Value = start;
					command.Parameters.Add("@DateEnd", DbType.DateTime).Value = end;

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var row = new EmployeeTotalRow
							{
								EmployeeId = reader.GetInt64(0),
								FullName = reader.GetString(1),
								Total = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2)
							};
							
							reportRows.Add(row);
						}
					}
				}
				connection.Close();
			}

			return reportRows;
		}
		#endregion
	}
}