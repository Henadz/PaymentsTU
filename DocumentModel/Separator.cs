namespace DocumentModel
{
	/// <summary>
	/// Renderer shall insert appropriate entity into the document when
	/// meets this. (E.g. an empty row for Excel).
	/// </summary>
	public sealed class Separator
    {
        public readonly static Separator Instance = new Separator();

        internal Separator()
        {
        }
    }
}