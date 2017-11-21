using FrameworkExtend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocumentModel.Processor
{
    public sealed class DefaultExportProcessor : IExportProcessor
    {
        private readonly IExportFactory _factory;

        public DefaultExportProcessor(IExportFactory factory)
        {
            _factory = factory;
        }
        
        public void Export(ProcessingModel model, Stream stream, Encoding encoding, bool testMode)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var list = new List<ProcessingChunk>();
            var format = model.Format;

            foreach (var exportable in model.Items)
            {
                var type = exportable.GetType();
                
                var exportStrategy = _factory.CreateExportStrategy(format, type);

                if (exportStrategy == null) throw new InvalidOperationException("Export strategy for type [" + type + "] and format [" + format + "] is not registered");

                list.Add(exportStrategy.GenerateDocumentChunk(exportable));
            }

            var renderStrategy = _factory.CreateRenderStrategy(format);

            if (renderStrategy == null) throw new InvalidOperationException("Render strategy for format [" + format + "] is not registered");

            var disposables = list.SelectMany(x => x != null && x.Artifacts != null ? x.Artifacts : new List<object>(0).AsEnumerable()).OfType<IDisposable>().ToArray();
            try
            {
                renderStrategy.Setup(model);
                renderStrategy.Render(list, stream, encoding);
            }
            finally
            {
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }
            }

        }
    }
}