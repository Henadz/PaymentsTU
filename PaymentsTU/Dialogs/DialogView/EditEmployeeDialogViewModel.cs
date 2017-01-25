using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Model;
using PaymentsTU.ViewModel;

namespace PaymentsTU.Dialogs.DialogView
{
	public class EditEmployeeDialogViewModel : EditDialogViewModelBase<Employee>
	{
		public ListCollectionView DepartmentsDataView { get; set; }

		public EditEmployeeDialogViewModel(string title, Employee record) : base(title, record)
		{
			var departments = new ObservableCollection<Department>(Dal.Instance.Departments());
			DepartmentsDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(departments);
			DepartmentsDataView.CustomSort = new DepartmentComparer();
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