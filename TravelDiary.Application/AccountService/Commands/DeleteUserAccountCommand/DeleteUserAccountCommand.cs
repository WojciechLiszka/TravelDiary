using MediatR;

namespace TravelDiary.Application.AccountService.Commands.DeleteUserAccountCommand
{
    public class DeleteUserAccountCommand : IRequest
    {
        public string Password { get; set; } = null!;
    }
}