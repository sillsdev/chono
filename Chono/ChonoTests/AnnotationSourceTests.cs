using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Paratext.PluginInterfaces;

namespace SIL.Chono
{
	[TestFixture]
	public class AnnotationSourceTests
	{
		private class TestProject : IProject
		{
			private class TestLanguage : IProjectLanguage
			{
				public TestLanguage(IQuotationMarkInfo quoteMarkInfo)
				{
					QuotationMarkInfo = quoteMarkInfo;
				}

				public bool Equals(ILanguage other)
				{
					throw new NotImplementedException();
				}

				public string Id { get; }
				public IFont Font { get; }
				public bool IsRtoL { get; }
				public string WordMatchRegex { get; }
				public IComparer<string> StringComparer { get; }
				public IQuotationMarkInfo QuotationMarkInfo { get; }
			}

			public TestProject(IQuotationMarkInfo quoteMarkInfo)
			{
				Language = new TestLanguage(quoteMarkInfo);
			}

			public bool Equals(IProject other)
			{
				throw new NotImplementedException();
			}

			public string ID { get; }
			public string ShortName { get; }
			public string LongName { get; }
			public IProjectLanguage Language { get; }
			public string LanguageName { get; }
			public ProjectType Type { get; }
			public bool IsResource { get; }
			public string NormalizeText(string text)
			{
				throw new NotImplementedException();
			}

			public string GetFormattedReference(IVerseRef reference, BookNameType nameType)
			{
				throw new NotImplementedException();
			}

			public IReadOnlyList<IScriptureTextSelection> FindMatchingScriptureSelections(IVerseRef reference, string selectedText, string verseUsfm = null, bool wholeWord = false, bool treatAsRegex = false)
			{
				throw new NotImplementedException();
			}

			public IScriptureTextSelection GetScriptureSelectionForVerse(IVerseRef reference, string verseUsfm = null)
			{
				throw new NotImplementedException();
			}

			public string GetUSX(int bookNum, bool strict = false, USXVersion version = USXVersion.Latest)
			{
				throw new NotImplementedException();
			}

			public string GetUSFM(int bookNum, int chapterNum = 0)
			{
				throw new NotImplementedException();
			}

			public string GetUSFM(int bookNum, int chapterNum, int verseNum)
			{
				throw new NotImplementedException();
			}

			public IEnumerable<IUSFMToken> GetUSFMTokens(int bookNum, int chapterNum = 0)
			{
				throw new NotImplementedException();
			}

			public IEnumerable<IUSFMToken> GetUSFMTokens(int bookNum, int chapterNum, int verseNum)
			{
				throw new NotImplementedException();
			}

			public TextReader GetPluginData(IPluginObject obj, string dataIdentifier)
			{
				throw new NotImplementedException();
			}

			public DateTime GetPluginDataModifiedTime(IPluginObject obj, string dataIdentifier)
			{
				throw new NotImplementedException();
			}

			public IReadOnlyList<IProjectNote> GetNotes(int bookNum = 0, int chapterNum = 0, bool unresolvedOnly = true)
			{
				throw new NotImplementedException();
			}

			public IProjectNote AddNote(IWriteLock writeLock, IScriptureTextSelection anchor, IEnumerable<CommentParagraph> contentParagraphs, ILanguage language = null, IUserInfo assignedUser = null)
			{
				throw new NotImplementedException();
			}

			public IWriteLock RequestWriteLock(IPluginObject obj, Action<IWriteLock> releaseRequested, int bookNum, int chapterNum = 0)
			{
				throw new NotImplementedException();
			}

			public IWriteLock RequestWriteLock(IPluginObject obj, Action<IWriteLock> releaseRequested, string pluginDataIdentifier = null)
			{
				throw new NotImplementedException();
			}

			public IWriteLock RequestWriteLock(IPluginObject obj, Action<IWriteLock> releaseRequested, WriteLockScope lockScope)
			{
				throw new NotImplementedException();
			}

			public void MakeHistoryPoint(string message, bool force = false)
			{
				throw new NotImplementedException();
			}

			public void PutUSX(IWriteLock writeLock, string usxData, bool strict = false)
			{
				throw new NotImplementedException();
			}

			public void PutUSFM(IWriteLock writeLock, string usfmData, int bookNum)
			{
				throw new NotImplementedException();
			}

			public void PutUSFMTokens(IWriteLock writeLock, IReadOnlyList<IUSFMToken> tokens, int bookNum)
			{
				throw new NotImplementedException();
			}

			public void PutPluginData(IWriteLock writeLock, IPluginObject obj, string dataIdentifier, Action<TextWriter> writeData)
			{
				throw new NotImplementedException();
			}

			public void DeletePluginData(IWriteLock writeLock, IPluginObject obj, string dataIdentifier)
			{
				throw new NotImplementedException();
			}

			public string ConvertToUSFMString(IReadOnlyList<IUSFMToken> tokens)
			{
				throw new NotImplementedException();
			}

			public bool CanEdit(IPluginObject obj, DataType dataType = DataType.PluginData)
			{
				throw new NotImplementedException();
			}

			public bool CanEdit(IPluginObject obj, int bookNum, int chapterNum = 0)
			{
				throw new NotImplementedException();
			}

			public IBiblicalTermRenderings GetBiblicalTermRenderings(IBiblicalTerm term, bool guessIfNotRendered)
			{
				throw new NotImplementedException();
			}

			public void OpenRegistrationPage()
			{
				throw new NotImplementedException();
			}

			public bool CheckForRegistration()
			{
				throw new NotImplementedException();
			}

			public bool IsRegistered { get; }
			public bool CanBeRegistered { get; }
			public IVersification Versification { get; }
			public IProject BaseProject { get; }
			public IKeyboard VernacularKeyboard { get; }
			public IKeyboard NotesKeyboard { get; }
			public IReadOnlyList<IBookInfo> AvailableBooks { get; }
			public IReadOnlyList<IUserInfo> NonObserverUsers { get; }
			public IReadOnlyList<ILanguage> AvailableNotesLanguages { get; }
			public IReadOnlyList<IMarkerInfo> ScriptureMarkerInformation { get; }
			public IReadOnlyList<IMarkerInfo> FrontBackMarkerInformation { get; }
			public USFMVersion USFMVersion { get; }
			public NormalizationType NormalizationType { get; }
			public IBiblicalTermList BiblicalTermList { get; }
			public event ScriptureDataChangedHandler ScriptureDataChanged;
			public event ProjectDataChangedHandler ProjectDataChanged;
			public event Action ProjectDeleted;
		}

		private class TestQuotationMarkInfo : IQuotationMarkInfo
		{
			public Dictionary<string, List<string>> ContinuersRequired { get; set; }

			public TestQuotationMarkInfo(List<IQuotationMarkLevel> primaryLevels,
				List<IQuotationMarkLevel> altLevels = null)
			{
				PrimaryLevels = primaryLevels;
				AlternateLevels = altLevels;
			}

			public bool IsContinuerRequired(string inParagraphStyle, string previousParagraphStyle)
			{
				if (ContinuersRequired == null)
					return true;
				return (ContinuersRequired.TryGetValue(previousParagraphStyle, out var list) &&
					list.Contains(inParagraphStyle));
			}

			public IReadOnlyList<IQuotationMarkLevel> PrimaryLevels { get; }
			public IReadOnlyList<IQuotationMarkLevel> AlternateLevels { get; }
			public bool FirstLevelCloserClosesAllLevels { get; set; }
		}

		[Test]
		public void GetAnnotations_NotQuoteLevelsDefined_ReturnsNull()
		{
			var quoteInfo = new TestQuotationMarkInfo(null);
			var sut = new AnnotationSource(null, new TestProject(quoteInfo));
			Assert.That(sut.GetAnnotations(new TestVerse(40, 6, 5), @"\c 6"), Is.Null);
		}
	}
}
