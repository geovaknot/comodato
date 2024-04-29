using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class SolicitacaoPecasController : BaseController
    {
        [_3MAuthentication]
        public ActionResult Index()
        {
            Models.SolicitacaoPecas solicitacaoPecas = new Models.SolicitacaoPecas
            {
                DT_CRIACAO_INICIO = DateTime.Now.AddDays(-90).ToString("dd/MM/yyyy"),
                DT_CRIACAO_FIM = DateTime.Now.ToString("dd/MM/yyyy"),
                clientes = new List<Models.Cliente>(),
                tecnicos = new List<Models.Tecnico>(),
                pedidos = new List<Models.Pedido>(),
                statusPedidos = new List<Models.StatusPedido>()
                //tiposStatusVisitaOS = new List<Models.TpStatusVisitaOS>(),
            };

            return View(solicitacaoPecas);
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.PreenchimentoSolicitacaoPecas preenchimentoSolicitacaoPecas = null;

            try
            {
                string IDKey = idKey.Replace(" ", "+");

                string[] conteudo = null;
                try
                {
                    conteudo = ControlesUtility.Criptografia.Descriptografar(IDKey).Split('|');
                }
                catch { }

                if (conteudo == null)
                {
                    if (IDKey.Substring(IDKey.Length - 1, 1) != "=")
                    {
                        IDKey += "=";
                    }

                    conteudo = ControlesUtility.Criptografia.Descriptografar(IDKey).Split('|');
                }

                ViewBag.URLParam = IDKey;

                //string[] conteudo = ControlesUtility.Criptografia.Descriptografar(idKey).Split('|');
                conteudo = ControlesUtility.Criptografia.Descriptografar(IDKey).Split('|');
                string CD_TECNICO = conteudo[0];
                Int64 ID_PEDIDO = Convert.ToInt64(conteudo[1]);
                string TP_TIPO_PEDIDO = conteudo[2];
                string tipoOrigemPagina = conteudo[3];
                string statusPedido = conteudo[4];
                string tipoPedido = "";
                string origem = "";
                if (conteudo.Length > 5)
                    tipoPedido = conteudo[5];
                if (conteudo.Length > 6)
                    origem = conteudo[6];

                var descOrigem = "";

                if (origem == null)
                    descOrigem = "";
                else if (origem == "")
                    descOrigem = "";
                else if(origem == "W")
                    descOrigem = "Web";
                else if (origem == "A")
                    descOrigem = "App";

                if (statusPedido == "")
                {
                    statusPedido = "0";
                }

                string Desc_Pedido = "";

                if (tipoPedido == "E")
                {
                    Desc_Pedido = "Especial";
                }
                else if (tipoPedido == "N")
                {
                    Desc_Pedido = "Normal";
                }
                else
                {
                    Desc_Pedido = "";
                }
                EstoqueData estoqueData = new EstoqueData();
                DataTable dataTable = estoqueData.ObterLista(new EstoqueEntity() { FL_ATIVO = "S", TP_ESTOQUE_TEC_3M = "3M%" });
                var listaEstoque = from row in dataTable.Rows.Cast<DataRow>()
                                   select new EstoqueEntity()
                                   {
                                       ID_ESTOQUE = row.FieldOrDefault<long>("ID_ESTOQUE"),
                                       CD_ESTOQUE = row["CD_ESTOQUE"].ToString(),
                                       DS_ESTOQUE = row["DS_ESTOQUE"].ToString().Trim(),
                                       TP_ESTOQUE_TEC_3M = row["TP_ESTOQUE_TEC_3M"].ToString().Trim()
                                   };

                preenchimentoSolicitacaoPecas = new Models.PreenchimentoSolicitacaoPecas
                {
                    //    idKeyInicial = ControlesUtility.Criptografia.Criptografar(ID_AGENDA.ToString() + "|" + ID_VISITA.ToString() + "|" + CD_CLIENTE.ToString() + "|" + CD_TECNICO.ToString() + "|0"),
                    ID_PEDIDO = ID_PEDIDO,
                    tecnico = new TecnicoEntity()
                    {
                        CD_TECNICO = CD_TECNICO
                    },
                    TP_TIPO_PEDIDO = TP_TIPO_PEDIDO,
                    tipoOrigemPagina = tipoOrigemPagina,
                    statusPedidos = new List<Models.StatusPedido>(),
                    clientes = new List<Models.Cliente>(),
                    TP_Especial = Desc_Pedido,
                    Origem = descOrigem
                };

                
                PedidoData pedidoD = new PedidoData();
                PedidoEntity pedidoE = new PedidoEntity();
                //DataTable dt = new DataTable();
                preenchimentoSolicitacaoPecas.TX_OBS = "";
                pedidoE.ID_PEDIDO = ID_PEDIDO;
                DataTableReader dtr = pedidoD.ObterLista(pedidoE).CreateDataReader();
                if (dtr.HasRows)
                {
                    while (dtr.Read())
                    {
                        preenchimentoSolicitacaoPecas.TX_OBS = Convert.ToString(dtr["TX_OBS"]).ToString();
                    }
                }

                if (preenchimentoSolicitacaoPecas.TP_TIPO_PEDIDO == "A")
                {
                    var tecnico = new TecnicoData().ObterTecnico(CD_TECNICO).FirstOrDefault();

                    if (tecnico != null)
                        preenchimentoSolicitacaoPecas.Telefone = tecnico.TX_TELEFONE;

                }

                switch (TP_TIPO_PEDIDO)
                {
                    case "T":
                        preenchimentoSolicitacaoPecas.TituloPagina = "Detalhes do pedido de reposição de peças para Técnico";
                        preenchimentoSolicitacaoPecas.DS_TP_TIPO_PEDIDO = ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Key;
                        preenchimentoSolicitacaoPecas.pedidoPecaTecnico = new Models.PedidoPecaTecnico();
                        preenchimentoSolicitacaoPecas.pedidoPecaTecnico.listaPecas = new List<PecaEntity>();
                        preenchimentoSolicitacaoPecas.pedidoPecaTecnico.pedidoPeca.Estoque3M1 = listaEstoque.Where(e => e.TP_ESTOQUE_TEC_3M == "3M1").FirstOrDefault();
                        preenchimentoSolicitacaoPecas.pedidoPecaTecnico.pedidoPeca.Estoque3M2 = listaEstoque.Where(e => e.TP_ESTOQUE_TEC_3M == "3M2").FirstOrDefault();
                        
                        break;

                    case "C":
                        preenchimentoSolicitacaoPecas.TituloPagina = "Detalhes do pedido de envio de peças para Cliente";
                        preenchimentoSolicitacaoPecas.DS_TP_TIPO_PEDIDO = ControlesUtility.Dicionarios.TipoPedido().ToArray()[2].Key;
                        preenchimentoSolicitacaoPecas.pedidoPecaCliente = new Models.PedidoPecaCliente();
                        preenchimentoSolicitacaoPecas.pedidoPecaCliente.listaPecas = new List<PecaEntity>();
                        preenchimentoSolicitacaoPecas.pedidoPecaCliente.pedidoPeca = new Models.PedidoPeca();
                        preenchimentoSolicitacaoPecas.pedidoPecaCliente.pedidoPeca.Estoque3M1 = listaEstoque.Where(e => e.TP_ESTOQUE_TEC_3M == "3M1").FirstOrDefault();
                        preenchimentoSolicitacaoPecas.pedidoPecaCliente.pedidoPeca.Estoque3M2 = listaEstoque.Where(e => e.TP_ESTOQUE_TEC_3M == "3M2").FirstOrDefault();
                        
                        break;

                    case "A":
                        preenchimentoSolicitacaoPecas.TituloPagina = "Detalhes do pedido de peças avulso (para Cliente ou Técnico)";
                        preenchimentoSolicitacaoPecas.DS_TP_TIPO_PEDIDO = ControlesUtility.Dicionarios.TipoPedido().ToArray()[1].Key;
                        preenchimentoSolicitacaoPecas.pedidoPecaAvulso = new Models.PedidoPecaAvulso();
                        preenchimentoSolicitacaoPecas.pedidoPecaAvulso.listaPecas = new List<PecaEntity>();
                        preenchimentoSolicitacaoPecas.pedidoPecaAvulso.pedidoPeca = new Models.PedidoPeca();
                        preenchimentoSolicitacaoPecas.pedidoPecaAvulso.pedidoPeca.Estoque3M1 = listaEstoque.Where(e => e.TP_ESTOQUE_TEC_3M == "3M1").FirstOrDefault();
                        preenchimentoSolicitacaoPecas.pedidoPecaAvulso.pedidoPeca.Estoque3M2 = listaEstoque.Where(e => e.TP_ESTOQUE_TEC_3M == "3M2").FirstOrDefault();
                        
                        break;
                }

                LotesData lotesData = new LotesData();
                //preenchimentoSolicitacaoPecas.lotes = pecaData.obterLotes(ID_PEDIDO);

                DataTableReader dataTableReader = lotesData.obterLotes(ID_PEDIDO).CreateDataReader();

                LotesEntity lotesEntity = new LotesEntity();
                List<Models.Lote> lotes = new List<Models.Lote>();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Lote lote = new Models.Lote
                        {
                            ID_LOTE_APROVACAO = Convert.ToInt64(dataTableReader["ID_LOTE_APROVACAO"]),
                            DS_ARQUIVO = dataTableReader["DS_ANEXO_NF_GUID"].ToString()
                        };
                        lotes.Add(lote);
                    }
                }

                ViewBag.PermitirAnexar = (
                    CurrentUser.perfil.nidPerfil == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M)
                    || CurrentUser.perfil.nidPerfil == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.AssistênciaTecnica3M))
                    //&& (tipoOrigemPagina == "Aprovacao" || tipoOrigemPagina == "Confirmacao") ? "S" : "N";
                    && (
                    //preenchimentoSolicitacaoPecas.statusPedidoAtual.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Aprovado)
                    //|| preenchimentoSolicitacaoPecas.statusPedidoAtual.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Pendente)
                    //|| preenchimentoSolicitacaoPecas.statusPedidoAtual.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Recebido)
                    //|| preenchimentoSolicitacaoPecas.statusPedidoAtual.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia)
                    //|| Convert.ToInt64(statusPedido) == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Aprovado)
                    //|| Convert.ToInt64(statusPedido) == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Pendente)
                    //|| Convert.ToInt64(statusPedido) == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Recebido)
                    //|| Convert.ToInt64(statusPedido) == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia)
                    lotes.Count > 0
                    ) ? "S" : "N";

                ViewBag.PermitirLoteDownload =
                    (
                    Convert.ToInt64(statusPedido) != Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.NovoRascunho)
                    && Convert.ToInt64(statusPedido) != Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Solicitado)
                    && Convert.ToInt64(statusPedido) != 0
                    ) ? "S" : "N";

                preenchimentoSolicitacaoPecas.listaLotes = lotes;

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (preenchimentoSolicitacaoPecas == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(preenchimentoSolicitacaoPecas);
            }
        }

        [_3MAuthentication]
        public ActionResult EditarEnvioBPCS(string idKey)
        {
            Models.PreenchimentoSolicitacaoPecas preenchimentoSolicitacaoPecas = null;

            try
            {
                string IDKey = idKey.Replace(" ", "+");

                string[] conteudo = null;
                try
                {
                    conteudo = ControlesUtility.Criptografia.Descriptografar(IDKey).Split('|');
                }
                catch { }

                if (conteudo == null)
                {
                    if (IDKey.Substring(IDKey.Length - 1, 1) != "=")
                    {
                        IDKey += "=";
                    }

                    conteudo = ControlesUtility.Criptografia.Descriptografar(IDKey).Split('|');
                }

                ViewBag.URLParam = IDKey;

                //string[] conteudo = ControlesUtility.Criptografia.Descriptografar(idKey).Split('|');
                conteudo = ControlesUtility.Criptografia.Descriptografar(IDKey).Split('|');
                string CD_TECNICO = conteudo[0];
                Int64 ID_PEDIDO = Convert.ToInt64(conteudo[1]);
                string TP_TIPO_PEDIDO = conteudo[2];
                string tipoOrigemPagina = conteudo[3];
                string statusPedido = conteudo[4];
                string tipoPedido = "";
                if (conteudo.Length > 5)
                    tipoPedido = conteudo[5];

                if (statusPedido == "")
                {
                    statusPedido = "0";
                }

                string Desc_Pedido = "";

                if (tipoPedido == "E")
                {
                    Desc_Pedido = "Especial";
                }
                else if (tipoPedido == "N")
                {
                    Desc_Pedido = "Normal";
                }
                else
                {
                    Desc_Pedido = "";
                }
                if (statusPedido == "")
                {
                    statusPedido = "0";
                }



                EstoqueData estoqueData = new EstoqueData();
                DataTable dataTable = estoqueData.ObterLista(new EstoqueEntity() { FL_ATIVO = "S", TP_ESTOQUE_TEC_3M = "3M%" });
                var listaEstoque = from row in dataTable.Rows.Cast<DataRow>()
                                   select new EstoqueEntity()
                                   {
                                       ID_ESTOQUE = row.FieldOrDefault<long>("ID_ESTOQUE"),
                                       CD_ESTOQUE = row["CD_ESTOQUE"].ToString(),
                                       DS_ESTOQUE = row["DS_ESTOQUE"].ToString().Trim(),
                                       TP_ESTOQUE_TEC_3M = row["TP_ESTOQUE_TEC_3M"].ToString().Trim()
                                   };

                preenchimentoSolicitacaoPecas = new Models.PreenchimentoSolicitacaoPecas
                {
                    //    idKeyInicial = ControlesUtility.Criptografia.Criptografar(ID_AGENDA.ToString() + "|" + ID_VISITA.ToString() + "|" + CD_CLIENTE.ToString() + "|" + CD_TECNICO.ToString() + "|0"),
                    ID_PEDIDO = ID_PEDIDO,
                    tecnico = new TecnicoEntity()
                    {
                        CD_TECNICO = CD_TECNICO
                    },
                    TP_TIPO_PEDIDO = TP_TIPO_PEDIDO,
                    tipoOrigemPagina = tipoOrigemPagina,
                    statusPedidos = new List<Models.StatusPedido>(),
                    clientes = new List<Models.Cliente>(),
                    TP_Especial = Desc_Pedido
                };

                PedidoData pedidoD = new PedidoData();
                PedidoEntity pedidoE = new PedidoEntity();
                //DataTable dt = new DataTable();
                preenchimentoSolicitacaoPecas.TX_OBS = "";
                pedidoE.ID_PEDIDO = ID_PEDIDO;
                DataTableReader dtr = pedidoD.ObterLista(pedidoE).CreateDataReader();
                if (dtr.HasRows)
                {
                    while (dtr.Read())
                    {
                        preenchimentoSolicitacaoPecas.TX_OBS = Convert.ToString(dtr["TX_OBS"]).ToString();
                    }
                }
                switch (TP_TIPO_PEDIDO)
                {
                    case "T":
                        preenchimentoSolicitacaoPecas.TituloPagina = "Detalhes do pedido de reposição de peças para Técnico";
                        preenchimentoSolicitacaoPecas.DS_TP_TIPO_PEDIDO = ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Key;
                        preenchimentoSolicitacaoPecas.pedidoPecaTecnico = new Models.PedidoPecaTecnico();
                        preenchimentoSolicitacaoPecas.pedidoPecaTecnico.listaPecas = new List<PecaEntity>();
                        preenchimentoSolicitacaoPecas.pedidoPecaTecnico.pedidoPeca.Estoque3M1 = listaEstoque.Where(e => e.TP_ESTOQUE_TEC_3M == "3M1").FirstOrDefault();
                        preenchimentoSolicitacaoPecas.pedidoPecaTecnico.pedidoPeca.Estoque3M2 = listaEstoque.Where(e => e.TP_ESTOQUE_TEC_3M == "3M2").FirstOrDefault();

                        break;

                    case "C":
                        preenchimentoSolicitacaoPecas.TituloPagina = "Detalhes do pedido de envio de peças para Cliente";
                        preenchimentoSolicitacaoPecas.DS_TP_TIPO_PEDIDO = ControlesUtility.Dicionarios.TipoPedido().ToArray()[2].Key;
                        preenchimentoSolicitacaoPecas.pedidoPecaCliente = new Models.PedidoPecaCliente();
                        preenchimentoSolicitacaoPecas.pedidoPecaCliente.listaPecas = new List<PecaEntity>();
                        preenchimentoSolicitacaoPecas.pedidoPecaCliente.pedidoPeca = new Models.PedidoPeca();
                        preenchimentoSolicitacaoPecas.pedidoPecaCliente.pedidoPeca.Estoque3M1 = listaEstoque.Where(e => e.TP_ESTOQUE_TEC_3M == "3M1").FirstOrDefault();
                        preenchimentoSolicitacaoPecas.pedidoPecaCliente.pedidoPeca.Estoque3M2 = listaEstoque.Where(e => e.TP_ESTOQUE_TEC_3M == "3M2").FirstOrDefault();

                        break;

                    case "A":
                        preenchimentoSolicitacaoPecas.TituloPagina = "Detalhes do pedido de peças avulso (para Cliente ou Técnico)";
                        preenchimentoSolicitacaoPecas.DS_TP_TIPO_PEDIDO = ControlesUtility.Dicionarios.TipoPedido().ToArray()[1].Key;
                        preenchimentoSolicitacaoPecas.pedidoPecaAvulso = new Models.PedidoPecaAvulso();
                        preenchimentoSolicitacaoPecas.pedidoPecaAvulso.listaPecas = new List<PecaEntity>();
                        preenchimentoSolicitacaoPecas.pedidoPecaAvulso.pedidoPeca = new Models.PedidoPeca();
                        preenchimentoSolicitacaoPecas.pedidoPecaAvulso.pedidoPeca.Estoque3M1 = listaEstoque.Where(e => e.TP_ESTOQUE_TEC_3M == "3M1").FirstOrDefault();
                        preenchimentoSolicitacaoPecas.pedidoPecaAvulso.pedidoPeca.Estoque3M2 = listaEstoque.Where(e => e.TP_ESTOQUE_TEC_3M == "3M2").FirstOrDefault();

                        break;
                }

                LotesData lotesData = new LotesData();
                //preenchimentoSolicitacaoPecas.lotes = pecaData.obterLotes(ID_PEDIDO);

                DataTableReader dataTableReader = lotesData.obterLotes(ID_PEDIDO).CreateDataReader();

                LotesEntity lotesEntity = new LotesEntity();
                List<Models.Lote> lotes = new List<Models.Lote>();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Lote lote = new Models.Lote
                        {
                            ID_LOTE_APROVACAO = Convert.ToInt64(dataTableReader["ID_LOTE_APROVACAO"]),
                            DS_ARQUIVO = dataTableReader["DS_ANEXO_NF_GUID"].ToString()
                        };
                        lotes.Add(lote);
                    }
                }

                ViewBag.PermitirAnexar = "N";

                ViewBag.PermitirLoteDownload = "N";

                preenchimentoSolicitacaoPecas.listaLotes = lotes;

                preenchimentoSolicitacaoPecas.statusPedido.ID_STATUS_PEDIDO = Convert.ToInt32(statusPedido);

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (preenchimentoSolicitacaoPecas == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(preenchimentoSolicitacaoPecas);
            }
        }
        

        [_3MAuthentication]
        public ActionResult AprovaSolicitacaoPecas()
        {
            Models.SolicitacaoPecas solicitacaoPecas = new Models.SolicitacaoPecas
            {
                DT_CRIACAO_INICIO = DateTime.Now.AddDays(-90).ToString("dd/MM/yyyy"),
                DT_CRIACAO_FIM = DateTime.Now.ToString("dd/MM/yyyy"),
                clientes = new List<Models.Cliente>(),
                tecnicos = new List<Models.Tecnico>(),
                pedidos = new List<Models.Pedido>(),
                statusPedidos = new List<Models.StatusPedido>()
                //tiposStatusVisitaOS = new List<Models.TpStatusVisitaOS>(),
            };

            return View(solicitacaoPecas);
        }

        public ActionResult EnviaSolicitacaodePecaBCPS()
        {
            Models.SolicitacaoPecas solicitacaoPecas = new Models.SolicitacaoPecas
            {
                DT_CRIACAO_INICIO = DateTime.Now.AddDays(-90).ToString("dd/MM/yyyy"),
                DT_CRIACAO_FIM = DateTime.Now.ToString("dd/MM/yyyy"),
                clientes = new List<Models.Cliente>(),
                tecnicos = new List<Models.Tecnico>(),
                pedidos = new List<Models.Pedido>(),
                statusPedidos = new List<Models.StatusPedido>()
                //tiposStatusVisitaOS = new List<Models.TpStatusVisitaOS>(),
            };

            return View(solicitacaoPecas);
        }

        [_3MAuthentication]
        public ActionResult ConfirmaRecebimentoPecas()
        {
            Models.SolicitacaoPecas solicitacaoPecas = new Models.SolicitacaoPecas
            {
                DT_CRIACAO_INICIO = DateTime.Now.AddDays(-90).ToString("dd/MM/yyyy"),
                DT_CRIACAO_FIM = DateTime.Now.ToString("dd/MM/yyyy"),
                clientes = new List<Models.Cliente>(),
                tecnicos = new List<Models.Tecnico>(),
                pedidos = new List<Models.Pedido>(),
                statusPedidos = new List<Models.StatusPedido>()
                //tiposStatusVisitaOS = new List<Models.TpStatusVisitaOS>(),
            };

            return View(solicitacaoPecas);
        }

        public JsonResult ObterListaSolicitacaoJson(Int32 CD_CLIENTE, string CD_TECNICO, Int64 CD_PEDIDO, Int64 ID_STATUS_PEDIDO, Int64 ID_STATUS_PEDIDO_RECEBIDO_PENDENCIA, string DT_CRIACAO_INICIO, string DT_CRIACAO_FIM)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaSolicitacaoPecas> listaSolicitacaoPecas = ObterListaSolicitacao(CD_CLIENTE, CD_TECNICO, CD_PEDIDO, ID_STATUS_PEDIDO, 0, DT_CRIACAO_INICIO, DT_CRIACAO_FIM, "Solicitacao");

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/SolicitacaoPecas/_gridMVC.cshtml", listaSolicitacaoPecas));
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

        public JsonResult ObterListaSolicitacaoBPCSJson(Int32 CD_CLIENTE, string CD_TECNICO, Int64 CD_PEDIDO, Int64 ID_STATUS_PEDIDO, Int64 ID_STATUS_PEDIDO_RECEBIDO_PENDENCIA, string DT_CRIACAO_INICIO, string DT_CRIACAO_FIM)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaSolicitacaoPecas> listaSolicitacaoPecas = ObterListaSolicitacao(CD_CLIENTE, CD_TECNICO, CD_PEDIDO, 8, 0, DT_CRIACAO_INICIO, DT_CRIACAO_FIM, "Solicitacao");

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/SolicitacaoPecas/_gridMVCBPCS.cshtml", listaSolicitacaoPecas));
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

        

        public JsonResult ObterListaSolicitacaoAprovaJson(Int32 CD_CLIENTE, string CD_TECNICO, Int64 CD_PEDIDO, Int64 ID_STATUS_PEDIDO, Int64 ID_STATUS_PEDIDO_RECEBIDO_PENDENCIA, string DT_CRIACAO_INICIO, string DT_CRIACAO_FIM)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaSolicitacaoPecas> listaSolicitacaoPecas = ObterListaSolicitacao(CD_CLIENTE, CD_TECNICO, CD_PEDIDO, ID_STATUS_PEDIDO, 5, DT_CRIACAO_INICIO, DT_CRIACAO_FIM, "Aprovacao");

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/SolicitacaoPecas/_gridMVCAprovaSolicitacaoPecas.cshtml", listaSolicitacaoPecas));
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

        public JsonResult ObterListaSolicitacaoRecebeJson(Int32 CD_CLIENTE, string CD_TECNICO, Int64 CD_PEDIDO, Int64 ID_STATUS_PEDIDO, Int64 ID_STATUS_PEDIDO_RECEBIDO_PENDENCIA, string DT_CRIACAO_INICIO, string DT_CRIACAO_FIM)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaSolicitacaoPecas> listaSolicitacaoPecas = ObterListaSolicitacao(CD_CLIENTE, CD_TECNICO, CD_PEDIDO, ID_STATUS_PEDIDO, ID_STATUS_PEDIDO_RECEBIDO_PENDENCIA, DT_CRIACAO_INICIO, DT_CRIACAO_FIM, "Confirmacao");

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/SolicitacaoPecas/_gridMVCConfirmaSolicitacaoPecas.cshtml", listaSolicitacaoPecas));
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

        protected List<Models.ListaSolicitacaoPecas> ObterListaSolicitacao(Int32 CD_CLIENTE, string CD_TECNICO, Int64 CD_PEDIDO, Int64 ID_STATUS_PEDIDO, Int64 ID_STATUS_PEDIDO_ADICIONAL, string DT_CRIACAO_INICIO, string DT_CRIACAO_FIM, string tipoOrigemPagina)
        {
            List<Models.ListaSolicitacaoPecas> listaSolicitacoesPecas = new List<Models.ListaSolicitacaoPecas>();
            Decimal Valor_Total_Pedidos = 0;

            try
            {
                PedidoEntity pedidoEntity = new PedidoEntity();
                pedidoEntity.cliente.CD_CLIENTE = CD_CLIENTE;
                pedidoEntity.tecnico.CD_TECNICO = CD_TECNICO;
                pedidoEntity.CD_PEDIDO = CD_PEDIDO;
                pedidoEntity.statusPedido.ID_STATUS_PEDIDO = ID_STATUS_PEDIDO;
                DateTime? DT_INICIO = null;
                DateTime? DT_FIM = null;

                if (!String.IsNullOrEmpty(DT_CRIACAO_INICIO))
                {
                    DT_INICIO = Convert.ToDateTime(DT_CRIACAO_INICIO);
                }

                if (!String.IsNullOrEmpty(DT_CRIACAO_FIM))
                {
                    DT_FIM = Convert.ToDateTime(DT_CRIACAO_FIM + " 23:59:59");
                }

                DataTableReader dataTableReader = new PedidoData().ObterListaSolicitacao(pedidoEntity, ID_STATUS_PEDIDO_ADICIONAL, DT_INICIO, DT_FIM).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        try
                        {
                            Models.ListaSolicitacaoPecas listaSolicitacaoPecas = new Models.ListaSolicitacaoPecas();

                            listaSolicitacaoPecas.idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_TECNICO"].ToString() + "|" + Convert.ToInt64(dataTableReader["ID_PEDIDO"]).ToString() + "|" + dataTableReader["TP_TIPO_PEDIDO"].ToString() + "|" + tipoOrigemPagina + "|" + Convert.ToInt32(dataTableReader["ID_STATUS_PEDIDO"]) + "|" + dataTableReader["TP_Especial"].ToString());

                            listaSolicitacaoPecas.ID_PEDIDO = Convert.ToInt64(dataTableReader["ID_PEDIDO"]);
                            listaSolicitacaoPecas.CD_PEDIDO = Convert.ToInt64(dataTableReader["CD_PEDIDO"]);
                            listaSolicitacaoPecas.CD_PEDIDO_Formatado = Convert.ToInt64(dataTableReader["CD_PEDIDO"]).ToString("000000");
                            listaSolicitacaoPecas.DT_CRIACAO = Convert.ToDateTime(dataTableReader["DT_CRIACAO"]).ToString("dd/MM/yyyy HH:mm");
                            if (dataTableReader["DT_Aprovacao"] != DBNull.Value)
                                listaSolicitacaoPecas.DT_Aprovacao = Convert.ToDateTime(dataTableReader["DT_Aprovacao"]).ToString("dd/MM/yyyy HH:mm");
                            else
                                listaSolicitacaoPecas.DT_Aprovacao = "";
                            listaSolicitacaoPecas.statusPedido = new StatusPedidoEntity();
                            listaSolicitacaoPecas.statusPedido.ID_STATUS_PEDIDO = Convert.ToInt64(dataTableReader["ID_STATUS_PEDIDO"]);
                            listaSolicitacaoPecas.statusPedido.DS_STATUS_PEDIDO = dataTableReader["DS_STATUS_PEDIDO"].ToString();
                            listaSolicitacaoPecas.QTD_SOLICITADA = Convert.ToInt64(dataTableReader["QTD_SOLICITADA"]);

                            listaSolicitacaoPecas.FL_EMERGENCIA = dataTableReader["FL_EMERGENCIA"].ToString();

                            listaSolicitacaoPecas.tecnico = new TecnicoEntity();
                            listaSolicitacaoPecas.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
                            listaSolicitacaoPecas.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();

                            listaSolicitacaoPecas.tecnico.empresa = new EmpresaEntity();
                            listaSolicitacaoPecas.tecnico.empresa.CD_Empresa = dataTableReader["CD_Empresa"] == DBNull.Value ? 0 : Convert.ToInt64(dataTableReader["CD_Empresa"]);
                            listaSolicitacaoPecas.tecnico.empresa.NM_Empresa = dataTableReader["NM_Empresa"] == DBNull.Value ? "" : dataTableReader["NM_Empresa"].ToString();

                            listaSolicitacaoPecas.TP_TIPO_PEDIDO = dataTableReader["TP_TIPO_PEDIDO"].ToString();
                            listaSolicitacaoPecas.DS_TP_TIPO_PEDIDO = ControlesUtility.Dicionarios.TipoPedido().Where(x => x.Value == dataTableReader["TP_TIPO_PEDIDO"].ToString()).ToArray()[0].Key;

                            if (!String.IsNullOrEmpty(dataTableReader["CD_CLIENTE"].ToString()))
                            {
                                listaSolicitacaoPecas.cliente.CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]);
                                listaSolicitacaoPecas.cliente.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + listaSolicitacaoPecas.cliente.CD_CLIENTE + ")";
                            }

                            listaSolicitacaoPecas.TP_Especial = dataTableReader["TP_Especial"].ToString();

                            if (Convert.ToInt64(dataTableReader["ID_STATUS_PEDIDO"]) == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Aprovado) ||
                                Convert.ToInt64(dataTableReader["ID_STATUS_PEDIDO"]) == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Cancelado) ||
                                Convert.ToInt64(dataTableReader["ID_STATUS_PEDIDO"]) == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Recebido) ||
                                Convert.ToInt64(dataTableReader["ID_STATUS_PEDIDO"]) == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia))
                            {
                                listaSolicitacaoPecas.ExibirExcluir = false;
                            }
                            else
                            {
                                listaSolicitacaoPecas.ExibirExcluir = true;
                            }

                            if (tipoOrigemPagina == "Confirmacao")
                            {
                                LotesData lotesData = new LotesData();
                                //preenchimentoSolicitacaoPecas.lotes = pecaData.obterLotes(ID_PEDIDO);

                                DataTableReader dataTableReader2 = lotesData.obterLotes(listaSolicitacaoPecas.ID_PEDIDO).CreateDataReader();

                                LotesEntity lotesEntity = new LotesEntity();
                                List<Models.Lote> lotes = new List<Models.Lote>();

                                if (dataTableReader2.HasRows)
                                {
                                    while (dataTableReader2.Read())
                                    {
                                        try
                                        {
                                            Models.Lote lote = new Models.Lote
                                            {
                                                ID_LOTE_APROVACAO = Convert.ToInt64(dataTableReader2["ID_LOTE_APROVACAO"]),
                                                DS_ARQUIVO = dataTableReader2["DS_ANEXO_NF_GUID"].ToString()
                                            };
                                            lotes.Add(lote);
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        
                                    }
                                }

                                listaSolicitacaoPecas.listaLotes = lotes;

                                if (dataTableReader2 != null)
                                {
                                    dataTableReader2.Dispose();
                                    dataTableReader2 = null;
                                }
                            }
                            PedidoPecaData pedidoPacaData = new PedidoPecaData();
                            IList<PedidoPecaSinc> pecasporPedido = new List<PedidoPecaSinc>();

                            pecasporPedido = new PedidoPecaData().ObterListaPedidoPecaPedidoTotal(listaSolicitacaoPecas.ID_PEDIDO);

                            Decimal Valor_Item = 0;
                            foreach (var pedPeca in pecasporPedido)
                            {
                                Valor_Item += Convert.ToDecimal(Convert.ToDecimal(pedPeca.QTD_SOLICITADA) * Convert.ToDecimal(pedPeca.VL_PECA));
                            }

                            Valor_Total_Pedidos += Valor_Item;
                            ViewBag.Valor_Total_Pedidos = Convert.ToDecimal(Valor_Total_Pedidos).ToString("N2");
                            listaSolicitacoesPecas.Add(listaSolicitacaoPecas);
                        }
                        catch (Exception ex)
                        {

                        }
                        
                        
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

            return listaSolicitacoesPecas;
        }

        //public JsonResult CriptografarChaveJson(string CD_TECNICO, string tipoPedido, string tipoOrigemPagina)
        //{
        //    Dictionary<string, object> jsonResult = new Dictionary<string, object>();

        //    try
        //    {
        //        string idKey = ControlesUtility.Criptografia.Criptografar(CD_TECNICO + "|0|" + tipoPedido + "|" + tipoOrigemPagina);

        //        //string a = System.Web.HttpUtility.HtmlDecode(idKey).ToString();


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

        public JsonResult ObterListaMensagemJson(Int64 ID_PEDIDO)
        {
            //ViewBag.Administrador3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M));
            //ViewBag.Tecnico3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Tecnico3M));

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.Mensagem> listaMensagens = ObterListaMensagem(ID_PEDIDO);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/SolicitacaoPecas/_gridmvcMensagem.cshtml", listaMensagens));
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

        protected List<Models.Mensagem> ObterListaMensagem(Int64 ID_PEDIDO)
        {
            List<Models.Mensagem> listaMensagens = new List<Models.Mensagem>();

            try
            {
                MensagemEntity mensagemEntity = new MensagemEntity();
                mensagemEntity.pedido.ID_PEDIDO = ID_PEDIDO;

                DataTableReader dataTableReader = new MensagemData().ObterLista(mensagemEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Mensagem mensagem = new Models.Mensagem
                        {
                            ID_MENSAGEM = Convert.ToInt64(dataTableReader["ID_MENSAGEM"]),
                            pedido = new PedidoEntity()
                            {
                                ID_PEDIDO = Convert.ToInt64(dataTableReader["ID_PEDIDO"])
                            },
                            DT_OCORRENCIA = Convert.ToDateTime(dataTableReader["DT_OCORRENCIA"]).ToString("dd/MM/yyyy HH:mm"),
                            usuario = new UsuarioEntity()
                            {
                                nidUsuario = Convert.ToInt64(dataTableReader["ID_USUARIO"]),
                                cnmNome = dataTableReader["cnmNome"].ToString()
                            },
                            //DS_MENSAGEM = dataTableReader["DS_MENSAGEM"].ToString(),
                            DS_MENSAGEM = Convert.ToDateTime(dataTableReader["DT_OCORRENCIA"]).ToString("dd/MM/yyyy HH:mm") + " - (" + dataTableReader["cnmNome"].ToString() + "): " + dataTableReader["DS_MENSAGEM"].ToString()
                        };

                        listaMensagens.Add(mensagem);
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

            return listaMensagens;
        }

        public JsonResult ObterListaNotasJson(Int64 ID_PEDIDO)
        {
            //ViewBag.Administrador3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M));
            //ViewBag.Tecnico3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Tecnico3M));

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.PEDIDO_BPCS_NF> listaMensagens = ObterListaNotas(ID_PEDIDO);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/SolicitacaoPecas/_gridmvcNotas.cshtml", listaMensagens));
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

        protected List<Models.PEDIDO_BPCS_NF> ObterListaNotas(Int64 ID_PEDIDO)
        {
            List<Models.PEDIDO_BPCS_NF> listaMensagens = new List<Models.PEDIDO_BPCS_NF>();

            try
            {
                PEDIDO_BPCS_NFEntity mensagemEntity = new PEDIDO_BPCS_NFEntity();
                mensagemEntity.ID_PEDIDO = ID_PEDIDO;

                DataTableReader dataTableReader = new MensagemData().ObterListaNotas(mensagemEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.PEDIDO_BPCS_NF mensagem = new Models.PEDIDO_BPCS_NF
                        {
                            ID_PEDIDO = Convert.ToInt64(dataTableReader["ID_PEDIDO"] is DBNull ? 0 : dataTableReader["ID_PEDIDO"]),
                            ID_ITEM_PEDIDO = Convert.ToInt64(dataTableReader["ID_ITEM_PEDIDO"] is DBNull ? 0 : dataTableReader["ID_ITEM_PEDIDO"]),
                            CD_PECA = dataTableReader["CD_PECA"] is DBNull ? "" : dataTableReader["CD_PECA"].ToString(),
                            NR_CONTROL = Convert.ToInt64(dataTableReader["NR_CONTROL"] is DBNull ? 0 : dataTableReader["NR_CONTROL"]),
                            NR_LINHA = Convert.ToInt32(dataTableReader["NR_LINHA"] is DBNull ? 0 : dataTableReader["NR_LINHA"]),
                            NR_NF = Convert.ToInt64(dataTableReader["NR_NF"] is DBNull ? 0 : dataTableReader["NR_NF"]),
                            NR_SESM = Convert.ToInt64(dataTableReader["NR_SESM"] is DBNull ? 0 : dataTableReader["NR_SESM"])
                        };

                        listaMensagens.Add(mensagem);
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

            return listaMensagens;
        }

        

        public JsonResult ObterListaPedidoItemJson(Int64 ID_PEDIDO, string CD_TECNICO, string TP_TIPO_PEDIDO, string tipoOrigemPagina, int idStatusPedido)
        {
            bool estoqueAbaixoSolicitado = false;
            bool pecaNaoEncontradaEstoque = false;

            //ViewBag.Administrador3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M));
            //ViewBag.Tecnico3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Tecnico3M));

            //ViewBag.ExibirEstoque3M = false;
            //if (tipoOrigemPagina == "Aprovacao" && (
            //    Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M) ||
            //    //Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Tecnico3M) ||
            //    Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.AssistênciaTecnica3M)
            //    ))
            //{
            //    ViewBag.ExibirEstoque3M = true;
            //}

            //20.08.2018 - F4.6.3 - I -Alterações de acordo com item específicado
            //ViewBag.ExibirEstoque3M1 = tipoOrigemPagina == "Aprovacao" && idStatusPedido == ControlesUtility.Enumeradores.StatusPedido.Solicitado.ToInt() &&
            //     (Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M) ||
            //     Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.AssistênciaTecnica3M));
            //20.08.2018 - F4.6.3 - F -Alterações de acordo com item específicado

            ViewBag.ExibirQtdPecaCli = false;

            if (TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[2].Value)
            {
                // Tipo de Pedido para Cliente
                ViewBag.ExibirQtdSugeridaPZ = true;
                ViewBag.ExibirCriticidade = false;
                ViewBag.ExibirQtdPecaCli = true;
            }
            else if (TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Value)
            {
                // Tipo de Pedido para Técnico
                ViewBag.ExibirQtdSugeridaPZ = true;
                ViewBag.ExibirCriticidade = true;
            }

            //if (tipoOrigemPagina == "Aprovacao")
            //    ViewBag.gridMVCStyle_NoTableStriped = true;

            ViewBag.ExibirValorPeca = tipoOrigemPagina == "Aprovacao" &&
                 (Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M) ||
                 Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.AssistênciaTecnica3M));

            //SL00035389
            ViewBag.ExibirExcluirPeca = true;

            if (tipoOrigemPagina == "Aprovacao" || tipoOrigemPagina == "Confirmacao")
            {
                ViewBag.ExibirColunaLote = true;
                //SL00035389
                ViewBag.ExibirExcluirPeca = false;
            }

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            //try
            //{
            List<Models.ListaPedidoPecas> listaPedidoPecas = ObterListaPedidoPeca(ID_PEDIDO, CD_TECNICO, TP_TIPO_PEDIDO, tipoOrigemPagina, ref estoqueAbaixoSolicitado, ref pecaNaoEncontradaEstoque);

            jsonResult.Add("Html", RenderRazorViewToString("~/Views/SolicitacaoPecas/_gridmvcPedidoItem.cshtml", listaPedidoPecas));
            jsonResult.Add("Status", "Success");
            jsonResult.Add("estoqueAbaixoSolicitado", estoqueAbaixoSolicitado == true ? "S" : "N");
            jsonResult.Add("pecaNaoEncontradaEstoque", pecaNaoEncontradaEstoque == true ? "S" : "N");
            //}
            //catch (Exception ex)
            //{
            //    LogUtility.LogarErro(ex);
            //    throw ex;
            //}

            //return Json(jsonResult, JsonRequestBehavior.AllowGet);
            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }

        public JsonResult ObterListaPedidoItemBPCSJson(Int64 ID_PEDIDO, string CD_TECNICO, string TP_TIPO_PEDIDO, string tipoOrigemPagina, int idStatusPedido)
        {
            bool estoqueAbaixoSolicitado = false;
            bool pecaNaoEncontradaEstoque = false;

            
            ViewBag.ExibirQtdPecaCli = false;

            if (TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[2].Value)
            {
                // Tipo de Pedido para Cliente
                ViewBag.ExibirQtdSugeridaPZ = true;
                ViewBag.ExibirCriticidade = false;
                ViewBag.ExibirQtdPecaCli = true;
            }
            else if (TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Value)
            {
                // Tipo de Pedido para Técnico
                ViewBag.ExibirQtdSugeridaPZ = true;
                ViewBag.ExibirCriticidade = true;
            }

            //if (tipoOrigemPagina == "Aprovacao")
            //    ViewBag.gridMVCStyle_NoTableStriped = true;

            ViewBag.ExibirValorPeca = false;

            //SL00035389
            ViewBag.ExibirExcluirPeca = true;

            ViewBag.ExibirColunaLote = true;
                //SL00035389
            ViewBag.ExibirExcluirPeca = false;

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            
            List<Models.ListaPedidoPecas> listaPedidoPecas = ObterListaPedidoPecaBPCS(ID_PEDIDO, CD_TECNICO, TP_TIPO_PEDIDO, tipoOrigemPagina, ref estoqueAbaixoSolicitado, ref pecaNaoEncontradaEstoque);
            listaPedidoPecas = listaPedidoPecas.OrderBy(x => x.CD_ESTOQUE).ToList();
            jsonResult.Add("Html", RenderRazorViewToString("~/Views/SolicitacaoPecas/_gridmvcPedidoItemBPCS.cshtml", listaPedidoPecas));
            jsonResult.Add("Status", "Success");
            jsonResult.Add("estoqueAbaixoSolicitado", estoqueAbaixoSolicitado == true ? "S" : "N");
            jsonResult.Add("pecaNaoEncontradaEstoque", pecaNaoEncontradaEstoque == true ? "S" : "N");
            
            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }

        protected List<Models.ListaPedidoPecas> ObterListaPedidoPeca(Int64 ID_PEDIDO, string CD_TECNICO, string TP_TIPO_PEDIDO, string tipoOrigemPagina, ref bool estoqueAbaixoSolicitado, ref bool pecaNaoEncontradaEstoque)
        {
            List<Models.ListaPedidoPecas> listasPedidoPecas = new List<Models.ListaPedidoPecas>();
            Decimal VL_TOTAL_PECA = 0;
            Decimal VL_TOTAL_PECA_SOLICITADA = 0;
            Decimal VL_TOTAL_PECA_Restante = 0;
            Decimal VL_TOTAL_PECA_CANCELADAS = 0;

            //try
            //{
            DataTable dataTableReader = new PedidoData().ObterListaSolicitacaoPeca(ID_PEDIDO, CD_TECNICO);


            foreach (DataRow row in dataTableReader.Rows)//row.Read())
            {
                string formatadorDecimais = string.Empty;

                //if (row.FieldOrDefault<string>("TX_UNIDADE") == "MT")
                //{
                //    formatadorDecimais = "N3";
                //}
                //else
                //{
                formatadorDecimais = "N0";
                //}

                Models.ListaPedidoPecas listaPedidoPecas = new Models.ListaPedidoPecas();
                listaPedidoPecas.peca = new Models.Peca()
                {
                    CD_PECA = row.FieldOrDefault<string>("CD_PECA"),
                    DS_PECA = row.FieldOrDefault<string>("DS_PECA"),
                    TX_UNIDADE = row.FieldOrDefault<string>("TX_UNIDADE"),
                    DESCRICAO_PECA = row.FieldOrDefault<string>("DESCRICAO_PECA"),
                    TP_PECA = row.FieldOrDefault<string>("TP_PECA")

                    //VL_PECA = row["VL_PECA"].ToString(),
                };
                if (listaPedidoPecas.peca.DS_PECA == null || listaPedidoPecas.peca.DS_PECA == "")
                {
                    listaPedidoPecas.peca.DS_PECA = listaPedidoPecas.peca.DESCRICAO_PECA;
                }

                if (listaPedidoPecas.peca.TP_PECA == "E" || listaPedidoPecas.peca.TP_PECA == "R")
                {
                    listaPedidoPecas.peca.DS_PECA = listaPedidoPecas.peca.DS_PECA.ToUpper() + " - " + listaPedidoPecas.peca.CD_PECA.ToUpper();
                }
                listaPedidoPecas.peca.VL_PECA = row["VL_PECA"].ToString();
                listaPedidoPecas.EnviaBPCS = row["EnviaBPCS"].ToString();

                listaPedidoPecas.planoZero = new Models.PlanoZero()
                {
                    ccdCriticidadeAbc = string.Empty, //dataTableReader["ccdCriticidadeAbc"].ToString(),
                    nqtPecaModelo = "0",
                };
                listaPedidoPecas.estoqueMovimentacao = new Models.EstoqueMovimentacao();
                listaPedidoPecas.estoquePeca = new Models.EstoquePeca();
                listaPedidoPecas.estoquePeca3M = new Models.EstoquePeca();
                listaPedidoPecas.estoquePeca3M2 = new Models.EstoquePeca();
                listaPedidoPecas.pedidoPeca = new Models.PedidoPeca()
                {
                    QTD_SOLICITADA = row.FieldOrDefault<decimal>("QTD_SOLICITADA").ToString(formatadorDecimais),
                    DS_ST_STATUS_ITEM = ControlesUtility.Dicionarios.StatusItem().Where(x => x.Value == row.FieldOrDefault<string>("ST_STATUS_ITEM")).ToArray()[0].Key,
                    ID_ITEM_PEDIDO = Convert.ToInt64(row.FieldOrDefault<Decimal>("ID_ITEM_PEDIDO")),
                };
                listaPedidoPecas.statusPedido = new StatusPedidoEntity();
                listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO = row.FieldOrDefault<long>("ID_STATUS_PEDIDO");
                //tipoOrigemPagina = tipoOrigemPagina

                if (row["QT_PECA_ATUAL"] != DBNull.Value)
                {
                    listaPedidoPecas.estoquePeca.QT_PECA_ATUAL = Convert.ToDecimal(Convert.ToString(row["QT_PECA_ATUAL"])).ToString(formatadorDecimais);
                    listaPedidoPecas.estoquePeca.QT_PECA = Convert.ToInt32(row["QT_PECA_ATUAL"]);
                }
                else
                {
                    listaPedidoPecas.estoquePeca.QT_PECA_ATUAL = "0";
                    listaPedidoPecas.estoquePeca.QT_PECA = 0;
                }

                if (TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[2].Value)
                {
                    // Tipo de Pedido para Cliente
                    listaPedidoPecas.planoZero.qtPecaModelo = "0";

                    if (row["QT_PECA_ATUAL_CLIENTE"] != DBNull.Value)
                    {
                        listaPedidoPecas.estoquePecaCLI.QT_PECA_ATUAL = Convert.ToDecimal(Convert.ToString(row["QT_PECA_ATUAL_CLIENTE"])).ToString(formatadorDecimais);
                        listaPedidoPecas.estoquePecaCLI.QT_PECA = Convert.ToInt32(row["QT_PECA_ATUAL_CLIENTE"]);
                    }
                    else
                    {
                        listaPedidoPecas.estoquePecaCLI.QT_PECA_ATUAL = "0";
                        listaPedidoPecas.estoquePecaCLI.QT_PECA = 0;
                    }

                }
                else if (TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Value)
                {
                    if (row["QT_SUGERIDA_PZ"] != DBNull.Value)
                    {
                        // Tipo de Pedido para Técnico
                        listaPedidoPecas.planoZero.qtPecaModelo = Convert.ToDecimal(row["QT_SUGERIDA_PZ"].ToString()).ToString(formatadorDecimais);
                        listaPedidoPecas.planoZero.ccdCriticidadeAbc = row.FieldOrDefault<string>("CD_CRITICIDADE_ABC");

                        if (String.IsNullOrEmpty(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA) || listaPedidoPecas.pedidoPeca.QTD_SOLICITADA == "0")
                        {
                            //listaPedidoPecas.pedidoPeca.QTD_SOLICITADA = Convert.ToDecimal(row["QT_SUGERIDA_PZ"].ToString()).ToString(formatadorDecimais);
                            int resSolicitado = Convert.ToInt32(listaPedidoPecas.planoZero.qtPecaModelo) - listaPedidoPecas.estoquePeca.QT_PECA;
                            if (resSolicitado < 1)
                            {
                                resSolicitado = 0;
                            }

                            listaPedidoPecas.pedidoPeca.QTD_SOLICITADA = Convert.ToString(resSolicitado);
                        }

                        // Atualiza a nova quantidade no registro de TB_PEDIDO_PECA
                        //new PedidoPecaData().AlterarQtdeSolicitada(listaPedidoPecas.pedidoPeca.ID_ITEM_PEDIDO, Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA), CurrentUser.usuario.nidUsuario);
                    }
                    else
                    {
                        listaPedidoPecas.planoZero.qtPecaModelo = "0";
                        listaPedidoPecas.planoZero.ccdCriticidadeAbc = "-";
                    }
                }

                if (row["QT_PECA_ATUAL_3M"] != DBNull.Value)
                {
                    listaPedidoPecas.estoquePeca3M.QT_PECA_ATUAL = Convert.ToInt32(row["QT_PECA_ATUAL_3M"]).ToString(); //Convert.ToDecimal(row["QT_PECA_ATUAL_3M"]).ToString(formatadorDecimais);
                    listaPedidoPecas.estoquePeca3M.QT_PECA = Convert.ToInt32(row["QT_PECA_ATUAL_3M"]);
                }
                else
                {
                    listaPedidoPecas.estoquePeca3M.QT_PECA_ATUAL = "0";
                    listaPedidoPecas.estoquePeca3M.QT_PECA = 0;
                    listaPedidoPecas.estoquePeca3M = listaPedidoPecas.estoquePeca;
                }

                if (row["QT_PECA_ATUAL_3M2"] != DBNull.Value)
                {
                    listaPedidoPecas.estoquePeca3M2.QT_PECA_ATUAL = Convert.ToInt32(row["QT_PECA_ATUAL_3M2"]).ToString(); //Convert.ToDecimal(row["QT_PECA_ATUAL_3M2"]).ToString(formatadorDecimais);
                    listaPedidoPecas.estoquePeca3M2.QT_PECA = Convert.ToInt32(row["QT_PECA_ATUAL_3M2"]);
                }
                else
                {
                    listaPedidoPecas.estoquePeca3M2.QT_PECA_ATUAL = "0";
                    listaPedidoPecas.estoquePeca3M2.QT_PECA = 0;
                    //listaPedidoPecas.estoquePeca3M2 = listaPedidoPecas.estoquePeca;
                }

                if (row["DT_MOVIMENTACAO"] != DBNull.Value)
                {
                    listaPedidoPecas.estoqueMovimentacao.DT_MOVIMENTACAO = Convert.ToDateTime(row["DT_MOVIMENTACAO"]).ToString("dd/MM/yyyy");
                }

                if (row["QTD_APROVADA"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA = Convert.ToInt32(row["QTD_APROVADA"]).ToString(); //Convert.ToDecimal(row["QTD_APROVADA"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA = "0";
                }

                if (row["Duplicado"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.Duplicado = row["Duplicado"].ToString(); //Convert.ToDecimal(row["QTD_APROVADA"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.Duplicado = "N";
                }

                if (row["QTD_APROVADA_3M1"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1 = Convert.ToInt32(row["QTD_APROVADA_3M1"]).ToString(); //Convert.ToDecimal(row["QTD_APROVADA_3M1"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1 = "0";
                }
                if (row["QTD_APROVADA_3M2"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M2 = Convert.ToInt32(row["QTD_APROVADA_3M2"]).ToString(); //Convert.ToDecimal(row["QTD_APROVADA_3M2"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M2 = "0";
                }
                if (row["CD_PECA_REFERENCIA"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA = row["CD_PECA_REFERENCIA"].ToString();
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA = "";
                }

                if (listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA != "")
                {
                    var pecaEntity = new PecaData().ObterPecas(listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA).FirstOrDefault();
                    if (pecaEntity != null)
                        listaPedidoPecas.peca.DS_PECA = pecaEntity.DS_PECA + "(Ref.: " + listaPedidoPecas.peca.CD_PECA + ")";
                    else
                        listaPedidoPecas.peca.DS_PECA = listaPedidoPecas.peca.DS_PECA + "(Ref.: " + listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA + ")";
                }
                //
                if (tipoOrigemPagina != "Confirmacao")
                {
                    if (listaPedidoPecas.EnviaBPCS == "S")
                    {
                        if (Convert.ToInt32(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1) == 0 && listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA != "")
                        {
                            listaPedidoPecas.pedidoPeca.QTD_APROVADA = listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M2;
                        }
                        else if (Convert.ToInt32(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1) == 0 && listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA == "")
                        {
                            listaPedidoPecas.pedidoPeca.QTD_APROVADA = listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1;
                        }
                        else if (Convert.ToInt32(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1) > 0)
                        {
                            listaPedidoPecas.pedidoPeca.QTD_APROVADA = listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1;
                        }
                    }
                    
                }
                
                if (row["QTD_RECEBIDA"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.QTD_RECEBIDA = Convert.ToInt32(row["QTD_RECEBIDA"]).ToString(); //Convert.ToDecimal(row["QTD_RECEBIDA"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.QTD_RECEBIDA = "0";
                }

                if (row["Estoque_Cli_Aprov"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.Estoque_Cli_Aprov = Convert.ToInt32(row["Estoque_Cli_Aprov"]).ToString(); //Convert.ToDecimal(row["QTD_RECEBIDA"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.Estoque_Cli_Aprov = "0";
                }
                if (row["Estoque_Tec_Aprov"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.Estoque_Tec_Aprov = Convert.ToInt32(row["Estoque_Tec_Aprov"]).ToString(); //Convert.ToDecimal(row["QTD_RECEBIDA"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.Estoque_Tec_Aprov = "0";
                }

                if (listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM == "Aprovado")
                {
                    Decimal QTD = Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1) + Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M2);
                    listaPedidoPecas.pedidoPeca.VL_TOTAL_PECA = (QTD * Convert.ToDecimal(listaPedidoPecas.peca.VL_PECA)).ToString("N2");
                     
                    VL_TOTAL_PECA += Convert.ToDecimal(listaPedidoPecas.pedidoPeca.VL_TOTAL_PECA);
                }

                if (listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM == "Aguardando Envio BPCS")
                {
                    if (listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1 == "0" && listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA == "")
                    {
                        listaPedidoPecas.pedidoPeca.VL_TOTAL_PECA = (0).ToString("N2");

                    }
                    else if(Convert.ToInt64(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1) > 0)
                    {
                        listaPedidoPecas.pedidoPeca.VL_TOTAL_PECA = Convert.ToDecimal(Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1) * Convert.ToDecimal(listaPedidoPecas.peca.VL_PECA)).ToString("N2");
                    }
                    else if (listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1 == "0" && listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA != "")
                    {
                        listaPedidoPecas.pedidoPeca.VL_TOTAL_PECA = Convert.ToDecimal(Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M2) * Convert.ToDecimal(listaPedidoPecas.peca.VL_PECA)).ToString("N2");
                    }

                    VL_TOTAL_PECA += Convert.ToDecimal(listaPedidoPecas.pedidoPeca.VL_TOTAL_PECA);
                }

                if (listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM == "Cancelado")
                {
                    var val = Convert.ToDecimal(Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA) * Convert.ToDecimal(listaPedidoPecas.peca.VL_PECA)).ToString("N2");
                    VL_TOTAL_PECA_CANCELADAS += Convert.ToDecimal(val);
                }

                if (listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM != "Cancelado" && listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM == "Solicitado")
                {
                    var VL = Convert.ToDecimal(Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA) * Convert.ToDecimal(listaPedidoPecas.peca.VL_PECA)).ToString("N2");
                    VL_TOTAL_PECA_Restante += Convert.ToDecimal(VL);
                }


                // INICIO TESTES
                //if ((listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.NovoRascunho) ||
                //    listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Pendente)) &&
                //    tipoOrigemPagina == "Solicitacao" && (
                //    Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M) ||
                //    Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Tecnico3M) ||
                //    Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira) ||
                //    Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica)
                //    ))
                //{
                //    listaPedidoPecas.permiteEditar = true;
                //    listaPedidoPecas.permiteExcluir = true;
                //}
                //else
                //{
                //    listaPedidoPecas.permiteEditar = true;
                //    listaPedidoPecas.permiteExcluir = false;
                //}
                //
                ViewBag.ExibirEstoque3M1 = false;
                ViewBag.ExibirEstoque3M2 = false;
                listaPedidoPecas.permiteEditar = false;
                listaPedidoPecas.permiteExcluir = false;
                listaPedidoPecas.permiteSelecionar = false;

                if (tipoOrigemPagina == "Solicitacao"
                    && listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.NovoRascunho))
                {
                    listaPedidoPecas.permiteEditar = true;
                    listaPedidoPecas.permiteExcluir = true;
                }
                else if (tipoOrigemPagina == "Aprovacao"
                  && (listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Solicitado)
                  || listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Pendente))
                  && Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M))
                {
                    ViewBag.ExibirEstoque3M1 = true;
                    ViewBag.ExibirEstoque3M2 = true;
                    listaPedidoPecas.permiteEditar = true;
                    listaPedidoPecas.permiteExcluir = true;
                    if (listaPedidoPecas.pedidoPeca.Duplicado == "S")
                    {
                        listaPedidoPecas.permiteSelecionar = false;
                        if(listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM != "Cancelado")
                            listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM = "Aguardando Envio BPCS";
                    }
                }
                else if (tipoOrigemPagina == "Confirmacao"
                  && (listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Aprovado)
                  || listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Pendente)
                  || listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia))
                  && Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M))
                {
                    listaPedidoPecas.permiteEditar = true;
                    listaPedidoPecas.permiteExcluir = false;
                    listaPedidoPecas.permiteSelecionar = true;
                }
                else if (tipoOrigemPagina == "Confirmacao"
                  && (listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Aprovado)
                  || listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Pendente)
                  || listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia))
                  && (Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Tecnico3M)
                  || Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira)
                  || Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Cliente)
                  || Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica)
                  ))
                {
                    listaPedidoPecas.permiteEditar = true;
                    listaPedidoPecas.permiteExcluir = false;
                    listaPedidoPecas.permiteSelecionar = true;
                    ViewBag.Tecnico3M = "S";
                }




                if (tipoOrigemPagina == "Aprovacao")
                {
                    if (string.IsNullOrEmpty(listaPedidoPecas.estoquePeca3M.QT_PECA_ATUAL) && string.IsNullOrEmpty(listaPedidoPecas.estoquePeca3M2.QT_PECA_ATUAL) && string.IsNullOrEmpty(listaPedidoPecas.pedidoPeca.QTD_APROVADA))
                    {
                        pecaNaoEncontradaEstoque = true;
                        listaPedidoPecas.cssRegraGRIDAplicar = "text-primary font-weight-bold";
                    }
                    else if (string.IsNullOrEmpty(listaPedidoPecas.pedidoPeca.QTD_APROVADA))
                    {
                        if (
                        (Convert.ToDecimal(listaPedidoPecas.estoquePeca3M.QT_PECA_ATUAL) < Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA)) ||
                            Convert.ToDecimal(listaPedidoPecas.estoquePeca3M2.QT_PECA_ATUAL) < Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA))
                        {
                            estoqueAbaixoSolicitado = true;
                            listaPedidoPecas.cssRegraGRIDAplicar = "text-danger font-weight-bold";
                        }
                    }
                    else
                    {
                        bool estoque3M1Valido;
                        bool estoque3M2Valido;

                        decimal aprovada3M1 = Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1);
                        decimal aprovada3M2 = Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M2);

                        if (TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Value)
                        {
                            //Validação pedido Técnico X Plano Zero
                            decimal quantidadePlanoZero = Convert.ToDecimal(listaPedidoPecas.planoZero.qtPecaModelo);

                            estoque3M1Valido = aprovada3M1 <= quantidadePlanoZero;
                            estoque3M2Valido = aprovada3M2 <= quantidadePlanoZero;
                        }
                        else
                        {
                            estoque3M1Valido = Convert.ToDecimal(listaPedidoPecas.estoquePeca3M.QT_PECA_ATUAL) >= aprovada3M1;
                            estoque3M2Valido = Convert.ToDecimal(listaPedidoPecas.estoquePeca3M2.QT_PECA_ATUAL) >= aprovada3M2;

                            if (!estoque3M1Valido || !estoque3M2Valido)
                            {
                                //Chamado SL00034833
                                if (listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM == "Aguardando")
                                {
                                    estoqueAbaixoSolicitado = true;
                                }
                            }
                        }

                        if (!estoque3M1Valido || !estoque3M2Valido)
                        {
                            listaPedidoPecas.cssRegraGRIDAplicar = "text-danger font-weight-bold";
                        }
                    }
                }

                if (row["ID_LOTE_APROVACAO"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.ID_LOTE_APROVACAO = Convert.ToInt64(row["ID_LOTE_APROVACAO"]);
                }
                if (row["NR_LINHA"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.NR_LINHA = Convert.ToInt64(row["NR_LINHA"]);
                }

                if (row["DS_DIR_FOTO"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.DS_DIR_FOTO = Convert.ToString(row["DS_DIR_FOTO"]);
                }

                if (listaPedidoPecas.planoZero.qtPecaModelo == "0,000" || listaPedidoPecas.planoZero.qtPecaModelo == "0.000")
                {
                    listaPedidoPecas.planoZero.qtPecaModelo = "0";
                }

                if (listaPedidoPecas.pedidoPeca.QTD_SOLICITADA == "0,000" || listaPedidoPecas.pedidoPeca.QTD_SOLICITADA == "0.000")
                {
                    listaPedidoPecas.pedidoPeca.QTD_SOLICITADA = "0";
                }

                if (listaPedidoPecas.pedidoPeca.QTD_APROVADA == "0,000" || listaPedidoPecas.pedidoPeca.QTD_APROVADA == "0.000")
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA = "0";
                }

                if (listaPedidoPecas.pedidoPeca.QTD_RECEBIDA == "0,000" || listaPedidoPecas.pedidoPeca.QTD_RECEBIDA == "0.000")
                {
                    listaPedidoPecas.pedidoPeca.QTD_RECEBIDA = "0";
                }

                if (!String.IsNullOrEmpty(listaPedidoPecas.planoZero.qtPecaModelo))
                    listaPedidoPecas.planoZero.qtPecaModelo.Replace(',', '.');

                if (!String.IsNullOrEmpty(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA))
                    listaPedidoPecas.pedidoPeca.QTD_SOLICITADA.Replace(',', '.');

                listaPedidoPecas.pedidoPeca.VL_TOTAL_PECA_SOLICITADA = Convert.ToDecimal(Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA) * Convert.ToDecimal(listaPedidoPecas.peca.VL_PECA)).ToString("N2");

                if (listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA == "")
                {
                    VL_TOTAL_PECA_SOLICITADA += Convert.ToDecimal(listaPedidoPecas.pedidoPeca.VL_TOTAL_PECA_SOLICITADA);
                }
                

                if(listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.AguardandoEnvioBPCS) || listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.EnviadoBPCS)){
                    listaPedidoPecas.permiteEditar = false;
                    listaPedidoPecas.permiteExcluir = false;
                    listaPedidoPecas.permiteSelecionar = false;
                }

                if (tipoOrigemPagina == "Confirmacao")
                {
                    if (listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA == "")
                    {
                        listasPedidoPecas.Add(listaPedidoPecas);
                    }
                }
                else
                {
                    listasPedidoPecas.Add(listaPedidoPecas);
                }
                
            }


            if (dataTableReader != null)
            {
                dataTableReader.Dispose();
                dataTableReader = null;
            }

            //}
            //catch (Exception ex)
            //{
            //    LogUtility.LogarErro(ex);
            //    throw ex;
            //}

            ViewBag.VL_TOTAL_PECA_SOLICITADA = Convert.ToDecimal(VL_TOTAL_PECA_SOLICITADA).ToString("N2");
            ViewBag.VL_TOTAL_PECA = Convert.ToDecimal(VL_TOTAL_PECA).ToString("N2");
            ViewBag.VL_TOTAL_PECA_Restante = Convert.ToDecimal(VL_TOTAL_PECA_Restante).ToString("N2"); 
            ViewBag.VL_TOTAL_PECA_CANCELADAS = Convert.ToDecimal(VL_TOTAL_PECA_CANCELADAS).ToString("N2");
            return listasPedidoPecas;
        }

        protected List<Models.ListaPedidoPecas> ObterListaPedidoPecaBPCS(Int64 ID_PEDIDO, string CD_TECNICO, string TP_TIPO_PEDIDO, string tipoOrigemPagina, ref bool estoqueAbaixoSolicitado, ref bool pecaNaoEncontradaEstoque)
        {
            List<Models.ListaPedidoPecas> listasPedidoPecas = new List<Models.ListaPedidoPecas>();
            Decimal VL_TOTAL_PECA = 0;
            Decimal VL_TOTAL_PECA_SOLICITADA = 0;
            Decimal VL_TOTAL_PECA_Restante = 0;
            Decimal VL_TOTAL_PECA_CANCELADAS = 0;

            //try
            //{
            DataTable dataTableReader = new PedidoData().ObterListaSolicitacaoPecaBPCS(ID_PEDIDO, CD_TECNICO);


            foreach (DataRow row in dataTableReader.Rows)//row.Read())
            {
                string formatadorDecimais = string.Empty;

                //if (row.FieldOrDefault<string>("TX_UNIDADE") == "MT")
                //{
                //    formatadorDecimais = "N3";
                //}
                //else
                //{
                formatadorDecimais = "N0";
                //}

                Models.ListaPedidoPecas listaPedidoPecas = new Models.ListaPedidoPecas();
                listaPedidoPecas.peca = new Models.Peca()
                {
                    CD_PECA = row.FieldOrDefault<string>("CD_PECA"),
                    DS_PECA = row.FieldOrDefault<string>("DS_PECA"),
                    TX_UNIDADE = row.FieldOrDefault<string>("TX_UNIDADE"),
                    DESCRICAO_PECA = row.FieldOrDefault<string>("DESCRICAO_PECA"),
                    TP_PECA = row.FieldOrDefault<string>("TP_PECA")
                    //VL_PECA = row["VL_PECA"].ToString(),
                };
                listaPedidoPecas.TP_Especial = row["TP_Especial"].ToString();
                if (listaPedidoPecas.peca.DS_PECA == null || listaPedidoPecas.peca.DS_PECA == "")
                {
                    listaPedidoPecas.peca.DS_PECA = listaPedidoPecas.peca.DESCRICAO_PECA;
                }

                if (listaPedidoPecas.peca.TP_PECA == "E" || listaPedidoPecas.peca.TP_PECA == "R")
                {
                    listaPedidoPecas.peca.DS_PECA = listaPedidoPecas.peca.DS_PECA.ToUpper() + " - " + listaPedidoPecas.peca.CD_PECA.ToUpper();
                }

                listaPedidoPecas.peca.VL_PECA = row["VL_PECA"].ToString();

                listaPedidoPecas.planoZero = new Models.PlanoZero()
                {
                    ccdCriticidadeAbc = string.Empty, //dataTableReader["ccdCriticidadeAbc"].ToString(),
                    nqtPecaModelo = "0",
                };
                listaPedidoPecas.estoqueMovimentacao = new Models.EstoqueMovimentacao();
                listaPedidoPecas.estoquePeca = new Models.EstoquePeca();
                listaPedidoPecas.estoquePeca3M = new Models.EstoquePeca();
                listaPedidoPecas.estoquePeca3M2 = new Models.EstoquePeca();
                listaPedidoPecas.pedidoPeca = new Models.PedidoPeca()
                {
                    QTD_SOLICITADA = row.FieldOrDefault<decimal>("QTD_SOLICITADA").ToString(formatadorDecimais),
                    DS_ST_STATUS_ITEM = ControlesUtility.Dicionarios.StatusItem().Where(x => x.Value == row.FieldOrDefault<string>("ST_STATUS_ITEM")).ToArray()[0].Key,
                    ID_ITEM_PEDIDO = Convert.ToInt64(row.FieldOrDefault<Decimal>("ID_ITEM_PEDIDO")),
                   
                };
                listaPedidoPecas.statusPedido = new StatusPedidoEntity();
                listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO = row.FieldOrDefault<long>("ID_STATUS_PEDIDO");
                //tipoOrigemPagina = tipoOrigemPagina

                if (row["QT_PECA_ATUAL"] != DBNull.Value)
                {
                    listaPedidoPecas.estoquePeca.QT_PECA_ATUAL = Convert.ToDecimal(Convert.ToString(row["QT_PECA_ATUAL"])).ToString(formatadorDecimais);
                    listaPedidoPecas.estoquePeca.QT_PECA = Convert.ToInt32(row["QT_PECA_ATUAL"]);
                }
                else
                {
                    listaPedidoPecas.estoquePeca.QT_PECA_ATUAL = "0";
                    listaPedidoPecas.estoquePeca.QT_PECA = 0;
                }

                if (TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[2].Value)
                {
                    // Tipo de Pedido para Cliente
                    listaPedidoPecas.planoZero.qtPecaModelo = "0";

                    if (row["QT_PECA_ATUAL_CLIENTE"] != DBNull.Value)
                    {
                        listaPedidoPecas.estoquePecaCLI.QT_PECA_ATUAL = Convert.ToDecimal(Convert.ToString(row["QT_PECA_ATUAL_CLIENTE"])).ToString(formatadorDecimais);
                        listaPedidoPecas.estoquePecaCLI.QT_PECA = Convert.ToInt32(row["QT_PECA_ATUAL_CLIENTE"]);
                    }
                    else
                    {
                        listaPedidoPecas.estoquePecaCLI.QT_PECA_ATUAL = "0";
                        listaPedidoPecas.estoquePecaCLI.QT_PECA = 0;
                    }

                }
                else if (TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Value)
                {
                    if (row["QT_SUGERIDA_PZ"] != DBNull.Value)
                    {
                        // Tipo de Pedido para Técnico
                        listaPedidoPecas.planoZero.qtPecaModelo = Convert.ToDecimal(row["QT_SUGERIDA_PZ"].ToString()).ToString(formatadorDecimais);
                        listaPedidoPecas.planoZero.ccdCriticidadeAbc = row.FieldOrDefault<string>("CD_CRITICIDADE_ABC");

                        if (String.IsNullOrEmpty(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA) || listaPedidoPecas.pedidoPeca.QTD_SOLICITADA == "0")
                        {
                            //listaPedidoPecas.pedidoPeca.QTD_SOLICITADA = Convert.ToDecimal(row["QT_SUGERIDA_PZ"].ToString()).ToString(formatadorDecimais);
                            int resSolicitado = Convert.ToInt32(listaPedidoPecas.planoZero.qtPecaModelo) - listaPedidoPecas.estoquePeca.QT_PECA;
                            if (resSolicitado < 1)
                            {
                                resSolicitado = 0;
                            }

                            listaPedidoPecas.pedidoPeca.QTD_SOLICITADA = Convert.ToString(resSolicitado);
                        }

                        // Atualiza a nova quantidade no registro de TB_PEDIDO_PECA
                        //new PedidoPecaData().AlterarQtdeSolicitada(listaPedidoPecas.pedidoPeca.ID_ITEM_PEDIDO, Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA), CurrentUser.usuario.nidUsuario);
                    }
                    else
                    {
                        listaPedidoPecas.planoZero.qtPecaModelo = "0";
                        listaPedidoPecas.planoZero.ccdCriticidadeAbc = "-";
                    }
                }

                if (row["QT_PECA_ATUAL_3M"] != DBNull.Value)
                {
                    listaPedidoPecas.estoquePeca3M.QT_PECA_ATUAL = Convert.ToInt32(row["QT_PECA_ATUAL_3M"]).ToString(); //Convert.ToDecimal(row["QT_PECA_ATUAL_3M"]).ToString(formatadorDecimais);
                    listaPedidoPecas.estoquePeca3M.QT_PECA = Convert.ToInt32(row["QT_PECA_ATUAL_3M"]);
                }
                else
                {
                    listaPedidoPecas.estoquePeca3M.QT_PECA_ATUAL = "0";
                    listaPedidoPecas.estoquePeca3M.QT_PECA = 0;
                    listaPedidoPecas.estoquePeca3M = listaPedidoPecas.estoquePeca;
                }

                if (row["QT_PECA_ATUAL_3M2"] != DBNull.Value)
                {
                    listaPedidoPecas.estoquePeca3M2.QT_PECA_ATUAL = Convert.ToInt32(row["QT_PECA_ATUAL_3M2"]).ToString(); //Convert.ToDecimal(row["QT_PECA_ATUAL_3M2"]).ToString(formatadorDecimais);
                    listaPedidoPecas.estoquePeca3M2.QT_PECA = Convert.ToInt32(row["QT_PECA_ATUAL_3M2"]);
                }
                else
                {
                    listaPedidoPecas.estoquePeca3M2.QT_PECA_ATUAL = "0";
                    listaPedidoPecas.estoquePeca3M2.QT_PECA = 0;
                    //listaPedidoPecas.estoquePeca3M2 = listaPedidoPecas.estoquePeca;
                }

                if (row["DT_MOVIMENTACAO"] != DBNull.Value)
                {
                    listaPedidoPecas.estoqueMovimentacao.DT_MOVIMENTACAO = Convert.ToDateTime(row["DT_MOVIMENTACAO"]).ToString("dd/MM/yyyy");
                }

                if (row["QTD_APROVADA"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA = Convert.ToInt32(row["QTD_APROVADA"]).ToString(); //Convert.ToDecimal(row["QTD_APROVADA"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA = "0";
                }



                if (row["QTD_APROVADA_3M1"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1 = Convert.ToInt32(row["QTD_APROVADA_3M1"]).ToString(); //Convert.ToDecimal(row["QTD_APROVADA_3M1"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1 = "0";
                }
                if (row["QTD_APROVADA_3M2"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M2 = Convert.ToInt32(row["QTD_APROVADA_3M2"]).ToString(); //Convert.ToDecimal(row["QTD_APROVADA_3M2"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M2 = "0";
                }
                //

                if (row["CD_PECA_REFERENCIA"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA = row["CD_PECA_REFERENCIA"].ToString();
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA = "";
                }

                if (listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA != "")
                {
                    if (listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA != "")
                    {
                        var pecaEntity = new PecaData().ObterPecas(listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA).FirstOrDefault();
                        if (pecaEntity != null)
                            listaPedidoPecas.peca.DS_PECA = pecaEntity.DS_PECA + "(Ref.: " + listaPedidoPecas.peca.CD_PECA + ")";
                        else
                            listaPedidoPecas.peca.DS_PECA = listaPedidoPecas.peca.DS_PECA + "(Ref.: " + listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA + ")";
                    }
                }
                //
                if (Convert.ToInt32(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1) == 0 && listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA != "")
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA = listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M2;
                }
                else if (Convert.ToInt32(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1) == 0 && listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA == "")
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA = listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1;
                }
                else if (Convert.ToInt32(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1) > 0)
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA = listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1;
                }

                if (listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA == "")
                {
                    listaPedidoPecas.CD_ESTOQUE = "3M1-F4";
                }
                else
                {
                    listaPedidoPecas.CD_ESTOQUE = "3M2-REC";
                }

                if (row["QTD_RECEBIDA"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.QTD_RECEBIDA = Convert.ToInt32(row["QTD_RECEBIDA"]).ToString(); //Convert.ToDecimal(row["QTD_RECEBIDA"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.QTD_RECEBIDA = "0";
                }

                if (row["Estoque_Cli_Aprov"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.Estoque_Cli_Aprov = Convert.ToInt32(row["Estoque_Cli_Aprov"]).ToString(); //Convert.ToDecimal(row["QTD_RECEBIDA"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.Estoque_Cli_Aprov = "0";
                }
                if (row["Estoque_Tec_Aprov"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.Estoque_Tec_Aprov = Convert.ToInt32(row["Estoque_Tec_Aprov"]).ToString(); //Convert.ToDecimal(row["QTD_RECEBIDA"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.Estoque_Tec_Aprov = "0";
                }

                if (listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM == "Aprovado")
                {

                    if (Convert.ToInt32(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1) == 0)
                    {
                        listaPedidoPecas.pedidoPeca.QTD_APROVADA = listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M2;

                        if (listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M2 != null && listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM == "Aprovado")
                        {
                            listaPedidoPecas.pedidoPeca.VL_TOTAL_PECA = Convert.ToDecimal(Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M2) * Convert.ToDecimal(listaPedidoPecas.peca.VL_PECA)).ToString("N2");
                        }

                    }
                    else
                    {
                        listaPedidoPecas.pedidoPeca.QTD_APROVADA = listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1;

                        if (listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1 != null && listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM == "Aprovado")
                        {
                            listaPedidoPecas.pedidoPeca.VL_TOTAL_PECA = Convert.ToDecimal(Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1) * Convert.ToDecimal(listaPedidoPecas.peca.VL_PECA)).ToString("N2");
                        }
                    }

                    VL_TOTAL_PECA += Convert.ToDecimal(listaPedidoPecas.pedidoPeca.VL_TOTAL_PECA);
                }

                if (listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM == "Cancelado")
                {
                    var val = Convert.ToDecimal(Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA) * Convert.ToDecimal(listaPedidoPecas.peca.VL_PECA)).ToString("N2");
                    VL_TOTAL_PECA_CANCELADAS += Convert.ToDecimal(val);
                }

                if (listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM != "Cancelado" && listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM == "Solicitado")
                {
                    var VL = Convert.ToDecimal(Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA) * Convert.ToDecimal(listaPedidoPecas.peca.VL_PECA)).ToString("N2");
                    VL_TOTAL_PECA_Restante += Convert.ToDecimal(VL);
                }

                if (row["InformadoDados"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.InformadoDados = row["InformadoDados"].ToString(); //Convert.ToDecimal(row["QTD_APROVADA_3M2"]).ToString(formatadorDecimais);
                }
                else
                {
                    listaPedidoPecas.pedidoPeca.InformadoDados = "N";
                }

                // INICIO TESTES
                //if ((listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.NovoRascunho) ||
                //    listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Pendente)) &&
                //    tipoOrigemPagina == "Solicitacao" && (
                //    Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M) ||
                //    Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Tecnico3M) ||
                //    Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira) ||
                //    Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica)
                //    ))
                //{
                //    listaPedidoPecas.permiteEditar = true;
                //    listaPedidoPecas.permiteExcluir = true;
                //}
                //else
                //{
                //    listaPedidoPecas.permiteEditar = true;
                //    listaPedidoPecas.permiteExcluir = false;
                //}
                //
                ViewBag.ExibirEstoque3M1 = false;
                ViewBag.ExibirEstoque3M2 = false;
                listaPedidoPecas.permiteEditar = false;
                listaPedidoPecas.permiteExcluir = false;
                listaPedidoPecas.permiteSelecionar = false;

                

                if (tipoOrigemPagina == "Solicitacao"
                    && listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.NovoRascunho))
                {
                    listaPedidoPecas.permiteEditar = true;
                    listaPedidoPecas.permiteSelecionar = true;
                }
                else if (tipoOrigemPagina == "Solicitacao"
                    && listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.AguardandoEnvioBPCS))
                {
                    if (listaPedidoPecas.pedidoPeca.InformadoDados == "N")
                    {
                        listaPedidoPecas.permiteSelecionar = true;
                    }
                    else
                    {
                        listaPedidoPecas.permiteSelecionar = false;
                    }
                }
                else if (tipoOrigemPagina == "Aprovacao"
                  && (listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Solicitado)
                  || listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Pendente))
                  && Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M))
                {
                    ViewBag.ExibirEstoque3M1 = true;
                    ViewBag.ExibirEstoque3M2 = true;
                    listaPedidoPecas.permiteEditar = true;
                    listaPedidoPecas.permiteExcluir = true;
                }
                else if (tipoOrigemPagina == "Confirmacao"
                  && (listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Aprovado)
                  || listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Pendente)
                  || listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia))
                  && Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M))
                {
                    listaPedidoPecas.permiteEditar = true;
                    listaPedidoPecas.permiteExcluir = false;
                    listaPedidoPecas.permiteSelecionar = true;
                }
                else if (tipoOrigemPagina == "Confirmacao"
                  && (listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Aprovado)
                  || listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Pendente)
                  || listaPedidoPecas.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia))
                  && (Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Tecnico3M)
                  || Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira)
                  || Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Cliente)
                  || Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.nidPerfil) == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica)
                  ))
                {
                    listaPedidoPecas.permiteEditar = true;
                    listaPedidoPecas.permiteExcluir = false;
                    listaPedidoPecas.permiteSelecionar = true;
                    ViewBag.Tecnico3M = "S";
                }




                if (tipoOrigemPagina == "Aprovacao")
                {
                    if (string.IsNullOrEmpty(listaPedidoPecas.estoquePeca3M.QT_PECA_ATUAL) && string.IsNullOrEmpty(listaPedidoPecas.estoquePeca3M2.QT_PECA_ATUAL) && string.IsNullOrEmpty(listaPedidoPecas.pedidoPeca.QTD_APROVADA))
                    {
                        pecaNaoEncontradaEstoque = true;
                        listaPedidoPecas.cssRegraGRIDAplicar = "text-primary font-weight-bold";
                    }
                    else if (string.IsNullOrEmpty(listaPedidoPecas.pedidoPeca.QTD_APROVADA))
                    {
                        if (
                        (Convert.ToDecimal(listaPedidoPecas.estoquePeca3M.QT_PECA_ATUAL) < Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA)) ||
                            Convert.ToDecimal(listaPedidoPecas.estoquePeca3M2.QT_PECA_ATUAL) < Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA))
                        {
                            estoqueAbaixoSolicitado = true;
                            listaPedidoPecas.cssRegraGRIDAplicar = "text-danger font-weight-bold";
                        }
                    }
                    else
                    {
                        bool estoque3M1Valido;
                        bool estoque3M2Valido;

                        decimal aprovada3M1 = Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1);
                        decimal aprovada3M2 = Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M2);

                        if (TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Value)
                        {
                            //Validação pedido Técnico X Plano Zero
                            decimal quantidadePlanoZero = Convert.ToDecimal(listaPedidoPecas.planoZero.qtPecaModelo);

                            estoque3M1Valido = aprovada3M1 <= quantidadePlanoZero;
                            estoque3M2Valido = aprovada3M2 <= quantidadePlanoZero;
                        }
                        else
                        {
                            estoque3M1Valido = Convert.ToDecimal(listaPedidoPecas.estoquePeca3M.QT_PECA_ATUAL) >= aprovada3M1;
                            estoque3M2Valido = Convert.ToDecimal(listaPedidoPecas.estoquePeca3M2.QT_PECA_ATUAL) >= aprovada3M2;

                            if (!estoque3M1Valido || !estoque3M2Valido)
                            {
                                //Chamado SL00034833
                                if (listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM == "Aguardando")
                                {
                                    estoqueAbaixoSolicitado = true;
                                }
                            }
                        }

                        if (!estoque3M1Valido || !estoque3M2Valido)
                        {
                            listaPedidoPecas.cssRegraGRIDAplicar = "text-danger font-weight-bold";
                        }
                    }
                }

                if (row["ID_LOTE_APROVACAO"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.ID_LOTE_APROVACAO = Convert.ToInt64(row["ID_LOTE_APROVACAO"]);
                }
                if (row["NR_LINHA"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.NR_LINHA = Convert.ToInt64(row["NR_LINHA"]);
                }

                if (row["DS_DIR_FOTO"] != DBNull.Value)
                {
                    listaPedidoPecas.pedidoPeca.DS_DIR_FOTO = Convert.ToString(row["DS_DIR_FOTO"]);
                }

                if (listaPedidoPecas.planoZero.qtPecaModelo == "0,000" || listaPedidoPecas.planoZero.qtPecaModelo == "0.000")
                {
                    listaPedidoPecas.planoZero.qtPecaModelo = "0";
                }

                if (listaPedidoPecas.pedidoPeca.QTD_SOLICITADA == "0,000" || listaPedidoPecas.pedidoPeca.QTD_SOLICITADA == "0.000")
                {
                    listaPedidoPecas.pedidoPeca.QTD_SOLICITADA = "0";
                }

                if (listaPedidoPecas.pedidoPeca.QTD_APROVADA == "0,000" || listaPedidoPecas.pedidoPeca.QTD_APROVADA == "0.000")
                {
                    listaPedidoPecas.pedidoPeca.QTD_APROVADA = "0";
                }

                if (listaPedidoPecas.pedidoPeca.QTD_RECEBIDA == "0,000" || listaPedidoPecas.pedidoPeca.QTD_RECEBIDA == "0.000")
                {
                    listaPedidoPecas.pedidoPeca.QTD_RECEBIDA = "0";
                }

                if (!String.IsNullOrEmpty(listaPedidoPecas.planoZero.qtPecaModelo))
                    listaPedidoPecas.planoZero.qtPecaModelo.Replace(',', '.');

                if (!String.IsNullOrEmpty(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA))
                    listaPedidoPecas.pedidoPeca.QTD_SOLICITADA.Replace(',', '.');

                listaPedidoPecas.pedidoPeca.VL_TOTAL_PECA_SOLICITADA = Convert.ToDecimal(Convert.ToDecimal(listaPedidoPecas.pedidoPeca.QTD_SOLICITADA) * Convert.ToDecimal(listaPedidoPecas.peca.VL_PECA)).ToString("N2");
                VL_TOTAL_PECA_SOLICITADA += Convert.ToDecimal(listaPedidoPecas.pedidoPeca.VL_TOTAL_PECA_SOLICITADA);

                if (listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM != "Cancelado")
                {
                    if ((Convert.ToInt32(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1) == 0 && listaPedidoPecas.pedidoPeca.CD_PECA_REFERENCIA != ""))
                    {
                        listasPedidoPecas.Add(listaPedidoPecas);
                    }
                    else if ((Convert.ToInt32(listaPedidoPecas.pedidoPeca.QTD_APROVADA_3M1) > 0))
                    {
                        listasPedidoPecas.Add(listaPedidoPecas);
                    }else if(listaPedidoPecas.TP_Especial == "E" && listaPedidoPecas.pedidoPeca.DS_ST_STATUS_ITEM != "Cancelado")
                    {
                        listasPedidoPecas.Add(listaPedidoPecas);
                    }
                }
                    

               
            }


            if (dataTableReader != null)
            {
                dataTableReader.Dispose();
                dataTableReader = null;
            }

            //}
            //catch (Exception ex)
            //{
            //    LogUtility.LogarErro(ex);
            //    throw ex;
            //}

            ViewBag.VL_TOTAL_PECA_SOLICITADA = Convert.ToDecimal(VL_TOTAL_PECA_SOLICITADA).ToString("N2");
            ViewBag.VL_TOTAL_PECA = Convert.ToDecimal(VL_TOTAL_PECA).ToString("N2");
            ViewBag.VL_TOTAL_PECA_Restante = Convert.ToDecimal(VL_TOTAL_PECA_Restante).ToString("N2");
            ViewBag.VL_TOTAL_PECA_CANCELADAS = Convert.ToDecimal(VL_TOTAL_PECA_CANCELADAS).ToString("N2");



            return listasPedidoPecas;
        }


    }
}