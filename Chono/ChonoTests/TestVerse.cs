using System;
using System.Collections.Generic;
using Paratext.PluginInterfaces;
using SIL.Scripture;

namespace SIL.Chono
{
	internal class TestVerse : IVerseRef
	{
		public TestVerse(int book, int chapter, int verse)
		{
			BookNum = book;
			ChapterNum = chapter;
			VerseNum = verse;
			BBBCCCVVV = book * 1000000 + chapter * 1000 + verse;
		}

		public bool Equals(IVerseRef other)
		{
			throw new NotImplementedException();
		}

		public int CompareTo(IVerseRef other)
		{
			throw new NotImplementedException();
		}

		public IVerseRef ChangeVersification(IVersification newVersification)
		{
			throw new NotImplementedException();
		}

		public IVerseRef GetPreviousVerse(IProject project)
		{
			throw new NotImplementedException();
		}

		public IVerseRef GetNextVerse(IProject project)
		{
			throw new NotImplementedException();
		}

		public IVerseRef GetPreviousChapter(IProject project)
		{
			throw new NotImplementedException();
		}

		public IVerseRef GetNextChapter(IProject project)
		{
			throw new NotImplementedException();
		}

		public IVerseRef GetPreviousBook(IProject project)
		{
			throw new NotImplementedException();
		}

		public IVerseRef GetNextBook(IProject project)
		{
			throw new NotImplementedException();
		}

		public string BookCode => BCVRef.NumberToBookCode(BookNum);
		public int BookNum { get; }
		public int ChapterNum { get; }
		public int VerseNum { get; }
		public int BBBCCCVVV { get; }
		public IVersification Versification { get; }
		public bool RepresentsMultipleVerses => false;
		public IReadOnlyList<IVerseRef> AllVerses => new[] { this };
	}
}
