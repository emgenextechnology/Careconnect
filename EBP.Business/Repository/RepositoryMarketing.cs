using EBP.Business.Entity;
using EBP.Business.Entity.Marketing;
using EBP.Business.Enums;
using EBP.Business.Filter;
using EBP.Business.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Repository
{
    public class RepositoryMarketing : _Repository
    {
        public DataResponse<EntityList<EntityMarketing>> GetAllList(FilterMarketing filter, int? businessId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityMarketing>>();
            try
            {
                base.DBInit();
                var query = DBEntity.MarketingDocuments.Where(a => a.BusinessId == businessId);
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                    if (!string.IsNullOrEmpty(filter.Keyword))
                        query = query.Where(a => a.Name.ToLower().Contains(filter.Keyword.ToLower()) 
                            || a.Description.ToLower().Contains(filter.Keyword.ToLower()));
                    if (filter.CategoryId != null)
                        query = query.Where(a => a.CategoryId == filter.CategoryId);
                    if (filter.DocumentTypeId != null)
                        query = query.Where(a => a.DocumentTypeId == filter.DocumentTypeId);
                    if (filter.UserId != null)
                        query = query.Where(a => a.CreatedBy == filter.UserId);
                }

                var selectQuery = query.Select(a => new EntityMarketing
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    DocumentTypeId = a.DocumentTypeId,
                    CategoryId = a.CategoryId,
                    CreatedOn = a.CreatedOn,
                }).OrderByDescending(o => o.CreatedOn);

                response = GetList<EntityMarketing>(selectQuery, skip, take);
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
        public DataResponse<EntityMarketing> Update(EntityMarketing entity)
        {
            var response = new DataResponse<EntityMarketing>();
            try
            {
                base.DBInit();
                var model = DBEntity.MarketingDocuments.FirstOrDefault(a => a.Id == entity.Id);

                model.Name = entity.Name;
                model.Description = entity.Description;
                model.DocumentTypeId = entity.DocumentTypeId;
                model.CategoryId = entity.CategoryId;
                model.UpdatedBy = entity.CurrentUserId;
                model.UpdatedOn = System.DateTime.UtcNow;
                if (DBEntity.SaveChanges() > 0)
                {
                    return GetMarketingById(model.Id, entity.CurrentUserId, entity.BusinessId);
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
        public DataResponse<EntityMarketing> Insert(EntityMarketing entity)
        {
            var response = new DataResponse<EntityMarketing>();
            try
            {
                base.DBInit();

                var model = new Database.MarketingDocument
                {
                    Name = entity.Name,
                    DocumentTypeId = entity.DocumentTypeId,
                    Description = entity.Description,
                    //Url=entity.Url,
                    CategoryId = entity.CategoryId,
                    CreatedOn = entity.CreatedOn,
                    CreatedBy = entity.CreatedBy,
                    BusinessId = entity.BusinessId,
                };

                if (base.DBSave(model) > 0)
                {
                    entity.Id = model.Id;

                    response.CreateResponse<EntityMarketing>(entity, DataResponseStatus.OK);
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

        public bool SaveFiles(List<string> FilesList, int MarketingId, bool isEdit, int CurrentUserId)
        {
            try
            {
                base.DBInit();
                foreach (var fileName in FilesList)
                {
                    var attachmentModel = new Database.MarketingAttachment
                    {
                        FileName = fileName,
                        MarketingId = MarketingId,
                        CreatedBy = CurrentUserId,
                        CreatedOn = System.DateTime.UtcNow
                    };

                    EntityAdd<Database.MarketingAttachment>(attachmentModel);
                }

                if (DBEntity.SaveChanges() <= 0)
                    return false;
            }
            catch (Exception ex)
            {
                ex.Log();
                return false;
            }
            finally
            {
                base.DBClose();
            }
            return true;
        }

        public DataResponse<EntityMarketing> GetMarketingById(int MarketingId, int currentUserId, int? currentBusinessId)
        {
            var response = new DataResponse<EntityMarketing>();
            try
            {
                base.DBInit();

                var query = DBEntity.MarketingDocuments.Where(a => a.Id == MarketingId).Select(a => new EntityMarketing
                {
                    Id = a.Id,
                    BusinessId = a.BusinessId,
                    Name = a.Name,
                    Description = a.Description,
                    CategoryId = a.CategoryId,
                    DocumentTypeId = a.DocumentTypeId,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedBy = a.UpdatedBy,
                    UpdatedOn = a.UpdatedOn,
                    FilesList = a.MarketingAttachments.Where(c => c.IsActive != false).Select(d => new FilesUploaded { Id = d.Id, FileName = d.FileName }),
                });
                response = GetFirst<EntityMarketing>(query);
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

        
        public DataResponse<bool> DeleteFile(int FileId)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();

                var model = DBEntity.MarketingAttachments.FirstOrDefault(a => a.Id == FileId);
                if (model != null)
                {
                    model.IsActive = false;
                }
                if (DBEntity.SaveChanges() > 0)
                {
                    response.Model = true;
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
