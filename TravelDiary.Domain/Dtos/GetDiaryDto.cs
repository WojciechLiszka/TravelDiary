using TravelDiary.Domain.Entities;

namespace TravelDiary.Domain.Dtos
{
    public class GetDiaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        public string Description { get; set; } = null!;
        public List<Entry> Entries { get; set; }
    }
}