using EBP.Business.Entity;
using EBP.Business.Entity.LeadSource;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
    public class RepositoryLeadSource : _Repository
    {
        public DataResponse<EntityList<EntityLeadSource>> GetAllLeadSources(FilterLeadSource filter, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityLeadSource>>();
            try
            {
                base.DBInit();

                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }

                var query = DBEntity.LookupLeadSources.Where(a=>1 == 1);

                if (!string.IsNullOrEmpty(filter.KeyWords))
                {
                    query = query.Where(ua => ua.LeadSource.ToLower().Contains(filter.KeyWords.ToLower()));
                }

                var selectQuery = query.Select(a => new EntityLeadSource
                {
                    Id = a.Id,
                    LeadSource = a.LeadSource,
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
                response = GetList<EntityLeadSource>(selectQuery, skip, take);
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

        public DataResponse<EntityLeadSource> Insert(EntityLeadSource entity)
        {
            var response = new DataResponse<EntityLeadSource>();
            try
            {
                base.DBInit();

                var model = new Database.LookupLeadSource
                {
                    LeadSource = entity.LeadSource,
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

        public DataResponse<EntityLeadSource> GetLeadsourceById(int id)
        {
            var response = new DataResponse<EntityLeadSource>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupLeadSources.Where(a => a.Id == id).Select(a => new EntityLeadSource
                {
                    Id = a.Id,
                    LeadSource = a.LeadSource,
                    IsActive = a.IsActive,
                    CreatedUser = a.User.FirstName,
                    CreatedOn = a.CreatedOn,
                    Updateduser = a.User1 == null ? null : a.User1.FirstName,
                    UpdatedOn = a.UpdatedOn
                });
                response = GetFirst<EntityLeadSource>(query);
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

        public DataResponse<EntityLeadSource> UpdateLeadSource(EntityLeadSource entity)
        {
            var response = new DataResponse<EntityLeadSource>();
            try
            {
                base.DBInit();

                var model = DBEntity.LookupLeadSources.FirstOrDefault(a => a.Id == entity.Id);
                model.LeadSource = entity.LeadSource;
                model.IsActive = entity.IsActive;
                model.UpdatedOn = entity.UpdatedOn;
                model.UpdatedBy = entity.UpdatedBy;

                if (base.DBSaveUpdate(model) > 0)
                {
                    return GetLeadsourceById(model.Id);
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

        public DataResponse<bool> DeleteLeadSource(int id)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                var model = DBEntity.LookupLeadSources.Find(id);
                DBEntity.LookupLeadSources.Remove(model);
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
