using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.DocumentType;
using EBP.Business.Filter;
using EBP.Business.Repository;
using EmgenexBusinessPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EmgenexBusinessPortal.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("api/documentType")]
    public class ApiDocumentTypeController : ApiBaseController
    {
        RepositoryDocumentTypes repository = new RepositoryDocumentTypes();

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAllDocumentTypes(FilterDocumentTypes filter)
        {
            if (filter == null)
            {
                filter = new FilterDocumentTypes();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetDocumentTypes(filter, CurrentBusinessId);
            return Ok<DataResponse<EntityList<EntityDocumentTypes>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreateDocumentType(DocumentTypeModel model)
        {
            var response = new DataResponse<EntityDocumentTypes>();

            if (ModelState.IsValid)
            {
                var entityDocumentTypes = new EntityDocumentTypes();
                entityDocumentTypes.DocumentType = model.DocumentType;
                entityDocumentTypes.BusinessId = CurrentBusinessId;
                entityDocumentTypes.CreatedBy = CurrentUserId;
                entityDocumentTypes.CreatedOn = DateTime.UtcNow;
                response = repository.Insert(entityDocumentTypes);
            }
            return Ok<DataResponse>(response);
        }

        [Route("{id}")]
        public IHttpActionResult GetDocumentTypeById(int id)
        {

            var response = new DataResponse<EntityDocumentTypes>();
            if (id != 0)
            {
                response = repository.GetDocumentTypeById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateDocumentType(DocumentTypeModel model)
        {
            var response = new DataResponse<EntityDocumentTypes>();

            if (ModelState.IsValid)
            {
                var entityDocumentTypes = new EntityDocumentTypes();
                entityDocumentTypes.Id = model.Id;
                entityDocumentTypes.DocumentType = model.DocumentType;
                entityDocumentTypes.UpdatedBy = CurrentUserId;
                entityDocumentTypes.UpdatedOn = System.DateTime.UtcNow;

                response = repository.Update(entityDocumentTypes);
            }
            return Ok<DataResponse>(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteDocumentType(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.Delete(id);
            return Ok<DataResponse>(response);
        }
    }
}
