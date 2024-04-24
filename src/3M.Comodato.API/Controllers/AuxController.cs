using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
//--------------------------

namespace _3M.Comodato.API.Controllers
{
    [Authorize]
    public static class AuxController
    {
        public static string RenderViewToString(string controllerName, string viewName, object viewData)
        {
            var context = HttpContext.Current;
            var contextBase = new HttpContextWrapper(context);
            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);

            var controllerContext = new ControllerContext(contextBase,
                                                          routeData,
                                                          new EmptyController());

            var razorViewEngine = new RazorViewEngine();
            var razorViewResult = razorViewEngine.FindView(controllerContext,
                                                           viewName,
                                                           "",
                                                           false);

            var writer = new StringWriter();
            var viewContext = new ViewContext(controllerContext,
                                              razorViewResult.View,
                                              new ViewDataDictionary(viewData),
                                              new TempDataDictionary(),
                                              writer
                                              );
     
            razorViewResult.View.Render(viewContext, writer);

            return writer.ToString();
        }

        
    }

    class EmptyController : ControllerBase
    {
        protected override void ExecuteCore() { }
    }
}