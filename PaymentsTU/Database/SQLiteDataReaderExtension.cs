using System.Data.SQLite;

namespace PaymentsTU.Database
{
	internal static class DataReaderExtensions
	{
		public static string SafeGetString(SQLiteDataReader reader, int colIndex)
		{
			return !reader.IsDBNull(colIndex) ? reader.GetString(colIndex) : null;
		}

		public static int? SafeGetInt(SQLiteDataReader reader, int colIndex)
		{
			if (!reader.IsDBNull(colIndex))
				return reader.GetInt32(colIndex);
			return null;
		}
	}
}