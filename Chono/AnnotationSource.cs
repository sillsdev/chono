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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Paratext.PluginInterfaces;

namespace SIL.Chono
{
    internal class AnnotationSource : IPluginAnnotationSource
    {
	    private const string kQuoteStylePrefix = "quote";
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

                if (!string.IsNullOrEmpty(lev.Continuer) && allMarks.TryGetValue(lev.Continuer, out i))
                {
	                if (Int32.Parse(bldr[i - 1].ToString()) != level)
						Insert(i, level, lev.Continuer);
                }
                else
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

                var currentQuoteLevel = startingQuoteLevel;
                var annotations = new List<IPluginAnnotation>();
                bool atParaStart = false;
                foreach (var tok in m_project.GetUSFMTokens(bookNum, c).Where(t => t.IsScripture))
                {
	                if (tok is IUSFMMarkerToken markerTok)
	                {
						if (currentQuoteLevel > 0 && markerTok.Marker == "v")
						{
							annotations.Add(new Annotation(new Selection(markerTok.Data, "", "", tok.VerseRef),
								kQuoteStylePrefix + currentQuoteLevel));
						}
						else if (markerTok.Type == MarkerType.Paragraph)
							atParaStart = true;
	                }
					else if (tok is IUSFMTextToken textTok)
	                {
		                var text = textTok.Text;
		                int start = 0;
		                var continuer = false;

		                void AddAnnotation(Capture capture, bool openingNestedQuote = false)
		                {
			                var cCaptureCharsIncludedInSel = 
				                (openingNestedQuote ? 0 : capture.Length);
			                annotations.Add(new Annotation(
				                new Selection(
					                text.Substring(start,
						                capture.Index + cCaptureCharsIncludedInSel - start),
					                text.Substring(0, start),
					                text.Substring(capture.Index + cCaptureCharsIncludedInSel),
					                textTok.VerseRef,
					                textTok.VerseOffset + start),
				                kQuoteStylePrefix + currentQuoteLevel));
		                }

		                foreach (Match match in m_findMarksRegex.Matches(text))
		                {
							// We skip the first one, which is always "0".
			                foreach (var matchGroup in match.SuccessfulMatchGroups().Skip(1))
			                {
				                if (matchGroup.Name.StartsWith(kRgxOpenLevelGroupPrefix))
				                {
					                var validMatchLevels = matchGroup.Name.Substring(3);
					                var index = validMatchLevels.IndexOf(currentQuoteLevel.ToString(),
						                StringComparison.Ordinal);
					                if (index >= 0)
					                {
						                if (currentQuoteLevel > 0)
						                {
							                continuer = atParaStart && index > 0;
											if (!continuer)
												AddAnnotation(match, true);
						                }

						                start = match.Index;
										if (!continuer)
											currentQuoteLevel++;
					                }
					                break;
				                }

				                if (matchGroup.Name.StartsWith(kRgxCloserGroupPrefix))
				                {
					                var validMatchLevels = matchGroup.Name.Substring(3);
					                if (validMatchLevels.Contains(currentQuoteLevel.ToString()))
					                {
						                AddAnnotation(match);
						                if (--currentQuoteLevel > 0)
							                start = match.Index + match.Length;
					                }
					                break;
				                }

				                if (matchGroup.Name == kRgxFinalGroup && currentQuoteLevel > 0)
				                {
					                AddAnnotation(match);
					                break;
				                }
			                
							}

			                atParaStart = false;
		                }
		                
		                atParaStart = false;
	                }
                }

                m_currentBookAnnotations[c] = new ChapterAnnotationInfo(annotations,
	                currentQuoteLevel, startingQuoteLevel);

                startingQuoteLevel = currentQuoteLevel;
            }
        }

        #endregion

        #region Annotation class
        private sealed class Annotation : IPluginAnnotation
        {
            public Annotation(IScriptureTextSelection selection, string styleName)
            {
                ScriptureSelection = selection;
                StyleName = styleName;
            }
            public IScriptureTextSelection ScriptureSelection { get; }

            public string StyleName { get; }

            public string Icon => null;

            public string IconToolTipText => null;
            
            public bool Click(MouseButton button, bool onIcon, Point location)
            {
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

            public string SelectedText { get; }

            public int Offset { get; }

            public string BeforeContext { get; }

            public string AfterContext { get; }

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
        }
        #endregion
    }
}
