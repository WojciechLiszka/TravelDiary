using MediatR;
using TravelDiary.Domain.Dtos;

namespace TravelDiary.Application.AccountService.Commands.RegisterUserAccount
{
    public class RegisterUserAccountCommand : RegisterUserAccountDto, IRequest
    {
    }
}