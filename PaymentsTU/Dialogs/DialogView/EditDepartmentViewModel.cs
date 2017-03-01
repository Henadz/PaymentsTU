using System.Windows;
using FrameworkExtend;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Model;
using PaymentsTU.Validation;

namespace PaymentsTU.Dialogs.DialogView
{
	public sealed class EditDepartmentViewModel : EditDialogViewModelBase<Department>
	{
		public string RecordTitle => "Подразделение";

		[Required(ErrorMessage = "Поле является обязательным и не может быть пустым")]
		public string RecordData
		{
			get { return Record.Name; }
			set { Record.Name = value; OnPropertyChanged(nameof(RecordData));}
		}

		public EditDepartmentViewModel(string title, Department record, Func<Department, bool> applyDataFunc) : base(title, record, applyDataFunc)
		{
		}

		protected override void OnApplyClicked(Window parameter)
		{
			if (ApplyDataFunc(Record))
			{
				if (AddNextRecord)
				{
					Record = new Department();
					OnPropertyChanged(nameof(RecordData));
				}
				else
					CloseDialogWithResult(parameter, DialogResult.Apply);
			}
		}
	}
}
