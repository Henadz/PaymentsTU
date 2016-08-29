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
using PaymentsTU.ViewModel;

namespace PaymentsTU
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IView<MainViewModel>
	{
		public MainWindow()
		{
            //ViewModel = new MainViewModel();
			InitializeComponent();
		}

	    public MainViewModel ViewModel { get; set; }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
