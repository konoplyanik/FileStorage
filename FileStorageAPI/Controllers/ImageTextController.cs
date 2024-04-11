using AutoMapper;
using FileStorage.App.Dto.UploadImage;
using FileStorage.App.Services;
using FileStorageAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageTextController : ControllerBase
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public ImageTextController(IWebHostEnvironment hostEnvironment, IFileService fileService, IMapper mapper)
    {
        _hostEnvironment = hostEnvironment;
        _fileService = fileService;
        _mapper = mapper;
    }

    [HttpPost]
    [Route("save-image")]
    public IActionResult UploadImageText([FromBody] ImageData imageData)
    {
        var imageDto = _mapper.Map<UploadImageDto>(imageData);

        _fileService.UploadImage(imageDto);

        return Ok();
    }

    [HttpGet]
    [Route("get-images")]
    public IActionResult GetImageText()
    {
        var imagesData = _fileService.GetImages();
        var result = _mapper.Map<IEnumerable<ImageData>>(imagesData);

        return Ok(result);
    }
}
