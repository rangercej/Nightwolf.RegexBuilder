using System;
using System.Collections.Generic;
using System.Text;

namespace Nightwolf.Regex
{
    public enum CharacterClass
    {
        AlphaUpper,
        AlphaLower,
        Digits,
        Whitespace
    }

    internal static class CharacterClassValues
    {
        internal static readonly Dictionary<CharacterClass, string> CharClasses = new Dictionary<CharacterClass, string> {
            { CharacterClass.AlphaLower, "a-z" },
            { CharacterClass.AlphaUpper, "A-Z" },
            { CharacterClass.Digits, "\\d" },
            { CharacterClass.Whitespace, "\\s" }
        };
    }
}
