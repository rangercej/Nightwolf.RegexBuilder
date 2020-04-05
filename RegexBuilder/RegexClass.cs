using System;
using System.Collections.Generic;
using System.Text;

namespace Nightwolf
{
    public sealed class RegexClass
    {
        public static readonly string AlphaUpper = "A-Z";
        public static readonly string AlphaLower = "a-z";
        public static readonly string Alpha = string.Format("{0}{1}", AlphaUpper, AlphaLower);
        public static readonly string Digits = "\\d";
        public static readonly string Whitespace = "\\s";
    }
}
