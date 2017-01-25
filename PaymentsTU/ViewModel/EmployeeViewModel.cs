using PaymentsTU.Model;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Dialogs.DialogView;

namespace PaymentsTU.ViewModel
{
	public sealed class EmployeeViewModel : ViewModelBase, IListPageViewModel<Employee>
	{
		private ObservableCollection<Employee> _employees;
		public DataNavigationBarViewModel<Employee> NavigationBar { get; private set; }
		public ListCollectionView ItemsDataView { get; set; }

		public string Title => "Сотрудники";

		public EmployeeViewModel()
		{
			_employees = new ObservableCollection<Employee>(Dal.Instance.Employees());
			ItemsDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(_employees);
			ItemsDataView.CustomSort = new EmployeeComparer();

			ItemsDataView.MoveCurrentToPosition(_employees.Count > 0 ? 0 : - 1);

			NavigationBar = new DataNavigationBarViewModel<Employee>(ItemsDataView, OnAddEmployee, OnDeleteEmployee, OnEditEmployee, () => {
				_employees.Clear();
				foreach (var payment in Dal.Instance.Employees())
				{
					_employees.Add(payment);
				}
				ItemsDataView.Refresh();
				ItemsDataView.MoveCurrentToPosition(_employees.Count > 0 ? 0 : -1);
			});
		}

		private void OnAddEmployee(Employee employee)
		{
			if (employee == null)
				employee = new Employee();
			var vm = new EditEmployeeDialogViewModel("Новый сотрудник", employee);
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Apply)
			{
				_employees.Add(employee);
				ItemsDataView.Refresh();
				ItemsDataView.MoveCurrentTo(employee);
			}
		}

		private void OnEditEmployee(Employee employee)
		{
			var editItem = (Employee)employee.Clone();
			var vm = new EditEmployeeDialogViewModel("Редактирование сотрудника", editItem);
			if (DialogService.OpenDialog(vm) == DialogResult.Apply)
			{
				var index = _employees.IndexOf(employee);
				_employees[index] = editItem;
			}
			ItemsDataView.Refresh();
		}

		private void OnDeleteEmployee(Employee employee)
		{
			var vm = new ConfirmDialogViewModel($"Вы действительно хотите удалить сотрудника {employee.FullName}?");
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Yes)
				if (Dal.Instance.DeleteEmployee(employee))
					_employees.Remove(employee);
		}
	}
}
