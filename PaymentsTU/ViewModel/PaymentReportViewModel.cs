using PaymentsTU.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentsTU.ViewModel
{
	public sealed class PaymentReportViewModel : ViewModelBase, IPageBase
	{
		public string Title => "Отчет по платежам";

		public PaymentReportViewModel()
		{
			var reportData = Dal.Instance.PaymentReport(DateTime.Today.AddDays(-30), DateTime.Today);
			//Items = new ObservableCollection<Payment>(Dal.Instance.Payments().OrderByDescending(x => x.DatePayment).ThenBy(x => x.FullName));
			//ItemsDataView.Culture = CultureInfo.CurrentCulture;
			//ItemsDataView.MoveCurrentToPosition(Items.Count > 0 ? 0 : -1);
			////TODO: refactoring viewmodel for navigation bar
			//NavigationBar = new DataNavigationBarViewModel<Payment>(ItemsDataView, OnAdd, OnDelete, OnEdit, () =>
			//{
			//	Items.Clear();
			//	foreach (var payment in Dal.Instance.Payments())
			//	{
			//		Items.Add(payment);
			//	}
			//	ItemsDataView.Refresh();
			//	ItemsDataView.MoveCurrentToPosition(Items.Count > 0 ? 0 : -1);
			//});
		}
	}
}
