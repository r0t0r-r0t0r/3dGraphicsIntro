using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Disunity.WorldManaging.Loading
{
    internal static class MatchUtils
    {
        public static IEnumerable<T> ParseGroups<T>(this Match match, int count, Func<string, T> parser)
        {
            return match.Groups.Cast<Group>().Skip(1).Take(count).Select(g => parser(g.Value));
        }
    }
}