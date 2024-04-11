using FileStorage.Repository.Models;
using FileStorage.Repository.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FileStorage.Repository.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly FileStorageConfig _fileStorageConfig;

        public FileRepository(IOptions<FileStorageConfig> fileStorageConfig)
        {
            _fileStorageConfig = fileStorageConfig.Value;
        }

        public void UploadImage(Image imageModel)
        {
            imageModel.UploadDate = DateTime.Now;
            var filePath = _fileStorageConfig.Path + $"{imageModel.Text}.json";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            var json = JsonSerializer.Serialize(imageModel);
            File.WriteAllText(filePath, json);
        }

        public IEnumerable<Image> GetImages()
        {
            var filePath = _fileStorageConfig.Path;
            var files = Directory.GetFiles(filePath, "*.json");

            var images = new List<Image>();
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var image = JsonSerializer.Deserialize<Image>(json);
                images.Add(image);
            }

            return images;
        }
    }
}
