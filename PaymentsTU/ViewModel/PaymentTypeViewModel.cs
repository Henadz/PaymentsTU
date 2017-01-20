using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Dialogs.DialogView;
using PaymentsTU.Model;

namespace PaymentsTU.ViewModel
{
	public sealed class PaymentTypeViewModel : ViewModelBase, IDirectoryPage<PaymentType>
	{
		public string Title => "Виды платежей";

		private ObservableCollection<PaymentType> _items;
		private ICollectionView _itemsView = null;

		public DataNavigationBarViewModel<PaymentType> NavigationBar { get; private set; }
		public ObservableCollection<PaymentType> Items => _items;

		public PaymentTypeViewModel()
		{
			_items = new ObservableCollection<PaymentType>(Dal.Instance.PaymentTypes());

			_itemsView = CollectionViewSource.GetDefaultView(_items);// as ListCollectionView;
																	 //_employeesView.CustomSort = new EmployeeComparer();

			_itemsView.MoveCurrentToPosition(_items.Count > 0 ? 0 : -1);

			NavigationBar = new DataNavigationBarViewModel<PaymentType>((CollectionView)_itemsView, OnAddPaymentType, OnDeletePaymentType, OnEditPaymentType, () => { _itemsView.Refresh(); });
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
				_itemsView.Refresh();
				_itemsView.MoveCurrentTo(item);
			}
		}

		private void OnDeletePaymentType(PaymentType item)
		{
			var vm = new ConfirmDialogViewModel($"Вы действительно хотите удалить вид платежа {item.Name}?");
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Yes)
				if (Dal.Instance.DeletePaymentType(item))
					Items.Remove(item);
		}

		private void OnEditPaymentType(PaymentType item)
		{
			var vm = new EditPaymentTypeViewModel("Редактирование вида платежа", item);
			var result = DialogService.OpenDialog(vm);
			_itemsView.Refresh();
		}
	}
}
