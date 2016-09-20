using PaymentsTU.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace PaymentsTU.ViewModel
{
    public sealed class DepartmentViewModel : ViewModelBase, IDirectoryPage<Department>
    {
        private ObservableCollection<Department> _departments;

        public ObservableCollection<Department> Items
        {
            get
            {
                return _departments;
            }
        }

        public string Title
        {
            get
            {
                return "Подразделения";
            }
        }

        public DepartmentViewModel()
        {
            _departments = new ObservableCollection<Department>(Dal.Instance.Departments());
        }
    }
}
