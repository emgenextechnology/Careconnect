using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EBP.Business.Entity;
using EBP.Business.Entity.Lead;
using EBP.Business.Entity.Practice;
using EBP.Business.Filter;
using System.Web.UI.WebControls;
using System.Linq.Expressions;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using EBP.Business.Entity.Task;
using EBP.Business.Enums;
using EntityFramework.Extensions;
using System.ComponentModel.DataAnnotations;

namespace EBP.Business.Repository
{
    public class RepositoryProvider : _Repository
    {

        public DataResponse<EntityProvider> GetProviderByNPI(string NPI)
        {
            var response = new DataResponse<EntityProvider>();
            try
            {
                base.DBInit();

                var query = DBEntity.Providers.Where(a => a.IsActive == true & a.NPI == NPI).Select(a => new EntityProvider
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    MiddleName = a.MiddleName,
                    DegreeId = a.DegreeId
                });
                response = GetFirst<EntityProvider>(query);
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