using System.Collections.ObjectModel;
using PaymentsTU.Model;

namespace PaymentsTU.ViewModel
{
	public sealed class MainViewModel : ViewModelBase
	{
		public ObservableCollection<TabPageModel> Pages { get; private set; }

		private int _selectedTab;
		public int SelectedTab
		{
			get { return _selectedTab; }
			set
			{
				_selectedTab = value;
				OnPropertyChanged(nameof(SelectedTab));
			}
		}

		public MainViewModel()
		{
			Pages = new ObservableCollection<TabPageModel>
			{
				//new MatrixViewModel(),
				new TabPageModel("Выплаты", new Lazy<IPageBase>(() => new PaymentViewModel())),
				new TabPageModel("Отчет за период", new Lazy<IPageBase>(() => new ReportViewModel())),
				new TabPageModel("Сотрудники", new Lazy<IPageBase>(() => new EmployeeViewModel())),
				new TabPageModel("Подразделения", new Lazy<IPageBase>(() => new DepartmentViewModel())),
				new TabPageModel("Виды платежей", new Lazy<IPageBase>(() => new PaymentTypeViewModel())),
				//new PeriodViewModel()
			};
		}
	}

	public sealed class TabPageModel
	{
		public string Title { get; private set; }
		public Lazy<IPageBase> PageModel { get; private set; }

		public TabPageModel(string title, Lazy<IPageBase> pageModel)
		{
			Title = title;
			PageModel = pageModel;
		}
	}
}