using MediatR;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;

namespace TravelDiary.Application.DiaryService.Commands.CreateDiaryCommand
{
    public class CreateDiaryCommandHandler : IRequestHandler<CreateDiaryCommand, string>
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly IUserRepository _userRepository;

        public CreateDiaryCommandHandler(IDiaryRepository diaryRepository, IUserRepository userRepository)
        {
            _diaryRepository = diaryRepository;
            _userRepository = userRepository;
        }

        public async Task<string> Handle(CreateDiaryCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.TakeFirst();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var Diary = new Diary()
            {
                Name = request.Name,
                Starts = DateTime.Now,
                Description = request.Description,
                Policy = request.Policy,
                CreatedBy = user,
                CreatedById = user.Id
            };
            await _diaryRepository.Create(Diary);
            return Diary.Id.ToString();
        }
    }
}