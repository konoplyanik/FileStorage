namespace FileStorage.Repository.Models;

public class Image
{
    public string Text { get; set; }
    public byte[] Picture { get; set; }
    public DateTime UploadDate { get; set; }
}
