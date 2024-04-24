using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3M.Comodato.Utility;
using _3M.Comodato.Entity;
using System.Data;
using _3M.Comodato.Data;

namespace _3M.Comodato.Front
{
    public class _3MAuthentication : AuthorizeAttribute
    {
        //readonly UsuarioEntity usuarioEntity;

        public _3MAuthentication()
        {
            //usuarioEntity = DependencyResolver.Current.GetService<UsuarioEntity>();
            //usuarioEntity = (UsuarioEntity)HttpContext.Current.Session["_CurrentUser"];
        }

        //public string nomeController { get; set; }
        protected bool SessionEmpty { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string currentController = string.Empty;
            UsuarioPerfilEntity usuarioPerfilEntity = (UsuarioPerfilEntity)HttpContext.Current.Session["_CurrentUser"];
            //string currentAction = string.Empty;
            var routeData = httpContext.Request.RequestContext.RouteData;

            currentController = routeData.GetRequiredString("controller");
            //currentAction = routeData.GetRequiredString("action");

            if (usuarioPerfilEntity != null)
            {
                if (usuarioPerfilEntity.usuario.nidUsuario == 0)
                {
                    SessionEmpty = true;
                    return false;
                }

                bool acesso = ControlesUtility.Seguranca.VerificarAcesso(usuarioPerfilEntity.usuario, currentController);
                return acesso;
            }
            else if (usuarioPerfilEntity == null)
            {
                SessionEmpty = true;
            }

            return false;
            //return base.AuthorizeCore(httpContext);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //nomeController = filterContext.Controller.ToString();
            base.OnAuthorization(filterContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);

            if (SessionEmpty == true)
                filterContext.Result = new RedirectResult(urlHelper.Action("Login", "Usuario"));
            else
                filterContext.Result = new RedirectResult(urlHelper.Action("AcessoNegado", "Home"));
            //base.HandleUnauthorizedRequest(filterContext);
        }
    }
}