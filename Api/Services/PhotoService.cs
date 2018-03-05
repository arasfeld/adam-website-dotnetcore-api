using Api.Entities;
using Api.Filters;
using Api.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Services
{
    public class PhotoService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IPhotoRepository _photoRepository;

        public PhotoService(IFileRepository fileRepository, IImageRepository imageRepository, IPhotoRepository photoRepository)
        {
            _fileRepository = fileRepository;
            _imageRepository = imageRepository;
            _photoRepository = photoRepository;
        }

        public IEnumerable<Photo> Browse(PhotoFilter filter)
        {
            IEnumerable<Photo> photos = _photoRepository.Browse(filter);
            foreach (Photo photo in photos.Where(p => p.Image != null))
            {
                byte[] imageData = _fileRepository.Get(photo.Image.FileName);
                photo.Image.Data = imageData;
            }
            return photos;
        }

        public Photo Read(int photoId)
        {
            return _photoRepository.Read(photoId);
        }

        public Photo Edit(Photo photo)
        {
            return _photoRepository.Edit(photo);
        }

        public Photo Add(Photo photo, IFormFile file)
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
                photo.ImageId = image.ImageId;
            }
            photo.Timestamp = DateTime.UtcNow;
            return _photoRepository.Add(photo);
        }

        public bool Delete(int photoId)
        {
            return _photoRepository.Delete(photoId);
        }
    }
}
