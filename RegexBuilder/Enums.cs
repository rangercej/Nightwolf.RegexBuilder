namespace Nightwolf.Regex
{
    /// <summary>Repeats enum</summary>
    public enum RegexRepeats
    {
        ZeroOrOne,
        ZeroOrMore,
        One,
        OneOrMore
    }

    /// <summary>Scopes for the regular expression</summary>
    public enum RegexScope
    {
        Anywhere,
        StartsWith,
        EndsWith,
        FullLine
    }
}
