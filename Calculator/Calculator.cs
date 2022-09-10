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

                case RightParenthesis:
                    CalculateParentheses();
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
            if (operation is LeftParenthesis)
            {
                throw new ArgumentException("The expression has unbalanced parentheses");
            }

            Calculate(operation);
        }

        return Numbers.Pop();
    }

    static void CalculateParentheses()
    {
        while (true)
        {
            // Throw the exception if there is no operation.
            var top = Operations.Pop();
            if (top is LeftParenthesis)
            {
                return;
            }

            Calculate(top);
        }
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
        else if (operation is UnaryOperation unary)
        {
            var right = Numbers.Pop();
            var result = unary.Calculate(right);
            Numbers.Push(result);
        }
    }
}