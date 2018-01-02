namespace DocumentModel.Processor
{
	public class Margin
	{
		public double Top { get; private set; }
		public double Bottom { get; private set; }
		public double Left { get; private set; }
		public double Right { get; private set; }

		public SizeUnit SizeUnit { get; private set; }

		public Margin(double left, double top, double right, double bottom)
		{
			Top = top;
			Bottom = bottom;
			Left = left;
			Right = right;

			SizeUnit = SizeUnit.Pixels;
		}
	}
}