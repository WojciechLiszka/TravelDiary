namespace TravelDiary.Domain.Entities
{
    public class Diary
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsPublic { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        public string Description { get; set; } = null!;


    }
}