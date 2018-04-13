using EBP.Business.Entity;
using EBP.Business.Entity.ParsingLog;
using EBP.Business.Enums;
using EBP.Business.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Repository
{
    public interface IRepositoryParsingLog
    {
        DataResponse<EntityList<EntityParsingLogSummary>> GetSummaryList(FilterParsingLog filter, int currentBusinessId);

        DataResponse<EntityList<EntityParsingLog>> GetAllLogsBySummaryId(int summaryId);
    }

    public class RepositoryParsingLog : _Repository, IRepositoryParsingLog
    {
        public DataResponse<EntityList<EntityParsingLog>> GetAllLogsBySummaryId(int summaryId)
        {
            throw new NotImplementedException();
        }

        public DataResponse<EntityList<EntityParsingLogSummary>> GetSummaryList(FilterParsingLog filter, int currentBusinessId)
        {
            var response = new DataResponse<EntityList<EntityParsingLogSummary>>();

            base.DBInit();

            var query = DBEntity.SalesImportSummeries.Where(a => a.ServiceId == filter.ServiceId);

            if (filter.ImportedDateFrom == null)
                filter.ImportedDateFrom = DateTime.UtcNow.AddDays(-67);

            query = query.Where(a => a.ImportedDate > filter.ImportedDateFrom);

            if (filter.ImportedDateTo != null)
            {
                query = query.Where(a => a.ImportedDate < filter.ImportedDateTo);
            }

            var entity = query.Select(a => new EntityParsingLogSummary
            {
                Id = a.Id,

                BusinessId = a.BusinessId,
                ImportedDate = a.ImportedDate,
                ImportMode = a.DataMode == (int)SalesDataMode.ServiceImport ? "Service Import" : "Web Upload",
                RowsSkipped = a.NumberOfSkips,
                RowsInserted = a.NumberOfInserts,
                RowsUpdated = a.NumberOfUpdates,
                RowsFailed = a.NumberOfFails,
                SourceFileUrl = a.ImportedFilePath,
                RowsAffected = a.RowsAffected,
                RowsImported = a.TotalRecords,
                RowsResolved = a.SalesImportDetails.Count(b => b.ImportStatus == 1 || b.ImportStatus == 2),
                NotReolved = a.SalesImportDetails.Count(b => b.ImportStatus == 5),
                ImportedFrom = a.DataMode == (int)SalesDataMode.ServiceImport ? "Service Import" : "Web Upload",
                FileType = a.SalesImportDetails.FirstOrDefault().DataType == (int)SalesDataType.MasterData ? "Collection File" : "Finance File"
            }).OrderByDescending(o => o.ImportedDate);

            response = GetList<EntityParsingLogSummary>(entity, filter.Skip, filter.Take);

            base.DBClose();

            return response;
        }

        public DataResponse<List<ResolvedSalesData>> ResolveSalesData(int type, int id, int businessId)
        {
            var response = new DataResponse<List<ResolvedSalesData>>();
            int responseType = 0;

            try
            {
                using (var dbEntity = new EBP.Business.Database.CareConnectCrmEntities())
                {
                    var queryRep = dbEntity.Reps.Where(a => a.RepGroup.BusinessId == businessId);
                    IEnumerable<SelectlistItem> querySelect = null;
                    ResolvedSalesData querySelectRep = null;
                    switch (type)
                    {
                        case (int)IncomingType.Practice:

                            var queryProvider = dbEntity.Providers.Where(a => a.PracticeProviderMappers.Any(b => b.Practice.BusinessId == businessId));

                            queryProvider = queryProvider.Where(a => a.PracticeProviderMappers.Any(b => b.PracticeId == id));
                            queryRep = queryRep.Where(a => a.Practices.Any(b => b.Id == id));

                            responseType = (int)IncomingType.Provider;
                            querySelect = queryProvider.Select(a => new SelectlistItem
                            {
                                Id = a.Id,
                                Value = a.FirstName + " " + a.MiddleName + " " + a.LastName
                            }).ToList();

                            querySelectRep = queryRep.Select(a => new ResolvedSalesData
                            {
                                DataType = (int)IncomingType.Rep,
                                RepName = a.User2.FirstName + " " + a.User2.MiddleName + " " + a.User2.LastName,
                                RepId = a.Id,
                                RepGroupId = a.RepGroupId,
                                RepGroupName = a.RepGroup.RepGroupName
                            }).FirstOrDefault();

                            break;

                        case (int)IncomingType.Provider:
                            var queryPractice = dbEntity.Practices.Where(a => a.BusinessId == businessId);
                            queryPractice = queryPractice.Where(a => a.PracticeProviderMappers.Any(b => b.ProviderId == id));
                            queryRep = queryRep.Where(a => a.Practices.Any(b => b.PracticeProviderMappers.Any(c => c.ProviderId == id)));

                            responseType = (int)IncomingType.Practice;
                            querySelect = queryPractice.Select(a =>
                                new SelectlistItem
                                {
                                    Id = a.Id,
                                    Value = a.PracticeName
                                }).ToList();

                            querySelectRep = queryRep.Select(a => new ResolvedSalesData
                            {
                                DataType = (int)IncomingType.Rep,
                                RepName = a.User2.FirstName + " " + a.User2.MiddleName + " " + a.User2.LastName,
                                RepId = a.Id,
                                RepGroupId = a.RepGroupId,
                                RepGroupName = a.RepGroup.RepGroupName
                            }).FirstOrDefault();

                            break;
                    }

                    var objResponse = new List<ResolvedSalesData>();
                    objResponse.Add(new ResolvedSalesData
                    {
                        DataType = responseType,
                        SelectList = querySelect
                    });
                    objResponse.Add(querySelectRep);

                    response.CreateResponse(DataResponseStatus.OK);

                    response.Model = objResponse;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }

            return response;
        }

        public DataResponse<EntityList<ImportMessage>> GetAllSalesMessages(int importDetailId)
        {
            var response = new DataResponse<EntityList<ImportMessage>>();
            try
            {
                DBInit();

                var query = DBEntity.SalesImportMessages.Where(a => a.ImportDetailId == importDetailId).Select(a => new ImportMessage
                {
                    Id = a.Id,
                    ColumnName = a.ColumnName,
                    Message = a.Message
                });

                response = GetList<ImportMessage>(query);
            }
            catch (Exception ex)
            {
                ex.Log();
            }

            return response;
        }

        public bool IsSpecimenExists(string specimenId)
        {
            try
            {
                DBInit();

                return DBEntity.ReportMasters.Any(a => a.SpecimenId == specimenId);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            finally
            {
                DBClose();
            }

            return false;
        }
    }
}
