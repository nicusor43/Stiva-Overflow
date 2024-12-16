
namespace ProiectStackOverflow.Services;

public class ImageStorageService : IImageStorageService
{
    private readonly string _storagePath;

    public ImageStorageService(IConfiguration configuration)
    {
        _storagePath = configuration["ImageStoragePath"]; // Get path from appsettings.json
    }

    public async Task<string> SaveImageAsync(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return null;
        }

        // Generate unique file name (consider image type if needed)
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), _storagePath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return fileName; // Return the unique file name
    }

    public string GetImageFilePath(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty; // Handle cases where no photo is available
        }
        return Path.Combine(_storagePath, fileName);
    }

    public async Task DeleteImageAsync(string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), _storagePath, fileName);

        if (File.Exists(filePath))
        {
            // Use Task.Run to avoid blocking the main thread
            await Task.Run(() => File.Delete(filePath));
        }
    }
}