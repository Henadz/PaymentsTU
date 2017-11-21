namespace DocumentModel
{
	public sealed class NewPage
    {
        public string Title { get; private set; }
        public bool IsLandscapeOrientation { get; private set; }

        internal NewPage()
        {
        }

        public NewPage(string title)
        {
            Title = title;
        }

        public NewPage(string title, bool isLandscapeOrientation)
        {
            Title = title;
            IsLandscapeOrientation = isLandscapeOrientation;
        }
    }
}