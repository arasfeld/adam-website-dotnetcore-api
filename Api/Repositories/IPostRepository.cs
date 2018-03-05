using Api.Entities;
using Api.Filters;
using System.Collections.Generic;

namespace Api.Repositories
{
    public interface IPostRepository
    {
        IEnumerable<Post> Browse(PostFilter filter);

        Post Read(int postId);

        Post Edit(Post post);

        Post Add(Post post);

        bool Delete(int postId);
    }
}
