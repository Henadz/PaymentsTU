using PaymentsTU.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Data;

namespace PaymentsTU.ViewModel
{
    public sealed class EmployeeViewModel : ViewModelBase, IDirectoryPage<Employee>
    {
        private ObservableCollection<Employee> _employees;
		private ICollectionView _employeesView = null;

	    public DataNavigationBarViewModel<Employee> NavigationBar { get; private set; }

		public ObservableCollection<Employee> Items => _employees;

	    private Employee CurrentItem => _employeesView.CurrentItem as Employee;

	    public string Title => "Сотрудники";

	    public Dal Repository
        {
            get;
            private set;
        }

	    private Action<Employee> RefreshData = null;

        public EmployeeViewModel()
        {
            Repository = new Dal();
            _employees = new ObservableCollection<Employee>(Repository.Employees());

			_employeesView = CollectionViewSource.GetDefaultView(_employees);
			_employeesView.MoveCurrentToPosition(_employees.Count > 0 ? 0 : - 1);

			RefreshData = e => { _employeesView.Refresh(); };

			NavigationBar = new DataNavigationBarViewModel<Employee>((CollectionView)_employeesView, null, null, null, RefreshData);
        }

    }
}
