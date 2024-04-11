using FileStorage.App.Dto.GetImages;
using FileStorage.App.Dto.UploadImage;

namespace FileStorage.App.Services;

public interface IFileService
{
    void UploadImage(UploadImageDto uploadImageDto);
    IEnumerable<GetImageDto> GetImages();
}
