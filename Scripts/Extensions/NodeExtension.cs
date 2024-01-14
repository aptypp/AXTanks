using System.Text;
using Godot;

namespace AXTanks.Scripts.Extensions;

public static class NodeExtension
{
    public static Variant CallDeferredExt(this Node node, string nameOfMethod, params Variant[] args)
    {
        return node.CallDeferred(ConvertToSnakeCase(nameOfMethod[(nameOfMethod.LastIndexOf(':') + 1)..]), args);
    }

    private static string ConvertToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        StringBuilder result = new StringBuilder();
        result.Append(char.ToLower(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                result.Append('_');
                result.Append(char.ToLower(input[i]));
            }
            else
            {
                result.Append(input[i]);
            }
        }

        return result.ToString();
    }
}