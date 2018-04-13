using EBP.Business.Entity.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.IO;
using System.Xml.Linq;
using EBP.Api.Helpers;
using EBP.Business.Repository;
using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Filter;
using EBP.Business.Helpers;
using System.Text;

namespace EBP.Api.Controllers
{
    [RoutePrefix("sales")]
    [ApiAuthorize]
    public class ApiSalesController : ApiBaseController
    {
        [HttpPost]
        [Route("getbyfilter")]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterSales filter)
        {
            string[] allowedRoles = { "RDSLS" };
            string[] superRoles = { "RDSLSALL" };
            bool hasSuperRight = HasSuperRight(superRoles);
            bool displayPatientName = HasSuperRight(new string[] { "VWSLSPTNT" });

            if (HasSuperRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositorySales();
                var response = repository.GetAllList(filter, CurrentUser.BusinessId, CurrentUserId, hasSuperRight, CurrentUserRoles, CurrentUserDepartments, CurrentUserPrivileges, IsSalesManager || IsRep, IsSalesDirector, displayPatientName);
                return Ok<DataResponse<EntityList<EntitySales>>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("getfilter/{serviceId}")]
        public IHttpActionResult GetFilter(int serviceId)
        {
            bool displayPatientName = HasSuperRight(new string[] { "VWSLSPTNT" });

            return Ok(new { Status = 200, Model = new RepositorySales().GetDynamicFilters(CurrentBusinessId, CurrentUserId, serviceId, IsBuzAdmin, CurrentUserRoles, CurrentUserDepartments, CurrentUserPrivileges, IsRep || IsSalesManager, displayPatientName) });
        }

        //[Route("getcolumns/{serviceId}")]
        //public IHttpActionResult GetDynamicColumns(int serviceId)
        //{
        //    return Ok(new RepositorySales().GetDynamicColumns(CurrentBusinessId, serviceId, CurrentUserId, IsBuzAdmin, IsRep || IsSalesManager));
        //}

        //[Route("Save")]
        //public IHttpActionResult ParseExcel(EntitySales model)
        //{
        //    #region Upload file, Save records to database

        //    DataResponse response = new DataResponse();

        //    if (model.Files != null && model.Files.Count > 0)
        //    {
        //        StringBuilder logString = new StringBuilder();
        //        string logPath = HttpContext.Current.Server.MapPath(Path.Combine("~/Assets", CurrentBusinessId.Value.ToString(), "Sales", "Sales-Archives", "Uploads", "Logs"));
        //        if (!Directory.Exists(logPath))
        //            Directory.CreateDirectory(logPath);
        //        string logFilePath = Path.Combine(logPath, string.Format("{0}.txt", DateTime.Now.ToString("MMddyyhhmmssttfff")));

        //        if (!File.Exists(logFilePath))
        //        {
        //            FileStream fs = File.Create(logFilePath);
        //            fs.Dispose();
        //        }

        //        logString.AppendLine("Parsing starts @ " + DateTime.Now);
        //        logString.AppendLine("BusinessId \t:\t" + CurrentBusinessId.Value);


        //        List<string> FilesList = new List<string>();

        //        foreach (var file in model.Files)
        //        {
        //            string FileName = SaveFile(file.Base64, file.FileName);
        //            FilesList.Add(FileName);
        //        }

        //        logString.AppendLine(string.Format("Total Files \t:\t{0}\t\nService Id \t:\t{1}", FilesList.Count(), model.ServiceId));

        //        foreach (var fileName in FilesList)
        //        {
        //            XmlHelper xmlHelper = new XmlHelper();

        //            string excelFile = HttpContext.Current.Server.MapPath(Path.Combine("~/Assets", CurrentBusinessId.Value.ToString(), "Sales", "Sales-Archives", "Uploads", fileName));
        //            xmlHelper.XmlMapper = XDocument.Load(HttpContext.Current.Server.MapPath("~/MappingConfig/EXCEL.xml"));

        //            int RecordCount;
        //            using (StreamReader sr = new StreamReader(excelFile))
        //            {
        //                Stream ExcelStream = sr.BaseStream;
        //                xmlHelper.xmlString = new ExcelToXml().GetXMLString(ref ExcelStream, true, out RecordCount);
        //                ExcelStream.Close();
        //            };

        //            logString.AppendLine(string.Format("File Name \t:\t{0} \t Starts @{1}", fileName, DateTime.UtcNow));

        //            using (System.IO.StreamWriter logWriter = new System.IO.StreamWriter(logFilePath))
        //            {
        //                logWriter.WriteLine(logString);
        //                logString.Clear();
        //                response = new RepositorySales().Insert(xmlHelper, RecordCount, CurrentBusinessId, CurrentUserId, model.ServiceId, false, logWriter);
        //                logString.AppendLine("File Successfully Processed");
        //                logString.AppendLine("File moved to :" + excelFile);
        //                logWriter.WriteLine(logString);
        //            }
        //        }
        //        new Exception(logString.ToString()).Log(logFilePath, true);
        //    }

        //    return Ok<DataResponse>(response);

        //    #endregion
        //}

        [Route("{id}")]
        public IHttpActionResult GetReport(int id)
        {
            bool displayPatientData = HasRight(new string[] { "VWSLSPTNT" });
            var repository = new RepositorySales();
            var response = repository.GetReport(id, CurrentUserId, IsBuzAdmin, CurrentUserRoles, CurrentUserDepartments, CurrentUserPrivileges, IsRep || IsSalesManager, displayPatientData);
            return Ok<DataResponse<EntitySales>>(response);
            //return Ok<DataResponse>(null);
        }

        private string SaveFile(string base64String, string fileName)
        {
            string rootPath = HttpContext.Current.Server.MapPath("~/Assets"),
                fileNameWithoutExt = string.Format("{0:yyyyMMddhhmmssfff}", DateTime.Now),
                extension = Path.GetExtension(fileName);
            fileName = string.Format("{0}{1}", fileNameWithoutExt, extension);
            string fileDirectory = Path.Combine(rootPath, CurrentBusinessId.Value.ToString(), "Sales", "Sales-Archives", "Uploads");
            if (!Directory.Exists(fileDirectory))
                Directory.CreateDirectory(fileDirectory);
            string fullPath = Path.Combine(fileDirectory, fileName);

            int count = 1;
        isExist:
            if (File.Exists(fullPath))
            {
                fileName = string.Format("{0}{1}{2}", fileNameWithoutExt, count, extension);
                fullPath = Path.Combine(fileDirectory, fileName);
                count++;
                goto isExist;
            }

            var bytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(bytes))
            {
                using (var fs = new FileStream(fullPath, FileMode.Create))
                {
                    ms.WriteTo(fs);
                    ms.Close();
                }
            }
            return fileName;
        }
    }
}