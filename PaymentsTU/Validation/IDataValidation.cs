using System.ComponentModel;

namespace PaymentsTU.Validation
{
	public interface IDataValidation : IDataErrorInfo
	{
		bool IsModelValid { get; set; }
	}
}