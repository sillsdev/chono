using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Paratext.PluginInterfaces;
using SIL.Extensions;
using static System.Environment;

namespace SIL.Chono
{
	[TestFixture]
	public class AnnotationSourceTests
	{
		public enum Continuers
		{
			RepeatAllLevels,
			SameAsOpener,
			SameAsCloser,
			None
		}

		private class TestProject : IProject
		{
			internal const string kJer1V5Line1 = "“Before I formed you in the womb I knew you, ";
			internal const string kJer1V5Line2 = "before you were born I set you apart; ";
			internal const string kJer1V5Line3 = "I appointed you as a prophet to the nations.”";
			
			internal const string kJer1V7BeforeQuote = "But the Lord said to me, ";
			internal const string kJer1V7Lev1QuotePart1 = "“Do not say, ";
			internal const string kJer1V7Lev2Quote = "‘I am too young.’";
			internal const string kJer1V7Lev1QuotePart2 = " You must go to everyone I send you to and say whatever I command you. ";

			internal const string kJer1V8Lev1Quote = "Do not be afraid of them, for I am with you and will rescue you,”";
			internal const string kJer1V8AfterQuote = " declares the Lord.";

			internal const string kMat5V4Text = "Blessed are those who mourn, for they shall be comforted.";

			internal const string kJohn4V9BeforeQuote = "The lady replied, ";
			internal const string kJohn4V9Quote = "“You are a Jew and I am a Samaritan woman. How can you ask me for water?”";
			internal const string kJohn4V9AfterQuote = " (For Jews avoid Samaritans.)";

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

			private class TestScriptureSelection : IScriptureTextSelection
			{
				public TestScriptureSelection(IVerseRef verseRefStart, string selectedText,
					string beforeContext, string afterContext, int additionalOffset = 0,
					IVerseRef verseRefEnd = null)
				{
					VerseRefStart = verseRefStart;
					VerseRefEnd = verseRefEnd ?? verseRefStart;
					Offset = beforeContext.Length + additionalOffset;
					SelectedText = selectedText;
					BeforeContext = beforeContext;
					AfterContext = afterContext;
				}

				public bool Equals(ISelection other)
				{
					return (other is IScriptureTextSelection s &&
						VerseRefStart.Equals(s.VerseRefStart) &&
						VerseRefEnd.Equals(s.VerseRefEnd) &&
						Offset.Equals(s.Offset) &&
						BeforeContext.Equals(s.BeforeContext) &&
						AfterContext.Equals(s.AfterContext));
				}

				public IVerseRef VerseRefStart { get; }
				public IVerseRef VerseRefEnd { get; }
				public string SelectedText { get; }
				public int Offset { get; }
				public string BeforeContext { get; }
				public string AfterContext { get; }
			}
			
			private abstract class TestTokenBase : IUSFMToken
			{
				protected TestTokenBase(IVerseRef verseRef, int offset = 0)
				{
					VerseRef = verseRef;
					VerseOffset = offset;
				}

				public IVerseRef VerseRef { get; }
				public int VerseOffset { get; }
				public virtual bool IsSpecial => false;
				public virtual bool IsFigure => false;
				public virtual bool IsFootnoteOrCrossReference => false;
				public virtual bool IsScripture => true;
				public virtual bool IsMetadata => false;
				public virtual bool IsPublishableVernacular => true;

				public abstract int EndVerseOffset { get; }
			}

			private class TestTextToken : TestTokenBase, IUSFMTextToken
			{
				public TestTextToken(IVerseRef verseRef, string text, int offset = 0) :
					base(verseRef, offset)
				{
					Text = text;
				}

				public string Text { get; }
				public override int EndVerseOffset => VerseOffset + Text.Length;
			}

			private class TestMarkerToken : TestTokenBase, IUSFMMarkerToken
			{
				public TestMarkerToken(IVerseRef verseRef, string marker = "p", string data = null, int offset = 0) :
					base(verseRef, offset)
				{
					if (marker == "v")
						Debug.Assert(offset == 0);
					Marker = marker;
					Data = data;
				}

				public MarkerType Type
				{
					get
					{
						switch (Marker)
						{
							case "id":
								return MarkerType.Book;
							case "c":
								return MarkerType.Chapter;
							case "v":
								return MarkerType.Verse;
							default:
								return MarkerType.Paragraph;
						}
					}
				}
				public string Marker { get; }
				public IEnumerable<IUSFMAttribute> Attributes => null;
				public string Data { get; }
				public string EndMarker => null;
				public override int EndVerseOffset => VerseOffset + @"\".Length + Marker.Length +
					" ".Length + (Data == null ? 0 : Data.Length + " ".Length);
			}

			public TestProject(IQuotationMarkInfo quoteMarkInfo)
			{
				Language = new TestLanguage(quoteMarkInfo);
				Versification = new TestStandardVersification();
			}

			public bool Equals(IProject other)
			{
				throw new NotImplementedException();
			}

			public string ID => TestContext.CurrentContext.WorkerId;
			public string ShortName => "Test";
			public string LongName => "Current test";
			public IProjectLanguage Language { get; }
			public string LanguageName => "Test tok";
			public ProjectType Type => ProjectType.Standard;
			public bool IsResource => false;
			public string NormalizeText(string text)
			{
				throw new NotImplementedException();
			}

			public string GetFormattedReference(IVerseRef reference, BookNameType nameType)
			{
				throw new NotImplementedException();
			}

			public IReadOnlyList<IScriptureTextSelection> FindMatchingScriptureSelections(
				IVerseRef reference, string selectedText, string verseUsfm = null,
				bool wholeWord = false, bool treatAsRegex = false)
			{
				if (selectedText == "")
					throw new Exception($"ERROR: The plugin {ChonoPlugin.kPluginName} created a " +
						"zero-length annotation without an icon.");

				IScriptureTextSelection sel = null;
				var v = @"\v " + $"{reference.VerseNum} ";
				switch (reference.BBBCCCVVV)
				{
					case 24001005:
						if (selectedText == kJer1V5Line1)
							sel = new TestScriptureSelection(reference, selectedText, v, "");
						else if (selectedText == kJer1V5Line2)
						{
							sel = new TestScriptureSelection(reference, selectedText, @"\q ", "",
								(v + kJer1V5Line1).Length);
						}
						else if (selectedText == kJer1V5Line3)
						{
							sel = new TestScriptureSelection(reference, selectedText, @"\q ", " ",
								(v + kJer1V5Line1 + @"\q " + kJer1V5Line2).Length);
						}
						break;
					case 24001007:
						if (selectedText == kJer1V7Lev1QuotePart1)
						{
							sel = new TestScriptureSelection(reference, selectedText,
								v + kJer1V7BeforeQuote, kJer1V7Lev2Quote + kJer1V7Lev1QuotePart2);
						}
						else if (selectedText == kJer1V7Lev2Quote)
						{
							sel = new TestScriptureSelection(reference, selectedText,
								v + kJer1V7BeforeQuote + kJer1V7Lev1QuotePart1, kJer1V7Lev1QuotePart2);
						}
						else if (selectedText == kJer1V7Lev1QuotePart2)
						{
							sel = new TestScriptureSelection(reference, selectedText,
								v + kJer1V7BeforeQuote + kJer1V7Lev1QuotePart1 + kJer1V7Lev2Quote, "");
						}
						break;
					case 24001008:
						if (selectedText == "8") // REVIEW: If we want to create verse-number annotations, is this what the text should be?
							sel = new TestScriptureSelection(reference, selectedText, "", "");
						else if (selectedText == kJer1V8Lev1Quote)
						{
							sel = new TestScriptureSelection(reference, selectedText,
								v, kJer1V8AfterQuote);
						}
						break;
					case 40005004:
						if (selectedText == "4") // REVIEW: If we want to create verse-number annotations, is this what the text should be?
							sel = new TestScriptureSelection(reference, selectedText, "", "");
						else if (selectedText.EndsWith(kMat5V4Text))
							sel = new TestScriptureSelection(reference, selectedText, v, "");
						break;
					case 43004009:
						if (selectedText == kJohn4V9Quote)
						{
							sel = new TestScriptureSelection(reference, selectedText,
								v + kJohn4V9BeforeQuote, kJohn4V9AfterQuote);
						}
						break;
				}

				if (sel == null)
					sel = new TestScriptureSelection(reference, selectedText, "NOT FOUND", "NOT FOUND");
				return new [] {sel}.ToReadOnlyList();
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
				IVerseRef verse;
				var versification = new TestStandardVersification();
				switch (bookNum)
				{
					case 24: // JER
						switch (chapterNum)
						{
							case 0:
							case 1:
								verse = new TestVerse(bookNum, 1, 0, versification);
								yield return new TestMarkerToken(verse, "id", "JHN");
								yield return new TestMarkerToken(verse, "c", "1");
								foreach (var tok in GetVerses(verse,
									         "Jeremiah was one of the priests in the territory of Benjamin. ",
									         "Lord spoke to him during the reign of Josiah king of Judah, ",
									         "until the 5th month of the 11th year of Zedekiah, when Jerusalem was exiled. ",
									         NewLine,
									         "The word of the Lord came to me, saying, ",
									         NewLine))
									yield return tok;
								verse = new TestVerse(bookNum, 1, 5, versification);
								foreach (var tok in GetPoetryLinesFor(verse,
									         "“Before I formed you in the womb I knew you, ",
									         "before you were born I set you apart; ",
									         "I appointed you as a prophet to the nations.” "))
									yield return tok;
								foreach (var tok in GetVerses(verse,
									         NewLine,
									         "“Oh, Sovereign Lord,” I said, “I do not know how to speak; I am young.”",
									         NewLine,
									         kJer1V7BeforeQuote + kJer1V7Lev1QuotePart1 + kJer1V7Lev2Quote + kJer1V7Lev1QuotePart2,
									         kJer1V8Lev1Quote + kJer1V8AfterQuote))
									yield return tok;
								break;
							default:
								yield break;
						}
						break;

					case 40: // MAT
						switch (chapterNum)
						{
							case 0:
							case 1:
								verse = new TestVerse(bookNum, 1, 0, versification);
								yield return new TestMarkerToken(verse, "id", "MAT");
								yield return new TestMarkerToken(verse, "c", "1");
								if (chapterNum == 0)
									goto case 2;
								break;
							case 2:
								verse = new TestVerse(bookNum, 2, 0, versification);
								yield return new TestMarkerToken(verse, "c", "2");
								if (chapterNum == 0)
									goto case 3;
								break;
							case 3:
								verse = new TestVerse(bookNum, 3, 0, versification);
								yield return new TestMarkerToken(verse, "c", "3");
								if (chapterNum == 0)
									goto case 4;
								break;
							case 4:
								verse = new TestVerse(bookNum, 4, 0, versification);
								yield return new TestMarkerToken(verse, "c", "4");
								if (chapterNum == 0)
									goto case 5;
								break;
							case 5:
								verse = new TestVerse(bookNum, 5, 0, versification);
								yield return new TestMarkerToken(verse, "c", "5");
								foreach (var tok in GetVerses(verse,
									         "Seeing the crowds, he sat down and his disciples came to him. ",
									         NewLine,
									         "And he opened his mouth and taught them, saying: ",
									         NewLine,
									         "“Blessed are the poor in spirit, for theirs is the kingdom of heaven. ",
									         NewLine,
									         Language.QuotationMarkInfo.PrimaryLevels[0].Continuer + kMat5V4Text))
									yield return tok;
								break;
						}
						break;
					case 43: // JHN
						switch (chapterNum)
						{
							case 0:
							case 1:
								verse = new TestVerse(bookNum, 1, 0, versification);
								yield return new TestMarkerToken(verse, "id", "JHN");
								yield return new TestMarkerToken(verse, "c", "1");
								if (chapterNum == 0)
									goto case 2;
								break;
							case 2:
								verse = new TestVerse(bookNum, 2, 0, versification);
								yield return new TestMarkerToken(verse, "c", "2");
								if (chapterNum == 0)
									goto case 3;
								break;
							case 3:
								verse = new TestVerse(bookNum, 3, 0, versification);
								yield return new TestMarkerToken(verse, "c", "3");
								if (chapterNum == 0)
									goto case 4;
								break;
							case 4:
								verse = new TestVerse(bookNum, 4, 0, versification);
								yield return new TestMarkerToken(verse, "c", "4");
								foreach (var tok in GetVerses(verse,
									         "Now Jesus learned that the Pharisees knew he was baptizing more disciples than John— ",
									         "although in fact it was not Jesus who baptized, but his disciples. ",
									         "So he left Judea and went back once more to Galilee. ",
									         NewLine,
									         "Now he had to go through Samaria. ",
									         "They came to a town near the ground Jacob had given to Joseph. ",
									         "Jesus, tired from the journey, sat down by the well around noon. ",
									         NewLine,
									         "A Samaritan lady came for water and Jesus said to her, “Will you give me a drink?” ",
									         "(His disciples had gone into the town to buy food.) ",
									         NewLine,
									         kJohn4V9BeforeQuote + kJohn4V9Quote + kJohn4V9AfterQuote))
									yield return tok;
								break;
							default:
								yield break;
						}
						break;
					default:
						yield break;
				}
			}

			private IEnumerable<IUSFMToken> GetVerses(IVerseRef verse, params string[] verseTextsAndParagraphBreaks)
			{
				TestTokenBase tok = null;
				foreach (var t in verseTextsAndParagraphBreaks)
				{
					if (t == NewLine)
					{
						tok = new TestMarkerToken(verse, offset:tok?.EndVerseOffset ?? 0);
						yield return tok;
						continue;
					}
					verse = verse.GetNextVerse(this);
					tok = new TestMarkerToken(verse, "v", verse.VerseNum.ToString());
					yield return tok;
					tok = new TestTextToken(verse, t, tok.EndVerseOffset);
					yield return tok;
				}
			}

			private IEnumerable<IUSFMToken> GetPoetryLinesFor(IVerseRef verse, params string[] poetryLines)
			{
				TestTokenBase tok = null;
				tok = new TestMarkerToken(verse, "v", verse.VerseNum.ToString());
				yield return tok;
				foreach (var t in poetryLines)
				{
					tok = new TestTextToken(verse, t, tok.EndVerseOffset);
					yield return tok;

					tok = new TestMarkerToken(verse, "q", offset:tok.EndVerseOffset);
					yield return tok;
				}
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

			public TestQuotationMarkInfo(IEnumerable<IQuotationMarkLevel> primaryLevels,
				IEnumerable<IQuotationMarkLevel> altLevels = null)
			{
				PrimaryLevels = primaryLevels?.ToList();
				AlternateLevels = altLevels?.ToList();
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
		public void GetAnnotations_NoQuoteLevelsDefined_ReturnsNull()
		{
			var quoteInfo = new TestQuotationMarkInfo(null);
			var sut = new AnnotationSource(null, new TestProject(quoteInfo));
			Assert.That(sut.GetAnnotations(new TestVerse(40, 6, 5), @"\v 5 This is test data."),
				Is.Null);
		}

		[Test]
		public void GetAnnotations_SinglePrimaryQuoteLevelDefined_ReturnsFirstLevelAnnotation()
		{
			var quoteInfo = GetQuoteInfo(1);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			var verse = new TestVerse(43, 4, 9);
			var annotation = sut.GetAnnotations(verse, @"\v 9 " +
				TestProject.kJohn4V9BeforeQuote + TestProject.kJohn4V9Quote +
				TestProject.kJohn4V9AfterQuote).Single();

			Assert.That(annotation.StyleName, Is.EqualTo("quote1"));
			Assert.That(annotation.ScriptureSelection.SelectedText, 
				Is.EqualTo(TestProject.kJohn4V9Quote));
			VerifyPluginAnnotation(annotation, project, verse);
		}

		[TestCase(1)]
		[TestCase(2)]
		public void GetAnnotations_ParagraphBreaksInsideQuote_ReturnsAnnotationsToCoverQuote(
			int quoteLevels)
		{
			var quoteInfo = GetQuoteInfo(quoteLevels);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			var verse = new TestVerse(24, 1, 5);
			var annotations = sut.GetAnnotations(verse, @"\v 5 " +
				TestProject.kJer1V5Line1 + @"\q " + TestProject.kJer1V5Line2 +
				@"\q " + TestProject.kJer1V5Line3);
			Assert.That(annotations.Count, Is.EqualTo(3));
			
			Assert.That(annotations[0].ScriptureSelection.SelectedText, 
				Is.EqualTo(TestProject.kJer1V5Line1));
			Assert.That(annotations[1].ScriptureSelection.SelectedText, 
				Is.EqualTo(TestProject.kJer1V5Line2));
			Assert.That(annotations[2].ScriptureSelection.SelectedText, 
				Is.EqualTo(TestProject.kJer1V5Line3));

			foreach (var annotation in annotations)
			{
				Assert.That(annotation.StyleName, Is.EqualTo("quote1"));
				VerifyPluginAnnotation(annotation, project, verse);
			}
		}

		[TestCase(2)]
		[TestCase(3)]
		[TestCase(2, Continuers.SameAsOpener)]
		[TestCase(2, Continuers.SameAsCloser)]
		[TestCase(2, Continuers.None)]
		[TestCase(3, Continuers.SameAsOpener)]
		[TestCase(3, Continuers.SameAsCloser)]
		[TestCase(3, Continuers.None)]
		public void GetAnnotations_MultiplePrimaryQuoteLevelsDefined_ReturnsLevel1And2Annotations(
			int quoteLevels, Continuers continuers = Continuers.RepeatAllLevels)
		{
			var quoteInfo = GetQuoteInfo(quoteLevels, continuers);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			var verse = new TestVerse(24, 1, 7);
			var annotations = sut.GetAnnotations(verse,
				@"\v 7 " + TestProject.kJer1V7BeforeQuote + TestProject.kJer1V7Lev1QuotePart1 +
				TestProject.kJer1V7Lev2Quote + TestProject.kJer1V7Lev1QuotePart2);

			Assert.That(annotations.Count, Is.EqualTo(3));
			Assert.That(annotations[0].ScriptureSelection.SelectedText,
				Is.EqualTo(TestProject.kJer1V7Lev1QuotePart1));
			Assert.That(annotations[0].StyleName, Is.EqualTo("quote1"));
			Assert.That(annotations[1].ScriptureSelection.SelectedText,
				Is.EqualTo(TestProject.kJer1V7Lev2Quote));
			Assert.That(annotations[1].StyleName, Is.EqualTo("quote2"));
			Assert.That(annotations[2].ScriptureSelection.SelectedText,
				Is.EqualTo(TestProject.kJer1V7Lev1QuotePart2));
			Assert.That(annotations[2].StyleName, Is.EqualTo("quote1"));
			foreach (var annotation in annotations)
				VerifyPluginAnnotation(annotation, project, verse);
		}

		[TestCase(1)]
		[TestCase(2)]
		[TestCase(2, Continuers.SameAsOpener)]
		[TestCase(2, Continuers.SameAsCloser)]
		[TestCase(2, Continuers.None)]
		public void GetAnnotations_VerseStartsAtQuoteLevel1_ReturnsAnnotation(int quoteLevels,
			Continuers continuers = Continuers.RepeatAllLevels)
		{
			var quoteInfo = GetQuoteInfo(quoteLevels, continuers);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			var verse = new TestVerse(24, 1, 8);
			var annotations = sut.GetAnnotations(verse,
				@"\v 8 " + TestProject.kJer1V8Lev1Quote + TestProject.kJer1V8AfterQuote);

			// The first annotation is for the verse number itself (even though Paratext may not
			// highlight it correctly).
			Assert.That(annotations.Count, Is.EqualTo(2));
			Assert.That(annotations[0].ScriptureSelection.Offset, Is.EqualTo(0));
			Assert.That(annotations[1].ScriptureSelection.SelectedText,
				Is.EqualTo(TestProject.kJer1V8Lev1Quote));

			foreach (var annotation in annotations)
			{
				VerifyPluginAnnotation(annotation, project, verse);
				Assert.That(annotation.StyleName, Is.EqualTo("quote1"));
			}
		}

		[TestCase(Continuers.RepeatAllLevels)]
		[TestCase(Continuers.SameAsOpener)]
		[TestCase(Continuers.SameAsCloser)]
		[TestCase(Continuers.None)]
		public void GetAnnotations_Lev1ContinuerAtStartOfVerse_ReturnsAnnotationsForContPara(
			Continuers continuers)
		{
			var quoteInfo = GetQuoteInfo(1, continuers);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			var verse = new TestVerse(40, 5, 4);
			var annotations = sut.GetAnnotations(verse,
				@"\v 4 " + quoteInfo.PrimaryLevels[0].Continuer + TestProject.kMat5V4Text);

			// The first annotation is for the verse number itself (even though Paratext may not
			// highlight it correctly).
			Assert.That(annotations.Count, Is.EqualTo(2));
			Assert.That(annotations[0].ScriptureSelection.Offset, Is.EqualTo(0));
			Assert.That(annotations[1].ScriptureSelection.SelectedText,
				Is.EqualTo(quoteInfo.PrimaryLevels[0].Continuer + TestProject.kMat5V4Text));

			foreach (var annotation in annotations)
			{
				VerifyPluginAnnotation(annotation, project, verse);
				Assert.That(annotation.StyleName, Is.EqualTo("quote1"));
			}
		}

		[TestCase(Continuers.RepeatAllLevels)]
		[TestCase(Continuers.SameAsOpener)]
		[TestCase(Continuers.SameAsCloser)]
		[TestCase(Continuers.None)]
		public void GetAnnotations_Lev2Continuer_ReturnsAnnotationsForContPara(
			Continuers continuers)
		{
			var quoteInfo = GetQuoteInfo(2, continuers);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			Assert.Fail("Write this test");
		}

		[TestCase(Continuers.RepeatAllLevels)]
		[TestCase(Continuers.SameAsOpener)]
		[TestCase(Continuers.SameAsCloser)]
		[TestCase(Continuers.None)]
		public void GetAnnotations_Lev3Continuer_ReturnsAnnotationsForContPara(
			Continuers continuers)
		{
			var quoteInfo = GetQuoteInfo(3, continuers);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			Assert.Fail("Write this test");
		}

		private TestQuotationMarkInfo GetQuoteInfo(int quoteLevels, Continuers continuers = Continuers.RepeatAllLevels)
		{
			var primaryLevels = new TestQuoteLevel[quoteLevels];
			primaryLevels[0] = new TestQuoteLevel("“", "”", "“");
			if (quoteLevels > 1)
				primaryLevels[1] = new TestQuoteLevel("‘", "’", GetContinuer(1, continuers));
			if (quoteLevels > 2)
				primaryLevels[2] = new TestQuoteLevel("“", "”", GetContinuer(2, continuers));

			return new TestQuotationMarkInfo(primaryLevels);
		}

		private string GetContinuer(int level, Continuers continuers)
		{
			switch (level)
			{
				case 1:
					switch (continuers)
					{
						case Continuers.RepeatAllLevels: return "“ ‘";
						case Continuers.SameAsOpener: return "‘";
						case Continuers.SameAsCloser: return "’";
					}
					break;
					
				case 2:
					switch (continuers)
					{
						case Continuers.RepeatAllLevels: return "“ ‘ “";
						case Continuers.SameAsOpener: return "“";
						case Continuers.SameAsCloser: return "”";
					}
					break;
			}
			return "";
		}

		private static void VerifyPluginAnnotation(IPluginAnnotation annotation, IProject project,
			TestVerse verse)
		{
			Assert.That(annotation.Icon, Is.Null);
			Assert.That(annotation.IconToolTipText, Is.Null);
			var arbitraryPoint = new Point(3, 0);
			Assert.That(annotation.Click(MouseButton.Left, false, arbitraryPoint), Is.False);

			var scrSel = annotation.ScriptureSelection;
			var matchingSelFromProject = project.FindMatchingScriptureSelections(
				scrSel.VerseRefStart, scrSel.SelectedText).Single();
			Assert.That(scrSel.SelectedText, Is.EqualTo(matchingSelFromProject.SelectedText));
			Assert.That(matchingSelFromProject.BeforeContext, Does.EndWith(scrSel.BeforeContext));
			Assert.That(scrSel.AfterContext, Is.EqualTo(matchingSelFromProject.AfterContext));
			Assert.That(scrSel.Offset, Is.EqualTo(matchingSelFromProject.Offset));
			Assert.That(scrSel.VerseRefStart.BBBCCCVVV, Is.EqualTo(verse.BBBCCCVVV));

			if (!verse.RepresentsMultipleVerses)
			{
				Assert.That(scrSel.VerseRefEnd, Is.EqualTo(scrSel.VerseRefStart));
			}
		}
	}
}
