using Microsoft.EntityFrameworkCore;
using TravelDiary.Domain.Entities;

namespace TravelDiary.Infrastructure.Persistence
{
    public class TravelDiaryDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Diary> Diaries { get; set; }
    }
}