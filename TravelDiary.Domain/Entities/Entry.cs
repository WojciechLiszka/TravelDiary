namespace TravelDiary.Domain.Entities
{
    public class Entry
    {
        public int Id { get; set; }
        public string Tittle { get; set; }
        public string Description { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}