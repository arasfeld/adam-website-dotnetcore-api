
namespace Api.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string PasswordHash { get; set; }
    }
}
