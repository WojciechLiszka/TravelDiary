using Microsoft.AspNetCore.Authorization;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.Authorization
{
    public class  DiaryResourceOperationRequirement:IAuthorizationRequirement

    {
        public ResourceOperation Operation { get; set; }
        public DiaryResourceOperationRequirement(ResourceOperation resourceOperation)
        {
            Operation = resourceOperation;
        }
    }



}