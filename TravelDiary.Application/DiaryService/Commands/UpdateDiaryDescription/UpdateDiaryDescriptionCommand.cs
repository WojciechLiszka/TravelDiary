using MediatR;

namespace TravelDiary.Application.DiaryService.Commands.UpdateDiaryDescription
{
    public class UpdateDiaryDescriptionCommand : IRequest
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
    }
}