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
	public sealed class EmployeeViewModel : ViewModelBase, IDirectoryPage<Employee>
	{
		private ObservableCollection<Employee> _employees;
		private ICollectionView _employeesView = null;

		public DataNavigationBarViewModel<Employee> NavigationBar { get; private set; }

		public ObservableCollection<Employee> Items => _employees;

		//public Employee CurrentItem => _employeesView.CurrentItem as Employee;

		public string Title => "Сотрудники";

		//private readonly Action RefreshData;

		public EmployeeViewModel()
		{
			_employees = new ObservableCollection<Employee>(Dal.Instance.Employees());

			_employeesView = CollectionViewSource.GetDefaultView(_employees);// as ListCollectionView;
			//_employeesView.CustomSort = new EmployeeComparer();

			_employeesView.MoveCurrentToPosition(_employees.Count > 0 ? 0 : - 1);

			NavigationBar = new DataNavigationBarViewModel<Employee>((CollectionView)_employeesView, OnAddEmployee, OnDeleteEmployee, OnEditEmployee, () => { _employeesView.Refresh(); });
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
				_employeesView.Refresh();
				_employeesView.MoveCurrentTo(employee);
			}
		}

		private void OnEditEmployee(Employee employee)
		{
			var vm = new EditEmployeeDialogViewModel("Редактирование сотрудника", employee);
			var result = DialogService.OpenDialog(vm);
			_employeesView.Refresh();
		}

		private void OnDeleteEmployee(Employee employee)
		{
			var vm = new ConfirmDialogViewModel($"Вы действительно хотите удалить сотрудника {employee.FullName}?");
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Yes)
				if (Dal.Instance.DeleteEmployee(employee))
					Items.Remove(employee);
		}

		//_customerView.CustomSort = new CustomerSorter();
	}
}
