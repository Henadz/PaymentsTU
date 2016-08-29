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
    public partial class DepartmentView : UserControl, IView<DepartmentViewModel>
    {

        public DepartmentView()
        {
            //ViewModel = new DepartmentViewModel();
            InitializeComponent();
        }

        //public DepartmentViewModel ViewModel
        //{
        //    get;
        //    set;
        //}

        public DepartmentViewModel ViewModel
        {
            get { return (DepartmentViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        /// <summary>
        /// Identifier for the ColumnCount dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
            typeof(DepartmentViewModel), typeof(DepartmentView), new PropertyMetadata(null));
    }
}
