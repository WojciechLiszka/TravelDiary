using MediatR;
using TravelDiary.Domain.Dtos;

namespace TravelDiary.Application.AccountService.Commands.RegisterUserAccountCommand
{
    public class RegisterUserAccountCommand : RegisterUserAccountDto, IRequest
    {

    }
}