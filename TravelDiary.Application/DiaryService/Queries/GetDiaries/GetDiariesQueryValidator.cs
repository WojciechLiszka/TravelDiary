using FluentValidation;
using TravelDiary.Domain.Entities;

namespace TravelDiary.Application.DiaryService.Queries.GetDiaries
{
    public class GetDiariesQueryValidator : AbstractValidator<GetDiariesQuery>
    {
        private int[] allowedPageSizes = new[] { 5, 10, 15 };

        private string[] allowedSortByColumnNames =
        {
            nameof(Diary.Name),
            nameof(Diary.Description),
            nameof(Diary.Starts),
            nameof(Diary.Ends)
        };

        public GetDiariesQueryValidator()
        {
            RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(r => r.PageSize).Custom((value, context) =>
            {
                if (!allowedPageSizes.Contains(value))
                {
                    context.AddFailure("PageSize", $"PageSize must in [{string.Join(",", allowedPageSizes)}]");
                }
            });

            RuleFor(r => r.SortBy)
                .Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
                .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
        }
    }
}