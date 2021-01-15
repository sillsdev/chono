using NUnit.Framework;

namespace SIL.Chono
{
	[TestFixture]
	class StringExtensionsTests
	{
		[TestCase("(C) 1999 By me.", ExpectedResult = "© 1999 By me.")]
		[TestCase("Missing", ExpectedResult = "Missing")]
		[TestCase("(C) 1999 By me & (C) 2021 by you.", ExpectedResult = "© 1999 By me & © 2021 by you.")]
		[TestCase("(c) oops, lowercase.", ExpectedResult = "© oops, lowercase.")]
		public string UseCopyrightSymbol_VariousTestCases_CInParenthesesReplacedWithCopyrightSymbol(string orig)
		{
			return orig.UseCopyrightSymbol();
		}
	}
}
