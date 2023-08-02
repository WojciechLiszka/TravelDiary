namespace TravelDiary.Domain.Dtos
{
    public class RegisterUserAccountDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public string NickName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Country { get; set; } = null!;
    }
}