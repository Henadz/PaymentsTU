using System;
using System.ComponentModel;
using FrameworkExtend;

namespace PaymentsTU.Validation
{
	internal static class AttributeValidator
	{
		public static string Validate(IDataErrorInfo source, string columnName)
		{
			var type = source.GetType();
			var property = type.GetProperty(columnName);
			var validators = (ValidationAttribute[])property.GetCustomAttributes(typeof(ValidationAttribute), true);
			if (validators.Any())
			{
				var value = property.GetValue(source, null);
				var errors = validators.Where(v => !v.IsValid(value)).Select(v => v.ErrorMessage ?? "").ToArray();
				return string.Join(Environment.NewLine, errors);
			}
			return string.Empty;
		}
	}
}