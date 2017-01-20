using System;
using System.Data.SQLite;
using System.IO;

namespace PaymentsTU.Database
{
	internal class DatabaseCreationOperation
	{
		private static string GetDatabasePath(string connectionString)
		{
			try
			{
				var builder = new System.Data.Common.DbConnectionStringBuilder {ConnectionString = connectionString};
				return builder["Data Source"] as string;
			}
			catch (Exception)
			{
				return null;
			}

		}

		public static bool IsDatabaseExist(string connectionString)
		{
			var dbpath = GetDatabasePath(connectionString);
			if (string.IsNullOrEmpty(dbpath))
				throw new InvalidOperationException("Invalid connection string: " + connectionString);

			return File.Exists(dbpath);
		}

		public static void CreateDatabase(string connectionString)
		{
			if (IsDatabaseExist(connectionString))
				return;

			var dbpath = GetDatabasePath(connectionString);
			var directory = Path.GetDirectoryName(dbpath);
			if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			SQLiteConnection.CreateFile(dbpath);
			CreateTables(connectionString);
			FillCurrencyTable(connectionString);
			FillPaymentTypeTable(connectionString);
		}

		private static void CreateTables(string connectionString)
		{
			using (var connection = new SQLiteConnection(connectionString))
			{
				connection.Open();
				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText =
						"CREATE TABLE PaymentType (Id INTEGER PRIMARY KEY ASC AUTOINCREMENT, Name NVARCHAR(50) NOT NULL)";
					command.ExecuteNonQuery();

					command.CommandText = "CREATE TABLE FinancialPeriod (" +
										  "Id INTEGER PRIMARY KEY ASC AUTOINCREMENT," +
										  "StartDate DATE NOT NULL UNIQUE," +
										  "EndDate DATE NOT NULL," +
										  "PaymentLimit DECIMAL(10,2) DEFAULT 0," +
										  "IsClosed INTEGER NOT NULL)";
					command.ExecuteNonQuery();

					command.CommandText =
						"CREATE TABLE Department (Id INTEGER PRIMARY KEY ASC AUTOINCREMENT, Name NVARCHAR(255) NOT NULL)";
					command.ExecuteNonQuery();

					command.CommandText =
						"CREATE TABLE Currency (DigitalCode INTEGER PRIMARY KEY, Code VARCHAR(10) NOT NULL, Name NVARCHAR(255))";
					command.ExecuteNonQuery();

					command.CommandText = "CREATE TABLE Employee (" +
										  "Id INTEGER PRIMARY KEY ASC AUTOINCREMENT," +
										  "Surname NVARCHAR(255) NOT NULL," +
										  "Name NVARCHAR(255) NOT NULL," +
										  "Patronimic NVARCHAR(255)," +
										  "IsFired INTEGER NOT NULL CHECK (IsFired IN (0,1))," +
										  "Note NVARCHAR(1024))";
					command.ExecuteNonQuery();

					command.CommandText = "CREATE TABLE Payment (" +
										  "Id INTEGER PRIMARY KEY ASC AUTOINCREMENT," +
										  "EmployeeId INTEGER NOT NULL," +
										  "PaymentTypeId INTEGER NOT NULL," +
										  "DepartmentId INTEGER NOT NULL," +
										  "DatePayment DATE NOT NULL," +
										  "CurrencyCode INTEGER NOT NULL," +
										  "Value DECIMAL(10,2) NOT NULL," +
										  "FOREIGN KEY (EmployeeId) REFERENCES Employee (Id) ON DELETE CASCADE ON UPDATE NO ACTION," +
										  "FOREIGN KEY (PaymentTypeId) REFERENCES PaymentType (Id) ON DELETE CASCADE ON UPDATE NO ACTION," +
										  "FOREIGN KEY (DepartmentId) REFERENCES Department (Id) ON DELETE CASCADE ON UPDATE NO ACTION," +
										  "FOREIGN KEY (CurrencyCode) REFERENCES Currency (DigitalCode) ON DELETE CASCADE ON UPDATE NO ACTION" +
										  ")";
					command.ExecuteNonQuery();
				}
			}
		}

		private static void FillCurrencyTable(string connectionString)
		{
			using (var connection = new SQLiteConnection(connectionString))
			{
				connection.Open();
				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText = "INSERT INTO Currency (DigitalCode, Code, Name)" +
										  "VALUES" +
										  "(933, \"BYN\", \"Белорусский рубль\")," +
										  "(974, \"BYR\", \"Белорусский рубль BYR\")";
					command.ExecuteNonQuery();
				}
			}
		}

		private static void FillPaymentTypeTable(string connectionString)
		{
			using (var connection = new SQLiteConnection(connectionString))
			{
				connection.Open();
				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText = "INSERT INTO PaymentType (Name)" +
										  "VALUES" +
										  "(\"Материальная помощь\")," +
										  "(\"Абонементы: бассейн\")," +
										  "(\"Абонементы: фитнес\")," +
										  "(\"Тренажерный зал\")," +
										  "(\"Билеты\")," +
										  "(\"Подарки\")," +
										  "(\"Продукты\")," +
										  "(\"Поездки\")";
					command.ExecuteNonQuery();
				}
			}
		}
	}
}