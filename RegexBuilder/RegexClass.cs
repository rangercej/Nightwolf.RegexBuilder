namespace Nightwolf.Regex
{
    using System.Collections.Generic;

    /// <summary>
    /// Regular expression character classes
    /// </summary>
    public enum CharacterClass
    {
        Any,
        AlphaUpper,
        AlphaLower,
        Digit,
        NotDigit,
        Whitespace,
        NotWhitespace,
        WordBoundary,
        WordChar,
        NotWordChar,
    }

    /// <summary>Regex value of character classes</summary>
    internal static class CharacterClassValues
    {
        internal static readonly Dictionary<CharacterClass, string> CharClasses = new Dictionary<CharacterClass, string> {
            { CharacterClass.Any, "." },
            { CharacterClass.AlphaLower, "[a-z]" },
            { CharacterClass.AlphaUpper, "[A-Z]" },
            { CharacterClass.Digit, "\\d" },
            { CharacterClass.NotDigit, "\\D" },
            { CharacterClass.Whitespace, "\\s" },
            { CharacterClass.NotWhitespace, "\\S" },
            { CharacterClass.WordBoundary, "\\b" },
            { CharacterClass.WordChar, "\\w" },
            { CharacterClass.NotWordChar, "\\W" }
        };
    }
}
