using Api.Entities;
using Api.Filters;
using System.Collections.Generic;

namespace Api.Repositories
{
    public interface IPhotoRepository
    {
        IEnumerable<Photo> Browse(PhotoFilter filter);

        Photo Read(int photoId);

        Photo Edit(Photo photo);

        Photo Add(Photo photo);

        bool Delete(int photoId);
    }
}
