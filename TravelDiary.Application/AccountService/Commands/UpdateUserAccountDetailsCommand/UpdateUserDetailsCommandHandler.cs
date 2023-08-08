using MediatR;
using TravelDiary.Domain.Exceptions;
using TravelDiary.Domain.Interfaces;

namespace TravelDiary.Application.AccountService.Commands.UpdateUserAccountDetailsCommand
{
    internal class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateUserDetailsCommand>
    {
        private readonly IUserContextService _userContextService;
        private readonly IAccountRepository _accountRepository;

        public UpdateUserDetailsCommandHandler(IUserContextService userContextService, IAccountRepository accountRepository)
        {
            _userContextService = userContextService;
            _accountRepository = accountRepository;
        }

        public async Task Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetUserId;
            if (userId == null)
            {
                throw new Exception("Something went wrong");
            }
            var user = await _accountRepository.GetById((Guid)userId);
            if (user == null)
            {
                throw new ItemNotFoundException("Account not found");
            }
            var userEmail = user.UserDetails.Email;
            user.UserDetails = new Domain.Entities.UserDetails()
            {
                Country = request.Country,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email=userEmail
            };
            await _accountRepository.Commit();
        }
    }
}