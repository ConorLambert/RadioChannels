using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace RadioChannels
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "StationApi",
                routeTemplate: "api/{controller}/{name}"
            );

            config.Routes.MapHttpRoute(
                name: "GenreApi",
                routeTemplate: "api/{controller}/{some_var}/{index}",
                defaults: new { index = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}"
            );            
            
        }
    }
}
