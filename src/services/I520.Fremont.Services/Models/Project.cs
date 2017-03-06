using System.Collections.Generic;

namespace I520.Fremont.Services.Models
{
    public class Project
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<string> ImageUrls { get; set; }
    }
}
