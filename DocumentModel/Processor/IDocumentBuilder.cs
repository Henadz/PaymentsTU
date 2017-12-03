using FrameworkExtend;
using System.Collections.Generic;

namespace DocumentModel.Processor
{
	public interface IDocumentBuilder
	{
		void Build(Func<IEnumerable<object>> modelStrategy, Func<DocumentType, IRenderStrategy> renderFabric);
	}
}
