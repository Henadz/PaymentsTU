using System.IO;
using System.Text;

namespace DocumentModel.Processor
{
    public interface IExportProcessor
    {
        void Export(ProcessingModel model, Stream stream, Encoding encoding, bool testMode);
    }
}