using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using FrameworkExtend;
using PaymentsTU.Model;

namespace PaymentsTU.Dialogs.DialogView
{
	public class EditEmployeeDialogViewModel : EditDialogViewModelBase<Employee>, IDataErrorInfo
	{
		public ListCollectionView DepartmentsDataView { get; set; }

		public string Surname { get { return Record.Surname; } set { Record.Surname = value; } }
		public string Name { get { return Record.Name; } set { Record.Name = value; } }
		public string Patronymic { get { return Record.Patronymic; } set { Record.Patronymic = value; } }
		public bool IsFired { get { return Record.IsFired; } set { Record.IsFired = value; } }
		public string Note { get { return Record.Note; } set { Record.Note = value; } }
		public long? DepartmentId { get { return Record.DepartmentId; } set { Record.DepartmentId = value; } }

		public EditEmployeeDialogViewModel(string title, Employee record, Func<Employee, bool> applyDataFunc) : base(title, record, applyDataFunc)
		{
			var departments = new ObservableCollection<Department>(Dal.Instance.Departments());
			DepartmentsDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(departments);
			DepartmentsDataView.CustomSort = new DepartmentComparer();
		}

		public string this[string columnName]
		{
			get
			{
				string message = null;
				switch (columnName)
				{
					case nameof(Surname):
						if (string.IsNullOrEmpty(this.Surname))
							message = "Должно быть заполнено";
						break;
					default:
						message = null;
						break;
				}
				return message;
			}
		}

		public string Error { get; }
	}
}