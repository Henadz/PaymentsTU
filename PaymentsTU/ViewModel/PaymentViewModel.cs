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

		private ObservableCollection<Payment> _items;
		private CollectionViewSource _itemsView = null;

		public DataNavigationBarViewModel<Payment> NavigationBar { get; private set; }

		public ObservableCollection<Payment> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				_itemsView = new CollectionViewSource();
				_itemsView.Source = _items;
				OnPropertyChanged(nameof(ItemsDataView));
			}
		}

		public ListCollectionView ItemsDataView => (ListCollectionView)_itemsView.View;

		public PaymentViewModel()
		{
			Items = new ObservableCollection<Payment>(Dal.Instance.Payments().OrderByDescending(x => x.DatePayment).ThenBy(x => x.FullName));
			ItemsDataView.Culture = CultureInfo.CurrentCulture;
			ItemsDataView.MoveCurrentToPosition(Items.Count > 0 ? 0 : -1);
			//TODO: refactoring viewmodel for navigation bar
			NavigationBar = new DataNavigationBarViewModel<Payment>(ItemsDataView, OnAdd, OnDelete, OnEdit, () =>
			{
				Items.Clear();
				foreach (var payment in Dal.Instance.Payments())
				{
					Items.Add(payment);
				}
				ItemsDataView.Refresh();
				ItemsDataView.MoveCurrentToPosition(Items.Count > 0 ? 0 : -1);
			});
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
					_items.Add(p);
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
			var vm = new ConfirmDialogViewModel($"Вы действительно хотите удалить выплату?");
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Yes)
				if (Dal.Instance.DeletePayment(item))
					Items.Remove(item);
		}

		private void OnEdit(Payment item)
		{
			var editItem = (Payment)item.Clone();
			var vm = new DialogPaymentViewModel("Редактирование платежа", editItem, p =>
			{
				if (Dal.Instance.SavePayment(p))
				{
					var index = Items.IndexOf(item);
					Items[index] = editItem;
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
