using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Data;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Dialogs.DialogView;
using PaymentsTU.Model;

namespace PaymentsTU.ViewModel
{
	public sealed class PaymentViewModel : ViewModelBase, IDirectoryPage<Payment>
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
			Items = new ObservableCollection<Payment>(Dal.Instance.Payments());

			//_itemsView.MoveCurrentToPosition(_items.Count > 0 ? 0 : -1);

			NavigationBar = new DataNavigationBarViewModel<Payment>(ItemsDataView, OnAdd, OnDelete, OnEdit, () =>
			{
				Items = new ObservableCollection<Payment>(Dal.Instance.Payments());
			});
		}

		private void OnAdd(Payment item)
		{
			if (item == null)
				item = new Payment
				{
					Value = 100.54M,
					DatePayment = DateTime.Today,
					CurrencyId = 933
				};
			var vm = new DialogPaymentViewModel("Новый платеж", item);
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Apply)
			{
				_items.Add(item);
				ItemsDataView.Refresh();
				ItemsDataView.MoveCurrentTo(item);
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
			var vm = new DialogPaymentViewModel("Редактирование платежа", item);
			var result = DialogService.OpenDialog(vm);
			ItemsDataView.Refresh();
		}
	}
}
