﻿namespace TravelDiary.Domain.Entities
{
    public class Diary
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        public string Description { get; set; } = null!;
        public PrivacyPolicy Policy { get; set; } = PrivacyPolicy.Private;
        public User CreatedBy { get; set; } = null!;
        public Guid CreatedById { get; set; }
        public List<Entry> Entries { get; set; } = new List<Entry>();
    }
}