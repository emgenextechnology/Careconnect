using EBP.Business.Entity;
using EBP.Business.Entity.PracticeSpeciality;
using EBP.Business.Entity.PracticeType;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
    public class RepositoryPracticeType:_Repository
    {
        public DataResponse<EntityList<EntityPracticeType>> GetAllPracticeType(FilterPracticeType filter,int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityPracticeType>>();
            try
            {
                base.DBInit();

                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                var query = DBEntity.LookupPracticeTypes.Where(a=>1==1);
                if (!string.IsNullOrEmpty(filter.KeyWords))
                {
                    query = query.Where(ua => ua.PracticeType.ToLower().Contains(filter.KeyWords.ToLower()));
                }               

                var selectQuery = query.Select(a => new EntityPracticeType
                {
                    Id = a.Id,
                    PracticeType = a.PracticeType,
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

                response = GetList<EntityPracticeType>(selectQuery, skip, take);
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
       
        public DataResponse<EntityPracticeType> Insert(EntityPracticeType entity)
        {
            var response = new DataResponse<EntityPracticeType>();
            try
            {
                base.DBInit();

                var model = new Database.LookupPracticeType
                {
                    PracticeType = entity.PracticeType,
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
       
        public DataResponse<EntityPracticeType> GetPracticeTypeById(int id)
        {
            var response = new DataResponse<EntityPracticeType>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupPracticeTypes.Where(a => a.Id == id).Select(a => new EntityPracticeType
                {
                    Id = a.Id,
                    PracticeType = a.PracticeType,
                    IsActive = a.IsActive,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn
                });
                response = GetFirst<EntityPracticeType>(query);

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
      
        public DataResponse<EntityPracticeType> UpdatePracticeSpeciality(EntityPracticeType entity)
        {
            var response = new DataResponse<EntityPracticeType>();
            try
            {
                base.DBInit();

                var model = DBEntity.LookupPracticeTypes.FirstOrDefault(a => a.Id == entity.Id);
                model.PracticeType = entity.PracticeType;
                model.IsActive = entity.IsActive;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;

                if (base.DBSaveUpdate(model) > 0)
                {
                    return GetPracticeTypeById(model.Id);
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
        
        public DataResponse<bool> DeletePracticeType(int id)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                var model = DBEntity.LookupPracticeTypes.Find(id);
                DBEntity.LookupPracticeTypes.Remove(model);
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
