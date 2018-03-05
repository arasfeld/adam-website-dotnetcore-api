using Api.Entities;
using System.Collections.Generic;

namespace Api.Repositories
{
    public interface IImageRepository
    {
        IEnumerable<Image> Browse();

        Image Read(int imageId);

        Image Edit(Image image);

        Image Add(Image image);

        bool Delete(int imageId);
    }
}
