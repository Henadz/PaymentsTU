using DocumentModel.Processor;
using System;

namespace PaymentsTU.Document
{
	internal static class DocumentRendererFabric
	{
		public static IRenderStrategy GetDocumentRender(DocumentType documentType)
		{
			switch (documentType)
			{
				case DocumentType.Xps:
					return new XpsRenderStrategy();
				default:
					throw new NotImplementedException($"Document format not supported: {documentType}");
			}
		}
	}
}
