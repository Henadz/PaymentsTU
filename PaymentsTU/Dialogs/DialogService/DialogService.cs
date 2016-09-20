using System.Windows;

namespace PaymentsTU.Dialogs.DialogService
{
	public static class DialogService
	{
		public static DialogResult OpenDialog(DialogViewModelBase vm)
		{
			var dialog = new DialogWindow
			{
				Owner = Application.Current.MainWindow,
				DataContext = vm
			};
			dialog.ShowDialog();
			var dialogViewModelBase = dialog.DataContext as DialogViewModelBase;
			if (dialogViewModelBase != null)
			{
				var result = dialogViewModelBase.UserDialogResult;
				return result;
			}
			return DialogResult.Undefined;
		}
	}
}