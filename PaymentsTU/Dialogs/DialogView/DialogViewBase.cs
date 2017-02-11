using System;
using System.Windows.Controls;
using PaymentsTU.Validation;

namespace PaymentsTU.Dialogs.DialogView
{
	public class DialogViewBase : UserControl
	{
		private int _errorCount;
		public DialogViewBase()
		{
			System.Windows.Controls.Validation.AddErrorHandler(this, OnChildControlError);
		}

		private void OnChildControlError(object sender, ValidationErrorEventArgs e)
		{
			switch (e.Action)
			{
				case ValidationErrorEventAction.Added:
					_errorCount += 1;
					break;
				case ValidationErrorEventAction.Removed:
					_errorCount -= 1;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			((IDataValidation)DataContext).IsModelValid = _errorCount == 0;
		}
	}
}