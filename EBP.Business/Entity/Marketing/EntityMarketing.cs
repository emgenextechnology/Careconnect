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

namespace EBP.Business.Entity.Marketing
{
    public class EntityMarketing
    {
        private string FilePath
        {
            get
            {
                return BusinessId != null && Id > 0 ?
                    string.Format("{0}/{1}/{2}/{3}/{4}", ConfigurationManager.AppSettings["BaseUrl"] ?? "http://localhost:20106/", "Assets", BusinessId.ToString(), "Marketing", Id.ToString()) :
                    string.Empty;
            }
        }
        public int CurrentUserId { get; set; }

        public int CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public int Id { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Name { get; set; }

        public DateTime? AlertDate { get; set; }

        public string AlertMessage { get; set; }

        public string CreatedByName { get; set; }

        public string Url { get; set; }

        public Nullable<int> DocumentTypeId { get; set; }

        public int CategoryId { get; set; }

        public string Description { get; set; }

        public List<FilesUploaded> Files { get; set; }
        public int? BusinessId { get; set; }
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
