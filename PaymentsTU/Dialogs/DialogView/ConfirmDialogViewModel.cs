using System.Windows;
using System.Windows.Input;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.ViewModel;

namespace PaymentsTU.Dialogs.DialogView
{
	public class ConfirmDialogViewModel : DialogViewModelBase
	{
		public ICommand YesCommand { get; set; } = null;

		public ICommand NoCommand { get; set; } = null;

		public string Message { get; set; }

		public ConfirmDialogViewModel(string message)
		{
			Title = "Подтверждение";
			Message = message;

			YesCommand = new RelayCommand<Window>(OnYesClicked);
			NoCommand = new RelayCommand<Window>(OnNoClicked);
		}

		private void OnNoClicked(Window parameter)
		{
			CloseDialogWithResult(parameter, DialogResult.No);
		}

		private void OnYesClicked(Window parameter)
		{
			CloseDialogWithResult(parameter, DialogResult.Yes);
		}
	}
}