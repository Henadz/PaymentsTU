using System.Collections.ObjectModel;
using PaymentsTU.Model;

namespace PaymentsTU.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        private ObservableCollection<FinancialPeriod> _periods;
        public ObservableCollection<FinancialPeriod> Periods
        {
            get { return _periods; }
            set
            {
                _periods = value;
                OnPropertyChanged(nameof(Periods));
            }
        }

        private FinancialPeriod _currentPeriod;

        public FinancialPeriod CurrentPeriod
        {
            get { return _currentPeriod; }
            set
            {
                _currentPeriod = value;
                OnPropertyChanged(nameof(CurrentPeriod));
            }
        }

        public MainViewModel()
        {
            _periods = new ObservableCollection<FinancialPeriod>(Dal.FinancialPeriods());
        }
    }
}