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
using EBP.Business.Entity.Services;
using System.Web;
using System.IO;

namespace EmgenexBusinessPortal.Areas.BusinessSettings.Controllers
{
    [RoutePrefix("Services")]
    public class ApiServiceController : ApiBaseController
    {
        protected int CurrentUserId { get { return 2; } }
        protected int? CurrentBusinessId { get { return 1; } }
        [HttpPost]
        [Route("getbyfilter")]
        [Route("")]
        public IHttpActionResult GetByFilter(FilterServices filter)
        {
            var repository = new RepositoryEnrolledServices();
            if (filter == null)
                filter = new FilterServices { PageSize = 25, CurrentPage = 1 };
            var response = repository.GetServices(filter, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityServices>>>(response);
        }
        [Route("getServicebyid/{Id}")]
        [Route("{Id}")]
        public IHttpActionResult GetServiceById(int? Id)
        {
            var response = new DataResponse<EntityServices>();
            var repository = new RepositoryEnrolledServices();
            if (Id.HasValue)
            {
                response = repository.GetServiceById(Id.Value);
            }
            else
            {
                response.Model = new EntityServices();
            }
            return Ok<DataResponse>(response);
        }
        ////[HttpPost]
        ////[Route("Save")]
        ////public IHttpActionResult InsertServiceData(EntityServices model)
        ////{
        ////    var response = new DataResponse<EntityServices>();

        ////    if (ModelState.IsValid)
        ////    {
        ////        model.UpdatedBy = model.CreatedBy = CurrentUserId;
        ////        model.BusinessId = CurrentBusinessId.Value;
        ////        if (model.Id > 0)
        ////        {
        ////            response = new RepositoryEnrolledServices().Update(model);
        ////            if (response.Status == DataResponseStatus.OK)
        ////            {
        ////                string rootPath = HttpContext.Current.Server.MapPath("~/Assets");
        ////                string OldPath = Path.Combine(rootPath, CurrentBusinessId.ToString(), "Sales", model.OldServiceName.Replace(" ", "").ToString());
        ////                string NewPath = Path.Combine(rootPath, CurrentBusinessId.ToString(), "Sales", model.ServiceName.Replace(" ", "").ToString());
        ////                DirectoryInfo dir = new DirectoryInfo(NewPath);
        ////                DirectoryInfo oldDir = new DirectoryInfo(OldPath);
        ////                if (oldDir.Exists)
        ////                {
        ////                    if (OldPath != NewPath)
        ////                        System.IO.Directory.Move(OldPath, NewPath);
        ////                }
        ////                else
        ////                {
        ////                    if (!dir.Exists)
        ////                        dir.Create();
        ////                }
        ////            }
        ////        }
        ////        else
        ////        {
        ////            response = new RepositoryEnrolledServices().Insert(model);
        ////            if (response.Status == DataResponseStatus.OK)
        ////            {
        ////                string rootPath = HttpContext.Current.Server.MapPath("~/Assets");
        ////                string path = Path.Combine(rootPath, CurrentBusinessId.ToString(), "Sales", response.Model.ServiceName.Replace(" ", "").ToString());
        ////                DirectoryInfo dir = new DirectoryInfo(path);
        ////                if (!dir.Exists)
        ////                    dir.Create();
        ////            }
        ////        }
        ////        return Ok<DataResponse>(response);
        ////    }
        ////    else
        ////    {
        ////        var errorList = ModelState.Where(a => a.Value.Errors.Any()).Select(s => new
        ////        {
        ////            Key = s.Key.Split('.').Last(),
        ////            Message = s.Value.Errors[0].ErrorMessage
        ////        });
        ////        return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = errorList });
        ////    }
        ////}
        [HttpPost]
        [Route("delete/{serviceid}")]
        [Route("{serviceid}/delete")]
        public IHttpActionResult DeleteService(int serviceid)
        {
            var repository = new RepositoryEnrolledServices();
            var response = repository.DeleteService(serviceid);
            return Ok<DataResponse>(response);
        }
        #region Reportcolumn
        [HttpPost]
        [Route("{ServiceId}/getbyfilter")]
        [Route("{ServiceId}/columns")]
        public IHttpActionResult ReportColumnGetByFilter(int? ServiceId,FilterServices filter)
        {
            var repository = new RepositoryEnrolledServices();
            if (filter == null)
                filter = new FilterServices { PageSize = 25, CurrentPage = 1 };
            var response = repository.GetReportColumns(filter,ServiceId, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityReportColumn>>>(response);
        }
        [Route("getReportColumnbyid/{ColumnId}")]
        [Route("{ServiceId}/columns/{ColumnId}")]
        public IHttpActionResult GetReportColumnById(int? ColumnId)
        {
            var response = new DataResponse<EntityReportColumn>();
            var repository = new RepositoryEnrolledServices();
            if (ColumnId.HasValue)
            {
                response = repository.GetReportColumnById(ColumnId.Value);
            }
            else
            {
                response.Model = new EntityReportColumn();
            }
            return Ok<DataResponse>(response);
        }
        [HttpPost]
        [Route("ReportColumn/Save")]
        public IHttpActionResult InsertReportColumnData(EntityReportColumn model)
        {
            string[] reservedFields = new string[] { "Id", "BusinessId", "ServiceId", "PatientId", "PatientFirstName", "PatientLastName", "SpecimenCollectionDate", "SpecimenReceivedDate", "ReportedDate","PracticeId", 
            "PracticeName", "ProviderId", "ProviderFirstName", "ProviderLastName", "ProviderNpi", "RepId","RepFirstName", "RepLastName", "CreatedOn", "CreatedBy", 
            "UpdatedOn", "UpdatedBy", "OldId", "IsColumnValuesImported", "OldReportId"};
            if (reservedFields.Contains(model.ColumnName))
            {
                var error = string.Format("{0} is reserved, please choose another column name", model.ColumnName);
                return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = error });
            }
            var response = new DataResponse<EntityReportColumn>();

            if (ModelState.IsValid)
            {
                model.UpdatedBy = model.CreatedBy = CurrentUserId;
                model.BusinessId = CurrentBusinessId.Value;
                if (model.Id > 0)
                {
                    response = new RepositoryEnrolledServices().UpdateRportColumn(model);
                }
                else
                {
                    response = new RepositoryEnrolledServices().InsertReportColumn(model);
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
        [Route("{ServiceId}/columns/{ColumnId}/delete")]
        public IHttpActionResult DeleteReportColumn(int ColumnId)
        {
            var repository = new RepositoryEnrolledServices();
            var response = repository.DeleteReportColumn(ColumnId);
            return Ok<DataResponse>(response);
        }
        #endregion
    }
}