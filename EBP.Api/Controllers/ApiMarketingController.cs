using EBP.Api.Models;
using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.Marketing;
using EBP.Business.Filter;
using EBP.Business.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace EBP.Api.Controllers
{
    [RoutePrefix("Marketing")]
    [ApiAuthorize]
    public class ApiMarketingController : ApiBaseController
    {
        [HttpPost]
        [Route("getbyfilter")]
        public IHttpActionResult GetByFilter(FilterMarketing filter)
        {
            var repository = new RepositoryMarketing();
            var response = repository.GetAllList(filter, CurrentUser.BusinessId);
            return Ok<DataResponse<EntityList<EntityMarketing>>>(response);
        }
        [Route("getfilter")]
        public IHttpActionResult GetFilter()
        {
            return Ok(new FilterMarketing());
        }

        [Route("GetMarketingById/{MarketingId}")]
        public IHttpActionResult GetMarketingById(int? MarketingId)
        {
            var response = new DataResponse<EntityMarketing>();
            var repository = new RepositoryMarketing();
            if (MarketingId.HasValue)
            {
                response = repository.GetMarketingById(MarketingId.Value, CurrentUserId, CurrentBusinessId);
            }
            else
            {
                response.Model = new EntityMarketing();
            }
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("Save")]
        public IHttpActionResult InsertMarketingData(EntityMarketing model)
        {
            var response = new DataResponse<EntityMarketing>();
            if (ModelState.IsValid)
            {
                model.UpdatedBy = model.CreatedBy = model.CurrentUserId = CurrentUserId;
                //model.CategoryId = 1;
                model.CreatedOn = System.DateTime.UtcNow;
                model.BusinessId = CurrentBusinessId;
                if (model.Id > 0)
                {
                    response = new RepositoryMarketing().Update(model);
                }
                else
                    response = new RepositoryMarketing().Insert(model);

                #region Upload file

                if (model.Files != null && model.Files.Count > 0)
                {
                    List<string> FilesList = new List<string>();
                    int MarketingId = response.Model.Id;

                    foreach (var file in model.Files)
                    {
                        string FileName = SaveFile(file.Base64, file.FileName, MarketingId);
                        FilesList.Add(FileName);
                    }

                    bool isImagesSaved = new RepositoryMarketing().SaveFiles(FilesList, MarketingId, model.Id > 0, CurrentUserId);
                }

                #endregion

                return Ok<DataResponse>(response);
            }
            else
            {
                var errorList = ModelState.Where(a => a.Value.Errors.Any()).Select(s => new
                {
                    Key = s.Key.Split('.').Last(),
                    Message = s.Value.Errors[0].ErrorMessage
                });
                return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = errorList });
            }
        }

        private string SaveFile(string base64String, string fileName, int MarketingId)
        {
            string rootPath = HttpContext.Current.Server.MapPath("~/Assets"),
                //fileNameWithoutExt = string.Format("{0:yyyyMMddhhmmssfff}", DateTime.Now),
                fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName),
                extension = Path.GetExtension(fileName);
            fileName = string.Format("{0}{1}", fileNameWithoutExt, extension);
            string fileDirectory = Path.Combine(rootPath, CurrentBusinessId.Value.ToString(), "Marketing", MarketingId.ToString());
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

        [HttpPost]
        [Route("deletefile/{id}")]
        public IHttpActionResult DeleteFile(int id)
        {
            var repository = new RepositoryMarketing();
            var response = repository.DeleteFile(id);
            return Ok<DataResponse>(response);
        }
        //private bool HasRight(string[] roles)
        //{
        //    if (CurrentUser.Roles.Contains("BusinessAdmin"))
        //        return true;
        //    return CurrentUser.UserPrivileges.Any(a => roles.Any(b => b == a));
        //}

        //private bool HasSuperRight(string[] roles)
        //{
        //    if (CurrentUser.Roles.Contains("BusinessAdmin"))
        //        return true;
        //    return CurrentUser.UserPrivileges.Any(a => roles.Any(b => b == a));
        //}
    }
}
