// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International.   
// <copyright from='2022' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Paratext.PluginInterfaces;
using static System.Char;

namespace SIL.Chono
{
    internal class AnnotationSource : IPluginAnnotationSource
    {
	    public const string kQuoteStylePrefix = "quote";
	    public const string kNonScrQuoteStylePrefix = "xquote";
	    private const string kError = "red";
	    private const string kRgxOpenLevelGroupPrefix = "lev";
	    private const string kRgxCloserGroupPrefix = "end";
	    private const string kRgxContinuerGroupPrefix = "con";
	    private const string kRgxFinalGroup = "fin";

	    private class ChapterAnnotationInfo
        {
	        public IReadOnlyList<IPluginAnnotation> Annotations { get; set; }
	        public int QuoteLevelAtEndOfChapter { get; }
	        public int QuoteLevelAtStartOfChapter { get; }

	        public ChapterAnnotationInfo(IReadOnlyList<IPluginAnnotation> annotations,
	            int quoteLevelAtEndOfChapter, int quoteLevelAtStartOfChapter)
			{
		        Annotations = annotations;
		        QuoteLevelAtEndOfChapter = quoteLevelAtEndOfChapter;
		        QuoteLevelAtStartOfChapter = quoteLevelAtStartOfChapter;
	        }
        }

        private readonly IPluginHost m_host;
        private readonly IProject m_project;
        private readonly HashSet<char> m_allQuoteChars;
	    private readonly Regex m_findMarksRegex;
	    private int m_currentBook = -1;
        private readonly Dictionary<int, ChapterAnnotationInfo> m_currentBookAnnotations =
	        new Dictionary<int, ChapterAnnotationInfo>();

        public AnnotationSource(IPluginHost host, IProject project)
        {
	        m_host = host;
	        m_project = project;
            project.ProjectDataChanged += ProjectDataChanged;
            project.ScriptureDataChanged += ScriptureDataChanged;
            var quotationMarks = project.Language.QuotationMarkInfo;
            if (quotationMarks?.PrimaryLevels == null)
                return;

            var allMarks = new Dictionary<string, int>();

            StringBuilder bldr = new StringBuilder();
            int currentLevelInBuilder = -1;

            void AppendQuotationMark(string mark)
            {
	            if (mark.Length > 1)
		            bldr.Append("(");
	            bldr.Append(Regex.Escape(mark));
	            if (mark.Length > 1)
		            bldr.Append(")");
            }
            
            void AppendNewLevel(int level, string mark)
            {
	            bldr.Append("(?<");
	            bldr.Append(kRgxOpenLevelGroupPrefix);
	            bldr.Append(level);
	            allMarks[mark] = bldr.Length;
	            bldr.Append(">");
            }
            
            void Insert(int i, int level, string mark)
            {
	            bldr.Insert(i, level);
	            var keysToIncrement = allMarks.Where(kvp => kvp.Value >= allMarks[mark])
		            .Select(kvp => kvp.Key).ToHashSet();
	            foreach (var key in keysToIncrement)
					allMarks[key]++;
            }

            // TODO: Handle secondary levels also.
            for (int level = 0; level < quotationMarks.PrimaryLevels.Count;)
            {
                IQuotationMarkLevel lev = quotationMarks.PrimaryLevels[level];
                int i;

				// TODO: Deal with apostrophe's (the same way Paratext does)...

                if (allMarks.TryGetValue(lev.Opener, out i))
                    Insert(i, level, lev.Opener);
                else
                {
	                if (level > currentLevelInBuilder)
                    {
                        if (bldr.Length > 0)
	                        bldr.Append(")");
                        AppendNewLevel(level, lev.Opener);
                        currentLevelInBuilder = level;
                    }
                    else
                    {
	                    bldr.Append("|");
	                    AppendNewLevel(level, lev.Opener);
                    }

	                AppendQuotationMark(lev.Opener);
					bldr.Append(")");
                }

                level++;

                if (allMarks.TryGetValue(lev.Closer, out i))
	                Insert(i, level, lev.Closer);
                else
                {
	                bldr.Append("|");
	                bldr.Append("(?<");
	                bldr.Append(kRgxCloserGroupPrefix);
	                bldr.Append(level);
	                allMarks[lev.Closer] = bldr.Length;
	                bldr.Append(">");
	                AppendQuotationMark(lev.Closer);
					currentLevelInBuilder = level;
					bldr.Append(")");
                }

                if (!string.IsNullOrEmpty(lev.Continuer) && !allMarks.TryGetValue(lev.Continuer, out i))
                {
	                if (level > currentLevelInBuilder)
	                {
                        AppendNewLevel(level, lev.Continuer);
		                currentLevelInBuilder = level;
	                }
	                else
	                {
		                bldr.Append("|");
		                bldr.Append("(?<");
		                bldr.Append(kRgxContinuerGroupPrefix);
		                bldr.Append(level);
		                allMarks[lev.Continuer] = bldr.Length;
		                bldr.Append(">");
	                }
	                bldr.Append("(^");
	                bldr.Append(Regex.Escape(lev.Continuer));
	                bldr.Append("))");
                }
            }

            bldr.Append("|(?<");
            bldr.Append(kRgxFinalGroup);
            bldr.Append(">.$)");

            m_findMarksRegex = new Regex(bldr.ToString(), RegexOptions.Compiled);
            m_allQuoteChars = new HashSet<char>(allMarks.Keys.SelectMany(k => k.ToCharArray()));
        }

        private void ProjectDataChanged(IProject sender, ProjectDataChangeType details)
        {
	        if (details == ProjectDataChangeType.WholeProject)
		        ClearAllAnnotations();
        }

        private void ScriptureDataChanged(IProject sender, int bookNum, int chapterNum)
        {
            if (bookNum == m_currentBook || bookNum == 0)
            {
                if (chapterNum == 0)
		            ClearAllAnnotations();
                else
                {
                    if (m_currentBookAnnotations.TryGetValue(chapterNum, out var annotationInfo))
	                    annotationInfo.Annotations = null;
                    var currentRef = m_host.ActiveWindowState.VerseRef;
                    if (currentRef.BookNum == m_currentBook && currentRef.ChapterNum <= chapterNum)
						AnnotationsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void ClearAllAnnotations()
        {
	        m_currentBookAnnotations.Clear();
	        m_currentBook = -1;
	        AnnotationsChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Implementation of IPluginAnnotationSource
        public event EventHandler AnnotationsChanged;

        public bool MaintainSelectionsOnWordBoundaries => false;

        public IReadOnlyList<AnnotationStyle> GetStyleInfo(double zoom)
        {
            return new[]
            {
                new AnnotationStyle(kQuoteStylePrefix + "0", "background-color:lightgreen;"),
                new AnnotationStyle(kQuoteStylePrefix + "1", "background-color:lawngreen;"),
                new AnnotationStyle(kQuoteStylePrefix + "2", "background-color:green;"),
                new AnnotationStyle(kQuoteStylePrefix + "3", "background-color:darkgreen;"),
				// Unlikely that we would ever need to go more than two levels deep in section
				// heads, intro material, etc.
                new AnnotationStyle(kNonScrQuoteStylePrefix + "0", "background-color:khaki;"),
                new AnnotationStyle(kNonScrQuoteStylePrefix + "1", "background-color:gold;"),
                new AnnotationStyle(kError, "background-color:red;"),
            };
        }

        public IReadOnlyList<IPluginAnnotation> GetAnnotations(IVerseRef verseRef, string usfm)
        {
	        if (m_findMarksRegex == null)
		        return null;

	        if (m_currentBook != verseRef.BookNum)
	        {
		        m_currentBookAnnotations.Clear();
		        m_currentBook = verseRef.BookNum;
	        }

	        var chapter = verseRef.ChapterNum;
	        PopulateAnnotations(verseRef.BookNum, chapter);

	        return m_currentBookAnnotations[chapter].Annotations.Where(a =>
		        a.ScriptureSelection.VerseRefStart.CompareTo(verseRef) <= 0 &&
		        a.ScriptureSelection.VerseRefEnd.CompareTo(verseRef) >= 0).ToList();
        }

        private void PopulateAnnotations(int bookNum, int chapter)
        {
            // First see if any "holes" in a preceding chapter require us to
            // rescan.
            int startingQuoteLevel = 0;
            for (int c = 1; c <= chapter; c++)
            {
                if (m_currentBookAnnotations.TryGetValue(c, out var annotationInfo))
                {
	                if (annotationInfo.Annotations != null)
	                {
		                startingQuoteLevel = annotationInfo.QuoteLevelAtEndOfChapter;
		                continue;
	                }
                }

                var currentQuoteLevel = new Dictionary<bool, int>
                {
	                [true] = startingQuoteLevel,
	                [false] = 0
                };
                var annotations = new List<IPluginAnnotation>();
                bool atParaStart = false;
                foreach (var tok in m_project.GetUSFMTokens(bookNum, c))
                {
					// Quotes in non-Scripture never carry over to subsequent non-Scripture text.
	                if (tok.IsScripture)
		                currentQuoteLevel[false] = 0;

	                if (tok is IUSFMMarkerToken markerTok)
	                {
		                if (markerTok.Marker == "v")
		                {
							// See https://github.com/ubsicap/paratext_demo_plugins/issues/18
							//if (currentQuoteLevel[tok.IsScripture] > 0)
							//{
							//	annotations.Add(new Annotation(new Selection($"\\v {markerTok.Data} ",
							//			"", "", tok.VerseRef, 1),
							//		currentQuoteLevel[true]));
							//}
						}
		                else if (markerTok.Type == MarkerType.Paragraph)
						{
							atParaStart = true;
						}
	                }
					else if (tok is IUSFMTextToken textTok)
	                {
		                var text = textTok.Text;
		                int start = 0;
		                var continuerLevel = 0;

		                void AddAnnotation(Capture capture, bool openingNestedQuote = false)
		                {
			                continuerLevel = 0;
			                var cCaptureCharsIncludedInSel = 
				                (openingNestedQuote ? 0 : capture.Length);
			                var selLength = capture.Index + cCaptureCharsIncludedInSel - start;
			                if (selLength == 0)
				                return;
			                var selectedText = text.Substring(start, selLength);
			                var level = currentQuoteLevel[tok.IsScripture];
			                if (!(annotations.LastOrDefault() is Annotation prevAnnotation) ||
			                    !prevAnnotation.TryExtend(tok.VerseRef, selectedText, level, 
				                    tok.IsScripture, m_allQuoteChars))
			                {
				                annotations.Add(new Annotation(
					                new Selection(
						                selectedText,
						                text.Substring(0, start),
						                text.Substring(capture.Index + cCaptureCharsIncludedInSel),
						                textTok.VerseRef,
						                textTok.VerseOffset + start),
					                level, tok.IsScripture));
			                }
		                }

		                foreach (Match match in m_findMarksRegex.Matches(text))
		                {
							// We skip the first one, which is always "0".
			                foreach (var matchGroup in match.SuccessfulMatchGroups().Skip(1))
			                {
				                var currLevel = currentQuoteLevel[tok.IsScripture];
								if (atParaStart)
								{
									if (continuerLevel < currLevel && IsCorrectContinuer(continuerLevel, match))
									{
										atParaStart = ++continuerLevel < currLevel;
										continue;
									}
									atParaStart = false;
								}

				                if (matchGroup.Name.StartsWith(kRgxOpenLevelGroupPrefix))
				                {
					                var validMatchLevels = matchGroup.Name.Substring(3);
					                var index = validMatchLevels.IndexOf(currLevel.ToString(),
						                StringComparison.Ordinal);
					                if (index >= 0)
					                {
						                if (currLevel > 0)
							                AddAnnotation(match, true);

						                start = match.Index;
						                currentQuoteLevel[tok.IsScripture]++;
					                }
					                break;
				                }

				                if (matchGroup.Name.StartsWith(kRgxCloserGroupPrefix))
				                {
					                if (currLevel > 0)
					                {
										continuerLevel = 0;
										var validMatchLevels = matchGroup.Name.Substring(3);
										if (validMatchLevels.Contains(currentQuoteLevel[tok.IsScripture].ToString()))
										{
											AddAnnotation(match);
											if (--currentQuoteLevel[tok.IsScripture] > 0)
												start = match.Index + match.Length;
										}
									} 
										
									break;
				                }

				                if (matchGroup.Name == kRgxFinalGroup && currentQuoteLevel[tok.IsScripture] > 0)
				                {
					                AddAnnotation(match);
					                continuerLevel = 0;
					                break;
				                }
			                
							}

			                atParaStart = continuerLevel > 0;
		                }
		                
		                atParaStart = false;
	                }
                }

                m_currentBookAnnotations[c] = new ChapterAnnotationInfo(annotations,
	                currentQuoteLevel[true], startingQuoteLevel);

                startingQuoteLevel = currentQuoteLevel[true];
            }
        }

        private bool IsCorrectContinuer(int continuerLevel, Match match)
        {
			// TODO: Handle secondary levels
	        return m_project.Language.QuotationMarkInfo.PrimaryLevels[continuerLevel].Continuer == match.Value;
        }

        #endregion

        #region Annotation class
        private sealed class Annotation : IPluginAnnotation
        {
	        private readonly Selection m_selection;
	        private int m_level;
	        private readonly bool m_isScripture;

	        public Annotation(Selection selection, int level, bool isScripture = true)
	        {
		        m_selection = selection;
		        m_level = level;
		        m_isScripture = isScripture;
	        }

	        public IScriptureTextSelection ScriptureSelection => m_selection;

            public string StyleName
            {
	            get
	            {
		            var prefix = m_isScripture ? kQuoteStylePrefix : kNonScrQuoteStylePrefix;
		            int level = m_isScripture ? m_level % 4 : m_level % 2;
			        return prefix + level.ToString(CultureInfo.InvariantCulture);
	            }
            }

            public string Icon => null;

            public string IconToolTipText => null;
            
            public bool Click(MouseButton button, bool onIcon, Point location)
            {
                return false;
            }

			public bool TryExtend(IVerseRef verse, string selectedText, int level, bool isScripture,
				HashSet<char> allQuoteChars)
			{
				if (verse.Equals(ScriptureSelection.VerseRefStart) &&
				    isScripture == m_isScripture)
				{
					if (level == m_level + 1)
					{
						if (ScriptureSelection.SelectedText.Any() &&
						    ScriptureSelection.SelectedText.All(c => allQuoteChars.Contains(c) || IsWhiteSpace(c)))
						{
							m_selection.Merge(selectedText);
							m_level = level;
							return true;
						}
					}
					if (level == m_level - 1 &&
					    selectedText.All(allQuoteChars.Contains))
					{
						m_selection.Merge(selectedText);
						return true;
					}
				}

				return false;
			}
        }
        #endregion
        
        #region Selection class
        private sealed class Selection : IScriptureTextSelection
        {
            public Selection(string selectedText, string beforeContext, string afterContext,
                IVerseRef verseRef, int offset = 0)
            {
                SelectedText = selectedText;
                BeforeContext = beforeContext;
                AfterContext = afterContext;
                VerseRefStart = verseRef;
                VerseRefEnd = verseRef;
                Offset = offset;
            }

            public IVerseRef VerseRefStart { get; }

            public IVerseRef VerseRefEnd { get; }

            public string SelectedText { get; private set; }

            public int Offset { get; }

            public string BeforeContext { get; }

            public string AfterContext { get; private set; }

            public bool Equals(ISelection other)
            {
	            if (!(other is IScriptureTextSelection o))
		            return false;
                return SelectedText.Equals(o.SelectedText) &&
	                BeforeContext.Equals(o.BeforeContext) &&
	                AfterContext.Equals(o.AfterContext) &&
	                VerseRefStart.Equals(o.VerseRefStart) &&
	                VerseRefEnd.Equals(o.VerseRefEnd) &&
	                Offset.Equals(o.Offset);
            }

			public void Merge(string additionalSelectedText)
			{
				if (!AfterContext.StartsWith(additionalSelectedText, StringComparison.Ordinal))
					throw new InvalidOperationException("Invalid merge!");
				SelectedText += additionalSelectedText;
				AfterContext = AfterContext.Substring(additionalSelectedText.Length);
			}
        }
        #endregion
    }
}
