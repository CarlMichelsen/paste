using System.Text.RegularExpressions;
using Database.Util;

namespace Application.Validation;

public static partial class FileNameSanitizer
{
    private static readonly Regex InvalidCharsRegex = InvalidCharsRegexGenerated();
    private static readonly Regex MultipleDotsRegex = MultipleDotsRegexGenerated();
    
    public static FileName SanitizedFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("Filename cannot be empty.", nameof(fileName));
        }
        
        // Normalize to lowercase and trim
        var sanitized = fileName.ToLowerInvariant().Trim();
        
        // Replace invalid characters with underscores
        sanitized = InvalidCharsRegex.Replace(sanitized, "_");
        
        // Replace multiple consecutive dots with single dot
        sanitized = MultipleDotsRegex.Replace(sanitized, ".");
        
        // Find name and extension parts
        var lastDotIndex = sanitized.LastIndexOf('.');
        var hasExtension = lastDotIndex > 0 && lastDotIndex < sanitized.Length - 1;
        
        if (!hasExtension)
        {
            sanitized = $"{sanitized}.txt";
        }
        
        return new FileName(sanitized);
    }

    [GeneratedRegex(@"[^a-z0-9._-]", RegexOptions.Compiled)]
    private static partial Regex InvalidCharsRegexGenerated();
    
    [GeneratedRegex(@"\.{2,}", RegexOptions.Compiled)]
    private static partial Regex MultipleDotsRegexGenerated();
}