using EBP.Business.Entity;
using EBP.Business.Entity.UserColumn;
using EBP.Business.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Repository
{
    public class RepositoryColumnVisibility : _Repository
    {
        public DataResponse<EntityList<EntityUserColumn>> GetAllColumns(int moduleId, int? businessId, int currentUserId, int? serviceId)
        {
            var response = new DataResponse<EntityList<EntityUserColumn>>();
            try
            {
                base.DBInit();

                var query = DBEntity.UserColumnVisibilities.Where(a => a.ModuleId == moduleId && a.BusinessId == businessId && a.UserId == currentUserId);

                if (serviceId != null && serviceId > 0)
                    query = query.Where(a => a.ServiceId == serviceId);

                var selectQuery = query.Select(a => new EntityUserColumn
                {
                    Id = a.Id,
                    ColumnName = a.ColumnName,
                    ServiceId = a.ServiceId
                }).OrderBy(o => o.Id);

                response = GetList<EntityUserColumn>(selectQuery, 0, 1000);
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

        //public bool SaveColumns(EntityUserColumn EntityColumn)
        //{
        //    bool isSuccess = false;
        //    try
        //    {
        //        base.DBInit();

        //        if (EntityColumn.IsChecked)
        //        {
        //            var objUserColumn = DBEntity.UserColumnVisibilities.Add(new Database.UserColumnVisibility
        //             {
        //                 ModuleId = EntityColumn.ModuleId,
        //                 BusinessId = EntityColumn.BusinessId,
        //                 ColumnName = EntityColumn.ColumnName,
        //                 ServiceId = EntityColumn.ServiceId,
        //                 UserId = EntityColumn.UserId,
        //                 CreatedOn = DateTime.UtcNow,
        //                 CreatedBy = EntityColumn.CreatedBy
        //             });

        //            return base.DBSave(objUserColumn) > 0;
        //        }
        //        else
        //        {
        //            var objUserColumn = DBEntity.UserColumnVisibilities.FirstOrDefault(a => a.ModuleId == EntityColumn.ModuleId && a.BusinessId == EntityColumn.BusinessId &&
        //                a.ColumnName == EntityColumn.ColumnName && a.ServiceId == EntityColumn.ServiceId);

        //            if (objUserColumn != null)
        //                return base.DBDelete(objUserColumn) > 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.Log();
        //    }
        //    finally
        //    {
        //        base.DBClose();
        //    }
        //    return isSuccess;
        //}
    }
}
