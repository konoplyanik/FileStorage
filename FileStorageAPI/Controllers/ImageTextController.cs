using FileStorageAPI.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FileStorageAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageTextController : ControllerBase
{
    private readonly IWebHostEnvironment _hostEnvironment;

    public ImageTextController(IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    [HttpPost]
    [Route("save-image")]
    public IActionResult SaveImageText([FromBody] ImageTextData imageTextData)
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Images", $"{imageTextData.Text}.json");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        var json = JsonSerializer.Serialize(imageTextData);
        System.IO.File.WriteAllText(filePath, json);
        return Ok();
    }

    [HttpGet]
    [Route("get-images")]
    public IActionResult GetImageTexts()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Images");
        var files = Directory.GetFiles(filePath, "*.json");

        var imageTexts = new List<ImageTextData>();
        foreach (var file in files)
        {
            var json = System.IO.File.ReadAllText(file);
            var imageText = JsonSerializer.Deserialize<ImageTextData>(json);
            imageTexts.Add(imageText);
        }

        return Ok(imageTexts);
    }
}
