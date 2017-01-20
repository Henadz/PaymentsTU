using System.Windows;
using System.Windows.Input;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Model;
using PaymentsTU.ViewModel;

namespace PaymentsTU.Dialogs.DialogView
{
	public class EditEmployeeDialogViewModel : EditDialogViewModelBase<Employee>
	{

		public EditEmployeeDialogViewModel(string title, Employee record) : base(title, record)
		{
		}

		protected override void OnApplyClicked(Window parameter)
		{
			if (Dal.Instance.SaveEmployee(Record))
			{
				CloseDialogWithResult(parameter, DialogResult.Apply);
			}
		}
	}
}