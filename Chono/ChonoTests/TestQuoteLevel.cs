using Paratext.PluginInterfaces;

namespace SIL.Chono
{
	internal class TestQuoteLevel : IQuotationMarkLevel
	{
		public TestQuoteLevel(string opener, string closer, string continuer = null)
		{
			Opener = opener;
			Closer = closer;
			Continuer = continuer;
		}

		public string Opener { get; }
		public string Closer { get; }
		public string Continuer { get; }
	}
}
