using Entities.Enums;
using System.Data;
using System.Reflection;
using System.Text;
using AppUtility.Extensions;
using System.Web;
using System.Net.Http.Headers;
using Response = AppUtility.UtilityModels;
using System.Net.Mail;
using System.Net;
using Entities.Models;

namespace AppUtility.Helper
{
    public class Utility
    {
        public static Utility O => instance.Value;
        private static Lazy<Utility> instance = new Lazy<Utility>(() => new Utility());
        private Utility() { }
        public string GetErrorDescription(int errorCode)
        {
            string error = ((Errorcodes)errorCode).DescriptionAttribute();
            return error;
        }
        public Response UploadFile(FileUploadModel request)
        {
            var response = Validate.O.IsFileValid(request.file);
            if (response.StatusCode == ResponseStatus.Success)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(request.FilePath);
                    if (!Directory.Exists(sb.ToString()))
                    {
                        Directory.CreateDirectory(sb.ToString());
                    }
                    var filename = ContentDispositionHeaderValue.Parse(request.file.ContentDisposition).FileName.Trim('"');
                    string originalExt = Path.GetExtension(filename).ToLower();
                    string[] Extensions = { ".png", ".jpeg", ".jpg", ".svg" };
                    if (Extensions.Contains(originalExt))
                    {
                        //originalExt = ".jpg";
                    }
                    //string originalFileName = Path.GetFileNameWithoutExtension(filename).ToLower() + originalExt;
                    if (string.IsNullOrEmpty(request.FileName))
                    {
                        request.FileName = filename;//Path.GetFileNameWithoutExtension(request.FileName).ToLower() + originalExt;
                    }
                    //request.FileName = string.IsNullOrEmpty(request.FileName) ? originalFileName.Trim() : request.FileName;
                    sb.Append(request.FileName);
                    using (FileStream fs = File.Create(sb.ToString()))
                    {
                        request.file.CopyTo(fs);
                        fs.Flush();
                        if (request.IsThumbnailRequired)
                        {
                            //GenrateThumbnail(request.file, request.FileName, 20L);
                        }
                    }
                    response.StatusCode = ResponseStatus.Success;
                    response.ResponseText = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    response.ResponseText = "Error in file uploading. Try after sometime...";
                }
            }
            return response;
        }

        public string GetRole(int roleId)
        {
            string error = ((UserRoles)roleId).ToString();
            return error;
        }

        public Dictionary<string, dynamic> ConvertToDynamicDictionary(object someObject)
        {
            var res = someObject.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => (dynamic)prop.GetValue(someObject, null));
            return res;
        }

        public Dictionary<string, string> ConvertToDictionary(object someObject)
        {
            var res = someObject.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(someObject, null));
            return res;
        }

        public string GenrateRandom(int length, bool isNumeric = false)
        {
            string valid = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ23456789";
            if (isNumeric)
            {
                valid = "1234567890";
            }
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }

        public async Task SendMail(EmailSettings setting)
        {
            await Task.Delay(0);
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(setting.EmailFrom);
                    mail.To.Add(setting.EmailTo);
                    mail.Subject = setting.Subject;
                    mail.Body = setting.Body;
                    mail.IsBodyHtml = true;
                    //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));//--Uncomment this to send any attachment
                    using (SmtpClient smtp = new SmtpClient(setting.HostName, setting.Port))
                    {
                        smtp.Credentials = new NetworkCredential(!string.IsNullOrEmpty(setting.UserId) ? setting.UserId : setting.EmailFrom, setting.Password);
                        smtp.EnableSsl = setting.IsSSL;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
