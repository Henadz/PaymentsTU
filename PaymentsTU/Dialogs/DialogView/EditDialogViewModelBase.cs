using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using FrameworkExtend;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Validation;
using PaymentsTU.ViewModel;

namespace PaymentsTU.Dialogs.DialogView
{
	public abstract class EditDialogViewModelBase<T> : DialogViewModelBase, IDataValidation
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

		private bool _isModelValid;
		public bool IsModelValid
		{
			get { return _isModelValid; }
			set { _isModelValid = value; OnPropertyChanged(nameof(IsModelValid)); }
		}

		public Func<T, bool> ApplyDataFunc { get; private set; }

		public bool AddNextRecord { get; set; }

		protected EditDialogViewModelBase(string title, T record, Func<T, bool> applyDataFunc)
		{
			Title = title;
			Record = record;
			ApplyDataFunc = applyDataFunc;
			_isModelValid = applyDataFunc != null;
			this.ApplyCommand = new RelayCommand<Window>(OnApplyClicked, w => IsModelValid);
			this.CancelCommand = new RelayCommand<Window>(OnCancelClicked);
		}

		protected virtual void OnApplyClicked(Window parameter)
		{
			if (ApplyDataFunc(Record))
			{
				if (AddNextRecord)
				{
					Record = new T();
					OnPropertyChanged(nameof(Record));
				}
				else
					CloseDialogWithResult(parameter, DialogResult.Apply);
			}
		}

		protected virtual void OnCancelClicked(Window parameter)
		{
			CloseDialogWithResult(parameter, DialogResult.Cancel);
		}

		public string this[string columnName] => AttributeValidator.Validate(this, columnName);

		public string Error
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}