using System.Windows;
using System.Windows.Controls;

namespace PaymentsTU.Controls
{
	/// <summary>
	/// Interaction logic for LabeledTextBox.xaml
	/// </summary>
	public partial class LabeledTextBox : UserControl
	{
		public static readonly DependencyProperty LabelProperty = DependencyProperty
		.Register("Label",
				typeof(string),
				typeof(LabeledTextBox),
				new FrameworkPropertyMetadata("Unnamed Label"));

		public static readonly DependencyProperty TextProperty = DependencyProperty
			.Register("Text",
					typeof(string),
					typeof(LabeledTextBox),
					new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public LabeledTextBox()
		{
			InitializeComponent();
			Root.DataContext = this;
		}

		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
}
