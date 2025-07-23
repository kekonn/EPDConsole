namespace Chipsoft.Assignments.EPDConsole;

using System.Text.RegularExpressions;

public static class SsnValidator
{
    /// <summary>
    /// Validates a Belgian Social Security Number (NISS/INSZ)
    /// </summary>
    /// <param name="ssn">The SSN to validate (with or without separators)</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool IsValid(string ssn)
    {
        if (string.IsNullOrWhiteSpace(ssn))
            return false;

        // Remove all non-digit characters
        var cleanSsn = CleanInsz(ssn);

        // Must be exactly 11 digits
        if (cleanSsn.Length != 11)
            return false;

        // Extract components
        var birthDate = cleanSsn.Substring(0, 6);
        var sequential = cleanSsn.Substring(6, 3);

        // Validate birth date format (YYMMDD)
        if (!IsValidBirthDate(birthDate))
            return false;

        // Validate sequential number (001-997 for men, 002-998 for women)
        if (!IsValidSequentialNumber(sequential))
            return false;

        // Validate check digits using modulo 97
        return IsValidCheckDigits(cleanSsn);
    }

    private static bool IsValidBirthDate(string birthDate)
    {
        if (birthDate.Length != 6)
            return false;

        if (!int.TryParse(birthDate.AsSpan(2, 2), out var month) || month < 1 || month > 12)
            return false;

        return int.TryParse(birthDate.AsSpan(4, 2), out var day) && day >= 1 && day <= 31;
    }

    private static bool IsValidSequentialNumber(string sequential)
    {
        if (!int.TryParse(sequential, out var seqNum))
            return false;

        return seqNum is >= 1 and <= 998;
    }

    private static bool IsValidCheckDigits(string ssn)
    {
        var first9Digits = ssn.Substring(0, 9);
        var checkDigits = ssn.Substring(9, 2);

        if (!long.TryParse(first9Digits, out var number) || 
            !int.TryParse(checkDigits, out var providedCheck))
            return false;

        // For people born in 2000 or later, add 2000000000 to the first 9 digits
        var numberFor2000Plus = number + 2000000000;
        
        var calculatedCheck = 97 - (int)(number % 97);
        var calculatedCheck2000Plus = 97 - (int)(numberFor2000Plus % 97);

        return providedCheck == calculatedCheck || providedCheck == calculatedCheck2000Plus;
    }

    /// <summary>
    /// Removes anything but digits from the given string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The digits found in the input string.</returns>
    public static string CleanInsz(string input)
    {
        // Remove all non-digit characters
        return Regex.Replace(input, @"\D", "");
    }
}
