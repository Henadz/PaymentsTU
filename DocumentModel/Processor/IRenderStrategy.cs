using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocumentModel.Processor
{
    public interface IRenderStrategy
    {
        void Setup(ProcessingModel model);
        void Render(IEnumerable<ProcessingChunk> items, Stream stream, Encoding encoding);
    }
}