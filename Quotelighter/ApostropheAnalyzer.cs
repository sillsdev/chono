using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SIL.Quotelighter
{
	/// <summary>
	/// Object to encapsulate the logic to apply language-specific rules to distinguish between
	/// a word-final apostrophe and a closing single quote mark.  Note that if single closing
	/// curly quotes are not used as quotation mark punctuation, then there is no need to
	/// use this class.
	/// </summary>
	/// <remarks>Since this behavior is language-specific and should affect how Paratext
	/// does quotation mark checking, it ultimately belongs in Paratext.</remarks>
	public class ApostropheAnalyzer
	{
		public enum SpecialTreatmentRule
		{
			/// <summary>
			/// This is the best option for languages that do not use apostrophes at all.
			/// </summary>
			Never,
			/// <summary>
			/// This is the best option for languages that can have word-final curly apostrophes
			/// that might frequently immediately precede punctuation. This is probably rare, but
			/// could occur if the apostrophe is used as a glottal or some other speech sound.
			/// </summary>
			WordFinal,
			/// <summary>
			/// This is the best option for languages that can have word-final curly apostrophes
			/// that would seldom or never occur before a punctuation character. For example, in
			/// English, this can happen, but it is very rare: "Those bicycles are her kids’."
			/// </summary>
			WordFinalExceptBeforePunctuation,
		}

		private Regex m_regexIsApostrophe = null;

		// Apostrophe treatment rules. TODO: Add UI for this (preferably to Paratext eventually)
		public SpecialTreatmentRule TreatSingleClosingCurlyQuoteAsApostrophe { get;  }
		public IReadOnlyCollection<string> LettersThanCanPrecedeApostrophes { get;  }

		/// <summary>
		/// Constructs an <see cref="ApostropheAnalyzer"/> object.
		/// </summary>
		/// <param name="treatSingleClosingCurlyQuoteAsApostropheRule">The rule indicating when to
		/// consider that a single closing curly quote that is detected as a possible closing quote
		/// might instead be an apostrophe.</param>
		/// <param name="lettersThanCanPrecedeApostrophes">Optional limited set of characters that
		/// might occur at the end of a word right before a final apostrophe. Typically this should
		/// be a single character, but the logic allows for multiple characters that function as a
		/// single unit in the language (e.g., diphthongs or surrogate pairs).</param>
		/// <exception cref="ArgumentException"></exception>
		public ApostropheAnalyzer(SpecialTreatmentRule treatSingleClosingCurlyQuoteAsApostropheRule,
			params string [] lettersThanCanPrecedeApostrophes)
		{
			TreatSingleClosingCurlyQuoteAsApostrophe = treatSingleClosingCurlyQuoteAsApostropheRule;
			if (TreatSingleClosingCurlyQuoteAsApostrophe == SpecialTreatmentRule.Never)
			{
				if (lettersThanCanPrecedeApostrophes != null && lettersThanCanPrecedeApostrophes.Any())
				{
					throw new ArgumentException($"The option {treatSingleClosingCurlyQuoteAsApostropheRule}" +
						" is not compatible with providing a list of letters that can precede apostrophes.",
						nameof(treatSingleClosingCurlyQuoteAsApostropheRule));
				}

				return;
			}

			LettersThanCanPrecedeApostrophes = lettersThanCanPrecedeApostrophes?.ToArray();

			var sbPattern = new StringBuilder(@"(?<=\p{L}|\p{M})"); // Must be preceded by a word-forming character.
			if (LettersThanCanPrecedeApostrophes != null && LettersThanCanPrecedeApostrophes.Any())
			{
				sbPattern.Append("(?<=(");
				sbPattern.Append(string.Join("|", LettersThanCanPrecedeApostrophes.Select(Regex.Escape)));
				sbPattern.Append("))");
			}

			sbPattern.Append(@"’(?!\w)");

			// Note: word boundary (\b) does not work because regex does not consider the
			// apostrophe/single curly quote to be word-forming.

			if (TreatSingleClosingCurlyQuoteAsApostrophe == SpecialTreatmentRule.WordFinalExceptBeforePunctuation)
				sbPattern.Append(@"(\P{P}|’|$)");

			m_regexIsApostrophe = new Regex(sbPattern.ToString(), RegexOptions.Compiled | RegexOptions.IgnoreCase);
		}

		public bool IsApostrophe(string containingText, int characterPos)
		{
			// For the purpose of implementing this, technically we don't actually need to
			// verify that the index is in range, but in practice if a caller ever provides
			// and argument that is out of range, something is wrong, and just returning false
			// probably isn't the most helpful thing we can do.
			if (characterPos < 0 || characterPos >= containingText.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(characterPos),
					$"Must be a character position in {nameof(containingText)}.");
			}

			if (TreatSingleClosingCurlyQuoteAsApostrophe == SpecialTreatmentRule.Never)
				return false;

			return m_regexIsApostrophe.Matches(containingText).Cast<Match>()
				.Any(match => match.Index == characterPos);
		}
	}
}
