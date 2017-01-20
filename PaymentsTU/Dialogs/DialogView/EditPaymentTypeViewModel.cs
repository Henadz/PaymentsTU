using System.Windows;
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

		public EditPaymentTypeViewModel(string title, PaymentType record) : base(title, record)
		{
		}

		protected override void OnApplyClicked(Window parameter)
		{
			if (Dal.Instance.SavePaymentType(Record))
			{
				CloseDialogWithResult(parameter, DialogResult.Apply);
			}
		}
	}
}