using AutoMapper;
using FileStorage.App.Dto.GetImages;
using FileStorage.App.Dto.UploadImage;
using FileStorage.Repository.Models;
using FileStorage.Repository.Repositories;

namespace FileStorage.App.Services;

public class FileService : IFileService
{
    private readonly IFileRepository _repository;
    private readonly IMapper _mapper;

    public FileService(IFileRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public void UploadImage(UploadImageDto uploadImageDto)
    {
        var image = _mapper.Map<Image>(uploadImageDto);

        _repository.UploadImage(image);
    }

    public IEnumerable<GetImageDto> GetImages()
    {
        var images = _repository.GetImages();

        var result = _mapper.Map<IEnumerable<GetImageDto>>(images);

        return result;
    }
}
