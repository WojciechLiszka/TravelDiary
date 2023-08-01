using MediatR;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;

namespace TravelDiary.Application.DiaryService.Commands.CreateDiaryCommand
{
    public class CreateDiaryCommandHandler : IRequestHandler<CreateDiaryCommand, string>
    {
        private readonly IDiaryRepository _diaryRepository;

        public CreateDiaryCommandHandler(IDiaryRepository diaryRepository)
        {
            _diaryRepository = diaryRepository;
        }

        public async Task<string> Handle(CreateDiaryCommand request, CancellationToken cancellationToken)
        {
            var Diary = new Diary()
            {
                Name = request.Name,
                Starts = DateTime.Now,
                Description = request.Description,
                Policy = request.Policy, //ToDo
               
            };
            await _diaryRepository.Create(Diary);
            return Diary.Id.ToString();
        }
    }
}