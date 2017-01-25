using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Dialogs.DialogView;
using PaymentsTU.Model;

namespace PaymentsTU.ViewModel
{
	public sealed class PaymentTypeViewModel : ViewModelBase, IListPageViewModel<PaymentType>
	{
		public string Title => "Виды платежей";

		private ObservableCollection<PaymentType> _items;

		public DataNavigationBarViewModel<PaymentType> NavigationBar { get; private set; }
		public ListCollectionView ItemsDataView { get; }

		public PaymentTypeViewModel()
		{
			_items = new ObservableCollection<PaymentType>(Dal.Instance.PaymentTypes());
			ItemsDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(_items);
			ItemsDataView.CustomSort = new PaymentTypeComparer();

			ItemsDataView.MoveCurrentToPosition(_items.Count > 0 ? 0 : -1);

			NavigationBar = new DataNavigationBarViewModel<PaymentType>(ItemsDataView, OnAddPaymentType, OnDeletePaymentType, OnEditPaymentType, () => {
				_items.Clear();
				foreach (var payment in Dal.Instance.PaymentTypes())
				{
					_items.Add(payment);
				}
				ItemsDataView.Refresh();
				ItemsDataView.MoveCurrentToPosition(_items.Count > 0 ? 0 : -1);
			});
		}

		private void OnAddPaymentType(PaymentType item)
		{
			if (item == null)
				item = new PaymentType();
			var vm = new EditPaymentTypeViewModel("Новый вид платежа", item);
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Apply)
			{
				_items.Add(item);
				ItemsDataView.Refresh();
				ItemsDataView.MoveCurrentTo(item);
			}
		}

		private void OnDeletePaymentType(PaymentType item)
		{
			var vm = new ConfirmDialogViewModel($"Вы действительно хотите удалить вид платежа {item.Name}?");
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Yes)
				if (Dal.Instance.DeletePaymentType(item))
					ItemsDataView.Remove(item);
		}

		private void OnEditPaymentType(PaymentType item)
		{
			var editItem = (PaymentType)item.Clone();
			var vm = new EditPaymentTypeViewModel("Редактирование вида платежа", item);
			if (DialogService.OpenDialog(vm) == DialogResult.Apply)
			{
				var index = _items.IndexOf(item);
				_items[index] = editItem;
			}
			ItemsDataView.Refresh();
		}
	}
}
