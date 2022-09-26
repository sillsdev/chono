using System;
using System.Collections.Generic;
using NUnit.Framework;
using Paratext.PluginInterfaces;
using SIL.Scripture;

namespace SIL.Quotelighter
{
	internal class TestVerse : IVerseRef
	{
		public TestVerse(int book, int chapter, int verse, IVersification versification = null)
		{
			BookNum = book;
			ChapterNum = chapter;
			VerseNum = verse;
			BBBCCCVVV = book * 1000000 + chapter * 1000 + verse;
			Versification = versification;
		}

		public bool Equals(IVerseRef other)
		{
			return BBBCCCVVV.Equals(other.BBBCCCVVV) &&
				Versification == other.Versification;
		}

		public int CompareTo(IVerseRef other)
		{
			return BBBCCCVVV.CompareTo(other.BBBCCCVVV);
		}

		public IVerseRef ChangeVersification(IVersification newVersification)
		{
			throw new NotImplementedException();
		}

		public IVerseRef GetPreviousVerse(IProject project)
		{
			Assert.That(VerseNum, Is.GreaterThanOrEqualTo(1));
			return new TestVerse(BookNum, ChapterNum, VerseNum - 1, Versification);
		}

		public IVerseRef GetNextVerse(IProject project)
		{
			return new TestVerse(BookNum, ChapterNum, VerseNum + 1, Versification);
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

		public override string ToString() => $"{BookCode} {ChapterNum}:{VerseNum}";
	}
}
