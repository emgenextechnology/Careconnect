using EBP.Business.Entity.Practice;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Entity.UserProfile
{
    public class EntityUser
    {
        public bool IsApiCall { get; set; }

        public string FilePath
        {
            get
            {
                if (IsApiCall)
                    return
                        string.Format("{0}\\{1}\\{2}\\{3}\\{4}.jpg", ConfigurationManager.AppSettings["PortalUri"], "Assets", BusinessId.ToString(), "Users", Id.ToString());
                return
                    string.Format("{0}/{1}/{2}/{3}/{4}.jpg", ConfigurationManager.AppSettings["PortalUrl"], "Assets", BusinessId.ToString(), "Users", Id.ToString());

            }
        }


        public int Id { get; set; }

        public int? BusinessId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } set { var nn = value; } }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public string UserToken { get; set; }

        public string BearerToken { get; set; }

        public string DeviceToken { get; set; }
        public string MiddleName { get; set; }

        public List<FilesUploaded> Files { get; set; }

        private IEnumerable<FilesUploaded> filesList { get; set; }
        public IEnumerable<FilesUploaded> FilesList
        {
            get
            {
                return filesList != null && filesList.Count() > 0 ? filesList.Select(a => new FilesUploaded
                {
                    Id = a.Id,
                    FilePath = string.Format("{0}/{1}", FilePath, a.FileName),
                    FileType = a.FileName.Split('.')[a.FileName.Split(',').Count()],
                    FileName = a.FileName
                }) : null;
            }
            set
            {
                filesList = value;
            }
        }

        public string BusinessName { get; set; }

        public string RelativeUrl { get; set; }
    }

    public class FilesUploaded
    {
        public int Id { get; set; }

        public string Base64 { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public int FileSize { get; set; }

        public string FileType { get; set; }
    }
}