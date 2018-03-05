using Microsoft.AspNetCore.Http;

namespace Api.Repositories
{
    public interface IFileRepository
    {
        byte[] Get(string fileName);

        bool Add(IFormFile file);

        bool Delete(string fileName);
    }
}
