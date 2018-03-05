using Api.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Api.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly string _connectionString;

        public ImageRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<Image> Browse()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<Image>($@"
                    SELECT * FROM [Images]");
            }
        }

        public Image Read(int imageId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QuerySingleOrDefault<Image>($@"
                    SELECT * FROM [Images]
                    WHERE [ImageId] = @{nameof(imageId)}",
                    new { imageId });
            }
        }

        public Image Edit(Image image)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute($@"
                    UPDATE [Images] 
                    SET [FileName] = @{nameof(Image.FileName)},
                        [ContentType] = @{nameof(Image.ContentType)},
                        [Timestamp] = GETUTCDATE()
                    WHERE [ImageId] = @{nameof(Image.ImageId)}", image);
            }

            return image;
        }

        public Image Add(Image image)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                image.ImageId = connection.QuerySingle<int>($@"
                    INSERT INTO [Images] ([FileName], [ContentType], [Timestamp])
                    VALUES (@{nameof(Image.FileName)}, @{nameof(Image.ContentType)}, GETUTCDATE());
                    SELECT CAST(SCOPE_IDENTITY() as int)", image);
            }

            return image;
        }

        public bool Delete(int imageId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute($@"
                    DELETE FROM [Images] 
                    WHERE [ImageId] = @{nameof(imageId)}",
                    new { imageId });
            }
            
            return true;
        }
    }
}
