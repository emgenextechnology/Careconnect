
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Mvc.Html;
using EBP.Business;

namespace EBP.Api.Handler._base
{
    public abstract class IHttpHandlerBase : IHttpHandler, IRequiresSessionState//, IRouteHandler
    {
        protected const string STR_GET = "GET";
        protected const string STR_DELETE = "DELETE";
        protected const string STR_POST = "POST";


        protected HttpContext ServiceContext { get; set; }

        public HttpResponse ServiceResponse
        {
            get
            {
                return this.ServiceContext.Response;
            }
        }

        protected string RequestUrl
        {
            get
            {
                return ServiceRequest.RawUrl.ToLower();
            }
        }

        protected HttpSessionState Session
        {
            get { return ServiceContext.Session; }
        }

        protected HttpRequest ServiceRequest
        {
            get
            {
                return this.ServiceContext.Request;
            }
        }

        private string CommandType
        {
            get { return ServiceRequest.Params.Get("CType"); }
        }

        public IPrincipal CurrentUser
        {
            get
            {
                ValidateRequest();
                return ServiceContext.User;
            }
        }

        public Guid CurrentUserKey
        {
            get
            {
                if (Session["currentMembershipKey"] == null)
                {
                    MembershipUser user = Membership.GetUser(ServiceContext.User.Identity.Name);
                    Session["currentMembershipKey"]=(Guid)user.ProviderUserKey;
                }
                return (Guid)Session["currentMembershipKey"];
            }
        }

        public string CurrentUserRole
        {
            get
            {
                var role = "";
                if (Session["CurrentUserRole"] == null)
                {

                    List<string> roles = Roles.GetRolesForUser().ToList();
                    if (roles.Contains("Administrator") || roles.Contains("AdminAgent"))
                    {
                        role = "admin";
                    }
                    else if (roles.Contains("Agent"))
                    {
                        role = "agent";
                    }
                    else if (roles.Contains("Owner"))
                    {
                        role = "owner";
                    }
                    else if (roles.Contains("Renter"))
                    {
                        role = "renter";
                    }

                    Session["CurrentUserRole"] = role;
                }
                role = Session["CurrentUserRole"].ToString();

                return role;
            }
        }


        /// <summary>
        /// service function name
        /// </summary>
        private string Command
        {
            get { return RequestParams.Get("Command"); }
        }


        public NameValueCollection RequestParams
        {
            get
            {
                return ServiceRequest.QueryString.Count > 0 ? ServiceRequest.QueryString : ServiceRequest.Form;
            }
        }

        public abstract void ProcessPost();
        public abstract void ProcessGet();
        public abstract void ProcessDelete();

        public virtual bool IsReusable
        {
            get { return false; }
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            this.ServiceContext = context;

            ServiceResponse.ContentType = "application/json";
            switch (context.Request.RequestType)
            {
                case STR_POST:
                    //ValidateRequest();
                    ProcessPost();
                    break;
                case STR_GET:
                    if (CommandType == STR_POST)
                        ProcessPost();
                    ProcessGet();
                    break;
                case STR_DELETE:
                    ProcessDelete();
                    break;
            }
            ServiceResponse.ContentType = "application/json";
            ServiceResponse.End();
        }

        void ValidateRequest()
        {
            if (!ServiceContext.User.Identity.IsAuthenticated)
            {
                //ServiceResponse.Write(new DataResponse { Status = DataResponseStatus.Unauthorized, Message = "Unauthorized" }.ToJson());
                ServiceResponse.Write(new DataResponse { Status = DataResponseStatus.Unauthorized, Message = "Unauthorized" });
                EndResponse();
            }
        }

        //protected void Error(string message)
        //{
        //    this.NrResponse.StatusCode = 500;
        //    this.NrResponse.Write("{error:'"+message+"'}");
        //    //throw new Exception(message);
        //    EndResponse();
        //}

        protected void EndResponse()
        {
            ServiceResponse.ContentType = "application/json";
            this.ServiceResponse.End();
        }

        protected void PostResponse(string data)
        {
            ServiceResponse.Write(data);
            EndResponse();
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        protected void ThrowError(string message, string content = "")
        {
            
        }
    }
}