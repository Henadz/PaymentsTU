namespace DocumentModel.Processor
{
    public interface IExportStrategy
    {
        ProcessingChunk GenerateDocumentChunk(object item);
    }
}