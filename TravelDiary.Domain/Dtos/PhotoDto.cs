namespace TravelDiary.Domain.Dtos
{
    public class PhotoDto
    {
        public Stream? Content { get; set; }
        public string? PhotoName { get; set; }
        public string? ContentType { get; set; }
    }
}