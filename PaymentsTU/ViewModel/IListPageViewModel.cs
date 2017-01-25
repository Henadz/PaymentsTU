using System.Collections.ObjectModel;
using System.Windows.Data;

namespace PaymentsTU.ViewModel
{
	internal interface IListPageViewModel<T> : IPageBase
	{
		ListCollectionView ItemsDataView { get; }
	}
}
