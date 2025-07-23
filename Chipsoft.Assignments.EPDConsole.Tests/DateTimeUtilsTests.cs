namespace Chipsoft.Assignments.EPDConsole.Tests;


public class DateTimeUtilsTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(14)]
    public void GenerateMidnightDates_GeneratesExactAmount(int amount)
    {
        // Act
        var dates = DateTimeUtils.GetMidnightDates(DateTime.Now, amount);
        
        // Assert
        Assert.Equal(amount, dates.Length);
    }
    
    [Fact]
    public void GenerateMidnightDates_GeneratedDatesStartWithStartpoint()
    {
        // Arrange
        var startDate = DateTime.Now.Date;
        
        // Act
        var dates = DateTimeUtils.GetMidnightDates(startDate, 7);
        
        // Assert
        Assert.Equal(7, dates.Length);
        Assert.Equal(startDate, dates[0]);
    }
    
    [Fact]
    public void GetIncrementedDates_GeneratedDatesStartWithStartpoint()
    {
        // Arrange
        var startDate = DateTime.Now.Date;
        
        // Act
        var dates = DateTimeUtils.GetIncrementedDates(startDate, 7, TimeSpan.FromMinutes(15));
        
        // Assert
        Assert.Equal(7, dates.Length);
        Assert.Equal(startDate, dates[0]);
    }

    
    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(14)]
    public void GetIncrementedDates_GeneratesExactAmount(int amount)
    {
        // Act
        var dates = DateTimeUtils.GetIncrementedDates(DateTime.Now, amount, TimeSpan.FromMinutes(15));
        
        // Assert
        Assert.Equal(amount, dates.Length);
    }
}