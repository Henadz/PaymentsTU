using System.Collections.Generic;

namespace DocumentModel.Processor
{
    public sealed class ProcessingModel
    {
        public ProcessingModel(DocumentType format, IEnumerable<object> items)
        {
            Format = format;
            Items = items;
        }

        public ProcessingModel(DocumentType format, object item)
        {
            Format = format;
            Items = new [] { item };
        }

        public DocumentType Format { get; private set; }
        public IEnumerable<object> Items { get; private set; }

        public string FileName { get; set; }
    }
}