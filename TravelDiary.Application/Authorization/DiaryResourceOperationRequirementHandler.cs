using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.Authorization
{
    public class DiaryResourceOperationRequirementHandler : AuthorizationHandler<DiaryResourceOperationRequirement, Diary>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DiaryResourceOperationRequirement requirement, Diary resource)
        {
            if (requirement.Operation is ResourceOperation.Create)
            {
                context.Succeed(requirement);
            }
            if (!context.User.Claims.Any()) // check for unauthorized user	
            {
                if (resource.Policy is PrivacyPolicy.Public && requirement.Operation is ResourceOperation.Read)
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }
            Guid userId = Guid.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var userRole = context.User.FindFirst(c => c.Type == ClaimTypes.Role).Value;

            if (resource.CreatedById == userId || userRole == "Admin")
            {
                context.Succeed(requirement);
            }
            if (resource.Policy is PrivacyPolicy.Public && requirement.Operation is ResourceOperation.Read)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}