using System.Text;
using Godot;

namespace AXTanks.Scripts.Extensions;

public static class NodeExtension
{
    public static Error RpcServerOnly(this Node node, StringName methodName, params 
        Variant[] args)
    {
        return node.RpcId(1, methodName, args);
    }

    public static Variant CallDeferredExt(this Node node, string nameOfMethod, params Variant[] args)
    {
        return node.CallDeferred(ConvertToSnakeCase(nameOfMethod), args);
    }

    private static string ConvertToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        string methodName = input[(input.LastIndexOf(':') + 1)..];

        StringBuilder result = new StringBuilder();
        result.Append(char.ToLower(methodName[0]));

        for (int i = 1; i < methodName.Length; i++)
        {
            if (char.IsUpper(methodName[i]))
            {
                result.Append('_');
                result.Append(char.ToLower(methodName[i]));
            }
            else
            {
                result.Append(methodName[i]);
            }
        }

        return result.ToString();
    }
}