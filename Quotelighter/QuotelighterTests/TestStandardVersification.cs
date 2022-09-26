using System;
using Paratext.PluginInterfaces;
using SIL.Scripture;

namespace SIL.Quotelighter
{
	internal class TestStandardVersification : IVersification
	{
		public TestStandardVersification(ScrVersType type = ScrVersType.English)
		{
			switch (type)
			{
				case ScrVersType.Unknown:
					throw new NotImplementedException();
				case ScrVersType.Original:
					Type = StandardScrVersType.Original;
					break;
				case ScrVersType.Septuagint:
					Type = StandardScrVersType.Septuagint;
					break;
				case ScrVersType.Vulgate:
					Type = StandardScrVersType.Vulgate;
					break;
				case ScrVersType.English:
					Type = StandardScrVersType.English;
					break;
				case ScrVersType.RussianProtestant:
					Type = StandardScrVersType.RussianProtestant;
					break;
				case ScrVersType.RussianOrthodox:
					Type = StandardScrVersType.RussianOrthodox;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		public bool Equals(IVersification other)
		{
			return other != null && other.Type == Type && !other.IsCustomized;
		}

		public int GetLastVerse(int book, int chapter)
		{
			return ScrVers.English.GetLastVerse(book, chapter);
		}

		public int GetLastChapter(int bookNum)
		{
			return ScrVers.English.GetLastChapter(bookNum);
		}

		public IVerseRef CreateReference(int bookNum, int chapterNum, int verseNum)
		{
			return new TestVerse(bookNum, chapterNum, verseNum);
		}

		public IVerseRef CreateReference(string refStr)
		{
			throw new NotImplementedException();
		}

		public IVerseRef ChangeVersification(IVerseRef verseRef)
		{
			throw new NotImplementedException();
		}

		public bool IsExcluded(int bbbcccvvv) => false;

		public StandardScrVersType Type { get; }
		public bool IsCustomized => false;
	}

}
