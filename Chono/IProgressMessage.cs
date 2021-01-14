// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2021, SIL International.
// <copyright from='2021' to='2021' company='SIL International'>
//		Copyright (c) 2021, SIL International.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
//
// File: IProgressMessage.cs
// ---------------------------------------------------------------------------------------------
namespace SIL.Chono
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Interface to allow a process to display messages showing progress.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public interface IProgressMessage
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the message to display.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		string Message { set; }
	}
}
