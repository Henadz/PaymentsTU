using PaymentsTU.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace PaymentsTU.ViewModel
{
    public class PeriodViewModel : ViewModelBase, IListPageViewModel<FinancialPeriod>
    {
        private ObservableCollection<FinancialPeriod> _periods;
        public ObservableCollection<FinancialPeriod> Items
        {
            get
            {
                return _periods;
            }
        }

	    public ListCollectionView ItemsDataView { get; }

	    //public Dal Repository
     //   {
     //       get
     //       {
     //           throw new NotImplementedException();
     //       }
     //   }

        public string Title
        {
            get
            {
                return "Платежные периоды";
            }
        }

        public PeriodViewModel()
        {
            //Repository = new Dal();
            _periods = new ObservableCollection<FinancialPeriod>(Dal.FinancialPeriods());
        }
    }
}
