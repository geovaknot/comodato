using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Front.Services;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class AgendaController : BaseController
    {
        private const int PRIMEIRO_ITEM_LISTA = 0;

        public OsPadraoService _osPadraoService;

        
        public AgendaController()
        {
            _osPadraoService = new OsPadraoService();
        }

        // GET: Agenda
        [_3MAuthentication]
        public ActionResult Index()
        {
            Models.AgendaAtendimento agendaAtendimento = new Models.AgendaAtendimento
            {
                clientes = new List<Models.Cliente>(),
                tecnicos = new List<Models.Tecnico>(),
                tiposStatusVisitaOS = new List<Models.TpStatusVisitaOS>(),
                regioes = new List<Entity.RegiaoEntity>(),
            };

            return View(agendaAtendimento);
        }

        [_3MAuthentication]
        public ActionResult EditarVisita(string idKey)
        {
            ViewBag.Perfil = ((_3M.Comodato.Entity.UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.ccdPerfil;

            Models.VisitaTecnica visitaTecnica = null;

            DefineAcessosPerfisViewBag();

            try
            {
                string[] conteudo = ControlesUtility.Criptografia.Descriptografar(idKey).Split('|');
                Int64 ID_AGENDA = Convert.ToInt64(conteudo[0]);
                Int64 ID_VISITA = Convert.ToInt64(conteudo[1]);
                Int32 CD_CLIENTE = Convert.ToInt32(conteudo[2]);
                string CD_TECNICO = conteudo[3];
                Int64 ID_OS = Convert.ToInt64(conteudo[4]);
                string tipoOrigemPagina = conteudo[5];

                visitaTecnica = new Models.VisitaTecnica
                {
                    OS = new OSEntity()
                    {
                        ID_OS = ID_OS
                    },
                    agenda = new AgendaEntity()
                    {
                        ID_AGENDA = ID_AGENDA
                    },
                    ID_VISITA = ID_VISITA,
                    cliente = new ClienteEntity()
                    {
                        CD_CLIENTE = CD_CLIENTE
                    },
                    tecnico = new TecnicoEntity()
                    {
                        CD_TECNICO = CD_TECNICO
                    },
                    tiposStatusVisitaOS = new List<Models.TpStatusVisitaOS>(),
                    listaAtivoCliente = ObterListaAtivoCliente(CD_Cliente: Convert.ToInt32(CD_CLIENTE), SomenteATIVOSsemDTDEVOLUCAO: true),
                    listaOS = ObterListaOS(ID_AGENDA: ID_AGENDA, ID_VISITA: ID_VISITA, CD_CLIENTE: CD_CLIENTE, CD_TECNICO: CD_TECNICO, tipoOrigemPagina: tipoOrigemPagina),
                    listaPecas = ObterListaPecas(ID_VISITA),
                    listaLogStatusVisita = new List<Models.LogStatusVisita>(),
                    tipoOrigemPagina = tipoOrigemPagina,
                };

                visitaTecnica.qtdeAtivosCliente = visitaTecnica.listaAtivoCliente.Count();
                // Só pode haver uma OS aberta em cada Visita
                visitaTecnica.ExisteOSAberta = (visitaTecnica.listaOS.Count(o => o.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOS.Aberta)) > 0 ? "S" : "N");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (visitaTecnica == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(visitaTecnica);
            }
        }

        [_3MAuthentication]
        public ActionResult EditarTeste(string idKey)
        {
            ViewBag.Perfil = ((_3M.Comodato.Entity.UsuarioPerfilEntity)Session["_CurrentUser"]).perfil.ccdPerfil;

            Models.VisitaTecnica visitaTecnica = null;

            DefineAcessosPerfisViewBag();

            try
            {
                string[] conteudo = ControlesUtility.Criptografia.Descriptografar(idKey).Split('|');
                Int64 ID_AGENDA = Convert.ToInt64(conteudo[0]);
                Int64 ID_VISITA = Convert.ToInt64(conteudo[1]);
                Int32 CD_CLIENTE = Convert.ToInt32(conteudo[2]);
                string CD_TECNICO = conteudo[3];
                Int64 ID_OS = Convert.ToInt64(conteudo[4]);
                string tipoOrigemPagina = conteudo[5];

                visitaTecnica = new Models.VisitaTecnica
                {
                    OS = new OSEntity()
                    {
                        ID_OS = ID_OS
                    },
                    agenda = new AgendaEntity()
                    {
                        ID_AGENDA = ID_AGENDA
                    },
                    ID_VISITA = ID_VISITA,
                    cliente = new ClienteEntity()
                    {
                        CD_CLIENTE = CD_CLIENTE
                    },
                    tecnico = new TecnicoEntity()
                    {
                        CD_TECNICO = CD_TECNICO
                    },
                    tiposStatusVisitaOS = new List<Models.TpStatusVisitaOS>(),
                    listaAtivoCliente = ObterListaAtivoCliente(CD_Cliente: Convert.ToInt32(CD_CLIENTE), SomenteATIVOSsemDTDEVOLUCAO: true),
                    listaOS = ObterListaOS(ID_AGENDA: ID_AGENDA, ID_VISITA: ID_VISITA, CD_CLIENTE: CD_CLIENTE, CD_TECNICO: CD_TECNICO, tipoOrigemPagina: tipoOrigemPagina),
                    listaPecas = ObterListaPecas(ID_VISITA),
                    listaLogStatusVisita = new List<Models.LogStatusVisita>(),
                    tipoOrigemPagina = tipoOrigemPagina,
                };

                visitaTecnica.qtdeAtivosCliente = visitaTecnica.listaAtivoCliente.Count();
                // Só pode haver uma OS aberta em cada Visita
                visitaTecnica.ExisteOSAberta = (visitaTecnica.listaOS.Count(o => o.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOS.Aberta)) > 0 ? "S" : "N");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (visitaTecnica == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(visitaTecnica);
            }
        }
        [_3MAuthentication]
        public ActionResult EditarOS(string idKey)
        {
            Models.PreenchimentoOS preenchimentoOS = null;

            DefineAcessosPerfisViewBag();

            try
            {
                string[] conteudo = ControlesUtility.Criptografia.Descriptografar(idKey).Split('|');
                Int64 ID_AGENDA = Convert.ToInt64(conteudo[0]);
                Int64 ID_VISITA = Convert.ToInt64(conteudo[1]);
                Int32 CD_CLIENTE = Convert.ToInt32(conteudo[2]);
                string CD_TECNICO = conteudo[3];
                Int64 ID_OS = Convert.ToInt64(conteudo[4]);
                string tipoOrigemPagina = conteudo[5];

                preenchimentoOS = new Models.PreenchimentoOS
                {
                    idKeyInicial = ControlesUtility.Criptografia.Criptografar(ID_AGENDA.ToString() + "|" + ID_VISITA.ToString() + "|" + CD_CLIENTE.ToString() + "|" + CD_TECNICO.ToString() + "|0|" + tipoOrigemPagina),
                    ID_OS = ID_OS,
                    ID_OS_Formatado = Convert.ToInt64(ID_OS).ToString("000000"),
                    agenda = new AgendaEntity()
                    {
                        ID_AGENDA = ID_AGENDA
                    },
                    visita = new VisitaEntity()
                    {
                        ID_VISITA = ID_VISITA
                    },
                    cliente = new ClienteEntity()
                    {
                        CD_CLIENTE = CD_CLIENTE
                    },
                    tecnico = new TecnicoEntity()
                    {
                        CD_TECNICO = CD_TECNICO
                    },
                    tiposStatusVisitaOS = new List<Models.TpStatusVisitaOS>(),
                    //listaAtivoCliente = ObterListaAtivoCliente(CD_Cliente: Convert.ToInt32(CD_CLIENTE), SomenteATIVOSsemDTDEVOLUCAO: true),
                    listaLogStatusVisita = new List<Models.LogStatusVisita>(),
                    ativos = new List<Models.Ativo>(),
                    tiposManutencao = ControlesUtility.Dicionarios.TipoManutencaoOS(),
                    pecaOS = new Models.PecaOS()
                    {
                        listaPecas = new List<PecaEntity>(),
                        tiposEstoqueUtilizado = ControlesUtility.Dicionarios.TipoEstoqueUtilizado()
                    },
                    pendenciaOS = new Models.PendenciaOS()
                    {
                        listaPecas = new List<PecaEntity>(),
                        tiposEstoqueUtilizado = ControlesUtility.Dicionarios.TipoEstoqueUtilizado(),
                        tiposStatusPendenciaOS = ControlesUtility.Dicionarios.TipoStatusPendenciaOS(),
                        tiposPendenciaOS = ControlesUtility.Dicionarios.TipoPendenciaOS()
                    },
                    tipoOrigemPagina = tipoOrigemPagina,
                };
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (preenchimentoOS == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(preenchimentoOS);
            }
        }

        [_3MAuthentication]
        public ActionResult ConsultaConfirmaVisita()
        {
            Models.ConsultaConfirmaVista consultaConfirmaVista = new Models.ConsultaConfirmaVista
            {
                DT_DATA_ABERTURA_INICIO = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy"),
                DT_DATA_ABERTURA_FIM = DateTime.Now.ToString("dd/MM/yyyy"),
                clientes = new List<Models.Cliente>(),
                tecnicos = new List<Models.Tecnico>(),
                ativos = new List<Models.Ativo>(),
                OSs = new List<Models.OS>(),
                tiposStatusVisitaOS = new List<Models.TpStatusVisitaOS>()
                //avaliacaoVisita = new Models.AvaliacaoVisita(),
            };

            return View(consultaConfirmaVista);
        }

        public ActionResult Visualizar(Int64 ID_OS)
        {
            return View(ObterOS(ID_OS));
        }

        private OsPadrao ObterOS(Int64 ID_OS)
        {
            var osPadrao = _osPadraoService.ObterOS(ID_OS);

            if (osPadrao != null)
            {
                PopularListas(osPadrao);
                osPadrao.ativoFixo.DS_ATIVO_FIXO_COMPLETA = osPadrao.ativoFixo.CD_ATIVO_FIXO + " - " + osPadrao.ativoFixo.DS_ATIVO_FIXO;
                osPadrao.cliente.NM_CLIENTE_Codigo = osPadrao.cliente.NM_CLIENTE + " - " + osPadrao.cliente.CD_CLIENTE;
            }

            return osPadrao;
        }

        private void PopularListas(OsPadrao osPadrao)
        {
            osPadrao.ativos = osPadrao.cliente.CD_CLIENTE > 0 ? ObterListaAtivoCliente(Convert.ToInt32(osPadrao.cliente.CD_CLIENTE), true) : new List<ListaAtivoCliente>();
            osPadrao.tiposStatusOsPadrao = ObterStatusOsPadrao();
            osPadrao.tiposOsPadrao = ObterTipoOsPadrao();
            if (osPadrao.CodigoTecnico != null && osPadrao.CodigoTecnico != "")
            {
                TecnicoSinc tecnico = new TecnicoData().ObterTecnicoOS(osPadrao.CodigoTecnico).FirstOrDefault();
                osPadrao.clientes = ObterListaClientePorUsuarioPerfil(Convert.ToInt64(tecnico.ID_USUARIO), true);
            }
            else
                osPadrao.clientes = ObterListaClientePorUsuarioPerfil(Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).usuario.nidUsuario), true);
            osPadrao.tecnicos = ObterListaTecnicosAtivos(Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).usuario.nidUsuario));
            osPadrao.tecnicos.RemoveAt(PRIMEIRO_ITEM_LISTA);
            osPadrao.regioes = ObterListaRegiao();
        }

        public JsonResult ObterListaAgendaAtendimentoJson(string CD_TECNICO, string CD_REGIAO, int? ST_TP_STATUS_VISITA_OS)
        {
            DefineAcessosPerfisViewBag();

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                var agenda = new AgendaEntity()
                {
                    tecnico = new TecnicoEntity()
                    {
                        CD_TECNICO = CD_TECNICO
                    },

                };

                List<Models.ListaAgendaAtendimento> listaAgendaAtendimento = ObterListaAgendaAtendimento(agenda, CD_REGIAO, CurrentUser.usuario.nidUsuario, ST_TP_STATUS_VISITA_OS);

                ViewBag.CD_TECNICO = CD_TECNICO;
                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Agenda/_gridMVC.cshtml", listaAgendaAtendimento));
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

        public List<Models.ListaAgendaAtendimento> ObterListaAgendaAtendimento(AgendaEntity agendaEntity, string CD_REGIAO, long? usuario, int? ST_TP_STATUS_VISITA_OS)
        {
            List<Models.ListaAgendaAtendimento> listaAgendaAtendimentos = new List<Models.ListaAgendaAtendimento>();

            try
            {
                bool limparVisitaOSIniciar = false;

                DataTableReader dataTableReader = new AgendaData().ObterListaAtendimento(agendaEntity, CD_REGIAO, null, usuario, ST_TP_STATUS_VISITA_OS).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaAgendaAtendimento listaAgendaAtendimento = new Models.ListaAgendaAtendimento();
                        //{
                        string cdTecnicoVisita = dataTableReader["CD_TECNICO_VISITA"].ToString();
                        if (String.IsNullOrEmpty(cdTecnicoVisita))
                        {
                            cdTecnicoVisita = agendaEntity.tecnico.CD_TECNICO;
                        }
                        //if (cdTecnicoVisita == "347803")
                        //{
                        //    var x = 1;
                        //}

                        if (Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]).ToString() != "" && Convert.ToInt64("0" + dataTableReader["ID_VISITA"]).ToString() != "" && Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"]).ToString() != "" && cdTecnicoVisita != "")
                            listaAgendaAtendimento.idKey = ControlesUtility.Criptografia.Criptografar(Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]).ToString() + "|" + Convert.ToInt64("0" + dataTableReader["ID_VISITA"]).ToString() + "|" + Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"]).ToString() + "|" + cdTecnicoVisita + "|0|Agenda");
                        //if (Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]).ToString() != "" && Convert.ToInt64("0" + dataTableReader["ID_VISITA"]).ToString() != "" && Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"]).ToString() != "" && dataTableReader["CD_TECNICO_PRINCIPAL"].ToString() != "")
                        //    listaAgendaAtendimento.idKey = ControlesUtility.Criptografia.Criptografar(Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]).ToString() + "|" + Convert.ToInt64("0" + dataTableReader["ID_VISITA"]).ToString() + "|" + Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"]).ToString() + "|" + dataTableReader["CD_TECNICO_PRINCIPAL"].ToString() + "|0|Agenda");

                        if (Convert.ToInt64("0" + dataTableReader["ID_VISITA"]).ToString() != "")
                            listaAgendaAtendimento.ID_VISITA = Convert.ToInt64("0" + dataTableReader["ID_VISITA"]);

                        if (Convert.ToInt32("0" + dataTableReader["ST_TP_STATUS_VISITA_OS"]).ToString() != "")
                            listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS = Convert.ToInt32("0" + dataTableReader["ST_TP_STATUS_VISITA_OS"]);

                        if (dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString() != "")
                            listaAgendaAtendimento.DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString();

                        if (Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]).ToString() != "")
                            listaAgendaAtendimento.ID_AGENDA = Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]);

                        if (Convert.ToInt64(dataTableReader["CD_CLIENTE"]).ToString() != "")
                            listaAgendaAtendimento.CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]);

                        if (dataTableReader["NM_CLIENTE"].ToString() != "" && dataTableReader["CD_CLIENTE"].ToString() != "" && dataTableReader["EN_CIDADE"].ToString() != "" && dataTableReader["EN_ESTADO"].ToString() != "")
                            listaAgendaAtendimento.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString();

                        if (dataTableReader["EN_CIDADE"].ToString() != "")
                            listaAgendaAtendimento.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();

                        if (dataTableReader["EN_ESTADO"].ToString() != "")
                            listaAgendaAtendimento.EN_ESTADO = dataTableReader["EN_ESTADO"].ToString();

                        if (dataTableReader["CD_REGIAO"].ToString() != "")
                            listaAgendaAtendimento.CD_REGIAO = dataTableReader["CD_REGIAO"].ToString();

                        if (dataTableReader["DS_REGIAO"].ToString() != "")
                            listaAgendaAtendimento.DS_REGIAO = dataTableReader["DS_REGIAO"].ToString();

                        if (dataTableReader["CD_TECNICO_PRINCIPAL"].ToString() != "")
                            listaAgendaAtendimento.CD_TECNICO_PRINCIPAL = dataTableReader["CD_TECNICO_PRINCIPAL"].ToString();

                        if (dataTableReader["NM_TECNICO_PRINCIPAL"].ToString() != "")
                            listaAgendaAtendimento.NM_TECNICO_PRINCIPAL = dataTableReader["NM_TECNICO_PRINCIPAL"].ToString();

                        if (dataTableReader["QT_PERIODO"] != DBNull.Value)
                            listaAgendaAtendimento.QT_PERIODO = Convert.ToInt32(dataTableReader["QT_PERIODO"]);
                        else
                            listaAgendaAtendimento.QT_PERIODO = 0;

                        listaAgendaAtendimento.NR_ORDENACAO = Convert.ToInt32(dataTableReader["NR_ORDENACAO"]);

                        if (listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Finalizada)
                            || listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Confirmada)
                            || listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Cancelada))
                        {
                            //listaAgendaAtendimento.idKeyINICIAR = ControlesUtility.Criptografia.Criptografar(Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]).ToString() + "|0|" + Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"]).ToString() + "|" + dataTableReader["CD_TECNICO_PRINCIPAL"].ToString() + "|0|Agenda");
                            listaAgendaAtendimento.idKeyINICIAR = ControlesUtility.Criptografia.Criptografar(Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]).ToString() + "|0|" + Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"]).ToString() + "|" + agendaEntity.tecnico.CD_TECNICO.ToString() + "|0|Agenda");
                            listaAgendaAtendimento.DS_TP_STATUS_VISITA_OS_INICIAR = "Iniciar";
                        }
                        else
                        {
                            listaAgendaAtendimento.idKeyINICIAR = string.Empty;
                            listaAgendaAtendimento.DS_TP_STATUS_VISITA_OS_INICIAR = string.Empty;
                        }

                        if (dataTableReader["DT_DATA_VISITA"] != DBNull.Value)
                            listaAgendaAtendimento.DT_DATA_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_VISITA"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DT_DESATIVACAO"] != DBNull.Value)
                            listaAgendaAtendimento.DT_DESATIVACAO = Convert.ToDateTime(dataTableReader["DT_DESATIVACAO"]).ToString("dd/MM/yyyy");

                        if (listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == null)
                            listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS = 0;

                        if (listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == 0)
                            listaAgendaAtendimento.DS_TP_STATUS_VISITA_OS = "Iniciar";

                        listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS_INICIAR = listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS;


                        // Se Status da Visita for 1-Nova, 2-Aberta, 5-Pausada, 8-Portaria, 9-Integração, 10-Treinamento, 11-Consultoria 
                        // DESATIVA a opção INICIAR para as visitas finalizadas e para os clientes que ainda não possuem nenhuma visita (Só pode haver uma visita em andamento nestes status)
                        if ((listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == 1 || listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == 2 || listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == 5
                            || listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == 8 || listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == 9 || listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == 10
                            || listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == 11) && cdTecnicoVisita == agendaEntity.tecnico.CD_TECNICO)
                            limparVisitaOSIniciar = true;

                        //listaAgendaAtendimento.listaLogStatusOs = ObterListaLogStatusOS(ID_VISITA: null, CD_CLIENTE: listaAgendaAtendimento.CD_CLIENTE);
                        //listaAgendaAtendimento.TempoGastoTOTAL = CalcularTempoGastoOS(listaAgendaAtendimento.listaLogStatusOs);
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
                                if (listaAgendaAtendimento.QT_PERIODO > 0)                                                                                                       //TimeSpan QT_PERIODO = TimeSpan.FromHours(listaAgendaAtendimento.QT_PERIODO);
                                    listaAgendaAtendimento.PERCENTUAL = Convert.ToDecimal((listaAgendaAtendimento.QT_PERIODO_REALIZADO * 100) / (listaAgendaAtendimento.QT_PERIODO)).ToString("N2");
                                else
                                    listaAgendaAtendimento.PERCENTUAL = Convert.ToDecimal(0).ToString("N2");
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

                if (limparVisitaOSIniciar == true)
                {
                    foreach (var item in listaAgendaAtendimentos)
                    {
                        item.ST_TP_STATUS_VISITA_OS_INICIAR = 0;
                        item.DS_TP_STATUS_VISITA_OS_INICIAR = string.Empty;
                        item.idKeyINICIAR = string.Empty;

                        if (item.ST_TP_STATUS_VISITA_OS == 0)
                        {
                            item.ST_TP_STATUS_VISITA_OS = null;
                            item.DS_TP_STATUS_VISITA_OS = string.Empty;
                        }
                    }
                }
                //if (limparVisitaOSIniciar == true)
                //{
                //    foreach (Models.ListaAgendaAtendimento listaAgendaAtendimento in listaAgendaAtendimentos)
                //    {
                //        //if (listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == 0)
                //        if (listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == 0)
                //        //|| listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Nova))
                //        {
                //            listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS = null;
                //            listaAgendaAtendimento.DS_TP_STATUS_VISITA_OS = string.Empty;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            // Só pode haver uma visita ATIVA por vez na agenda do técnico. 
            // ATIVO = (Nova, Aberta, Portaria, Integração ou Pausada)
            //if (listaAgendaAtendimentos.Exists(x => x.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Nova) ||
            //     x.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Aberta) ||
            //     x.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Portaria) ||
            //     x.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Integracao) ||
            //     x.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Pausada) ||
            //     x.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Treinamento) ||
            //     x.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Consultoria)
            //))
            return listaAgendaAtendimentos;

        }

        /// <summary>
        /// Consulta o histórico de LOG_STATUS_OS
        /// </summary>
        /// <param name="ID_VISITA"></param>
        /// <param name="CD_CLIENTE"></param>
        /// <returns></returns>
        internal protected List<Models.ListaLogStatusOS> ObterListaLogStatusOS(Int64? ID_VISITA = null, Int64? CD_CLIENTE = null)
        {
            List<Models.ListaLogStatusOS> listasLogStatusOs = new List<Models.ListaLogStatusOS>();

            try
            {
                DataTableReader dataTableReader = new LogStatusOSData().ObterListaAgendaVisita(ID_VISITA: ID_VISITA, CD_CLIENTE: CD_CLIENTE).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaLogStatusOS listaLogStatusOS = new Models.ListaLogStatusOS
                        {
                            OS = new OSEntity()
                            {
                                ID_OS = Convert.ToInt64(dataTableReader["ID_OS"])
                            },
                            //ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]),
                            tecnico = new TecnicoEntity()
                            {
                                CD_TECNICO = dataTableReader["CD_TECNICO"].ToString()
                            },
                            //CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                            DT_DATA_LOG_OS = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_OS"]),
                            tpStatusVisitaOS = new TpStatusVisitaOSEntity()
                            {
                                ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]),
                                DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString()
                            }
                            //ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]),
                            //DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString()
                        };
                        listasLogStatusOs.Add(listaLogStatusOS);
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

            //CalcularTempoGasto(listasLogStatusOs);

            return listasLogStatusOs;
        }

        /// <summary>
        /// Calcula o tempo gasto no histórico de LOG_STATUS_OS
        /// </summary>
        /// <param name="listasLogStatusOs"></param>
        /// <returns></returns>
        internal protected TimeSpan CalcularTempoGastoOS(List<Models.ListaLogStatusOS> listasLogStatusOs)
        {
            string CD_TECNICO = string.Empty;
            TimeSpan TempoGasto = new TimeSpan();
            TimeSpan TempoGastoTOTALTECNICO = new TimeSpan();
            TimeSpan TempoGastoTOTAL = new TimeSpan();
            string TempoTOTAL = string.Empty;
            DateTime? DT_DATA_LOG1 = null;
            DateTime? DT_DATA_LOG2 = null;

            try
            {
                foreach (Models.ListaLogStatusOS listaLogStatusOs in listasLogStatusOs)
                {
                    if (listaLogStatusOs.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS != Convert.ToInt32(Utility.ControlesUtility.Enumeradores.TpStatusOS.Nova))
                    {
                        if (string.IsNullOrEmpty(CD_TECNICO))
                        {
                            CD_TECNICO = listaLogStatusOs.tecnico.CD_TECNICO;
                        }

                        if (CD_TECNICO != listaLogStatusOs.tecnico.CD_TECNICO)
                        {
                            CD_TECNICO = listaLogStatusOs.tecnico.CD_TECNICO;
                            TempoGastoTOTALTECNICO = new TimeSpan();
                            DT_DATA_LOG1 = null;
                            DT_DATA_LOG2 = null;
                        }

                        if (listaLogStatusOs.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(Utility.ControlesUtility.Enumeradores.TpStatusOS.Aberta))
                        {
                            DT_DATA_LOG1 = listaLogStatusOs.DT_DATA_LOG_OS;
                            DT_DATA_LOG2 = null;
                        }
                        else
                        {
                            DT_DATA_LOG2 = listaLogStatusOs.DT_DATA_LOG_OS;
                        }

                        if (DT_DATA_LOG1 != null && DT_DATA_LOG2 != null)
                        {
                            TempoGasto = (Convert.ToDateTime(DT_DATA_LOG2) - Convert.ToDateTime(DT_DATA_LOG1));
                            TempoGastoTOTALTECNICO += TempoGasto;
                            TempoGastoTOTAL += TempoGasto;

                            listaLogStatusOs.TempoGasto = TempoGasto;
                            listaLogStatusOs.TempoGastoTOTAL = TempoGastoTOTALTECNICO;

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

        public JsonResult ObterListaLogStatusVisitaJson(Int64 ID_VISITA)
        {
            DefineAcessosPerfisViewBag();

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            TimeSpan TempoGastoTOTAL = new TimeSpan();
            try
            {
                List<Models.LogStatusVisita> listaLogStatusVisita = ObterListaLogStatusVisita(ID_VISITA);
                TempoGastoTOTAL = CalcularTempoGastoVisita(listaLogStatusVisita);
                string[] TempoGasto = TempoGastoTOTAL.TotalHours.ToString("N2").Replace(".", ",").Split(',');
                string Horas = Convert.ToInt32(TempoGasto[0]).ToString("00");
                string Minutos = ((Convert.ToDecimal(TempoGasto[1]) / 100) * 60).ToString("00");

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Agenda/_gridMVCHistoricoVisita.cshtml", listaLogStatusVisita));
                jsonResult.Add("TempoGastoTOTAL", Horas + ":" + Minutos);
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
                            ID_LOG_STATUS_VISITA = Convert.ToInt64(dataTableReader["ID_LOG_STATUS_VISITA"]),
                            visita = new VisitaEntity()
                            {
                                ID_VISITA = Convert.ToInt64(dataTableReader["ID_VISITA"])
                            },
                            DT_DATA_LOG_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_VISITA"]), //.ToString("dd/MM/yyyy HH:mm"),
                            tpStatusVisitaOS = new TpStatusVisitaOSEntity()
                            {
                                ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]),
                                DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString()
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

                        if (logStatusVisita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(Utility.ControlesUtility.Enumeradores.TpStatusVisita.Aberta))
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

        public JsonResult ObterListaLogStatusOSJson(Int64 ID_OS)
        {
            DefineAcessosPerfisViewBag();

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            TimeSpan TempoGastoTOTAL = new TimeSpan();
            try
            {
                List<Models.LogStatusOS> listaLogStatusOS = ObterListaLogStatusOS(ID_OS);
                TempoGastoTOTAL = CalcularTempoGastoOS(listaLogStatusOS);
                string[] TempoGasto = TempoGastoTOTAL.TotalHours.ToString("N2").Replace(".", ",").Split(',');
                string Horas = Convert.ToInt32(TempoGasto[0]).ToString("00");
                string Minutos = ((Convert.ToDecimal(TempoGasto[1]) / 100) * 60).ToString("00");

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Agenda/_gridMVCHistoricoOS.cshtml", listaLogStatusOS));
                jsonResult.Add("TempoGastoTOTAL", Horas + ":" + Minutos);
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

        protected List<Models.LogStatusOS> ObterListaLogStatusOS(Int64 ID_OS)
        {
            List<Models.LogStatusOS> listaLogStatusOS = new List<Models.LogStatusOS>();

            if (ID_OS == 0)
            {
                return listaLogStatusOS;
            }

            try
            {
                LogStatusOSEntity logStatusOSEntity = new LogStatusOSEntity();
                logStatusOSEntity.OS.ID_OS = ID_OS;
                DataTableReader dataTableReader = new LogStatusOSData().ObterLista(logStatusOSEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.LogStatusOS logStatusOS = new Models.LogStatusOS
                        {
                            ID_LOG_STATUS_OS = Convert.ToInt64(dataTableReader["ID_LOG_STATUS_OS"]),
                            OS = new OSEntity()
                            {
                                ID_OS = Convert.ToInt64(dataTableReader["ID_OS"])
                            },
                            DT_DATA_LOG_OS = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_OS"]), //.ToString("dd/MM/yyyy HH:mm"),
                            tpStatusVisitaOS = new TpStatusVisitaOSEntity()
                            {
                                ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]),
                                DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString()
                            },
                            usuario = new UsuarioEntity()
                            {
                                cnmNome = dataTableReader["cnmNome"].ToString()
                            }
                        };

                        listaLogStatusOS.Add(logStatusOS);
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

            return listaLogStatusOS;

        }

        /// <summary>
        /// Calcula o tempo gasto no histórico de LOG_STATUS_OS
        /// </summary>
        /// <param name="listasLogStatusOS"></param>
        /// <returns></returns>
        protected TimeSpan CalcularTempoGastoOS(List<Models.LogStatusOS> listasLogStatusOS)
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
                foreach (Models.LogStatusOS logStatusOS in listasLogStatusOS)
                {
                    if (logStatusOS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS != Convert.ToInt32(Utility.ControlesUtility.Enumeradores.TpStatusOS.Nova))
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

                        if (logStatusOS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(Utility.ControlesUtility.Enumeradores.TpStatusOS.Aberta))
                        {
                            DT_DATA_LOG1 = logStatusOS.DT_DATA_LOG_OS;
                            DT_DATA_LOG2 = null;
                        }
                        else
                        {
                            DT_DATA_LOG2 = logStatusOS.DT_DATA_LOG_OS;
                        }

                        if (DT_DATA_LOG1 != null && DT_DATA_LOG2 != null)
                        {
                            TempoGasto = (Convert.ToDateTime(DT_DATA_LOG2) - Convert.ToDateTime(DT_DATA_LOG1));
                            TempoGastoTOTALTECNICO += TempoGasto;
                            TempoGastoTOTAL += TempoGasto;

                            logStatusOS.TempoGasto = TempoGasto;
                            logStatusOS.TempoGastoTOTAL = TempoGastoTOTALTECNICO;

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

        protected List<Models.OS> ObterListaOS(Int64 ID_AGENDA, Int64 ID_VISITA, Int64 CD_CLIENTE, string CD_TECNICO, string tipoOrigemPagina)
        {
            OSEntity oSEntity = new OSEntity();
            oSEntity.visita.ID_VISITA = ID_VISITA;
            oSEntity.tecnico.CD_TECNICO = CD_TECNICO;
            new OSData().ExcluirSemAtivo(oSEntity);

            List<Models.OS> listaOSs = new List<Models.OS>();

            try
            {
                OSEntity osEntity = new OSEntity();
                osEntity.visita.ID_VISITA = (ID_VISITA == 0 ? -1 : ID_VISITA);
                DataTableReader dataTableReader = new OSData().ObterLista(osEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.OS OS = new Models.OS
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(ID_AGENDA.ToString() + "|" + ID_VISITA.ToString() + "|" + CD_CLIENTE.ToString() + "|" + CD_TECNICO + "|" + dataTableReader["ID_OS"].ToString() + "|" + tipoOrigemPagina),
                            ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]),
                            ID_OS_Formatado = Convert.ToInt64(dataTableReader["ID_OS"]).ToString("000000"),
                            ativoFixo = new AtivoFixoEntity()
                            {
                                modelo = new ModeloEntity()
                                {
                                    DS_MODELO = dataTableReader["CD_ATIVO_FIXO"].ToString() + " - " + dataTableReader["DS_MODELO"].ToString() + " - " + dataTableReader["TX_ANO_MÁQUINA"].ToString()
                                },
                            },
                            tpStatusVisitaOS = new TpStatusVisitaOSEntity()
                            {
                                ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]),
                                DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString()
                            },
                        };

                        if (dataTableReader["DT_ULTIMA_MANUTENCAO"] != DBNull.Value)
                            OS.DT_ULTIMA_MANUTENCAO = Convert.ToDateTime(dataTableReader["DT_ULTIMA_MANUTENCAO"]); //.ToString("dd/MM/yyyy");

                        listaOSs.Add(OS);
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

            return listaOSs;

        }

        protected List<Models.Peca> ObterListaPecas(Int64 ID_VISITA)
        {
            List<Models.Peca> listaPecas = new List<Models.Peca>();

            try
            {
                DataTableReader dataTableReader = new OSData().ObterListaPecas(ID_VISITA).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Peca OS = new Models.Peca
                        {
                            CD_PECA = dataTableReader["CD_PECA"].ToString(),
                            DS_PECA = dataTableReader["DS_PECA"].ToString(),
                            QTD_ESTOQUE = Convert.ToInt64(dataTableReader["QT_PECA"]).ToString()
                        };

                        listaPecas.Add(OS);
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

            return listaPecas;

        }
        public JsonResult ObterListaPendenciaOSJson(Int64 CD_CLIENTE, Int64 ID_OS, string CD_TECNICO)
        {
            DefineAcessosPerfisViewBag();

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaPendenciaOS> listaPendenciaOS = ObterListaPendenciaOS(CD_CLIENTE, ID_OS, CD_TECNICO);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Agenda/_gridMVCPendenciaOS.cshtml", listaPendenciaOS));
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

        protected List<Models.ListaPendenciaOS> ObterListaPendenciaOS(Int64 CD_CLIENTE, Int64 ID_OS, string CD_TECNICO)
        {
            List<Models.ListaPendenciaOS> listaPendenciasOS = new List<Models.ListaPendenciaOS>();

            if (CD_CLIENTE == 0 || ID_OS == 0)
            {
                return listaPendenciasOS;
            }

            try
            {
                DataTableReader dataTableReader = new PendenciaOSData().ObterListaCliente(CD_CLIENTE, ID_OS, CD_TECNICO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaPendenciaOS listaPendenciaOS = new Models.ListaPendenciaOS
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_PENDENCIA_OS"].ToString()),
                            ID_PENDENCIA_OS = Convert.ToInt64(dataTableReader["ID_PENDENCIA_OS"]),
                            OsPadrao = new Models.OsPadrao()
                            {
                                ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]),
                                ID_OS_Formatado = Convert.ToInt64(dataTableReader["ID_OS"]).ToString("000000"),
                            },
                            DT_ABERTURA = Convert.ToDateTime(dataTableReader["DT_ABERTURA"]).ToString("dd/MM/yyyy"),
                            DS_DESCRICAO = dataTableReader["DS_DESCRICAO"].ToString(),
                            peca = new PecaEntity()
                            {
                                CD_PECA = dataTableReader["CD_PECA"].ToString(),
                                DS_PECA = dataTableReader["DS_PECA"].ToString(),
                                TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                            },
                            tecnico = new TecnicoEntity()
                            {
                                CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                            },
                            //QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]).ToString("N3"),
                            ST_STATUS_PENDENCIA = ControlesUtility.Dicionarios.TipoStatusPendenciaOS().Where(x => x.Value == dataTableReader["ST_STATUS_PENDENCIA"].ToString()).ToArray()[0].Value,
                            DS_STATUS_PENDENCIA = ControlesUtility.Dicionarios.TipoStatusPendenciaOS().Where(x => x.Value == dataTableReader["ST_STATUS_PENDENCIA"].ToString()).ToArray()[0].Key,
                        };

                        if (dataTableReader["CD_TP_ESTOQUE_CLI_TEC"] != DBNull.Value)
                        {
                            listaPendenciaOS.CD_TP_ESTOQUE_CLI_TEC = ControlesUtility.Dicionarios.TipoEstoqueUtilizado().Where(x => x.Value == dataTableReader["CD_TP_ESTOQUE_CLI_TEC"].ToString()).ToArray()[0].Value;
                            listaPendenciaOS.DS_TP_ESTOQUE_CLI_TEC = ControlesUtility.Dicionarios.TipoEstoqueUtilizado().Where(x => x.Value == dataTableReader["CD_TP_ESTOQUE_CLI_TEC"].ToString()).ToArray()[0].Key;
                        }

                        if (dataTableReader["QT_PECA"] != DBNull.Value)
                        {
                            if (listaPendenciaOS.peca.TX_UNIDADE == "MT")
                                listaPendenciaOS.QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]).ToString("N3");
                            else
                                listaPendenciaOS.QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]).ToString("N0");
                        }

                        listaPendenciasOS.Add(listaPendenciaOS);
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

            return listaPendenciasOS;

        }

        public JsonResult ObterListaPecaOSJson(Int64 ID_OS)
        {
            DefineAcessosPerfisViewBag();

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.PecaOS> listaPecaOS = ObterListaPecaOS(ID_OS);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Agenda/_gridMVCPecaOS.cshtml", listaPecaOS));
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

        protected List<Models.PecaOS> ObterListaPecaOS(Int64 ID_OS)
        {
            PecaOSEntity pecaOSEntity = new PecaOSEntity();
            List<Models.PecaOS> listaPecasOS = new List<Models.PecaOS>();

            if (ID_OS == 0)
            {
                return listaPecasOS;
            }

            try
            {
                pecaOSEntity.OS.ID_OS = ID_OS;

                DataTableReader dataTableReader = new PecaOSData().ObterLista(pecaOSEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.PecaOS pecaOS = new Models.PecaOS
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_PECA_OS"].ToString()),
                            ID_PECA_OS = Convert.ToInt64(dataTableReader["ID_PECA_OS"]),
                            OS = new OSEntity()
                            {
                                ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]),
                            },
                            peca = new PecaEntity()
                            {
                                CD_PECA = dataTableReader["CD_PECA"].ToString(),
                                DS_PECA = dataTableReader["DS_PECA"].ToString(),
                                TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                            },
                            //QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]).ToString("N3"),
                            CD_TP_ESTOQUE_CLI_TEC = ControlesUtility.Dicionarios.TipoEstoqueUtilizado().Where(x => x.Value == dataTableReader["CD_TP_ESTOQUE_CLI_TEC"].ToString()).ToArray()[0].Value,
                            DS_TP_ESTOQUE_CLI_TEC = ControlesUtility.Dicionarios.TipoEstoqueUtilizado().Where(x => x.Value == dataTableReader["CD_TP_ESTOQUE_CLI_TEC"].ToString()).ToArray()[0].Key,
                        };

                        if (pecaOS.peca.TX_UNIDADE == "MT")
                        {
                            pecaOS.QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]).ToString("N3");
                        }
                        else
                        {
                            pecaOS.QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]).ToString("N0");
                        }

                        pecaOS.DS_OBSERVACAO = dataTableReader["DS_OBSERVACAO"].ToString() ?? "";

                        listaPecasOS.Add(pecaOS);
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

            return listaPecasOS;

        }

        public JsonResult ObterOSJson(Int64 ID_OS)
        {
            //DefineAcessosPerfisViewBag();

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                OSPadraoEntity oS = new OSPadraoEntity();
                oS.ID_OS = ID_OS;
                List<OSPadraoEntity> listaVisita = new OSPadraoData().ObterListaOSSinc(oS).ToList();

                if (CurrentUser.perfil.nidPerfil == (Int64)ControlesUtility.Enumeradores.Perfil.Cliente)
                    jsonResult.Add("Html", RenderRazorViewToString("~/Views/Agenda/_gridMVCConsultaConfirmaVisitaCliente.cshtml", listaVisita));
                else
                    jsonResult.Add("Html", RenderRazorViewToString("~/Views/Agenda/_gridMVCConsultaConfirmaVisita.cshtml", listaVisita));

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

        public JsonResult ObterListaVisitaJson(Int32 CD_CLIENTE, string CD_ATIVO_FIXO, string CD_TECNICO, Int64 ID_OS, Int32 ST_TP_STATUS_VISITA_OS, string DT_DATA_ABERTURA_INICIO, string DT_DATA_ABERTURA_FIM)
        {
            //DefineAcessosPerfisViewBag();

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.VisitaTecnica> listaVisita = ObterListaVisita(CD_CLIENTE, CD_ATIVO_FIXO, CD_TECNICO, ID_OS, ST_TP_STATUS_VISITA_OS, DT_DATA_ABERTURA_INICIO, DT_DATA_ABERTURA_FIM);

                if(CurrentUser.perfil.nidPerfil== (Int64)ControlesUtility.Enumeradores.Perfil.Cliente)
                    jsonResult.Add("Html", RenderRazorViewToString("~/Views/Agenda/_gridMVCConsultaConfirmaVisitaCliente.cshtml", listaVisita));
                else
                    jsonResult.Add("Html", RenderRazorViewToString("~/Views/Agenda/_gridMVCConsultaConfirmaVisita.cshtml", listaVisita));

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

        protected List<Models.VisitaTecnica> ObterListaVisita(Int32 CD_CLIENTE, string CD_ATIVO_FIXO, string CD_TECNICO, Int64 ID_OS, Int32 ST_TP_STATUS_VISITA_OS, string DT_DATA_ABERTURA_INICIO, string DT_DATA_ABERTURA_FIM)
        {
            List<Models.VisitaTecnica> listaVisitaTecnica = new List<Models.VisitaTecnica>();

            try
            {
                VisitaEntity visitaEntity = new VisitaEntity();
                DateTime? dtAberturaInicio = null;
                DateTime? dtAberturaFim = null;

                if (!string.IsNullOrEmpty(DT_DATA_ABERTURA_INICIO))
                {
                    dtAberturaInicio = Convert.ToDateTime(DT_DATA_ABERTURA_INICIO);
                }

                if (!string.IsNullOrEmpty(DT_DATA_ABERTURA_FIM))
                {
                    dtAberturaFim = Convert.ToDateTime(DT_DATA_ABERTURA_FIM + " 23:59:59");
                }

                visitaEntity.cliente.CD_CLIENTE = CD_CLIENTE;
                visitaEntity.tecnico.CD_TECNICO = CD_TECNICO;
                visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = ST_TP_STATUS_VISITA_OS;

                DataTableReader dataTableReader = new VisitaData().ObterListaVisitaOS(visitaEntity, CD_ATIVO_FIXO, ID_OS, dtAberturaInicio, dtAberturaFim).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.VisitaTecnica visitaTecnica = new Models.VisitaTecnica
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]).ToString() + "|" + Convert.ToInt64("0" + dataTableReader["ID_VISITA"]).ToString() + "|" + Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"]).ToString() + "|" + dataTableReader["CD_TECNICO"].ToString() + "|0|ConsultaConfirma"),
                            ID_VISITA = Convert.ToInt64("0" + dataTableReader["ID_VISITA"]),
                            DT_DATA_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_VISITA"]).ToString("dd/MM/yyyy HH:mm"),
                            cliente = new ClienteEntity()
                            {
                                CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString(),
                                EN_CIDADE = dataTableReader["EN_CIDADE"].ToString(),
                                EN_ESTADO = dataTableReader["EN_ESTADO"].ToString()
                            },
                            tecnico = new TecnicoEntity()
                            {
                                CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                                NM_TECNICO = dataTableReader["NM_TECNICO"].ToString()
                            },
                            tpStatusVisitaOS = new TpStatusVisitaOSEntity()
                            {
                                ST_TP_STATUS_VISITA_OS = Convert.ToInt32("0" + dataTableReader["ST_TP_STATUS_VISITA_OS"]),
                                DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString()
                            },
                            DS_NOME_RESPONSAVEL = dataTableReader["DS_NOME_RESPONSAVEL"].ToString(),
                            agenda = new AgendaEntity()
                            {
                                ID_AGENDA = Convert.ToInt64("0" + dataTableReader["ID_AGENDA"])
                            }
                        };

                        TimeSpan TempoGastoTOTAL = new TimeSpan();
                        List<Models.LogStatusVisita> listaLogStatusVisita = ObterListaLogStatusVisita(visitaTecnica.ID_VISITA);
                        TempoGastoTOTAL = CalcularTempoGastoVisita(listaLogStatusVisita);
                        string[] TempoGasto = TempoGastoTOTAL.TotalHours.ToString("N2").Replace(".", ",").Split(',');
                        string Horas = Convert.ToInt32(TempoGasto[0]).ToString("00");
                        string Minutos = ((Convert.ToDecimal(TempoGasto[1]) / 100) * 60).ToString("00");

                        visitaTecnica.HR_TOTAL = Horas + ":" + Minutos;

                        visitaTecnica.NR_DIAS_CONFIRMADO = Convert.ToInt64((DateTime.Now - Convert.ToDateTime(dataTableReader["DT_DATA_VISITA"])).TotalDays);

                        // Visitas confirmadas com mais de 3 dias não podem ser desfeitas
                        if ((Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(Convert.ToDateTime(dataTableReader["dtmDataHoraAtualizacao"]).ToShortDateString())).Days > 3 &&
                            visitaTecnica.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == 4)
                            visitaTecnica.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = 0;

                        listaVisitaTecnica.Add(visitaTecnica);
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

            return listaVisitaTecnica;
        }

        protected void DefineAcessosPerfisViewBag()
        {
            ViewBag.Administrador3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M));
            ViewBag.Tecnico3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Tecnico3M));
            if (ViewBag.Tecnico3M == "N")
            {
                ViewBag.Tecnico3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica));
            }

            if (ViewBag.Tecnico3M == "N")
            {
                ViewBag.Tecnico3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira));
            }

            if (ViewBag.Tecnico3M == "N")
            {
                ViewBag.Tecnico3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M));
            }
        }


        public ActionResult MontarIdKey(long CD_CLIENTE, string CD_TECNICO)
        {
            string idKeyINICIAR = "";
            AgendaEntity agendaEntity = new AgendaEntity();

            agendaEntity.cliente.CD_CLIENTE = CD_CLIENTE;
            agendaEntity.tecnico.CD_TECNICO = CD_TECNICO;

            try
            {

                DataTableReader dataTableReader = new AgendaData().ObterListaAtendimento(agendaEntity, null, null, null, null).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        idKeyINICIAR = ControlesUtility.Criptografia.Criptografar(Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]).ToString() + "|0|" + Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"]).ToString() + "|" + agendaEntity.tecnico.CD_TECNICO.ToString() + "|0|HistoricoVisita");
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


            return RedirectToAction("EditarVisita", "Agenda", new { @idkey = idKeyINICIAR }); ;

    }

    }

}