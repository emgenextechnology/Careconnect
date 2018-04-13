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
using EBP.Business.Entity.MarketingCategory;

namespace EmgenexBusinessPortal.Areas.BusinessSettings.Controllers
{
    [RoutePrefix("MarketingCategories")]
    public class ApiMarketingCategoriesController : ApiBaseController
    {
        protected int CurrentUserId { get { return 2; } }
        protected int? CurrentBusinessId { get { return 1; } }
        [HttpPost]
        [Route("getbyfilter")]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterMarketingCategories filter)
        {
            var repository = new RepositoryMarketingCategories();
            if (filter == null)
                filter = new FilterMarketingCategories { PageSize = 25, CurrentPage = 1 };
            var response = repository.GetMarketingCategories(filter, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityMarketingCategories>>>(response);
        }
        [Route("getmarketingcategorybyid/{Id}")]
        [Route("{Id}")]
        public IHttpActionResult GetMarketingCategoryById(int? Id)
        {
            var response = new DataResponse<EntityMarketingCategories>();
            var repository = new RepositoryMarketingCategories();
            if (Id.HasValue)
            {
                response = repository.GetMarketingCategoryById(Id.Value);
            }
            else
            {
                response.Model = new EntityMarketingCategories();
            }
            return Ok<DataResponse>(response);
        }
        [HttpPost]
        [Route("Save")]
        public IHttpActionResult InsertMarketingCategoryData(EntityMarketingCategories model)
        {
            var response = new DataResponse<EntityMarketingCategories>();

            if (ModelState.IsValid)
            {
                model.UpdatedBy= model.CreatedBy = CurrentUserId;
                model.BusinessId = CurrentBusinessId.Value;
                if (model.Id > 0)
                {
                    response = new RepositoryMarketingCategories().Update(model);
                }
                else
                {
                    response = new RepositoryMarketingCategories().Insert(model);
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
        [Route("delete/{markrtingcategoryid}")]
        [Route("{markrtingcategoryid}/delete")]
        public IHttpActionResult Delete(int markrtingcategoryid)
        {
            var repository = new RepositoryMarketingCategories();
            var response = repository.Delete(markrtingcategoryid);
            return Ok<DataResponse>(response);
        }
    }
}