using System.Collections.Generic;
using System.Linq;
using Glyssen.Shared;
using Paratext.PluginInterfaces;

namespace SIL.Quotelighter
{
	internal class ParatextVerseWrapper : IVerse
	{
		private readonly IVerseRef m_verse;

		public ParatextVerseWrapper(IVerseRef verse)
		{
			m_verse = verse;
		}

		public int StartVerse => m_verse.VerseNum;
		public int EndVerse => m_verse.AllVerses.Last().VerseNum;
		public int LastVerseOfBridge => m_verse.RepresentsMultipleVerses ? EndVerse : 0;
		public IEnumerable<int> AllVerseNumbers => m_verse.AllVerses.Select(v => v.VerseNum);
	}
}
