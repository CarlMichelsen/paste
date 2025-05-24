using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using MimeDetective;
using MimeDetective.Definitions;
using Presentation.Repository;

namespace Application;

public class FileContent : IFileContent
{
    private static readonly IContentInspector ContentInspector;
    
    // Static constructor to initialize ContentInspector once
    static FileContent()
    {
        ContentInspector = new ContentInspectorBuilder
        {
            Definitions = DefaultDefinitions.All(),
        }.Build();
    }
    
    public FileContent(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File cannot be null or empty", nameof(file));
        }

        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        this.Bytes = memoryStream.ToArray();
        
        this.MimeType = file.ContentType;
        this.Checksum = CalculateChecksum(this.Bytes);
    }
    
    public FileContent(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
        {
            throw new ArgumentException("Byte array cannot be null or empty", nameof(bytes));
        }

        this.Bytes = bytes;
        this.MimeType = DetectMimeType(bytes);
        this.Checksum = CalculateChecksum(bytes);
    }
    
    public byte[] Bytes { get; }
    
    public string MimeType { get; }
    
    public string ChecksumAlgorithm { get; } = "SHA256";
    
    public string Checksum { get; }
    
    private static string DetectMimeType(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        var inspectionResult = ContentInspector.Inspect(stream);
        var firstMatch = inspectionResult.ByMimeType().FirstOrDefault();
        return firstMatch?.MimeType ?? "application/octet-stream";
    }
    
    private static string CalculateChecksum(byte[] bytes)
    {
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}