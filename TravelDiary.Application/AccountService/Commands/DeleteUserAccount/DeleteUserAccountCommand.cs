using MediatR;

namespace TravelDiary.Application.AccountService.Commands.DeleteUserAccount
{
    public class DeleteUserAccountCommand : IRequest
    {
        public string Password { get; set; } = null!;
    }
}