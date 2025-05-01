using System;
using System.Collections.Generic;

public static class RPNEvaluator
{
    public static int Evaluate(string expression, Dictionary<string, int> variables)
    {
        string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Stack<int> stack = new Stack<int>();

        foreach (var token in tokens)
        {
            if (int.TryParse(token, out int val))
            {
                // Token is a number
                stack.Push(val);
            }
            else if (variables.ContainsKey(token))
            {
                // Token is a variable name (look up its value)
                stack.Push(variables[token]);
            }
            else
            {
                // Token is an operator
                switch (token)
                {
                    case "+":
                        stack.Push(stack.Pop() + stack.Pop());
                        break;

                    case "-":
                        {
                            int b = stack.Pop();
                            int a = stack.Pop();
                            stack.Push(a - b);
                            break;
                        }

                    case "*":
                        stack.Push(stack.Pop() * stack.Pop());
                        break;

                    case "/":
                        {
                            int b = stack.Pop();
                            int a = stack.Pop();
                            stack.Push(b == 0 ? 0 : a / b);
                            break;
                        }

                    case "%":
                        {
                            int b = stack.Pop();
                            int a = stack.Pop();
                            stack.Push(a % b);
                            break;
                        }

                    default:
                        throw new Exception($"Unrecognized token: '{token}'");
                }
            }
        }

        if (stack.Count != 1)
            throw new Exception("Invalid RPN expression: stack did not end with exactly one result");

        return stack.Pop();
    }
}
