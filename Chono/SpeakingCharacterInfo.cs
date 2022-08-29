// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2022, SIL International.
// <copyright from='2022' to='2022' company='SIL International'>
//		Copyright (c) 2022, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// ---------------------------------------------------------------------------------------------
using System.Windows.Forms;
using GlyssenEngine.Character;

namespace SIL.Chono
{
	internal class SpeakingCharacterInfo : ListViewItem
	{
		public SpeakingCharacterInfo(CharacterSpeakingMode character) :
			base(new [] {character.Character, character.Alias, character.Delivery,
				character.QuoteType.ToString(), character.ParallelPassageReferences,
				character.LocalizedCharacter})
		{
		}
	}
}
