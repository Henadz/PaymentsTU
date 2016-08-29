using System.Collections.ObjectModel;
using PaymentsTU.Model;

namespace PaymentsTU.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        //private ObservableCollection<FinancialPeriod> _periods;
        //public ObservableCollection<FinancialPeriod> Periods
        //{
        //    get { return _periods; }
        //    set
        //    {
        //        _periods = value;
        //        OnPropertyChanged(nameof(Periods));
        //    }
        //}

        //private FinancialPeriod _currentPeriod;

        //public FinancialPeriod CurrentPeriod
        //{
        //    get { return _currentPeriod; }
        //    set
        //    {
        //        _currentPeriod = value;
        //        OnPropertyChanged(nameof(CurrentPeriod));
        //    }
        //}

        //private ViewType _currentView;
        //public ViewType CurrentView
        //{
        //    get { return _currentView; }
        //    set
        //    {
        //        _currentView = value;
        //        OnPropertyChanged(nameof(CurrentView));
        //    }
        //}
        public ObservableCollection<IPageBase> Pages { get; private set; }

        private object _pageModel;
        public object PageModel
        {
            get { return _pageModel; }
            private set
            {
                _pageModel = value;
                OnPropertyChanged(nameof(PageModel));
            }
        }

        private int _selectedTab;
        public int SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;
                _pageModel = Pages[_selectedTab];
                OnPropertyChanged(nameof(SelectedTab));
            }
        }

        public MainViewModel()
        {
            Pages = new ObservableCollection<IPageBase>
            {
                new MatrixViewModel(),
                new PaymentViewModel(),
                new EmployeeViewModel(),
                new DepartmentViewModel(),
                new PeriodViewModel()
            };
            //_periods = new ObservableCollection<FinancialPeriod>(Dal.FinancialPeriods());
            //_currentView = ViewType.Balance;
        }
    }
}