using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Front.Services;
using _3M.Comodato.Utility;
using Newtonsoft.Json.Linq;

namespace _3M.Comodato.Front.Controllers
{
    public class VisitaPadraoController : BaseController
    {
        #region Constantes

        private const int PRIMEIRO_ITEM_LISTA = 0;

        #endregion

        #region Propriedades

        private VisitaPadraoService _visitaPadraoService;
        private OsPadraoService _osPadraoService;

        #endregion

        #region Construtores

        public VisitaPadraoController()
        {
            _visitaPadraoService = new VisitaPadraoService();
            _osPadraoService = new OsPadraoService();
        }

        #endregion

        #region Métodos

        [_3MAuthentication]
        public ActionResult Index()
        {
            var visitaPadrao = new VisitaPadrao
            {
                tiposStatusVisitaPadrao = new List<TpStatusVisitaPadraoEntity>(),
                tiposMotivoVisitaPadrao = new List<TpMotivoVisitaPadraoEntity>(),
                clientes = new List<Cliente>(),
                tecnicos = new List<Tecnico>(),
                regioes = new List<Regiao>()
            };

            return View(visitaPadrao);
        }

        public JsonResult ObterLista(string CD_TECNICO, string CD_REGIAO, int? ST_STATUS_VISITA, int? CD_MOTIVO_VISITA, int? CD_CLIENTE, string orderby = "", string ordertype = "", string DT_INICIAL = "", string DT_FINAL = "")
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                VisitaPadraoEntity visitaPadraoEntity = new VisitaPadraoEntity();
                
                if (CurrentUser.perfil.nidPerfil != (int)ControlesUtility.Enumeradores.Perfil.Administrador3M && CD_TECNICO != "")
                {
                    visitaPadraoEntity = _visitaPadraoService.CriarVisitaPadraoEntity(CD_TECNICO, CD_REGIAO, ST_STATUS_VISITA, CD_MOTIVO_VISITA, CD_CLIENTE);
                }
                else if (CurrentUser.perfil.nidPerfil == (int)ControlesUtility.Enumeradores.Perfil.Administrador3M && CD_TECNICO != "")
                {
                    visitaPadraoEntity = _visitaPadraoService.CriarVisitaPadraoEntity(CD_TECNICO, CD_REGIAO, ST_STATUS_VISITA, CD_MOTIVO_VISITA, CD_CLIENTE);
                }
                else if (CurrentUser.perfil.nidPerfil == (int)ControlesUtility.Enumeradores.Perfil.Administrador3M && CD_TECNICO == "")
                {
                    visitaPadraoEntity = _visitaPadraoService.CriarVisitaPadraoEntity(null, CD_REGIAO, ST_STATUS_VISITA, CD_MOTIVO_VISITA, CD_CLIENTE);
                }

                DateTime? dataInicio = null;
                DateTime? dataFim = null;

                if (DT_INICIAL != "" && DT_INICIAL != null)
                {
                    dataInicio = Convert.ToDateTime(DT_INICIAL);
                }
                if (DT_FINAL != "" && DT_FINAL != null)
                {
                    dataFim = Convert.ToDateTime(DT_FINAL);
                }

                IEnumerable<VisitaPadrao> visitas = _visitaPadraoService.ObterVisitas(new VisitaPadraoData().ObterLista(visitaPadraoEntity, dataInicio, dataFim));

                visitas = OrdernarListaVisita(visitas, orderby, ordertype);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/VisitaPadrao/_gridMVC.cshtml", visitas.ToList()));
                jsonResult.Add("Status", "Success");

                var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
                jsonList.MaxJsonLength = int.MaxValue;
                return jsonList;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        [_3MAuthentication]
        public ActionResult Incluir(string CD_TECNICO)
        {
            VisitaPadrao visitaPadrao = new VisitaPadrao();

            visitaPadrao.CodigoTecnico = CD_TECNICO;

            PopularListas(visitaPadrao);

            //visitaPadrao.clientes = visitaPadrao.clientes.Where(x => x.CD_TECNICO == CD_TECNICO).ToList();

            _visitaPadraoService.AdicionarValoresPadraoIncluirVisita(visitaPadrao, Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).usuario.nidUsuario));

            return View(visitaPadrao);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(VisitaPadrao visitaPadrao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    OsPadrao osPadrao = new OsPadrao();

                    osPadrao.CodigoTecnico = visitaPadrao.CodigoTecnico;
                    if (visitaPadrao.CodigoTecnico == null && visitaPadrao.ID_VISITA != null && visitaPadrao.ID_VISITA != 0)
                    {
                        osPadrao.CodigoTecnico = _visitaPadraoService.ObterCod_Tecnico(visitaPadrao.ID_VISITA);
                    }
                    osPadrao.HR_FIM = visitaPadrao.HR_FIM;
                    osPadrao.HR_INICIO = visitaPadrao.HR_INICIO;
                    osPadrao.DT_DATA_OS = Convert.ToDateTime(visitaPadrao.DT_DATA_VISITA);
                    osPadrao.TpStatusOS.ST_STATUS_OS = 2;
                    bool existeOS = _osPadraoService.ValidarDataHoraOS(osPadrao);

                    bool existeVisita = _visitaPadraoService.ValidarDataHoraVisita(visitaPadrao);

                    if (existeOS == true || existeVisita == true)
                    {
                        //throw new Exception("Já existe Ordem de Serviço ou Visita Registrada nesse Período");
                        visitaPadrao.JavaScriptToRun = "MensagemJaExiste()";
                    }
                    else
                    {
                        VisitaPadraoEntity visitaPadraoEntity = new VisitaPadraoEntity();

                        _visitaPadraoService.InformarStatus(visitaPadrao, CurrentUser.usuario.nidUsuario);

                        _visitaPadraoService.MaperarCamposIncluirVisita(visitaPadrao, visitaPadraoEntity, CurrentUser.usuario.nidUsuario);
                        visitaPadraoEntity.Origem = "W";
                        new VisitaPadraoData().Inserir(ref visitaPadraoEntity);

                        if (visitaPadrao.ID_VISITA == 0)
                        {
                            visitaPadrao.ID_VISITA = visitaPadraoEntity.ID_VISITA;
                        }

                        _visitaPadraoService.EnviarEmail(visitaPadrao, CurrentUser.usuario.nidUsuario);

                        visitaPadrao.JavaScriptToRun = "MensagemSucesso()";
                    }

                    
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            PopularListas(visitaPadrao);

            return View(visitaPadrao);
        }

        [_3MAuthentication]
        public ActionResult Visualizar(string idKey)
        {
            return View(Obter(idKey));
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            return View(Obter(idKey));
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(VisitaPadrao visitaPadrao)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    OsPadrao osPadrao = new OsPadrao();

                    osPadrao.CodigoTecnico = visitaPadrao.CodigoTecnico;
                    if (visitaPadrao.CodigoTecnico == null && visitaPadrao.ID_VISITA != null && visitaPadrao.ID_VISITA != 0)
                    {
                        osPadrao.CodigoTecnico = _visitaPadraoService.ObterCod_Tecnico(visitaPadrao.ID_VISITA);
                    }
                    osPadrao.HR_FIM = visitaPadrao.HR_FIM;
                    osPadrao.HR_INICIO = visitaPadrao.HR_INICIO;
                    osPadrao.DT_DATA_OS = Convert.ToDateTime(visitaPadrao.DT_DATA_VISITA);
                    osPadrao.TpStatusOS.ST_STATUS_OS = 2;
                    bool existeOS = _osPadraoService.ValidarDataHoraOS(osPadrao);

                    bool existeVisita = _visitaPadraoService.ValidarDataHoraVisita(visitaPadrao);

                    if (existeOS == true || existeVisita == true)
                    {
                        //throw new Exception("Já existe Ordem de Serviço ou Visita Registrada nesse Período");
                        visitaPadrao.JavaScriptToRun = "MensagemJaExiste()";
                    }
                    else
                    {
                        
                        VisitaPadraoEntity visitaPadraoEntity = new VisitaPadraoEntity();

                        _visitaPadraoService.InformarStatus(visitaPadrao, CurrentUser.usuario.nidUsuario);

                        _visitaPadraoService.MapearCamposVisitaPadraoParaVisitaPadraoEntity(visitaPadrao, visitaPadraoEntity, CurrentUser.usuario.nidUsuario);

                        new VisitaPadraoData().Alterar(visitaPadraoEntity);

                        if(visitaPadrao.TpStatusVisita.ST_STATUS_VISITA == 4)
                            _visitaPadraoService.EnviarEmail(visitaPadrao, CurrentUser.usuario.nidUsuario);

                        visitaPadrao.JavaScriptToRun = "MensagemSucesso()";
                    }
                    
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            PopularListas(visitaPadrao);

            return View(visitaPadrao);
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            var visitaPadrao = Obter(idKey);

           _visitaPadraoService.ValidarExcluirVisita(visitaPadrao);

            return View(visitaPadrao);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            VisitaPadrao visitaPadrao = new VisitaPadrao();

            try
            {
                if (ModelState.IsValid)
                {
                    VisitaPadraoEntity visitaPadraoEntity = new VisitaPadraoEntity();

                    visitaPadraoEntity.ID_VISITA = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    visitaPadraoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new VisitaPadraoData().Excluir(visitaPadraoEntity);

                    visitaPadrao.JavaScriptToRun = "MensagemSucesso()";
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(visitaPadrao);
        }

        public ActionResult Cancelar(string idKey)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                var visitaPadrao = Obter(idKey);

                VisitaPadraoEntity visitaPadraoEntity = new VisitaPadraoEntity();

                visitaPadrao.TpStatusVisita.ST_STATUS_VISITA = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisitaPadrao.Cancelada);

               _visitaPadraoService.MapearCamposVisitaPadraoParaVisitaPadraoEntity(visitaPadrao, visitaPadraoEntity, CurrentUser.usuario.nidUsuario);

                new VisitaPadraoData().Alterar(visitaPadraoEntity);

                jsonResult.Add("Status", "Success");

                var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
                jsonList.MaxJsonLength = int.MaxValue;
                return jsonList;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        public JsonResult ValidarPermiteIncluirVisita(string codTecnico)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                if (!ValidarExisteVisitaEmAndamento(codTecnico, jsonResult))
                    ValidarExisteOsEmAndamento(codTecnico, jsonResult);

                var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
                jsonList.MaxJsonLength = int.MaxValue;
                return jsonList;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        public JsonResult ValidarPermiteIncluirOsAberta(string codTecnico)
        {

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                OSPadraoEntity os = new OSPadraoEntity();
                os.Tecnico.CD_TECNICO = codTecnico;
                var OSAberta = new OSPadraoData().ObterListaOSSincAbertas(os, 2);

                if (OSAberta?.Count > 0)
                    jsonResult.Add("Aberta", "TemOS");

                var OSAguardandoInicio = new OSPadraoData().ObterListaOSSincAbertas(os, 1);

                if (OSAguardandoInicio?.Count > 0)
                    jsonResult.Add("Iniciar", "TemOS");

                VisitaPadraoEntity visita = new VisitaPadraoEntity();
                visita.Tecnico.CD_TECNICO = codTecnico;

                var VisitaAberto = new VisitaPadraoData().ObterListaVisitaSincAbertas(visita, 3);

                if (VisitaAberto?.Count > 0)
                    jsonResult.Add("Aberta", "TemVisita");

                var VisitaAguardando = new VisitaPadraoData().ObterListaVisitaSincAbertas(visita, 2);

                if (VisitaAguardando?.Count > 0)
                    jsonResult.Add("Iniciar", "TemVisita");

                var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
                jsonList.MaxJsonLength = int.MaxValue;
                return jsonList;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        private bool ValidarExisteVisitaEmAndamento(string codTecnico, Dictionary<string, object> jsonResult)
        {
            if (!_visitaPadraoService.VerificarExisteVisita(codTecnico, ControlesUtility.Enumeradores.TpStatusVisitaPadrao.Iniciar))
            {
                if (_visitaPadraoService.VerificarExisteVisita(codTecnico, ControlesUtility.Enumeradores.TpStatusVisitaPadrao.Aberta) && CurrentUser.perfil.nidPerfil != (int)ControlesUtility.Enumeradores.Perfil.Administrador3M)
                {
                    jsonResult.Add("Aberta", "TemVisita");
                    return true;
                }
            }
            else
            {
                jsonResult.Add("Iniciar", "TemVisita");
                return true;
            }

            return false;
        }

        private bool ValidarExisteOsEmAndamento(string codTecnico, Dictionary<string, object> jsonResult)
        {
            if (!_visitaPadraoService.VerificarExisteOs(codTecnico, ControlesUtility.Enumeradores.TpStatusOSPadrao.AguardandoInicio))
            {
                if (_visitaPadraoService.VerificarExisteOs(codTecnico, ControlesUtility.Enumeradores.TpStatusOSPadrao.Aberta) && CurrentUser.perfil.nidPerfil != (int)ControlesUtility.Enumeradores.Perfil.Administrador3M)
                {
                    jsonResult.Add("Aberta", "TemOS");
                    return true;
                }
            }
            else
            {
                jsonResult.Add("Iniciar", "TemOS");
                return true;
            }

            return false;
        }

        public JsonResult ValidarPermiteIncluirVisitaSalvar(string codTecnico, int idVisita, string dataVisita, string horaInicio, string horaFim)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                if (_visitaPadraoService.VerificarExisteVisitaDataHora(codTecnico, idVisita, dataVisita, horaInicio, horaFim))
                    jsonResult.Add("Resultado", "TemVisita");
                else if (_visitaPadraoService.VerificarExisteOSDataHora(codTecnico, dataVisita, horaInicio, horaFim))
                    jsonResult.Add("Resultado", "TemVisita");

                var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
                jsonList.MaxJsonLength = int.MaxValue;
                return jsonList;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        public VisitaPadrao Obter(string idKey)
        {
            var visitaPadrao = _visitaPadraoService.Obter(idKey);

            if (visitaPadrao != null)
            {
                PopularListas(visitaPadrao);
                visitaPadrao.cliente.NM_CLIENTE_Codigo = visitaPadrao.cliente.NM_CLIENTE + " - " + visitaPadrao.cliente.CD_CLIENTE;
            }
                

            return visitaPadrao;
        }

        private void PopularListas(VisitaPadrao visitaPadrao)
        {
            visitaPadrao.tecnicos = ObterListaTecnicosAtivos(Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).usuario.nidUsuario));
            visitaPadrao.tecnicos.RemoveAt(PRIMEIRO_ITEM_LISTA);
            if (visitaPadrao.CodigoTecnico != null && visitaPadrao.CodigoTecnico != "")
            {
                TecnicoSinc tecnico = new TecnicoData().ObterTecnicoOS(visitaPadrao.CodigoTecnico).FirstOrDefault();
                visitaPadrao.clientes = ObterListaClientePorUsuarioPerfil(Convert.ToInt64(tecnico.ID_USUARIO), true);
            }
            else
                visitaPadrao.clientes = ObterListaClientePorUsuarioPerfil(Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).usuario.nidUsuario), true);
            visitaPadrao.regioes = ObterListaRegiao();
            visitaPadrao.tiposStatusVisitaPadrao = ObterStatusVisitaPadrao();
            visitaPadrao.tiposMotivoVisitaPadrao = ObterMotivoVisitaPadrao();
        }

        public ActionResult Pesquisa(Int64 ID_VISITA)
        {
            ViewBag.Visita = ID_VISITA;

            var visita = _visitaPadraoService.ObterVisita(ID_VISITA);

            ViewBag.NomeTecnico = visita.tecnico.NM_TECNICO;
            ViewBag.DataVisita = visita.DT_DATA_VISITA;

            return View("Pesquisa");
        }

        [HttpPost]
        public JsonResult AvaliarVisita(VisitaResposta visitaResposta)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            try
            {
                if (ModelState.IsValid)
                {

                    visitaResposta.DataResposta = DateTime.UtcNow;
                    visitaResposta.dtmDataHoraAtualizacao = DateTime.UtcNow;
                    if (CurrentUser == null)
                    {
                        var visita = _visitaPadraoService.ObterVisita(visitaResposta.ID_VISITA);

                        ClienteEntity _clienteEntity = new ClienteEntity();
                        _clienteEntity.CD_CLIENTE = visita.cliente.CD_CLIENTE;
                        DataTableReader dataTableReader = new ClienteData().ObterLista(_clienteEntity).CreateDataReader();

                        if (dataTableReader.HasRows)
                        {
                            if (dataTableReader.Read())
                            {
                                _clienteEntity.CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]);
                                _clienteEntity.TX_EMAIL = dataTableReader["TX_EMAIL"].ToString();
                                _clienteEntity.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString();
                            }
                        }

                        if (dataTableReader != null)
                        {
                            dataTableReader.Dispose();
                            dataTableReader = null;
                        }
                        visitaResposta.DS_NOME_RESPONDEDOR = _clienteEntity.NM_CLIENTE;
                        visitaResposta.nidUsuarioAtualizacao = 1;
                    }
                    else
                    {
                        visitaResposta.DS_NOME_RESPONDEDOR = CurrentUser.usuario.cnmNome;
                        visitaResposta.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    }
                    
                    visitaResposta.ID_OS = 0;

                    VisitaRespostaEntity respost = new VisitaRespostaEntity();

                    _visitaPadraoService.MapearCamposVisitaRespostaParaVisitaRespostaEntity(visitaResposta, respost);

                    var avaliacao = new VisitaPadraoData().ObterListaRespostasVISITA(respost);

                    if (avaliacao?.Count == 0)
                    {
                        new VisitaPadraoData().InserirPesquisa(respost);
                        jsonResult.Add("Status", "Success");
                    }
                    else
                    {
                        jsonResult.Add("Error", "Já foi feita uma avaliação a esta Visita!");
                    }

                    
                }

                var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
                jsonList.MaxJsonLength = int.MaxValue;
                return jsonList;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        public ActionResult PesquisaRedirect()
        {
            return View("PesquisaRedirect");
        }
        public ActionResult PesquisaRedirectErro()
        {
            return View("PesquisaRedirectErro");
        }

        #endregion
    }
}