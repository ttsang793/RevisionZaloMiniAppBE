using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace backend.Services;

public class UploadService
{
    private readonly Cloudinary _cloudinary;

    public UploadService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("Vui lòng chọn hình ảnh.");

        if (file.Length >= 3 * 1024 * 1024) throw new ArgumentException("Hình phải nhỏ hơn 3MB.");

        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
        if (!allowedTypes.Contains(file.ContentType.ToLower())) throw new ArgumentException("Chỉ cho phép file JPG, PNG.");

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        return result;
    }

    public async Task<RawUploadResult> UploadPDFAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("Vui lòng chọn file.");

        if (file.Length >= 5 * 1024 * 1024) throw new ArgumentException("File phải nhỏ hơn 5MB.");

        if (file.ContentType.ToLower() != "application/pdf") throw new ArgumentException("Chỉ cho phép file PDF.");

        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            Type = "upload"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        return result;
    }
}
