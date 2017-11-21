using System;

namespace DocumentModel.Processor
{
    public interface IExportFactory
    {
        IRenderStrategy CreateRenderStrategy(DocumentType type);
        IExportStrategy CreateExportStrategy(DocumentType format, Type type);
    }
}