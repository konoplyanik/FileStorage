namespace FileStorage.App.Dto.GetImages
{
    public class GetImageDto
    {
        public string Text { get; set; }
        public byte[] Image { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
