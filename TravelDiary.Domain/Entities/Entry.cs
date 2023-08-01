namespace TravelDiary.Domain.Entities
{
    public class Entry
    {
        public Guid Id { get; set; }
        public string Tittle { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime Date { get; set; }
        public Diary Diary { get; set; } = null!;
        public List<Photo> Photos { get; set; } = new List<Photo>();
    }
}