namespace PaymentsTU.Validation
{
	public class RequiredAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null)
				return new ValidationResult(ErrorMessage);

			var s = value as string;
			if (s != null && string.IsNullOrEmpty(s.Trim()))
				return new ValidationResult(ErrorMessage);

			return ValidationResult.Success;
		}
	}
}