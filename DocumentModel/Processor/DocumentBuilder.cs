using System.Collections.Generic;
using System.IO;
using System.Text;
using FrameworkExtend;

namespace DocumentModel.Processor
{
	public sealed class DocumentBuilder : IDocumentBuilder
	{
		public DocumentType DocumentType { get; private set; }
		public Page PageSettings { get; private set; }
		public Stream DocumentStream { get; private set; }
		public Encoding DocumentEncoding { get; private set; }

		public DocumentBuilder(DocumentType type, Page page, Stream stream)
			:this(type, page, stream, Encoding.UTF8)
		{
		}

		public DocumentBuilder(DocumentType type, Page page, Stream stream, Encoding encoding)
		{
			DocumentType = type;
			DocumentStream = stream;
			DocumentEncoding = encoding;
			PageSettings = page;
		}

		public void Build(Func<IEnumerable<object>> modelStrategy, Func<DocumentType, Page, IRenderStrategy> renderFabric)
		{
			var items = modelStrategy();
			var renderer = renderFabric(DocumentType, PageSettings);

			renderer.Render(new[] { new ProcessingChunk(items) }, DocumentStream, DocumentEncoding);
		}
	}
}
