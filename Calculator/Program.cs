var tokens = Tokenizer.Tokenize("5 * (cos(3.14))");
var result = Calculator.Calculate(tokens);
Console.WriteLine(result);