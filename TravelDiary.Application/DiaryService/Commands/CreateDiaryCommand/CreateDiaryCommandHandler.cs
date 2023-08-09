using MediatR;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;

namespace TravelDiary.Application.DiaryService.Commands.CreateDiaryCommand
{
    public class CreateDiaryCommandHandler : IRequestHandler<CreateDiaryCommand, string>
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly IAccountRepository _userRepository;
        private readonly IUserContextService _userContextService;

        public CreateDiaryCommandHandler(IDiaryRepository diaryRepository, IAccountRepository userRepository, IUserContextService userContextService)
        {
            _diaryRepository = diaryRepository;
            _userRepository = userRepository;
            _userContextService = userContextService;
        }

        public async Task<string> Handle(CreateDiaryCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetUserId;

            if (userId == null)
            {
                throw new Exception("Invalid User Token");
            }

            var user = await _userRepository.GetById((Guid)userId);


            if (user == null)
            {
                throw new Exception("UserNotFound");
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