using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentsTU.ViewModel
{
    public sealed class PaymentViewModel : ViewModelBase, IPageBase
    {
        public string Title
        {
            get
            {
                return "Платежи";
            }
        }
    }
}
