using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SimpleCommerce.BLL.Services.Interfaces;

namespace SimpleCommerce.BLL.Services.Implementations;

public class ProductImageService : IProductImageService
{
    public const string RelativeFolder = "images/products";

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp"
    };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private readonly IWebHostEnvironment _environment;

    public ProductImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveAsync(IFormFile file)
    {
        if (file.Length == 0)
            throw new InvalidOperationException("Image file is empty.");

        if (file.Length > MaxFileSizeBytes)
            throw new InvalidOperationException("Image file must be 5 MB or smaller.");

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Only JPG, PNG, GIF, and WEBP images are allowed.");

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var directory = Path.Combine(_environment.WebRootPath, RelativeFolder);
        Directory.CreateDirectory(directory);

        var fullPath = Path.Combine(directory, fileName);
        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream);

        return $"{RelativeFolder}/{fileName}".Replace('\\', '/');
    }

    public void DeleteIfExists(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return;

        var fullPath = Path.Combine(
            _environment.WebRootPath,
            relativePath.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
