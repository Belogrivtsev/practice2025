namespace task01;

public static class StringExtensions  
{
    public static bool IsPalindrome(this string input)
    {
        input = input.ToLower();
        for (int i = 0; i < input.Length; i++)
        {
            if (Char.IsPunctuation(input[i]) || Char.IsWhiteSpace(input[i]))
            {
                input = input.Remove(i, 1);
                i--;
            }
        }

        char[] stringBySymbols = input.ToCharArray();
        char[] stringBySymbolsReversed = (char[])stringBySymbols.Clone();
        Array.Reverse(stringBySymbolsReversed);

        if (input == "") { return false; }

        return stringBySymbolsReversed.SequenceEqual(stringBySymbols);
    }
}