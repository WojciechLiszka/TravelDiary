using MediatR;
using TravelDiary.Domain.Interfaces;

namespace TravelDiary.Application.DiaryService.Commands.UpdateDiaryDescription
{
    public class UpdateDiaryDescriptionCommandHandler : IRequestHandler<UpdateDiaryDescriptionCommand>
    {
        private readonly IUserContextService _userContextService;
        private readonly IDiaryRepository _diaryRepository;

        public UpdateDiaryDescriptionCommandHandler(IUserContextService userContextService, IDiaryRepository diaryRepository)
        {
            _userContextService = userContextService;
            _diaryRepository = diaryRepository;
        }

        public Task Handle(UpdateDiaryDescriptionCommand request, CancellationToken cancellationToken)
        {
            // _diaryRepository.GetById();
            throw new NotImplementedException();
        }
    }
}