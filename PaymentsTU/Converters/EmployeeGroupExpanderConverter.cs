using System;
using System.Globalization;
using System.Windows.Data;
using FrameworkExtend;
using PaymentsTU.Model;

namespace PaymentsTU.Converters
{
	internal sealed class EmployeeGroupExpanderConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var employee = values.FirstOrDefault(x => x is Employee) as Employee;
			var group = values.FirstOrDefault(x => x is string) as string ?? string.Empty;

			return employee?.Surname.Substring(0, 1).Equals(group.Substring(0,1), StringComparison.OrdinalIgnoreCase);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
