using System.Windows;
using System.Windows.Input;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.ViewModel;

namespace PaymentsTU.Dialogs.DialogView
{
	public abstract class EditDialogViewModelBase<T> : DialogViewModelBase
	{
		public ICommand ApplyCommand { get; set; } = null;

		public ICommand CancelCommand { get; set; } = null;

		public T Record { get; set; }

		protected EditDialogViewModelBase(string title, T record)
		{
			Title = title;
			Record = record;
			this.ApplyCommand = new RelayCommand<Window>(OnApplyClicked);
			this.CancelCommand = new RelayCommand<Window>(OnCancelClicked);
		}

		protected abstract void OnApplyClicked(Window parameter);

		protected virtual void OnCancelClicked(Window parameter)
		{
			CloseDialogWithResult(parameter, DialogResult.Cancel);
		}
	}
}