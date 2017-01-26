using PaymentsTU.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PaymentsTU.View
{
	/// <summary>
	/// Interaction logic for EmployeeView.xaml
	/// </summary>
	public partial class PaymentView : UserControl, IView<PaymentViewModel>
	{

		public PaymentView()
		{
			InitializeComponent();
		}

		public PaymentViewModel ViewModel
		{
			get { return (PaymentViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		/// <summary>
		/// Identifier for the ColumnCount dependency property.
		/// </summary>
		public static readonly DependencyProperty ViewModelProperty =
			DependencyProperty.Register("ViewModel",
			typeof(PaymentViewModel), typeof(PaymentView), new PropertyMetadata(null));
	}
}
