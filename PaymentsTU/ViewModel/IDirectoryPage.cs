using System.Collections.ObjectModel;

namespace PaymentsTU.ViewModel
{
	internal interface IDirectoryPage<T> : IPageBase
	{
		ObservableCollection<T> Items { get; }
	}
}
