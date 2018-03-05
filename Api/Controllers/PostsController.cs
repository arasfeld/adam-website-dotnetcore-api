using Api.Entities;
using Api.Filters;
using Api.Repositories;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        private readonly PostService postService;

        public PostsController(IFileRepository fileRepository, IImageRepository imageRepository, IPostRepository postRepository)
        {
            postService = new PostService(fileRepository, imageRepository, postRepository);
        }

        // GET api/posts
        [HttpGet]
        public IEnumerable<Post> Get([FromQuery]PostFilter filter)
        {
            return postService.Browse(filter);
        }

        // GET api/posts/5
        [HttpGet("{id}")]
        public Post Get(int id)
        {
            return postService.Read(id);
        }

        // POST api/posts
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpPost]
        public Post Post()
        {
            var i = User;
            Post post = new Post
            {
                Title = Request.Form["title"],
                Body = Request.Form["body"]
            };
            IFormFile file = Request.Form.Files["file"];
            return postService.Add(post, file);
        }

        // PUT api/posts
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpPut]
        public Post Put()
        {
            Post post = new Post
            {
                Title = Request.Form["title"],
                Body = Request.Form["body"]
            };
            IFormFile file = Request.Form.Files["file"];
            return postService.Edit(post, file);
        }

        // DELETE api/posts/5
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            return postService.Delete(id);
        }
    }
}
