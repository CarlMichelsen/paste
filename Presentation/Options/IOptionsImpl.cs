namespace Presentation.Options;

public interface IOptionsImpl
{
    /// <summary>
    /// Gets the location in  config for this options' implementation.
    /// </summary>
    static abstract string SectionName { get; }
}