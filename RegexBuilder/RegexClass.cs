namespace Nightwolf.Regex
{
    using System.Collections.Generic;

    /// <summary>
    /// Regular expression character classes
    /// </summary>
    public enum CharacterClass
    {
        AlphaUpper,
        AlphaLower,
        Digits,
        Whitespace
    }

    /// <summary>Regex value of character classes</summary>
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
