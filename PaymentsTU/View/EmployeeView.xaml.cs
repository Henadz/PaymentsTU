using PaymentsTU.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PaymentsTU.View
{
    /// <summary>
    /// Interaction logic for EmployeeView.xaml
    /// </summary>
    public partial class EmployeeView : UserControl, IView<EmployeeViewModel>
    {

        public EmployeeView()
        {
           // ViewModel = new EmployeeViewModel();
            InitializeComponent();
            var dc = this.DataContext;
        }

        public EmployeeViewModel ViewModel
        {
            get { return (EmployeeViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        /// <summary>
        /// Identifier for the ColumnCount dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel",
            typeof(EmployeeViewModel), typeof(EmployeeView), new PropertyMetadata(null));

		//private void Expander_Expanded(object sender, RoutedEventArgs e)
		//{
		//	this.ScrollIntoView(this.Directory.SelectedItem);
		//}

		//public void ScrollIntoView(object item)
		//{
		//	if (this.Directory.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
		//		this.OnBringItemIntoView(item);
		//	else
		//		this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Delegate)new DispatcherOperationCallback(this.OnBringItemIntoView), item);
		//}

		//private object OnBringItemIntoView(object arg)
		//{
		//	FrameworkElement frameworkElement = this.Directory.ItemContainerGenerator.ContainerFromItem(arg) as FrameworkElement;
		//	if (frameworkElement != null)
		//		frameworkElement.BringIntoView();
		//	else if (!this.Directory.IsGrouping && this.Directory.Items.Contains(arg))
		//	{
		//		//VirtualizingPanel virtualizingPanel = this.Directory.ItemsHost as VirtualizingPanel;
		//		//if (virtualizingPanel != null)
		//		//	virtualizingPanel.Directory.BringIndexIntoView(this.Items.IndexOf(arg));
		//	}
		//	return (object)null;
		//}
	}
}
