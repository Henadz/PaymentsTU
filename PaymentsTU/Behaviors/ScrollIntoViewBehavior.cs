using System.Windows;
using System.Windows.Controls;

namespace PaymentsTU.Behaviors
{
	public class ScrollIntoViewBehavior
	{
		public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.RegisterAttached
			(
				"SelectedValue",
				typeof (object),
				typeof (ScrollIntoViewBehavior),
				new PropertyMetadata(null, OnSelectedValueChange)
			);

		public static void SetSelectedValue(DependencyObject source, object value)
		{
			source.SetValue(SelectedValueProperty, value);
		}

		public static object GetSelectedValue(DependencyObject source)
		{
			return source.GetValue(SelectedValueProperty);
		}

		private static void OnSelectedValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var listbox = d as ListBox;
			listbox?.ScrollIntoView(e.NewValue);
		}
	}
}