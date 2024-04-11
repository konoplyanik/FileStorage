using FileStorage.Repository.Models;

namespace FileStorage.Repository.Repositories;

public interface IFileRepository
{
    void UploadImage(Image imageModel);
    IEnumerable<Image> GetImages();
}
