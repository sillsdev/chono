using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SIL.Quotelighter
{
	internal static class MatchExtensions
	{
		internal static IEnumerable<Group> SuccessfulMatchGroups(this Match match)
		{
			foreach (Group group in match.Groups)
			{
				if (group.Success)
					yield return group;
			}
		}
	}
}
