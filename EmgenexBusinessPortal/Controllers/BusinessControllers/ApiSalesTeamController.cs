using EBP.Business;
using EBP.Business.Entity;
using EBP.Business.Entity.RepGroups;
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
    [RoutePrefix("api/repgroups")]
    public class ApiSalesTeamController : ApiBaseController
    {
        RepositoryRepGroups repository = new RepositoryRepGroups();

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAllRepgroups(FilterRepGroups filter)
        {
            if (filter == null)
            {
                filter = new FilterRepGroups();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetRepGroups(filter, CurrentBusinessId.Value);

            return Ok<DataResponse<EntityList<EntityRepGroups>>>(response);
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult CreateRepgroup(RepgroupModel model)
        {
            var response = new DataResponse<EntityRepGroups>();

            if (ModelState.IsValid)
            {
                var entityRepGroups = new EntityRepGroups();
                entityRepGroups.RepGroupName = model.RepGroupName;
                entityRepGroups.Description = model.Description;
               // entityRepGroups.SalesDirectorId = model.SalesDirectorId;
                entityRepGroups.BusinessId = CurrentBusinessId;
                entityRepGroups.CreatedBy = CurrentUserId;
                entityRepGroups.CreatedOn = DateTime.UtcNow;
                entityRepGroups.SalesDirectorIds = new List<int>();
                foreach (var item in model.SalesDirectorIds)
                {
                    entityRepGroups.SalesDirectorIds.Add(item);
                }
                entityRepGroups.RepGroupManagerIds = new List<int>();
                foreach (var item in model.RepGroupManagerIds)
                {
                    entityRepGroups.RepGroupManagerIds.Add(item);
                }

                response = repository.Insert(entityRepGroups);
            }
            return Ok<DataResponse>(response);
        }

        [Route("{id}")]
        public IHttpActionResult GetRepgroupById(int id)
        {
            var response = new DataResponse<EntityRepGroups>();
            if (id != 0)
            {
                response = repository.GetRepGroupById(id);

                return Ok<DataResponse>(response);
            }
            else
            {
                return null;
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateRepgroup(RepgroupModel model)
        {
            var response = new DataResponse<EntityRepGroups>();

            if (ModelState.IsValid)
            {
                var entityRepGroups = new EntityRepGroups();
                entityRepGroups.Id = model.Id;
                entityRepGroups.RepGroupName = model.RepGroupName;
                entityRepGroups.Description = model.Description;
                entityRepGroups.SalesDirectorId = model.SalesDirectorId;
                entityRepGroups.BusinessId = CurrentBusinessId;
                entityRepGroups.RepGroupManagerIds = new List<int>();
                entityRepGroups.SalesDirectorIds = new List<int>();
                if (model.IsActive != null)
                    entityRepGroups.IsActive = model.IsActive.Value;
                entityRepGroups.CreatedBy = CurrentUserId;
                entityRepGroups.CreatedOn = DateTime.UtcNow;
                foreach (var item in model.SalesDirectorIds)
                {
                    entityRepGroups.SalesDirectorIds.Add(item);
                }
                foreach (var item in model.RepGroupManagerIds)
                {
                    entityRepGroups.RepGroupManagerIds.Add(item);
                }

                response = repository.Update(entityRepGroups);
            }
            return Ok<DataResponse>(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteRepgroupById(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.Delete(id);
            return Ok<DataResponse>(response);
        }

        //[Route("getalldirectors")]
        //public IHttpActionResult GetAllDirectors()
        //{

        //    DataResponse<EntityList<EntitySelectItem>> response = new DataResponse<EntityList<EntitySelectItem>>();
        //    RepositoryLookups repositoryLookup = new RepositoryLookups();
        //    response = repositoryLookup.GetAllDirectors(CurrentBusinessId);
        //    return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        //}

        //[Route("getallmanagers")]
        //public IHttpActionResult GetAllManagers()
        //{

        //    DataResponse<EntityList<EntitySelectItem>> response = new DataResponse<EntityList<EntitySelectItem>>();
        //    RepositoryLookups repositoryLookup = new RepositoryLookups();
        //    response = repositoryLookup.GetAllMangers(CurrentBusinessId);
        //    return Ok<DataResponse<EntityList<EntitySelectItem>>>(response);
        //}
    }
}
