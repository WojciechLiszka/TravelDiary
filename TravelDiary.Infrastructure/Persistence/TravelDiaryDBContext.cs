using Microsoft.EntityFrameworkCore;
using TravelDiary.Domain.Entities;

namespace TravelDiary.Infrastructure.Persistence
{
    public class TravelDiaryDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Diary> Diaries { get; set; }
        public DbSet<UserRole> Roles { get; set; }

        public TravelDiaryDbContext(DbContextOptions<TravelDiaryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(eb =>
            {
                eb
               .HasOne(u => u.UserRole)
               .WithMany(r => r.Users)
               .HasForeignKey(u=>u.UserRoleId);

                eb
                .Property(u => u.NickName)
                .HasMaxLength(25)
                .IsRequired();

                eb
                .OwnsOne(u => u.UserDetails);
                eb
                .HasMany(u => u.UserDiaries)
                .WithOne(d => d.CreatedBy)
                .HasForeignKey(d=>d.CreatedById);
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
                .WithOne(e => e.Entry);
            });
        }
    }
}