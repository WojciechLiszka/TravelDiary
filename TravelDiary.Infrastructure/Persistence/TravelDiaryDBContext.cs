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
        public DbSet<UserRole> Roles { get; set; }

        public TravelDiaryDBContext(DbContextOptions<TravelDiaryDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(eb =>
            {
                eb
               .HasOne(u => u.Role)
               .WithMany(r => r.Users);

                eb
                .Property(u => u.NickName)
                .HasMaxLength(25)
                .IsRequired();

                eb
                .OwnsOne(u => u.UserDetails);

                eb
                .HasMany(u => u.UserDiaries)
                .WithOne(d => d.CreatedBy);
            });

            modelBuilder.Entity<Diary>(eb =>
            {
                eb
                 .HasMany(d => d.Entries)
                 .WithOne(e => e.Diary);
            });
            modelBuilder.Entity<Entry>(eb =>
            {
                eb
                .HasMany(e => e.Photos)
                .WithOne(e => e.Entry)
            });
        }
    }
}