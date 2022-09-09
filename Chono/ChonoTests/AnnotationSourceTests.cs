using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Paratext.PluginInterfaces;
using SIL.Extensions;
using static System.Char;
using static System.Environment;
using static SIL.Chono.AnnotationSourceTests;

namespace SIL.Chono
{
	internal interface ITestToken
	{
		int EndVerseOffset { get; }
	}

	[TestFixture]
	public class AnnotationSourceTests
	{
		public enum Continuers
		{
			RepeatAllOpeners,
			RepeatAllClosers,
			// Continuers (at each level) are different from either the openers or the closers
			DifferentPunct,
			// SameAsOpener, Paratext UI does not make this possible
			// SameAsCloser, Paratext UI does not make this possible
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

			internal const string kMat6SectHead1To4 = "Beware of “showing off";
			internal const string kMat7SectHead21To23 = "What it means to have Jesus as Lord";
			internal const string kMat7SectHead24To27 = "Jesus says: “Build Your House on the Rock”";
			internal const string kMat7V23Lev1Quote = "Then I’ll tell them, ";
			internal const string kMat7V23Lev2Quote = "‘I never knew you. Leave, you creeps!’ ";
			internal const string kMat7V24SansCont = "Everyone who heeds my words will be like a wise man who built on the rock. ";

			internal const string kMat22V44Line1SansLev1Cont = "‘The Lord said to my Lord, ";
			internal const string kMat22V44Line2SansLev12Cont = "“Sit at my right hand, ";
			internal const string kMat22V44Line3SansConts = "until I put your enemies under your feet”’";
			internal const string kMat22V44Line3AfterQuote = "? ";

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
			
			private abstract class TestTokenBase : IUSFMToken, ITestToken
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
				private bool _isScripture;

				public TestTextToken(IVerseRef verseRef, string text, int offset = 0, bool isScripure = true) :
					base(verseRef, offset)
				{
					Text = text;
					_isScripture = isScripure;
				}

				public string Text { get; }
				public override bool IsScripture => _isScripture;
				public override int EndVerseOffset => VerseOffset + Text.Length;
			}

			private class TestMarkerToken : TestTokenBase, IUSFMMarkerToken
			{
				public TestMarkerToken(IVerseRef verseRef, string marker, string data = null, int offset = 0) :
					base(verseRef, offset)
				{
					if (marker == "v")
						Debug.Assert(offset == 0);
					Marker = marker;
					Data = data;
				}

				public TestMarkerToken(TestTokenBase prevToken, string marker) : 
					this(prevToken.VerseRef, marker, offset:prevToken.EndVerseOffset)
				{
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

			/// <summary>
			///  This gets the "expected results"
			/// </summary>
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
						if (selectedText == @"\v 8 ")
							sel = new TestScriptureSelection(reference, selectedText, "", "", 1);
						else if (selectedText == kJer1V8Lev1Quote)
						{
							sel = new TestScriptureSelection(reference, selectedText,
								v, kJer1V8AfterQuote);
						}
						break;
					case 40005004:
						if (selectedText == @"\v 4 ")
							sel = new TestScriptureSelection(reference, selectedText, "", "", 1);
						else if (selectedText.EndsWith(kMat5V4Text))
							sel = new TestScriptureSelection(reference, selectedText, v, "");
						break;
					case 40022044:
						if (selectedText == @"\v 44 ")
							sel = new TestScriptureSelection(reference, selectedText, "", "", 1);
						else if (selectedText.EndsWith(kMat22V44Line1SansLev1Cont))
							sel = new TestScriptureSelection(reference, selectedText, v, "");
						else
						{
							var offsetToLine2Start = (v + GetCont(1, "p", "q1") + kMat22V44Line1SansLev1Cont +
								@"\q1 ").Length;

							if (selectedText.EndsWith(kMat22V44Line2SansLev12Cont))
							{
								sel = new TestScriptureSelection(reference, selectedText, "", "",
									offsetToLine2Start);
							}
							else
							{
								var offsetToLine3Start = offsetToLine2Start +
									(GetCont(2, "p", "q1") + kMat22V44Line2SansLev12Cont + @"\q3 ").Length;

								if (selectedText.EndsWith(kMat22V44Line3SansConts))
								{
									sel = new TestScriptureSelection(reference, selectedText, "",
										kMat22V44Line3AfterQuote, offsetToLine3Start);
								}
								else if (selectedText == kMat22V44Line3AfterQuote)
								{
									sel = new TestScriptureSelection(reference, selectedText,
										GetCont(3, "q1", "q2") + kMat22V44Line3SansConts, "",
										offsetToLine3Start);
								}
							}
						}

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
				TestTokenBase lastTok = null; // Defined here for convenience. Used in several cases.
				switch (bookNum)
				{
					case 24: // JER
						switch (chapterNum)
						{
							case 0:
							case 1:
								verse = new TestVerse(bookNum, 1, 0, versification);
								lastTok = new TestMarkerToken(verse, "id", "JHN");
								yield return lastTok;
								yield return new TestMarkerToken(verse, "c", "1", lastTok.EndVerseOffset);
								foreach (var tok in GetVerses(verse,
									         "Jeremiah was one of the priests in the territory of Benjamin. ",
									         "Lord spoke to him during the reign of Josiah king of Judah, ",
									         "until the 5th month of the 11th year of Zedekiah, when Jerusalem was exiled. ",
									         NewLine,
									         "The word of the Lord came to me, saying, "))
								{
									lastTok = tok;
									yield return tok;
								}

								verse = new TestVerse(bookNum, 1, 5, versification);
								foreach (var tok in GetPoetryLinesFor(verse, lastTok.EndVerseOffset,
									         kJer1V5Line1,
									         kJer1V5Line2,
									         kJer1V5Line3 + " "))
								{
									lastTok = tok;
									yield return tok;
								}

								yield return new TestMarkerToken(lastTok, "p");
								foreach (var tok in GetVerses(verse,
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
								lastTok = new TestMarkerToken(verse, "id", "MAT");
								yield return lastTok;
								yield return new TestMarkerToken(verse, "c", "1", lastTok.EndVerseOffset);
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
								if (chapterNum == 0)
									goto case 6;
								break;
							case 6:
								verse = new TestVerse(bookNum, 6, 0, versification);
								lastTok = new TestMarkerToken(verse, "c", "6");
								yield return lastTok; 
								lastTok = new TestMarkerToken(verse, "s", offset:lastTok.EndVerseOffset);
								yield return lastTok;
								lastTok = new TestTextToken(verse, kMat6SectHead1To4, lastTok.EndVerseOffset, false);
								yield return lastTok;
								yield return new TestMarkerToken(lastTok, "p");
								foreach (var tok in GetVerses(verse,
									         Language.QuotationMarkInfo.PrimaryLevels.FirstOrDefault()?.Continuer +
									         "Beware of showing off your righteousness, unless you want no reward from God. "))
									yield return tok;
								if (chapterNum == 0)
									goto case 7;
								break;
							case 7:
								verse = new TestVerse(bookNum, 7, 0, versification);
								lastTok = new TestMarkerToken(verse, "c", "7");
								yield return lastTok;
								yield return new TestMarkerToken(lastTok, "p");
								foreach (var tok in GetVerses(verse,
									         Language.QuotationMarkInfo.PrimaryLevels.FirstOrDefault()?.Continuer +
									         "Judge not, that you be not judged. "))
								{
									lastTok = tok;
									yield return tok;
								}

								yield return new TestMarkerToken(lastTok, "p");
								verse = new TestVerse(bookNum, 7, 20, versification);
								lastTok = new TestMarkerToken(verse, "v", "20");
								yield return lastTok;
								lastTok= new TestTextToken(verse,
									"So you will know who they are by what comes out. ",
									lastTok.EndVerseOffset);
								yield return lastTok;
								lastTok = new TestMarkerToken(lastTok, "s");
								yield return lastTok;
								lastTok = new TestTextToken(verse, kMat7SectHead21To23, lastTok.EndVerseOffset, false);
								yield return lastTok;
								yield return new TestMarkerToken(lastTok, "p");
								foreach (var tok in GetVerses(verse,
									         "“Not all who say, ‘Lord, Lord,’ will enter heaven, just those who do God’s will. ",
									         "Many will say, ‘Lord, Lord, did we not do our work in your name?’ ",
									         kMat7V23Lev1Quote + kMat7V23Lev2Quote))
								{
									lastTok = tok;
									yield return tok;
								}

								lastTok = new TestMarkerToken(lastTok, "s");
								yield return lastTok;
								lastTok = new TestTextToken(verse, kMat7SectHead24To27, lastTok.EndVerseOffset, false);
								yield return lastTok;
								yield return new TestMarkerToken(lastTok, "p");
								foreach (var tok in GetVerses(verse,
									         Language.QuotationMarkInfo.PrimaryLevels.FirstOrDefault()?.Continuer +
									         kMat7V24SansCont,
											 "A big storm could not topple it because it was founded on rock. ",
											 "Disregarding my words makes you like a fool who built on sand. ",
									         "It got wet and went splat!” ",
											 NewLine,
									         "When people heard Jesus, they knew he was the real deal."))
									yield return tok;
								if (chapterNum == 0)
									goto case 8;
								break;
							case 8:
								verse = new TestVerse(bookNum, 8, 0, versification);
								yield return new TestMarkerToken(verse, "c", "8");
								if (chapterNum == 0)
									goto case 9;
								break;
							case 9:
								verse = new TestVerse(bookNum, 9, 0, versification);
								yield return new TestMarkerToken(verse, "c", "9");
								if (chapterNum == 0)
									goto case 10;
								break;
							case 10:
								verse = new TestVerse(bookNum, 10, 0, versification);
								yield return new TestMarkerToken(verse, "c", "10");
								if (chapterNum == 0)
									goto case 11;
								break;
							case 11:
								verse = new TestVerse(bookNum, 11, 0, versification);
								yield return new TestMarkerToken(verse, "c", "11");
								if (chapterNum == 0)
									goto case 12;
								break;
							case 12:
								verse = new TestVerse(bookNum, 12, 0, versification);
								yield return new TestMarkerToken(verse, "c", "12");
								if (chapterNum == 0)
									goto case 13;
								break;
							case 13:
								verse = new TestVerse(bookNum, 13, 0, versification);
								yield return new TestMarkerToken(verse, "c", "13");
								if (chapterNum == 0)
									goto case 14;
								break;
							case 14:
								verse = new TestVerse(bookNum, 14, 0, versification);
								yield return new TestMarkerToken(verse, "c", "14");
								if (chapterNum == 0)
									goto case 15;
								break;
							case 15:
								verse = new TestVerse(bookNum, 15, 0, versification);
								yield return new TestMarkerToken(verse, "c", "15");
								if (chapterNum == 0)
									goto case 16;
								break;
							case 16:
								verse = new TestVerse(bookNum, 16, 0, versification);
								yield return new TestMarkerToken(verse, "c", "16");
								if (chapterNum == 0)
									goto case 17;
								break;
							case 17:
								verse = new TestVerse(bookNum, 17, 0, versification);
								yield return new TestMarkerToken(verse, "c", "17");
								if (chapterNum == 0)
									goto case 18;
								break;
							case 18:
								verse = new TestVerse(bookNum, 18, 0, versification);
								yield return new TestMarkerToken(verse, "c", "18");
								if (chapterNum == 0)
									goto case 19;
								break;
							case 19:
								verse = new TestVerse(bookNum, 19, 0, versification);
								yield return new TestMarkerToken(verse, "c", "19");
								if (chapterNum == 0)
									goto case 20;
								break;
							case 20:
								verse = new TestVerse(bookNum, 20, 0, versification);
								yield return new TestMarkerToken(verse, "c", "20");
								if (chapterNum == 0)
									goto case 21;
								break;
							case 21:
								verse = new TestVerse(bookNum, 21, 0, versification);
								yield return new TestMarkerToken(verse, "c", "21");
								if (chapterNum == 0)
									goto case 22;
								break;
							case 22:
								verse = new TestVerse(bookNum, 22, 0, versification);
								lastTok = new TestMarkerToken(verse, "c", "22");
								yield return lastTok;
								yield return new TestMarkerToken(lastTok, "p");
								verse = new TestVerse(bookNum, 22, 40, versification);
								foreach (var tok in GetVerses(verse,
									         "While the Pharisees were together, Jesus questioned them, ",
									         "saying, “Whose son is the Christ?” They replied, “The son of David.” ",
									         "He said to them, “Why does David call him Lord, saying, "))
								{
									lastTok = tok;
									yield return tok;
								}
								verse = new TestVerse(bookNum, 22, 44, versification);
								foreach (var tok in GetPoetryLinesFor(verse, lastTok.EndVerseOffset,
									         "1:" + GetCont(1, "p", "q1") + kMat22V44Line1SansLev1Cont,
									         "1:" + GetCont(2, "p", "q1") + kMat22V44Line2SansLev12Cont,
									         "2:" + GetCont(3, "q1", "q2") + kMat22V44Line3SansConts + kMat22V44Line3AfterQuote))
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
								lastTok = new TestMarkerToken(verse, "id", "JHN");
								yield return lastTok;
								yield return new TestMarkerToken(verse, "c", "1", lastTok.EndVerseOffset);
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

			internal string GetCont(int lev, string prevParaTag, string thisParaTag) =>
				Language.QuotationMarkInfo.IsContinuerRequired(thisParaTag, prevParaTag) ?
					GetCont(lev) : "";

			internal string GetCont(int lev)
			{
				var primaryQ = Language.QuotationMarkInfo.PrimaryLevels;
				if (lev == 0)
					return primaryQ[lev].Continuer;
				var sb = new StringBuilder();
				for (var l = 0; l < lev; l++)
					sb.Append(primaryQ[l].Continuer);

				return sb.ToString();
			}

			private IEnumerable<TestTokenBase> GetVerses(IVerseRef verse, params string[] verseTextsAndParagraphBreaks)
			{
				Assert.That(verseTextsAndParagraphBreaks.Length, Is.GreaterThan(0));
				Assert.That(verseTextsAndParagraphBreaks[0], Is.Not.EqualTo(NewLine));
				Assert.That(verseTextsAndParagraphBreaks[0], Does.Not.StartWith("\\"));

				TestTokenBase tok = null;
				foreach (var t in verseTextsAndParagraphBreaks)
				{
					var marker = t == NewLine ? "p" : (t.StartsWith("\\") ? t.Substring(1) : null);
					if (marker != null)
					{
						Debug.Assert(tok != null);
						tok = new TestMarkerToken(tok, marker);
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

			private IEnumerable<TestTokenBase> GetPoetryLinesFor(IVerseRef verse, int startingOffset, params string[] poetryLines)
			{
				TestTokenBase tok = null;
				bool outputVerseToken = true;

				foreach (var t in poetryLines)
				{
					var marker = "q";
					var text = t;
					if (IsDigit(t[0]) && t[1] == ':')
					{
						text = text.Substring(2);
						marker += t.Substring(0, 1);
					}

					tok = new TestMarkerToken(
						outputVerseToken? verse.GetPreviousVerse(null) : verse, 
						marker, offset:tok?.EndVerseOffset ?? startingOffset);
					yield return tok;

					if (outputVerseToken)
					{
						outputVerseToken = false;
						tok = new TestMarkerToken(verse, "v", verse.VerseNum.ToString());
						yield return tok;
					}

					tok = new TestTextToken(verse, text, tok.EndVerseOffset);
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
			var quoteInfo = GetQuoteInfo(1, Continuers.RepeatAllOpeners);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			var verse = new TestVerse(43, 4, 9);
			SanityCheckUSFMs(project, verse);
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
			var quoteInfo = GetQuoteInfo(quoteLevels, Continuers.RepeatAllOpeners);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			var verse = new TestVerse(24, 1, 5);
			SanityCheckUSFMs(project, verse);
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

		[Test]
		public void SanityCheckOfGetQuoteInfoAndGetContHelperMethod()
		{
			var quoteInfo = GetQuoteInfo(2, Continuers.RepeatAllOpeners);
			Assert.That(quoteInfo.PrimaryLevels.Count, Is.EqualTo(2));
			Assert.That(quoteInfo.PrimaryLevels[0].Opener, Is.EqualTo("“"));
			Assert.That(quoteInfo.PrimaryLevels[0].Closer, Is.EqualTo("”"));
			Assert.That(quoteInfo.PrimaryLevels[0].Continuer, Is.EqualTo("“"));
			Assert.That(quoteInfo.PrimaryLevels[1].Opener, Is.EqualTo("‘"));
			Assert.That(quoteInfo.PrimaryLevels[1].Closer, Is.EqualTo("’"));
			Assert.That(quoteInfo.PrimaryLevels[1].Continuer, Is.EqualTo("‘"));
			quoteInfo.ContinuersRequired = new Dictionary<string, List<string>>
			{
				["p"] = new List<string>(new [] {"p", "q1"}),
				["q1"] = new List<string>(new [] {"q2"})
			};
			var project = new TestProject(quoteInfo);
			Assert.That(project.GetCont(1), Is.EqualTo(quoteInfo.PrimaryLevels[0].Continuer));
			Assert.That(project.GetCont(2), Is.EqualTo("“‘"));
			Assert.That(project.GetCont(1, "p", "q1"), Is.EqualTo(quoteInfo.PrimaryLevels[0].Continuer));
			Assert.That(project.GetCont(1, "p", "p"), Is.EqualTo(quoteInfo.PrimaryLevels[0].Continuer));
			Assert.That(project.GetCont(2, "q1", "q2"), Is.EqualTo("“‘"));
			Assert.That(project.GetCont(2, "p", "p"), Is.EqualTo("“‘"));
			Assert.That(project.GetCont(1, "q1", "q1"), Is.Empty);
			Assert.That(project.GetCont(1, "s", "s"), Is.Empty);
			Assert.That(project.GetCont(2, "q1", "q1"), Is.Empty);
			Assert.That(project.GetCont(2, "q2", "q2"), Is.Empty);
		}
		
		[TestCase(2, Continuers.RepeatAllOpeners)]
		[TestCase(3, Continuers.RepeatAllOpeners)]
		[TestCase(2, Continuers.RepeatAllClosers)]
		[TestCase(3, Continuers.RepeatAllClosers)]
		[TestCase(2, Continuers.DifferentPunct)]
		[TestCase(3, Continuers.DifferentPunct)]
		[TestCase(2, Continuers.None)]
		[TestCase(3, Continuers.None)]
		public void GetAnnotations_MultiplePrimaryQuoteLevelsDefined_ReturnsLevel1And2Annotations(
			int quoteLevels, Continuers continuers)
		{
			var quoteInfo = GetQuoteInfo(quoteLevels, continuers);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			var verse = new TestVerse(24, 1, 7);
			SanityCheckUSFMs(project, verse);
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

		[TestCase(3, Continuers.RepeatAllOpeners)]
		[TestCase(3, Continuers.RepeatAllClosers)]
		[TestCase(3, Continuers.DifferentPunct)]
		[TestCase(3, Continuers.None)]
		public void GetAnnotations_ThreePrimaryQuoteLevelsDefined_ReturnsLevel2And3Annotations(
			int quoteLevels, Continuers continuers)
		{
			var quoteInfo = GetQuoteInfo(quoteLevels, continuers);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			var verse = new TestVerse(40, 22, 44);
			SanityCheckUSFMs(project, verse);

			var line1Text = project.GetCont(1) + TestProject.kMat22V44Line1SansLev1Cont;
			var line2Text = project.GetCont(2) + TestProject.kMat22V44Line2SansLev12Cont;
			var line3Text = project.GetCont(3) + TestProject.kMat22V44Line3SansConts;
			var annotations = sut.GetAnnotations(verse, @"\v 44 " + @"\q1 " + line1Text +
				@"\q2 " + line2Text + @"\q2 " + line3Text + TestProject.kMat22V44Line3AfterQuote);

			// The first annotation is for the verse number itself.
			Assert.That(annotations.Count, Is.EqualTo(5));
			Assert.That(annotations[0].ScriptureSelection.Offset, Is.EqualTo(1));
			Assert.That(annotations[0].ScriptureSelection.SelectedText,
				Is.EqualTo(@"\v 44 "));
			Assert.That(annotations[0].StyleName, Is.AnyOf("quote1", "quote2"));
			Assert.That(annotations[1].ScriptureSelection.SelectedText,
				Is.EqualTo(line1Text));
			Assert.That(annotations[1].StyleName, Is.EqualTo("quote2"));
			Assert.That(annotations[2].ScriptureSelection.SelectedText,
				Is.EqualTo(line2Text));
			Assert.That(annotations[2].StyleName, Is.EqualTo("quote3"));
			Assert.That(annotations[3].ScriptureSelection.SelectedText,
				Is.EqualTo(line3Text));
			Assert.That(annotations[3].StyleName, Is.EqualTo("quote3"));
			Assert.That(annotations[4].ScriptureSelection.SelectedText,
				Is.EqualTo(TestProject.kMat22V44Line3AfterQuote));
			Assert.That(annotations[4].StyleName, Is.EqualTo("quote1"));
			foreach (var annotation in annotations)
				VerifyPluginAnnotation(annotation, project, verse);
		}

		[TestCase(1, Continuers.RepeatAllOpeners)]
		[TestCase(1, Continuers.RepeatAllClosers)]
		[TestCase(1, Continuers.DifferentPunct)]
		[TestCase(1, Continuers.None)]
		[TestCase(2, Continuers.RepeatAllOpeners)]
		[TestCase(2, Continuers.RepeatAllClosers)]
		[TestCase(2, Continuers.None)]
		public void GetAnnotations_VerseStartsAtQuoteLevel1_ReturnsAnnotation(int quoteLevels,
			Continuers continuers)
		{
			var quoteInfo = GetQuoteInfo(quoteLevels, continuers);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			var verse = new TestVerse(24, 1, 8);
			var annotations = sut.GetAnnotations(verse,
				@"\v 8 " + TestProject.kJer1V8Lev1Quote + TestProject.kJer1V8AfterQuote);

			// The first annotation is for the verse number itself.
			Assert.That(annotations.Count, Is.EqualTo(2));
			Assert.That(annotations[0].ScriptureSelection.Offset, Is.EqualTo(1));
			Assert.That(annotations[0].ScriptureSelection.SelectedText,
				Is.EqualTo(@"\v 8 "));
			Assert.That(annotations[1].ScriptureSelection.SelectedText,
				Is.EqualTo(TestProject.kJer1V8Lev1Quote));

			foreach (var annotation in annotations)
			{
				VerifyPluginAnnotation(annotation, project, verse);
				Assert.That(annotation.StyleName, Is.EqualTo("quote1"));
			}
		}
		
		[Test]
		public void GetAnnotations_SectionHeadInMiddleOfQuote_SectionHeadNotAnnotated()
		{
			var quoteInfo = GetQuoteInfo(3, Continuers.RepeatAllOpeners);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			Assert.Ignore("Write this test");
		}
		
		[Test]
		public void GetAnnotations_SectionHeadWithQuoteInMiddleOfQuote_SectionHeadNotAnnotated()
		{
			var quoteInfo = GetQuoteInfo(3, Continuers.RepeatAllOpeners);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			Assert.Ignore("Write this test");
		}
		
		[Test]
		public void GetAnnotations_SectionHeadWithQuoteLeftOpenInMiddleOfQuote_SectionHeadNotAnnotated()
		{
			var quoteInfo = GetQuoteInfo(3, Continuers.RepeatAllOpeners);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			Assert.Ignore("Write this test");
		}

		[TestCase(Continuers.RepeatAllOpeners)]
		[TestCase(Continuers.RepeatAllClosers)]
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

			// The first annotation is for the verse number itself.
			Assert.That(annotations.Count, Is.EqualTo(2));
			Assert.That(annotations[0].ScriptureSelection.Offset, Is.EqualTo(1));
			Assert.That(annotations[0].ScriptureSelection.SelectedText,
				Is.EqualTo(@"\v 4 "));
			Assert.That(annotations[1].ScriptureSelection.SelectedText,
				Is.EqualTo(quoteInfo.PrimaryLevels[0].Continuer + TestProject.kMat5V4Text));

			foreach (var annotation in annotations)
			{
				VerifyPluginAnnotation(annotation, project, verse);
				Assert.That(annotation.StyleName, Is.EqualTo("quote1"));
			}
		}

		[TestCase(Continuers.RepeatAllOpeners)]
		[TestCase(Continuers.RepeatAllClosers)]
		[TestCase(Continuers.None)]
		public void GetAnnotations_Lev1ContinuerAtStartOfMidVersePara_ReturnsAnnotationsForContPara(
			Continuers continuers)
		{
			var quoteInfo = GetQuoteInfo(1, continuers);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			Assert.Ignore("Write this test");
		}

		[TestCase(Continuers.RepeatAllOpeners)]
		[TestCase(Continuers.RepeatAllClosers)]
		[TestCase(Continuers.None)]
		public void GetAnnotations_Lev2Continuer_ReturnsAnnotationsForContPara(
			Continuers continuers)
		{
			var quoteInfo = GetQuoteInfo(2, continuers);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			Assert.Ignore("Write this test");
		}

		[TestCase(Continuers.RepeatAllOpeners)]
		[TestCase(Continuers.None)]
		public void GetAnnotations_Lev3Continuer_ReturnsAnnotationsForContPara(
			Continuers continuers)
		{
			var quoteInfo = GetQuoteInfo(3, continuers);
			var project = new TestProject(quoteInfo);
			var sut = new AnnotationSource(null, project);
			Assert.Ignore("Write this test");
		}

		private TestQuotationMarkInfo GetQuoteInfo(int quoteLevels, Continuers continuers)
		{
			var primaryLevels = new TestQuoteLevel[quoteLevels];
			primaryLevels[0] = new TestQuoteLevel("“", "”", GetContinuer(0, continuers));
			if (quoteLevels > 1)
				primaryLevels[1] = new TestQuoteLevel("‘", "’", GetContinuer(1, continuers));
			if (quoteLevels > 2)
				primaryLevels[2] = new TestQuoteLevel("“", "”", GetContinuer(2, continuers));

			return new TestQuotationMarkInfo(primaryLevels);
		}

		/// <summary>
		/// Gets the string representing a well-formed "66/99" quote continuer for
		/// the requested level and option
		/// </summary>
		/// <param name="level">0-based level</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private string GetContinuer(int level, Continuers continuers)
		{
			switch (continuers)
			{
				case Continuers.RepeatAllOpeners:
					return level % 2 == 0 ? "“" : "‘";
				case Continuers.RepeatAllClosers:
					return level % 2 == 0 ? "”" : "’";
				case Continuers.DifferentPunct:
					switch (level)
					{
						case 0: return "~";
						case 1: return "#";
						case 2: return "^";
						case 3: return "%";
						case 4: return "*";
						default: throw new NotImplementedException(
							"Wasn't expecting to get this nested!");
					}
				case Continuers.None:return "";
				default:
					throw new ArgumentOutOfRangeException(nameof(continuers), continuers, null);
			}
		}

		private static void SanityCheckUSFMs(IProject project, IVerseRef verse)
		{
			var gotId = false;
			var gotChapter = false;
			Assert.That(verse.ChapterNum, Is.GreaterThan(0),
				"This sanity check method is only for checking the markers for a single chapter.");
			int expectedOffset = 0;
			verse = new TestVerse(verse.BookNum, verse.ChapterNum, 0);
			foreach (var tok in project.GetUSFMTokens(verse.BookNum, verse.ChapterNum))
			{
				Assert.That(tok.VerseRef.BookNum.Equals(verse.BookNum), $"SETUP problem ({tok.VerseRef})");
				Assert.That(tok.VerseRef.ChapterNum.Equals(verse.ChapterNum), $"SETUP problem ({tok.VerseRef})");
				if (tok is IUSFMMarkerToken markerToken)
				{
					switch (markerToken.Marker)
					{
						case "id":
							Assert.IsFalse(gotId, $"SETUP problem ({tok.VerseRef}): Too many IDs");
							gotId = true;
							Assert.That(verse.ChapterNum, Is.EqualTo(1),
								$"SETUP problem ({tok.VerseRef}, id {markerToken.Data}): " +
								"Should only get an ID token as part of a chapter 1, verse 0.");
							Assert.That(tok.VerseRef.VerseNum, Is.EqualTo(0),
								$"SETUP problem ({tok.VerseRef}, id {markerToken.Data}): " +
								"Should only get an ID token as part of a chapter 1, verse 0.");
							break;
						case "c":
							Assert.IsFalse(gotChapter, $"SETUP problem ({tok.VerseRef}): " +
								"Too many chapters");
							gotChapter = true;
							Assert.That(tok.VerseRef.VerseNum, Is.EqualTo(0),
								$"SETUP problem ({tok.VerseRef}, c {markerToken.Data}): " +
								"Should only get this token as part of verse 0.");
							Assert.That(int.Parse(markerToken.Data), Is.EqualTo(verse.ChapterNum),
								$"SETUP problem ({tok.VerseRef}, c {markerToken.Data})");
							verse = markerToken.VerseRef;
							break;
						case "v":
							// ENHANCE: handle verse bridges
							Assert.That(int.Parse(markerToken.Data), Is.GreaterThan(verse.VerseNum),
								$"SETUP problem ({tok.VerseRef}, v {markerToken.Data}): " +
								"Verses should be in ascending order.");
							verse = markerToken.VerseRef;
							expectedOffset = 0;
							break;
						case "s":
						case "s1":
						case "s2":
							Assert.False(markerToken.IsScripture, 
								$"SETUP problem ({tok.VerseRef}, {markerToken.Marker})");
							Assert.That(tok.VerseRef.Equals(verse), 
								$"SETUP problem ({tok.VerseRef}, {markerToken.Marker}): " +
								"Verse should change only when a verse token is encountered.");
							break;
						default:
							Assert.True(markerToken.IsScripture,
								$"SETUP problem ({tok.VerseRef}, {markerToken.Marker})");
							Assert.That(tok.VerseRef.Equals(verse),
								$"SETUP problem ({tok.VerseRef}, {markerToken.Marker}): " +
								"Verse should change only when a chapter or verse token is encountered.");
							break;
					}
				}
				else
				{
					Assert.That(tok.VerseRef.Equals(verse), $"SETUP problem ({tok.VerseRef}): " +
						"Verse should change only when a verse token is encountered.");

					if (tok is IUSFMTextToken textToken)
					{
						Assert.That(textToken.Text.Length, Is.GreaterThan(0), 
							$"SETUP problem ({tok.VerseRef})");
					}
					else
					{
						Assert.Fail($"SETUP problem ({tok.VerseRef}): " +
							"This type of token not yet accounted for in tests.");
					}
				}

				Assert.That(tok.VerseOffset, Is.EqualTo(expectedOffset),
					$"SETUP problem ({tok.VerseRef})");
				var newEndOffset =((ITestToken)tok).EndVerseOffset; 
				Assert.That(newEndOffset, Is.GreaterThan(expectedOffset));
				expectedOffset = newEndOffset;
			}
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
			string GetDetails(string property) => $"Annotation for {scrSel.SelectedText} in {scrSel.VerseRefStart} had unexpected {property}.";
			Assert.That(scrSel.SelectedText, Is.EqualTo(matchingSelFromProject.SelectedText),
				GetDetails(nameof(IScriptureTextSelection.SelectedText)));
			Assert.That(matchingSelFromProject.BeforeContext, Does.EndWith(scrSel.BeforeContext),
				GetDetails(nameof(IScriptureTextSelection.BeforeContext)));
			Assert.That(scrSel.AfterContext, Is.EqualTo(matchingSelFromProject.AfterContext),
				GetDetails(nameof(IScriptureTextSelection.AfterContext)));
			Assert.That(scrSel.Offset, Is.EqualTo(matchingSelFromProject.Offset),
				GetDetails(nameof(IScriptureTextSelection.Offset)));
			Assert.That(scrSel.VerseRefStart.BBBCCCVVV, Is.EqualTo(verse.BBBCCCVVV),
				GetDetails(nameof(IScriptureTextSelection.VerseRefStart)));

			if (!verse.RepresentsMultipleVerses)
			{
				Assert.That(scrSel.VerseRefEnd, Is.EqualTo(scrSel.VerseRefStart),
					GetDetails(nameof(IScriptureTextSelection.VerseRefEnd)));
			}
		}
	}
}
