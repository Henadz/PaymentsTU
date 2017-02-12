using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using FrameworkExtend;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Model;
using PaymentsTU.Validation;

namespace PaymentsTU.Dialogs.DialogView
{
	public sealed class DialogPaymentViewModel: EditDialogViewModelBase<Payment>
	{
		public ListCollectionView EmployeesDataView { get; set; }

		public ListCollectionView DepartmentsDataView { get; set; }

		public ListCollectionView PaymentTypesDataView { get; set; }

		public ListCollectionView CurrenciesDataView { get; set; }

		private readonly ObservableCollection<Employee> _employees;

		public Employee CurrentEmployee
		{
			get { return _employees.FirstOrDefault(x => x.Id == Record.Id); }
			set
			{
				if (value != null)
				{
					Record.DepartmentId = value.DepartmentId ?? 0;
					OnPropertyChanged(nameof(CurrentDepartment));
				}
				OnPropertyChanged(nameof(CurrentEmployee));
			}
		}

		private readonly ObservableCollection<Department> _departments;

		public Department CurrentDepartment
		{
			get { return _departments.FirstOrDefault(x => x.Id == Record.DepartmentId); }
			set
			{
				OnPropertyChanged(nameof(CurrentDepartment));
			}
		}
		[Required(ErrorMessage = "Поле является обязательным и не может быть пустым")]
		public long? EmployeeId
		{
			get { return Record.EmployeeId == 0 ? (long?)null : Record.EmployeeId; }
			set { Record.EmployeeId = value ?? 0; OnPropertyChanged(nameof(EmployeeId));}
		}
		[Required(ErrorMessage = "Поле является обязательным и не может быть пустым")]
		public long PaymentTypeId { get { return Record.PaymentTypeId; } set { Record.PaymentTypeId = value; } }
		[Required(ErrorMessage = "Поле является обязательным и не может быть пустым")]
		public long DepartmentId { get { return Record.DepartmentId; } set { Record.DepartmentId = value; } }
		[Required]
		public DateTime DatePayment { get { return Record.DatePayment; } set { Record.DatePayment = value; } }
		[Required]
		public int CurrencyId { get { return Record.CurrencyId; } set { Record.CurrencyId = value; } }
		[Required]
		public decimal Value { get { return Record.Value; } set { Record.Value = value; } }

		public DialogPaymentViewModel(string title, Payment record, Func<Payment, bool> applyDataFunc) : base(title, record, applyDataFunc)
		{
			_employees = new ObservableCollection<Employee>(Dal.Instance.Employees(true));
			EmployeesDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(_employees);
			EmployeesDataView.CustomSort = new EmployeeComparer();
			EmployeesDataView.MoveCurrentToFirst();

			var currencies = new ObservableCollection<Currency>(Dal.Instance.Currencies());
			CurrenciesDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(currencies);

			_departments = new ObservableCollection<Department>(Dal.Instance.Departments());
			DepartmentsDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(_departments);
			DepartmentsDataView.CustomSort = new DepartmentComparer();

			var types = new ObservableCollection<PaymentType>(Dal.Instance.PaymentTypes());
			PaymentTypesDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(types);
			PaymentTypesDataView.CustomSort = new PaymentTypeComparer();
		}

		protected override void OnApplyClicked(Window parameter)
		{
			if (ApplyDataFunc(Record))
			{
				if (AddNextRecord)
				{
					Record = new Payment
					{
						Value = 0M,
						DatePayment = DateTime.Today,
						CurrencyId = 933
					};
				}
				else
					CloseDialogWithResult(parameter, DialogResult.Apply);
			}
		}
	}
}
