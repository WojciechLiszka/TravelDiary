namespace TravelDiary.Domain.Entities
{
    public class Photo
    {
        public Guid Id { get; set; }
        public string Tittle { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Url { get; set; } = null!;
    }
}