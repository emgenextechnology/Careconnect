using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.Services;
using EBP.Business.Enums;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
    public class RepositoryEnrolledServices : _Repository
    {
        #region Enrolled Services

        public DataResponse<EntityList<EntityServices>> GetServices(FilterServices filter, int? currentBusineId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityServices>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }

                base.DBInit();

                var query = DBEntity.LookupEnrolledServices.Where(a => a.BusinessId == currentBusineId);
                if (filter != null)
                {
                    if (!String.IsNullOrEmpty(filter.KeyWords))
                    {
                        query = query.Where(a => a.ServiceName.ToLower().Contains(filter.KeyWords));
                    }
                }

                var selectQuery = query.Select(a => new EntityServices
                {
                    Id = a.Id,
                    ServiceName = a.ServiceName,
                    ServiceDecription = a.ServiceDecription,
                    ServiceColor = a.ServiceColor,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    BusinessId = a.BusinessId,
                    IsActive = a.IsActive,
                    OldId = a.OldId,
                    Status = a.Status,
                    ImportMode = a.ImportMode,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
                });

                if (string.IsNullOrEmpty(filter.SortKey) || string.IsNullOrEmpty(filter.SortOrder))
                {
                    selectQuery = selectQuery.OrderByDescending(o => o.CreatedOn);
                }
                else
                {
                    string orderBy = string.Format("{0} {1}", filter.SortKey, filter.SortOrder);
                    selectQuery = selectQuery.OrderBy(orderBy);
                }
                response = GetList<EntityServices>(selectQuery, skip, take);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }

            return response;
        }

        public DataResponse<EntityServices> GetServiceById(int ServiceId)
        {
            var response = new DataResponse<EntityServices>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupEnrolledServices.Where(a => a.Id == ServiceId).Select(a => new EntityServices
                {
                    Id = a.Id,
                    ImportMode = a.ImportMode.Value,
                    FtpInfo = a.ImportMode == (int)ServiceReportImportModes.Ftp ? a.ServiceFtpInfoes.Select(b => new Entity.Services.EntityServiceFtpInfo
                    {
                        Host = b.HostName,
                        PortNumber = b.PortNumber,
                        Protocol = b.Protocol ?? (int)FTPProtocol.Ftp,
                        Username = b.Username,
                        Passsword = b.Password,
                        RemotePath = b.RemotePath
                    }).FirstOrDefault() : null,
                    BoxUrl = a.BoxUrl,
                    ServiceName = a.ServiceName,
                    ServiceDecription = a.ServiceDecription,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    BusinessId = a.BusinessId,
                    ServiceColor = a.ServiceColor.Replace("#", ""),
                    IsActive = a.IsActive,
                    OldId = a.OldId,
                    Status = a.Status,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName
                });

                response = GetFirst<EntityServices>(query);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<EntityList<EntityServices>> GetServicesByImportMode(ServiceReportImportModes importType)
        {
            var response = new DataResponse<EntityList<EntityServices>>();

            try
            {
                base.DBInit();

                int importModeId = (int)importType;
                var query = DBEntity.LookupEnrolledServices.Where(a => a.ImportMode == importModeId && a.IsActive == true).Select(a => new EntityServices
                {
                    Id = a.Id,
                    BoxUrl = a.BoxUrl,
                    ServiceName = a.ServiceName,
                    BusinessId = a.BusinessId,
                    FtpInfo = a.ImportMode == (int)ServiceReportImportModes.Ftp ? a.ServiceFtpInfoes.Select(b => new Entity.Services.EntityServiceFtpInfo
                    {
                        Host = b.HostName,
                        PortNumber = b.PortNumber,
                        Protocol = b.Protocol ?? (int)FTPProtocol.Ftp,
                        Username = b.Username,
                        Passsword = b.Password,
                        RemotePath = b.RemotePath
                    }).FirstOrDefault() : null
                });

                response = GetList<EntityServices>(query);

            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }

            return response;
        }

        public DataResponse<EntityList<EntityServices>> GetBoxTypeServicesById(int ServiceId, int boxType)
        {
            var response = new DataResponse<EntityList<EntityServices>>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupEnrolledServices.Where(a => a.Id == ServiceId && a.ImportMode == boxType).Select(a => new EntityServices
                {
                    Id = a.Id,
                    ImportMode = a.ImportMode.Value,
                    BoxUrl = a.BoxUrl,
                    ServiceName = a.ServiceName,
                    ServiceDecription = a.ServiceDecription,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    BusinessId = a.BusinessId,
                    ServiceColor = a.ServiceColor,
                    IsActive = a.IsActive,
                    OldId = a.OldId,
                    Status = a.Status,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
                });

                response = GetList<EntityServices>(query);

            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<EntityServices> Update(EntityServices entityServices)
        {
            var response = new DataResponse<EntityServices>();
            try
            {
                base.DBInit();
                if ("#FFFFFF" == "#" + entityServices.ServiceColor)
                {
                    response.Id = entityServices.Id;
                    response.Status = DataResponseStatus.InternalServerError;
                    response.Message = "please select any other color!";
                }
                else if (DBEntity.LookupEnrolledServices.Count(a => a.Id != entityServices.Id && a.ServiceColor == "#" + entityServices.ServiceColor) > 0)
                {
                    response.Id = entityServices.Id;
                    response.Status = DataResponseStatus.InternalServerError;
                    response.Message = "Service Color already exists";
                }
                else
                {
                    var model = DBEntity.LookupEnrolledServices.FirstOrDefault(a => a.Id == entityServices.Id);
                    entityServices.OldServiceName = model.ServiceName;
                    model.ServiceName = entityServices.ServiceName;
                    model.ServiceDecription = entityServices.ServiceDecription;
                    model.IsActive = entityServices.IsActive;
                    model.UpdatedOn = entityServices.UpdatedOn;
                    model.UpdatedBy = entityServices.UpdatedBy;
                    model.ServiceColor = entityServices.ServiceColor;
                    model.ImportMode = entityServices.ImportMode;
                    model.BoxUrl = entityServices.ImportMode == (int)EBP.Business.Enums.ServiceReportImportModes.BoxAPI ? entityServices.BoxUrl : null;

                    if (base.DBSaveUpdate(model) > 0)
                    {
                        #region FTP
                        if (model.ImportMode == (int)EBP.Business.Enums.ServiceReportImportModes.Ftp)
                        {
                            var ftpModel = model.ServiceFtpInfoes.FirstOrDefault(a => a.ServiceId == entityServices.Id);
                            if (ftpModel != null)
                            {
                                ftpModel.Protocol = entityServices.FtpInfo.Protocol;
                                ftpModel.HostName = entityServices.FtpInfo.Host;
                                ftpModel.Username = entityServices.FtpInfo.Username;
                                ftpModel.Password = entityServices.FtpInfo.Passsword;
                                ftpModel.PortNumber = entityServices.FtpInfo.PortNumber;
                                ftpModel.RemotePath = entityServices.FtpInfo.RemotePath;

                                DBEntity.SaveChanges();
                            }
                            else
                            {
                                DBEntity.ServiceFtpInfoes.Add(new ServiceFtpInfo
                                {
                                    HostName = entityServices.FtpInfo.Host,
                                    Protocol = entityServices.FtpInfo.Protocol,
                                    Username = entityServices.FtpInfo.Username,
                                    Password = entityServices.FtpInfo.Passsword,
                                    RemotePath = entityServices.FtpInfo.RemotePath,
                                    PortNumber = entityServices.FtpInfo.PortNumber,
                                    ServiceId = entityServices.Id
                                });
                                DBEntity.SaveChanges();
                            }
                            #endregion                          
                        }
                        var responseModel = GetServiceById(model.Id);
                        responseModel.Model.OldServiceName = model.ServiceName;
                        responseModel.Status = DataResponseStatus.OK;
                        return responseModel;
                    }
                    else
                    {
                        response.CreateResponse(DataResponseStatus.InternalServerError);
                    }
                }
            }

            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<EntityServices> Insert(EntityServices entity)
        {
            var response = new DataResponse<EntityServices>();
            try
            {
                base.DBInit();
                if ("#FFFFFF" == "#" + entity.ServiceColor)
                {
                    response.Id = entity.Id;
                    response.Status = DataResponseStatus.InternalServerError;
                    response.Message = "please select any other color!";
                }
                else if (DBEntity.LookupEnrolledServices.Count(a => a.ServiceColor == "#" + entity.ServiceColor) > 0)
                {
                    response.Id = entity.Id;
                    response.Status = DataResponseStatus.InternalServerError;
                    response.Message = "Service Color already exists";
                }
                else
                {
                    var model = new Database.LookupEnrolledService
                    {
                        BusinessId = entity.BusinessId,
                        ServiceName = entity.ServiceName,
                        ServiceDecription = entity.ServiceDecription,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = entity.CreatedOn,
                        ServiceColor = "#" + entity.ServiceColor,
                        IsActive = true,
                        ImportMode = entity.ImportMode,
                        BoxUrl = entity.ImportMode == (int)EBP.Business.Enums.ServiceReportImportModes.BoxAPI ? entity.BoxUrl : null,
                    };
                    if (base.DBSave(model) > 0)
                    {
                        #region FTP
                        if (model.ImportMode == (int)EBP.Business.Enums.ServiceReportImportModes.Ftp)
                        {
                            DBEntity.ServiceFtpInfoes.Add(new ServiceFtpInfo
                            {
                                HostName = entity.FtpInfo.Host,
                                Protocol = entity.FtpInfo.Protocol,
                                Username = entity.FtpInfo.Username,
                                Password = entity.FtpInfo.Passsword,
                                RemotePath = entity.FtpInfo.RemotePath,
                                PortNumber = entity.FtpInfo.PortNumber,
                                ServiceId = model.Id
                            });
                            DBEntity.SaveChanges();
                        }
                        #endregion                       
                        var responseModel = GetServiceById(model.Id);
                        responseModel.Status = DataResponseStatus.OK;
                        return responseModel;
                    }
                    else
                    {
                        response.CreateResponse(DataResponseStatus.InternalServerError);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<bool> DeleteService(int serviceid)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                LookupEnrolledService lookupEnrolledService = DBEntity.LookupEnrolledServices.Find(serviceid);
                try
                {
                    DBEntity.LookupEnrolledServices.Remove(lookupEnrolledService);
                    if (DBEntity.SaveChanges() > 0)
                    {
                        response.Status = DataResponseStatus.OK;
                        response.Message = "Successfully Deleted.";
                        response.Model = true;
                    }
                }
                catch (DbUpdateException ex)
                {
                    response.Status = DataResponseStatus.InternalServerError;
                    response.Message = "There are some releted item in database, please delete those first.";
                    response.Model = false;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse SetDefaultService(ServiceToggle entity)
        {
            var response = new DataResponse<bool?>();
            try
            {
                base.DBInit();

                var lookupEnrolledServices = DBEntity.LookupEnrolledServices.Where(a => a.BusinessId == entity.BusinessId).ToList();
                var model = lookupEnrolledServices.FirstOrDefault(a => a.Id == entity.ServiceId);
                if (entity.Status == true)
                {
                    lookupEnrolledServices.ForEach(a => a.Status = false);
                }
                model.Status = entity.Status;
                DBEntity.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        #endregion

        #region ReportColumn

        public DataResponse<EntityList<EntityReportColumn>> GetReportColumns(FilterServices filter, int? ServiceId, int currentBusineId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityReportColumn>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                base.DBInit();
                var query = DBEntity.ReportColumns.Where(a => a.BusinessId == currentBusineId && a.LookupEnrolledService.Id == ServiceId && a.LookupEnrolledService.BusinessId == currentBusineId);
                if (filter != null)
                {
                    if (!String.IsNullOrEmpty(filter.KeyWords))
                    {
                        query = query.Where(a => a.ColumnName.ToLower().Contains(filter.KeyWords));
                    }
                }
                var selectQuery = query.Select(a => new EntityReportColumn
                {
                    Id = a.Id,
                    ColumnName = a.ColumnName,
                    DisplayInFilter = a.DisplayInFilter,
                    DisplayName = a.DisplayName,
                    IsMandatory = a.IsMandatory,
                    ShowInGrid = a.ShowInGrid,
                    ColumnType = a.ColumnType,
                    InputType = a.InputType,
                    ServiceId = a.ServiceId,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    BusinessId = a.BusinessId,
                    ServiceName = a.LookupEnrolledService.ServiceName,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
                    RolePrivilegeIds = a.SalesRolePrivileges.Select(b => b.Id).ToList(),
                    DepartmentPrivilegeIds = a.SalesDepartmentPrivileges.Select(b => b.Id).ToList(),
                    UserPrivilegeIds = a.SalesUserPrivileges.Select(b => b.Id).ToList()
                });

                if (string.IsNullOrEmpty(filter.SortKey) || string.IsNullOrEmpty(filter.SortOrder))
                {
                    selectQuery = selectQuery.OrderByDescending(o => o.CreatedOn);
                }
                else
                {
                    string orderBy = string.Format("{0} {1}", filter.SortKey, filter.SortOrder);
                    selectQuery = selectQuery.OrderBy(orderBy);
                }

                response = GetList<EntityReportColumn>(selectQuery, skip, take);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }

            return response;
        }

        public DataResponse<EntityReportColumn> GetReportColumnById(int Id)
        {
            var response = new DataResponse<EntityReportColumn>();
            try
            {
                base.DBInit();

                var query = DBEntity.ReportColumns.Where(a => a.Id == Id).Select(a => new EntityReportColumn
                {
                    Id = a.Id,
                    ColumnName = a.ColumnName,
                    DisplayInFilter = a.DisplayInFilter,
                    DisplayName = a.DisplayName,
                    IsMandatory = a.IsMandatory,
                    ShowInGrid = a.ShowInGrid,
                    ServiceId = a.ServiceId,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    BusinessId = a.BusinessId,
                    ColumnType = a.ColumnType,
                    InputType = a.InputType,
                    ServiceName = a.LookupEnrolledService.ServiceName,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
                    RolePrivilegeIds = a.SalesRolePrivileges.Where(t => t.ColumnId == a.Id).Select(t => t.Role.Id).ToList(),
                    DepartmentPrivilegeIds = a.SalesDepartmentPrivileges.Where(t => t.ColumnId == a.Id).Select(t => t.Department.Id).ToList(),
                    UserPrivilegeIds = a.SalesUserPrivileges.Where(t => t.ColumnId == a.Id).Select(t => t.User.Id).ToList(),
                });
                response = GetFirst<EntityReportColumn>(query);

            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<EntityReportColumn> UpdateRportColumn(EntityReportColumn entity)
        {
            var response = new DataResponse<EntityReportColumn>();
            try
            {
                base.DBInit();
                if (DBEntity.ReportColumns.FirstOrDefault(a => a.Id != entity.Id && a.BusinessId == entity.BusinessId && a.ServiceId == entity.ServiceId && a.ColumnName.ToLower().Replace(" ", "") == entity.ColumnName.ToLower().Replace(" ", "")) != null)
                {
                    response.Status = DataResponseStatus.Found;
                    response.Message = "Column name already exists";
                    response.Id = entity.Id;
                }
                else
                {
                    #region Prepare model
                    var model = DBEntity.ReportColumns.FirstOrDefault(a => a.Id == entity.Id);
                    string oldColumnName = model.ColumnName;
                    model.ServiceId = entity.ServiceId;
                    model.ColumnName = entity.ColumnName.Replace(" ", "");
                    model.IsMandatory = entity.IsMandatory;
                    model.DisplayInFilter = entity.DisplayInFilter;
                    model.UpdatedBy = entity.UpdatedBy;
                    model.UpdatedOn = entity.UpdatedOn;
                    model.ShowInGrid = entity.ShowInGrid;
                    model.DisplayName = entity.DisplayName;
                    model.ColumnType = entity.ColumnType;
                    model.InputType = entity.InputType;
                    #endregion

                    #region RolePrivileges
                    var RolePrivileges = DBEntity.SalesRolePrivileges.Where(t => t.ColumnId == model.Id).Select(a => a.RoleId).ToList();
                    foreach (var item in RolePrivileges)
                    {
                        var delete = DBEntity.SalesRolePrivileges.Where(a => a.ColumnId == model.Id & a.RoleId == item).FirstOrDefault();
                        DBEntity.SalesRolePrivileges.Remove(delete);
                        DBEntity.SaveChanges();
                    }
                    if (entity.RolePrivilegeIds != null)
                    {
                        foreach (var item in entity.RolePrivilegeIds)
                        {
                            DBEntity.SalesRolePrivileges.Add(new SalesRolePrivilege
                            {
                                ColumnId = model.Id,
                                RoleId = item,
                                CreatedBy = model.CreatedBy,
                                CreatedOn = model.CreatedOn,
                                UpdatedBy = entity.UpdatedBy,
                                UpdatedOn = entity.UpdatedOn
                            });
                            DBEntity.SaveChanges();
                        }
                    }
                    #endregion

                    #region DepartmentPrivileges
                    var DepartmentPrivileges = DBEntity.SalesDepartmentPrivileges.Where(t => t.ColumnId == model.Id).Select(a => a.DepartmentId).ToList();
                    foreach (var item in DepartmentPrivileges)
                    {
                        var delete = DBEntity.SalesDepartmentPrivileges.Where(a => a.ColumnId == model.Id & a.DepartmentId == item).FirstOrDefault();
                        DBEntity.SalesDepartmentPrivileges.Remove(delete);
                        DBEntity.SaveChanges();
                    }
                    if (entity.DepartmentPrivilegeIds != null)
                    {
                        foreach (var item in entity.DepartmentPrivilegeIds)
                        {
                            DBEntity.SalesDepartmentPrivileges.Add(new SalesDepartmentPrivilege
                            {
                                ColumnId = model.Id,
                                DepartmentId = item,
                                CreatedBy = model.CreatedBy,
                                CreatedOn = model.CreatedOn,
                                UpdatedBy = entity.UpdatedBy,
                                UpdatedOn = entity.UpdatedOn
                            });
                            DBEntity.SaveChanges();
                        }
                    }
                    #endregion

                    #region UserPrivileges
                    var UserPrivileges = DBEntity.SalesUserPrivileges.Where(t => t.ColumnId == model.Id).Select(a => a.UserId).ToList();
                    foreach (var item in UserPrivileges)
                    {
                        var delete = DBEntity.SalesUserPrivileges.Where(a => a.ColumnId == model.Id & a.UserId == item).FirstOrDefault();
                        DBEntity.SalesUserPrivileges.Remove(delete);
                        DBEntity.SaveChanges();
                    }
                    if (entity.UserPrivilegeIds != null)
                    {
                        foreach (var item in entity.UserPrivilegeIds)
                        {
                            DBEntity.SalesUserPrivileges.Add(new SalesUserPrivilege
                            {
                                ColumnId = model.Id,
                                UserId = item,
                                CreatedBy = model.CreatedBy,
                                CreatedOn = model.CreatedOn,
                                UpdatedBy = entity.UpdatedBy,
                                UpdatedOn = entity.UpdatedOn
                            });
                            DBEntity.SaveChanges();
                        }
                    }
                    #endregion

                    #region ColumnVisibility
                    var objUserColumnVisibility = DBEntity.UserColumnVisibilities.Where(a => a.BusinessId == entity.BusinessId && a.ColumnName == oldColumnName && a.ServiceId == model.ServiceId);
                    if (objUserColumnVisibility.Count() > 0)
                        foreach (var item in objUserColumnVisibility)
                        {
                            item.ColumnName = entity.ColumnName;
                            item.DisplayName = entity.DisplayName;
                        }
                    #endregion

                    if (base.DBSaveUpdate(model) > 0)
                    {
                        return GetReportColumnById(model.Id);
                    }
                    else
                    {
                        response.CreateResponse(DataResponseStatus.InternalServerError);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<EntityReportColumn> InsertReportColumn(EntityReportColumn entity)
        {
            var response = new DataResponse<EntityReportColumn>();
            try
            {
                base.DBInit();
                if (DBEntity.ReportColumns.FirstOrDefault(a => a.BusinessId == entity.BusinessId && a.ServiceId == entity.ServiceId && a.ColumnName.ToLower().Replace(" ", "") == entity.ColumnName.ToLower().Replace(" ", "")) != null)
                {
                    response.Status = DataResponseStatus.Found;
                    response.Message = "Column name already exists";
                    response.Id = entity.Id;
                }
                else
                {
                    var model = new Database.ReportColumn
                    {
                        ColumnName = entity.ColumnName.Replace(" ", ""),
                        BusinessId = entity.BusinessId,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = entity.CreatedOn,
                        ServiceId = entity.ServiceId,
                        DisplayInFilter = entity.DisplayInFilter,
                        IsMandatory = entity.IsMandatory,
                        ShowInGrid = entity.ShowInGrid,
                        DisplayName = entity.DisplayName,
                        ColumnType = entity.ColumnType,
                        InputType = entity.InputType
                    };
                    if (base.DBSave(model) > 0)
                    {
                        if (entity.RolePrivilegeIds != null && entity.RolePrivilegeIds.Count() > 0)
                        {
                            foreach (var item in entity.RolePrivilegeIds)
                            {
                                DBEntity.SalesRolePrivileges.Add(new SalesRolePrivilege { ColumnId = model.Id, RoleId = item, CreatedBy = entity.CreatedBy, CreatedOn = System.DateTime.UtcNow });
                                DBEntity.SaveChanges();
                            }
                        }
                        if (entity.DepartmentPrivilegeIds != null && entity.DepartmentPrivilegeIds.Count() > 0)
                        {
                            foreach (var item in entity.DepartmentPrivilegeIds)
                            {
                                DBEntity.SalesDepartmentPrivileges.Add(new SalesDepartmentPrivilege { ColumnId = model.Id, DepartmentId = item, CreatedBy = entity.CreatedBy, CreatedOn = System.DateTime.UtcNow });
                                DBEntity.SaveChanges();
                            }
                        }
                        if (entity.UserPrivilegeIds != null && entity.UserPrivilegeIds.Count() > 0)
                        {
                            foreach (var item in entity.UserPrivilegeIds)
                            {
                                DBEntity.SalesUserPrivileges.Add(new SalesUserPrivilege { ColumnId = model.Id, UserId = item, CreatedBy = entity.CreatedBy, CreatedOn = System.DateTime.UtcNow });
                                DBEntity.SaveChanges();
                            }
                        }
                        entity.Id = model.Id;
                        response.CreateResponse(entity, DataResponseStatus.OK);
                    }
                    else
                    {
                        response.CreateResponse(DataResponseStatus.InternalServerError);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<bool> DeleteReportColumn(int ColumnId)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                ReportColumn reportColumn = DBEntity.ReportColumns.Find(ColumnId);
                try
                {
                    DBEntity.ReportColumns.Remove(reportColumn);
                    if (DBEntity.SaveChanges() > 0)
                    {
                        response.Status = DataResponseStatus.OK;
                        response.Message = "Successfully Deleted.";
                        response.Model = true;
                    }
                }
                catch (DbUpdateException ex)
                {
                    response.Status = DataResponseStatus.InternalServerError;
                    response.Message = "There are some releted item in database, please delete those first.";
                    response.Model = false;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        #endregion

        #region ReportColumnLookup

        public DataResponse<EntityList<EntityColumnLookup>> GetReportColumnLookup(FilterServices filter, int? ReportColumnId, int currentBusineId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityColumnLookup>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                base.DBInit();
                var query = DBEntity.LookupServiceColumns.Where(a => a.ColumnId == ReportColumnId);
                if (filter != null)
                {
                    if (!String.IsNullOrEmpty(filter.KeyWords))
                    {
                        query = query.Where(a => a.Text.ToLower().Contains(filter.KeyWords));
                    }
                }
                var selectQuery = query.Select(a => new EntityColumnLookup
                {
                    Id = a.Id,
                    Text = a.Text,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    ColumnId = a.ColumnId,
                    ColumnName = a.ReportColumn.ColumnName,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName
                });

                if (string.IsNullOrEmpty(filter.SortKey) || string.IsNullOrEmpty(filter.SortOrder))
                {
                    selectQuery = selectQuery.OrderByDescending(o => o.CreatedOn);
                }
                else
                {
                    string orderBy = string.Format("{0} {1}", filter.SortKey, filter.SortOrder);
                    selectQuery = selectQuery.OrderBy(orderBy);
                }

                response = GetList<EntityColumnLookup>(selectQuery, skip, take);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }

            return response;
        }

        public DataResponse<EntityColumnLookup> GetReportColumnLookupById(int Id)
        {
            var response = new DataResponse<EntityColumnLookup>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupServiceColumns.Where(a => a.Id == Id).Select(a => new EntityColumnLookup
                {
                    Id = a.Id,
                    Text = a.Text,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    ColumnId = a.ColumnId,
                    ColumnName = a.ReportColumn.ColumnName,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName
                });
                response = GetFirst<EntityColumnLookup>(query);

            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<EntityColumnLookup> InsertReportColumnLookup(EntityColumnLookup entity)
        {
            var response = new DataResponse<EntityColumnLookup>();
            try
            {
                base.DBInit();

                var model = new Database.LookupServiceColumn
                {
                    Text = entity.Text,
                    ColumnId = entity.ColumnId,
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = System.DateTime.UtcNow
                };
                if (base.DBSave(model) > 0)
                {
                    entity.Id = model.Id;
                    response.CreateResponse(entity, DataResponseStatus.OK);
                }
                else
                {
                    response.CreateResponse(DataResponseStatus.InternalServerError);
                }

            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<EntityColumnLookup> UpdateReportColumnLookup(EntityColumnLookup entity)
        {
            var response = new DataResponse<EntityColumnLookup>();
            try
            {
                base.DBInit();
                var model = DBEntity.LookupServiceColumns.FirstOrDefault(a => a.Id == entity.Id);

                model.Text = entity.Text;
                model.ColumnId = entity.ColumnId;
                model.UpdatedBy = entity.UpdatedBy;
                model.UpdatedOn = System.DateTime.UtcNow;

                if (base.DBSaveUpdate(model) > 0)
                {
                    return GetReportColumnLookupById(model.Id);
                }
                else
                {
                    response.CreateResponse(DataResponseStatus.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        public DataResponse<bool> DeleteReportColumnLookup(int id)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                LookupServiceColumn reportColumn = DBEntity.LookupServiceColumns.Find(id);
                try
                {
                    DBEntity.LookupServiceColumns.Remove(reportColumn);
                    if (DBEntity.SaveChanges() > 0)
                    {
                        response.Status = DataResponseStatus.OK;
                        response.Message = "Successfully Deleted.";
                        response.Model = true;
                    }
                }
                catch (DbUpdateException ex)
                {
                    response.Status = DataResponseStatus.InternalServerError;
                    response.Message = "There are some releted item in database, please delete those first.";
                    response.Model = false;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }

        #endregion
    }
}
