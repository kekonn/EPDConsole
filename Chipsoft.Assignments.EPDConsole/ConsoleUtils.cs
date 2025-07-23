using Spectre.Console;

namespace Chipsoft.Assignments.EPDConsole;

/// <summary>
/// Various utility functions to handle console tasks.
/// </summary>
public static class ConsoleUtils
{
    /// <summary>
    /// Will prompt the user to enter a string value and repeat this until a non-null, non-empty value is entered.
    /// </summary>
    /// <param name="prompt">The prompt to show the user.</param>
    /// <returns>A non-null, non-empty string value.</returns>
    public static string ReadRequiredString(string prompt)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>(prompt)
                .Validate(i => !string.IsNullOrWhiteSpace(i), "This field is required")
        );
    }

    /// <summary>
    /// Prompts for a valid INSZ number.
    /// </summary>
    /// <returns></returns>
    public static string ReadRequiredInsz()
    {
        var InszPrompt = new TextPrompt<string>("INSZ: ")
            .Validate(SsnValidator.IsValid, "Please enter a valid INSZ");
        
        return SsnValidator.CleanInsz(AnsiConsole.Prompt(InszPrompt));
    }
}