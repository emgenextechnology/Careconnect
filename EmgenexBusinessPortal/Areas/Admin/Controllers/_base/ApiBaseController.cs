using EmgenexBusinessPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace EmgenexBusinessPortal.Areas.Admin.Controllers._base
{
    public class ApiBaseController : ApiController
    {
        protected int CurrentUserId { get { return CurrentUser.Id; } }

        protected int? CurrentBusinessId { get { return CurrentUser.BusinessId; } }

        protected UserDetailsModel CurrentUser
        {
            get
            {

                var model = HttpContext.Current.Items["CurrentUser"] as UserDetailsModel;
                return model;
            }
        }


    }
}
