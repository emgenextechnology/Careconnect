using EBP.Business.Entity;
using EBP.Business.Entity.PracticeSpeciality;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
   public class RepositoryPracticeSpeciality:_Repository
    {
        public DataResponse<EntityList<EntityPracticeSpecialityType>> GetAllSpecialityType(PracticeSpecialityFilter filter,int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityPracticeSpecialityType>>();
            try
            {
                base.DBInit();
              
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }

                var query = DBEntity.LookupPracticeSpecialityTypes.Where(a=>1==1);
                if (!string.IsNullOrEmpty(filter.KeyWords))
                {
                    query = query.Where(ua => ua.PracticeSpecialityType.ToLower().Contains(filter.KeyWords.ToLower()));
                }

                var selectQuery = query.Select(a => new EntityPracticeSpecialityType
                {
                    Id = a.Id,
                    PracticeSpecialityType=a.PracticeSpecialityType,
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

                response = GetList<EntityPracticeSpecialityType>(selectQuery, skip, take);
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
     
        public DataResponse<EntityPracticeSpecialityType> Insert(EntityPracticeSpecialityType entity)
        {
            var response = new DataResponse<EntityPracticeSpecialityType>();
            try
            {
                base.DBInit();

                var model = new Database.LookupPracticeSpecialityType
                {
                    PracticeSpecialityType=entity.PracticeSpecialityType,                   
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
       
        public DataResponse<EntityPracticeSpecialityType> GetPracticeSpecialityTypeById(int id)
        {
            var response = new DataResponse<EntityPracticeSpecialityType>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupPracticeSpecialityTypes.Where(a => a.Id == id).Select(a => new EntityPracticeSpecialityType
                {
                    Id = a.Id,
                    PracticeSpecialityType=a.PracticeSpecialityType,
                    IsActive = a.IsActive,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn
                });
                response = GetFirst<EntityPracticeSpecialityType>(query);

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
        
        public DataResponse<EntityPracticeSpecialityType> UpdatePracticeSpeciality(EntityPracticeSpecialityType entity)
        {
            var response = new DataResponse<EntityPracticeSpecialityType>();
            try
            {
                base.DBInit();

                #region Prepare model
                var model = DBEntity.LookupPracticeSpecialityTypes.FirstOrDefault(a => a.Id == entity.Id);
                model.PracticeSpecialityType = entity.PracticeSpecialityType;               
                model.IsActive = entity.IsActive;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;
                #endregion

                if (base.DBSaveUpdate(model) > 0)
                {
                    return GetPracticeSpecialityTypeById(model.Id);
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
        
        public DataResponse<bool> DeletePracticeSpeciality(int id)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                var model = DBEntity.LookupPracticeSpecialityTypes.Find(id);
                DBEntity.LookupPracticeSpecialityTypes.Remove(model);
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
