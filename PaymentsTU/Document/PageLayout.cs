using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PaymentsTU.Document
{
	internal class PageLayout
	{
		private Grid _pageGrid;

		private Grid _content;
		private Grid _baseContent;
		private Grid _contentColumn;

		private Size _pageSize;
		private Thickness _pageMargin;

		private double _availableHeight;
		private double _columnHeight;

		private int _columnIndex;

		public event EventHandler<EventArgs> OnNewColumn;

		public double ContentWidth { get; private set; }

		public PageLayout(Size pageSize, Thickness margin)
		{
			_pageSize = pageSize;
			_pageMargin = margin;

			Initialize();
		}

		private void Initialize()
		{
			_pageGrid = new Grid();
			_baseContent = new Grid();

			AddGridRow(_pageGrid, GridLength.Auto);
			AddGridRow(_pageGrid, new GridLength(1d, GridUnitType.Star));
			AddGridRow(_pageGrid, GridLength.Auto);

			_baseContent.SetValue(Grid.RowProperty, 1);
			_pageGrid.Children.Add(_baseContent);

			_availableHeight = _pageSize.Height - (_pageMargin.Top + _pageMargin.Bottom);

			SetContentColumns(1);
		}

		private int AddGridRow(Grid grid, GridLength rowHeight)
		{
			var rowDef = new RowDefinition {Height = rowHeight};

			grid.RowDefinitions.Add(rowDef);

			return grid.RowDefinitions.Count - 1;
		}

		public void SetHeader(object content)
		{
			//var pageContent = new ContentControl { Content = content };
			//AddGridRow(_content, new GridLength(1d, GridUnitType.Star));
			//pageContent.SetValue(Grid.RowProperty, _content.RowDefinitions.Count - 1);

			//_content.Children.Add(pageContent);
		}

		public void SetFooter(object content)
		{
			//var pageContent = new ContentControl { Content = content };
			//AddGridRow(_content, new GridLength(1d, GridUnitType.Star));
			//pageContent.SetValue(Grid.RowProperty, _content.RowDefinitions.Count - 1);

			//_content.Children.Add(pageContent);
		}

		public bool AddContent(FrameworkElement content)
		{
			var height = MeasureHeight(content);

			if (height > _availableHeight)
			{
				if (_contentColumn == null || _columnIndex == _contentColumn.ColumnDefinitions.Count - 1)
					return false;
				ChangeColumnIndex(++_columnIndex);
			}

			var rowIndex = AddGridRow(_content, GridLength.Auto);
			content.SetValue(Grid.RowProperty, rowIndex);
			content.SetValue(Grid.ColumnProperty, _columnIndex);

			_content.Children.Add(content);

			_availableHeight -= height;

			return true;
		}

		public void SetContentColumns(int count, double columnGap = 25.0)
		{
			var grid = new Grid();
			var columnWidth = (_pageSize.Width - _pageMargin.Left - _pageMargin.Right) / count;
			ContentWidth = columnWidth - columnGap;

			for (var i = 0; i < count; i++)
			{
				var colDefinition = new ColumnDefinition { Width = new GridLength(columnWidth, GridUnitType.Pixel) };
				grid.ColumnDefinitions.Add(colDefinition);
			}

			AddGridRow(_baseContent, new GridLength(1d, GridUnitType.Star));
			grid.SetValue(Grid.RowProperty, _baseContent.RowDefinitions.Count - 1);

			_baseContent.Children.Add(grid);

			_contentColumn = grid;
			_columnHeight = _availableHeight;
			ChangeColumnIndex(0);
		}

		private void ChangeColumnIndex(int index)
		{
			var grid = new Grid();

			if (_contentColumn.RowDefinitions.Count == 0)
				AddGridRow(_contentColumn, new GridLength(1d, GridUnitType.Star));
			grid.SetValue(Grid.RowProperty, 0);
			grid.SetValue(Grid.ColumnProperty, index);

			_contentColumn.Children.Add(grid);

			_content = grid;
			_columnIndex = index;
			_availableHeight = _columnHeight;

			OnNewColumn?.Invoke(this, EventArgs.Empty);
		}

		public PageContent GetPageContent()
		{
			var page = new FixedPage
			{
				RenderSize = _pageSize,
				Margin = _pageMargin
			};

			page.Children.Add(_pageGrid);

			var content = new PageContent();
			((System.Windows.Markup.IAddChild)content).AddChild(page);
			return content;
		}

		private double MeasureHeight(FrameworkElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			element.Measure(new Size(_pageSize.Width - (_pageMargin.Left + _pageMargin.Right), _pageSize.Height));
			return element.DesiredSize.Height;
		}
	}
}