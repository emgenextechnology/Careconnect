using EBP.Business.Database;
using EBP.Business.Entity;
using EBP.Business.Entity.Practice;
using EBP.Business.Enums;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace EBP.Business.Repository
{
    public class RepositoryAddress : _Repository
    {
        public DataResponse<EntityList<EntityPracticeAddress>> GetAddress(FilterAddress filter, int currentBusineId, int take = 10, int skip = 0)
        {
            var response = new DataResponse<EntityList<EntityPracticeAddress>>();
            try
            {
                if (filter != null)
                {
                    take = filter.Take;
                    skip = filter.Skip;
                }

                base.DBInit();

                var query = DBEntity.PracticeAddressMappers.Where(a => a.Practice.BusinessId == currentBusineId);

                if (!string.IsNullOrEmpty(filter.KeyWords))
                {
                    query = query.Where(ua =>
                        (ua.Address.LocationId != null && ua.Address.LocationId.ToLower().Contains(filter.KeyWords.ToLower())) ||
                        ua.Practice.PracticeName.ToLower().Contains(filter.KeyWords.ToLower()) ||
                        ua.Address.Line1.ToLower().Contains(filter.KeyWords.ToLower()) ||
                        //ua.AddressType.ToLower().Contains(filter.KeyWords.ToLower()) ||
                        ua.Address.City.ToLower().Contains(filter.KeyWords.ToLower()) ||
                        ua.Address.LookupState.StateName.ToLower().Contains(filter.KeyWords.ToLower()) ||
                        ua.Address.Zip.ToLower().Contains(filter.KeyWords.ToLower())
                        );
                }

                var selectQuery = query.Select(s => new EntityPracticeAddress
                {
                    Id = s.Address.Id,
                    PracticeName = s.Practice.PracticeName,
                    TypeId = s.Address.AddressTypeId,
                    LocationId = s.Address.LocationId,
                    Line1 = s.Address.Line1,
                    Line2 = s.Address.Line2,
                    City = s.Address.City,
                    Zip = s.Address.Zip,
                    AddressTypeId = s.Address.AddressTypeId,
                    StateId = s.Address.StateId,
                    State = s.Address.LookupState.StateName,
                    Phone = s.Address.Phones.Select(p => new EntityPracticePhone
                    {
                        PhoneId = p.Id,
                        PhoneNumber = p.PhoneNumber,
                        Extension = p.Extension
                    }),
                    //AddressType = EnumHelper.GetEnumName<AddressType>(s.Address.AddressTypeId)
                });

                //query.ForEach(a => a.AddressType = EnumHelper.GetEnumName<AddressType>(a.AddressTypeId));

                if (string.IsNullOrEmpty(filter.SortKey) || string.IsNullOrEmpty(filter.SortOrder))
                {
                    selectQuery = selectQuery.OrderByDescending(o => o.Id);
                }
                else
                {
                    string orderBy = string.Format("{0} {1}", filter.SortKey, filter.SortOrder);
                    selectQuery = selectQuery.OrderBy(orderBy);
                }

                response = GetList<EntityPracticeAddress>(selectQuery, skip, take);
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

        public DataResponse<EntityPracticeAddress> GetAddressById(int AddressId)
        {
            var response = new DataResponse<EntityPracticeAddress>();
            try
            {
                base.DBInit();

                var query = DBEntity.Addresses.Where(a => a.Id == AddressId).Select(a => new EntityPracticeAddress
                {
                    Id = a.Id,
                    PracticeName = a.PracticeAddressMappers.FirstOrDefault().Practice.PracticeName,
                    AddressTypeId = a.AddressTypeId,
                    Line1 = a.Line1,
                    Line2 = a.Line2,
                    LocationId = a.LocationId,
                    City = a.City,
                    State = a.LookupState.StateName,
                    Zip = a.Zip
                });

                response = GetFirst<EntityPracticeAddress>(query);

                response.Model.AddressType = EnumHelper.GetEnumName<AddressType>(response.Model.AddressTypeId);
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

        public DataResponse<EntityPracticeAddress> Update(EntityPracticeAddress entity)
        {
            var response = new DataResponse<EntityPracticeAddress>();
            try
            {
                base.DBInit();
                if (DBEntity.Addresses.FirstOrDefault(a => a.Id != entity.Id && a.LocationId == entity.LocationId) != null)
                {
                    response.Id = entity.Id;
                    response.Status = DataResponseStatus.Found;
                    response.Message = "LocationId already exists";
                }
                else
                {
                    var model = DBEntity.Addresses.FirstOrDefault(a => a.Id == entity.Id);
                    model.LocationId = entity.LocationId;

                    if (base.DBSaveUpdate(model) > 0)
                    {
                        entity.Id = model.Id;
                        response.CreateResponse(entity, DataResponseStatus.OK);
                    }
                    else
                    {
                        response.CreateResponse(DataResponseStatus.InternalServerError);
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
