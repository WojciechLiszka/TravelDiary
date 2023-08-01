using TravelDiary.Domain.Entities;

namespace TravelDiary.Domain.Dtos
{
    public class CreateDiaryDto
    {
        public string Name { get; set; } = null!;
        public DateTime Starts { get; set; }
        public string Description { get; set; } = null!;
        public PrivacyPolicy Policy { get; set; } = PrivacyPolicy.Private;
    }
}