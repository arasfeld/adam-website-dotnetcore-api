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
    public class PhotosController : Controller
    {
        private readonly PhotoService photoService;

        public PhotosController(IFileRepository fileRepository, IImageRepository imageRepository, IPhotoRepository photoRepository)
        {
            photoService = new PhotoService(fileRepository, imageRepository, photoRepository);
        }

        // GET api/photos
        [HttpGet]
        public IEnumerable<Photo> Get([FromQuery]PhotoFilter filter)
        {
            return photoService.Browse(filter);
        }

        // GET api/photos/5
        [HttpGet("{id}")]
        public Photo Get(int id)
        {
            return photoService.Read(id);
        }

        // POST api/photos
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpPost]
        public Photo Post()
        {
            Photo photo = new Photo
            {
                Title = Request.Form["title"],
                Caption = Request.Form["caption"]
            };
            IFormFile file = Request.Form.Files["file"];
            return photoService.Add(photo, file);
        }

        // PUT api/photos
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpPut]
        public Photo Put([FromBody]Photo photo)
        {
            return photoService.Edit(photo);
        }

        // DELETE api/photos/5
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            return photoService.Delete(id);
        }
    }
}
