using MediatR;

namespace TravelDiary.Application.DiaryService.Commands.DeleteDiary
{
    public class DeleteDiaryCommand :IRequest
    {
        public int Id { get; set; }
    }
}