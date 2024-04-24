using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Front.Services;
using _3M.Comodato.Utility;

namespace _3M.Comodato.Front.Controllers
{
    public class OsPadraoController : BaseController
    {
        #region Constantes

        private const int PRIMEIRO_ITEM_LISTA = 0;
        private const string SIM = "S";
        private const string NAO = "N";
        private const int OPERACAO_ESTOQUE_ENTRADA = 1;
        private const int OPERACAO_ESTOQUE_SAIDA = 2;

        #endregion

        #region Propriedades

        public OsPadraoService _osPadraoService;
        public VisitaPadraoService _visitaPadraoService;

        #endregion

        #region Construtores
        public OsPadraoController()
        {
            _osPadraoService = new OsPadraoService();
            _visitaPadraoService = new VisitaPadraoService();
        }

        #endregion


        #region Métodos

        [_3MAuthentication]
        public ActionResult Index()
        {

            var OsPadrao = new OsPadrao
            {
                ativos = new List<ListaAtivoCliente>(),
                tiposStatusOsPadrao = new List<TpStatusOSPadraoEntity>(),
                tiposOsPadrao = new List<TpOSPadraoEntity>(),
                clientes = new List<Cliente>(),
                tecnicos = new List<Tecnico>(),
                regioes = new List<Regiao>()
            };

            return View(OsPadrao);
        }

        public JsonResult ObterLista(string CD_TECNICO, string CD_REGIAO, int? ST_STATUS_OS, int? CD_TIPO_OS, int? CD_CLIENTE, string orderby = "", string ordertype = "", string DT_INICIAL = "", string DT_FINAL = "")
        {
            DefineAcessosPerfisViewBag();

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                OSPadraoEntity oSPadraoEntity = new OSPadraoEntity();
                if (CurrentUser.perfil.nidPerfil != (int)ControlesUtility.Enumeradores.Perfil.Administrador3M && CD_TECNICO != "")
                {
                    oSPadraoEntity = _osPadraoService.CriarOSPadraoEntity(CD_TECNICO, CD_REGIAO, ST_STATUS_OS, CD_TIPO_OS, CD_CLIENTE);
                }
                else if (CurrentUser.perfil.nidPerfil == (int)ControlesUtility.Enumeradores.Perfil.Administrador3M && CD_TECNICO != "")
                {
                    oSPadraoEntity = _osPadraoService.CriarOSPadraoEntity(CD_TECNICO, CD_REGIAO, ST_STATUS_OS, CD_TIPO_OS, CD_CLIENTE);
                }
                else if (CurrentUser.perfil.nidPerfil == (int)ControlesUtility.Enumeradores.Perfil.Administrador3M && CD_TECNICO == "")
                {
                    oSPadraoEntity = _osPadraoService.CriarOSPadraoEntity(null, CD_REGIAO, ST_STATUS_OS, CD_TIPO_OS, CD_CLIENTE);
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

                IEnumerable<OsPadrao> listaOs = _osPadraoService.ObterListaOrdemServico(new OSPadraoData().ObterListaOs(oSPadraoEntity, dataInicio, dataFim));

                if (listaOs != null && CD_TECNICO != "")
                {
                    int Qtd_OS = listaOs.Count();
                    foreach (var os in listaOs)
                    {
                        double Porcentagem = 0;
                        Porcentagem = (Convert.ToDouble(Qtd_OS) * 100) / Convert.ToDouble(os.QT_PERIODO);
                        os.PERCENTUAL = Porcentagem.ToString("N2");
                    }
                }

                listaOs = OrdernarListaOs(listaOs, orderby, ordertype);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/OsPadrao/_gridMVC.cshtml", listaOs.ToList()));
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
            OsPadrao osPadrao = new OsPadrao();

            osPadrao.CodigoTecnico = CD_TECNICO;

            DefineAcessosPerfisViewBag();

            PopularListas(osPadrao);

            //osPadrao.clientes = osPadrao.clientes.Where(x => x.CD_TECNICO == CD_TECNICO).ToList();

            _osPadraoService.AdicionarValoresPadraoIncluirOs(osPadrao, Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).usuario.nidUsuario));

            return View(osPadrao);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(OsPadrao osPadrao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //bool existeOS = _osPadraoService.ValidarDataHoraOS(osPadrao);

                    VisitaPadrao visitaPadrao = new VisitaPadrao();
                    visitaPadrao.CodigoTecnico = osPadrao.CodigoTecnico;

                    if (osPadrao.CodigoTecnico == null && osPadrao.ID_OS != null && osPadrao.ID_OS != 0)
                    {
                        visitaPadrao.CodigoTecnico = _osPadraoService.ObterCod_Tecnico(osPadrao.ID_OS);
                    }
                    visitaPadrao.HR_FIM = osPadrao.HR_FIM;
                    visitaPadrao.HR_INICIO = osPadrao.HR_INICIO;
                    visitaPadrao.DT_DATA_VISITA = osPadrao.DT_DATA_OS.ToString();
                    visitaPadrao.TpStatusVisita.ST_STATUS_VISITA = 3;

                    bool existeOS = _osPadraoService.ValidarDataHoraOS(osPadrao);

                    bool existeVisita = _visitaPadraoService.ValidarDataHoraVisita(visitaPadrao);

                    if (existeOS == true || existeVisita == true)
                    {
                        //throw new Exception("Já existe Ordem de Serviço ou Visita Registrada nesse Período");
                        osPadrao.JavaScriptToRun = "MensagemJaExiste()";
                    }
                    else if (osPadrao.ativoFixo.CD_ATIVO_FIXO == null || osPadrao.ativoFixo.CD_ATIVO_FIXO == "")
                    {
                        osPadrao.JavaScriptToRun = "MensagemAtivo()";
                    }
                    else
                    {
                        OSPadraoEntity osPadraoEntity = new OSPadraoEntity();

                        _osPadraoService.InformarStatus(osPadrao);

                        _osPadraoService.MapearCamposIncluirOs(osPadrao, osPadraoEntity, CurrentUser.usuario.nidUsuario);

                        osPadraoEntity.Criado = "Web";

                        osPadraoEntity.Origem = "W";

                        new OSPadraoData().Inserir(ref osPadraoEntity);

                        osPadrao.idKey = ControlesUtility.Criptografia.Criptografar(Convert.ToString(osPadraoEntity.ID_OS));

                        osPadrao.JavaScriptToRun = osPadrao.TpStatusOS.ST_STATUS_OS == Convert.ToInt64(ControlesUtility.Enumeradores.TpStatusOSPadrao.Aberta)
                            ? "MensagemSucessoOsAberta()" : "MensagemSucessoAguardandoInicio()";
                    }
                    
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            PopularListas(osPadrao);

            return View(osPadrao); 
        }

        

        [_3MAuthentication]
        public ActionResult Visualizar(string idKey)
        {
            return View(Obter(idKey));
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            DefineAcessosPerfisViewBag();

            return View(Obter(idKey));
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(OsPadrao osPadrao)
        {
            try
            {
                bool existeOS = _osPadraoService.ValidarDataHoraOS(osPadrao);

                VisitaPadrao visitaPadrao = new VisitaPadrao();

                visitaPadrao.CodigoTecnico = osPadrao.CodigoTecnico;
                if (osPadrao.CodigoTecnico == null && osPadrao.ID_OS != null && osPadrao.ID_OS != 0)
                {
                    visitaPadrao.CodigoTecnico = _osPadraoService.ObterCod_Tecnico(osPadrao.ID_OS);
                }
                visitaPadrao.HR_FIM = osPadrao.HR_FIM;
                visitaPadrao.HR_INICIO = osPadrao.HR_INICIO;
                visitaPadrao.DT_DATA_VISITA = osPadrao.DT_DATA_OS.ToString();
                visitaPadrao.TpStatusVisita.ST_STATUS_VISITA = 3;

                bool existeVisita = _visitaPadraoService.ValidarDataHoraVisita(visitaPadrao);

                if (existeOS == true || existeVisita == true)
                {
                    //throw new Exception("Já existe Ordem de Serviço ou Visita Registrada nesse Período");
                    osPadrao.JavaScriptToRun = "MensagemJaExiste()";
                }
                else
                {
                    if (osPadrao.TpStatusOS.ST_STATUS_OS == 3)
                    {

                        List<PecaOSSinc> pecasOS = _osPadraoService.ObterListaPecaOSFinalizar(osPadrao.ID_OS);

                        long CD_Cli = _osPadraoService.ObterCod_Cliente(osPadrao.ID_OS);

                        string CD_TEC = _osPadraoService.ObterCod_Tecnico(osPadrao.ID_OS);

                        OSPadraoEntity oSEntity = new OSPadraoEntity();
                        oSEntity.ID_OS = osPadrao.ID_OS;
                        oSEntity.Tecnico.CD_TECNICO = CD_TEC;
                        oSEntity.Cliente.CD_CLIENTE = CD_Cli;

                        List<ListaPendenciaOS> listaPendenciaOS = _osPadraoService.ObterListaPendenciaOS(CD_Cli, osPadrao.ID_OS, osPadrao.tecnico.CD_TECNICO);

                        if (listaPendenciaOS != null)
                        {
                            listaPendenciaOS = listaPendenciaOS.Where(x => x.PENDENCIA_OS == osPadrao.ID_OS).ToList();
                            foreach (var Pen in listaPendenciaOS)
                            {
                                new OSPadraoData().RealizarAtualizacaoStatusPendenciaFinalizada(Pen.ID_PENDENCIA_OS);
                            }
                        }

                        if (pecasOS != null)
                        {
                            foreach (var it in pecasOS)
                            {
                                new OSPadraoData().RealizarAtualizacaoEstoque(it, oSEntity, CurrentUser.usuario.nidUsuario, OPERACAO_ESTOQUE_SAIDA);
                            }

                        }

                        OSPadraoEntity OsEmail = new OSPadraoEntity();

                        _osPadraoService.MapearCamposOsPadraoParaOsPadraoEntity(osPadrao, OsEmail, CurrentUser.usuario.nidUsuario);

                        if (OsEmail.AtivoFixo.CD_ATIVO_FIXO == null && OsEmail.AtivoFixo.modelo.DS_MODELO == null)
                        {
                            var ativo = _osPadraoService.ObterOS(OsEmail.ID_OS);
                            OsEmail.AtivoFixo.CD_ATIVO_FIXO = ativo.ativoFixo.CD_ATIVO_FIXO;
                            OsEmail.AtivoFixo.modelo.DS_MODELO = ativo.ativoFixo.DS_ATIVO_FIXO;
                        }

                        if (OsEmail.Tecnico.CD_TECNICO == null)
                        {
                            OsEmail.Tecnico.CD_TECNICO = _osPadraoService.ObterCod_Tecnico(OsEmail.ID_OS);
                        }

                        if (OsEmail.Cliente.CL_CLIENTE == null)
                        {
                            OsEmail.Cliente.CD_CLIENTE = _osPadraoService.ObterCod_Cliente(OsEmail.ID_OS);
                        }

                        SincronismoData sincronismoData = new SincronismoData();
                        Int64? idOs = null;
                        if (osPadrao.ID_OS>0)
                        {
                            idOs = osPadrao.ID_OS;
                        }
                        sincronismoData.EnviarEmailOS(OsEmail, idOs);
                    }

                    else
                    {
                        osPadrao.pecaOS = null;
                    }

                    osPadrao.pendenciaOS = null;

                    ModelState.Clear();
                    if (ModelState.IsValid)
                    {
                        OSPadraoEntity osPadraoEntity = new OSPadraoEntity();

                        _osPadraoService.InformarStatus(osPadrao);

                        _osPadraoService.MapearCamposOsPadraoParaOsPadraoEntity(osPadrao, osPadraoEntity, CurrentUser.usuario.nidUsuario);

                        new OSPadraoData().Alterar(osPadraoEntity);

                        osPadrao.JavaScriptToRun = "MensagemSucesso()";

                        return RedirectToAction("Index");
                    }
                }

                
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            PopularListas(osPadrao);

            return View(osPadrao);
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            DefineAcessosPerfisViewBag();

            var osPadrao = Obter(idKey);

           _osPadraoService.ValidarExcluirOs(osPadrao);

            return View(osPadrao);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            OsPadrao osPadrao = new OsPadrao();

            try
            {
                if (ModelState.IsValid)
                {
                    OSPadraoEntity osPadraoEntity = new OSPadraoEntity
                    {
                        ID_OS = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey)),
                        nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario
                    };

                    new OSPadraoData().Excluir(osPadraoEntity);

                    osPadrao.JavaScriptToRun = "MensagemSucesso()";
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(osPadrao);
        }

        public ActionResult Cancelar(string idKey)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                var osPadrao = Obter(idKey);

                OSPadraoEntity osPadraoEntity = new OSPadraoEntity();

                osPadrao.TpStatusOS.ST_STATUS_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOSPadrao.Cancelada);

                if (osPadrao.TpStatusOS.ST_STATUS_OS == 4)
                {
                    List<PecaOSSinc> pecasOS = _osPadraoService.ObterListaPecaOSFinalizar(osPadrao.ID_OS);

                    long CD_Cli = _osPadraoService.ObterCod_Cliente(osPadrao.ID_OS);

                    OSPadraoEntity oSEntity = new OSPadraoEntity();
                    oSEntity.ID_OS = osPadrao.ID_OS;
                    oSEntity.Tecnico.CD_TECNICO = osPadrao.tecnico.CD_TECNICO;
                    oSEntity.Cliente.CD_CLIENTE = CD_Cli;

                    List<ListaPendenciaOS> listaPendenciaOS = _osPadraoService.ObterListaPendenciaOS(CD_Cli, osPadrao.ID_OS, osPadrao.tecnico.CD_TECNICO);
                    List<ReclamacaoOs> listaReclamacaoOs = _osPadraoService.ObterListaReclamacaoOS(osPadrao.ID_OS);

                    if (listaPendenciaOS != null)
                    {
                        listaPendenciaOS = listaPendenciaOS.Where(x => x.PENDENCIA_OS == osPadrao.ID_OS).ToList();
                        foreach (var Pen in listaPendenciaOS)
                        {
                            new OSPadraoData().RealizarAtualizacaoStatusPendenciaCancelada(Pen.ID_PENDENCIA_OS);
                        }
                    }

                    if (pecasOS != null)
                    {
                        foreach (var it in pecasOS)
                        {
                            new OSPadraoData().RealizarAtualizacaoEstoque(it, oSEntity, CurrentUser.usuario.nidUsuario, OPERACAO_ESTOQUE_ENTRADA);
                        }

                    }

                    if (listaReclamacaoOs != null)
                    {
                        new OSPadraoData().CancelarReclamacaoOS(osPadrao.ID_OS);
                    }
                }

                _osPadraoService.MapearCamposOsPadraoParaOsPadraoEntity(osPadrao, osPadraoEntity, CurrentUser.usuario.nidUsuario);

                new OSPadraoData().Alterar(osPadraoEntity);

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

        public JsonResult ValidarPermiteIncluirOs(string codTecnico)
        {
            
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                if (!ValidarExisteOsEmAndamento(codTecnico, jsonResult))
                    ValidarExisteVisitaEmAndamento(codTecnico, jsonResult);

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

                if(OSAguardandoInicio?.Count > 0)
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
            if (!_osPadraoService.VerificarExisteVisita(codTecnico, ControlesUtility.Enumeradores.TpStatusVisitaPadrao.Iniciar))
            {
                if (_osPadraoService.VerificarExisteVisita(codTecnico, ControlesUtility.Enumeradores.TpStatusVisitaPadrao.Aberta) && CurrentUser.perfil.nidPerfil != (int)ControlesUtility.Enumeradores.Perfil.Administrador3M)
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
            UsuarioEntity usuarioEntity = new UsuarioEntity();
            usuarioEntity.nidUsuario = CurrentUser.usuario.nidUsuario;
            
            if (!_osPadraoService.VerificarExisteOs(codTecnico, ControlesUtility.Enumeradores.TpStatusOSPadrao.AguardandoInicio))
            {
                if (_osPadraoService.VerificarExisteOs(codTecnico, ControlesUtility.Enumeradores.TpStatusOSPadrao.Aberta) && CurrentUser.perfil.nidPerfil != (int)ControlesUtility.Enumeradores.Perfil.Administrador3M)
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

        public JsonResult ObterListaPecaOSJson(Int64 ID_OS, bool visualizarOS)
        {
            DefineAcessosPerfisViewBag(visualizarOS);

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<PecaOS> listaPecaOS = _osPadraoService.ObterListaPecaOS(ID_OS);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/OsPadrao/_gridMVCPecaOS.cshtml", listaPecaOS));
                jsonResult.Add("Status", "Success");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }

        
        public JsonResult ObterListaReclamacaoOSJson(Int64 ID_OS, bool visualizarOS)
        {
            DefineAcessosPerfisViewBag(visualizarOS);

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<ReclamacaoOs> listaReclamacaoOs = _osPadraoService.ObterListaReclamacaoOS(ID_OS);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/OsPadrao/_gridMVCReclamacaoOS.cshtml", listaReclamacaoOs));
                jsonResult.Add("Status", "Success");
                jsonResult.Add("lista", listaReclamacaoOs);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }

        public JsonResult ObterListaPendenciaOSJson(Int64 CD_CLIENTE, Int64 ID_OS, bool visualizarOS, string CD_TECNICO)
        {
            DefineAcessosPerfisViewBag(visualizarOS);

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<ListaPendenciaOS> listaPendenciaOS = _osPadraoService.ObterListaPendenciaOS(CD_CLIENTE, ID_OS, CD_TECNICO);

                listaPendenciaOS = listaPendenciaOS.Where(x => x.ST_STATUS_PENDENCIA == "1" && ID_OS >= x.PENDENCIA_OS).ToList();

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/OsPadrao/_gridMVCPendenciaOS.cshtml", listaPendenciaOS));
                jsonResult.Add("Status", "Success");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }

        protected void DefineAcessosPerfisViewBag(bool visualizarOs = false)
        {
            ViewBag.Administrador3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M));
            ViewBag.Tecnico3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Tecnico3M));
            ViewBag.Visualizar = visualizarOs;

            if (ViewBag.Tecnico3M == NAO)
            {
                ViewBag.Tecnico3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica));
            }

            if (ViewBag.Tecnico3M == NAO)
            {
                ViewBag.Tecnico3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira));
            }

            if (ViewBag.Tecnico3M == NAO)
            {
                ViewBag.Tecnico3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M));
            }
        }

        public JsonResult ValidarPermiteIncluirOsSalvar(string codTecnico, int idOs, string dataOs, string horaInicio, string horaFim)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                if (_osPadraoService.VerificarExisteOsDataHora(codTecnico, idOs, dataOs, horaInicio, horaFim))
                    jsonResult.Add("Resultado", "TemOS");
                else if (_osPadraoService.VerificarExisteVisitaDataHora(codTecnico, dataOs, horaInicio, horaFim))
                    jsonResult.Add("Resultado", "TemOS");

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

        private void PopularListas(OsPadrao osPadrao)
        {
            osPadrao.ativos = osPadrao.cliente.CD_CLIENTE > 0 ? ObterListaAtivoCliente(Convert.ToInt32(osPadrao.cliente.CD_CLIENTE), true) : new List<ListaAtivoCliente>();
            osPadrao.tiposStatusOsPadrao = ObterStatusOsPadrao();
            osPadrao.tiposOsPadrao = ObterTipoOsPadrao();
            if (osPadrao.CodigoTecnico != null && osPadrao.CodigoTecnico != "")
            {
                TecnicoSinc tecnico = new TecnicoData().ObterTecnicoOS(osPadrao.CodigoTecnico).FirstOrDefault();
                osPadrao.clientes = ObterListaClientePorUsuarioPerfil(Convert.ToInt64(tecnico.ID_USUARIO), true);
            }else
                osPadrao.clientes = ObterListaClientePorUsuarioPerfil(Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).usuario.nidUsuario), true);
            osPadrao.tecnicos = ObterListaTecnicosAtivos(Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).usuario.nidUsuario));
            osPadrao.tecnicos.RemoveAt(PRIMEIRO_ITEM_LISTA);
            osPadrao.regioes = ObterListaRegiao();
        }

        private OsPadrao Obter(string idKey)
        {
            var osPadrao = _osPadraoService.Obter(idKey);
            
            if (osPadrao != null)
            {
                PopularListas(osPadrao);
                osPadrao.ativoFixo.DS_ATIVO_FIXO_COMPLETA = osPadrao.ativoFixo.CD_ATIVO_FIXO + " - " + osPadrao.ativoFixo.DS_ATIVO_FIXO;
                osPadrao.cliente.NM_CLIENTE_Codigo = osPadrao.cliente.NM_CLIENTE + " - " + osPadrao.cliente.CD_CLIENTE;
            }
             
            return osPadrao;
        }

        public ActionResult Pesquisa(Int64 ID_OS)
        {
            ViewBag.Visita = ID_OS;

            var visita = _osPadraoService.ObterOS(ID_OS);

            ViewBag.NomeTecnico = visita.tecnico.NM_TECNICO;
            var DataVisita = Convert.ToDateTime(visita.DT_DATA_OS).ToString("dd/MM/yyyy");
            ViewBag.DataVisita = DataVisita.ToString();

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
                        var visita = _osPadraoService.ObterOS(visitaResposta.ID_OS);

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

                    visitaResposta.ID_VISITA = 0;

                    VisitaRespostaEntity respost = new VisitaRespostaEntity();

                    _visitaPadraoService.MapearCamposVisitaRespostaParaVisitaRespostaEntity(visitaResposta, respost);

                    var avaliacao = new VisitaPadraoData().ObterListaRespostasOS(respost);

                    if (avaliacao?.Count == 0)
                    {
                        new VisitaPadraoData().InserirPesquisa(respost);
                        jsonResult.Add("Status", "Success");
                    }
                    else
                    {
                        jsonResult.Add("Error", "Já foi feita uma avaliação a esta O.S!");
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