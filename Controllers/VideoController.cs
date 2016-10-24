using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using VideoLoaderMVC.Repository.Interfaces;
using VideoLoaderMVC.Repository;
using VideoLoaderMVC.Models;
using VideoLoaderMVC.ViewModel;
using VideoLoaderMVC.Utilities;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace VideoLoaderMVC.Controllers
{
    public class VideoController : Controller
    {
        private IVideoRepository _repository;

        private static readonly string _awsAccessKey = ConfigurationManager.AppSettings["AWSAccessKey"];

        private static readonly string _awsSecretKey = ConfigurationManager.AppSettings["AWSSecretKey"];

        private static readonly string _bucketName = ConfigurationManager.AppSettings["Bucketname"];
        
        //
        // Constructors
        public VideoController()
        {
            /// Create Video Repository           
            _repository = new EntityVideoRepository();
        }

        public VideoController(IVideoRepository repository)
        {
            this._repository = repository;
        }

        //
        // GET: /Video/
        [Authorize]
        public ActionResult Index(string videoFileName)
        {
            int userID = 0;
            
            //Get the UserID from Cookie
            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("UserID"))
            {
                userID = Utils.ConvertToInt(this.ControllerContext.HttpContext.Request.Cookies["UserID"].Value);
            } //ToDo to user login

            ViewBag.VideoFiles = this._repository.GetAll(userID);
            ViewBag.VideoToPlay = videoFileName == null ? "http://vjs.zencdn.net/v/oceans.mp4" : "https://s3-us-west-2.amazonaws.com/videokeith/" + videoFileName; //Set Default Video

            return View();
        }       
        
        [HttpPost]
        [Authorize]
        public ActionResult Index(HttpPostedFileBase file)
        {
            VideoFileModel videoToCreate = null;
            string userID = string.Empty;
            string fileName = string.Empty;

            if (file != null && file.ContentLength > 0)
            {                
                try
                {                                       
                    using (AmazonS3Client s3Client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
                    {
                        var request = new PutObjectRequest()
                        {
                            BucketName = _bucketName,
                            CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                            Key = file.FileName,
                            InputStream = file.InputStream,//SEND THE FILE STREAM
                            ContentType = "video/mp4"
                        };

                        s3Client.PutObject(request);
                    }
                }
                catch (Exception ex) //ToDo
                {

                }

                //Get the UserID from Cookie
                if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("UserID"))
                {
                    userID = this.ControllerContext.HttpContext.Request.Cookies["UserID"].Value;
                } //ToDo add error checking

                videoToCreate = new VideoFileModel();
                videoToCreate.FileName = file.FileName;
                videoToCreate.FileDescription = "Video Uploaded To Acme";
                videoToCreate.FilePath = "https://s3-us-west-2.amazonaws.com/videokeith/" + fileName;
                videoToCreate.FileSize = file.ContentLength.ToString();
                videoToCreate.UserID = userID;
            }

            UploadPageModel upm = this._repository.Create(videoToCreate);

            ViewBag.VideoFiles = this._repository.GetAll(Utils.ConvertToInt(userID));

            if (upm.SuccessfulTransfer)
            {
                //Send out Email
                Utilities.Utils.SendOutEmail(fileName, "salomon.keith@gmail.com");
                return View("Index", upm);
            }

            return RedirectToAction("Index"); //ToDo
        }

        public ActionResult Delete(int id)
        {
            return View(this._repository.Get(id));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(VideoFileModel videoToDelete)
        {
            if (this._repository.Delete(videoToDelete.id))
            return RedirectToAction("Index");

            return View();
        }        

        [HttpPost]
        public ActionResult UploadToS3(UploadPageModel inputModel)
        {
            string filePath = inputModel.FilePath/*@"C:\MP4\SampleVideo_1280x720_2mb.mp4"*/;
            string keyName = inputModel.FileName/*"SampleVideo_1280x720_2mb.mp4"*/; //FileName
            string errorMessage = string.Empty;
            bool fileTransferStatus = false;

            if (ModelState.IsValid)
            {
                try
                {
                    TransferUtility fileTransferUtility = new
                        TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast1));

                    // 1. Upload a file, file name is used as the object key name.
                    fileTransferUtility.Upload(filePath, _bucketName);
                    
                    // 2. Specify object key name explicitly.
                    fileTransferUtility.Upload(filePath,
                                              _bucketName, keyName);

                    // 3. Upload data from a type of System.IO.Stream.
                    using (FileStream fileToUpload =
                        new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        fileTransferUtility.Upload(fileToUpload,
                                                   _bucketName, keyName);
                    }
                    
                    // 4.Specify advanced settings/options.
                    TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = _bucketName,
                        FilePath = filePath,
                        StorageClass = S3StorageClass.ReducedRedundancy,
                        PartSize = 6291456, // 6 MB.
                        Key = keyName,
                        CannedACL = S3CannedACL.PublicRead
                    };

                    fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                    fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                    fileTransferUtility.Upload(fileTransferUtilityRequest);
                    
                }
                catch (AmazonS3Exception s3Exception)
                {
                    errorMessage += string.Format("Upload Errors: {0} - {1}", s3Exception.Message, s3Exception.InnerException);
                }
                catch (Exception ex) //ToDo
                {
                    errorMessage += string.Format("Upload Errors: {0}", ex.ToString());                    
                }
            }
            else
            {
                errorMessage = "<div class=\"validation-summary-errors\">"
                 + "The following errors occurred:<ul>";
                foreach (var key in ModelState.Keys)
                {
                    var error = ModelState[key].Errors.FirstOrDefault();
                    if (error != null)
                    {
                        errorMessage += "<li class=\"field-validation-error\">"
                         + error.ErrorMessage + "</li>";
                    }

                }
            }

            if(string.IsNullOrEmpty(errorMessage))
            {
                fileTransferStatus = true;
            }

            return Json(new UploadPageModel { UploadErrors = errorMessage, FileName = keyName, SuccessfulTransfer = fileTransferStatus });
        }
    }
}
