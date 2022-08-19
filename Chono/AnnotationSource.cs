﻿// ---------------------------------------------------------------------------------------------
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
        private readonly IQuotationMarkInfo quotationMarks;
        private readonly List<Regex> findMarksRegexes = new List<Regex>();
        
        public AnnotationSource(IProject project)
        {
            quotationMarks = project.Language.QuotationMarkInfo;
            if (quotationMarks == null)
                return;

            HashSet<string> allMarks = new HashSet<string>();

            // NOTE: This doesn't actually work correctly if multiple levels contain the same quotation marks,
            // but figuring out the nesting is a lot more work then we want to attempt for this demo plugin.
            
            for (int level = 0; level < quotationMarks.PrimaryLevels.Count; level++)
            {
                IQuotationMarkLevel lev = quotationMarks.PrimaryLevels[level];

                StringBuilder bldr = new StringBuilder();

                if (!allMarks.Contains(lev.Opener))
                {
                    allMarks.Add(lev.Opener);
                    bldr.Append(Regex.Escape(lev.Opener));
                }

                if (!allMarks.Contains(lev.Closer))
                {
                    allMarks.Add(lev.Closer);
                    if (bldr.Length > 0)
                        bldr.Append('|');
                    bldr.Append(Regex.Escape(lev.Closer));
                }

                if (!allMarks.Contains(lev.Continuer))
                {
                    allMarks.Add(lev.Continuer);
                    if (bldr.Length > 0)
                        bldr.Append('|');
                    bldr.Append(Regex.Escape(lev.Continuer));
                }

                findMarksRegexes.Add(bldr.Length > 0 ? new Regex(bldr.ToString(), RegexOptions.Compiled) : null);
            }
        }

        #region Implementation of IPluginAnnotationSource
        public event EventHandler AnnotationsChanged;

        public bool MaintainSelectionsOnWordBoundaries => false;

        public IReadOnlyList<AnnotationStyle> GetStyleInfo(double zoom)
        {
            return new[]
            {
                new AnnotationStyle("quote0", "background-color:MediumOrchid;"),
                new AnnotationStyle("quote1", "background-color:Cyan;"),
                new AnnotationStyle("quote2", "background-color:Chartreuse;"),
                new AnnotationStyle("quote3", "background-color:RosyBrown;")
            };
        }

        public IReadOnlyList<IPluginAnnotation> GetAnnotations(IVerseRef verseRef, string usfm)
        {
            List<IPluginAnnotation> annotations = new List<IPluginAnnotation>();
            for (int level = 0; level < findMarksRegexes.Count; level++)
            {
                if (findMarksRegexes[level] == null)
                    continue;
                foreach (Match match in findMarksRegexes[level].Matches(usfm))
                {
                    Selection sel = new Selection(match.Value, usfm.Substring(0, match.Index),
                        usfm.Substring(match.Index + match.Length), verseRef, match.Index);
                    annotations.Add(new Annotation(sel, "quote" + level));
                }
            }

            return annotations;
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
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
