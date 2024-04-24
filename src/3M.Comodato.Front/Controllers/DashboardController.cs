using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;

namespace _3M.Comodato.Front.Controllers
{
    public class DashboardController : BaseController
    {
        [_3MAuthentication]
        public ActionResult Planejamento()
        {
            Models.Dashboard dashboard = new Models.Dashboard
            {
                tiposDashboard = ControlesUtility.Dicionarios.TipoDashboard(),
                tipoDashboard = "P",
                grupos = new List<Models.Grupo>(),
                regioes = new List<Models.Regiao>(),
                modelos = new List<Models.Modelo>(),
                vendedores = new List<Models.Vendedor>(),
                linhasProdutos = new List<Models.LinhaProduto>(),
            };
            return View(dashboard);
        }

        [_3MAuthentication]
        public ActionResult Vendas()
        {
            Models.Dashboard dashboard = new Models.Dashboard
            {
                tiposDashboard = ControlesUtility.Dicionarios.TipoDashboard(),
                tipoDashboard = "V",
                grupos = new List<Models.Grupo>(),
                regioes = new List<Models.Regiao>(),
                modelos = new List<Models.Modelo>(),
                vendedores = new List<Models.Vendedor>(),
                usuariosRegionais = new List<Models.Usuario>(),
                linhasProdutos = new List<Models.LinhaProduto>(),
            };
            return View(dashboard);
        }

        [_3MAuthentication]
        public ActionResult Negocios()
        {
            Models.Dashboard dashboard = new Models.Dashboard
            {
                tiposDashboard = ControlesUtility.Dicionarios.TipoDashboard(),
                tipoDashboard = "N",
                grupos = new List<Models.Grupo>(),
                regioes = new List<Models.Regiao>(),
                modelos = new List<Models.Modelo>(),
                vendedores = new List<Models.Vendedor>(),
                linhasProdutos = new List<Models.LinhaProduto>(),
            };
            return View(dashboard);
        }

        [_3MAuthentication]
        public ActionResult AreaTecnica()
        {
            Models.Dashboard dashboard = new Models.Dashboard
            {
                tiposDashboard = ControlesUtility.Dicionarios.TipoDashboard(),
                tipoDashboard = "T",
                grupos = new List<Models.Grupo>(),
                regioes = new List<Models.Regiao>(),
                modelos = new List<Models.Modelo>(),
                vendedores = new List<Models.Vendedor>(),
                linhasProdutos = new List<Models.LinhaProduto>(),
            };
            return View(dashboard);
        }

        #region Compartilhado
        public JsonResult ObterListaClienteJson(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaCliente> listaCliente = ObterListaCliente(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Dashboard/_gridMVCCliente.cshtml", listaCliente));
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

        public JsonResult ObterListaClienteVisaoResumidaJson(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaCliente> listaCliente = ObterListaCliente(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Dashboard/_gridMVCClienteVisaoResumida.cshtml", listaCliente));
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

        public JsonResult ObterListaClienteVisaoResumidaAreaTecnicaJson(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaCliente> listaCliente = ObterListaCliente(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Dashboard/_gridMVCClienteVisaoResumidaAreaTecnica.cshtml", listaCliente));
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

        protected List<Models.ListaCliente> ObterListaCliente(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaCliente> lista = new List<Models.ListaCliente>();
            decimal MargemPercentual = Convert.ToDecimal("0" + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MargemDashVendas));
            decimal RangeSuperior = 0;
            decimal RangeInferior = 0;

            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterListaCliente(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaCliente listaCliente = new Models.ListaCliente();

                        listaCliente.cliente.CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]);
                        listaCliente.cliente.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString();
                        listaCliente.cliente.regiao.CD_REGIAO = dataTableReader["CD_REGIAO"].ToString();
                        listaCliente.cliente.regiao.DS_REGIAO = dataTableReader["DS_REGIAO"].ToString();
                        listaCliente.ATIVOS = Convert.ToInt64("0" + dataTableReader["ATIVOS"]);
                        listaCliente.KAT = dataTableReader["KAT"].ToString();

                        if (dataTableReader["VISITAS"] != DBNull.Value)
                            listaCliente.VISITAS = Convert.ToInt64(dataTableReader["VISITAS"]);

                        if (dataTableReader["ST_TP_STATUS_VISITA_OS"] != DBNull.Value)
                            listaCliente.ST_TP_STATUS_VISITA_OS = Convert.ToInt64(dataTableReader["ST_TP_STATUS_VISITA_OS"]);

                        switch (dataTableReader["ST_TP_STATUS_VISITA_OS"])
                        {
                            case 1: //Novo
                                listaCliente.AGORA = "AGUARDANDO INÍCIO";
                                break;
                            case 2: //Aberta
                                listaCliente.AGORA = "ABERTA";
                                break;
                            case 3: //Portaria
                                listaCliente.AGORA = "FINALIZADA";
                                break;
                            case 4: //Integração
                                listaCliente.AGORA = "CANCELADA";
                                break;
                            case 5: //Treinamento
                                listaCliente.AGORA = "CONFIRMADA";
                                break;
                        }

                        listaCliente.TECNICO = dataTableReader["NM_TECNICO_PRINCIPAL"].ToString();

                        if (dataTableReader["PESQUISA"] != DBNull.Value)
                            listaCliente.PESQUISA = Convert.ToDecimal(dataTableReader["PESQUISA"]).ToString("N1");

                        //if (dataTableReader["SOLICIT"] != DBNull.Value)
                        //    listaCliente.SOLICIT = Convert.ToInt64(dataTableReader["SOLICIT"]);

                        if (dataTableReader["GM_ANO_ANTERIOR"] != DBNull.Value)
                            listaCliente.GM_ANO_ANTERIOR = Convert.ToDecimal(dataTableReader["GM_ANO_ANTERIOR"]).ToString("P2");

                        if (dataTableReader["GM_ANO_ATUAL"] != DBNull.Value)
                            listaCliente.GM_ANO_ATUAL = Convert.ToDecimal(dataTableReader["GM_ANO_ATUAL"]).ToString("P2");

                        if (dataTableReader["VENDAS_ANO_ANTERIOR"] != DBNull.Value)
                            listaCliente.VENDAS_ANO_ANTERIOR = Convert.ToDecimal(Convert.ToDecimal(dataTableReader["VENDAS_ANO_ANTERIOR"]) / 1000).ToString("N0");

                        if (dataTableReader["VENDAS_ANO_ATUAL"] != DBNull.Value)
                            listaCliente.VENDAS_ANO_ATUAL = Convert.ToDecimal(Convert.ToDecimal(dataTableReader["VENDAS_ANO_ATUAL"]) / 1000).ToString("N0");

                        if (dataTableReader["PERIODOS"] != DBNull.Value)
                            listaCliente.PERIODO = Convert.ToInt64(dataTableReader["PERIODOS"]);

                        if (dataTableReader["VENDAS_ANO_ANTERIOR"] != DBNull.Value)
                        {
                            RangeSuperior = Convert.ToDecimal(dataTableReader["VENDAS_ANO_ANTERIOR"]) + ((Convert.ToDecimal(dataTableReader["VENDAS_ANO_ANTERIOR"]) * MargemPercentual) / 100);
                            RangeInferior = Convert.ToDecimal(dataTableReader["VENDAS_ANO_ANTERIOR"]) - ((Convert.ToDecimal(dataTableReader["VENDAS_ANO_ANTERIOR"]) * MargemPercentual) / 100);
                        }

                        if (dataTableReader["VENDAS_ANO_ATUAL"] != DBNull.Value)
                        {
                            if (Convert.ToDecimal(dataTableReader["VENDAS_ANO_ATUAL"]) >= RangeInferior && Convert.ToDecimal(dataTableReader["VENDAS_ANO_ATUAL"]) <= RangeSuperior)
                                listaCliente.ICONEVENDAS = "=";
                            else if (Convert.ToDecimal(dataTableReader["VENDAS_ANO_ATUAL"]) > RangeSuperior)
                                listaCliente.ICONEVENDAS = "+";
                            else
                                listaCliente.ICONEVENDAS = "-";
                        }

                        lista.Add(listaCliente);
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

            return lista;

        }

        public JsonResult ObterListaEquipamentoJson(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                Models.EquipamentosGrid equipamentosGrid = ObterListaEquipamento(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO);
                List<Models.ListaEquipamento> listaEquipamento = equipamentosGrid.ListaEquipamento; 
                
                //new List<Models.ListaEquipamento>();
                //jsonResult.Add("DEPRECIACAO", listaEquipamento.Sum(x => x.TOT_DEPRECIACAO).ToString("C2"));
                //jsonResult.Add("TOT_PECAS", listaEquipamento.Sum(x => x.TOT_PECAS).ToString("C2"));
                //jsonResult.Add("TOT_MAO_OBRA", listaEquipamento.Sum(x => x.TOT_MAO_OBRA).ToString("C2"));

                jsonResult.Add("ManutHs", equipamentosGrid.ManutHs);
                jsonResult.Add("ManutPc", equipamentosGrid.ManutPc);
                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Dashboard/_gridMVCEquipamento.cshtml", listaEquipamento));
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

        protected Models.EquipamentosGrid ObterListaEquipamento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaEquipamento> lista = new List<Models.ListaEquipamento>();
            Models.EquipamentosGrid equipamentosGrid = new Models.EquipamentosGrid();

            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterListaEquipamento(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();
                //DataTableReader dataTableReader2 = Tabelas[1].CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaEquipamento listaEquipamento = new Models.ListaEquipamento();

                        listaEquipamento.modelo.CD_MODELO = dataTableReader["CD_MODELO"].ToString();
                        listaEquipamento.modelo.DS_MODELO = dataTableReader["DS_MODELO"].ToString();
                        listaEquipamento.ATIVOS = Convert.ToInt64("0" + dataTableReader["TOTAL"]);

                        if (dataTableReader["TOT_DEPRECIACAO"] != DBNull.Value)
                        {
                            listaEquipamento.TOT_DEPRECIACAO = Convert.ToDecimal(dataTableReader["TOT_DEPRECIACAO"]);
                            listaEquipamento.DEPRECIACAO = Convert.ToDecimal(dataTableReader["TOT_DEPRECIACAO"]).ToString("N2");
                        }

                        if (dataTableReader["CUSTO_MANUTENCAO"] != DBNull.Value)
                            listaEquipamento.CUSTO_MANUTENCAO = Convert.ToDecimal(dataTableReader["CUSTO_MANUTENCAO"]).ToString("C2");

                        if (dataTableReader["TOT_PECAS"] != DBNull.Value)
                            listaEquipamento.TOT_PECAS = Convert.ToDecimal(dataTableReader["TOT_PECAS"]);

                        if (dataTableReader["TOT_MAO_OBRA"] != DBNull.Value)
                            listaEquipamento.TOT_MAO_OBRA = Convert.ToDecimal(dataTableReader["TOT_MAO_OBRA"]);

                        lista.Add(listaEquipamento);
                    }
                }

                //if (dataTableReader2.HasRows)
                //{
                //    while (dataTableReader2.Read())
                //    {
                //        if (dataTableReader2["ManutHs"] != DBNull.Value)
                //            equipamentosGrid.ManutHs = Convert.ToDecimal(dataTableReader2["ManutHs"]);

                //        if (dataTableReader2["ManutPc"] != DBNull.Value)
                //            equipamentosGrid.ManutPc = Convert.ToDecimal(dataTableReader2["ManutPc"]);
                //    }
                //}

                equipamentosGrid.ListaEquipamento = lista;

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                //if (dataTableReader2 != null)
                //{
                //    dataTableReader2.Dispose();
                //    dataTableReader2 = null;
                //}

                DataTable TabelasManutencaoHsPc = new DashboardData().ObterManutencaoHsPc(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO);
                DataTableReader dataTableReaderManutencaoHsPc = TabelasManutencaoHsPc.CreateDataReader();

                if (dataTableReaderManutencaoHsPc.Read())
                {
                    if (dataTableReaderManutencaoHsPc["TOT_MAO_OBRA"] != DBNull.Value)
                        equipamentosGrid.ManutHs = Convert.ToDecimal(dataTableReaderManutencaoHsPc["TOT_MAO_OBRA"]).ToString("C2");

                    if (dataTableReaderManutencaoHsPc["TOT_PECAS"] != DBNull.Value)
                        equipamentosGrid.ManutPc = Convert.ToDecimal(dataTableReaderManutencaoHsPc["TOT_PECAS"]).ToString("C2");
                }

                if (TabelasManutencaoHsPc != null)
                {
                    TabelasManutencaoHsPc.Dispose();
                    TabelasManutencaoHsPc = null;
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return equipamentosGrid;

        }
        #endregion

        #region Planejamento
        public JsonResult ObterListaEquipamentoWorkFlowJson(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaEquipamentoWorkFlow> listaEquipamentoWorkFlow = ObterListaEquipamentoWorkFlow(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Dashboard/_gridMVCEquipamentoWorkFlow.cshtml", listaEquipamentoWorkFlow));
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

        protected List<Models.ListaEquipamentoWorkFlow> ObterListaEquipamentoWorkFlow(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaEquipamentoWorkFlow> lista = new List<Models.ListaEquipamentoWorkFlow>();

            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterListaEquipamentoWorkFlow(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaEquipamentoWorkFlow listaEquipamentoWorkFlow = new Models.ListaEquipamentoWorkFlow();

                        listaEquipamentoWorkFlow.modelo.CD_MODELO = dataTableReader["CD_MODELO"].ToString();
                        listaEquipamentoWorkFlow.modelo.DS_MODELO = dataTableReader["DS_MODELO"].ToString();
                        listaEquipamentoWorkFlow.ATIVOS = Convert.ToInt64("0" + dataTableReader["TOTAL"]);

                        if (dataTableReader["ENV"] != DBNull.Value)
                            listaEquipamentoWorkFlow.ENV = Convert.ToInt64(dataTableReader["ENV"]);

                        if (dataTableReader["MEN"] != DBNull.Value)
                            listaEquipamentoWorkFlow.MEN = Convert.ToDecimal(dataTableReader["MEN"]);

                        if (dataTableReader["DEV"] != DBNull.Value)
                            listaEquipamentoWorkFlow.DEV = Convert.ToInt64(dataTableReader["DEV"]);

                        if (dataTableReader["PRJ"] != DBNull.Value)
                            listaEquipamentoWorkFlow.PRJ = Convert.ToInt64(dataTableReader["PRJ"]);

                        if (listaEquipamentoWorkFlow.PRJ > 0)
                            listaEquipamentoWorkFlow.PERCENTUAL = Convert.ToDecimal(Convert.ToDecimal((listaEquipamentoWorkFlow.ENV * 100) / listaEquipamentoWorkFlow.PRJ).ToString("N2"));

                        if (listaEquipamentoWorkFlow.PRJ > 0)
                        {
                            decimal Limite = listaEquipamentoWorkFlow.PRJ - ((listaEquipamentoWorkFlow.PRJ * 10) / 100);
                            if (listaEquipamentoWorkFlow.ENV < Limite)
                                listaEquipamentoWorkFlow.CORFUNDO = "VERMELHO";
                            else if (listaEquipamentoWorkFlow.ENV >= Limite && listaEquipamentoWorkFlow.ENV < listaEquipamentoWorkFlow.PRJ)
                                listaEquipamentoWorkFlow.CORFUNDO = "AMARELO";
                            else if (listaEquipamentoWorkFlow.ENV >= listaEquipamentoWorkFlow.PRJ)
                                listaEquipamentoWorkFlow.CORFUNDO = "VERDE";
                        }

                        lista.Add(listaEquipamentoWorkFlow);
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

            return lista;

        }
        #endregion

        #region Vendas
        public JsonResult ObterListaModeloJson(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaModelo> listaModelo = ObterListaModelo(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Dashboard/_gridMVCModelo.cshtml", listaModelo));
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

        protected List<Models.ListaModelo> ObterListaModelo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaModelo> lista = new List<Models.ListaModelo>();

            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterListaModelo(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaModelo listaModelo = new Models.ListaModelo();

                        listaModelo.modelo.CD_MODELO = dataTableReader["CD_MODELO"].ToString();
                        listaModelo.modelo.DS_MODELO = dataTableReader["DS_MODELO"].ToString();
                        listaModelo.ATIVOS = Convert.ToInt64("0" + dataTableReader["TOTAL"]);

                        lista.Add(listaModelo);
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

            return lista;

        }

        public JsonResult ObterListaHistoricoJson(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaHistorico> listaHistorico = ObterListaHistorico(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Dashboard/_gridMVCHistorico.cshtml", listaHistorico));
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

        protected List<Models.ListaHistorico> ObterListaHistorico(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaHistorico> lista = new List<Models.ListaHistorico>();
            Models.ListaHistorico listaHistorico_Header = null;
            Models.ListaHistorico listaHistorico_Vendas = null;
            Models.ListaHistorico listaHistorico_Quantidade = null;
            Models.ListaHistorico listaHistorico_Volume = null;
            decimal[] TotalVendas = new decimal[12];
            decimal[] TotalQuantidade = new decimal[12];
            decimal[] TotalVolume = new decimal[12];

            string CD_CONSUMIVEL = string.Empty;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterListaHistorico(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        if (CD_CONSUMIVEL != dataTableReader["CD_CONSUMIVEL"].ToString())
                        {
                            if (!string.IsNullOrEmpty(CD_CONSUMIVEL))
                            {
                                lista.Add(listaHistorico_Vendas);
                                lista.Add(listaHistorico_Quantidade);
                                lista.Add(listaHistorico_Volume);
                            }

                            CD_CONSUMIVEL = dataTableReader["CD_CONSUMIVEL"].ToString();

                            listaHistorico_Header = new Models.ListaHistorico();
                            //listaHistorico.consumivel.CD_CONSUMIVEL = dataTableReader["CD_CONSUMIVEL"].ToString();
                            listaHistorico_Header.ITEM = dataTableReader["DS_CONSUMIVEL"].ToString();
                            lista.Add(listaHistorico_Header);

                            listaHistorico_Vendas = new Models.ListaHistorico();
                            listaHistorico_Vendas.ITEM = ".     Vendas";

                            listaHistorico_Quantidade = new Models.ListaHistorico();
                            listaHistorico_Quantidade.ITEM = ".     Quantidade item (unit.)";

                            listaHistorico_Volume = new Models.ListaHistorico();
                            listaHistorico_Volume.ITEM = ".     Volume (m²,kg)";
                        }

                        //listaHistorico.CD_MES = Convert.ToInt32(dataTableReader["CD_MES"]);

                        switch (dataTableReader["CD_MES"].ToString().Substring(4, 2))
                        {
                            case "01":
                                listaHistorico_Vendas.JANEIRO = Convert.ToDecimal(dataTableReader["TOT_VENDAS"]).ToString("C0");
                                listaHistorico_Volume.JANEIRO = Convert.ToDecimal(dataTableReader["VOLUME"]).ToString("N0");
                                TotalVendas[0] += Convert.ToDecimal(dataTableReader["TOT_VENDAS"]);
                                TotalVolume[0] += Convert.ToDecimal(dataTableReader["VOLUME"]);
                                break;
                            case "02":
                                listaHistorico_Vendas.FEVEREIRO = Convert.ToDecimal(dataTableReader["TOT_VENDAS"]).ToString("C0");
                                listaHistorico_Volume.FEVEREIRO = Convert.ToDecimal(dataTableReader["VOLUME"]).ToString("N0");
                                TotalVendas[1] += Convert.ToDecimal(dataTableReader["TOT_VENDAS"]);
                                TotalVolume[1] += Convert.ToDecimal(dataTableReader["VOLUME"]);
                                break;
                            case "03":
                                listaHistorico_Vendas.MARCO = Convert.ToDecimal(dataTableReader["TOT_VENDAS"]).ToString("C0");
                                listaHistorico_Volume.MARCO = Convert.ToDecimal(dataTableReader["VOLUME"]).ToString("N0");
                                TotalVendas[2] += Convert.ToDecimal(dataTableReader["TOT_VENDAS"]);
                                TotalVolume[2] += Convert.ToDecimal(dataTableReader["VOLUME"]);
                                break;
                            case "04":
                                listaHistorico_Vendas.ABRIL = Convert.ToDecimal(dataTableReader["TOT_VENDAS"]).ToString("C0");
                                listaHistorico_Volume.ABRIL = Convert.ToDecimal(dataTableReader["VOLUME"]).ToString("N0");
                                TotalVendas[3] += Convert.ToDecimal(dataTableReader["TOT_VENDAS"]);
                                TotalVolume[3] += Convert.ToDecimal(dataTableReader["VOLUME"]);
                                break;
                            case "05":
                                listaHistorico_Vendas.MAIO = Convert.ToDecimal(dataTableReader["TOT_VENDAS"]).ToString("C0");
                                listaHistorico_Volume.MAIO = Convert.ToDecimal(dataTableReader["VOLUME"]).ToString("N0");
                                TotalVendas[4] += Convert.ToDecimal(dataTableReader["TOT_VENDAS"]);
                                TotalVolume[4] += Convert.ToDecimal(dataTableReader["VOLUME"]);
                                break;
                            case "06":
                                listaHistorico_Vendas.JUNHO = Convert.ToDecimal(dataTableReader["TOT_VENDAS"]).ToString("C0");
                                listaHistorico_Volume.JUNHO = Convert.ToDecimal(dataTableReader["VOLUME"]).ToString("N0");
                                TotalVendas[5] += Convert.ToDecimal(dataTableReader["TOT_VENDAS"]);
                                TotalVolume[5] += Convert.ToDecimal(dataTableReader["VOLUME"]);
                                break;
                            case "07":
                                listaHistorico_Vendas.JULHO = Convert.ToDecimal(dataTableReader["TOT_VENDAS"]).ToString("C0");
                                listaHistorico_Volume.JULHO = Convert.ToDecimal(dataTableReader["VOLUME"]).ToString("N0");
                                TotalVendas[6] += Convert.ToDecimal(dataTableReader["TOT_VENDAS"]);
                                TotalVolume[6] += Convert.ToDecimal(dataTableReader["VOLUME"]);
                                break;
                            case "08":
                                listaHistorico_Vendas.AGOSTO = Convert.ToDecimal(dataTableReader["TOT_VENDAS"]).ToString("C0");
                                listaHistorico_Volume.AGOSTO = Convert.ToDecimal(dataTableReader["VOLUME"]).ToString("N0");
                                TotalVendas[7] += Convert.ToDecimal(dataTableReader["TOT_VENDAS"]);
                                TotalVolume[7] += Convert.ToDecimal(dataTableReader["VOLUME"]);
                                break;
                            case "09":
                                listaHistorico_Vendas.SETEMBRO = Convert.ToDecimal(dataTableReader["TOT_VENDAS"]).ToString("C0");
                                listaHistorico_Volume.SETEMBRO = Convert.ToDecimal(dataTableReader["VOLUME"]).ToString("N0");
                                TotalVendas[8] += Convert.ToDecimal(dataTableReader["TOT_VENDAS"]);
                                TotalVolume[8] += Convert.ToDecimal(dataTableReader["VOLUME"]);
                                break;
                            case "10":
                                listaHistorico_Vendas.OUTUBRO = Convert.ToDecimal(dataTableReader["TOT_VENDAS"]).ToString("C0");
                                listaHistorico_Volume.OUTUBRO = Convert.ToDecimal(dataTableReader["VOLUME"]).ToString("N0");
                                TotalVendas[9] += Convert.ToDecimal(dataTableReader["TOT_VENDAS"]);
                                TotalVolume[9] += Convert.ToDecimal(dataTableReader["VOLUME"]);
                                break;
                            case "11":
                                listaHistorico_Vendas.NOVEMBRO = Convert.ToDecimal(dataTableReader["TOT_VENDAS"]).ToString("C0");
                                listaHistorico_Volume.NOVEMBRO = Convert.ToDecimal(dataTableReader["VOLUME"]).ToString("N0");
                                TotalVendas[10] += Convert.ToDecimal(dataTableReader["TOT_VENDAS"]);
                                TotalVolume[10] += Convert.ToDecimal(dataTableReader["VOLUME"]);
                                break;
                            case "12":
                                listaHistorico_Vendas.DEZEMBRO = Convert.ToDecimal(dataTableReader["TOT_VENDAS"]).ToString("C0");
                                listaHistorico_Volume.DEZEMBRO = Convert.ToDecimal(dataTableReader["VOLUME"]).ToString("N0");
                                TotalVendas[11] += Convert.ToDecimal(dataTableReader["TOT_VENDAS"]);
                                TotalVolume[11] += Convert.ToDecimal(dataTableReader["VOLUME"]);
                                break;
                        }
                    }
                }

                if (listaHistorico_Vendas != null)
                    lista.Add(listaHistorico_Vendas);
                if (listaHistorico_Quantidade != null)
                    lista.Add(listaHistorico_Quantidade);
                if (listaHistorico_Volume != null)
                    lista.Add(listaHistorico_Volume);

                //TOTAIS
                listaHistorico_Vendas = new Models.ListaHistorico();
                listaHistorico_Vendas.ITEM = ".     Vendas";
                listaHistorico_Vendas.TOTAL = true;

                listaHistorico_Quantidade = new Models.ListaHistorico();
                listaHistorico_Quantidade.ITEM = ".     Quantidade item (unit.)";
                listaHistorico_Quantidade.TOTAL = true;

                listaHistorico_Volume = new Models.ListaHistorico();
                listaHistorico_Volume.ITEM = ".     Volume (m²,kg)";
                listaHistorico_Volume.TOTAL = true;

                for (int a = 0; a < 12; a++)
                {
                    switch (a)
                    {
                        case 0:
                            listaHistorico_Vendas.JANEIRO = TotalVendas[a].ToString("C0");
                            listaHistorico_Volume.JANEIRO = TotalVolume[a].ToString("C0");
                            break;
                        case 1:
                            listaHistorico_Vendas.FEVEREIRO = TotalVendas[a].ToString("C0");
                            listaHistorico_Volume.FEVEREIRO = TotalVolume[a].ToString("C0");
                            break;
                        case 2:
                            listaHistorico_Vendas.MARCO = TotalVendas[a].ToString("C0");
                            listaHistorico_Volume.MARCO = TotalVolume[a].ToString("C0");
                            break;
                        case 03:
                            listaHistorico_Vendas.ABRIL = TotalVendas[a].ToString("C0");
                            listaHistorico_Volume.ABRIL = TotalVolume[a].ToString("C0");
                            break;
                        case 4:
                            listaHistorico_Vendas.MAIO = TotalVendas[a].ToString("C0");
                            listaHistorico_Volume.MAIO = TotalVolume[a].ToString("C0");
                            break;
                        case 5:
                            listaHistorico_Vendas.JUNHO = TotalVendas[a].ToString("C0");
                            listaHistorico_Volume.JUNHO = TotalVolume[a].ToString("C0");
                            break;
                        case 6:
                            listaHistorico_Vendas.JULHO = TotalVendas[a].ToString("C0");
                            listaHistorico_Volume.JULHO = TotalVolume[a].ToString("C0");
                            break;
                        case 7:
                            listaHistorico_Vendas.AGOSTO = TotalVendas[a].ToString("C0");
                            listaHistorico_Volume.AGOSTO = TotalVolume[a].ToString("C0");
                            break;
                        case 8:
                            listaHistorico_Vendas.SETEMBRO = TotalVendas[a].ToString("C0");
                            listaHistorico_Volume.SETEMBRO = TotalVolume[a].ToString("C0");
                            break;
                        case 9:
                            listaHistorico_Vendas.OUTUBRO = TotalVendas[a].ToString("C0");
                            listaHistorico_Volume.OUTUBRO = TotalVolume[a].ToString("C0");
                            break;
                        case 10:
                            listaHistorico_Vendas.NOVEMBRO = TotalVendas[a].ToString("C0");
                            listaHistorico_Volume.NOVEMBRO = TotalVolume[a].ToString("C0");
                            break;
                        case 11:
                            listaHistorico_Vendas.DEZEMBRO = TotalVendas[a].ToString("C0");
                            listaHistorico_Volume.DEZEMBRO = TotalVolume[a].ToString("C0");
                            break;
                    }
                }

                lista.Add(listaHistorico_Vendas);
                lista.Add(listaHistorico_Quantidade);
                lista.Add(listaHistorico_Volume);

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

            return lista;

        }
        #endregion

        #region Negócios
        public JsonResult ObterListaHistoricoValoresJson(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaHistoricoValores> listaHistoricoValores = ObterListaHistoricoValores(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Dashboard/_gridMVCHistoricoValores.cshtml", listaHistoricoValores));
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

        protected List<Models.ListaHistoricoValores> ObterListaHistoricoValores(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaHistoricoValores> lista = new List<Models.ListaHistoricoValores>();
            Models.ListaHistoricoValores listaHistoricoValor = null;

            string CD_MES = string.Empty;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterListaHistoricoValores(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        if (CD_MES != dataTableReader["CD_MES"].ToString())
                        {
                            if (!string.IsNullOrEmpty(CD_MES))
                            {
                                lista.Add(listaHistoricoValor);
                            }

                            CD_MES = dataTableReader["CD_MES"].ToString();

                            listaHistoricoValor = new Models.ListaHistoricoValores();
                            listaHistoricoValor.ANO = DateTime.Now.Year.ToString();
                            listaHistoricoValor.MES = dataTableReader["DS_MES"].ToString();

                            //switch (dataTableReader["CD_MES"].ToString().Substring(4, 2))
                            //{
                            //    case "01":
                            //        listaHistoricoValor.MES = "JANEIRO";
                            //        break;
                            //    case "02":
                            //        listaHistoricoValor.MES = "FEVEREIRO";
                            //        break;
                            //    case "03":
                            //        listaHistoricoValor.MES = "MARÇO";
                            //        break;
                            //    case "04":
                            //        listaHistoricoValor.MES = "ABRIL";
                            //        break;
                            //    case "05":
                            //        listaHistoricoValor.MES = "MAIO";
                            //        break;
                            //    case "06":
                            //        listaHistoricoValor.MES = "JUNHO";
                            //        break;
                            //    case "07":
                            //        listaHistoricoValor.MES = "JULHO";
                            //        break;
                            //    case "08":
                            //        listaHistoricoValor.MES = "AGOSTO";
                            //        break;
                            //    case "09":
                            //        listaHistoricoValor.MES = "SETEMBRO";
                            //        break;
                            //    case "10":
                            //        listaHistoricoValor.MES = "OUTUBRO";
                            //        break;
                            //    case "11":
                            //        listaHistoricoValor.MES = "NOVEMBRO";
                            //        break;
                            //    case "12":
                            //        listaHistoricoValor.MES = "DEZEMBRO";
                            //        break;
                            //}
                        }

                        if (dataTableReader["CD_COMMODITY"].ToString() == "2650")
                        {
                            listaHistoricoValor.VENDAS_2650 = Convert.ToDecimal(dataTableReader["TOTAL_VENDAS"]);
                            listaHistoricoValor.MANUTENCAO_2650 = Convert.ToDecimal(dataTableReader["TOTAL_MANUTENCAO"]);
                            listaHistoricoValor.DEPRECIACAO_2650 = Convert.ToDecimal(dataTableReader["TOTAL_DEPRECIACAO"]);
                        }
                        else if (dataTableReader["CD_COMMODITY"].ToString() == "2690")
                        {
                            listaHistoricoValor.VENDAS_2690 = Convert.ToDecimal(dataTableReader["TOTAL_VENDAS"]);
                            listaHistoricoValor.MANUTENCAO_2690 = Convert.ToDecimal(dataTableReader["TOTAL_MANUTENCAO"]);
                            listaHistoricoValor.DEPRECIACAO_2690 = Convert.ToDecimal(dataTableReader["TOTAL_DEPRECIACAO"]);
                        }

                        listaHistoricoValor.VENDAS_TOTAL = listaHistoricoValor.VENDAS_2650 + listaHistoricoValor.VENDAS_2690;
                        listaHistoricoValor.MANUTENCAO_TOTAL = listaHistoricoValor.MANUTENCAO_2650 + listaHistoricoValor.MANUTENCAO_2690;
                        listaHistoricoValor.DEPRECIACAO_TOTAL = listaHistoricoValor.DEPRECIACAO_2650 + listaHistoricoValor.DEPRECIACAO_2690;
                    }
                }

                if (listaHistoricoValor != null)
                    lista.Add(listaHistoricoValor);

                //TOTAIS
                listaHistoricoValor = new Models.ListaHistoricoValores();
                listaHistoricoValor.ANO = "TOTAL";
                listaHistoricoValor.TOTAL = true;

                listaHistoricoValor.VENDAS_2650 = lista.Sum(a => a.VENDAS_2650);
                listaHistoricoValor.MANUTENCAO_2650 = lista.Sum(a => a.MANUTENCAO_2650);
                listaHistoricoValor.DEPRECIACAO_2650 = lista.Sum(a => a.DEPRECIACAO_2650);

                listaHistoricoValor.VENDAS_2690 = lista.Sum(a => a.VENDAS_2690);
                listaHistoricoValor.MANUTENCAO_2690 = lista.Sum(a => a.MANUTENCAO_2690);
                listaHistoricoValor.DEPRECIACAO_2690 = lista.Sum(a => a.DEPRECIACAO_2690);

                listaHistoricoValor.VENDAS_TOTAL = lista.Sum(a => a.VENDAS_TOTAL);
                listaHistoricoValor.MANUTENCAO_TOTAL = lista.Sum(a => a.MANUTENCAO_TOTAL);
                listaHistoricoValor.DEPRECIACAO_TOTAL = lista.Sum(a => a.DEPRECIACAO_TOTAL);

                lista.Add(listaHistoricoValor);

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

            return lista;

        }
        #endregion

        #region Área Técnica
        public JsonResult ObterListaTecnicoJson(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaTecnico> listaTecnico = ObterListaTecnico(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Dashboard/_gridMVCTecnico.cshtml", listaTecnico));
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

        protected List<Models.ListaTecnico> ObterListaTecnico(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaTecnico> listaTecnicos = new List<Models.ListaTecnico>();

            try
            {
                // Busca todos os técnicos de CD_ORDEM = 1 (Somente os que possuem agenda)
                DataTableReader dtTecnicoCliente = new DashboardData().ObterListaTecnico(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dtTecnicoCliente.HasRows)
                {
                    while (dtTecnicoCliente.Read())
                    {
                        // Para cada técnico com agenda, busca as qtde. de períodos, realizados e percentual por cliente
                        List<Models.ListaAgendaAtendimento> listaAgendaAtendimentos = new List<Models.ListaAgendaAtendimento>();

                        AgendaEntity agendaEntity = new AgendaEntity();
                        agendaEntity.tecnico.CD_TECNICO = dtTecnicoCliente["CD_TECNICO"].ToString();

                        DataTableReader dataTableReader = new AgendaData().ObterListaAtendimento(agendaEntity, null, null, null, null).CreateDataReader();

                        if (dataTableReader.HasRows)
                        {
                            while (dataTableReader.Read())
                            {
                                Models.ListaAgendaAtendimento listaAgendaAtendimento = new Models.ListaAgendaAtendimento();
                                if (Convert.ToInt64("0" + dataTableReader["ID_VISITA"]).ToString() != "")
                                    listaAgendaAtendimento.ID_VISITA = Convert.ToInt64("0" + dataTableReader["ID_VISITA"]);

                                if (Convert.ToInt32("0" + dataTableReader["ST_TP_STATUS_VISITA_OS"]).ToString() != "")
                                    listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS = Convert.ToInt32("0" + dataTableReader["ST_TP_STATUS_VISITA_OS"]);

                                //if (Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]).ToString() != "")
                                //    listaAgendaAtendimento.ID_AGENDA = Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]);

                                if (dataTableReader["QT_PERIODO"] != DBNull.Value)
                                    listaAgendaAtendimento.QT_PERIODO = Convert.ToInt32(dataTableReader["QT_PERIODO"]);
                                else
                                    listaAgendaAtendimento.QT_PERIODO = 0;

                                listaAgendaAtendimento.listaLogStatusVisita = ObterListaLogStatusVisita(listaAgendaAtendimento.ID_VISITA);
                                listaAgendaAtendimento.TempoGastoTOTAL = CalcularTempoGastoVisita(listaAgendaAtendimento.listaLogStatusVisita);

                                try
                                {
                                    bool contabilizarVisita = listaAgendaAtendimento.listaLogStatusVisita
                                        .Where(c => c.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == ControlesUtility.Enumeradores.TpStatusVisita.Consultoria.ToInt()).Count() == 0;

                                    if (contabilizarVisita)
                                    {
                                        listaAgendaAtendimento.QT_PERIODO_REALIZADO = Convert.ToDecimal(listaAgendaAtendimento.TempoGastoTOTAL.TotalHours) / 3;//new TimeSpan(listaAgendaAtendimento.TempoGastoTOTAL.Ticks / 3);
                                        listaAgendaAtendimento.QT_PERIODO_REALIZADO_FORMATADO = listaAgendaAtendimento.QT_PERIODO_REALIZADO.ToString("N2"); //Convert.ToDateTime(listaAgendaAtendimento.QT_PERIODO_REALIZADO.ToString()).ToString("HH:mm");
                                                                                                                                                            //TimeSpan QT_PERIODO = TimeSpan.FromHours(listaAgendaAtendimento.QT_PERIODO);
                                        if (listaAgendaAtendimento.QT_PERIODO > 0)
                                            listaAgendaAtendimento.PERCENTUAL = Convert.ToDecimal((listaAgendaAtendimento.QT_PERIODO_REALIZADO * 100) / (listaAgendaAtendimento.QT_PERIODO * 3)).ToString("N2");
                                    }
                                    else
                                    {
                                        listaAgendaAtendimento.QT_PERIODO_REALIZADO = 0;
                                        listaAgendaAtendimento.QT_PERIODO_REALIZADO_FORMATADO = listaAgendaAtendimento.QT_PERIODO_REALIZADO.ToString("N2");
                                        listaAgendaAtendimento.PERCENTUAL = Convert.ToDecimal(0).ToString("N2");
                                    }
                                }
                                catch
                                {
                                    listaAgendaAtendimento.PERCENTUAL = Convert.ToDecimal(0).ToString("N2");
                                }

                                listaAgendaAtendimentos.Add(listaAgendaAtendimento);
                            }
                        }

                        if (dataTableReader != null)
                        {
                            dataTableReader.Dispose();
                            dataTableReader = null;
                        }

                        // Adiciona na lista de Técnicos da grade, o técnico e a soma de períodos e percentuais realizados (todos os seus clientes)
                        Models.ListaTecnico listaTecnico = new Models.ListaTecnico();

                        listaTecnico.tecnico.CD_TECNICO = dtTecnicoCliente["CD_TECNICO"].ToString();
                        listaTecnico.tecnico.NM_TECNICO = dtTecnicoCliente["NM_TECNICO"].ToString();
                        listaTecnico.tecnico.NM_REDUZIDO = dtTecnicoCliente["NM_REDUZIDO"].ToString();
                        listaTecnico.tecnico.EN_ESTADO = dtTecnicoCliente["EN_ESTADO"].ToString();

                        listaTecnico.CARGA = listaAgendaAtendimentos.Sum(a => a.QT_PERIODO);
                        if (listaTecnico.CARGA > 0)
                            listaTecnico.PERCENTUAL = Convert.ToInt64((listaAgendaAtendimentos.Sum(a => a.QT_PERIODO_REALIZADO) * 100) / (listaAgendaAtendimentos.Sum(a => a.QT_PERIODO) * 3)); //listaAgendaAtendimentos.Sum(a => Convert.ToDecimal(a.PERCENTUAL));

                        if (dtTecnicoCliente["VISITAS"] != DBNull.Value)
                            listaTecnico.VISITAS = Convert.ToInt64(dtTecnicoCliente["VISITAS"]);
                        if (dtTecnicoCliente["ST_TP_STATUS_VISITA_OS"] != DBNull.Value)
                            listaTecnico.ST_TP_STATUS_VISITA_OS = Convert.ToInt64(dtTecnicoCliente["ST_TP_STATUS_VISITA_OS"]);

                        if (listaTecnico.ST_TP_STATUS_VISITA_OS == null || listaTecnico.ST_TP_STATUS_VISITA_OS == 0)
                        {
                            listaTecnico.ST_TP_STATUS_VISITA_OS = 3;
                        }
                        switch (dtTecnicoCliente["ST_TP_STATUS_VISITA_OS"])
                        {
                            case 1: //Novo
                                listaTecnico.AGORA = "AGUARDANDO INÍCIO";
                                break;
                            case 2: //Aberta
                                listaTecnico.AGORA = "ABERTA";
                                break;
                            case 3: //Portaria
                                listaTecnico.AGORA = "FINALIZADA";
                                break;
                            case 4: //Integração
                                listaTecnico.AGORA = "CANCELADA";
                                break;
                            case 5: //Treinamento
                                listaTecnico.AGORA = "CONFIRMADA";
                                break;
                            default:
                                listaTecnico.AGORA = string.Empty;
                                break;
                        }

                        listaTecnicos.Add(listaTecnico);
                    }
                }

                if (dtTecnicoCliente != null)
                {
                    dtTecnicoCliente.Dispose();
                    dtTecnicoCliente = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            listaTecnicos = listaTecnicos.OrderByDescending(x => x.AGORA).ThenBy(x => x.tecnico.NM_REDUZIDO).ToList();

            return listaTecnicos;

        }

        protected List<Models.LogStatusVisita> ObterListaLogStatusVisita(Int64 ID_VISITA)
        {
            List<Models.LogStatusVisita> listaLogStatusVisita = new List<Models.LogStatusVisita>();

            if (ID_VISITA == 0)
            {
                return listaLogStatusVisita;
            }

            try
            {
                LogStatusVisitaEntity logStatusVisitaEntity = new LogStatusVisitaEntity();
                logStatusVisitaEntity.visita.ID_VISITA = ID_VISITA;
                DataTableReader dataTableReader = new LogStatusVisitaData().ObterLista(logStatusVisitaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.LogStatusVisita logStatusVisita = new Models.LogStatusVisita
                        {
                            ID_LOG_STATUS_VISITA = Convert.ToInt64(dataTableReader["ID_LOG_STATUS_OS"]),
                            visita = new VisitaEntity()
                            {
                                ID_VISITA = Convert.ToInt64(dataTableReader["ID_OS"])
                            },
                            DT_DATA_LOG_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_OS"]), //.ToString("dd/MM/yyyy HH:mm"),
                            tpStatusVisitaOS = new TpStatusVisitaOSEntity()
                            {
                                ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_STATUS_OS"]),
                                DS_TP_STATUS_VISITA_OS = dataTableReader["DS_STATUS_OS"].ToString()
                            },
                            usuario = new UsuarioEntity()
                            {
                                cnmNome = dataTableReader["cnmNome"].ToString()
                            }
                        };

                        listaLogStatusVisita.Add(logStatusVisita);
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

            return listaLogStatusVisita;

        }

        /// <summary>
        /// Calcula o tempo gasto no histórico de LOG_STATUS_VISITA
        /// </summary>
        /// <param name="listasLogStatusVisita"></param>
        /// <returns></returns>
        protected TimeSpan CalcularTempoGastoVisita(List<Models.LogStatusVisita> listasLogStatusVisita)
        {
            //string CD_TECNICO = string.Empty;
            TimeSpan TempoGasto = new TimeSpan();
            TimeSpan TempoGastoTOTALTECNICO = new TimeSpan();
            TimeSpan TempoGastoTOTAL = new TimeSpan();
            string TempoTOTAL = string.Empty;
            DateTime? DT_DATA_LOG1 = null;
            DateTime? DT_DATA_LOG2 = null;

            try
            {
                foreach (Models.LogStatusVisita logStatusVisita in listasLogStatusVisita)
                {
                    if (logStatusVisita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS != Convert.ToInt32(Utility.ControlesUtility.Enumeradores.TpStatusVisita.Nova))
                    {
                        //if (string.IsNullOrEmpty(CD_TECNICO))
                        //    CD_TECNICO = logStatusVisita.CD_TECNICO;

                        //if (CD_TECNICO != logStatusVisita.CD_TECNICO)
                        //{
                        //    CD_TECNICO = logStatusVisita.CD_TECNICO;
                        //    TempoGastoTOTALTECNICO = new TimeSpan();
                        //    DT_DATA_LOG1 = null;
                        //    DT_DATA_LOG2 = null;
                        //}

                        if (logStatusVisita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(Utility.ControlesUtility.Enumeradores.TpStatusOSPadrao.Aberta))
                        {
                            DT_DATA_LOG1 = logStatusVisita.DT_DATA_LOG_VISITA;
                            DT_DATA_LOG2 = null;
                        }
                        else
                        {
                            DT_DATA_LOG2 = logStatusVisita.DT_DATA_LOG_VISITA;
                        }

                        if (DT_DATA_LOG1 != null && DT_DATA_LOG2 != null)
                        {
                            TempoGasto = (Convert.ToDateTime(DT_DATA_LOG2) - Convert.ToDateTime(DT_DATA_LOG1));
                            TempoGastoTOTALTECNICO += TempoGasto;
                            TempoGastoTOTAL += TempoGasto;

                            logStatusVisita.TempoGasto = TempoGasto;
                            logStatusVisita.TempoGastoTOTAL = TempoGastoTOTALTECNICO;

                            DT_DATA_LOG1 = null;
                            DT_DATA_LOG2 = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return TempoGastoTOTAL;
        }
        #endregion
    }

}