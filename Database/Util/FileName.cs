using System.Text.RegularExpressions;

namespace Database.Util;

public record FileName
{
    private const int MinNameLength = 2;
    private const int MinExtensionLength = 1;
    
    private static readonly Regex ValidFileNameRegex = new(
        $@"^[a-z0-9._-]{{{MinNameLength},}}\.[a-z0-9_-]{{{MinExtensionLength},}}$", 
        RegexOptions.Compiled);
    
    public FileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Filename cannot be empty.",
                nameof(name));
        }

        var normalizedName = name.ToLowerInvariant().Trim();
        if (!ValidFileNameRegex.IsMatch(normalizedName))
        {
            throw new ArgumentException(
                "Filename must contain only lowercase ASCII letters, numbers, dots, underscores, or hyphens. " +
                "No spaces or slashes allowed. Must have a file extension.",
                nameof(name));
        }
        
        if (name.Length > 255)
        {
            throw new ArgumentException(
                "Filename must not be over 255 characters.",
                nameof(name));
        }

        // Find the last dot for extension parsing
        var lastDotIndex = normalizedName.LastIndexOf('.');
        if (lastDotIndex <= 0 || lastDotIndex == normalizedName.Length - 1)
        {
            throw new ArgumentException(
                "Filename must have a valid extension.",
                nameof(name));
        }

        var nameBeforeExtension = normalizedName[..lastDotIndex];
        var extension = normalizedName[(lastDotIndex + 1)..];

        // Validate name length (minimum characters)
        if (nameBeforeExtension.Length < MinNameLength)
        {
            throw new ArgumentException(
                $"Name part before extension must be at least {MinNameLength} characters long.",
                nameof(name));
        }

        // Validate extension length (minimum 1 character - already ensured by regex)
        if (extension.Length < MinExtensionLength)
        {
            throw new ArgumentException(
                $"File extension must be at least {MinExtensionLength} character long.",
                nameof(name));
        }

        this.Value = normalizedName;
        this.NameWithoutExtension = nameBeforeExtension;
        this.Extension = extension;
    }
    
    public string Value { get; init; }
    
    public string NameWithoutExtension { get; init; }
    
    public string Extension { get; init; }
    
    public static implicit operator string(FileName name) => name.Value;
    
    public static implicit operator FileName(string value) => new(value);

    public override int GetHashCode() => this.Value.GetHashCode();

    public override string ToString() => this.Value;
}