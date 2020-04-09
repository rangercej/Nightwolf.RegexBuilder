# Nightwolf.RegexBuilder
Library to ease the pain of creating regular expressions, using a fluent-like syntax. This library is still 
under development, but should be usable in it's current form.

For example, given something like a SQL Statement that looks like:

    CREATE TABLE dbo.Bob (a INT IDENTITY(1,1))
    CREATE TABLE Bob (a INT IDENTITY(1,1))

And extracting the table name ("Bob"), the regex would look something like:

    ^CREATE TABLE (?:dbo\.)?([A-Za-z]+) \(

or if we wanted to be looser with allowing different whitespace: 

    ^CREATE\s+TABLE\s+(?:dbo\.)?([A-Za-z]+)\s+\(

Instead, we can write something like:

    var regex = new RegexBuilder(Scope.StartsWith)
        .Literal("CREATE TABLE ", true)
        .Literal("dbo.").Repeat(RegexRepeats.ZeroOrOne)
        .AnyOf(CharacterClass.AlphaLower).Repeat(RegexRepeats.OneOrMore).Capture()
        .Literal(" ", true)
        .ToRegex(RegexOptions.IgnoreCase);

Yes, it's more verbose, but it is more descriptive of what we're trying to capture and look for.
