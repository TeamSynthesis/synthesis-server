namespace synthesis.api.Helpers;

public static class CodeGenerator
{
    private static readonly Random _random = new Random();

    public static string GenerateCode()
    {
        return (char)_random.Next(97, 127) + "-" + _random.Next(100000, 999999).ToString("D6");
    }
}