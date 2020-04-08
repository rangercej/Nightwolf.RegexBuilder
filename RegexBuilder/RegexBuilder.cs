﻿namespace Nightwolf.Regex
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Regular expression fluent builder class
    /// </summary>
    public sealed class RegexBuilder
    {
        /// <summary>Holds each component of the regex as it's built</summary>
        private readonly List<string> regex = new List<string>(10);

        /// <summary>Anywhere in a line, full line, something else?</summary>
        private readonly Scope scope;

        /// <summary>Negate flag.</summary>
        private AutoResetFlag negate;

        /// <summary>
        /// Create a new builder with the defined scope
        /// </summary>
        /// <param name="scope"></param>
        public RegexBuilder(Scope scope = Scope.Anywhere) 
        {
            this.negate = new AutoResetFlag(false);
            this.scope = scope;
        }

        /// <summary>
        /// Get the current regular expression string
        /// </summary>
        /// <returns>Regular expression as a string</returns>
        public override string ToString()
        {
            var prefix = (this.scope == Scope.StartsWith) || (this.scope == Scope.FullLine) ? "^" : "";
            var suffix = (this.scope == Scope.EndsWith) || (this.scope == Scope.FullLine) ? "$" : "";
            
            return string.Format("{0}{1}{2}", prefix, string.Join("", this.regex), suffix);
        }

        /// <summary>
        /// Return a regular expression object containing the current regex.
        /// </summary>
        /// <param name="options">Regular expression options</param>
        /// <returns>Instance of regular expression class</returns>
        public Regex ToRegex(RegexOptions options = RegexOptions.None)
        {
            return options == RegexOptions.None
                ? new Regex(this.ToString())
                : new Regex(this.ToString(), options);
        }

        /// <summary>
        /// Add a literal string
        /// </summary>
        /// <param name="s">String to add</param>
        /// <param name="replaceWhitespace">When true, replaces all space chars with a regex to match all whitespace</param>
        /// <returns>This regular expression builder</returns>
        public RegexBuilder Literal(string s, bool replaceWhitespace = false)
        {
            var escaped = Regex.Escape(s);
            if (replaceWhitespace)
            {
                this.AddExpression(this.negate.Read() ? "(?!{0})" : "(?:{0})", escaped.Replace(@"\ ", @"\s+"));
            } 
            else
            {
                this.AddExpression(this.negate.Read() ? "(?!{0})" : "(?:{0})", escaped);
            }

            return this;
        }

        /// <summary>
        /// Add a regex alternate for a set of regular expressions
        /// </summary>
        /// <param name="regex">Set of alternate regex to match</param>
        /// <returns>This regular expression builder</returns>
        public RegexBuilder AnyOf(IEnumerable<RegexBuilder> regex)
        {
            this.AddExpression(this.negate.Read() ? "(?!{0})" : "(?:{0})", string.Join("|", regex.Select(x => x.ToString()).ToArray()));
            return this;
        }

        /// <summary>
        /// Add a regex alternate for a set of string values
        /// </summary>
        /// <param name="values">Set of alternate strings to match</param>
        /// <returns>This regular expression builder</returns>
        public RegexBuilder AnyOf(IEnumerable<string> values)
        {
            this.AddExpression(this.negate.Read() ? "(?!(?:{0}))" : "(?:{0})", string.Join("|", values.Select(x => Regex.Escape(x))));
            return this;
        }

        /// <summary>
        /// Add a regex alternate for a set of characters
        /// </summary>
        /// <param name="values">Set of characters to match</param>
        /// <returns>This regular expression builder</returns>
        public RegexBuilder AnyOf(IEnumerable<char> values)
        {
            var vals = new StringBuilder();
            foreach (var ch in values)
            {
                if (ch == '\\' || ch == ']' || ch == '^' || ch == '-')
                {
                    vals.AppendFormat(@"\{0}", ch);
                } 
                else
                {
                    vals.Append(ch);
                }
            }

            this.AddExpression(this.negate.Read() ? "[^{0}]" : "[{0}]", vals.ToString());
            return this;
        }

        /// <summary>
        /// Insert an existing regular expression
        /// </summary>
        /// <param name="values">Expression to insert</param>
        /// <returns>This regular expression builder</returns>
        public RegexBuilder Include(RegexBuilder expression)
        {
            this.AddExpression(this.negate.Read() ? "(?!{0})" : "(?:{0})", expression.ToString());
            return this;
        }

        /// <summary>
        /// Negate the next expression
        /// </summary>
        /// <returns>This regular expression builder</returns>
        public RegexBuilder Not()
        {
            this.negate.Set();
            return this;
        }

        /// <summary>
        /// Capture the previous expression into a capture group
        /// </summary>
        /// <returns>This regular expression builder</returns>
        public RegexBuilder Capture()
        {
            var idx = this.regex.Count - 1;
            var expression = this.regex[idx];
            this.regex[idx] = string.Format("({0})", expression);

            return this;
        }

        /// <summary>
        /// How many of the previous expression should match
        /// </summary>
        /// <param name="repeats">Repeat indicator</param>
        /// <returns>This regular expression builder</returns>
        public RegexBuilder Limit(Repeats repeats)
        {
            var idx = this.regex.Count - 1;

            switch (repeats)
            {
                case Repeats.ZeroOrOne: this.regex[idx] += "?"; break;
                case Repeats.ZeroOrMore: this.regex[idx] += "*"; break;
                case Repeats.OneOrMore: this.regex[idx] += "+"; break;
                default: break;
            }

            return this;
        }

        /// <summary>
        /// How many of the previous expression should match
        /// </summary>
        /// <param name="min">Minimum repeats</param>
        /// <param name="max">Maximum repeats</param>
        /// <returns>This regular expression builder</returns>
        public RegexBuilder Limit(int? min, int? max)
        {
            var idx = this.regex.Count - 1;
            if (min == null && max == null)
            {
                throw new ArgumentException("min and max cannot both be null");
            }
            else if (min != null && max != null)
            {
                if (min.Value > max.Value)
                {
                    throw new ArgumentException("min cannot be larger than max");
                }
                else
                {
                    this.regex[idx] += string.Format("{{{0},{1}}}", min, max);
                }
            } 
            else if (min == null && max != null)
            {
                this.regex[idx] += string.Format("{{,{0}}}", max);
            } 
            else if (min != null && max == null)
            {
                this.regex[idx] += string.Format("{{{0},}}", min);
            }

            return this;
        }

        /// <summary>
        /// Add the expression value to the list
        /// </summary>
        /// <param name="format">Regular expression value</param>
        /// <param name="expression">Parameters for the expression</param>
        private void AddExpression(string format, params string[] expression)
        {
            var s = string.Format(format, expression);
            this.regex.Add(s);
        }

        /// <summary>Repeats enum</summary>
        public enum Repeats
        {
            ZeroOrOne,
            ZeroOrMore,
            One,
            OneOrMore
        }

        /// <summary>Scopes for the regular expression</summary>
        public enum Scope
        {
            Anywhere,
            StartsWith,
            EndsWith,
            FullLine
        }
    }
}
