namespace TravelDiary.Domain.Dtos
{
    public class CreateEntryDto
    {
        public string Tittle { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}