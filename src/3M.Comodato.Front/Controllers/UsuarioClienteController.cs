using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.IO;
using System.Data;

namespace _3M.Comodato.Front.Controllers
{
    public class UsuarioClienteController : BaseController
    {
        // GET: UsuarioCliente
        [_3MAuthentication]
        public ActionResult Index()
        {
            Models.UsuarioClienteDetalhe usuarioClienteDetalhe = new Models.UsuarioClienteDetalhe
            {
                clientes = new List<Models.Cliente>(),
                usuarios = new List<Models.Usuario>()
            };

            return View(usuarioClienteDetalhe);
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.UsuarioClienteDetalhe usuarioClienteDetalhe = null;

            try
            {
                usuarioClienteDetalhe = new Models.UsuarioClienteDetalhe
                {
                    cliente = new ClienteEntity()
                    {
                        CD_CLIENTE = Convert.ToInt32(Utility.ControlesUtility.Criptografia.Descriptografar(idKey))
                    },
                    usuarios = new List<Models.Usuario>()
                };

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (usuarioClienteDetalhe == null)
                return HttpNotFound();
            else
                return View(usuarioClienteDetalhe);
        }

        public JsonResult ObterListaUsuarioClienteJson(int CD_CLIENTE, int nidUsuario)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaUsuarioCliente> listaUsuarioCliente = ObterListaUsuarioCliente(CD_CLIENTE, nidUsuario);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/UsuarioCliente/_gridMVC.cshtml", listaUsuarioCliente));
                jsonResult.Add("Status", "Success");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            //return Json(jsonResult, JsonRequestBehavior.AllowGet);
            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }

        protected List<Models.ListaUsuarioCliente> ObterListaUsuarioCliente(int CD_CLIENTE, int nidUsuario)
        {
            List<Models.ListaUsuarioCliente> listaUsuarioCliente = new List<Models.ListaUsuarioCliente>();

            try
            {
                UsuarioClienteEntity tecnicoClienteEntity = new UsuarioClienteEntity();
                tecnicoClienteEntity.cliente.CD_CLIENTE = CD_CLIENTE;
                tecnicoClienteEntity.usuario.nidUsuario = nidUsuario;
                DataTableReader dataTableReader = new UsuarioClienteData().ObterListaQtdeUsuarios(tecnicoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaUsuarioCliente usuarioCliente = new Models.ListaUsuarioCliente
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_CLIENTE"].ToString()),
                            CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]),
                            NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString(),
                            nvlQtdeUsuarios = Convert.ToInt64("0" + dataTableReader["nvlQtdeUsuarios"]),
                        };
                        listaUsuarioCliente.Add(usuarioCliente);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return listaUsuarioCliente;
        }

        public JsonResult ObterListaUsuarioClienteSelecionadoJson(int CD_CLIENTE)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaUsuarioClienteSelecinado> listaUsuarioClienteSelecionado = ObterListaUsuarioClienteSelecionado(CD_CLIENTE);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/UsuarioCliente/_gridMVCSelecionado.cshtml", listaUsuarioClienteSelecionado));
                jsonResult.Add("Status", "Success");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            //return Json(jsonResult, JsonRequestBehavior.AllowGet);
            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }

        protected List<Models.ListaUsuarioClienteSelecinado> ObterListaUsuarioClienteSelecionado(int CD_CLIENTE)
        {
            List<Models.ListaUsuarioClienteSelecinado> listaUsuarioClienteSelecionados = new List<Models.ListaUsuarioClienteSelecinado>();

            try
            {
                UsuarioClienteEntity usuarioClienteEntity = new UsuarioClienteEntity();
                usuarioClienteEntity.cliente.CD_CLIENTE = CD_CLIENTE;
                DataTableReader dataTableReader = new UsuarioClienteData().ObterLista(usuarioClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaUsuarioClienteSelecinado listaUsuarioClienteSelecionado = new Models.ListaUsuarioClienteSelecinado
                        {
                            nidUsuarioCliente = Convert.ToInt64("0" + dataTableReader["nidUsuarioCliente"]),
                            cliente = new ClienteEntity
                            {
                                CD_CLIENTE = Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"]),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + dataTableReader["CD_CLIENTE"].ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString(),
                            },
                            usuario = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["nidUsuario"]),
                                cnmNome = dataTableReader["cnmNome"].ToString()
                            },
                        };
                        listaUsuarioClienteSelecionados.Add(listaUsuarioClienteSelecionado);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return listaUsuarioClienteSelecionados;
        }

    }
}