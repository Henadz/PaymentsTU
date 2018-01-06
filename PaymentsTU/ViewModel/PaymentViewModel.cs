using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using FrameworkExtend;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Dialogs.DialogView;
using PaymentsTU.Model;

namespace PaymentsTU.ViewModel
{
	public sealed class PaymentViewModel : ViewModelBase, IListPageViewModel<Payment>
	{
		public string Title => "Выплаты";

		private readonly ObservableCollection<Payment> _items;

		public DataNavigationBarViewModel<Payment> NavigationBar { get; private set; }

		private DateTime _from;
		public DateTime From
		{
			get => _from;
			set
			{
				_from = value;
				OnPropertyChanged(nameof(From));
			}
		}

		private DateTime _to;
		public DateTime To
		{
			get => _to;
			set
			{
				_to = value;
				OnPropertyChanged(nameof(To));
			}
		}

		public ListCollectionView ItemsDataView { get; }

		public PaymentViewModel()
		{
			var year = DateTime.Today.Year;
			var month = DateTime.Today.Month;
			_from = new DateTime(year, month, 1);
			_to = new DateTime(year, month, DateTime.DaysInMonth(year, month));

			_items = new ObservableCollection<Payment>(Dal.Instance.Payments(_from, _to).OrderByDescending(x => x.DatePayment).ThenBy(x => x.FullName));

			ItemsDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(_items);
			ItemsDataView.Culture = CultureInfo.CurrentCulture;
			ItemsDataView.MoveCurrentToPosition(_items.Count > 0 ? 0 : -1);

			//TODO: refactoring viewmodel for navigation bar
			NavigationBar = new DataNavigationBarViewModel<Payment>(ItemsDataView, OnAdd, OnDelete, OnEdit, () =>
			{
				_items.Clear();
				foreach (var payment in Dal.Instance.Payments(_from, _to).OrderByDescending(x => x.DatePayment)
					.ThenBy(x => x.FullName))
				{
					_items.Add(payment);
				}

				ItemsDataView.Refresh();
				ItemsDataView.MoveCurrentToPosition(_items.Count > 0 ? 0 : -1);
			})
			{
				RefreshCanExecute = () => _from <= _to
			};

		}

		private void OnAdd(Payment item)
		{
			if (item == null)
				item = new Payment
				{
					Value = 0M,
					DatePayment = DateTime.Today,
					CurrencyId = 933
				};
			var vm = new DialogPaymentViewModel("Новый платеж", item, p =>
			{
				if (Dal.Instance.SavePayment(p))
				{
					_items.Insert(0, p);
					ItemsDataView.Refresh();
					ItemsDataView.MoveCurrentTo(p);
					return true;
				}
				return false;
			});
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Apply)
			{
			}
		}

		private void OnDelete(Payment item)
		{
			var vm = new ConfirmDialogViewModel("Вы действительно хотите удалить выплату?");
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Yes)
				if (Dal.Instance.DeletePayment(item))
					_items.Remove(item);
		}

		private void OnEdit(Payment item)
		{
			var editItem = (Payment)item.Clone();
			var vm = new DialogPaymentViewModel("Редактирование платежа", editItem, p =>
			{
				if (Dal.Instance.SavePayment(p))
				{
					var index = _items.IndexOf(item);
					_items[index] = editItem;
					ItemsDataView.Refresh();
					return true;
				}
				return false;
			});
			if (DialogService.OpenDialog(vm) == DialogResult.Apply)
			{
			}
			ItemsDataView.Refresh();
		}
	}
}
