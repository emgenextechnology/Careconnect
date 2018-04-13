using EmgenexBusinessPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmgenexBusinessPortal.Extension
{
    public static class HttpContext
    {

        public static int GetCurrentBusinessId(this HttpContextBase context)
        {
            return GetCurrentUser(context).BusinessId.Value;
        }

      


        public static UserDetailsModel GetCurrentUser(this HttpContextBase context)
        {
            if (context == null)
                throw new Exception("Context is null");

            return context.Items["CurrentUser"] as UserDetailsModel;

        }




        //public UserDetailsModel CurrentUser
        //{
        //    get
        //    {
        //        var model = HttpContext.Items["CurrentUser"] as UserDetailsModel;
        //        return model;
        //    }
        //}


    }
    
}