using MediatR;

namespace TravelDiary.Application.AccountService.Commands.UpdateUserAccountDetails
{
    public class UpdateUserDetailsCommand : IRequest
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Country { get; set; } = null!;
    }
}