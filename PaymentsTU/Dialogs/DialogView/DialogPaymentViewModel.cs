using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Data;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Model;

namespace PaymentsTU.Dialogs.DialogView
{
	public sealed class DialogPaymentViewModel: EditDialogViewModelBase<Payment>
	{
		public ListCollectionView EmployeesDataView { get; set; }

		public ListCollectionView DepartmentsDataView { get; set; }

		public ListCollectionView PaymentTypesDataView { get; set; }

		public ListCollectionView CurrenciesDataView { get; set; }

		public DialogPaymentViewModel(string title, Payment record) : base(title, record)
		{
			var employees = new ObservableCollection<Employee>(Dal.Instance.Employees(true));
			EmployeesDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(employees);
			EmployeesDataView.CustomSort = new EmployeeComparer();
			EmployeesDataView.MoveCurrentToFirst();

			var currencies = new ObservableCollection<Currency>(Dal.Instance.Currencies());
			CurrenciesDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(currencies);

			var departments = new ObservableCollection<Department>(Dal.Instance.Departments());
			DepartmentsDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(departments);
			DepartmentsDataView.CustomSort = new DepartmentComparer();

			var types = new ObservableCollection<PaymentType>(Dal.Instance.PaymentTypes());
			PaymentTypesDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(types);
			PaymentTypesDataView.CustomSort = new PaymentTypeComparer();
		}

		protected override void OnApplyClicked(Window parameter)
		{
			if (Dal.Instance.SavePayment(Record))
			{
				CloseDialogWithResult(parameter, DialogResult.Apply);
			}
		}
	}
}
