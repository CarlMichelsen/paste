namespace Presentation.Repository;

public interface IFileContent
{
    byte[] Bytes { get; }
    
    string MimeType { get; }
    
    string ChecksumAlgorithm { get; }
    
    string Checksum { get; }
}