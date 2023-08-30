using MediatR;
using Microsoft.AspNetCore.Http;
using TravelDiary.Domain.Dtos;

namespace TravelDiary.Application.PhotoService.Queries
{
    public class GetPhotoQuery : IRequest<PhotoDto>
    {
        public Guid Id { get; set; }
    }
}