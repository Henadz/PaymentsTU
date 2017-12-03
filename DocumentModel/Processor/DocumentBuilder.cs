using System.Collections.Generic;
using System.IO;
using System.Text;
using FrameworkExtend;

namespace DocumentModel.Processor
{
	public sealed class DocumentBuilder : IDocumentBuilder
	{
		public DocumentType DocumentType { get; private set; }
		public Stream DocumentStream { get; private set; }
		public Encoding DocumentEncoding { get; private set; }

		public DocumentBuilder(DocumentType type, Stream stream)
			:this(type, stream, Encoding.UTF8)
		{
		}

		public DocumentBuilder(DocumentType type, Stream stream, Encoding encoding)
		{
			DocumentType = type;
			DocumentStream = stream;
			DocumentEncoding = encoding;
		}

		public void Build(Func<IEnumerable<object>> modelStrategy, Func<DocumentType, IRenderStrategy> renderFabric)
		{
			var items = modelStrategy();
			var renderer = renderFabric(DocumentType);

			renderer.Render(new[] { new ProcessingChunk(items) }, DocumentStream, DocumentEncoding);
		}
	}
}
