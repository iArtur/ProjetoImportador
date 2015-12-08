using MundiPagg.Importador.WebApi.Formatters;
using MundiPagg.Importador.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MundiPagg.Importador.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Add(new FileMediaFormatter<UploadViewModel>());

            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
        }
    }
}
