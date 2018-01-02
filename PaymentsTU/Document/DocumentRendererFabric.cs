using DocumentModel.Processor;
using System;
using System.Windows;

namespace PaymentsTU.Document
{
	internal static class DocumentRendererFabric
	{
		public static IRenderStrategy GetDocumentRender(DocumentType documentType, Page pageSettings)
		{
			switch (documentType)
			{
				case DocumentType.Xps:
				{
					var ps = pageSettings.PageSize;
					var m = pageSettings.PageMargin;
					//if (m.SizeUnit == SizeUnit.Pixels)
					return new XpsRenderStrategy(new Size(ps.Width, ps.Height), new Thickness(m.Left, m.Top, m.Right, m.Bottom));
				}
				default:
					throw new NotImplementedException($"Document format not supported: {documentType}");
			}
		}
	}
}
