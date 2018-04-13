using EBP.Business.Entity;
using EBP.Business.Entity.PrivilegeModules;
using EBP.Business.Entity.Privileges;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
    public class RepositoryPrivilege : _Repository
    {
        public DataResponse<EntityList<EntityPrivilege>> GetAllPrivilege(FilterPrivilege filter, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityPrivilege>>();
            try
            {
                base.DBInit();
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                var query = DBEntity.Privileges.Where(a => 1 == 1);
                if (!string.IsNullOrEmpty(filter.KeyWords))
                {
                    query = query.Where(ua => ua.Title.ToLower().Contains(filter.KeyWords.ToLower()) || ua.PrivilegeKey.ToLower().Contains(filter.KeyWords.ToLower()));
                }

                var selectQuery = query.Select(a => new EntityPrivilege
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    PrivilegeKey = a.PrivilegeKey,
                    Module = a.ModuleId != null ? a.ModulesMaster.Title : null,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
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

                response = GetList<EntityPrivilege>(selectQuery, skip, take);
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
      
        public DataResponse<EntityPrivilege> Insert(EntityPrivilege entity)
        {
            var response = new DataResponse<EntityPrivilege>();
            try
            {
                base.DBInit();

                var model = new Database.Privilege
                {
                    Title = entity.Title,
                    Description = entity.Description,
                    PrivilegeKey = entity.PrivilegeKey,
                    ModuleId = entity.ModuleId,
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
       
        public DataResponse<EntityPrivilege> GetPrivilegeById(int id)
        {
            var response = new DataResponse<EntityPrivilege>();
            try
            {
                base.DBInit();

                var query = DBEntity.Privileges.Where(a => a.Id == id).Select(a => new EntityPrivilege
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    ModuleId = a.ModuleId,
                    PrivilegeKey = a.PrivilegeKey,
                    Module = a.ModulesMaster.Title,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn
                });
                response = GetFirst<EntityPrivilege>(query);

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
        
        public DataResponse<EntityPrivilege> UpdatePrivilegeById(EntityPrivilege entity)
        {
            var response = new DataResponse<EntityPrivilege>();
            try
            {
                base.DBInit();

                var model = DBEntity.Privileges.FirstOrDefault(a => a.Id == entity.Id);
                model.Title = entity.Title;
                model.Description = entity.Description;
                model.PrivilegeKey = entity.PrivilegeKey;
                model.ModuleId = entity.ModuleId;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;

                if (base.DBSaveUpdate(model) > 0)
                {
                    return GetPrivilegeById(model.Id);
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
      
        public DataResponse<bool> DeletePrivilege(int id)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                var model = DBEntity.Privileges.Find(id);
                DBEntity.Privileges.Remove(model);
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
