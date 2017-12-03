using DocumentModel.Processor;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PaymentsTU.Document
{
	internal class XpsRenderStrategy : IRenderStrategy
	{
		public void Render(IEnumerable<ProcessingChunk> items, Stream stream, Encoding encoding)
		{
			throw new NotImplementedException();
		}

		public void Setup(ProcessingModel model)
		{
			throw new NotImplementedException();
		}
	}
}
