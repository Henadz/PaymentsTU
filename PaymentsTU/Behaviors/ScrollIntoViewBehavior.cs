using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using FrameworkExtend;

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

		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.RegisterAttached
			(
				"SelectedItem",
				typeof(object),
				typeof(ScrollIntoViewBehavior),
				new PropertyMetadata(null, OnSelectedItemChange)
			);

		public static void SetSelectedItem(DependencyObject source, object value)
		{
			source.SetValue(SelectedItemProperty, value);
		}

		public static object GetSelectedItem(DependencyObject source)
		{
			return source.GetValue(SelectedItemProperty);
		}

		private static void OnSelectedItemChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var listbox = d as ListBox;
			if (listbox == null) return;

			if (listbox.IsGrouping)
			{
				if (listbox.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
				{
					listbox.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => DelayedBringIntoView(listbox, e.NewValue)));
				}
				else
				{
					listbox.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
				}
			}
			else
			{
				listbox.ScrollIntoView(e.NewValue);
			}
		}

		private static void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
		{
			var control = (ListBox) sender;
			if (control.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
				return;

			control.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
			control.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => DelayedBringIntoView(control, control.SelectedItem)));
		}

		private static void DelayedBringIntoView(ListBox control, object selectedItem)
		{
			var item = control.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListBoxItem;
			item?.BringIntoView();
		}
	}
}