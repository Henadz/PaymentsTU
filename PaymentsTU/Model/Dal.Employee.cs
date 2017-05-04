using FrameworkExtend;
using PaymentsTU.Database;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq.Expressions;
using System.Text;

namespace PaymentsTU.Model
{
	internal sealed partial class Dal
	{
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

		public IEnumerable<Employee> Employees(Expression<Func<Employee, bool>> @where)
		{
			var resultset = new List<Employee>();
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				var statement = new StringBuilder();
				statement.AppendLine("SELECT Employee.Id, Employee.Surname, Employee.Name, Employee.Patronimic, Employee.IsFired, Employee.Note, Employee.DepartmentId, Department.Name as Department ");
				statement.AppendLine("FROM Employee");
				statement.AppendLine("LEFT JOIN Department ON Employee.DepartmentId = Department.Id");
				if (@where != null)
				{
					var tr = new WhereTranslator();
					statement.AppendLine(tr.Translate(@where).ToString());
				}

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
	}
}
