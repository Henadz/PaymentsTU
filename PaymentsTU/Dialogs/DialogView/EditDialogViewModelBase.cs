using System.Windows;
using System.Windows.Input;
using FrameworkExtend;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.ViewModel;

namespace PaymentsTU.Dialogs.DialogView
{
	public abstract class EditDialogViewModelBase<T> : DialogViewModelBase
		where T: new()
	{
		public ICommand ApplyCommand { get; set; } = null;

		public ICommand CancelCommand { get; set; } = null;

		private T _record;

		public T Record
		{
			get {return _record;}
			set { _record = value; OnPropertyChanged(nameof(Record)); }
		}

		public Func<T, bool> ApplyDataFunc { get; private set; }

		public bool AddNextRecord { get; set; }

		protected EditDialogViewModelBase(string title, T record, Func<T, bool> applyDataFunc)
		{
			Title = title;
			Record = record;
			ApplyDataFunc = applyDataFunc;
			this.ApplyCommand = new RelayCommand<Window>(OnApplyClicked);
			this.CancelCommand = new RelayCommand<Window>(OnCancelClicked);
		}

		protected virtual void OnApplyClicked(Window parameter)
		{
			if (ApplyDataFunc(Record))
			{
				if (AddNextRecord)
					Record = new T();
				else
					CloseDialogWithResult(parameter, DialogResult.Apply);
			}
		}

		protected virtual void OnCancelClicked(Window parameter)
		{
			CloseDialogWithResult(parameter, DialogResult.Cancel);
		}
	}
}