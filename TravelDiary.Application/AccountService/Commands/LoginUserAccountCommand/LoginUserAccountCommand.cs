using MediatR;
using TravelDiary.Domain.Dtos;

namespace TravelDiary.Application.AccountService.Commands.LoginUserAccountCommand
{
    public class LoginUserAccountCommand : LoginUserDto, IRequest<string>
    {

    }
}