using System.Collections.ObjectModel;
using System.Windows.Data;
using FrameworkExtend;
using PaymentsTU.Model;
using PaymentsTU.Validation;
using System.Windows;
using PaymentsTU.Dialogs.DialogService;

namespace PaymentsTU.Dialogs.DialogView
{
	public class EditEmployeeDialogViewModel : EditDialogViewModelBase<Employee>
	{
		public ListCollectionView DepartmentsDataView { get; set; }

		[Required(ErrorMessage = "Поле является обязательным и не может быть пустым")]
		public string Surname { get { return Record.Surname; } set { Record.Surname = value; OnPropertyChanged(nameof(Surname));} }
		[Required(ErrorMessage = "Поле является обязательным и не может быть пустым")]
		public string Name { get { return Record.Name; } set { Record.Name = value; OnPropertyChanged(nameof(Name));} }
		public string Patronymic { get { return Record.Patronymic; } set { Record.Patronymic = value; OnPropertyChanged(nameof(Patronymic));} }
		public bool IsFired { get { return Record.IsFired; } set { Record.IsFired = value; OnPropertyChanged(nameof(IsFired));} }
		public string Note { get { return Record.Note; } set { Record.Note = value; OnPropertyChanged(nameof(Note));} }
		public long? DepartmentId { get { return Record.DepartmentId; } set { Record.DepartmentId = value; OnPropertyChanged(nameof(DepartmentId));} }

		public EditEmployeeDialogViewModel(string title, Employee record, Func<Employee, bool> applyDataFunc) : base(title, record, applyDataFunc)
		{
			var departments = new ObservableCollection<Department>(Dal.Instance.Departments());
			DepartmentsDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(departments);
			DepartmentsDataView.CustomSort = new DepartmentComparer();
		}

		protected override void OnApplyClicked(Window parameter)
		{
			if (CheckDuplicate(Record) == DialogResult.Yes && ApplyDataFunc(Record))
			{
				if (AddNextRecord)
				{
					Record = new Employee();
					OnPropertyChanged(nameof(Surname));
					OnPropertyChanged(nameof(Name));
					OnPropertyChanged(nameof(Patronymic));
					OnPropertyChanged(nameof(IsFired));
					OnPropertyChanged(nameof(Note));
					OnPropertyChanged(nameof(DepartmentId));
				}
				else
					CloseDialogWithResult(parameter, DialogResult.Apply);
			}
		}

		private DialogResult CheckDuplicate(Employee employee)
		{
			var s = employee.Surname?.Trim();
			var n = employee.Name?.Trim();
			var p = employee.Patronymic?.Trim();
			var r = Dal.Instance.Employees(x => x.Surname.Trim() == s && x.Name.Trim() == n && x.Patronymic.Trim() == p);
			if (r.Count() == 0 || (r.Any(x => x.Id == employee.Id)))
				return DialogResult.Yes;

			var vm = new ConfirmDialogViewModel($"База данных уже содержит сотрудника {employee.FullName}.\r\nВы уверены что хотите добавить сотрудника с такими же Ф.И.О.?");
			return DialogService.DialogService.OpenDialog(vm);
		}

	}
}