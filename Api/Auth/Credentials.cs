using Api.Validation;
using FluentValidation.Attributes;

namespace Api.Auth
{
    [Validator(typeof(CredentialsValidator))]
    public class Credentials
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
