using EBP.Business.Entity;
using EBP.Business.Entity.Practice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Repository
{
    public class RepositoryPractice : _Repository
    {
        public DataResponse<EntityList<EntitySelectItem>> GetAllPractices(int? PracticeType, string PracticeName, int? businessId, int currentUserId, bool isbuzAdmin)
        {
            var response = new DataResponse<EntityList<EntitySelectItem>>();
            try
            {
                base.DBInit();

                var query = DBEntity.Practices.Where(a => a.BusinessId == businessId);

                if (!isbuzAdmin)
                {
                    var reps = new List<int>();
                    reps = DBEntity.Reps.Where(a => a.RepGroup.RepgroupManagerMappers.Any(b => b.ManagerId == currentUserId)).Select(a => a.UserId).ToList();//|| a.RepGroup.SalesDirectorId == currentUserId
                    //---ManagerChange      reps = DBEntity.Reps.Where(a => a.RepGroup.ManagerId == currentUserId).Select(a => a.UserId).ToList();
                    reps.Add(currentUserId);

                    query = query.Where(a => reps.Contains(a.Rep.UserId));
                }

                if (PracticeType != null && PracticeType > 0)
                    query = query.Where(a => a.PracticeTypeId == PracticeType);

                if (!string.IsNullOrEmpty(PracticeName))
                    query = query.Where(a => a.PracticeName.Contains(PracticeName));

                var selectQuery = query.Select(a => new EntitySelectItem
                {
                    Id = a.Id,
                    Value = a.PracticeName
                }).OrderBy(o => o.Value);

                response = GetList<EntitySelectItem>(selectQuery, 0, 1500);
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

        public DataResponse SetApiActiveFlagStatus(int Id,bool isLead, bool isActive, bool flag)
        {
            var response = new DataResponse<int?>();

            try
            {
                base.DBInit();

                if (isLead == true)
                {
                    var model = DBEntity.Leads.FirstOrDefault(a => a.Id == Id);
                    // response.Model = model.IsActive;

                    if (model != null)
                    {
                        model.HasFlag = flag;
                        model.IsActive = isActive;
                    }
                    if (base.DBSaveUpdate(model) > 0)
                    {
                        // response.Model = model.IsActive;
                        response.Model = model.Id;
                    }
                }
                else
                {
                    var model = DBEntity.Accounts.FirstOrDefault(a => a.Id == Id);
                    // response.Model = model.IsActive;

                    if (model != null)
                    {
                        model.IsActive = isActive;
                        model.Lead.HasFlag = flag;
                    }
                    if (base.DBSaveUpdate(model) > 0)
                    {
                        // response.Model = model.IsActive;
                        response.Model = model.Id;
                    }
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
