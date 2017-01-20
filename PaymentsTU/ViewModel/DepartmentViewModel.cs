using PaymentsTU.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using PaymentsTU.Dialogs.DialogService;
using PaymentsTU.Dialogs.DialogView;

namespace PaymentsTU.ViewModel
{
    public sealed class DepartmentViewModel : ViewModelBase, IDirectoryPage<Department>
    {
	    public string Title => "Подразделения";

		private ObservableCollection<Department> _items;
		private ICollectionView _itemsView = null;

		public DataNavigationBarViewModel<Department> NavigationBar { get; private set; }
		public ObservableCollection<Department> Items => _items;

		public DepartmentViewModel()
		{
			_items = new ObservableCollection<Department>(Dal.Instance.Departments());

			_itemsView = CollectionViewSource.GetDefaultView(_items);// as ListCollectionView;
																	 //_employeesView.CustomSort = new EmployeeComparer();

			_itemsView.MoveCurrentToPosition(_items.Count > 0 ? 0 : -1);

			NavigationBar = new DataNavigationBarViewModel<Department>((CollectionView)_itemsView, OnAdd, OnDelete, OnEdit, () => { _itemsView.Refresh(); });
		}

		private void OnAdd(Department item)
		{
			if (item == null)
				item = new Department();
			var vm = new EditDepartmentViewModel("Новое подразделение", item);
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Apply)
			{
				_items.Add(item);
				_itemsView.Refresh();
				_itemsView.MoveCurrentTo(item);
			}
		}

		private void OnDelete(Department item)
		{
			var vm = new ConfirmDialogViewModel($"Вы действительно хотите удалить подразделение {item.Name}?");
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Yes)
				if (Dal.Instance.DeleteDepartment(item))
					Items.Remove(item);
		}

		private void OnEdit(Department item)
		{
			var vm = new EditDepartmentViewModel("Редактирование подразделение", item);
			var result = DialogService.OpenDialog(vm);
			_itemsView.Refresh();
		}
	}
}
