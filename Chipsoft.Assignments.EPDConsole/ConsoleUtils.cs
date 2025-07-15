namespace Chipsoft.Assignments.EPDConsole;

/// <summary>
/// Various utility functions to handle console tasks.
/// </summary>
internal static class ConsoleUtils
{
    /// <summary>
    /// Will prompt the user to enter a string value and repeat this until a non-null, non-empty value is entered.
    /// </summary>
    /// <param name="prompt">The prompt to show the user.</param>
    /// <returns>A non-null, non-empty string value.</returns>
    public static string ReadRequiredString(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();
        
        while (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("This field is required.");
            Console.Write(prompt);
            input = Console.ReadLine();
        }

        return input;
    }

    /// <summary>
    /// Generates a paged list in a console and allows you to pick a numbered item from said list.
    /// </summary>
    /// <param name="models">The models to generate a list for. Uses ToString for the display value.</param>
    /// <param name="pageSize">The amount of items on a single page. Defaults to 5. Must be at least 1.</param>
    /// <typeparam name="TModel">The type of model to paginate. Must be a class.</typeparam>
    /// <returns>The selected model or null if the selection was cancelled.</returns>
    public static TModel? ListPicker<TModel>(ICollection<TModel> models, int pageSize = 5)
        where TModel : class
    {
        if (models == null) throw new ArgumentNullException(nameof(models));
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        
        // todo: calculate page and item size, list pages and controls at the bottom of page
        // use readkey, not readline
    }
}