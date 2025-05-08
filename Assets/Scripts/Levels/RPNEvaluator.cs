using System;
using System.Collections.Generic;

public static class RPNEvaluator
{
    public static float Evaluate(string expression, Dictionary<string, float> variables)
    {
        string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Stack<float> stack = new Stack<float>();

        foreach (var token in tokens)
        {
            if (float.TryParse(token, out float val))
            {
                stack.Push(val);
            }
            else if (variables.ContainsKey(token))
            {
                stack.Push(variables[token]);
            }
            else
            {
                switch (token)
                {
                    case "+":
                        stack.Push(stack.Pop() + stack.Pop());
                        break;

                    case "-":
                        {
                            float b = stack.Pop();
                            float a = stack.Pop();
                            stack.Push(a - b);
                            break;
                        }

                    case "*":
                        stack.Push(stack.Pop() * stack.Pop());
                        break;

                    case "/":
                        {
                            float b = stack.Pop();
                            float a = stack.Pop();
                            stack.Push(b == 0f ? 0f : a / b);
                            break;
                        }

                    case "%":
                        {
                            float b = stack.Pop();
                            float a = stack.Pop();
                            stack.Push(a % b);
                            break;
                        }

                    default:
                        throw new Exception($"Unrecognized token: '{token}'");
                }
            }
        }

        if (stack.Count != 1)
        {
            throw new Exception(expression);
        }

        return stack.Pop();
    }
}
