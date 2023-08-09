using MediatR;
using TravelDiary.Domain.Dtos;

namespace TravelDiary.Application.AccountService.Commands.LoginUserAccount
{
    public class LoginUserAccountCommand : LoginUserDto, IRequest<string>
    {

    }
}