namespace TravelDiary.Domain.Entities
{
    public class Photo
    {
        public Guid Id { get; set; }
        public string Tittle { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public Entry Entry { get; set; } = null!;
        public int EntryId { get; set; }
    }
}