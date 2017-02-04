﻿using System.Windows;
using FrameworkExtend;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Model;

namespace PaymentsTU.Dialogs.DialogView
{
	public class EditPaymentTypeViewModel: EditDialogViewModelBase<PaymentType>
	{
		public string RecordTitle => "Вид платежа";

		public string RecordData
		{
			get { return Record.Name; }
			set { Record.Name = value; }
		}

		public EditPaymentTypeViewModel(string title, PaymentType record, Func<PaymentType, bool> applyDataFunc) : base(title, record, applyDataFunc)
		{
		}

		protected override void OnApplyClicked(Window parameter)
		{
			if (ApplyDataFunc(Record))
			{
				if (AddNextRecord)
				{
					Record = new PaymentType();
					OnPropertyChanged(nameof(RecordData));
				}
				else
					CloseDialogWithResult(parameter, DialogResult.Apply);
			}
		}
	}
}