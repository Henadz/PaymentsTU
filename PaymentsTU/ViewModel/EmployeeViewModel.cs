using PaymentsTU.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PaymentsTU.ViewModel
{
    public sealed class EmployeeViewModel : ViewModelBase, IDirectoryPage<Employee>
    {
        private ObservableCollection<Employee> _employees;

        public ObservableCollection<Employee> Items
        {
            get
            {
                return _employees;
            }
        }

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
        }

    }
}
