using EBP.Business.Entity.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.IO;
using System.Xml.Linq;
using EmgenexBusinessPortal.Helpers;
using EBP.Business.Repository;
using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Filter;
using EBP.Business.Helpers;
using System.Text;
using NPOI.HSSF.UserModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using EBP.Business.Entity.ParsingLog;
using NPOI.XSSF.UserModel;
using System.Drawing;
using System.Reflection;

namespace EmgenexBusinessPortal.Controllers
{
    [RoutePrefix("sales")]
    [ApiAuthorize(Roles = "admin")]
    public class ApiSalesController : ApiBaseController
    {
        IRepositoryParsingLog logRepository = null;

        public ApiSalesController()
        {
            logRepository = new RepositoryParsingLog();
        }

        private int ServiceId { get; set; }

        private bool IsFinanceFile { get; set; }

        private string MapperFilePath
        {
            get
            {
                if (this.ServiceId <= 0)
                    return null;
                string serviceName = new RepositoryLookups().GetServiceNameById(this.ServiceId).Replace(" ", ""),
                    mapperFilePath = HttpContext.Current.Server.MapPath(Path.Combine("~/Assets", CurrentBusinessId.Value.ToString(), "Sales", serviceName, IsFinanceFile ? "Finance" : "", "ReportColumnMapper.xml"));

                if (!File.Exists(mapperFilePath))
                    return null;

                return mapperFilePath;
            }
        }

        [HttpPost]
        [Route("GetAllGroupedSales")]
        [Route("")]
        public IHttpActionResult GetAllGroupedSales(FilterSales filter)
        {
            string[] allowedRoles = { "RDSLS" };
            string[] superRoles = { "RDSLSALL" };
            bool hasSuperRight = HasRight(superRoles);
            bool displayPatientName = HasRight(new string[] { "VWSLSPTNT" });

            if (HasRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositorySales();
                if (filter.ServiceId.HasValue)
                {
                    this.ServiceId = filter.ServiceId.Value;
                    this.IsFinanceFile = true;
                }

                if (!File.Exists(this.MapperFilePath))
                    this.IsFinanceFile = false;

                var response = repository.GetAllGroupedSales(filter, CurrentUser.BusinessId, CurrentUserId, hasSuperRight, CurrentUserRoles, CurrentUserDepartments, CurrentUserPrivileges, IsSalesManager || IsRep, IsSalesDirector, displayPatientName, mapperFilePath: this.MapperFilePath);
                return Ok<DataResponse<EntityList<GroupedSales>>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [HttpPost]
        [Route("getbyfilter")]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterSales filter)
        {
            string[] allowedRoles = { "RDSLS" };
            string[] superRoles = { "RDSLSALL" };
            bool hasSuperRight = HasRight(superRoles);
            bool displayPatientName = HasRight(new string[] { "VWSLSPTNT" });

            if (HasRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositorySales();
                if (filter.ServiceId.HasValue)
                {
                    this.ServiceId = filter.ServiceId.Value;
                    this.IsFinanceFile = true;
                }

                if (!File.Exists(this.MapperFilePath))
                    this.IsFinanceFile = false;

                var response = repository.GetAllList(filter, CurrentUser.BusinessId, CurrentUserId, hasSuperRight, CurrentUserRoles, CurrentUserDepartments, CurrentUserPrivileges, IsSalesManager || IsRep, IsSalesDirector, displayPatientName, mapperFilePath: this.MapperFilePath);
                return Ok<DataResponse<EntityList<EntitySales>>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("financedata/{reportId}")]
        public IHttpActionResult GetByFilter(int reportId)
        {
            string[] allowedRoles = { "RDSLS" };
            string[] superRoles = { "RDSLSALL" };
            bool hasSuperRight = HasRight(superRoles);

            if (HasRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositorySales();

                var response = repository.GetFinanceDataList(reportId, CurrentUser.BusinessId, CurrentUserId, hasSuperRight, CurrentUserRoles, CurrentUserDepartments);
                return Ok<DataResponse<EntityList<FinanceData>>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("getcolumnlookups/{serviceId}")]
        [HttpGet]
        public IHttpActionResult getColumnLookups(int serviceId)
        {
            string[] allowedRoles = { "RDSLS" };
            string[] superRoles = { "RDSLSALL" };
            bool hasSuperRight = HasRight(superRoles);

            if (HasRight(allowedRoles) || hasSuperRight)
            {
                var repository = new RepositorySales();

                var response = repository.GetColumnLookups(serviceId, CurrentUser.BusinessId, CurrentUserId, hasSuperRight, CurrentUserRoles, CurrentUserDepartments);
                return Ok<DataResponse<EntityList<SalesColumnLookup>>>(response);
            }
            else
            {
                return Ok<DataResponse>(null);
            }
        }

        [Route("getfilter/{serviceId}")]
        public IHttpActionResult GetFilter(int serviceId)
        {
            var objRepositorySales = new RepositorySales();
            this.ServiceId = serviceId;
            if (File.Exists(this.MapperFilePath))
                objRepositorySales.XmlMapper = XDocument.Load(this.MapperFilePath);
            bool displayPatientName = HasRight(new string[] { "VWSLSPTNT" });
            return Ok(objRepositorySales.GetDynamicFilters(CurrentBusinessId, CurrentUserId, serviceId, IsBuzAdmin, CurrentUserRoles, CurrentUserDepartments, CurrentUserPrivileges, IsSalesManager || IsRep, displayPatientName));
        }

        [Route("Save")]
        public IHttpActionResult ParseExcel(EntitySales model)
        {
            try
            {
                #region Upload file, Save records to database

                this.IsFinanceFile = model.IsFinanceFile;

                if (!HasRight(new string[] { "SLSIMPRT" }))
                    return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = new { Message = "Unauthorized Operation" } });

                DataResponse response = new DataResponse();

                if (model.Files != null && model.Files.Count > 0)
                {
                    StringBuilder logString = new StringBuilder();
                    string logPath = HttpContext.Current.Server.MapPath(Path.Combine("~/Assets", CurrentBusinessId.Value.ToString(), "Sales", "Sales-Archives", "Uploads", "Logs"));
                    if (!Directory.Exists(logPath))
                        Directory.CreateDirectory(logPath);
                    string logFilePath = Path.Combine(logPath, string.Format("{0}.txt", DateTime.Now.ToString("MMddyyhhmmssttfff")));

                    if (!File.Exists(logFilePath))
                    {
                        FileStream fs = File.Create(logFilePath);
                        fs.Dispose();
                    }

                    logString.AppendLine("Parsing starts @ " + DateTime.Now);
                    logString.AppendLine("BusinessId \t:\t" + CurrentBusinessId.Value);

                    List<FileInfo> FilesList = new List<FileInfo>();
                    string serviceName = new RepositoryLookups().GetServiceNameById(model.ServiceId);

                    foreach (var file in model.Files)
                    {
                        string FileName = SaveFile(file.Base64, file.FileName, serviceName);
                        FilesList.Add(new FileInfo
                        {
                            SavedFileName = FileName,
                            IncomingFileName = file.FileName
                        });
                    }

                    logString.AppendLine(string.Format("Total Files \t:\t{0}\t\nService Id \t:\t{1}", FilesList.Count(), model.ServiceId));

                    foreach (var fileInfo in FilesList)
                    {
                        XmlHelper xmlHelper = new XmlHelper();

                        string uploadPath = Path.Combine(CurrentBusinessId.Value.ToString(), "Sales", "Sales-Archives", "Uploads", serviceName, fileInfo.SavedFileName);
                        string excelFile = HttpContext.Current.Server.MapPath(Path.Combine("~/Assets", uploadPath));
                        string importedPath = Path.Combine("Assets", uploadPath);
                        this.ServiceId = model.ServiceId;

                        if (this.MapperFilePath == null)
                        {
                            logString.Append(string.Format("XML mapper file is missing for the service \"{0}\"", serviceName));
                            continue;
                        }
                        xmlHelper.XmlMapper = XDocument.Load(this.MapperFilePath);

                        int RecordCount;

                        xmlHelper.xmlString = new ExcelToXml().GetXMLString(excelFile, true, out RecordCount);

                        logString.AppendLine(string.Format("File Name \t:\t{0} \t Starts @{1}", fileInfo.SavedFileName, DateTime.UtcNow));

                        using (System.IO.StreamWriter logWriter = new System.IO.StreamWriter(logFilePath))
                        {
                            logWriter.WriteLine(logString);
                            logString.Clear();
                            int importSummeryId;
                            response = new RepositorySales().Insert(xmlHelper, RecordCount, CurrentBusinessId, CurrentUserId, model.ServiceId, IsFinanceFile, out importSummeryId, logWriter, "Web Upload", importedPath, fileInfo.IncomingFileName);
                            logString.AppendLine("File Successfully Processed");
                            logString.AppendLine("File moved to :" + excelFile);
                            logWriter.WriteLine(logString);
                        }
                    }
                    new Exception(logString.ToString()).Log(logFilePath, true);
                }

                HttpRuntime.Cache[CurrentUserId.ToString()] = true;
                return Ok<DataResponse>(response);

                #endregion
            }
            catch (Exception ex)
            {
                ex.Log();
                return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = new { Message = string.Format("{0}|{1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : "") } });
            }
        }

        [Route("SaveData")]
        public IHttpActionResult ParseExcel(SalesEntity model)
        {
            this.ServiceId = model.ServiceId;
            this.IsFinanceFile = true;
            DataResponse response = new DataResponse();

            if (!HasRight(new string[] { "SLSIMPRT" }))
            {
                return Ok<DataResponse>(response);
            }
            bool displayPatient = HasRight(new string[] { "VWSLSPTNT" });

            if (model != null)
            {
                StringBuilder logString = new StringBuilder();
                string logPath = HttpContext.Current.Server.MapPath(Path.Combine("~/Assets", CurrentBusinessId.Value.ToString(), "Sales", "Sales-Archives", "Uploads", "Logs"));
                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);
                string logFilePath = Path.Combine(logPath, string.Format("{0}.txt", DateTime.Now.ToString("MMddyyhhmmssttfff")));

                if (!File.Exists(logFilePath))
                {
                    FileStream fs = File.Create(logFilePath);
                    fs.Dispose();
                }

                logString.AppendLine("Parsing starts @ " + DateTime.Now);
                logString.AppendLine("BusinessId \t:\t" + CurrentBusinessId.Value);

                if (model.SalesList != null && model.SalesList.Count > 0)
                {
                    using (System.IO.StreamWriter logWriter = new System.IO.StreamWriter(logFilePath))
                    {
                        if (!File.Exists(this.MapperFilePath))
                            this.IsFinanceFile = false;

                        response = new RepositorySales().Insert(model.SalesList, CurrentBusinessId, CurrentUserId, model.ServiceId, this.MapperFilePath, 0, displayPatient, logWriter);
                    }
                }
            }
            return Ok<DataResponse>(response);
        }

        [Route("{id}")]
        public IHttpActionResult GetReport(int id)
        {
            bool displayPatientData = HasRight(new string[] { "VWSLSPTNT" });
            var repository = new RepositorySales();
            var response = repository.GetReport(id, CurrentUserId, IsBuzAdmin, CurrentUserRoles, CurrentUserDepartments, CurrentUserPrivileges, IsRep || IsSalesManager, displayPatientData);
            return Ok<DataResponse<EntitySales>>(response);
        }

        [Route("finance/delete/{id}")]
        [HttpGet]
        public IHttpActionResult DeleteFinance(int id)
        {
            var repository = new RepositorySales();
            string[] financeDeletePrivileges = { "DLTFNANCE" };
            var response = repository.DeleteFinance(id, CurrentUserId, HasRight(financeDeletePrivileges));

            return Ok<DataResponse>(response);
        }

        [Route("export")]
        [HttpPost]
        public IHttpActionResult ExportAllToExcel(FilterSales filter)
        {
            var response = new DataResponse<string>();
            try
            {
                if (filter.GroupBy <= 0)
                {
                    #region Sales Default Export

                    NPOI.SS.UserModel.IWorkbook workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
                    NPOI.SS.UserModel.ISheet sheet = workbook.CreateSheet("SalesReport");
                    NPOI.SS.UserModel.ICreationHelper cH = workbook.GetCreationHelper();

                    string[] columnname = filter.DynamicFilters.Where(a => a.IsVisible == true).Select(a => a.ColumnName).ToArray();
                    string[] headers, columns = null;
                    headers = columns = columnname;

                    columns = columns.Select(x => x.Replace("SalesTeam", "RepGroup")).ToArray();
                    columns = columns.Select(x => x.Replace("CollectedDate", "CollectionDate")).ToArray();

                    //byte[] rgb = new byte[3] { 22, 183, 223 };
                    //XSSFCellStyle HeaderCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    //HeaderCellStyle.SetFillBackgroundColor(new XSSFColor(rgb));

                    var headerRow = sheet.CreateRow(0);
                    //headerRow.RowStyle.FillBackgroundColor= (short)ColorTranslator.ToWin32(Color.Aqua);

                    //create header
                    for (int i = 0; i < headers.Length; i++)
                    {
                        sheet.DefaultColumnWidth = 20;
                        XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();
                        XSSFColor colorToFill = new XSSFColor(Color.Aqua);
                        style.FillBackgroundColor = (short)ColorTranslator.ToWin32(Color.Aqua);
                        headerRow.RowStyle = style;

                        var cell = headerRow.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                    }

                    string[] allowedRoles = { "RDSLS" };
                    string[] superRoles = { "RDSLSALL" };
                    bool hasSuperRight = HasRight(superRoles);
                    bool displayPatientName = HasRight(new string[] { "VWSLSPTNT" });

                    if (HasRight(allowedRoles) || hasSuperRight)
                    {
                        var repository = new RepositorySales();
                        var dataResponse = repository.GetAllList(filter, CurrentUser.BusinessId, CurrentUserId, hasSuperRight, CurrentUserRoles, CurrentUserDepartments, CurrentUserPrivileges, IsRep || IsSalesManager, IsSalesDirector, displayPatientName, 10, 0, false, true, mapperFilePath: this.MapperFilePath);

                        List<EntitySales> salesList = dataResponse.Model.List.ToList();
                        int recordCount = dataResponse.Model.Pager.TotalCount;

                        //fill content 
                        var rowIndex = 0;
                        for (int i = 0; i < recordCount; i++)
                        {
                            rowIndex++;
                            var row = sheet.CreateRow(rowIndex);

                            for (int j = 0; j < columns.Length; j++)
                            {
                                var font = workbook.CreateFont();
                                font.FontHeightInPoints = 11;
                                font.FontName = "Calibri";
                                font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                                var cell = row.CreateCell(j);
                                cell.CellStyle = workbook.CreateCellStyle();
                                cell.CellStyle.SetFont(font);

                                string value = null;
                                var objSalesItem = salesList[i];
                                var objStaticItem = objSalesItem.GetType().GetTypeInfo().GetProperty(columns[j]);

                                if (objStaticItem != null)
                                {
                                    var property = salesList[i].GetType().GetRuntimeProperties().FirstOrDefault(p => string.Equals(p.Name, objStaticItem.Name, StringComparison.OrdinalIgnoreCase));
                                    if (property != null)
                                    {
                                        value = Convert.ToString(property.GetValue(salesList[i], null));
                                    }
                                }
                                else
                                {
                                    var objDynamicItem = objSalesItem.ReportColumnValues.FirstOrDefault(a => a.ColumnName == columns[j]);
                                    if (objDynamicItem != null)
                                    {
                                        value = objDynamicItem.Value;
                                    }
                                }
                                cell.SetCellValue(value);
                            }

                            bool isFinanceDataCount = salesList[i].FinanceDataRecordCount > 1;
                            if (isFinanceDataCount)
                            {
                                for (int k = 0; k < salesList[i].FinanceDataRecordCount; k++)
                                {
                                    rowIndex++;
                                    var financeRow = sheet.CreateRow(rowIndex);

                                    for (int j = 0; j < columns.Length; j++)
                                    {
                                        var cell = financeRow.CreateCell(j);
                                        string value = null;
                                        var objSalesItem = salesList[i].FinanceDataList.ToList()[k];
                                        var objStaticItem = objSalesItem.GetType().GetProperty(columns[j]);
                                        if (objStaticItem != null)
                                        {
                                            value = Convert.ToString(objStaticItem.GetValue(objSalesItem, null));
                                        }
                                        else
                                        {
                                            var objDynamicItem = objSalesItem.FinanceColumnValues.FirstOrDefault(a => a.ColumnName == columns[j]);
                                            if (objDynamicItem != null)
                                            {
                                                value = objDynamicItem.Value;
                                            }
                                        }
                                        cell.SetCellValue(value);
                                    }
                                }
                            }
                        }

                        string directory = Path.Combine("Assets", CurrentBusinessId.Value.ToString(), "Sales", "Sales-Archives", "Exports");
                        string fileUri = HttpContext.Current.Server.MapPath(Path.Combine("~/", directory));
                        if (!Directory.Exists(fileUri))
                            Directory.CreateDirectory(fileUri);
                        string fileName = string.Format("{0:yyyyMMddhhmmssfff}", DateTime.Now),
                            extension = "xlsx";

                        string filePath = Path.Combine(fileUri, string.Format("{0}.{1}", fileName, extension));

                        int count = 1;
                        isExist:
                        if (File.Exists(filePath))
                        {
                            fileName = string.Format("{0}{1}{2}", fileName, count, extension);
                            filePath = Path.Combine(fileUri, fileName);
                            count++;
                            goto isExist;
                        }

                        using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            workbook.Write(stream);
                        }

                        response.Model = Path.Combine(directory, string.Format("{0}.{1}", fileName, extension));
                    }

                    return Ok<DataResponse>(response);

                    #endregion
                }

                #region Sales Grouped Report
                return ExportGroupedSalesToExcel(filter);
                #endregion
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            response.Message = "Internal Server Error";
            return Ok<DataResponse>(response);
        }

        public IHttpActionResult ExportGroupedSalesToExcel(FilterSales filter)
        {
            var response = new DataResponse<string>();
            try
            {
                #region Sales Default Export

                NPOI.SS.UserModel.IWorkbook workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
                NPOI.SS.UserModel.ISheet sheet = workbook.CreateSheet("SalesReport");
                NPOI.SS.UserModel.ICreationHelper cH = workbook.GetCreationHelper();

                string[] headers = new string[] { filter.GroupBy == 1 ? "Practice Name" : filter.GroupBy == 2 ? "Rep Name" : "Sales Team", "Sales", "Last Activity On" };

                var headerRow = sheet.CreateRow(0);

                //create header
                for (int i = 0; i < headers.Length; i++)
                {
                    sheet.DefaultColumnWidth = 20;
                    XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();
                    XSSFColor colorToFill = new XSSFColor(Color.Aqua);
                    style.FillBackgroundColor = (short)ColorTranslator.ToWin32(Color.Aqua);
                    headerRow.RowStyle = style;

                    var cell = headerRow.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                }

                string[] allowedRoles = { "RDSLS" };
                string[] superRoles = { "RDSLSALL" };
                bool hasSuperRight = HasRight(superRoles);
                bool displayPatientName = HasRight(new string[] { "VWSLSPTNT" });

                if (HasRight(allowedRoles) || hasSuperRight)
                {
                    var repository = new RepositorySales();
                    var dataResponse = repository.GetAllGroupedSales(filter, CurrentUser.BusinessId, CurrentUserId, hasSuperRight, CurrentUserRoles, CurrentUserDepartments, CurrentUserPrivileges, IsRep || IsSalesManager, IsSalesDirector, displayPatientName, 10, 0, false, true, mapperFilePath: this.MapperFilePath);

                    List<GroupedSales> salesList = dataResponse.Model.List.ToList();
                    int recordCount = dataResponse.Model.Pager.TotalCount;

                    //fill content 
                    var rowIndex = 0;
                    for (int i = 0; i < recordCount; i++)
                    {
                        rowIndex++;
                        var row = sheet.CreateRow(rowIndex);
                        var objSalesItem = salesList[i];
                        if (objSalesItem == null)
                            continue;

                        var font = workbook.CreateFont();
                        font.FontHeightInPoints = 11;
                        font.FontName = "Calibri";
                        font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

                        var cell1 = row.CreateCell(0);
                        cell1.CellStyle = workbook.CreateCellStyle();
                        cell1.CellStyle.SetFont(font);
                        cell1.SetCellValue(string.IsNullOrEmpty(objSalesItem.KeyName) ? "Missing Information" : objSalesItem.KeyName);

                        var cell2 = row.CreateCell(1);
                        cell2.CellStyle = workbook.CreateCellStyle();
                        cell2.CellStyle.SetFont(font);
                        cell2.SetCellValue(objSalesItem.Count);

                        var cell3 = row.CreateCell(2);
                        cell3.CellStyle = workbook.CreateCellStyle();
                        cell3.CellStyle.SetFont(font);
                        cell3.SetCellValue(Convert.ToString(objSalesItem.LastActivityOn));
                    }

                    string directory = Path.Combine("Assets", CurrentBusinessId.Value.ToString(), "Sales", "Sales-Archives", "Exports");
                    string fileUri = HttpContext.Current.Server.MapPath(Path.Combine("~/", directory));
                    if (!Directory.Exists(fileUri))
                        Directory.CreateDirectory(fileUri);
                    string fileName = string.Format("{0:yyyyMMddhhmmssfff}", DateTime.Now),
                        extension = "xlsx";

                    string filePath = Path.Combine(fileUri, string.Format("{0}.{1}", fileName, extension));

                    int count = 1;
                    isExist:
                    if (File.Exists(filePath))
                    {
                        fileName = string.Format("{0}{1}{2}", fileName, count, extension);
                        filePath = Path.Combine(fileUri, fileName);
                        count++;
                        goto isExist;
                    }

                    using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(stream);
                    }

                    response.Model = Path.Combine(directory, string.Format("{0}.{1}", fileName, extension));
                }

                return Ok<DataResponse>(response);

                #endregion
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            response.Message = "Internal Server Error";
            return Ok<DataResponse>(response);
        }

        private string SaveFile(string base64String, string fileName, string serviceName)
        {
            string rootPath = HttpContext.Current.Server.MapPath("~/Assets"),
                fileNameWithoutExt = string.Format("{0:yyyyMMddhhmmssfff}", DateTime.Now),
                extension = Path.GetExtension(fileName);
            fileName = string.Format("{0}{1}", fileNameWithoutExt, extension);
            string fileDirectory = Path.Combine(rootPath, CurrentBusinessId.Value.ToString(), "Sales", "Sales-Archives", "Uploads", serviceName);
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

        [Route("logsummary")]
        [HttpPost]
        public IHttpActionResult GetLogSummary(FilterParsingLog filter)
        {

            if (HasRight(new string[] { "SLSIMPRT" }))
            {
                return Ok<DataResponse<EntityList<EntityParsingLogSummary>>>(logRepository.GetSummaryList(filter, 0));
            }

            return Ok<DataResponse>(null);
        }

        public IHttpActionResult GetLogs(int summaryId)
        {
            if (HasRight(new string[] { "SLSIMPRT" }))
            {

                return Ok<DataResponse<EntityList<EntityParsingLog>>>(logRepository.GetAllLogsBySummaryId(summaryId));
            }
            return Ok<DataResponse>(null);
        }

        [Route("ResolveSalesData/{type}/{id}")]
        public IHttpActionResult GetLogs(int type, int id)
        {
            if (HasRight(new string[] { "SLSIMPRT" }))
            {
                return Ok<DataResponse<List<ResolvedSalesData>>>(new RepositoryParsingLog().ResolveSalesData(type, id, CurrentBusinessId.Value));
            }
            return Ok<DataResponse>(null);
        }

        [Route("ParserMessages/{id}")]
        [HttpGet]
        public IHttpActionResult ParsetMessages(int id)
        {
            return Ok<DataResponse<EntityList<ImportMessage>>>(new RepositoryParsingLog().GetAllSalesMessages(id));
        }

        [Route("IsSpecimenExists/{specimenId}")]
        [HttpGet]
        public IHttpActionResult IsSpecimenExists(string specimenId = null)
        {
            return Ok<bool>(new RepositoryParsingLog().IsSpecimenExists(specimenId));
        }

        [Route("IsParsingCompleted")]
        [HttpGet]
        public bool IsParsingCompleted()
        {
            try
            {
                if (HttpRuntime.Cache[CurrentUserId.ToString()] != null && (bool)HttpRuntime.Cache[CurrentUserId.ToString()])
                {
                    HttpRuntime.Cache.ClearCache(CurrentUserId.ToString());
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }

            return false;
        }
    }

    public class FileInfo
    {
        public string IncomingFileName { get; set; }

        public string SavedFileName { get; set; }
    }

}