using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Model;

namespace PaymentsTU.Dialogs.DialogView
{
	public class DialogPaymentViewModel: EditDialogViewModelBase<Payment>
	{
		public DialogPaymentViewModel(string title, Payment record) : base(title, record)
		{
		}

		protected override void OnApplyClicked(Window parameter)
		{
			CloseDialogWithResult(parameter, DialogResult.Apply);
		}
	}
}
