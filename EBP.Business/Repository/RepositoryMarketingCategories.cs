using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.MarketingCategory;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Repository
{
    public class RepositoryMarketingCategories : _Repository
    {
        public DataResponse<EntityList<EntityMarketingCategories>> GetMarketingCategories(FilterMarketingCategories filter, int currentBusineId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityMarketingCategories>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }
                base.DBInit();
                var query = DBEntity.LookupMarketingCategories.Where(a => a.BusinessId == currentBusineId);
                if (filter != null)
                {
                    if (!String.IsNullOrEmpty(filter.KeyWords))
                    {
                        query = query.Where(a => a.Category.ToLower().Contains(filter.KeyWords));
                    }
                }
                var selectQuery = query.Select(a => new EntityMarketingCategories
                {
                    Id = a.Id,
                    Category = a.Category,
                    CreatedBy=a.CreatedBy,
                    CreatedOn=a.CreatedOn,
                    UpdatedBy=a.UpdatedBy,
                    UpdatedOn=a.UpdatedOn,
                    BusinessId = a.BusinessId,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName = a.User1.FirstName + " " + a.User1.LastName,
                }).OrderByDescending(a=>a.CreatedOn);

                response = GetList<EntityMarketingCategories>(selectQuery, skip, take);
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
        public DataResponse<EntityMarketingCategories> GetMarketingCategoryById(int MarketingCategoriesId)
        {
            var response = new DataResponse<EntityMarketingCategories>();
            try
            {
                base.DBInit();

                var query = DBEntity.LookupMarketingCategories.Where(a => a.Id == MarketingCategoriesId).Select(a => new EntityMarketingCategories
                {
                    Id = a.Id,
                    Category = a.Category,
                    CreatedBy = a.CreatedBy,
                    CreatedOn = a.CreatedOn,
                    UpdatedOn=a.UpdatedOn,
                    UpdatedBy=a.UpdatedBy,
                    BusinessId = a.BusinessId,
                    CreatedByName = a.User.FirstName + " " + a.User.LastName,
                    UpdatedByName=a.User1.FirstName + " " + a.User1.LastName,
                });
                response = GetFirst<EntityMarketingCategories>(query);

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
        public DataResponse<EntityMarketingCategories> Update(EntityMarketingCategories entity)
        {
            var response = new DataResponse<EntityMarketingCategories>();
            try
            {
                base.DBInit();


                #region Prepare model
                var model = DBEntity.LookupMarketingCategories.FirstOrDefault(a => a.Id == entity.Id);
                model.Category = entity.Category;
                model.UpdatedBy = entity.UpdatedBy;
                model.UpdatedOn = entity.UpdatedOn;
                #endregion

                if (base.DBSaveUpdate(model) > 0)
                {
                   
                    return GetMarketingCategoryById(model.Id);
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
        public DataResponse<EntityMarketingCategories> Insert(EntityMarketingCategories entity)
        {
            var response = new DataResponse<EntityMarketingCategories>();
            try
            {
                base.DBInit();

                var model = new Database.LookupMarketingCategory
                {
                    Category=entity.Category,
                    BusinessId=entity.BusinessId,
                    CreatedOn = entity.CreatedOn,
                    CreatedBy = entity.CreatedBy

                };
                if (base.DBSave(model) > 0)
                {
                    return GetMarketingCategoryById(model.Id);
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

        public DataResponse<bool> Delete(int markrtingcategoryid)
        {
            var response = new DataResponse<bool>();
            try
            {
                base.DBInit();
                LookupMarketingCategory lookupMarketingCategory = DBEntity.LookupMarketingCategories.Find(markrtingcategoryid);
                try
                {
                    DBEntity.LookupMarketingCategories.Remove(lookupMarketingCategory);
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
