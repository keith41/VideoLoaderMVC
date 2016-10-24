using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VideoLoaderMVC.Models;
using VideoLoaderMVC.ViewModel;

namespace VideoLoaderMVC.Repository.Interfaces
{
    public interface IVideoRepository
    {
        UploadPageModel Create(VideoFileModel video);
        bool Update(VideoFileModel video, int recordID);
        bool Delete(int id);
        int Exists(string videoFileName);
        VideoFileModel Get(int id);
        VideoFileModel Get(string videoFileName);
        List<VideoFileModel> GetAll(int userID);
    }
}