using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Validation;
using VideoLoaderMVC.Models;
using VideoLoaderMVC.Repository.Interfaces;
using VideoLoaderMVC.ViewModel;
using VideoLoaderMVC.Utilities;

namespace VideoLoaderMVC.Repository
{
    public class EntityVideoRepository : IVideoRepository
    {
        public UploadPageModel Create(VideoFileModel video)
        {
            int videoExistsID = 0;

            //ToDo use DI
            UploadPageModel upm = new UploadPageModel();
            string outMessage = string.Empty;

            //populate ViewModel common values  
            upm.FileName = Utils.ParseFileName(video.FileName);
            upm.FileDescription = video.FileDescription.Trim();

            //Check to see if Video exists, Upsert if so
            videoExistsID = Exists(video.FileName.Trim());
            if (videoExistsID != 0)
            {
                if (!Update(video, videoExistsID))
                {
                    upm.SuccessfulTransfer = false;
                    upm.UploadErrors = "Update Failed!";
                    return upm;
                }
                else
                {
                    //Populate ViewModel status and errors                   
                    upm.SuccessfulTransfer = true;
                    upm.UploadErrors = outMessage;
                    return upm;
                }
            }            
                        
            // Creates a new video.
            using (ksalomon_listEntities db = new ksalomon_listEntities())
            {
                FileInfo videoInfo = new FileInfo
                {
                    FileName = Utils.ParseFileName(video.FileName),
                    FilePath = video.FilePath,
                    FileDescription = video.FileDescription.Trim(),
                    FileSize = video.FileSize,
                    MemberFK = Utils.ConvertToInt(video.UserID),
                    CreatedDate = DateTime.Now
                };

                // Add the new object to the Members collection.
                db.FileInfoes.Add(videoInfo);

                // Save the change to the database.
                try
                {
                    db.SaveChanges();
                    //Populate ViewModel status and errors                   
                    upm.SuccessfulTransfer = true;
                    upm.UploadErrors = outMessage;
                }
                catch (DbEntityValidationException e) //Capture Entity level errors
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        outMessage += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            outMessage += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    //Populate ViewModel status and errors
                    upm.SuccessfulTransfer = false;
                    upm.UploadErrors = outMessage;
                    return upm;
                }
                catch (Exception ex)                   //Capture generic errors
                {
                    //Populate ViewModel status and errors
                    upm.SuccessfulTransfer = false;
                    upm.UploadErrors = ex.ToString();
                    return upm;
                }
            }

            return upm;
        }

        public bool Update(VideoFileModel video, int recordID)
        {
            FileInfo updateVideo = null;
            // Updates an existing video.
            try
            {
                using (ksalomon_listEntities db = new ksalomon_listEntities())
                {
                    updateVideo = (from v in db.FileInfoes where v.id == recordID select v).FirstOrDefault();
                    if (updateVideo != null)
                    {
                        updateVideo.FileName = Utils.ParseFileName(video.FileName);
                        updateVideo.FilePath = video.FilePath;
                        updateVideo.FileDescription = video.FileDescription.Trim();
                        updateVideo.FileSize = video.FileSize;
                        updateVideo.UpdatedDate = DateTime.Now;                           
                                                
                        db.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Delete(int id)
        {
            // Deletes an existing video from the database.

            return true;
        }

        public int Exists(string videoFileName)
        {
            FileInfo video = null;
            string parsedVideoName = string.Empty;
            parsedVideoName = Utils.ParseFileName(videoFileName);

            try
            {                
                using (ksalomon_listEntities db = new ksalomon_listEntities())
                {
                    video = (from v in db.FileInfoes where v.FileName.Trim() == parsedVideoName select v).FirstOrDefault();
                    if (video != null)
                    {
                        return video.id;                        
                    }
                }
            }
            catch
            {
                return 0;
            }

            return 0;
        }

        public VideoFileModel Get(int id)
        {
            // Returns an instance of a video.

            return null;
        }

        public VideoFileModel Get(string videoFileName)
        {
            FileInfo video = null;
            VideoFileModel videoInfo = null;

            try
            {
                // Returns an instance of a video.
                using (ksalomon_listEntities db = new ksalomon_listEntities())
                {
                    video = (from v in db.FileInfoes where v.FileName.Trim() == videoFileName.Trim() select v).FirstOrDefault();
                    if (video != null)
                    {
                        videoInfo = new VideoFileModel
                        {
                            FileName = video.FileName,
                            FilePath = video.FilePath,
                            FileDescription = video.FileDescription,
                            FileSize = video.FileSize,
                            CreatedDate = video.CreatedDate,
                            UserID = video.MemberFK.ToString()
                        };
                    }
                }
            }
            catch
            {
                return null;
            }

            return videoInfo;
        }

        public List<VideoFileModel> GetAll(int userID)
        {
            // Returns a list with all videos for a userID.
            List<VideoFileModel> videoModels = new List<VideoFileModel>();
            try
            {
                using (ksalomon_listEntities db = new ksalomon_listEntities())
                {
                    List<FileInfo> videos = (from v in db.FileInfoes where v.MemberFK == userID select v).OrderByDescending(v => v.id).ToList();

                    //ToDo use data mapper
                    foreach (FileInfo record in videos)
                    {
                        VideoFileModel videoInfo = new VideoFileModel
                        {
                            FileName = record.FileName,
                            FilePath = record.FilePath,
                            FileDescription = record.FileDescription,
                            FileSize = record.FileSize,
                            CreatedDate = record.CreatedDate,
                            UpdatedDate = record.UpdatedDate,
                            UserID = record.MemberFK.ToString()
                        };

                        videoModels.Add(videoInfo);
                    }
                }
            }
            catch 
            {
                return null;
            }

            return videoModels;
        }
    }
}