using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Documents;
using FrameworkExtend;
using PaymentsTU.Model;
using PaymentsTU.Validation;

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

		
	}
}