class Calculator
{
    static Stack<double> Numbers;
    static Stack<Operation> Operations;

    public static double Calculate(List<Token> tokens)
    {
        Numbers = new();
        Operations = new();

        foreach (var token in tokens)
        {
            switch (token)
            {
                case Number number:
                    Numbers.Push(number.Value);
                    break;

                case Operation operation:
                    while (Operations.TryPeek(out var top) && top.IsPriorThan(operation))
                    {
                        Calculate(top);
                        Operations.Pop();
                    }

                    Operations.Push(operation);
                    break;
            }
        }

        while (Operations.TryPop(out var operation))
        {
            Calculate(operation);
        }

        return Numbers.Pop();
    }

    static void Calculate(Operation operation)
    {
        if (operation is BinaryOperation binary)
        {
            var right = Numbers.Pop();
            var left = Numbers.Pop();
            var result = binary.Calculate(left, right);
            Numbers.Push(result);
        }
    }
}