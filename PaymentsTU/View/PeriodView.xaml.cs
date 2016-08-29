using PaymentsTU.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PaymentsTU.View
{
    /// <summary>
    /// Interaction logic for EmployeeView.xaml
    /// </summary>
    public partial class PeriodView : UserControl, IView<PeriodViewModel>
    {

        public PeriodView()
        {
            //ViewModel = new PeriodViewModel();
            InitializeComponent();
        }

        public PeriodViewModel ViewModel
        {
            get;
            set;
        }
    }
}
