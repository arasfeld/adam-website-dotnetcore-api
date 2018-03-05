using Api.Entities;
using Api.Filters;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Api.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly string _connectionString;

        public PhotoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<Photo> Browse(PhotoFilter filter)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<Photo, Image, Photo>($@"
                    SELECT * FROM [Photos] P
                        INNER JOIN [Images] I ON P.[ImageId] = I.[ImageId]"
                    + GetWhereClause(filter),
                    (photo, image) =>
                    {
                        photo.Image = image;
                        return photo;
                    },
                    splitOn: "ImageId");
            }
        }

        public Photo Read(int photoId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                IEnumerable<Photo> photos = connection.Query<Photo, Image, Photo>($@"
                    SELECT * FROM [Photos] P
                        INNER JOIN [Images] I ON P.[ImageId] = I.[ImageId]",
                    (photo, image) =>
                    {
                        photo.Image = image;
                        return photo;
                    },
                    splitOn: "ImageId");
                return photos.FirstOrDefault();
            }
        }

        public Photo Edit(Photo photo)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute($@"
                    UPDATE [Photos] 
                    SET [ImageId] = @{nameof(Photo.ImageId)},
                        [Title] = @{nameof(Photo.Title)},
                        [Caption] = @{nameof(Photo.Caption)},
                        [Timestamp] = GETUTCDATE()
                    WHERE [ImageId] = @{nameof(Image.ImageId)}", photo);
            }

            return photo;
        }

        public Photo Add(Photo photo)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                photo.PhotoId = connection.QuerySingle<int>($@"
                    INSERT INTO [Photos] ([ImageId], [Title], [Caption], [Timestamp])
                    VALUES (@{nameof(Photo.ImageId)}, @{nameof(Photo.Title)}, @{nameof(Photo.Caption)}, GETUTCDATE());
                    SELECT CAST(SCOPE_IDENTITY() as int)", photo);
            }

            return photo;
        }

        public bool Delete(int photoId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute($@"
                    DELETE FROM [Photos] 
                    WHERE [PhotoId] = @{nameof(photoId)}",
                    new { photoId });
            }

            return true;
        }

        private string GetWhereClause(PhotoFilter filter)
        {
            if (filter == null) return "";

            List<string> conditions = new List<string>();
            if (filter.PhotoId.HasValue) conditions.Add("P.[PhotoId] = " + filter.PhotoId.Value);
            if (filter.StartTime.HasValue) conditions.Add("P.[Timestamp] >= '" + filter.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'");
            if (filter.EndTime.HasValue) conditions.Add("P.[Timestamp] <= '" + filter.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'");

            if (!conditions.Any()) return "";

            return " WHERE " + string.Join(" AND ", conditions);
        }
    }
}
