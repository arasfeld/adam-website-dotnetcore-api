using Api.Validation;
using FluentValidation.Attributes;

namespace Api.Auth
{
    [Validator(typeof(RegistrationValidator))]
    public class Registration
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
