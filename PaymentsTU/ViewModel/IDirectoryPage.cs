using PaymentsTU.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PaymentsTU.ViewModel
{
    interface IDirectoryPage<T> : IPageBase
    {
        Dal Repository { get; }
        ObservableCollection<T> Items { get; }
    }
}
