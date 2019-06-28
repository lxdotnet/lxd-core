using System;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lxd.Core.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<Match> MatchesOf(this string input, Regex regex)
        {
            #region param validation

            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            if (null == regex)
                throw new ArgumentNullException("regex");

            #endregion

            Match match = regex.Match(input);
            while (match.Success)
            {
                yield return match;
                match = match.NextMatch();
            }
        }

        public static IEnumerable<Match> MatchesOf(this string input, string pattern)
        {
            return input.MatchesOf(new Regex(pattern));
        }

        public static string Replace(this string input, string pattern, MatchEvaluator evaluator)
        {
            return string.IsNullOrEmpty(input) ? input : new Regex(pattern).Replace(input, evaluator);
        }

        public static string ToLowerCamelCase(this string input)
        {
            string[] tmp = input.Split('.');
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = Char.ToLowerInvariant(tmp[i][0]) + tmp[i].Substring(1);
            }
            return String.Join(".", tmp);
        }

        //public static string SquareTagsToHtml(this string value)
        //{
        //    if (string.IsNullOrEmpty(value))
        //        return value;

        //    //return Regex.Replace(value, @"\[(?'tag'[A-Za-z/]+?)\]", match =>
        //    //    string.Format("<{0}>", match.Groups["tag"].Value));
        //    return Regex.Replace(value, @"\[(?'tag'.+?)\]", match =>
        //        string.Format("<{0}>", match.Groups["tag"].Value));
        //}

        //public static T FromFile<T>(this string filePath)
        //{
        //    using (var reader = new XmlTextReader(filePath))
        //    {
        //        var serializer = new XmlSerializer(typeof(T));
        //        return (T)serializer.Deserialize(reader);
        //    }
        //}

        public static T To<T>(this string s, CultureInfo culture)
        {
            return (T)s.To(typeof(T), culture);
        }

        public static T To<T>(this string s)
        {
            return s.To<T>(CultureInfo.InvariantCulture);
        }

        public static object To(this string s, Type desired)
        {
            return s.To(desired, CultureInfo.InvariantCulture);
        }

        public static object To(this string s, Type desired, CultureInfo culture)
        {
            if (typeof(string) == desired)
                return s;

            if (typeof(Enum).IsAssignableFrom(desired))
            {
                try
                {
                    return Enum.Parse(desired, s, true); // supports everything including parsing enums with flags starting with .net 4.0
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(string.Format("{0} cannot be converted to {1}", s, desired.FullName), e);
                }
            }

            if (desired.IsGenericType && typeof(Nullable<>) == desired.GetGenericTypeDefinition())
            {
                var underlyingType = Nullable.GetUnderlyingType(desired);

                if (String.IsNullOrEmpty(s))
                    return null;

                var value = s.To(underlyingType, culture);
                return Activator.CreateInstance(desired, value);
            }

            // bool has no .Parse:
            if (typeof(bool) == desired)
            {
                if (string.Equals(bool.TrueString, s, StringComparison.InvariantCultureIgnoreCase))
                    return true;

                if (string.Equals(bool.FalseString, s, StringComparison.InvariantCultureIgnoreCase))
                    return false;

                decimal dec;
                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out dec))
                    return Convert.ToBoolean(dec);

                throw new InvalidOperationException("The value does not represent a boolean: " + s);
            }

            if (string.IsNullOrEmpty(s))
                return desired.DefaultValue();

            // look if the type has the method Parse:
            MethodInfo parse = desired.GetMethod("Parse", new [] { typeof(string), typeof(IFormatProvider) });

            if (parse != null)
                return parse.Invoke(null, new object[] { s, culture });

            parse = desired.GetMethod("Parse", new[] { typeof(string) });
            if (parse != null)
                return parse.Invoke(null, new object[] { s });

            // look if there is a constructor accepting a string:
            var ctor = desired.GetConstructor(new[] { typeof(string) });
            if (ctor != null)
            {
                return Activator.CreateInstance(desired, s);
            }

            if (desired == typeof(object))
                return s;

            if (typeof(IConvertible).IsAssignableFrom(desired)) // Convert relies on IConvertible
            {
                // last desperate attempt:
                return Convert.ChangeType(s, desired, culture);
            }

            return desired.DefaultValue();
        }

        public static ConstantExpression ToExpression(this string s, Type desired)
        {
            object c = s.To(desired);
            return Expression.Constant(c);
        }

        public static string CrLf(this string s)
        {
            return s + Environment.NewLine;
        }

        public static string CrLf(this string s, int count)
        {
            return Enumerable.Range(0, count).Aggregate(s, (current, i) => current.CrLf());
        }

        public static string[] SplitBy(this string s, params char[] separators)
        {
            return s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitBy(this string s, params string[] separators)
        {
            return s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string Strip(this string s, string toStrip)
        {
            return Regex.Replace(s, toStrip + "$", string.Empty);
        }

        public static string CSharpify(this string identifier)
        {
            return identifier.Replace(@"[\.]", match => ""); // todo: implement properly
        }
    }
}