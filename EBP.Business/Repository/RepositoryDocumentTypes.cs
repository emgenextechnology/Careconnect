using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.DocumentType;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
    public class RepositoryDocumentTypes : _Repository
    {
        public DataResponse<EntityList<EntityDocumentTypes>> GetDocumentTypes(FilterDocumentTypes filter, int? currentBusineId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityDocumentTypes>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                base.DBInit();
                var query = DBEntity.LookupDocumentTypes.Where(a => a.BusinessId == currentBusineId);
                if (filter != null)
                {
                    if (!String.IsNullOrEmpty(filter.KeyWords))
                    {
                        query = query.Where(a => a.DocumentType.ToLower().Contains(filter.KeyWords));
                    }
                }
                var selectQuery = query.Select(a => new EntityDocumentTypes
                {
                    Id = a.Id,
                    DocumentType = a.DocumentType,
                    CreatedBy=a.CreatedBy,
                    CreatedOn=a.CreatedOn,
                    UpdatedBy=a.UpdatedBy,
                    UpdatedOn=a.UpdatedOn,
                    BusinessId = a.BusinessId,
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

                response = GetList<EntityDocumentTypes>(selectQuery, skip, take);
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
        public DataResponse<EntityDocumentTypes> GetDocumentTypeById(int DocumentTypesId)
        {
            var response = new DataResponse<EntityDocumentTypes>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupDocumentTypes.Where(a => a.Id == DocumentTypesId).Select(a => new EntityDocumentTypes
                {
                    Id = a.Id,
                    DocumentType = a.DocumentType,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedOn=a.UpdatedOn,
                    UpdatedBy=a.UpdatedBy,
                    BusinessId = a.BusinessId,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
                });
                response = GetFirst<EntityDocumentTypes>(query);

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
        public DataResponse<EntityDocumentTypes> Update(EntityDocumentTypes entity)
        {
            var response = new DataResponse<EntityDocumentTypes>();
            try
            {
                base.DBInit();


                #region Prepare model
                var model = DBEntity.LookupDocumentTypes.FirstOrDefault(a => a.Id == entity.Id);
                model.DocumentType = entity.DocumentType;
                model.UpdatedBy = entity.UpdatedBy;
                model.UpdatedOn = entity.UpdatedOn;
                #endregion

                if (base.DBSaveUpdate(model) > 0)
                {
                   
                    return GetDocumentTypeById(model.Id);
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
        public DataResponse<EntityDocumentTypes> Insert(EntityDocumentTypes entity)
        {
            var response = new DataResponse<EntityDocumentTypes>();
            try
            {
                base.DBInit();

                var model = new Database.LookupDocumentType
                {
                    DocumentType=entity.DocumentType,
                    BusinessId=entity.BusinessId,
                    CreatedOn = entity.CreatedOn,
                    CreatedBy = entity.CreatedBy

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

        public DataResponse<bool> Delete(int Documenttypeid)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                LookupDocumentType lookupDocumentType = DBEntity.LookupDocumentTypes.Find(Documenttypeid);
                try
                {
                    DBEntity.LookupDocumentTypes.Remove(lookupDocumentType);
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
    }
}
