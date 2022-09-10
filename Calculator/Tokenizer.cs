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

        ("log", _ => new UnaryOperation(Math.Log10, 4)),
        ("ln",  _ => new UnaryOperation(Math.Log, 4)),
        ("exp", _ => new UnaryOperation(Math.Exp, 4)),
        ("sqrt",_ => new UnaryOperation(Math.Sqrt, 4)),
        ("abs", _ => new UnaryOperation(Math.Abs, 4)),
        ("atan",_ => new UnaryOperation(Math.Atan, 4)),
        ("acos",_ => new UnaryOperation(Math.Acos, 4)),
        ("asin",_ => new UnaryOperation(Math.Asin, 4)),
        ("sinh",_ => new UnaryOperation(Math.Sinh, 4)),
        ("cosh",_ => new UnaryOperation(Math.Cosh, 4)),
        ("tanh",_ => new UnaryOperation(Math.Tanh, 4)),
        ("tan", _ => new UnaryOperation(Math.Tan, 4)),
        ("sin", _ => new UnaryOperation(Math.Sin, 4)),
        ("cos", _ => new UnaryOperation(Math.Cos, 4)),

        ("\\(", _ => new LeftParenthesis()),
        ("\\)", _ => new RightParenthesis()),
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

record Operation(int? Priority = null, bool RightAssociative = false) : Token
{
    public bool IsPriorThan(Operation next)
    {
        if (Priority is null)
        {
            return false;
        }

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

record BinaryOperation(Func<double, double, double> Calculate, int? Priority, bool RightAssociative = false)
    : Operation(Priority, RightAssociative);

record UnaryOperation(Func<double, double> Calculate, int? Priority)
    : Operation(Priority, RightAssociative: true);

record LeftParenthesis : Operation;
record RightParenthesis : Operation;