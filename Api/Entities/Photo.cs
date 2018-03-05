using System;

namespace Api.Entities
{
    public class Photo
    {
        public int PhotoId { get; set; }

        public int ImageId { get; set; }

        public string Title { get; set; }

        public string Caption { get; set; }

        public DateTime Timestamp { get; set; }

        // Navigation properties

        public Image Image { get; set; }
    }
}
