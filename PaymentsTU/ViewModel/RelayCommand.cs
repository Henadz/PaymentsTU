using System;
using System.Windows.Input;

namespace PaymentsTU.ViewModel
{
	public delegate void Action();
	public class RelayCommand<T> : ICommand
	{
		private readonly Action<T> _action;
		private readonly Predicate<T> _canExecute;

		public RelayCommand(Action<T> action)
		: this(action, null)
		{
		}

		public RelayCommand(Action<T> action, Predicate<T> canExecute)
		{
			_action = action ?? throw new ArgumentNullException(nameof(action));
			_canExecute = canExecute;
		}

		public void Execute(object parameter)
		{
			_action((T)parameter);
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke((T)parameter) ?? true;
		}

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}
	}

	public class RelayCommand : ICommand
	{
		private readonly Action _action;
		private readonly Predicate<object> _canExecute;

		public RelayCommand(Action action)
		: this(action, null)
		{
		}

		public RelayCommand(Action action, Predicate<object> canExecute)
		{
			_action = action ?? throw new ArgumentNullException(nameof(action));
			_canExecute = canExecute;
		}

		public void Execute(object parameter)
		{
			_action();
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke(parameter) ?? true;
		}

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}
	}
}