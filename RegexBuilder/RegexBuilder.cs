namespace Nightwolf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public sealed class RegexBuilder
    {
        private readonly List<string> regex = new List<string>(10);

        private readonly Scope scope;

        public RegexBuilder(Scope scope = Scope.Anywhere) 
        {
            this.scope = scope;
        }

        public override string ToString()
        {
            var prefix = (this.scope == Scope.StartsWith) || (this.scope == Scope.FullLine) ? "^" : "";
            var suffix = (this.scope == Scope.EndsWith) || (this.scope == Scope.FullLine) ? "$" : "";
            
            return string.Format("{0}{1}{2}", prefix, string.Join("", this.regex), suffix);
        }

        public Regex ToRegex(RegexOptions options = RegexOptions.None)
        {
            return options == RegexOptions.None
                ? new Regex(this.ToString())
                : new Regex(this.ToString(), options);
        }

        public RegexBuilder Literal(string s, bool replaceWhitespace = false)
        {
            var escaped = Regex.Escape(s);
            if (replaceWhitespace)
            {
                this.AddExpression("(?:{0})", escaped.Replace(@"\ ", @"\s+"));
            } 
            else
            {
                this.AddExpression("(?:{0})", escaped);
            }

            return this;
        }

        public RegexBuilder AnyOf(IEnumerable<RegexBuilder> regex)
        {
            this.AddExpression("(?:{0})", string.Join("|", regex.Select(x => x.ToString()).ToArray()));
            return this;
        }

        public RegexBuilder AnyOf(IEnumerable<string> values)
        {
            this.AddExpression("(?:{0})", string.Join("|", values.Select(x => Regex.Escape(x))));
            return this;
        }

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

            this.AddExpression("[{0}]", vals.ToString());
            return this;
        }

        public RegexBuilder Include(RegexBuilder expression)
        {
            this.AddExpression("(?:{0})", expression.ToString());
            return this;
        }

        public RegexBuilder Capture()
        {
            var idx = this.regex.Count - 1;
            var expression = this.regex[idx];
            this.regex[idx] = string.Format("({0})", expression);

            return this;
        }

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

        private void AddExpression(string format, params string[] expression)
        {
            var s = string.Format(format, expression);
            this.regex.Add(s);
        }

        public enum Repeats
        {
            ZeroOrOne,
            ZeroOrMore,
            One,
            OneOrMore
        }

        public enum Scope
        {
            Anywhere,
            StartsWith,
            EndsWith,
            FullLine
        }
    }
}
