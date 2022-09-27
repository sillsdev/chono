using System;
using System.Linq;
using NUnit.Framework;
using static SIL.Quotelighter.ApostropheAnalyzer;

namespace SIL.Quotelighter
{
	[TestFixture]
	public class ApostropheAnalyzerTests
	{
		[TestCase("Why even ask?", 0)]
		[TestCase("Don’t do that", 3)]
		[TestCase("Those bikes are her kids’", 24)]
		[TestCase("Those bikes are her kids’.", 24)]
		public void IsApostrophe_Never_ReturnsFalse(string containingText, int characterPos)
		{
			var analyzer = new ApostropheAnalyzer(SpecialTreatmentRule.Never);
			Assert.That(analyzer.IsApostrophe(containingText, characterPos), Is.False);
		}

		[TestCase(SpecialTreatmentRule.Never, "Why even ask?", 100)]
		[TestCase(SpecialTreatmentRule.WordFinal, "Why even ask?", -1)]
		[TestCase(SpecialTreatmentRule.WordFinalExceptBeforePunctuation, "Don’t do that", 13)]
		[TestCase(SpecialTreatmentRule.WordFinal, "Those bikes are her kids’", 25)]
		public void IsApostrophe_CharacterPosOutOfRange_Throws(SpecialTreatmentRule rule,
			string containingText, int characterPos)
		{
			var analyzer = new ApostropheAnalyzer(rule);
			Assert.That(() => analyzer.IsApostrophe(containingText, characterPos), Throws.TypeOf<ArgumentOutOfRangeException>());
		}

		[TestCase(SpecialTreatmentRule.WordFinalExceptBeforePunctuation, "Why even ask?", 0)]
		[TestCase(SpecialTreatmentRule.WordFinal, "Why eve’ ask?", 11)]
		[TestCase(SpecialTreatmentRule.WordFinalExceptBeforePunctuation, "Why eve’ ask?", 12)]
		public void IsApostrophe_CharacterPosIsNotClosingCurly_ReturnsFalse(
			SpecialTreatmentRule rule, string containingText, int characterPos)
		{
			var analyzer = new ApostropheAnalyzer(SpecialTreatmentRule.WordFinalExceptBeforePunctuation);
			Assert.That(analyzer.IsApostrophe(containingText, characterPos), Is.False);
		}
		
		[TestCase("Why eve’ ask?", 7, ExpectedResult = true)]
		[TestCase("’What?", 0, ExpectedResult = false)]
		[TestCase("’ Why?", 0, ExpectedResult = false)]
		[TestCase("Don’t do that", 3, ExpectedResult = false)]
		[TestCase("Those bikes are her ‘kids’’.", 25, ExpectedResult = true)]
		[TestCase("Those bikes are her ‘kids’’.", 26, ExpectedResult = false)]
		[TestCase("Those bikes are her kids’", 24, ExpectedResult = true)]
		[TestCase("Those bikes are her kids’.", 24, ExpectedResult = true)]
		public bool IsApostrophe_WordFinal_ReturnsExpectedResult(string containingText,
			int characterPos)
		{
			Assert.That(containingText[characterPos], Is.EqualTo('’'), "Sanity check");

			var analyzer = new ApostropheAnalyzer(SpecialTreatmentRule.WordFinal);
			return analyzer.IsApostrophe(containingText, characterPos);
		}

		[TestCase("Why eve’ ask?", 7, ExpectedResult = true)]
		[TestCase("Why eve\u0300’ ask?", 8, ExpectedResult = true)]
		[TestCase("Why eve’, ask?", 7, ExpectedResult = false)]
		[TestCase("Why eve’+ ask?", 7, ExpectedResult = true)] // symbol, not punctuation
		[TestCase("’What?", 0, ExpectedResult = false)]
		[TestCase("’ Why?", 0, ExpectedResult = false)]
		[TestCase("Don’t do that", 3, ExpectedResult = false)]
		[TestCase("Those bikes are her ‘kids’’.", 25, ExpectedResult = true)]
		[TestCase("Those bikes are her ‘kids\u0303’’.", 26, ExpectedResult = true)]
		[TestCase("Those bikes are her ‘kids’’.", 26, ExpectedResult = false)]
		[TestCase("Those bikes are her kids’", 24, ExpectedResult = true)]
		[TestCase("Those bikes are her kids’.", 24, ExpectedResult = false)]
		public bool IsApostrophe_WordFinalExceptBeforePunctuation_ReturnsExpectedResult(
			string containingText, int characterPos)
		{
			Assert.That(containingText[characterPos], Is.EqualTo('’'), "Sanity check");

			var analyzer = new ApostropheAnalyzer(SpecialTreatmentRule.WordFinalExceptBeforePunctuation);
			return analyzer.IsApostrophe(containingText, characterPos);
		}
		
		[TestCase("Why eve’ ask?", 7, ExpectedResult = false)]
		[TestCase("’What?", 0, ExpectedResult = false)]
		[TestCase("’ Why?", 0, ExpectedResult = false)]
		[TestCase("Don’t do that", 3, ExpectedResult = false)]
		[TestCase("Those bikes are her ‘kids’’.", 25, ExpectedResult = true)]
		[TestCase("Those bikes are her ‘kids\u0303’’.", 26, "s", "s\u0303", ExpectedResult = true)]
		[TestCase("Those bikes are her ‘kids’’.", 26, ExpectedResult = false)]
		[TestCase("Those bikes are her kids’", 24, ExpectedResult = true)]
		[TestCase("Those bikes are her kids’.", 24, ExpectedResult = true)]
		[TestCase("Those bikes are her KIDS’.", 24, ExpectedResult = true)]
		public bool IsApostrophe_AfterS_ReturnsExpectedResult(string containingText,
			int characterPos, params string [] additionalCharacters)
		{
			Assert.That(containingText[characterPos], Is.EqualTo('’'), "Sanity check");

			var analyzer = additionalCharacters.Any() ?
				new ApostropheAnalyzer(SpecialTreatmentRule.WordFinal, additionalCharacters) :
				new ApostropheAnalyzer(SpecialTreatmentRule.WordFinal, "s");
			return analyzer.IsApostrophe(containingText, characterPos);
		}
		
		[TestCase("Why eve’ ask?", 7, ExpectedResult = false)]
		[TestCase("’What?", 0, ExpectedResult = false)]
		[TestCase("’ Why?", 0, ExpectedResult = false)]
		[TestCase("Don’t do that", 3, ExpectedResult = false)]
		[TestCase("Those bikes are her ‘kids’’.", 25, ExpectedResult = true)]
		[TestCase("Those are her kids’ bikes.", 18, ExpectedResult = true)]
		[TestCase("Those bikes are her ‘kidt’’.", 25, ExpectedResult = false)]
		[TestCase("Those bikes are her ‘kids\u0303’’.", 26, "s", "s\u0303", ExpectedResult = true)]
		[TestCase("Those bikes are her ‘kids’’.", 26, ExpectedResult = false)]
		[TestCase("Those bikes are her kids’", 24, ExpectedResult = true)]
		[TestCase("Those bikes are her kids’.", 24, ExpectedResult = false)]
		[TestCase("Those bikes are her KIDS’.", 24, ExpectedResult = false)]
		public bool IsApostrophe_AfterSButNotBeforePunctuation_ReturnsExpectedResult(string containingText,
			int characterPos, params string [] additionalCharacters)
		{
			Assert.That(containingText[characterPos], Is.EqualTo('’'));
			var analyzer = additionalCharacters.Any() ?
				new ApostropheAnalyzer(SpecialTreatmentRule.WordFinalExceptBeforePunctuation, additionalCharacters) :
				new ApostropheAnalyzer(SpecialTreatmentRule.WordFinalExceptBeforePunctuation, "s");
			return analyzer.IsApostrophe(containingText, characterPos);
		}
	}
}
