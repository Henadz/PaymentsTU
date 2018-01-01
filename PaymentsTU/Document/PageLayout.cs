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

		private Size _pageSize;
		private Thickness _pageMargin;

		private double _availableHeight;

		public PageLayout(Size pageSize, Thickness margin)
		{
			_pageSize = pageSize;
			_pageMargin = margin;

			Initialize();
		}

		private void Initialize()
		{
			_pageGrid = new Grid();
			_content = new Grid();

			AddGridRow(_pageGrid, GridLength.Auto);
			AddGridRow(_pageGrid, new GridLength(1d, GridUnitType.Star));
			AddGridRow(_pageGrid, GridLength.Auto);

			_content.SetValue(Grid.RowProperty, 1);
			_pageGrid.Children.Add(_content);

			_availableHeight = _pageSize.Height - (_pageMargin.Top + _pageMargin.Bottom);
		}

		private void AddGridRow(Grid grid, GridLength rowHeight)
		{
			if (grid == null)
				return;

			var rowDef = new RowDefinition {Height = rowHeight};

			grid.RowDefinitions.Add(rowDef);
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
				return false;

			AddGridRow(_content, new GridLength(1d, GridUnitType.Star));
			content.SetValue(Grid.RowProperty, _content.RowDefinitions.Count - 1);

			_content.Children.Add(content);

			_availableHeight -= height;

			return true;
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