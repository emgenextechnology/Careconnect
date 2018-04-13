using EBP.Business.Entity;
using EBP.Business.Entity.States;
using EBP.Business.Entity.Practice;
using EBP.Business.Entity.Rep;
using EBP.Business.Enums;
using EBP.Business.Filter;
using EBP.Business.Resource;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;


namespace EBP.Business.Repository
{
    public class RepositoryStates : _Repository
    {
        public DataResponse<EntityList<EntityStates>> GetAllStates(FilterStates filter, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityStates>>();
            try
            {
                base.DBInit();

                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                var query = DBEntity.LookupStates.Where(a => 1== 1);
                if (!string.IsNullOrEmpty(filter.KeyWords))
                {
                    query = query.Where(ua => ua.StateName.ToLower().Contains(filter.KeyWords.ToLower()) || ua.StateCode.ToLower().Contains(filter.KeyWords.ToLower()));
                }
                var selectQuery = query.Select(a => new EntityStates
                {
                    Id = a.Id,
                    StateName = a.StateName,
                    StateCode = a.StateCode,
                    CountryCode = a.LookupCountry.CountryCode,
                    CreatedOn = a.CreatedOn,
                    CreatedUser = a.User.FirstName,
                    UpdatedOn = a.UpdatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
                    IsActive = a.IsActive
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

                response = GetList<EntityStates>(selectQuery, skip, take);
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

        public DataResponse<EntityStates> Insert(EntityStates entity)
        {
            var response = new DataResponse<EntityStates>();
            try
            {
                base.DBInit();

                var model = new Database.LookupState
                {
                    StateName = entity.StateName,
                    StateCode = entity.StateCode,
                    CountryId = entity.CountryId,
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = System.DateTime.UtcNow,
                    IsActive = entity.IsActive
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

        public DataResponse<EntityStates> GetStateById(int id)
        {
            var response = new DataResponse<EntityStates>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupStates.Where(a => a.Id == id).Select(a => new EntityStates
                {
                    Id = a.Id,
                    StateName = a.StateName,
                    StateCode = a.StateCode,
                    CountryCode = a.LookupCountry.CountryCode,
                    CountryId = a.CountryId,
                    IsActive = a.IsActive,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn
                });
                response = GetFirst<EntityStates>(query);

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

        public DataResponse<EntityStates> UpdateState(EntityStates entity)
        {
            var response = new DataResponse<EntityStates>();
            try
            {
                base.DBInit();

                var model = DBEntity.LookupStates.FirstOrDefault(a => a.Id == entity.Id);
                model.StateName = entity.StateName;
                model.StateCode = entity.StateCode;
                model.CountryId = entity.CountryId;
                model.IsActive = entity.IsActive;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;

                if (base.DBSaveUpdate(model) > 0)
                {
                    return GetStateById(model.Id);
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

        public DataResponse<bool> DeleteState(int id)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                var model = DBEntity.LookupStates.Find(id);
                DBEntity.LookupStates.Remove(model);
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
