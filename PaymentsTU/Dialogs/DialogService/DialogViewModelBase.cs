using PaymentsTU.ViewModel;

namespace PaymentsTU.Dialogs.DialogService
{
	public abstract class DialogViewModelBase : ViewModelBase
	{
		public string Title { get; set; }
		public DialogResult UserDialogResult
		{
			get;
			protected set;
		}
	}
}