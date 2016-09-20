using System.Windows;
using System.Windows.Input;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Model;
using PaymentsTU.ViewModel;

namespace PaymentsTU.Dialogs.DialogView
{
	public class EditEmployeeDialogViewModel : DialogViewModelBase
	{
		public ICommand ApplyCommand { get; set; } = null;

		public ICommand CancelCommand { get; set; } = null;

		public Employee Employee { get; set; }

		public EditEmployeeDialogViewModel(string title, Employee employee)
		{
			Title = title;
			Employee = employee;
			this.ApplyCommand = new RelayCommand<Window>(OnApplyClicked);
			this.CancelCommand = new RelayCommand<Window>(OnCancelClicked);
		}

		private void OnApplyClicked(object parameter)
		{
			if (Dal.Instance.SaveEmployee(Employee))
			{
				this.CloseDialogWithResult(parameter as Window, DialogResult.Apply);
			}
		}

		private void OnCancelClicked(object parameter)
		{
			this.CloseDialogWithResult(parameter as Window, DialogResult.Cancel);
		}

		public void CloseDialogWithResult(Window dialog, DialogResult result)
		{
			this.UserDialogResult = result;
			if (dialog != null)
				dialog.DialogResult = true;
		}
	}
}