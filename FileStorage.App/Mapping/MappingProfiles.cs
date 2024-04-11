using AutoMapper;
using FileStorage.App.Dto.GetImages;
using FileStorage.App.Dto.UploadImage;
using FileStorage.Repository.Models;

namespace FileStorage.App.Mapping;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<UploadImageDto, Image>()
            .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Image));

        CreateMap<Image, GetImageDto>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Picture));
    }
}

