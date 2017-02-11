using System;
using System.ComponentModel;
using System.Windows;
using PaymentsTU.Validation;
using PaymentsTU.ViewModel;

namespace PaymentsTU.Dialogs.DialogService
{
	public abstract class DialogViewModelBase : ViewModelBase
	{
		public string Title { get; set; }
		public DialogResult UserDialogResult
		{
			get;
			protected set;
		}

		public void CloseDialogWithResult(Window dialog, DialogResult result)
		{
			this.UserDialogResult = result;
			if (dialog != null)
				dialog.DialogResult = true;
		}
	}
}