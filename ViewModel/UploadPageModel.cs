using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoLoaderMVC.ViewModel
{
    public class UploadPageModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileDescription { get; set; }
        public string FileKey { get; set; }
        public string UploadErrors { get; set; }
        public bool SuccessfulTransfer { get; set; }
    }
}