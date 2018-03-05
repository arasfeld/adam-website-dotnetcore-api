using Api.Entities;
using Api.Filters;
using Api.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services
{
    public class PostService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IPostRepository _postRepository;

        public PostService(IFileRepository fileRepository, IImageRepository imageRepository, IPostRepository postRepository)
        {
            _fileRepository = fileRepository;
            _imageRepository = imageRepository;
            _postRepository = postRepository;
        }

        public IEnumerable<Post> Browse(PostFilter filter)
        {
            IEnumerable<Post> posts = _postRepository.Browse(filter);
            foreach (Post post in posts.Where(p => p.Image != null))
            {
                byte[] imageData = _fileRepository.Get(post.Image.FileName);
                post.Image.Data = imageData;
            }
            return posts;
        }

        public Post Read(int postId)
        {
            Post post = _postRepository.Read(postId);
            if (post.Image != null)
            {
                byte[] imageData = _fileRepository.Get(post.Image.FileName);
                post.Image.Data = imageData;
            }
            return post;
        }

        public Post Edit(Post post, IFormFile file)
        {
            if (file != null)
            {
                bool fileAdded = _fileRepository.Add(file);
                Image image = new Image
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Timestamp = DateTime.UtcNow
                };
                image = _imageRepository.Add(image);
                post.ImageId = image.ImageId;
            }
            post.Timestamp = DateTime.UtcNow;
            return _postRepository.Edit(post);
        }

        public Post Add(Post post, IFormFile file)
        {
            if (file != null)
            {
                bool fileAdded = _fileRepository.Add(file);
                Image image = new Image
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Timestamp = DateTime.UtcNow
                };
                image = _imageRepository.Add(image);
                post.ImageId = image.ImageId;
            }
            post.Timestamp = DateTime.UtcNow;
            return _postRepository.Add(post);
        }

        public bool Delete(int postId)
        {
            return _postRepository.Delete(postId);
        }
    }
}
