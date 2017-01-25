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
	public sealed class DepartmentViewModel : ViewModelBase, IListPageViewModel<Department>
	{
		public string Title => "Подразделения";

		private ObservableCollection<Department> _items;

		public DataNavigationBarViewModel<Department> NavigationBar { get; private set; }
		public ListCollectionView ItemsDataView { get; }

		public DepartmentViewModel()
		{
			_items = new ObservableCollection<Department>(Dal.Instance.Departments());
			ItemsDataView = (ListCollectionView)CollectionViewSource.GetDefaultView(_items);
			ItemsDataView.CustomSort = new DepartmentComparer();

			ItemsDataView.MoveCurrentToPosition(_items.Count > 0 ? 0 : -1);

			NavigationBar = new DataNavigationBarViewModel<Department>(ItemsDataView, OnAdd, OnDelete, OnEdit, () => {
				_items.Clear();
				foreach (var payment in Dal.Instance.Departments())
				{
					_items.Add(payment);
				}
				ItemsDataView.Refresh();
				ItemsDataView.MoveCurrentToPosition(_items.Count > 0 ? 0 : -1);
			});
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
				ItemsDataView.Refresh();
				ItemsDataView.MoveCurrentTo(item);
			}
		}

		private void OnDelete(Department item)
		{
			var vm = new ConfirmDialogViewModel($"Вы действительно хотите удалить подразделение {item.Name}?");
			var result = DialogService.OpenDialog(vm);
			if (result == DialogResult.Yes)
				if (Dal.Instance.DeleteDepartment(item))
					_items.Remove(item);
		}

		private void OnEdit(Department item)
		{
			var editItem = (Department)item.Clone();
			
			var vm = new EditDepartmentViewModel("Редактирование подразделение", item);
			if (DialogService.OpenDialog(vm) == DialogResult.Apply)
			{
				var index = _items.IndexOf(item);
				_items[index] = editItem;
			}

			ItemsDataView.Refresh();
		}
	}
}
