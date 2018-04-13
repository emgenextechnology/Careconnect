using System;
using EBP.Business;
using System.Web;

public static class DataResponseExt
{
    public static void CreateResponse(this DataResponse respose, DataResponseStatus status, int Id = 0)
    {
        respose.Status = status;
        respose.Message = status.ToString();
        respose.Id = Id;
    }

    public static void CreateResponse<T>(this DataResponse<T> respose, T model, DataResponseStatus status)
    {
        respose.Status = status;
        respose.Message = status.ToString();
        respose.Model = model;
    }


    //create error response
    public static void ThrowError(this DataResponse response, Exception ex)
    {
        ex.Log();//log in db
        CreateResponse(response, DataResponseStatus.InternalServerError); //create basic
        var innerException = ex.InnerException == null ? "" : ex.InnerException.ToString();
        response.Content = ex.Message + " - " + innerException; // add exception message
    }

    public static void CacheIt<T>(this DataResponse<T> obj, string name)
    {
        if (obj.Status == DataResponseStatus.OK)
            HttpRuntime.Cache[name] = obj.Model;
    }

    public static void ClearCache(this object obj, string name)
    {
        HttpRuntime.Cache.Remove(name);
    }

    public static DataResponse<T> GetCached<T>(this DataResponse<T> obj, string name)
    {
        var dataResponse = new DataResponse<T>();
        var cachedModel = (T)HttpRuntime.Cache[name];
        if (cachedModel == null)
            dataResponse.CreateResponse<T>(default(T), DataResponseStatus.NotFound);
        else
            dataResponse.CreateResponse<T>(cachedModel, DataResponseStatus.OK);

        return dataResponse;
    }

}