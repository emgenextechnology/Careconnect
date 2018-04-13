using EBP.Business;
using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.Services;
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
using static EmgenexBusinessPortal.Models.ServiceModelcs;

namespace EmgenexBusinessPortal.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("api/service")]
    public class ApiEnrolledServiceController : ApiBaseController
    {
        RepositoryEnrolledServices repository = new RepositoryEnrolledServices();
        CareConnectCrmEntities DBEntity = new CareConnectCrmEntities();

        #region Enrolled Service

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetAll(FilterServices filter)
        {
            if (filter == null)
            {
                filter = new FilterServices();
                filter.PageSize = 25;
                filter.CurrentPage = 1;
            }
            var response = repository.GetServices(filter, CurrentBusinessId);
            return Ok<DataResponse<EntityList<EntityServices>>>(response);
        }

        [Route("{Id}")]
        public IHttpActionResult GetServiceById(int? Id)
        {
            var response = new DataResponse<EntityServices>();
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

        [HttpPost]
        [Route("Save")]
        public IHttpActionResult InsertServiceData(EnrolledServiceModel serviceModel)
        {
            var response = new DataResponse<EntityServices>();

            if (ModelState.IsValid)
            {
                serviceModel.CreatedBy = CurrentUserId;
                serviceModel.BusinessId = CurrentBusinessId.Value;

                EntityServices entityService = new EntityServices();
                entityService.BusinessId = serviceModel.BusinessId;
                entityService.ServiceName = serviceModel.ServiceName;
                entityService.ServiceDecription = serviceModel.ServiceDecription;
                entityService.CreatedBy = serviceModel.CreatedBy;
                entityService.CreatedOn = DateTime.UtcNow;
                entityService.ServiceColor = "#" + serviceModel.ServiceColor;
                entityService.IsActive = true;
                entityService.ImportMode = serviceModel.ImportMode;
                entityService.BoxUrl = serviceModel.BoxUrl;
                if (serviceModel.FtpInfo != null)
                {
                    entityService.FtpInfo = new EntityServiceFtpInfo();
                    entityService.FtpInfo.Host = serviceModel.FtpInfo.Host;
                    entityService.FtpInfo.Username = serviceModel.FtpInfo.Username;
                    entityService.FtpInfo.Passsword = serviceModel.FtpInfo.Passsword; 
                    entityService.FtpInfo.Protocol = serviceModel.FtpInfo.Protocol;
                    entityService.FtpInfo.RemotePath = serviceModel.FtpInfo.RemotePath;
                    entityService.FtpInfo.PortNumber = serviceModel.FtpInfo.PortNumber;
                }
                response = new RepositoryEnrolledServices().Insert(entityService);
                if (response.Status == DataResponseStatus.OK)
                {
                    string rootPath = HttpContext.Current.Server.MapPath("~/Assets");
                    string OldPath = Path.Combine(rootPath, CurrentBusinessId.ToString(), "Sales", response.Model.ServiceName.Replace(" ", "").ToString());
                    string NewPath = Path.Combine(rootPath, CurrentBusinessId.ToString(), "Sales", response.Model.ServiceName.Replace(" ", "").ToString());                    
                    DirectoryInfo oldDir = new DirectoryInfo(OldPath);
                    DirectoryInfo newdir = new DirectoryInfo(NewPath);
                    if (oldDir.Exists)
                    {
                        if (OldPath != NewPath)
                            System.IO.Directory.Move(OldPath, NewPath);
                    }
                    else
                    {
                        if (!newdir.Exists)
                            newdir.Create();
                    }
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

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateServiceById(EnrolledServiceModel serviceModel)
        {
            var response = new DataResponse<EntityServices>();

            if (ModelState.IsValid)
            {
                serviceModel.CreatedBy = CurrentUserId;
                serviceModel.BusinessId = CurrentBusinessId.Value;

                EntityServices entityService = new EntityServices();
                entityService.Id = serviceModel.Id;
                entityService.BusinessId = serviceModel.BusinessId;
                entityService.ServiceName = serviceModel.ServiceName;
                entityService.ServiceDecription = serviceModel.ServiceDecription;
                entityService.ServiceColor = "#" + serviceModel.ServiceColor;
                entityService.IsActive = true;
                entityService.ImportMode = serviceModel.ImportMode;
                entityService.FtpInfo = new EntityServiceFtpInfo();
                entityService.BoxUrl = serviceModel.BoxUrl;
                if (serviceModel.FtpInfo != null)
                {
                    entityService.FtpInfo.Host = serviceModel.FtpInfo.Host;
                    entityService.FtpInfo.Username = serviceModel.FtpInfo.Username;
                    entityService.FtpInfo.Passsword = serviceModel.FtpInfo.Passsword;
                    entityService.FtpInfo.Protocol = serviceModel.FtpInfo.Protocol;
                    entityService.FtpInfo.RemotePath = serviceModel.FtpInfo.RemotePath;
                    entityService.FtpInfo.PortNumber = serviceModel.FtpInfo.PortNumber;
                }

                response = new RepositoryEnrolledServices().Update(entityService);
                if (response.Status == DataResponseStatus.OK)
                {
                    string rootPath = HttpContext.Current.Server.MapPath("~/Assets");
                    string path = Path.Combine(rootPath, CurrentBusinessId.ToString(), "Sales", response.Model.ServiceName.Replace(" ", "").ToString());
                    DirectoryInfo dir = new DirectoryInfo(path);
                    if (!dir.Exists)
                        dir.Create();
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

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteService(int id)
        {
            var response = new DataResponse<bool>();
            response = repository.DeleteService(id);
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("{id}/setdefaultservice")]
        public IHttpActionResult SetDefaultService(ServiceModel model)
        {
            ServiceToggle entity = new ServiceToggle();
            entity.ServiceId = model.ServiceId;
            entity.Status = model.Status;
            entity.BusinessId = CurrentBusinessId.Value;
            var response = repository.SetDefaultService(entity);
            return Ok<DataResponse>(response);
        }

        #endregion

        #region Report Column

        [HttpPost]
        [Route("{serviceId}/reportcolumn")]
        public IHttpActionResult GetAllServiceReportColumns(int? ServiceId, FilterServices filter)
        {

            var response = repository.GetReportColumns(filter, ServiceId, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityReportColumn>>>(response);
        }

        [Route("reportcolumn/{id}")]
        public IHttpActionResult GetReportColumnById(int? id)
        {
            var response = new DataResponse<EntityReportColumn>();
            var repository = new RepositoryEnrolledServices();
            if (id.HasValue)
            {
                response = repository.GetReportColumnById(id.Value);
            }
            else
            {
                response.Model = new EntityReportColumn();
            }
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("reportColumn/save")]
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
            if (DBEntity.ReportColumns.FirstOrDefault(a => a.BusinessId == CurrentBusinessId && a.ServiceId == model.ServiceId && a.ColumnName.ToLower().Replace(" ", "") == model.ColumnName.ToLower().Replace(" ", "")) != null)
            {
                var error = "Already Used";
                ModelState.AddModelError("Error", "Already Used");
                return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = error });
            }
            var response = new DataResponse<EntityReportColumn>();
            //if (ModelState.IsValid)
            //{
            model.UpdatedBy = model.CreatedBy = CurrentUserId;
            model.BusinessId = CurrentBusinessId.Value;
            EntityReportColumn entityReportColumn = new EntityReportColumn();
            entityReportColumn.ColumnName = model.ColumnName.Replace(" ", "");
            entityReportColumn.BusinessId = model.BusinessId;
            entityReportColumn.CreatedBy = model.CreatedBy;
            entityReportColumn.CreatedOn = System.DateTime.UtcNow;
            entityReportColumn.ServiceId = model.ServiceId;
            entityReportColumn.DisplayInFilter = model.DisplayInFilter;
            entityReportColumn.IsMandatory = model.IsMandatory;
            entityReportColumn.ShowInGrid = model.ShowInGrid;
            entityReportColumn.DisplayName = model.DisplayName;
            entityReportColumn.ColumnType = model.ColumnType;
            entityReportColumn.InputType = model.InputType;
            entityReportColumn.RolePrivilegeIds = new List<int>();
            entityReportColumn.DepartmentPrivilegeIds = new List<int>();
            entityReportColumn.UserPrivilegeIds = new List<int>();

            if (model.RolePrivilegeIds != null)
            {
                foreach (var item in model.RolePrivilegeIds)
                {

                    entityReportColumn.RolePrivilegeIds.Add(item);

                }
            }
            if (model.DepartmentPrivilegeIds != null)
            {
                foreach (var item in model.DepartmentPrivilegeIds)
                {

                    entityReportColumn.DepartmentPrivilegeIds.Add(item);

                }
            }
            if (model.UserPrivilegeIds != null)
            {
                foreach (var item in model.UserPrivilegeIds)
                {

                    entityReportColumn.UserPrivilegeIds.Add(item);

                }
            }
            response = new RepositoryEnrolledServices().InsertReportColumn(entityReportColumn);

            return Ok<DataResponse>(response);
            //}
            //else
            //{
            //    var errorList = ModelState.Where(a => a.Value.Errors.Any()).Select(s => new
            //    {
            //        Key = s.Key.Split('.').Last(),
            //        Message = s.Value.Errors[0].ErrorMessage
            //    });
            //    return Ok<dynamic>(new { Status = HttpStatusCode.BadRequest, Model = errorList });
            //}
        }

        [HttpPut]
        [Route("reportColumn/{id}")]
        public IHttpActionResult UpdateReportColumnData(EntityReportColumn model)
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
                EntityReportColumn entityReportColumn = new EntityReportColumn();
                entityReportColumn.Id = model.Id;
                entityReportColumn.ColumnName = model.ColumnName.Replace(" ", "");
                entityReportColumn.BusinessId = model.BusinessId;
                entityReportColumn.CreatedBy = model.CreatedBy;
                entityReportColumn.CreatedOn = System.DateTime.UtcNow;
                entityReportColumn.UpdatedBy = model.UpdatedBy;
                entityReportColumn.UpdatedOn = System.DateTime.UtcNow;
                entityReportColumn.ServiceId = model.ServiceId;
                entityReportColumn.DisplayInFilter = model.DisplayInFilter;
                entityReportColumn.IsMandatory = model.IsMandatory;
                entityReportColumn.ShowInGrid = model.ShowInGrid;
                entityReportColumn.DisplayName = model.DisplayName;
                entityReportColumn.ColumnType = model.ColumnType;
                entityReportColumn.InputType = model.InputType;
                entityReportColumn.RolePrivilegeIds = new List<int>();
                entityReportColumn.DepartmentPrivilegeIds = new List<int>();
                entityReportColumn.UserPrivilegeIds = new List<int>();

                foreach (var item in model.RolePrivilegeIds)
                {

                    entityReportColumn.RolePrivilegeIds.Add(item);

                }
                foreach (var item in model.DepartmentPrivilegeIds)
                {

                    entityReportColumn.DepartmentPrivilegeIds.Add(item);

                }
                foreach (var item in model.UserPrivilegeIds)
                {

                    entityReportColumn.UserPrivilegeIds.Add(item);

                }
                response = new RepositoryEnrolledServices().UpdateRportColumn(entityReportColumn);

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

        [HttpDelete]
        [Route("reportColumn/{id}")]
        public IHttpActionResult DeleteReportColumn(int id)
        {
            var repository = new RepositoryEnrolledServices();
            var response = repository.DeleteReportColumn(id);
            return Ok<DataResponse>(response);
        }

        #endregion

        #region Input lookup

        [HttpPost]
        [Route("{reportcolumnid}/columnlookup")]
        public IHttpActionResult GetAllReportColumnLookup(int? reportcolumnid, FilterServices filter)
        {
            var response = repository.GetReportColumnLookup(filter, reportcolumnid, CurrentBusinessId.Value);
            return Ok<DataResponse<EntityList<EntityColumnLookup>>>(response);
        }
        [Route("columnlookup/{id}")]
        public IHttpActionResult GetReportColumnLookupById(int id)
        {
            var response = new DataResponse<EntityColumnLookup>();
            var repository = new RepositoryEnrolledServices();
            if (id > 0)
            {
                response = repository.GetReportColumnLookupById(id);
            }
            else
            {
                response.Model = new EntityColumnLookup();
            }
            return Ok<DataResponse>(response);
        }

        [HttpPost]
        [Route("columnlookup/save")]
        public IHttpActionResult CreateReportColumnLookup(ReportColumnLookupModel model)
        {
            var response = new DataResponse<EntityColumnLookup>();
            var entityColumnLookup = new EntityColumnLookup();
            entityColumnLookup.Text = model.Text;
            entityColumnLookup.CreatedBy = CurrentUserId;
            entityColumnLookup.CreatedOn = DateTime.UtcNow;
            entityColumnLookup.ColumnId = model.ColumnId;
            response = repository.InsertReportColumnLookup(entityColumnLookup);
            return Ok<DataResponse>(response);
        }

        [HttpPut]
        [Route("columnlookup/{id}")]
        public IHttpActionResult UpdateReportColumnLookup(ReportColumnLookupModel model)
        {
            var response = new DataResponse<EntityColumnLookup>();
            var entityColumnLookup = new EntityColumnLookup();
            entityColumnLookup.Id = model.Id;
            entityColumnLookup.Text = model.Text;
            entityColumnLookup.UpdatedBy = CurrentUserId;
            entityColumnLookup.ColumnId = model.ColumnId;
            response = repository.UpdateReportColumnLookup(entityColumnLookup);
            return Ok<DataResponse>(response);
        }

        [HttpDelete]
        [Route("columnlookup/{id}")]
        public IHttpActionResult DeleteReportColumnLookup(int id)
        {
            var repository = new RepositoryEnrolledServices();
            var response = repository.DeleteReportColumnLookup(id);
            return Ok<DataResponse>(response);
        }

        #endregion
    }
}
