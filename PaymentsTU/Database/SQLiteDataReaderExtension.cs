using System.Data.SQLite;

namespace PaymentsTU.Database
{
	internal static class DataReaderExtensions
	{
		public static string SafeGetString(SQLiteDataReader reader, int colIndex)
		{
			return !reader.IsDBNull(colIndex) ? reader.GetString(colIndex) : null;
		}

		public static long? SafeGetLong(SQLiteDataReader reader, int colIndex)
		{
			if (!reader.IsDBNull(colIndex))
				return reader.GetInt64(colIndex);
			return null;
		}
	}
}