// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2021' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.   
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// ---------------------------------------------------------------------------------------------
using AddInSideViews;
using SIL.Scripture;

namespace SIL.Chono
{
    public class ScrVers : IScrVers
    {
        private readonly IHost m_host;

        public ScrVers(IHost host, string versificationName)
        {
            m_host = host;
            Name = versificationName;
        }

	    public int GetLastBook()
	    {
		    return Canon.LastBook;
	    }

	    public int GetLastChapter(int bookNum)
        {
            return m_host.GetLastChapter(bookNum, Name);
        }

        public int GetLastVerse(int bookNum, int chapterNum)
        {
            return m_host.GetLastVerse(bookNum, chapterNum, Name);
        }

        public int ChangeVersification(int reference, IScrVers scrVersSource)
        {
            return this.Equals(scrVersSource) ? reference :
                m_host.ChangeVersification(reference, scrVersSource.Name, Name);
        }

	    public bool IsExcluded(int bbbcccvvv)
	    {
		    throw new System.NotImplementedException();
	    }

	    public VerseRef? FirstIncludedVerse(int bookNum, int chapterNum)
	    {
		    throw new System.NotImplementedException();
	    }

	    public string[] VerseSegments(int bbbcccvvv)
	    {
		    throw new System.NotImplementedException();
	    }

	    public void ChangeVersification(ref VerseRef reference)
	    {
		    throw new System.NotImplementedException();
	    }

	    public bool ChangeVersificationWithRanges(VerseRef reference, out VerseRef newReference)
	    {
		    throw new System.NotImplementedException();
	    }

	    public string Name { get; }
    }
}
