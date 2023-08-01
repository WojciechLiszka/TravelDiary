namespace TravelDiary.Domain.Entities
{
    public class UserRole
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = null!;
        public List<User> Users { get; set; } = new List<User>();
    }
}