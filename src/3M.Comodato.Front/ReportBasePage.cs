using _3M.Comodato.Utility;
using System;

namespace _3M.Comodato.Front
{
    public abstract class ReportBasePage : System.Web.UI.Page
    {
        protected string idKey => ControlesUtility.Criptografia.Descriptografar(Request.QueryString["IdKey"]);
        protected string[] parametros => idKey.Split('|');
        protected Entity.UsuarioPerfilEntity UsuarioAutenticado => Session["_CurrentUser"] == null ? null : Session["_CurrentUser"] as Entity.UsuarioPerfilEntity;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UsuarioAutenticado == null)
            {
                System.Web.Mvc.UrlHelper urlHelper = new System.Web.Mvc.UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
                System.Web.HttpContext.Current.Response.Redirect(urlHelper.Action("Login", "Usuario"));
            }

            if (!IsPostBack)
            {
                try
                {
                    ExibirRelatorio();
                }
                catch (Exception ex)
                {
                    LogUtility.LogarErro(ex);
                    throw ex;
                }
            }
        }

        protected abstract void ExibirRelatorio();
    }
}