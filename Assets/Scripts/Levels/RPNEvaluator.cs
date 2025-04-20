public static class RPNEvaluator
{
    public static int Evaluate(string expression, int baseValue, int wave)
    {
        string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Stack<int> stack = new Stack<int>();

        foreach (var token in tokens)
        {
            if (token == "wave")
            {
                stack.Push(wave);
            }
            else if (token == "base")
            {
                stack.Push(baseValue);
            }
            else if (int.TryParse(token, out int val))
            {
                stack.Push(val);
            }
            else
            {
                switch (token)
                {
                    case "+": stack.Push(stack.Pop() + stack.Pop()); break;
                    case "-": { int b = stack.Pop(); int a = stack.Pop(); stack.Push(a - b); break; }
                    case "*": stack.Push(stack.Pop() * stack.Pop()); break;
                    case "/": { int b = stack.Pop(); int a = stack.Pop(); stack.Push(b == 0 ? 0 : a / b); break; }
                    case "%": { int b = stack.Pop(); int a = stack.Pop(); stack.Push(a % b); break; }
                    default: throw new Exception($"Unrecognized token: '{token}'");
                }
            }
        }

        return stack.Pop();
    }
}
