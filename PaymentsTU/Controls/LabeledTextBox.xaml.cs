using System.Windows;
using System.Windows.Controls;
using PaymentsTU.Behaviors;

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

		public static readonly DependencyProperty TextBoxHeightProperty = DependencyProperty
			.Register("TextBoxHeight",
					typeof(int),
					typeof(LabeledTextBox),
					new FrameworkPropertyMetadata(23));

		public static readonly DependencyProperty MaskProperty = DependencyProperty
			.Register("Mask",
					typeof(MaskType),
					typeof(LabeledTextBox),
					new FrameworkPropertyMetadata(MaskType.Any));

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

		public int TextBoxHeight
		{
			get { return (int)GetValue(TextBoxHeightProperty); }
			set { SetValue(TextBoxHeightProperty, value); }
		}

		public MaskType Mask
		{
			get { return (MaskType)GetValue(MaskProperty); }
			set { SetValue(MaskProperty, value); }
		}
	}
}
