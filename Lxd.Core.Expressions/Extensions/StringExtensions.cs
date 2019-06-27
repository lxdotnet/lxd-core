
using System.Text.RegularExpressions;

namespace Lxd.Core.Expressions.Extensions
{
    internal static class StringExtensions
    {
        public static string SquareTagsToHtml(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            //return Regex.Replace(value, @"\[(?'tag'[A-Za-z/]+?)\]", match =>
            //    string.Format("<{0}>", match.Groups["tag"].Value));
            return Regex.Replace(value, @"\[(?'tag'.+?)\]", match =>
                string.Format("<{0}>", match.Groups["tag"].Value));
        }
    }
}
