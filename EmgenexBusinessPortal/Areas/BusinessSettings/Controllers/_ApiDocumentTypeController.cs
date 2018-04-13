using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EBP.Business.Database;
using EBP.Business.Filter;
using EBP.Business;
using EBP.Business.Entity;
using EmgenexBusinessPortal.Controllers;
using EBP.Business.Repository;
using EBP.Business.Entity.DocumentType;

namespace EmgenexBusinessPortal.Areas.BusinessSettings.Controllers
{
    [RoutePrefix("DocumentTypes")]
    public class _ApiDocumentTypeController : ApiBaseController
    {
        protected int CurrentUserId { get { return 2; } }
        protected int? CurrentBusinessId { get { return 1; } }
        [HttpPost]
        [Route("getbyfilter")]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterDocumentTypes filter)
        {
            var repository = new RepositoryDocumentTypes();
            if (filter == null)
                filter = new FilterDocumentTypes { PageSize = 25, CurrentPage = 1 };
            var response = repository.GetDocumentTypes(filter, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityDocumentTypes>>>(response);
        }
        [Route("getDocumentTypebyid/{Id}")]
        [Route("{Id}")]
        public IHttpActionResult GetTaskById(int? Id)
        {
            var response = new DataResponse<EntityDocumentTypes>();
            var repository = new RepositoryDocumentTypes();
            if (Id.HasValue)
            {
                response = repository.GetDocumentTypeById(Id.Value);
            }
            else
            {
                response.Model = new EntityDocumentTypes();
            }
            return Ok<DataResponse>(response);
        }
        [HttpPost]
        [Route("Save")]
        public IHttpActionResult InsertDocumentTypeData(EntityDocumentTypes model)
        {
            var response = new DataResponse<EntityDocumentTypes>();

            if (ModelState.IsValid)
            {
                model.UpdatedBy= model.CreatedBy = CurrentUserId;
                model.BusinessId = CurrentBusinessId;
                if (model.Id > 0)
                {
                    response = new RepositoryDocumentTypes().Update(model);
                }
                else
                {
                    response = new RepositoryDocumentTypes().Insert(model);
                }
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
        [HttpPost]
        [Route("delete/{Documenttypeid}")]
        [Route("{Documenttypeid}/delete")]
        public IHttpActionResult Delete(int Documenttypeid)
        {
            var repository = new RepositoryDocumentTypes();
            var response = repository.Delete(Documenttypeid);
            return Ok<DataResponse>(response);
        }
    }
}