namespace Chipsoft.Assignments.EPDConsole;

public static class DateTimeUtils
{
    /// <summary>
    /// Returns an array of dates pointing to midnight of the date.
    /// </summary>
    /// <param name="startDate">The starting date.</param>
    /// <param name="count">How many dates have to be generated. Minimum value of 1.</param>
    /// <returns>An array of DateTime objects representing midnight on consecutive days.</returns>
    /// <exception cref="ArgumentOutOfRangeException">When <paramref name="count"/> is lesser than 1.</exception>
    public static DateTime[] GetMidnightDates(DateTime startDate, int count)
    {
        if (count < 1) throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than or equal to 1.");
        
        DateTime[] result = new DateTime[count];
        result[0] = startDate.Date;
        
        for (int i = 1; i < count; i++)
        {
            result[i] = startDate.AddDays(i).Date;
        }
        
        return result;
    }
    
    /// <summary>
    /// Returns an array of dates incremented by a specified interval.
    /// </summary>
    /// <param name="startDate">The starting date.</param>
    /// <param name="count">How many increments to generate. Minimum value of 1.</param>
    /// <param name="interval">The time span between each increment.</param>
    /// <returns>An array of DateTime objects representing incremented dates.</returns>
    /// <exception cref="ArgumentOutOfRangeException">When <paramref name="count"/> is lesser than 1.</exception>
    public static DateTime[] GetIncrementedDates(DateTime startDate, int count, TimeSpan interval)
    {
        if (count < 1) throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than or equal to 1.");
        
        DateTime[] result = new DateTime[count];
        result[0] = startDate;
        
        for (int i = 1; i < count; i++)
        {
            var newDate = startDate.Add(interval * i);
            if (newDate.Date > startDate.Date) break;
            
            result[i] = startDate.Add(interval * i);
        }
        
        return result;
    }
    
    /// <summary>
    /// Round the given date to the next given interval.
    /// </summary>
    /// <param name="dateTime">The date to round.</param>
    /// <param name="interval">The interval to use.</param>
    /// <returns>The rounded date.</returns>
    public static DateTime RoundToNextInterval(DateTime dateTime, TimeSpan interval)
    {
        // Calculate the difference in ticks between the current time and the next interval boundary
        var ticksDifference = (interval.Ticks - (dateTime.Ticks % interval.Ticks)) % interval.Ticks;
        
        // Add the ticks difference to the current date to get the rounded date
        return dateTime.AddTicks(ticksDifference);
    }
}