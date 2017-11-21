using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace DocumentModel.Processor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class MappedStrategyTypeAttribute : Attribute
    {
        public Type MappedType { get; private set; }

        public ReadOnlyCollection<DocumentType> ExportTypes { get; private set; }

        public MappedStrategyTypeAttribute(Type mappedType, params DocumentType[] exportTypes)
        {
            MappedType = mappedType;
            ExportTypes = new ReadOnlyCollection<DocumentType>(exportTypes);
        }
    }
}