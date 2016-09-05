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

		public ObservableCollection<Employee> Items => _employees;

	    private Employee _currentItem => _employeesView.CurrentItem as Employee;

	    public string Title
        {
            get
            {
                return "Сотрудники";
            }
        }

        public Dal Repository
        {
            get;
            private set;
        }

        public EmployeeViewModel()
        {
            Repository = new Dal();
            _employees = new ObservableCollection<Employee>(Repository.Employees());

			_employeesView = CollectionViewSource.GetDefaultView(_employees);
			_employeesView.MoveCurrentToPosition(_employees.Count > 0 ? 0 : - 1);
        }

    }
}
