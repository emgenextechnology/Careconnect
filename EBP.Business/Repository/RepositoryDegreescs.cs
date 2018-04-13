using EBP.Business.Entity;
using EBP.Business.Entity.Degree;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
    public class RepositoryDegreescs : _Repository
    {
        public DataResponse<EntityList<EntityDegrees>> GetAllDegrees(FilterDegree filter, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityDegrees>>();
            try
            {
                base.DBInit();
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                var query = DBEntity.LookupDegrees.Where(a => 1 == 1);
                if (!string.IsNullOrEmpty(filter.KeyWords))
                {
                    query = query.Where(ua => ua.DegreeName.ToLower().Contains(filter.KeyWords.ToLower()) || ua.ShortCode.ToLower().Contains(filter.KeyWords.ToLower()));
                }

                var selectQuery = query.Select(a => new EntityDegrees
                {
                    Id = a.Id,
                    DegreeName = a.DegreeName,
                    ShortCode = a.ShortCode,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn,
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
                response = GetList<EntityDegrees>(selectQuery, skip, take);
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

        public DataResponse<EntityDegrees> Insert(EntityDegrees entity)
        {
            var response = new DataResponse<EntityDegrees>();
            try
            {
                base.DBInit();

                var model = new Database.LookupDegree
                {
                    DegreeName = entity.DegreeName,
                    ShortCode = entity.ShortCode,
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

        public DataResponse<EntityDegrees> GetDegreeById(int id)
        {
            var response = new DataResponse<EntityDegrees>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupDegrees.Where(a => a.Id == id).Select(a => new EntityDegrees
                {
                    Id = a.Id,
                    DegreeName = a.DegreeName,
                    ShortCode = a.ShortCode,
                    IsActive = a.IsActive,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn
                });
                response = GetFirst<EntityDegrees>(query);

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

        public DataResponse<EntityDegrees> UpdateDegree(EntityDegrees entity)
        {
            var response = new DataResponse<EntityDegrees>();
            try
            {
                base.DBInit();

                #region Prepare model
                var model = DBEntity.LookupDegrees.FirstOrDefault(a => a.Id == entity.Id);
                model.DegreeName = entity.DegreeName;
                model.ShortCode = entity.ShortCode;
                model.IsActive = entity.IsActive;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;
                #endregion

                if (base.DBSaveUpdate(model) > 0)
                {
                    return GetDegreeById(model.Id);
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

        public DataResponse<bool> DeleteDegree(int id)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                var model = DBEntity.LookupDegrees.Find(id);
                DBEntity.LookupDegrees.Remove(model);
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
