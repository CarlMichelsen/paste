using Database.Util;

namespace Test.Unit;

public class FileNameTests
{
    [Theory]
    [InlineData("hello", typeof(ArgumentException))]
    [InlineData("hello.txt", null)]
    [InlineData("||||.txt", typeof(ArgumentException))]
    [InlineData("d.txt", typeof(ArgumentException))]
    [InlineData("hello.", typeof(ArgumentException))]
    [InlineData(".", typeof(ArgumentException))]
    [InlineData("", typeof(ArgumentException))]
    [InlineData("   ", typeof(ArgumentException))]
    [InlineData(null!, typeof(ArgumentException))]
    [InlineData("memes.c", null)]
    [InlineData("abc.d", null)]
    public void FileNameMustHaveExtension(string fileNameString, Type? expectedExceptionType)
    {
        // Arrange
        // Act
        FileName? fileName = null;
        Exception? actualException = null;
        if (expectedExceptionType is not null)
        {
            actualException = Assert.Throws(expectedExceptionType, () => fileName = new FileName(fileNameString));
        }
        else
        {
            fileName = new FileName(fileNameString);
        }
        
        // Assert
        Assert.Equal(expectedExceptionType, actualException?.GetType());
        if (expectedExceptionType is null)
        {
            Assert.NotNull(fileName);
        }
    }
    
    [Theory]
    [InlineData(1024, true)]
    [InlineData(1025, true)]
    [InlineData(255, false)]
    [InlineData(256, true)]
    [InlineData(12, false)]
    public void FileNameMustBeNoLongerThan255Characters(int fileNameLength, bool shouldThrow)
    {
        // Arrange
        const string ext = ".txt";
        var stringName = new string('a', fileNameLength - ext.Length) + ext;
        
        // Act
        // Assert
        if (shouldThrow)
        {
            Assert.Throws<ArgumentException>(() => new FileName(stringName));
        }
        else
        {
            Assert.Equal(stringName, new FileName(stringName));
        }
    }
}