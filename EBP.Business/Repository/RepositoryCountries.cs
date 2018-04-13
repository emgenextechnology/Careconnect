using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.Country;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
    public class RepositoryCountries : _Repository
    {
        public DataResponse<EntityList<EntityCountry>> GetAllCountries(FilterCountry filter, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityCountry>>();
            try
            {
                base.DBInit();
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                var query = DBEntity.LookupCountries.Where(a => 1 == 1);
                if (!string.IsNullOrEmpty(filter.KeyWords))
                {
                    query = query.Where(ua => ua.CountryName.ToLower().Contains(filter.KeyWords.ToLower()) || ua.CountryCode.ToLower().Contains(filter.KeyWords.ToLower()));
                }

                var selectQuery = query.Select(a => new EntityCountry
                {
                    Id = a.Id,
                    CountryName = a.CountryName,
                    CountryCode = a.CountryCode,
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
                response = GetList<EntityCountry>(selectQuery, skip, take);
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

        public DataResponse<EntityCountry> Insert(EntityCountry entity)
        {
            var response = new DataResponse<EntityCountry>();
            try
            {
                base.DBInit();

                var model = new Database.LookupCountry
                {
                    CountryName = entity.CountryName,
                    CountryCode = entity.CountryCode,
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
                response.ThrowError(ex);
            }
            finally
            {
                base.DBClose();
            }

            return response;
        }

        public DataResponse<EntityCountry> GetCountryById(int id)
        {
            var response = new DataResponse<EntityCountry>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupCountries.Where(a => a.Id == id).Select(a => new EntityCountry
                {
                    Id = a.Id,
                    CountryName = a.CountryName,
                    CountryCode = a.CountryCode,
                    IsActive = a.IsActive,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn
                });

                response = GetFirst<EntityCountry>(query);
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

        public DataResponse<EntityCountry> UpdateCountry(EntityCountry entity)
        {
            var response = new DataResponse<EntityCountry>();
            try
            {
                base.DBInit();

                var model = DBEntity.LookupCountries.FirstOrDefault(a => a.Id == entity.Id);
                model.CountryName = entity.CountryName;
                model.CountryCode = entity.CountryCode;
                model.IsActive = entity.IsActive;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;

                if (base.DBSaveUpdate(model) > 0)
                {
                    return GetCountryById(model.Id);
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

        public DataResponse<bool> DeleteCountry(int id)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                var model = DBEntity.LookupCountries.Find(id);
                DBEntity.LookupCountries.Remove(model);
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
