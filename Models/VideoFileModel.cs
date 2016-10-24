using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoLoaderMVC.Models
{
    public class VideoFileModel
    {
        public int id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileDescription { get; set; }
        public string FileSize { get; set; }
        public string UserID { get; set; }
        //nullable to match entity model
        public DateTime ?CreatedDate { get; set; }
        public DateTime ?UpdatedDate { get; set; }
    }
}