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

namespace PaymentsTU.View
{
	/// <summary>
	/// Interaction logic for TypePaymentView.xaml
	/// </summary>
	public partial class TypePaymentView : UserControl, IView<PaymentTypeViewModel>
	{
		public TypePaymentView()
		{
			InitializeComponent();
		}

		public PaymentTypeViewModel ViewModel
		{
			get { return (PaymentTypeViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty =
			DependencyProperty.Register("ViewModel",
			typeof(PaymentTypeViewModel), typeof(TypePaymentView), new PropertyMetadata(null));
	}
}
