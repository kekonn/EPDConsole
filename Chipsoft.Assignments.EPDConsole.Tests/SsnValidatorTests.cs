namespace Chipsoft.Assignments.EPDConsole.Tests;

public class SsnValidatorTests
{
    [Theory]
    [InlineData("89120542718")]
    [InlineData("89.12.05-427.18")]
    public void SsnValidator_ShouldAcceptValidSSN(string input)
    {
        // Act
        var result = SsnValidator.IsValid(input);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData("89120542719")]
    [InlineData("89.12.05-427.19")]
    [InlineData("89120442718")]
    public void SsnValidator_ShouldNotAcceptInvalidSSN(string input)
    {
        // Act
        var result = SsnValidator.IsValid(input);
        
        // Assert
        Assert.False(result);
    }
}