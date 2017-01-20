using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
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

		public EditDepartmentViewModel(string title, Department record) : base(title, record)
		{
		}

		protected override void OnApplyClicked(Window parameter)
		{
			if (Dal.Instance.SaveDepartment(Record))
			{
				CloseDialogWithResult(parameter, DialogResult.Apply);
			}
		}
	}
}
