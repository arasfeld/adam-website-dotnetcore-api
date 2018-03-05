using Api.Entities;
using FluentValidation;

namespace Api.Validation
{
    public class PostValidator : AbstractValidator<Post>
    {
        public PostValidator()
        {
            RuleFor(p => p.Title).NotEmpty().WithMessage("Title cannot be empty");
            RuleFor(p => p.Body).NotEmpty().WithMessage("Body cannot be empty");
        }
    }
}
