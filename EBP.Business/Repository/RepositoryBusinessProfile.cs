using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.Business;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq.Dynamic;


namespace EBP.Business.Repository
{
    public class RepositoryBusinessProfiles : _Repository
    {
        public DataResponse<EntityBusinessProfile> GetBusinessProfileById(int BusinessId)
        {
            var response = new DataResponse<EntityBusinessProfile>();
            try
            {
                base.DBInit();

                var query = DBEntity.BusinessMasters.Where(a => a.Id == BusinessId).Select(a => new EntityBusinessProfile
                {
                    Id = a.Id,
                    BusinessName = a.BusinessName,
                    Description = a.Description,
                    Address = a.Address,
                    About = a.About,
                    DomainUrl = a.DomainUrl,
                    RelativeUrl = a.RelativeUrl,
                    City = a.City,
                    State = a.State,
                    Country = a.Country,
                    IsActive = a.IsActive,
                    DateRange = a.DateRange,
                    Status = a.Status,
                    OtherEmails = a.OtherEmails,
                    SalesGroup = a.SalesGroupBy,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    BusinessId = BusinessId
                });

                response = GetFirst<EntityBusinessProfile>(query);
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

        public DataResponse<EntityBusinessProfile> Update(EntityBusinessProfile entity)
        {
            var response = new DataResponse<EntityBusinessProfile>();
            try
            {
                base.DBInit();

                var model = DBEntity.BusinessMasters.FirstOrDefault(a => a.Id == entity.Id);
                model.DomainUrl = entity.DomainUrl;
                model.BusinessName = entity.BusinessName;
                model.Description = entity.Description;
                model.About = entity.About;
                model.Address = entity.Address;
                model.City = entity.City;
                model.Country = entity.Country;
                model.State = entity.State;
                model.IsActive = entity.IsActive;
                model.DomainUrl = entity.DomainUrl;
                model.DateRange = entity.DateRange;
                model.SalesGroupBy = entity.SalesGroup;
                model.CreatedBy = entity.CreatedBy;
                model.RelativeUrl = entity.RelativeUrl;
                model.OtherEmails = entity.OtherEmails;
                model.IsActive = entity.IsActive;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;

                if (base.DBSaveUpdate(model) > 0)
                {
                    return GetBusinessProfileById(model.Id);
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

        public DataResponse<EntityList<EntityBusinessMaster>> GetAllBusinesses(FilterBusiness filter, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityBusinessMaster>>();
            try
            {
                base.DBInit();

                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                var query = DBEntity.BusinessMasters.Where(a => 1 == 1);
                if (!string.IsNullOrEmpty(filter.KeyWords))
                {
                    query = query.Where(ua => ua.BusinessName.ToLower().Contains(filter.KeyWords.ToLower()));
                }

                var selectQuery = query.Select(a => new EntityBusinessMaster
                {
                    Id = a.Id,
                    BusinessName = a.BusinessName,
                    Description = a.Description,
                    RelativeUrl = a.RelativeUrl,
                    IsActive = a.IsActive,
                    Status = a.Status,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    UpdatedUser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn
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

                response = GetList<EntityBusinessMaster>(selectQuery, skip, take);
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

        public DataResponse<EntityBusinessMaster> Insert(EntityBusinessMaster entity)
        {
            var response = new DataResponse<EntityBusinessMaster>();
            try
            {
                base.DBInit();

                var model = new Database.BusinessMaster
                {
                    BusinessName = entity.BusinessName,
                    RelativeUrl = entity.BusinessName.ToLower().Replace(" ", "-"),
                    Description = entity.Description,
                    CreatedBy = 1,
                    CreatedOn = System.DateTime.UtcNow,
                    IsActive = true,
                    Status = 1
                };

                if (base.DBSave(model) > 0)
                {
                    entity.Id = model.Id;
                    response.CreateResponse<EntityBusinessMaster>(entity, DataResponseStatus.OK);
                    return response;
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

        public DataResponse<EntityBusinessMaster> UpdateBusiness(EntityBusinessMaster entity)
        {
            var response = new DataResponse<EntityBusinessMaster>();
            try
            {
                base.DBInit();

                var model = DBEntity.BusinessMasters.FirstOrDefault(a => a.Id == entity.Id);
                model.BusinessName = entity.BusinessName;
                model.Description = entity.Description;
                model.IsActive = entity.IsActive;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;

                if (base.DBSaveUpdate(model) > 0)
                {
                    return GetBusinessById(model.Id);
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

        public DataResponse<EntityBusinessMaster> GetBusinessById(int BusinessId)
        {
            var response = new DataResponse<EntityBusinessMaster>();
            try
            {
                base.DBInit();

                var query = DBEntity.BusinessMasters.Where(a => a.Id == BusinessId).Select(a => new EntityBusinessMaster
                {
                    Id = a.Id,
                    BusinessName = a.BusinessName,
                    Description = a.Description,
                    RelativeUrl = a.RelativeUrl,
                    IsActive = a.IsActive,
                    Status = a.Status,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    UpdatedUser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn
                });
                response = GetFirst<EntityBusinessMaster>(query);

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

        public DataResponse<bool> DeleteBusiness(int id)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                var model = DBEntity.BusinessMasters.Find(id);
                DBEntity.BusinessMasters.Remove(model);
                if (DBEntity.SaveChanges() > 0)
                {
                    response.Message = "Success";
                    response.Status = DataResponseStatus.OK;
                    response.Model = true;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                response.Message = "Error, There are some related item in database, please delete those first";
                response.Model = false;
            }
            finally
            {
                base.DBClose();
            }
            return response;
        }
    }
}
