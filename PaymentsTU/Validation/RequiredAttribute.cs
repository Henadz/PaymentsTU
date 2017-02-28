using System;

namespace PaymentsTU.Validation
{
	public sealed class RequiredAttribute : ValidationAttribute
	{
		public Type Type { get; set; }
		public object InvalidValue { get; set; }
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null)
				return new ValidationResult(ErrorMessage);

			if (InvalidValue != null)
			{
				if (Type == null) Type = typeof (object);
				var tv = Convert.ChangeType(value, value.GetType());
				var tiv = Convert.ChangeType(InvalidValue, Type);
				if (object.Equals(tv, tiv))
					return new ValidationResult(ErrorMessage);
			}

			var s = value as string;
			if (s != null && string.IsNullOrEmpty(s.Trim()))
				return new ValidationResult(ErrorMessage);

			return ValidationResult.Success;
		}
	}
}