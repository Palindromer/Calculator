using System.Text.RegularExpressions;

static class Tokenizer
{
    static readonly List<(string Pattern, Func<string, Token> ToToken)> Patterns = new()
    {
        ("\\d+(\\.\\d+)?(e[+-]?\\d+)?", match => new Number(double.Parse(match))),

        ("-",   _ => new BinaryOperation((x, y) => x - y, 1)),
        ("\\+", _ => new BinaryOperation((x, y) => x + y, 1)),
        ("\\*", _ => new BinaryOperation((x, y) => x * y, 2)),
        ("/",   _ => new BinaryOperation((x, y) => x / y, 2)),

        ("&",   _ => new BinaryOperation(Math.Pow, 3, RightAssociative: true)),
    };

    static readonly Regex Regex;
    const string GroupPrefix = "name_";

    static Tokenizer()
    {
        var patterns = Patterns.Select((item, index) => $"(?<{GroupPrefix}{index}>{item.Pattern})");
        var pattern = string.Join('|', patterns);
        Regex = new Regex(pattern, RegexOptions.IgnoreCase);
    }

    public static List<Token> Tokenize(string expression)
    {
        var tokens = Regex
            .Matches(expression)
            .Select(match => match.Groups.Values.Last(gr => gr.Success))
            .Select(gr =>
            {
                var indexStr = gr.Name.Substring(GroupPrefix.Length);
                var index = int.Parse(indexStr);
                return Patterns[index].ToToken(gr.Value);
            })
            .ToList();

        return tokens;
    }
}

record Token;

record Number(double Value) : Token;

record Operation(int Priority, bool RightAssociative = false) : Token
{
    public bool IsPriorThan(Operation next)
    {
        if (Priority > next.Priority)
        {
            return true;
        }

        if (Priority == next.Priority && !next.RightAssociative)
        {
            return true;
        }

        return false;
    }
}


record BinaryOperation(Func<double, double, double> Calculate, int Priority, bool RightAssociative = false)
    : Operation(Priority, RightAssociative);

