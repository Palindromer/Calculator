var tokens = Tokenizer.Tokenize("asdfa 5 * -4&-3 - 2");
var result = Calculator.Calculate(tokens);
Console.WriteLine(result);