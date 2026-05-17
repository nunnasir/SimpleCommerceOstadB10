using Microsoft.AspNetCore.Http;

namespace SimpleCommerce.BLL.Services.Interfaces;

public interface IProductImageService
{
    Task<string> SaveAsync(IFormFile file);
    void DeleteIfExists(string? relativePath);
}
