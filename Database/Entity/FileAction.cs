using System.Diagnostics.CodeAnalysis;

namespace Database.Entity;

/// <summary>
/// The name of the enums are values. 
/// </summary>
public enum FileAction
{
    /// <summary>
    /// File was created.
    /// </summary>
    Create,
    
    /// <summary>
    /// File content was updated.
    /// </summary>
    ContentUpdate,
    
    /// <summary>
    /// File name was updated.
    /// </summary>
    NameUpdate,
    
    /// <summary>
    /// File was deleted.
    /// </summary>
    Delete,
}

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Enum and extensions kept together")]
public static class FileActionExtensions
{
    private static readonly Dictionary<string, FileAction> StringMap = Enum.GetValues<FileAction>()
        .ToDictionary(ConvertToActionString, v => v);
    
    public static string ConvertToActionString(this FileAction action)
        => ConvertToActionString(action.ToString());
    
    public static FileAction ConvertToActionEnum(this string action)
        => StringMap[action];
    
    private static string ConvertToActionString(string actionString)
        => actionString.ToUpperInvariant().Trim();
}