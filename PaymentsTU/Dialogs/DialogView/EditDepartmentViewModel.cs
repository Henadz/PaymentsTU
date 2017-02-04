using System.Windows;
using FrameworkExtend;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Model;

namespace PaymentsTU.Dialogs.DialogView
{
	public sealed class EditDepartmentViewModel : EditDialogViewModelBase<Department>
	{
		public string RecordTitle => "Подразделение";

		public string RecordData
		{
			get { return Record.Name; }
			set { Record.Name = value; }
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
