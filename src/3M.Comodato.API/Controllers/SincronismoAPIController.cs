using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using static _3M.Comodato.Entity.SincronismoEntity;
using ControlesUtility = _3M.Comodato.Utility.ControlesUtility;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/SincronismoAPI")]
    [Authorize]
    public class SincronismoAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("ObterListaSincronismoSinc")]
        public IHttpActionResult ObterListaSincronismoSinc(Int64 idUsuario)
        {
            IList<LogSincronismoEntity> listaSincronismo = new List<LogSincronismoEntity>();

            try
            {
                SincronismoData sincronismoData = new SincronismoData();
                listaSincronismo = sincronismoData.ObterListaSincronismoSinc(idUsuario);

                JObject JO = new JObject
                {
                    { "SINCRONISMO", JArray.FromObject(listaSincronismo) }
                };

                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("LimparLogSincronismoUsuario")]
        public IHttpActionResult LimparLogSincronismoUsuario(Int64 idUsuario)
        {
            try
            {
                SincronismoData sincronismoData = new SincronismoData();
                sincronismoData.LimparLogSincronismoUsuario(idUsuario);
                return Ok();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Enviar Listas diversas para gravação no Server (Mobile->Server) durante Sincronismo
        /// </summary>
        /// <param name="JO">JObject contendo Listas (PEDIDO, PEDIDO_PECA, ESTOQUE_MOVI, ESTOQUE_PECA, AGENDA, VISITA, USUARIO, 
        /// LOG_STATUS_VISITA, OS, PECA_OS, PENDENCIA_OS, LOG_STATUS_OS</param>
        /// <param name="idUsuario">id do usuario tecnico, dono do mobile</param>
        /// <returns></returns>
        [HttpPost]
        [Route("GravarDiversosSinc")]
        public IHttpActionResult GravarDiversosSinc(JObject JO, Int64 idUsuario)
        {

            SincronismoData sincData = new SincronismoData();
            Int64 id_log_sincronismo = 0;
            id_log_sincronismo = sincData.GravaLogSincronismo(idUsuario, Convert.ToString(JO));

            try
            {
                List<PedidoSinc> listaPedido = new List<PedidoSinc>();
                listaPedido = JsonConvert.DeserializeObject<List<PedidoSinc>>(JO["PEDIDO"] is null ? "" : JO["PEDIDO"].ToString());

                List<PedidoPecaSinc> listaPedidoPeca = new List<PedidoPecaSinc>();
                listaPedidoPeca = JsonConvert.DeserializeObject<List<PedidoPecaSinc>>(JO["PEDIDO_PECA"] is null ? "" : JO["PEDIDO_PECA"].ToString());

                List<PedidoPecaSinc> listaPedidoPecaRecebimento = new List<PedidoPecaSinc>();
                listaPedidoPecaRecebimento = JsonConvert.DeserializeObject<List<PedidoPecaSinc>>(JO["PEDIDO_PECA_RECEBIMENTO"] is null ? "" : JO["PEDIDO_PECA_RECEBIMENTO"].ToString());

                List<PedidoPecaLogSinc> listaPedidoPecaLog = new List<PedidoPecaLogSinc>();
                listaPedidoPecaLog = JsonConvert.DeserializeObject<List<PedidoPecaLogSinc>>(JO["PEDIDO_PECA_LOG"] is null ? "" : JO["PEDIDO_PECA_LOG"].ToString());

                List<EstoqueMoviSinc> listaEstoqueMovi = new List<EstoqueMoviSinc>();
                listaEstoqueMovi = JsonConvert.DeserializeObject<List<EstoqueMoviSinc>>(JO["ESTOQUE_MOVI"] is null ? "" : JO["ESTOQUE_MOVI"].ToString());

                List<EstoquePecaSinc> listaEstoquePeca = new List<EstoquePecaSinc>();
                listaEstoquePeca = JsonConvert.DeserializeObject<List<EstoquePecaSinc>>(JO["ESTOQUE_PECA"] is null ? "" : JO["ESTOQUE_PECA"].ToString());

                List<AgendaSinc> listAgenda = new List<AgendaSinc>();
                listAgenda = JsonConvert.DeserializeObject<List<AgendaSinc>>(JO["AGENDA"] is null ? "" : JO["AGENDA"].ToString());

                List<VisitaSinc> listVisita = new List<VisitaSinc>();
                listVisita = JsonConvert.DeserializeObject<List<VisitaSinc>>(JO["VISITA"] is null ? "" : JO["VISITA"].ToString());

                List<VisitaPadraoEntity> listVisitaPadrao = new List<VisitaPadraoEntity>();
                listVisitaPadrao = JsonConvert.DeserializeObject<List<VisitaPadraoEntity>>(JO["VISITAPADRAO"] is null ? "" : JO["VISITAPADRAO"].ToString());
                ValidarVisita(listVisitaPadrao);

                List<LogStatusVisitaSinc> listLogStatusVisita = new List<LogStatusVisitaSinc>();
                listLogStatusVisita = JsonConvert.DeserializeObject<List<LogStatusVisitaSinc>>(JO["LOG_STATUS_VISITA"] is null ? "" : JO["LOG_STATUS_VISITA"].ToString());

                List<OsSinc> listOS = new List<OsSinc>();
                listOS = JsonConvert.DeserializeObject<List<OsSinc>>(JO["OS"] is null ? "" : JO["OS"].ToString());

                List<OSPadraoEntity> listOSPadrao = new List<OSPadraoEntity>();
                listOSPadrao = JsonConvert.DeserializeObject<List<OSPadraoEntity>>(JO["OSPADRAO"] is null ? "" : JO["OSPADRAO"].ToString());
                listOSPadrao = listOSPadrao.OrderBy(x => x.TOKEN).ToList();
                ValidarOs(listOSPadrao);

                List<RelatorioReclamacaoSincEntity> listRR = new List<RelatorioReclamacaoSincEntity>();
                listRR = JsonConvert.DeserializeObject<List<RelatorioReclamacaoSincEntity>>(JO["RR"] is null ? "" : JO["RR"].ToString());

                List<RRComentSincEntity> listRRComent = new List<RRComentSincEntity>();
                listRRComent = JsonConvert.DeserializeObject<List<RRComentSincEntity>>(JO["RRCOMENT"] is null ? "" : JO["RRCOMENT"].ToString());

                List<PecaOSSinc> listPecaOS = new List<PecaOSSinc>();
                listPecaOS = JsonConvert.DeserializeObject<List<PecaOSSinc>>(JO["PECA_OS"] is null ? "" : JO["PECA_OS"].ToString());

                List<PendenciaOSSinc> listPendenciaOS = new List<PendenciaOSSinc>();
                listPendenciaOS = JsonConvert.DeserializeObject<List<PendenciaOSSinc>>(JO["PENDENCIA_OS"] is null ? "" : JO["PENDENCIA_OS"].ToString());

                List<PendenciaOSSinc> listPendenciaOSOutros = new List<PendenciaOSSinc>();
                listPendenciaOSOutros = JsonConvert.DeserializeObject<List<PendenciaOSSinc>>(JO["PENDENCIA_OS_OUTROS"] is null ? "" : JO["PENDENCIA_OS_OUTROS"].ToString());

                List<LogStatusOSSinc> listLogStatusOS = new List<LogStatusOSSinc>();
                listLogStatusOS = JsonConvert.DeserializeObject<List<LogStatusOSSinc>>(JO["LOG_STATUS_OS"] is null ? "" : JO["LOG_STATUS_OS"].ToString());

                List<int> listCodigosNotificacao = new List<int>();
                listCodigosNotificacao = JsonConvert.DeserializeObject<List<int>>(JO["CODIGO_NOTIFICACAO"] is null ? "" : JO["CODIGO_NOTIFICACAO"].ToString());

                //Grava imagens disponíveis em disco e, atualiza nome do arquivo
                string diretorio = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.CaminhoUpload) + ControlesUtility.Constantes.PastaFotosPecasSincronismo;
                if (listaPedidoPeca != null)
                    listaPedidoPeca.Where(c => !string.IsNullOrEmpty(c.DS_DIR_FOTO) && string.IsNullOrEmpty(c.ID_ITEM_PEDIDO.ToString())).ToList()
                        .ForEach(c => c.DS_DIR_FOTO = ControlesUtility.ImageUtility.SaveImage(diretorio, c.DS_DIR_FOTO));

                sincData.GravarDiversosSinc(idUsuario, listAgenda, listVisita, listVisitaPadrao, listLogStatusVisita, listOS, listOSPadrao, listPecaOS, listPendenciaOS,
                    listLogStatusOS, listaEstoqueMovi, ref listaPedido, listaPedidoPeca, listRR, listRRComent, listCodigosNotificacao, listPendenciaOSOutros, listaPedidoPecaLog, 
                    listaPedidoPecaRecebimento);
               

                if (listVisita != null)
                    listVisita.Where(c => c.ID_VISITA is null && c.ST_TP_STATUS_VISITA_OS == 3).ToList().ForEach(v => new AnaliseSatisfacaoAPIController().NotificarAvaliacaoDisponivel(v));

                string resultadoSincronismo = "sucesso";
                sincData.AtualizaLogSincronismo(id_log_sincronismo, resultadoSincronismo);

                if (listaPedido != null && listaPedido.Count > 0)
                    EnviarEmailPedido(listaPedido, "SOLICITADO");

                return Ok(resultadoSincronismo);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                sincData.AtualizaLogSincronismo(id_log_sincronismo, "ERRO:" + ex.Message);
                if (ex.Message.Contains("Visita"))
                {
                    //return Unauthorized();
                    return StatusCode(System.Net.HttpStatusCode.PaymentRequired);
                }
                else
                {
                    return BadRequest(ex.Message);
                }
                
            }

        }

        private void ValidarOs(List<OSPadraoEntity> listOSPadrao)
        {
            if (listOSPadrao?.Count > 0)
            {
                var ContaOSAberta = listOSPadrao.Where(x => x.TpStatusOS.ST_STATUS_OS == 2).ToList();
                if (ContaOSAberta?.Count > 1)
                {
                    throw new Exception("Você já tem Visita ou Ordem de Serviço registrada neste período, corrija as informações deste formulário, se necessário");
                }
                
                foreach (var osPadraoEntity in listOSPadrao)
                {
                    var OsVerifica = osPadraoEntity;
                    OsVerifica.Tecnico.CD_TECNICO = osPadraoEntity.Tecnico.CD_TECNICO;
                    var verificaQTDOS = new OSPadraoData().ObterListaOSSincAbertas(OsVerifica, 2);
                    int contaOS = 0;

                    if (verificaQTDOS.Count > 1)
                    {
                        throw new Exception("Você já tem Visita ou Ordem de Serviço registrada neste período, corrija as informações deste formulário, se necessário");
                    }
                    else if (verificaQTDOS?.Count == 1)
                    {
                        //Verifica se a OS aberta esta na lista passada
                        var OSValidar = verificaQTDOS.FirstOrDefault();
                        
                        foreach(var os in listOSPadrao)
                        {
                            if(os.TOKEN == OSValidar.TOKEN)
                            {
                                contaOS += 1;
                            }
                        }

                        if (contaOS == 0)
                        {
                            throw new Exception("Você já tem Visita ou Ordem de Serviço registrada neste período, corrija as informações deste formulário, se necessário");
                        }
                    }
                    else
                    {
                        //Verifica se a OS aberta esta na lista passada
                        var OSValidar = verificaQTDOS.FirstOrDefault();
                        int OsEstaNaLista = 0;

                        foreach (var Ordem in listOSPadrao)
                        {
                            var OS = verificaQTDOS.Where(x => x.ID_OS == Ordem.ID_OS || x.TOKEN == Ordem.TOKEN).ToList();
                            if (OS != null)
                                OsEstaNaLista += 1;
                        }

                        if (OsEstaNaLista == 0)
                        {
                            throw new Exception("Você já tem Visita ou Ordem de Serviço registrada neste período, corrija as informações deste formulário, se necessário");
                        }
                    }

                    if (ValidarPermiteIncluirOsSalvar(osPadraoEntity))
                    {
                        throw new Exception("Você já tem Visita ou Ordem de Serviço registrada neste período, corrija as informações deste formulário, se necessário");
                    }
                    
                }
            }
        }

        protected void EnviarEmailOS(OSPadraoEntity listOSPadrao)
        {
            //Método Modelo envio de email
            try
            {
                //string emailsSolicitacaoPeca = ControlesUtility.Parametro.ObterValorParametro("emailsOSFinalizada");

                
                if (listOSPadrao.TpStatusOS.ST_STATUS_OS == 3)
                {
                    TecnicoEntity tecnicoEntity = new TecnicoEntity();
                    tecnicoEntity.CD_TECNICO = listOSPadrao.Tecnico.CD_TECNICO;
                    DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            tecnicoEntity.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                            tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                            tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                            tecnicoEntity.usuario.cdsEmail = "";
                            tecnicoEntity.usuarioCoordenador.cdsEmail = "";
                            if (dataTableReader["cdsEmail"] != DBNull.Value)
                            {
                                tecnicoEntity.usuario.cdsEmail = dataTableReader["cdsEmail"].ToString();
                            }
                            if (dataTableReader["cdsEmailCoordenador"] != DBNull.Value)
                            {
                                tecnicoEntity.usuarioCoordenador.cdsEmail = dataTableReader["cdsEmailCoordenador"].ToString();
                            }
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    ClienteEntity _clienteEntity = new ClienteEntity();
                    _clienteEntity.CD_CLIENTE = listOSPadrao.Cliente.CD_CLIENTE;
                    dataTableReader = new ClienteData().ObterLista(_clienteEntity).CreateDataReader();

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


                    // Envia a requisição de troca de senha por e-mail

                    MailSender mailSender = new MailSender();

                    string mailTo = "";

                    if (tecnicoEntity.usuario.cdsEmail != "")
                    {
                        mailTo += tecnicoEntity.usuario.cdsEmail + ";";
                    }
                    if (_clienteEntity.TX_EMAIL != "")
                    {
                        mailTo += _clienteEntity.TX_EMAIL + ";";
                    }

                    string mailSubject = "3M.Comodato - Finalização de OS";
                    string mailMessage = string.Empty;
                    System.Net.Mail.Attachment Attachments = null;
                    string mailCopy = null;

                    var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                    string Conteudo = string.Empty;
                    string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                    //URLSite += "/SolicitacaoPecas/Editar?idKey=" + ControlesUtility.Criptografia.Criptografar(tecnicoEntity.CD_TECNICO + "|" + pedidoEntity.ID_PEDIDO.ToString() + "|" + pedidoEntity.TP_TIPO_PEDIDO + "|Solicitação");

                    Conteudo += "<p>Uma OS acaba de ser <strong>Finalizada </strong>!</p>";
                    Conteudo += "<p>Segue dados da OS:</p>";
                    Conteudo += "<p>Técnico: " + tecnicoEntity.NM_TECNICO + " - " + tecnicoEntity.empresa.NM_Empresa + "<br/>";
                    if (listOSPadrao.ID_OS != 0)
                    {
                        Conteudo += "OS: <strong>" + Convert.ToInt64(listOSPadrao.ID_OS) + "</strong><br/>";
                    }
                    
                    Conteudo += "Data: " + Convert.ToDateTime(listOSPadrao.DT_DATA_OS).ToString("dd/MM/yyyy") + "<br/>";
                    Conteudo += "Status da OS: Finalizada<br/>";
                        
                    MensagemEmail.Replace("#Conteudo", Conteudo);
                    mailMessage = MensagemEmail.ToString();

                    mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);
                }
                

                
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                
            }
        }

        protected void EnviarEmailVisita(VisitaPadraoEntity visitaPadrao)
        {
            //Método Modelo envio de email
            try
            {
                //string emailsSolicitacaoPeca = ControlesUtility.Parametro.ObterValorParametro("emailsOSFinalizada");

                
                if (visitaPadrao.TpStatusVisita.ST_STATUS_VISITA == 4)
                {
                    TecnicoEntity tecnicoEntity = new TecnicoEntity();
                    tecnicoEntity.CD_TECNICO = visitaPadrao.Tecnico.CD_TECNICO;
                    DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            tecnicoEntity.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                            tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                            tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                            tecnicoEntity.usuario.cdsEmail = "";
                            tecnicoEntity.usuarioCoordenador.cdsEmail = "";
                            if (dataTableReader["cdsEmail"] != DBNull.Value)
                            {
                                tecnicoEntity.usuario.cdsEmail = dataTableReader["cdsEmail"].ToString();
                            }
                            if (dataTableReader["cdsEmailCoordenador"] != DBNull.Value)
                            {
                                tecnicoEntity.usuarioCoordenador.cdsEmail = dataTableReader["cdsEmailCoordenador"].ToString();
                            }
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    ClienteEntity _clienteEntity = new ClienteEntity();
                    _clienteEntity.CD_CLIENTE = visitaPadrao.Cliente.CD_CLIENTE;
                    dataTableReader = new ClienteData().ObterLista(_clienteEntity).CreateDataReader();

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


                    // Envia a requisição de troca de senha por e-mail

                    MailSender mailSender = new MailSender();

                    string mailTo = "";

                    if (tecnicoEntity.usuario.cdsEmail != "")
                    {
                        mailTo += tecnicoEntity.usuario.cdsEmail + ";";
                    }
                    if (_clienteEntity.TX_EMAIL != "")
                    {
                        mailTo += _clienteEntity.TX_EMAIL + ";";
                    }

                    string mailSubject = "3M.Comodato - Finalização de Visita";
                    string mailMessage = string.Empty;
                    System.Net.Mail.Attachment Attachments = null;
                    string mailCopy = null;

                    var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                    string Conteudo = string.Empty;
                    string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                    //URLSite += "/SolicitacaoPecas/Editar?idKey=" + ControlesUtility.Criptografia.Criptografar(tecnicoEntity.CD_TECNICO + "|" + pedidoEntity.ID_PEDIDO.ToString() + "|" + pedidoEntity.TP_TIPO_PEDIDO + "|Solicitação");

                    Conteudo += "<p>Uma Visita acaba de ser <strong>Finalizada </strong>!</p>";
                    Conteudo += "<p>Segue dados da Visita:</p>";
                    Conteudo += "<p>Técnico: " + tecnicoEntity.NM_TECNICO + " - " + tecnicoEntity.empresa.NM_Empresa + "<br/>";
                    if (visitaPadrao.ID_VISITA != 0)
                    {
                        Conteudo += "Visita: <strong>" + Convert.ToInt64(visitaPadrao.ID_VISITA) + "</strong><br/>";
                    }
                    
                    Conteudo += "Data: " + Convert.ToDateTime(visitaPadrao.DT_DATA_VISITA).ToString("dd/MM/yyyy") + "<br/>";
                    Conteudo += "Status da OS: Finalizada<br/>";

                    MensagemEmail.Replace("#Conteudo", Conteudo);
                    mailMessage = MensagemEmail.ToString();

                    mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);
                }
            


            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);

            }
        }

        private void ValidarVisita(List<VisitaPadraoEntity> listVisitaPadrao)
        {
            if (listVisitaPadrao?.Count > 0)
            {
                foreach (var visitaPadraoEntity in listVisitaPadrao)
                {
                    if (ValidarPermiteIncluirVisitaSalvar(visitaPadraoEntity))
                    {
                        throw new Exception("Você já tem Visita ou Ordem de Serviço registrada neste período, corrija as informações deste formulário, se necessário");
                    }
                    
                }
            }
        }

        protected void EnviarEmailPedido(List<PedidoSinc> listaPedido, string msgStatus)
            {
                //Método Modelo envio de email
                try
                {

                    foreach (PedidoSinc pedidoEntity in listaPedido)
                    {
                        if (!(pedidoEntity.ID_PEDIDO > 0))
                        {

                        string emailsSolicitacaoPeca = ControlesUtility.Parametro.ObterValorParametro("emailsSolicitacaoPeca");

                        if (string.IsNullOrEmpty(emailsSolicitacaoPeca))
                        {
                            return;
                        }

                        TecnicoEntity tecnicoEntity = new TecnicoEntity();
                        tecnicoEntity.CD_TECNICO = pedidoEntity.CD_TECNICO;
                        DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                        if (dataTableReader.HasRows)
                        {
                            if (dataTableReader.Read())
                            {
                                tecnicoEntity.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                                tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                                tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                                tecnicoEntity.usuario.cdsEmail = "";
                                tecnicoEntity.usuarioCoordenador.cdsEmail = "";
                                if (dataTableReader["cdsEmail"] != DBNull.Value)
                                {
                                    tecnicoEntity.usuario.cdsEmail = dataTableReader["cdsEmail"].ToString();
                                }
                                if (dataTableReader["cdsEmailCoordenador"] != DBNull.Value)
                                {
                                    tecnicoEntity.usuarioCoordenador.cdsEmail = dataTableReader["cdsEmailCoordenador"].ToString();
                                }
                            }
                        }

                        if (dataTableReader != null)
                        {
                            dataTableReader.Dispose();
                            dataTableReader = null;
                        }

                        PedidoEntity _pedidoEntity2 = new PedidoEntity();
                        _pedidoEntity2.ID_PEDIDO = Convert.ToInt64(pedidoEntity.ID_PEDIDO_INSERIDO);
                        dataTableReader = new PedidoData().ObterLista(_pedidoEntity2).CreateDataReader();

                        if (dataTableReader.HasRows)
                        {
                            if (dataTableReader.Read())
                            {
                                pedidoEntity.DT_CRIACAO = Convert.ToDateTime(dataTableReader["DT_CRIACAO"]);
                                pedidoEntity.CD_PEDIDO = Convert.ToInt64(dataTableReader["CD_PEDIDO"]);
                                //pedidoEntity.FL_EMERGENCIA = dataTableReader["FL_EMERGENCIA"].ToString();
                            }
                        }

                        if (dataTableReader != null)
                        {
                            dataTableReader.Dispose();
                            dataTableReader = null;
                        }

                        string msgEmergencia = "Não";
                        //if (pedidoEntity.FL_EMERGENCIA == "S")
                        //{
                        //    msgEmergencia = "<strong>SIM</strong>";
                        //}

                        // Envia a requisição de troca de senha por e-mail
                        MailSender mailSender = new MailSender();

                        string mailTo = emailsSolicitacaoPeca;
                        if (msgStatus != "SOLICITADO")
                        {
                            mailTo = "";
                            if (tecnicoEntity.usuario.cdsEmail != "")
                            {
                                mailTo += tecnicoEntity.usuario.cdsEmail + ";";
                            }
                            if (tecnicoEntity.usuarioCoordenador.cdsEmail != "")
                            {
                                mailTo += tecnicoEntity.usuarioCoordenador.cdsEmail + ";";
                            }
                            //if (mailTo != "")
                            //{
                            //    mailTo = mailTo.Remove(mailTo.Length - 1);
                            //}
                        }
                        string mailSubject = "3M.Comodato Mobile - Pedido de Peça ( " + msgStatus + " )";

#if (DEBUG == true)
                        mailSubject = "3M.Comodato Mobile [TESTE]- Pedido de Peça ( " + msgStatus + " )";
#endif


                        string mailMessage = string.Empty;
                        System.Net.Mail.Attachment Attachments = null;
                        string mailCopy = null;

                        var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                        string Conteudo = string.Empty;
                        string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                        //URLSite += "/SolicitacaoPecas/Editar?idKey=" + ControlesUtility.Criptografia.Criptografar(tecnicoEntity.CD_TECNICO + "|" + pedidoEntity.ID_PEDIDO.ToString() + "|" + pedidoEntity.TP_TIPO_PEDIDO + "|Solicitação");

                        Conteudo += "<p>Um Pedido de Peças acaba de ser <strong>" + msgStatus + "</strong>!</p>";
                        Conteudo += "<p>Segue dados do pedido:</p>";
                        Conteudo += "<p>Técnico: " + tecnicoEntity.NM_TECNICO + " - " + tecnicoEntity.empresa.NM_Empresa + "<br/>";
                        Conteudo += "Pedido: <strong>" + Convert.ToInt64(pedidoEntity.CD_PEDIDO).ToString("000000") + "</strong><br/>";
                        Conteudo += "Data: " + Convert.ToDateTime(pedidoEntity.DT_CRIACAO).ToString("dd/MM/yyyy") + "<br/>";
                        Conteudo += "Status do pedido: " + msgStatus + "<br/>";
                        Conteudo += "Tipo: <strong>";
                        if (pedidoEntity.TP_TIPO_PEDIDO.ToString() == ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Value)
                        {
                            Conteudo += ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Key + "</strong><br/>";
                        }
                        else if (pedidoEntity.TP_TIPO_PEDIDO.ToString() == ControlesUtility.Dicionarios.TipoPedido().ToArray()[1].Value)
                        {
                            Conteudo += ControlesUtility.Dicionarios.TipoPedido().ToArray()[1].Key + "</strong><br/>";
                        }
                        else if (pedidoEntity.TP_TIPO_PEDIDO.ToString() == ControlesUtility.Dicionarios.TipoPedido().ToArray()[2].Value)
                        {
                            Conteudo += ControlesUtility.Dicionarios.TipoPedido().ToArray()[2].Key + "</strong><br/>";
                        }
                        Conteudo += "Emergência: " + msgEmergencia + "<br/></p>";
                        Conteudo += "<p>Por favor, clique no link abaixo ou copie seu conteúdo para o Navegador WEB:</p>";
                        Conteudo += "<p><a href='" + URLSite + "'></a>" + URLSite + "</p>";

                        MensagemEmail.Replace("#Conteudo", Conteudo);
                        mailMessage = MensagemEmail.ToString();


#if (DEBUG == true)
                        mailTo = "daniel.custodio-external@wayon.global;daniel_almeida_custodio@yahoo.com.br";
#endif


                        // mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);

                        Task.Run(() =>

                        mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy)

                        );

                        

                    }

                    }
                }
                catch (Exception ex)
                {
                    LogUtility.LogarErro("Mobile sincronismo - Não foi possível enviar e - mail para este pedido!" + ex.ToString());
                    //Mensagem = "Mobile sincronismo - Não foi possível enviar e-mail para este pedido!";
                }
            }
        
    }
}
