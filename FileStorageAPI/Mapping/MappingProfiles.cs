using AutoMapper;
using FileStorage.App.Dto.GetImages;
using FileStorage.App.Dto.UploadImage;
using FileStorageAPI.Model;

namespace FileStorageAPI.Mapping;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<ImageData, UploadImageDto>();
        CreateMap<GetImageDto, ImageData>();
    }
}
