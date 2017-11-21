using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace DocumentModel.Processor
{
	/// <summary>
	/// Defines content that shall be rendered by the Export Processor
	/// </summary>
	public sealed class ProcessingChunk
    {
        /// <summary>
        /// Artifacts to be rendered in order of arrangement within the list
        /// </summary>
        public ReadOnlyCollection<object> Artifacts { get; private set; }

        /// <summary>
        /// USed for serialization
        /// </summary>
        internal ProcessingChunk()
        {
        }

        public ProcessingChunk(params object[] artifacts)
        {
            if (artifacts == null)
                throw new ArgumentNullException("artifacts");

            Artifacts = new ReadOnlyCollection<object>(artifacts);
        }

        public ProcessingChunk(IEnumerable<object> artifacts)
        {
            if (artifacts == null)
                throw new ArgumentNullException("artifacts");

            var lst = new List<object>(artifacts);

            Artifacts = new ReadOnlyCollection<object>(lst);
        }
    }
}