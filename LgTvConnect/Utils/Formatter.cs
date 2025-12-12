namespace LgTvConnect.Utils;

internal static class Formatter
{
    /// <summary>
    /// This method creates a string with a fixed length from a number. So a 345 can be converted to a "00345"
    /// </summary>
    /// <param name="number">The number you want to have leading zeros in front of</param>
    /// <param name="n">The length of the string</param>
    /// <returns></returns>
    public static string IntToStringWithLeadingZeros(int number, int n)
    {
        var result = number.ToString();
        var length = result.Length;

        for (var i = length; i < n; i++)
        {
            result = "0" + result;
        }

        return result;
    }
}