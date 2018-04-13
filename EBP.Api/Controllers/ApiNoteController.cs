using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EBP.Business.Repository;
using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Filter;
using EBP.Business.Entity.Note;
using EBP.Api.Models;
using EBP.Business.Enums;
using EBP.Business.Resource;
using System.Globalization;
using EBP.Business.Entity.EntityNotificationSettings;

namespace EBP.Api.Controllers
{

    [RoutePrefix("{Type:regex(leads|tasks|accounts)}/{ParentId}/notes")]
    [ApiAuthorize]
    public class ApiNoteController : ApiBaseController
    {
        [HttpPost]
        [Route("")]
        public IHttpActionResult Index(FilterNote filter)
        {
            string typeValue = ControllerContext.RouteData.Values["Type"].ToString();
            filter.ParentId = int.Parse(ControllerContext.RouteData.Values["ParentId"].ToString());

            switch (typeValue)
            {
                case "tasks":
                    filter.ParentTypeId = (int)NoteType.Task;
                    break;
                case "leads":
                case "accounts":
                    filter.ParentTypeId = (int)NoteType.Lead;
                    break;
            }

            var repository = new RepositoryNote();
            var response = repository.GetAllNotes(filter, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityNote>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult InsertNotedata(VMNoteModel entity)
        {
            var repository = new RepositoryNote();
            var response = new DataResponse<EntityNote>();
            if (ModelState.IsValid)
            {
                string typeValue = ControllerContext.RouteData.Values["Type"].ToString();
                entity.ParentId = int.Parse(ControllerContext.RouteData.Values["ParentId"].ToString());

                switch (typeValue)
                {
                    case "tasks":
                        entity.ParentTypeId = (int)NoteType.Task;
                        break;
                    case "leads":
                    case "accounts":
                        entity.ParentTypeId = (int)NoteType.Lead;
                        break;
                }

                var model = new EntityNote
                {
                    Id = entity.Id,
                    ParentId = entity.ParentId,
                    ParentTypeId = entity.ParentTypeId,
                    CreatedBy = CurrentUser.Id,
                    UpdatedBy = CurrentUser.Id,
                    Description = entity.Description,
                    CreatedOn = DateTime.UtcNow,
                    BusinessId=CurrentBusinessId,
                    CreatedByName=string.Format("{0} {1}", CurrentUser.FirstName, CurrentUser.LastName)
                };
              
                response = repository.SaveNote(model);
               
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
            var dataResponse = repository.GetNoteById(response.Model.ParentId, response.Model.ParentTypeId);
            // response.Message = response.Model.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss:fff");
            //response.Model.CreatedOn = DateTime.Parse(response.Model.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            return Ok<DataResponse>(dataResponse);
        }

        //[HttpPost]
        //[Route("delete/{id}")]
        //public IHttpActionResult DeleteNote(int id)
        //{
        //    var DeleteNote = new RepositoryNote().DeleteNote(id);
        //    if (DeleteNote)
        //        return Ok<dynamic>(new { IsSuccess = 1, Status = 200, Message = "Note deleted!" });
        //    else
        //        return Ok<dynamic>(new { IsSuccess = 0, Message = "Note Notfound!" });
        //}
    }
}