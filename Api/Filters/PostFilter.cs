using System;

namespace Api.Filters
{
    public class PostFilter
    {
        public int? PostId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 15;
    }
}
