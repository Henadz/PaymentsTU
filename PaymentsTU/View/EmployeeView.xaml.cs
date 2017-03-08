using PaymentsTU.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PaymentsTU.View
{
	/// <summary>
	/// Interaction logic for EmployeeView.xaml
	/// </summary>
	public partial class EmployeeView : UserControl, IView<EmployeeViewModel>
	{

		public EmployeeView()
		{
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
	}
}
