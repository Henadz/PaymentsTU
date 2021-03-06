﻿using PaymentsTU.Model;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using PaymentsTU.Converters;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Dialogs.DialogView;

namespace PaymentsTU.ViewModel
{
	public sealed class EmployeeViewModel : ViewModelBase, IListPageViewModel<Employee>
	{
		private ObservableCollection<Employee> _employees;
		public DataNavigationBarViewModel<Employee> NavigationBar { get; private set; }
		public ListCollectionView ItemsDataView { get; set; }

		public Employee CurrentItem => (Employee) ItemsDataView.CurrentItem;

		public string Title => "Сотрудники";

		public EmployeeViewModel()
		{
			_employees = new ObservableCollection<Employee>(Dal.Instance.Employees());
			ItemsDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(_employees);
			ItemsDataView.CustomSort = new EmployeeComparer();
			var groupDescription = new PropertyGroupDescription("Surname", new FirstLetterConverter());
			ItemsDataView?.GroupDescriptions?.Add(groupDescription);

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
			ItemsDataView.CurrentChanged += ItemsDataView_CurrentChanged;
		}

		private void ItemsDataView_CurrentChanged(object sender, EventArgs e)
		{
			OnPropertyChanged(nameof(CurrentItem));
		}

		private void OnAddEmployee(Employee employee)
		{
			if (employee == null)
				employee = new Employee();
			var vm = new EditEmployeeDialogViewModel("Новый сотрудник", employee, e =>
			{
				if (Dal.Instance.SaveEmployee(e))
				{
					_employees.Add(e);
					ItemsDataView.Refresh();
					ItemsDataView.MoveCurrentTo(e);
					return true;
				}
				return false;
			});
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Apply)
			{
				//_employees.Add(employee);
				ItemsDataView.Refresh();
				//ItemsDataView.MoveCurrentTo(employee);
			}
		}

		private void OnEditEmployee(Employee employee)
		{
			var editItem = (Employee)employee.Clone();
			var vm = new EditEmployeeDialogViewModel("Редактирование сотрудника", editItem, e =>
			{
				if (Dal.Instance.SaveEmployee(e))
				{
					var index = _employees.IndexOf(employee);
					_employees[index] = editItem;
					ItemsDataView.Refresh();
					return true;
				}
				return false;
			});
			if (DialogService.OpenDialog(vm) == DialogResult.Apply)
			{
				//var index = _employees.IndexOf(employee);
				//_employees[index] = editItem;
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
