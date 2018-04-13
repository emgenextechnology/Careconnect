using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business
{
    public class DataResponse
    {
        /// <summary>
        /// Database Id
        /// </summary>
        public int? Id
        {
            get;
            set;
        }

        ///// <summary>
        ///// Is Reponse Valid
        ///// </summary>
        //public bool IsValid           //not needed in primary assumption. since we have status..
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Status equivalent to HttpStatusCode
        /// </summary>
        public DataResponseStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// Basic Message
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Optional Content to return
        /// </summary>
        public string Content
        {
            get;
            set;
        }

        public bool IsOk()
        {
            return Status == DataResponseStatus.OK;
        }

        public bool IsSaveUpdate()
        {
            return Status == DataResponseStatus.Created || Status == DataResponseStatus.OK;
        }
    }

    public class DataResponse<T> : DataResponse
    {
        private T _model;
        public T Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
            }
        }
    }



    public enum DataResponseStatus
    {
        // Summary:
        //     Equivalent to HTTP status 100. System.Net.HttpStatusCode.Continue indicates
        //     that the client can continue with its request.
        Continue = 100,
        //
        // Summary:
        //     Equivalent to HTTP status 101. System.Net.HttpStatusCode.SwitchingProtocols
        //     indicates that the protocol version or protocol is being changed.
        SwitchingProtocols = 101,
        //
        // Summary:
        //     Equivalent to HTTP status 200. System.Net.HttpStatusCode.OK indicates that
        //     the request succeeded and that the requested information is in the response.
        //     This is the most common status code to receive.
        OK = 200,
        //
        // Summary:
        //     Equivalent to HTTP status 201. System.Net.HttpStatusCode.Created indicates
        //     that the request resulted in a new resource created before the response was
        //     sent.
        Created = 201,
        //
        // Summary:
        //     Equivalent to HTTP status 202. System.Net.HttpStatusCode.Accepted indicates
        //     that the request has been accepted for further processing.
        Accepted = 202,
        //
        // Summary:
        //     Equivalent to HTTP status 203. System.Net.HttpStatusCode.NonAuthoritativeInformation
        //     indicates that the returned metainformation is from a cached copy instead
        //     of the origin server and therefore may be incorrect.
        NonAuthoritativeInformation = 203,
        //
        // Summary:
        //     Equivalent to HTTP status 204. System.Net.HttpStatusCode.NoContent indicates
        //     that the request has been successfully processed and that the response is
        //     intentionally blank.
        NoContent = 204,
        //
        // Summary:
        //     Equivalent to HTTP status 205. System.Net.HttpStatusCode.ResetContent indicates
        //     that the client should reset (not reload) the current resource.
        ResetContent = 205,
        //
        // Summary:
        //     Equivalent to HTTP status 206. System.Net.HttpStatusCode.PartialContent indicates
        //     that the response is a partial response as requested by a GET request that
        //     includes a byte range.
        PartialContent = 206,
        //
        // Summary:
        //     Equivalent to HTTP status 300. System.Net.HttpStatusCode.MultipleChoices
        //     indicates that the requested information has multiple representations. The
        //     default action is to treat this status as a redirect and follow the contents
        //     of the Location header associated with this response.
        MultipleChoices = 300,
        //
        // Summary:
        //     Equivalent to HTTP status 300. System.Net.HttpStatusCode.Ambiguous indicates
        //     that the requested information has multiple representations. The default
        //     action is to treat this status as a redirect and follow the contents of the
        //     Location header associated with this response.
        Ambiguous = 300,
        //
        // Summary:
        //     Equivalent to HTTP status 301. System.Net.HttpStatusCode.MovedPermanently
        //     indicates that the requested information has been moved to the URI specified
        //     in the Location header. The default action when this status is received is
        //     to follow the Location header associated with the response.
        MovedPermanently = 301,
        //
        // Summary:
        //     Equivalent to HTTP status 301. System.Net.HttpStatusCode.Moved indicates
        //     that the requested information has been moved to the URI specified in the
        //     Location header. The default action when this status is received is to follow
        //     the Location header associated with the response. When the original request
        //     method was POST, the redirected request will use the GET method.
        Moved = 301,
        //
        // Summary:
        //     Equivalent to HTTP status 302. System.Net.HttpStatusCode.Found indicates
        //     that the requested information is located at the URI specified in the Location
        //     header. The default action when this status is received is to follow the
        //     Location header associated with the response. When the original request method
        //     was POST, the redirected request will use the GET method.
        Found = 302,
        //
        // Summary:
        //     Equivalent to HTTP status 302. System.Net.HttpStatusCode.Redirect indicates
        //     that the requested information is located at the URI specified in the Location
        //     header. The default action when this status is received is to follow the
        //     Location header associated with the response. When the original request method
        //     was POST, the redirected request will use the GET method.
        Redirect = 302,
        //
        // Summary:
        //     Equivalent to HTTP status 303. System.Net.HttpStatusCode.SeeOther automatically
        //     redirects the client to the URI specified in the Location header as the result
        //     of a POST. The request to the resource specified by the Location header will
        //     be made with a GET.
        SeeOther = 303,
        //
        // Summary:
        //     Equivalent to HTTP status 303. System.Net.HttpStatusCode.RedirectMethod automatically
        //     redirects the client to the URI specified in the Location header as the result
        //     of a POST. The request to the resource specified by the Location header will
        //     be made with a GET.
        RedirectMethod = 303,
        //
        // Summary:
        //     Equivalent to HTTP status 304. System.Net.HttpStatusCode.NotModified indicates
        //     that the client's cached copy is up to date. The contents of the resource
        //     are not transferred.
        NotModified = 304,
        //
        // Summary:
        //     Equivalent to HTTP status 305. System.Net.HttpStatusCode.UseProxy indicates
        //     that the request should use the proxy server at the URI specified in the
        //     Location header.
        UseProxy = 305,
        //
        // Summary:
        //     Equivalent to HTTP status 306. System.Net.HttpStatusCode.Unused is a proposed
        //     extension to the HTTP/1.1 specification that is not fully specified.
        Unused = 306,
        //
        // Summary:
        //     Equivalent to HTTP status 307. System.Net.HttpStatusCode.RedirectKeepVerb
        //     indicates that the request information is located at the URI specified in
        //     the Location header. The default action when this status is received is to
        //     follow the Location header associated with the response. When the original
        //     request method was POST, the redirected request will also use the POST method.
        RedirectKeepVerb = 307,
        //
        // Summary:
        //     Equivalent to HTTP status 307. System.Net.HttpStatusCode.TemporaryRedirect
        //     indicates that the request information is located at the URI specified in
        //     the Location header. The default action when this status is received is to
        //     follow the Location header associated with the response. When the original
        //     request method was POST, the redirected request will also use the POST method.
        TemporaryRedirect = 307,
        //
        // Summary:
        //     Equivalent to HTTP status 400. System.Net.HttpStatusCode.BadRequest indicates
        //     that the request could not be understood by the server. System.Net.HttpStatusCode.BadRequest
        //     is sent when no other error is applicable, or if the exact error is unknown
        //     or does not have its own error code.
        BadRequest = 400,
        //
        // Summary:
        //     Equivalent to HTTP status 401. System.Net.HttpStatusCode.Unauthorized indicates
        //     that the requested resource requires authentication. The WWW-Authenticate
        //     header contains the details of how to perform the authentication.
        Unauthorized = 401,
        //
        // Summary:
        //     Equivalent to HTTP status 402. System.Net.HttpStatusCode.PaymentRequired
        //     is reserved for future use.
        PaymentRequired = 402,
        //
        // Summary:
        //     Equivalent to HTTP status 403. System.Net.HttpStatusCode.Forbidden indicates
        //     that the server refuses to fulfill the request.
        Forbidden = 403,
        //
        // Summary:
        //     Equivalent to HTTP status 404. System.Net.HttpStatusCode.NotFound indicates
        //     that the requested resource does not exist on the server.
        NotFound = 404,
        //
        // Summary:
        //     Equivalent to HTTP status 405. System.Net.HttpStatusCode.MethodNotAllowed
        //     indicates that the request method (POST or GET) is not allowed on the requested
        //     resource.
        MethodNotAllowed = 405,
        //
        // Summary:
        //     Equivalent to HTTP status 500. System.Net.HttpStatusCode.InternalServerError
        //     indicates that a generic error has occurred on the server.
        InternalServerError = 500,
    }
}
