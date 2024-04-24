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
    public class TecnicoXClienteController : BaseController
    {
        // GET: TecnicoXCliente
        [_3MAuthentication]
        public ActionResult Index()
        {
            Models.TecnicoXClienteDetalhe tecnicoXClienteDetalhe = new Models.TecnicoXClienteDetalhe
            {
                clientes = new List<Models.Cliente>(),
                tecnicos = new List<Models.Tecnico>()
            };

            return View(tecnicoXClienteDetalhe);
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.TecnicoXClienteDetalhe tecnicoXClienteDetalhe = null;

            try
            {
                tecnicoXClienteDetalhe = new Models.TecnicoXClienteDetalhe
                {
                    cliente = new ClienteEntity()
                    {
                        CD_CLIENTE = Convert.ToInt32(Utility.ControlesUtility.Criptografia.Descriptografar(idKey))
                    },
                    tecnicos = new List<Models.Tecnico>()//,
                    //listaTecnicoXClienteEscalas = new List<Models.ListaTecnicoXClienteEscala>()
                };

                //ViewBag.URLAPI = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLAPI);
                //ViewBag.nidUsuario = CurrentUser.usuario.nidUsuario;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (tecnicoXClienteDetalhe == null)
                return HttpNotFound();
            else
                return View(tecnicoXClienteDetalhe);
        }

        public JsonResult ObterListaTecnicoXClienteJson(int CD_CLIENTE, string CD_TECNICO, int? nvlQtdeTecnicos)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaTecnicoXCliente> listaTecnicoXCliente = ObterListaTecnicoXCliente(CD_CLIENTE, CD_TECNICO, nvlQtdeTecnicos);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/TecnicoXCliente/_gridMVC.cshtml", listaTecnicoXCliente));
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

        protected List<Models.ListaTecnicoXCliente> ObterListaTecnicoXCliente(int CD_CLIENTE, string CD_TECNICO, int? nvlQtdeTecnicos)
        {
            List<Models.ListaTecnicoXCliente> listaTecnicoXCliente = new List<Models.ListaTecnicoXCliente>();

            try
            {
                TecnicoClienteEntity tecnicoClienteEntity = new TecnicoClienteEntity();
                tecnicoClienteEntity.cliente.CD_CLIENTE = CD_CLIENTE;
                tecnicoClienteEntity.tecnico.CD_TECNICO = CD_TECNICO;
                DataTableReader dataTableReader = new TecnicoClienteData().ObterListaQtdeTecnicos(tecnicoClienteEntity, nvlQtdeTecnicos, CurrentUser.usuario.nidUsuario).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaTecnicoXCliente tecnicoXCliente = new Models.ListaTecnicoXCliente
                        {
                            CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_CLIENTE"].ToString()),
                            NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString(),
                            nvlQtdeTecnicos = Convert.ToInt64("0" + dataTableReader["nvlQtdeTecnicos"]),
                            nvlCargaTecnica = Convert.ToInt64("0" + dataTableReader["nvlCargaTecnica"]),
                            QT_PERIODO = Convert.ToInt64(dataTableReader["QT_PERIODO"]),
                            NM_TECNICO_PRINCIPAL = dataTableReader["NM_TECNICO_PRINCIPAL"].ToString()
                        };
                        listaTecnicoXCliente.Add(tecnicoXCliente);
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

            return listaTecnicoXCliente;
        }

        public JsonResult ObterListaTecnicoXClienteEscalaJson(int CD_CLIENTE)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaTecnicoXClienteEscala> listaTecnicoXClienteEscala = ObterListaTecnicoXClienteEscala(CD_CLIENTE);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/TecnicoXCliente/_gridMVCEscala.cshtml", listaTecnicoXClienteEscala));
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

        protected List<Models.ListaTecnicoXClienteEscala> ObterListaTecnicoXClienteEscala(int CD_CLIENTE)
        {
            List<Models.ListaTecnicoXClienteEscala> listaTecnicosXClientesEscalas = new List<Models.ListaTecnicoXClienteEscala>();

            try
            {
                TecnicoClienteEntity tecnicoClienteEntity = new TecnicoClienteEntity();
                tecnicoClienteEntity.cliente.CD_CLIENTE = CD_CLIENTE;
                DataTableReader dataTableReader = new TecnicoClienteData().ObterListaEscala(tecnicoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaTecnicoXClienteEscala listaTecnicoXClienteEscala = new Models.ListaTecnicoXClienteEscala
                        {
                            CD_ORDEM = Convert.ToInt32(dataTableReader["CD_ORDEM"]),
                            empresa = new EmpresaEntity
                            {
                                CD_Empresa = Convert.ToInt64("0" + dataTableReader["CD_Empresa"]),
                                NM_Empresa = dataTableReader["NM_Empresa"].ToString(),
                            },
                            tecnico = new TecnicoEntity
                            {
                                CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                                NM_TECNICO = dataTableReader["NM_TECNICO"].ToString()
                            },
                            nvlCargaTecnica = Convert.ToInt64(dataTableReader["nvlCargaTecnica"])
                        };
                        listaTecnicosXClientesEscalas.Add(listaTecnicoXClienteEscala);
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

            return listaTecnicosXClientesEscalas;
        }

    }
}