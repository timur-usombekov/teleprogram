using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teleprogram.Models
{
    public class TvShow
    {
        public string Title { get; set; }
        public string Channel { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public Genre Genre { get; set; }

        public DateTime EndTime => StartTime + Duration;
    }
    public enum Genre
    {
        News,
        Sports,
        Movie,
        Series,
        Other
    }
}
