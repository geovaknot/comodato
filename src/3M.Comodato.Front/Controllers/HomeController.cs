using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Data;

namespace _3M.Comodato.Front.Controllers
{
    public class HomeController : BaseController
    {

        public ActionResult Index()
        {
            try
            {
                if (CurrentUser == null)
                    //if (Session["CurrentUser"] == null)
                    return RedirectToAction("Login", "Usuario");
                else if (CurrentUser.usuario.nidUsuario == 0)
                    return RedirectToAction("Login", "Usuario");
                //Session["UserName"] = ((UsuarioEntity)Session["CurrentUser"]).cnmNome;
                //ViewBag.cnmNome = CurrentUser.cnmNome;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View();
        }

        public ActionResult AcessoNegado()
        {
            try
            {
                if (CurrentUser != null)
                {
                    UsuarioEntity usuarioEntity = new UsuarioEntity();
                    usuarioEntity.nidUsuario = CurrentUser.usuario.nidUsuario;

                    DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            ViewBag.cdsPerfil = dataTableReader["cdsPerfil"].ToString();
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View();
        }
    }
}