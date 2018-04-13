using EBP.Business.Entity;
using EBP.Business.Entity.PrivilegeModules;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
   public  class RepositoryPrivilegeModules:_Repository
    {
        public DataResponse<EntityList<EntityPrivilegeModules>> GetAllPrivilegeModules(FilterPrivilegeModule filter,int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityPrivilegeModules>>();
            try
            {
                base.DBInit();
               
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                var query = DBEntity.ModulesMasters.Where(a => 1 == 1);
                if (!string.IsNullOrEmpty(filter.KeyWords))
                {
                    query = query.Where(ua => ua.Title.ToLower().Contains(filter.KeyWords.ToLower()) || ua.Description.ToLower().Contains(filter.KeyWords.ToLower()));
                }

                var selectQuery = query.Select(a => new EntityPrivilegeModules
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn,
                    IsActive=a.IsActive
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

                response = GetList<EntityPrivilegeModules>(selectQuery, skip, take);
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
        
        public DataResponse<EntityPrivilegeModules> Insert(EntityPrivilegeModules entity)
        {
            var response = new DataResponse<EntityPrivilegeModules>();
            try
            {
                base.DBInit();

                var model = new Database.ModulesMaster
                {
                    Title = entity.Title,
                    Description = entity.Description,
                    IsActive=entity.IsActive,
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
       
        public DataResponse<EntityPrivilegeModules> GetPrivilegeModulesById(int id)
        {
            var response = new DataResponse<EntityPrivilegeModules>();
            try
            {
                base.DBInit();

                var query = DBEntity.ModulesMasters.Where(a => a.Id == id).Select(a => new EntityPrivilegeModules
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn,
                    IsActive=a.IsActive
                });
                response = GetFirst<EntityPrivilegeModules>(query);
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
       
        public DataResponse<EntityPrivilegeModules> UpdatePrivilegeModuleById(EntityPrivilegeModules entity)
        {
            var response = new DataResponse<EntityPrivilegeModules>();
            try
            {
                base.DBInit();

                var model = DBEntity.ModulesMasters.FirstOrDefault(a => a.Id == entity.Id);
                model.Title = entity.Title;
                model.Description = entity.Description;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;
                model.IsActive = entity.IsActive;

                if (base.DBSaveUpdate(model) > 0)
                {
                    return GetPrivilegeModulesById(model.Id);
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
       
        public DataResponse<bool> DeletePrivilegeModule(int id)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                var model = DBEntity.ModulesMasters.Find(id);
                DBEntity.ModulesMasters.Remove(model);
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
