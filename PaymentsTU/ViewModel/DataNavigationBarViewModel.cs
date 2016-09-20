using System;
using System.Windows.Data;
using System.Windows.Input;

namespace PaymentsTU.ViewModel
{
	public class DataNavigationBarViewModel<T> : ViewModelBase
	{
		private readonly CollectionView _view;

		public T CurrentItem => (T)_view.CurrentItem;

		public DataNavigationBarViewModel(CollectionView view, Action<T> newCommand, Action<T> deleteCommand, Action<T> editCommand, Action refreshCommand)
		{
			_view = view;

			_view.CurrentChanged += (sender, args) => { OnPropertyChanged(nameof(CurrentItem)); };

			FirstCommand = new RelayCommand(() => _view.MoveCurrentToFirst(), _ => _view != null && !_view.IsEmpty && _view.CurrentPosition > 0);
			PreviousCommand = new RelayCommand(() => _view.MoveCurrentToPrevious(), _ => _view != null && !_view.IsEmpty && _view.CurrentPosition > 0);
			NextCommand = new RelayCommand(() => _view.MoveCurrentToNext(), _ => _view != null && !_view.IsEmpty && _view.CurrentPosition + 1 < _view.Count);
			LastCommand = new RelayCommand(() => _view.MoveCurrentToLast(), _ => _view != null && !_view.IsEmpty && _view.CurrentPosition + 1 < _view.Count);

			NewCommand = newCommand == null ? new RelayCommand<T>(o => { }, _ => false) : new RelayCommand<T>(newCommand, _ => _view != null);
			DeleteCommand = deleteCommand == null ? new RelayCommand<T>(o => { }, _ => false) : new RelayCommand<T>(deleteCommand, _ => _view != null && !_view.IsEmpty && !_view.IsCurrentAfterLast && !_view.IsCurrentBeforeFirst);
			EditCommand = editCommand == null ? new RelayCommand<T>(o => { }, _ => false) : new RelayCommand<T>(editCommand, _ => _view != null && !_view.IsEmpty && !_view.IsCurrentAfterLast && !_view.IsCurrentBeforeFirst);
			RefreshCommand = refreshCommand == null ? new RelayCommand(() => { }, _ => false) : new RelayCommand(refreshCommand, _ => _view != null);
		}

		public ICommand FirstCommand { get; private set; }
		public ICommand PreviousCommand { get; private set; }
		public ICommand NewCommand { get; private set; }
		public ICommand DeleteCommand { get; private set; }
		public ICommand EditCommand { get; private set; }
		public ICommand NextCommand { get; private set; }
		public ICommand LastCommand { get; private set; }
		public ICommand RefreshCommand { get; private set; }
	}
}