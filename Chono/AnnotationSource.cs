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
using System.Text;
using System.Text.RegularExpressions;
using Paratext.PluginInterfaces;

namespace SIL.Chono
{
    internal class AnnotationSource : IPluginAnnotationSource
    {
	    private const string kQuoteStylePrefix = "quote";
	    private const string kError = "red";

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
        private Dictionary<int, ChapterAnnotationInfo> m_currentBookAnnotations;

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


            // TODO: Handle secondary levels also.
            for (int level = 0; level < quotationMarks.PrimaryLevels.Count;)
            {
                IQuotationMarkLevel lev = quotationMarks.PrimaryLevels[level];
                int i;

                if (allMarks.TryGetValue(lev.Opener, out i))
                {
	                bldr.Insert(i, level);
	                allMarks[lev.Opener]++;
                }
                else
                {
                    if (level > currentLevelInBuilder)
                    {
                        if (bldr.Length > 0)
	                        bldr.Append(")");
                        bldr.Append("(?<lev");
                        bldr.Append(level);
                        allMarks[lev.Opener] = bldr.Length;
                        bldr.Append(">");
                        currentLevelInBuilder = level;
                    }
                    else
                    {
	                    bldr.Append("|");
                    }
                    bldr.Append(Regex.Escape(lev.Opener));
                }

                level++;

                if (allMarks.TryGetValue(lev.Closer, out i))
                {
	                bldr.Insert(i, level);
	                allMarks[lev.Closer]++;
                }
                else
                {
	                bldr.Append("|");
	                bldr.Append("(?<lev");
	                bldr.Append(level);
	                allMarks[lev.Closer] = bldr.Length;
	                bldr.Append(">");
	                bldr.Append(Regex.Escape(lev.Closer));
					currentLevelInBuilder = level;
                }

                if (!string.IsNullOrEmpty(lev.Continuer) && allMarks.TryGetValue(lev.Continuer, out i))
                {
	                if (Int32.Parse(bldr[i - 1].ToString()) != level)
	                {
		                bldr.Insert(i, level);
		                allMarks[lev.Continuer]++;
	                }
                }
                else
                {
	                if (level > currentLevelInBuilder)
	                {
		                bldr.Append("(?<lev");
		                bldr.Append(level);
		                allMarks[lev.Continuer] = bldr.Length;
		                bldr.Append(">");
		                currentLevelInBuilder = level;
	                }
	                else
	                {
		                bldr.Append("|");
	                }
	                bldr.Append(Regex.Escape(lev.Continuer));
                }
                bldr.Append(")");

                m_findMarksRegex = new Regex(bldr.ToString(), RegexOptions.Compiled);
            }
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
	        m_currentBookAnnotations = null;
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

	        return m_currentBookAnnotations[chapter].Annotations;
        }

        private void PopulateAnnotations(int bookNum, int chapter)
        {
            // First see if any "holes" in a preceding chapter require us to
            // rescan.
            int startingQuoteLevel = 0;
            for (int c = 1; c < chapter; c++)
            {
                if (m_currentBookAnnotations.TryGetValue(c, out var annotationInfo))
                {
	                if (annotationInfo.Annotations != null)
	                {
		                startingQuoteLevel = annotationInfo.QuoteLevelAtEndOfChapter;
		                continue;
	                }

	                var annotations = new List<IPluginAnnotation>();
	                foreach (var tok in m_project.GetUSFMTokens(bookNum, c))
	                {
		                if (tok.IsScripture)
                        {
                            if (tok is IUSFMTextToken text)
                            {
	                            foreach (var ch in text.Text)
	                            {
		                            
	                            }
                            }
                        }
	                }
                }
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
                IVerseRef verseRef, int offset)
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
