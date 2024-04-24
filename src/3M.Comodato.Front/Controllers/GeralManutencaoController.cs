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
    public class GeralManutencaoController : BaseController
    {
        // GET: GeralManutencao
        [_3MAuthentication]
        public ActionResult Index()
        {
            Models.GeralManutencao geralManutencao = new Models.GeralManutencao
            {
                DT_INICIO = DateTime.Now.AddDays(-90).ToString("dd/MM/yyyy"),
                DT_FIM = DateTime.Now.ToString("dd/MM/yyyy"),
                filtroAtual = "Cliente",

                clientes = new List<Models.Cliente>(),
                AllClientes = ObterListaClientePorUsuarioPerfil(Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).usuario.nidUsuario), true),

                grupos = new List<Models.Grupo>(),
                AllGrupos = ObterListaGrupo(false),

                modelos = new List<Models.Modelo>(),
                AllModelos = ObterListaModelo(false),

                tecnicos = new List<Models.Tecnico>(),
                AllTecnicos = ObterListaTecnico(true),

                pecas = new List<Models.Peca>(),
                AllPecas = ObterListaPeca(true),

                ativos = new List<Models.Ativo>(),
                AllAtivos = ObterListaAtivoFixo(false),
            };

            return View(geralManutencao);
        }

        //[HttpPost]
        //[_3MAuthentication]
        //public ActionResult Index(Models.GeralManutencao geralManutencao)
        //{
        //    geralManutencao.clientes = new List<Models.Cliente>();
        //    geralManutencao.AllClientes = ObterListaClientePorUsuarioPerfil(Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).usuario.nidUsuario), true);

        //    geralManutencao.grupos = new List<Models.Grupo>();
        //    geralManutencao.AllGrupos = ObterListaGrupo(false);

        //    geralManutencao.modelos = new List<Models.Modelo>();
        //    geralManutencao.AllModelos = ObterListaModelo(false);

        //    geralManutencao.tecnicos = new List<Models.Tecnico>();
        //    geralManutencao.AllTecnicos = ObterListaTecnico(true);

        //    geralManutencao.pecas = new List<Models.Peca>();
        //    geralManutencao.AllPecas = ObterListaPeca(true);

        //    geralManutencao.ativos = new List<Models.Ativo>();
        //    geralManutencao.AllAtivos = ObterListaAtivoFixo();

        //    string idKey = string.Empty;

        //    switch (geralManutencao.filtroAtual)
        //    {
        //        case "Cliente":
        //            if (geralManutencao.ClientesSelecionados != null)
        //            {
        //                foreach (var item in geralManutencao.ClientesSelecionados)
        //                {
        //                    idKey += item.ToString() + ",";
        //                }
        //            }
        //            break;
        //        case "Grupo":
        //            if (geralManutencao.GruposSelecionados != null)
        //            {
        //                foreach (var item in geralManutencao.GruposSelecionados)
        //                {
        //                    idKey += item.ToString() + ",";
        //                }
        //            }
        //            break;
        //        case "Modelo":
        //            if (geralManutencao.ModelosSelecionados != null)
        //            {
        //                foreach (var item in geralManutencao.ModelosSelecionados)
        //                {
        //                    idKey += item.ToString() + ",";
        //                }
        //            }
        //            break;
        //        case "Técnico":
        //            if (geralManutencao.TecnicosSelecionados != null)
        //            {
        //                foreach (var item in geralManutencao.TecnicosSelecionados)
        //                {
        //                    idKey += item.ToString() + ",";
        //                }
        //            }
        //            break;
        //        case "Peça":
        //            if (geralManutencao.PecasSelecionados != null)
        //            {
        //                foreach (var item in geralManutencao.PecasSelecionados)
        //                {
        //                    idKey += item.ToString() + ",";
        //                }
        //            }
        //            break;
        //        case "Equipamento":
        //            if (geralManutencao.AtivosSelecionados != null)
        //            {
        //                foreach (var item in geralManutencao.AtivosSelecionados)
        //                {
        //                    idKey += item.ToString() + ",";
        //                }
        //            }
        //            break;
        //    }

        //    if (!string.IsNullOrEmpty(idKey))
        //        idKey = idKey.Substring(0, idKey.Length - 1);

        //    idKey += "|" + geralManutencao.DT_INICIO + "|" + geralManutencao.DT_FIM + "|" + geralManutencao.filtroAtual;
        //    Session["ReportGeralManutencao"] = idKey;

        //    //return RedirectPermanent($"~/RelatorioGeralManutencao.aspx?IdKey={idKey}");
        //    return RedirectPermanent($"~/RelatorioGeralManutencao.aspx");

        //    //return View(geralManutencao);
        //}

        //public JsonResult CriptografarChaveJson(string Conteudo)
        //{
        //    Dictionary<string, object> jsonResult = new Dictionary<string, object>();

        //    try
        //    {
        //        string idKey = ControlesUtility.Criptografia.Criptografar(Conteudo);
        //        jsonResult.Add("idKey", idKey);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogarErro(ex);
        //        throw ex;
        //    }

        //    //return Json(jsonResult, JsonRequestBehavior.AllowGet);
        //    var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
        //    jsonList.MaxJsonLength = int.MaxValue;
        //    return jsonList;

        //}

        protected List<Models.Cliente> ObterListaClientePorUsuarioPerfil(Int64 nidUsuario, bool? SomenteAtivos = false)
        {
            List<Models.Cliente> clientes = new List<Models.Cliente>();

            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();
                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity, nidUsuario).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        if (SomenteAtivos == true)
                        {
                            if (dataTableReader["DT_DESATIVACAO"] == DBNull.Value)
                            {
                                Models.Cliente cliente = new Models.Cliente
                                {
                                    CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                                    NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString()
                                };
                                clientes.Add(cliente);
                            }
                        }
                        else
                        {
                            Models.Cliente cliente = new Models.Cliente
                            {
                                CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString()
                            };
                            clientes.Add(cliente);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return clientes;
        }

        protected List<Models.Tecnico> ObterListaTecnico(bool? SomenteAtivos = false)
        {
            List<Models.Tecnico> tecnicos = new List<Models.Tecnico>();

            try
            {
                TecnicoEntity tecnicoEntity = new TecnicoEntity();
                if (SomenteAtivos == true)
                    tecnicoEntity.FL_ATIVO = "S";

                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Tecnico tecnico = new Models.Tecnico
                        {
                            CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                            NM_TECNICO = dataTableReader["NM_TECNICO"].ToString()
                        };
                        tecnicos.Add(tecnico);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return tecnicos;
        }

        protected List<Models.Peca> ObterListaPeca(bool? SomenteAtivos = false)
        {
            List<Models.Peca> pecas = new List<Models.Peca>();

            try
            {
                PecaEntity pecaEntity = new PecaEntity();
                if (SomenteAtivos == true)
                    pecaEntity.FL_ATIVO_PECA = "S";

                DataTableReader dataTableReader = new PecaData().ObterListaNew(pecaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Peca peca = new Models.Peca
                        {
                            CD_PECA = dataTableReader["CD_PECA"].ToString(),
                            DS_PECA = dataTableReader["DS_PECA"].ToString()
                        };
                        pecas.Add(peca);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return pecas;
        }

    }
}