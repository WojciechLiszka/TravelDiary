namespace TravelDiary.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string NickName { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserDetails UserDetails { get; set; } = null!;
        public List<Diary> UserDiaries { get; set; } = new List<Diary>();
        public UserRole UserRole { get; set; } = null!;
        public int UserRoleId { get; set; }
    }
}