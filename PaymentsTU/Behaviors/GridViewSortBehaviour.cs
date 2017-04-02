using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PaymentsTU.Behaviors
{
	internal sealed class GridViewSortBehaviour
	{
		#region Public attached properties

		public static ICommand GetCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(CommandProperty);
		}

		public static void SetCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(CommandProperty, value);
		}

		// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached(
				"Command",
				typeof(ICommand),
				typeof(GridViewSortBehaviour),
				new UIPropertyMetadata(
					null,
					(o, e) =>
					{
						ItemsControl listView = o as ItemsControl;
						if (listView != null)
						{
							if (!GetAutoSort(listView)) // Don't change click handler if AutoSort enabled
							{
								if (e.OldValue != null && e.NewValue == null)
								{
									listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
								}
								if (e.OldValue == null && e.NewValue != null)
								{
									listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
								}
							}
						}
					}
				)
			);

		public static bool GetAutoSort(DependencyObject obj)
		{
			return (bool)obj.GetValue(AutoSortProperty);
		}

		public static void SetAutoSort(DependencyObject obj, bool value)
		{
			obj.SetValue(AutoSortProperty, value);
		}

		// Using a DependencyProperty as the backing store for AutoSort.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty AutoSortProperty =
			DependencyProperty.RegisterAttached(
				"AutoSort",
				typeof(bool),
				typeof(GridViewSortBehaviour),
				new UIPropertyMetadata(
					false,
					(o, e) =>
					{
						ListView listView = o as ListView;
						if (listView != null)
						{
							if (GetCommand(listView) == null) // Don't change click handler if a command is set
							{
								bool oldValue = (bool)e.OldValue;
								bool newValue = (bool)e.NewValue;
								if (oldValue && !newValue)
								{
									listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
								}
								if (!oldValue && newValue)
								{
									listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
								}
							}
						}
					}
				)
			);

		public static string GetPropertyName(DependencyObject obj)
		{
			return (string)obj.GetValue(PropertyNameProperty);
		}

		public static void SetPropertyName(DependencyObject obj, string value)
		{
			obj.SetValue(PropertyNameProperty, value);
		}

		// Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PropertyNameProperty =
			DependencyProperty.RegisterAttached(
				"PropertyName",
				typeof(string),
				typeof(GridViewSortBehaviour),
				new UIPropertyMetadata(null)
			);

		public static bool GetShowSortGlyph(DependencyObject obj)
		{
			return (bool)obj.GetValue(ShowSortGlyphProperty);
		}

		public static void SetShowSortGlyph(DependencyObject obj, bool value)
		{
			obj.SetValue(ShowSortGlyphProperty, value);
		}

		// Using a DependencyProperty as the backing store for ShowSortGlyph.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ShowSortGlyphProperty =
			DependencyProperty.RegisterAttached("ShowSortGlyph", typeof(bool), typeof(GridViewSortBehaviour), new UIPropertyMetadata(true));

		public static ImageSource GetSortGlyphAscending(DependencyObject obj)
		{
			return (ImageSource)obj.GetValue(SortGlyphAscendingProperty);
		}

		public static void SetSortGlyphAscending(DependencyObject obj, ImageSource value)
		{
			obj.SetValue(SortGlyphAscendingProperty, value);
		}

		// Using a DependencyProperty as the backing store for SortGlyphAscending.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SortGlyphAscendingProperty =
			DependencyProperty.RegisterAttached("SortGlyphAscending", typeof(ImageSource), typeof(GridViewSortBehaviour), new UIPropertyMetadata(null));

		public static ImageSource GetSortGlyphDescending(DependencyObject obj)
		{
			return (ImageSource)obj.GetValue(SortGlyphDescendingProperty);
		}

		public static void SetSortGlyphDescending(DependencyObject obj, ImageSource value)
		{
			obj.SetValue(SortGlyphDescendingProperty, value);
		}

		// Using a DependencyProperty as the backing store for SortGlyphDescending.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SortGlyphDescendingProperty =
			DependencyProperty.RegisterAttached("SortGlyphDescending", typeof(ImageSource), typeof(GridViewSortBehaviour), new UIPropertyMetadata(null));

		#endregion

		#region Private attached properties

		private static GridViewColumnHeader GetSortedColumnHeader(DependencyObject obj)
		{
			return (GridViewColumnHeader)obj.GetValue(SortedColumnHeaderProperty);
		}

		private static void SetSortedColumnHeader(DependencyObject obj, GridViewColumnHeader value)
		{
			obj.SetValue(SortedColumnHeaderProperty, value);
		}

		// Using a DependencyProperty as the backing store for SortedColumn.  This enables animation, styling, binding, etc...
		private static readonly DependencyProperty SortedColumnHeaderProperty =
			DependencyProperty.RegisterAttached("SortedColumnHeader", typeof(GridViewColumnHeader), typeof(GridViewSortBehaviour), new UIPropertyMetadata(null));

		#endregion

		#region Column header click event handler

		private static void ColumnHeader_Click(object sender, RoutedEventArgs e)
		{
			GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
			if (headerClicked != null && headerClicked.Column != null)
			{
				string propertyName = GetPropertyName(headerClicked.Column);
				if (!string.IsNullOrEmpty(propertyName))
				{
					ListView listView = GetAncestor<ListView>(headerClicked);
					if (listView != null)
					{
						ICommand command = GetCommand(listView);
						if (command != null)
						{
							if (command.CanExecute(propertyName))
							{
								command.Execute(propertyName);
							}
						}
						else if (GetAutoSort(listView))
						{
							ApplySort(listView.Items, propertyName, listView, headerClicked);
						}
					}
				}
			}
		}

		#endregion

		#region Helper methods

		public static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
		{
			DependencyObject parent = VisualTreeHelper.GetParent(reference);
			while (!(parent is T))
			{
				parent = VisualTreeHelper.GetParent(parent);
			}
			if (parent != null)
				return (T)parent;
			else
				return null;
		}

		public static void ApplySort(ICollectionView view, string propertyName, ListView listView, GridViewColumnHeader sortedColumnHeader)
		{
			ListSortDirection direction = ListSortDirection.Ascending;
			if (view.SortDescriptions.Count > 0)
			{
				SortDescription currentSort = view.SortDescriptions[0];
				if (currentSort.PropertyName == propertyName)
				{
					if (currentSort.Direction == ListSortDirection.Ascending)
						direction = ListSortDirection.Descending;
					else
						direction = ListSortDirection.Ascending;
				}
				view.SortDescriptions.Clear();

				GridViewColumnHeader currentSortedColumnHeader = GetSortedColumnHeader(listView);
				if (currentSortedColumnHeader != null)
				{
					RemoveSortGlyph(currentSortedColumnHeader);
				}
			}
			if (!string.IsNullOrEmpty(propertyName))
			{
				view.SortDescriptions.Add(new SortDescription(propertyName, direction));
				if (GetShowSortGlyph(listView))
					AddSortGlyph(
						sortedColumnHeader,
						direction,
						direction == ListSortDirection.Ascending ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView));
				SetSortedColumnHeader(listView, sortedColumnHeader);
			}
		}

		private static void AddSortGlyph(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
			adornerLayer.Add(
				new SortGlyphAdorner(
					columnHeader,
					direction,
					sortGlyph
					));
		}

		private static void RemoveSortGlyph(GridViewColumnHeader columnHeader)
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
			Adorner[] adorners = adornerLayer.GetAdorners(columnHeader);
			if (adorners != null)
			{
				foreach (Adorner adorner in adorners)
				{
					if (adorner is SortGlyphAdorner)
						adornerLayer.Remove(adorner);
				}
			}
		}

		#endregion

		#region SortGlyphAdorner nested class

		private class SortGlyphAdorner : Adorner
		{
			private GridViewColumnHeader _columnHeader;
			private ListSortDirection _direction;
			private ImageSource _sortGlyph;

			public SortGlyphAdorner(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
				: base(columnHeader)
			{
				_columnHeader = columnHeader;
				_direction = direction;
				_sortGlyph = sortGlyph;
			}

			private Geometry GetDefaultGlyph()
			{
				double x1 = _columnHeader.ActualWidth - 13;
				double x2 = x1 + 10;
				double x3 = x1 + 5;
				double y1 = _columnHeader.ActualHeight / 2 - 3;
				double y2 = y1 + 5;

				if (_direction == ListSortDirection.Ascending)
				{
					double tmp = y1;
					y1 = y2;
					y2 = tmp;
				}

				PathSegmentCollection pathSegmentCollection = new PathSegmentCollection();
				pathSegmentCollection.Add(new LineSegment(new Point(x2, y1), true));
				pathSegmentCollection.Add(new LineSegment(new Point(x3, y2), true));

				PathFigure pathFigure = new PathFigure(
					new Point(x1, y1),
					pathSegmentCollection,
					true);

				PathFigureCollection pathFigureCollection = new PathFigureCollection();
				pathFigureCollection.Add(pathFigure);

				PathGeometry pathGeometry = new PathGeometry(pathFigureCollection);
				return pathGeometry;
			}

			protected override void OnRender(DrawingContext drawingContext)
			{
				base.OnRender(drawingContext);

				if (_sortGlyph != null)
				{
					double x = _columnHeader.ActualWidth - 13;
					double y = _columnHeader.ActualHeight / 2 - 5;
					Rect rect = new Rect(x, y, 10, 10);
					drawingContext.DrawImage(_sortGlyph, rect);
				}
				else
				{
					drawingContext.DrawGeometry(Brushes.LightGray, new Pen(Brushes.Gray, 1.0), GetDefaultGlyph());
				}
			}
		}

		#endregion
	}
}
/*public class ListViewGridSortableBehavior : Behavior
    {
        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(GridHeaderClickEventHandler));
            }
            base.OnAttached();
        }
 
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;
 
        void GridHeaderClickEventHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked =
                  e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;
 
            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }
 
                    if (_lastHeaderClicked != null) 
                    {
                        SetSortDownVisibility(_lastHeaderClicked, Visibility.Collapsed);
                        SetSortUpVisibility(_lastHeaderClicked, Visibility.Collapsed);
                    }
 
                    //string header = headerClicked.Column.Header as string;
                    String sortString = GetSortHeaderString(headerClicked);
                    if (String.IsNullOrEmpty(sortString)) return;
 
                    Sort(sortString, direction);
 
                    if (direction == ListSortDirection.Ascending)
                    {
                        SetSortDownVisibility(headerClicked, Visibility.Collapsed);
                        SetSortUpVisibility(headerClicked, Visibility.Visible);
                    }
                    else
                    {
                        SetSortDownVisibility(headerClicked, Visibility.Visible);
                        SetSortUpVisibility(headerClicked, Visibility.Collapsed);
                    }
 
                    // Remove arrow from previously sorted header 
                    if (_lastHeaderClicked != null &amp;&amp; _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }
 
 
                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
 
        }
        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(AssociatedObject.ItemsSource);
 
            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
 
        public static readonly DependencyProperty SortHeaderStringProperty =
           DependencyProperty.RegisterAttached
           (
               "SortHeaderString",
               typeof(String),
               typeof(GridViewColumnHeader),
               new UIPropertyMetadata(String.Empty)
           );
 
        public static String GetSortHeaderString(DependencyObject obj)
        {
            return (String)obj.GetValue(SortHeaderStringProperty);
        }
 
        public static void SetSortHeaderString(DependencyObject obj, String value)
        {
            obj.SetValue(SortHeaderStringProperty, value);
        }
 
        public static readonly DependencyProperty SortDownVisibilityProperty =
          DependencyProperty.RegisterAttached
          (
              "SortDownVisibility",
              typeof(Visibility),
              typeof(GridViewColumnHeader),
              new UIPropertyMetadata(Visibility.Collapsed)
          );
 
        public static Visibility GetSortDownVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(SortDownVisibilityProperty);
        }
 
        public static void SetSortDownVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(SortDownVisibilityProperty, value);
        }
 
        public static readonly DependencyProperty SortUpVisibilityProperty =
         DependencyProperty.RegisterAttached
         (
             "SortUpVisibility",
             typeof(Visibility),
             typeof(GridViewColumnHeader),
             new UIPropertyMetadata(Visibility.Collapsed)
         );
 
        public static Visibility GetSortUpVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(SortUpVisibilityProperty);
        }
 
        public static void SetSortUpVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(SortUpVisibilityProperty, value);
        }
    }*/
