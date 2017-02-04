using System.Collections.ObjectModel;
using System.Windows.Data;
using FrameworkExtend;
using PaymentsTU.Model;

namespace PaymentsTU.Dialogs.DialogView
{
	public class EditEmployeeDialogViewModel : EditDialogViewModelBase<Employee>
	{
		public ListCollectionView DepartmentsDataView { get; set; }

		public EditEmployeeDialogViewModel(string title, Employee record, Func<Employee, bool> applyDataFunc) : base(title, record, applyDataFunc)
		{
			var departments = new ObservableCollection<Department>(Dal.Instance.Departments());
			DepartmentsDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(departments);
			DepartmentsDataView.CustomSort = new DepartmentComparer();
		}
	}
}