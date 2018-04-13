using EBP.NotificationApp.Entity.User;
using EBP.NotificationApp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.NotificationApp.Repository
{
    public class RepositoryUsers : _Repository
    {


        public DataResponse<EntityProfile> GetUserDeviceIdById(int UserId)
        {
            DataResponse<EntityProfile> response = new DataResponse<EntityProfile>();
            try
            {
                base.DBInit();

                var query = DBEntity.UserProfiles.Where(a => a.UserId == UserId).FirstOrDefault();
                var entity = new EntityProfile
                {
                  Id=query.UserId.Value,
                  DeviceId=query.DeviceId

                };

                if (query != null)
                {

                    response.CreateResponse(entity, DataResponseStatus.OK);
                }
                else
                {
                    response.CreateResponse(DataResponseStatus.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                response.ThrowError(ex);
            }

            finally
            {
                base.DBClose();
            }
            return response;
        }
    }
}