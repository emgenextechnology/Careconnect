using EBP.Business;
using EBP.Business.Entity.Practice;
using EBP.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace EmgenexBusinessPortal.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("Provider")]
    public class APIProviderController : ApiBaseController
    {
        [Route("getGlobalProviderByNPI")]
        [AllowAnonymous]
        public async Task<string> GetObjectsAsync(string NPI)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    return await httpClient.GetStringAsync("https://npiregistry.cms.hhs.gov/api/?number=" + NPI);
                }
            }
            catch (Exception ex)
            {
                ex.Log();

                return "{}";
            }
        }

        [Route("getProviderByNPI")]
        public IHttpActionResult GetTaskById(string NPI)
        {
            var response = new DataResponse<EntityProvider>();
            var repository = new RepositoryProvider();
            if (!string.IsNullOrEmpty(NPI))
            {
                response = repository.GetProviderByNPI(NPI);
            }
            else
            {
                response.Model = new EntityProvider();
            }
            return Ok<dynamic>(new { IsSuccess = 1, FromDb = response.Model != null, Status = HttpStatusCode.OK, model = response.Model == null ? null : response });
        }
    }
}
