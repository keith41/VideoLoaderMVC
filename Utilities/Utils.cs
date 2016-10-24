using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net.Mail;

namespace VideoLoaderMVC.Utilities
{
    public static class Utils
    {
        public static int ConvertToInt(string integer)
        {
            try
            {
                return Convert.ToInt32(integer);
            }
            catch
            {
                return 0;
            }
        }

        public static bool SendOutEmail(string videoName, string emailAddress)
        {
            string mes = string.Empty;
            string sSubject = string.Format("Acme Corp Video Upload: {0}", videoName);

            string sBody = string.Format("Thanks for uploading the following video: {0}\n\n", videoName);
                              
            
            sBody += "<div style=\"height: 50px;  width: 150px; align-self:center; display: block;  margin-left: auto;  margin-right: auto\">\n" +
                 "<span style=\"font-size:10px; color:black; text-align:center;\">Acme Corp L.L.C. 2015-2016</span>\n" +
                 "</div>\n";

            string sMailServer = "127.0.0.1";
            //string sMailServer = "relay-hosting.secureserver.net";

            MailMessage myMessage = new MailMessage();
            myMessage.From = new MailAddress("video@AcmeVideo.com", "Acme");
            myMessage.To.Add(emailAddress);            
            myMessage.Subject = sSubject;
            myMessage.IsBodyHtml = true;

            myMessage.Body = sBody;

            myMessage.BodyEncoding = Encoding.UTF8;
            myMessage.IsBodyHtml = true;

            SmtpClient mySmtpClient = new SmtpClient();
            System.Net.NetworkCredential myCredential = new System.Net.NetworkCredential("ksalomon@run4delivery.com", "Ssvlsi123");
            mySmtpClient.Host = sMailServer;
            mySmtpClient.UseDefaultCredentials = false;
            mySmtpClient.Credentials = myCredential;
            mySmtpClient.ServicePoint.MaxIdleTime = 1;

            try
            {
                mySmtpClient.Send(myMessage);
                myMessage.Dispose();
            }
            catch 
            {
                return false;
            }
            finally
            {
                //SmtpClient.Dispose in .Net 4.0 
                mySmtpClient.Dispose();
            }

            return true;
        }

        public static string ParseFileName(string fileName)
        {
            //IE and FireFox deal with filename on an upload differntly
            string[] fileNameParts;
            int fileSectionsCount = 0;
            try
            {
                fileNameParts = fileName.Split('\\');
                fileSectionsCount = fileNameParts.Length;
                if (fileSectionsCount == 1)
                {
                    return fileName.Trim();
                }
                else
                {
                    return fileNameParts[fileSectionsCount - 1].Trim();
                }
            }
            catch
            {
                return fileName;
            }            
        }
    }
}