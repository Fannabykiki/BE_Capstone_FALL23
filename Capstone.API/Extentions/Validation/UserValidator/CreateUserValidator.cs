using Capstone.Common.DTOs.User;
using FluentValidation;

namespace Capstone.API.Extentions.Validation.UserValidator
{
    public class CreateUserValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidator() {
            RuleFor(user => user.UserName).NotNull().NotEmpty().WithMessage("Username is required").WithMessage("Username length is between 1 -50 char");
        }
    }
}
