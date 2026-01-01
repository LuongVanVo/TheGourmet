using Microsoft.AspNetCore.Http;

namespace TheGourmet.Application.Features.Products.Commands.CreateProduct;

public class FormFileWrapper : IFormFile
{
    private readonly Stream _stream;

    public FormFileWrapper(Stream stream, string fileName, string contentType)
    {
        _stream = stream;
        FileName = fileName;
        ContentType = contentType;
        Length = stream.Length;
    }

    public string ContentType { get; }
    public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{FileName}\"";
    public IHeaderDictionary Headers => new HeaderDictionary();
    public long Length { get; }
    public string Name => "file";
    public string FileName { get; }

    public void CopyTo(Stream target) => _stream.CopyTo(target);

    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) 
        => _stream.CopyToAsync(target, cancellationToken);

    public Stream OpenReadStream() => _stream;
}