namespace SIL.Quotelighter
{
	public static class StringExtensions
	{
		public static string UseCopyrightSymbol(this string orig) => orig.Replace("(C)", "©").Replace("(c)", "©");
	}
}
