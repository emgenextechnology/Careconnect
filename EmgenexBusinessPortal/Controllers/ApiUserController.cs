using EmgenexBusinessPortal.Models;
using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.UserProfile;
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

namespace EmgenexBusinessPortal.Controllers
{
    [RoutePrefix("User")]
    [ApiAuthorize]
    public class ApiUserController : ApiBaseController
    {
        [Route("GetUserById/{UserId}")]
        public IHttpActionResult GetUserById(int UserId)
        {
            var response = new DataResponse<EntityUser>();
            var repository = new RepositoryUserProfile();
            if (UserId != null)
            {
                response = repository.GetCurrentUserbyId(UserId);
            }
            else
            {
                response.Model = new EntityUser();
            }
            return Ok<DataResponse>(response);
        }
        [HttpPost]
        [Route("Save")]
        public IHttpActionResult InsertUserData(EntityUser model)
        {
            var response = new DataResponse<EntityUser>();
            if (ModelState.IsValid)
            {
                model.BusinessId = CurrentBusinessId;
                model.Id = CurrentUserId;
                response = new RepositoryUserProfile().Update(model);
                #region Upload file

                if (model.Files != null && model.Files.Count > 0)
                {
                    List<string> FilesList = new List<string>();
                    int UserId = response.Model.Id;

                    foreach (var file in model.Files)
                    {
                        var fileName = CurrentUserId + ".jpg";
                        string FileName = SaveFile(file.Base64, fileName, UserId);
                        FilesList.Add(FileName);
                    }

                    //bool isImagesSaved = new RepositoryMarketing().SaveFiles(FilesList, MarketingId, model.Id > 0, CurrentUserId);
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
        private string SaveFile(string base64String, string fileName, int UserId)
        {
            string rootPath = HttpContext.Current.Server.MapPath("~/Assets"),
                //fileNameWithoutExt = string.Format("{0:yyyyMMddhhmmssfff}", DateTime.Now),
                fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName),
                extension = Path.GetExtension(fileName);
            fileName = string.Format("{0}{1}", fileNameWithoutExt, extension);
            string fileDirectory = Path.Combine(rootPath, CurrentBusinessId.Value.ToString(), "Users");
            if (!Directory.Exists(fileDirectory))
                Directory.CreateDirectory(fileDirectory);
            string fullPath = Path.Combine(fileDirectory, fileName);

        //    int count = 1;
        //isExist:
        //    if (File.Exists(fullPath))
        //    {
        //        fileName = string.Format("{0}{1}{2}", fileNameWithoutExt, count, extension);
        //        fullPath = Path.Combine(fileDirectory, fileName);
        //        count++;
        //        goto isExist;
        //    }

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
