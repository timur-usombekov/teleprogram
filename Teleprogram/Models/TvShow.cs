using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teleprogram.Models
{
    public class TvShow
    {
        public DateTime Date { get; set; }
        public string Time => Date.ToString("HH:mm");
        public string Channel { get; set; }
        public string Genre { get; set; }
        public string Title { get; set; }

    }
}
