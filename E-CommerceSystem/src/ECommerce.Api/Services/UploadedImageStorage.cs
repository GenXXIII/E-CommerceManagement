namespace ECommerce.Api.Services;

public sealed class UploadedImageStorage
{
    private static readonly IReadOnlyDictionary<string, string> AllowedTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/webp"] = ".webp"
        };

    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public UploadedImageStorage(
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    public static string? Validate(IFormFile image, string label)
    {
        if (image.Length == 0)
            return "Choose an image to upload.";

        if (image.Length > 5 * 1024 * 1024)
            return $"{label} images must be 5 MB or smaller.";

        return AllowedTypes.ContainsKey(image.ContentType)
            ? null
            : "Use a JPG, PNG, or WebP image.";
    }

    public async Task<string> SaveAsync(
        string folderName,
        Guid ownerId,
        IFormFile image,
        CancellationToken cancellationToken)
    {
        var extension = AllowedTypes[image.ContentType];
        var fileName = $"{ownerId:N}-{Guid.NewGuid():N}{extension}";
        var uploadDirectory = Path.Combine(
            _environment.ContentRootPath,
            "uploads",
            folderName);
        Directory.CreateDirectory(uploadDirectory);

        var uploadPath = Path.Combine(uploadDirectory, fileName);
        await using (var stream = File.Create(uploadPath))
            await image.CopyToAsync(stream, cancellationToken);

        foreach (var mirrorDirectory in GetFrontendMirrorDirectories(folderName))
        {
            Directory.CreateDirectory(mirrorDirectory);
            File.Copy(uploadPath, Path.Combine(mirrorDirectory, fileName), true);
        }

        return $"/uploads/{folderName}/{fileName}";
    }

    public void Delete(string folderName, string? imageUrl, params string[] legacyFolders)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return;

        var fileName = Path.GetFileName(imageUrl);
        foreach (var folder in new[] { folderName }.Concat(legacyFolders))
        {
            DeleteIfPresent(Path.Combine(
                _environment.ContentRootPath,
                "uploads",
                folder,
                fileName));
        }

        foreach (var mirrorDirectory in GetFrontendMirrorDirectories(folderName))
            DeleteIfPresent(Path.Combine(mirrorDirectory, fileName));
    }

    private IEnumerable<string> GetFrontendMirrorDirectories(string folderName)
    {
        var configuredRoot = _configuration["ImageStorage:FrontendRoot"];
        var frontendRoot = !string.IsNullOrWhiteSpace(configuredRoot)
            ? Path.GetFullPath(configuredRoot)
            : Path.GetFullPath(Path.Combine(
                _environment.ContentRootPath,
                "..",
                "..",
                "..",
                "E-CommerceInterface"));

        if (!Directory.Exists(frontendRoot))
            yield break;

        yield return Path.Combine(frontendRoot, "public", "images", folderName);
        yield return Path.Combine(frontendRoot, "dist", "images", folderName);
    }

    private static void DeleteIfPresent(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}
