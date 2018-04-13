using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace EmgenexBusinessPortal
{
    public class RouteConstraints : IRouteConstraint
    {
         private readonly string _host;

         public RouteConstraints(string pattern)
        {
            _host = pattern;
        }

        public bool Match(
            HttpContextBase httpContext,
            Route route,
            string parameterName,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {

            if (values["area"] == null && values["businessname"] != null)
            {

                values.Remove("controller");
                values.Remove("action");

                values.Add("controller", "Home");
                values.Add("action", "business");
            }

            return true;
        }
    }
}