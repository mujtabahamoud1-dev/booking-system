namespace BookingSystem.API.Shared.Database;

// Turns what someone typed into a search box into an ILIKE pattern.
//
// The wildcards are escaped rather than honoured: a patient searching "50%" means
// the characters, not "match anything", and an admin typing "_" should not get
// every row back. Queries using these patterns must say ESCAPE '\' so Postgres
// reads the escapes the same way.
public static class SqlSearch
{
    public static string? ToPattern(string? term)
    {
        if (string.IsNullOrWhiteSpace(term)) return null;

        var escaped = term.Trim()
            .Replace(@"\", @"\\")
            .Replace("%", @"\%")
            .Replace("_", @"\_");

        return $"%{escaped}%";
    }
}
