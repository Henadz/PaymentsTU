using System;
using System.Globalization;
using System.Windows.Data;

namespace PaymentsTU.Converters
{
	internal sealed class FirstLetterConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var s = value as string;
			return string.IsNullOrEmpty(s?.Trim())
				? string.Empty
				: s[0].ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}