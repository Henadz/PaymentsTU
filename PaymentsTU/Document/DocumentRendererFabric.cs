using DocumentModel.Processor;
using System;
using System.Windows;

namespace PaymentsTU.Document
{
	internal static class DocumentRendererFabric
	{
		public static IRenderStrategy GetDocumentRender(DocumentType documentType)
		{
			switch (documentType)
			{
				case DocumentType.Xps:
					{
						return new XpsRenderStrategy(new Size(793, 1122), new Thickness(30, 15, 15, 15));
					}
				default:
					throw new NotImplementedException($"Document format not supported: {documentType}");
			}
		}
	}
}
