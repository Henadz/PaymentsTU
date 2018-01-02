namespace PaymentsTU.Document
{
	public class Utility
	{
		private struct PixelUnitFactor
		{
			public const double Pixel = 1.0;
			public const double Inch = 96.0;
			public const double Centimeter = 4800.0 / (double) sbyte.MaxValue;
			public const double Point = 1.33333333333333;
		}

		public static double CentimeterToPixel(double cm)
		{
			return cm * PixelUnitFactor.Centimeter;
		}
	}
}