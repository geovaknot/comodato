using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using System.Linq;
using Newtonsoft.Json;
using System.Data;

namespace _3M.Comodato.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/OSPadraoAPI")]
    public class OSPadraoAPIController : BaseAPIController
    {
        private long _idLogSincronismo;
        private const string SINCRONIZADO_SUCESSO = "sucesso";

        [HttpPost]
        [Route("Incluir")]
        public IHttpActionResult Incluir(OSPadraoEntity osPadraoEntity)
        {
            var sincData = new SincronismoData();
            _idLogSincronismo = sincData.GravaLogSincronismo(osPadraoEntity.nidUsuarioAtualizacao, JsonConvert.SerializeObject(osPadraoEntity));

            var OsVerifica = osPadraoEntity;
            OsVerifica.Tecnico.CD_TECNICO = osPadraoEntity.Tecnico.CD_TECNICO;
            var verificaQTDOS = new OSPadraoData().ObterListaOSSincAbertas(OsVerifica, 2);
            
            if (verificaQTDOS.Count >= 1)
            {
                throw new Exception("Você já tem Visita ou Ordem de Serviço registrada neste período, corrija as informações deste formulário, se necessário");
            }

            if (osPadraoEntity.TOKEN <= 0)
                throw new Exception($"Token para integração de registro do cliente código {osPadraoEntity.Cliente.CD_CLIENTE} não informado.");

            if (ValidarPermiteIncluirOsSalvar(osPadraoEntity))
                throw new Exception("Você já tem Visita ou Ordem de Serviço registrada neste período, corrija as informações deste formulário, se necessário");

            try
            {
                osPadraoEntity.DT_DATA_OS = DateTime.Now;
                osPadraoEntity.Origem = "A";
                new OSPadraoData().Inserir(ref osPadraoEntity);

                JObject JO = new JObject
                {
                    { "SUCESSO", true },
                    { "MENSAGEM", "" },
                    { "ID_OS", osPadraoEntity.ID_OS },
                    { "TOKEN", osPadraoEntity.TOKEN }
                };

                
                sincData.AtualizaLogSincronismo(_idLogSincronismo, SINCRONIZADO_SUCESSO);
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                sincData.AtualizaLogSincronismo(_idLogSincronismo, "ERRO:" + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Alterar")]
        public IHttpActionResult Alterar(OSPadraoEntity osPadraoEntity)
        {
            var sincData = new SincronismoData();
            _idLogSincronismo = sincData.GravaLogSincronismo(osPadraoEntity.nidUsuarioAtualizacao, JsonConvert.SerializeObject(osPadraoEntity));

            try
            {
                if (ValidarPermiteIncluirOsSalvar(osPadraoEntity))
                    throw new Exception("Você já tem Visita ou Ordem de Serviço registrada neste período, corrija as informações deste formulário, se necessário");

                new OSPadraoData().Alterar(osPadraoEntity);

                if (osPadraoEntity.TpStatusOS.ST_STATUS_OS == Convert.ToInt64(ControlesUtility.Enumeradores.TpStatusOSPadrao.Finalizada))
                    EnviarEmailOS(osPadraoEntity, osPadraoEntity.ID_OS);

                JObject JO = new JObject
                {
                    { "SUCESSO", true },
                    { "MENSAGEM", "" },
                    { "ID_OS", osPadraoEntity.ID_OS }
                };

                sincData.AtualizaLogSincronismo(_idLogSincronismo, SINCRONIZADO_SUCESSO);
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                sincData.AtualizaLogSincronismo(_idLogSincronismo, "ERRO:" + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        public void EnviarEmailOS(OSPadraoEntity listOSPadrao, long? Id_Os)
        {
            //Método Modelo envio de email
            try
            {
                //string emailsSolicitacaoPeca = ControlesUtility.Parametro.ObterValorParametro("emailsOSFinalizada");

                Id_Os = listOSPadrao.ID_OS;
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
                            _clienteEntity.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
                            _clienteEntity.EmailsInfo = dataTableReader["EmailsInfo"].ToString();
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

                    var pecasOS = new PecaOSData().ObterListaPecaOsEmail(listOSPadrao.ID_OS);

                    if (listOSPadrao.Email == null || listOSPadrao.Email == "")
                    {
                        //if (tecnicoEntity.usuario.cdsEmail != "")
                        //{
                        //    mailTo += tecnicoEntity.usuario.cdsEmail + ";";
                        //}
                        if (_clienteEntity.TX_EMAIL != "")
                        {
                            mailTo += _clienteEntity.TX_EMAIL + ";";
                        }
                    }
                    else if (listOSPadrao.Email != null && listOSPadrao.Email != "")
                    {
                        if (listOSPadrao.Email == tecnicoEntity.usuario.cdsEmail)
                            mailTo += _clienteEntity.TX_EMAIL + ";";
                        else
                            mailTo += listOSPadrao.Email + ";";
                    }



                    string mailSubject = "3M.Comodato - Finalização de OS";
                    string mailMessage = string.Empty;
                    System.Net.Mail.Attachment Attachments = null;
                    string mailCopy = null;

                    var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                    string Conteudo = string.Empty;
                    string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                    //URLSite += "/SolicitacaoPecas/Editar?idKey=" + ControlesUtility.Criptografia.Criptografar(tecnicoEntity.CD_TECNICO + "|" + pedidoEntity.ID_PEDIDO.ToString() + "|" + pedidoEntity.TP_TIPO_PEDIDO + "|Solicitação");
                    URLSite += "/OsPadrao/Pesquisa?ID_OS=" + Id_Os;
                    Conteudo += "<p>Uma OS acaba de ser <strong>Finalizada </strong>!</p>";
                    Conteudo += "<p>Segue dados da OS:</p>";
                    Conteudo += "Cliente: " + _clienteEntity.CD_CLIENTE + " - " + _clienteEntity.NM_CLIENTE + "<br/>";
                    Conteudo += "Cidade: " + _clienteEntity.EN_CIDADE + "<br/>";
                    Conteudo += "Técnico: " + tecnicoEntity.NM_TECNICO + " - " + tecnicoEntity.empresa.NM_Empresa + "<br/>";

                    if (Id_Os != null && Id_Os != 0)
                    {
                        Conteudo += "OS: <strong>" + Convert.ToInt64(Id_Os) + "</strong><br/>";
                    }
                    else if (listOSPadrao.ID_OS != 0)
                    {
                        Conteudo += "OS: <strong>" + Convert.ToInt64(listOSPadrao.ID_OS) + "</strong><br/>";
                    }

                    Conteudo += "Data: " + Convert.ToDateTime(listOSPadrao.DT_DATA_OS).ToString("dd/MM/yyyy") + "<br/>";
                    Conteudo += "Hora Inicio: " + listOSPadrao.HR_INICIO.ToString() + "<br/>";
                    Conteudo += "Hora de Finalização: " + listOSPadrao.HR_FIM.ToString() + "<br/>";
                    Conteudo += "Ativo Fixo: " + listOSPadrao.AtivoFixo.CD_ATIVO_FIXO + "-" + listOSPadrao.AtivoFixo.modelo.DS_MODELO + "<br/>";
                    Conteudo += "Linha: " + listOSPadrao.NOME_LINHA + "<br/>";
                    Conteudo += "Observação: " + listOSPadrao.DS_OBSERVACAO + "<br/>";
                    if (listOSPadrao.DS_RESPONSAVEL != null)
                    {
                        Conteudo += "Acompanhante: " + listOSPadrao.DS_RESPONSAVEL.ToString() + "<br/>";
                    }
                    else
                    {
                        Conteudo += "Acompanhante: " + "" + "<br/>";
                    }

                    var manutencao = "";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 1)
                        manutencao = "Preventiva";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 2)
                        manutencao = "Corretiva";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 3)
                        manutencao = "Instalação";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 4)
                        manutencao = "Outros";
                    Conteudo += "Tipo de Manutenção: " + manutencao + "<br/>";
                    Conteudo += "Status da OS: Finalizada<br/><br/>";

                    if (pecasOS?.Count > 0)
                    {
                        Conteudo += "<p><strong>Peças Utilizadas: </strong></p>";
                        int contaPc = 0;
                        foreach (var peca in pecasOS)
                        {
                            contaPc++;
                            var estoque = peca.CD_TP_ESTOQUE_CLI_TEC == 'C' ? "Cliente" : "Intermediario";
                            Conteudo += $" {contaPc}. {peca.DS_PECA} || Quantidade: {peca.QT_PECA} || Estoque: {estoque}<br/>";
                        }
                    }


                    var button = mailSender.GetConteudoHTML("button.html");

                    button.Replace("TEXTO_URL", "Avalie o Atendimento");
                    button.Replace("URL", URLSite);

                    Conteudo += button;
                    //Conteudo += $"<a class='btn btn-danger m-1' href = '{URLSite}'>Avalie o Atendimento</a>";
                    MensagemEmail.Replace("#Conteudo", Conteudo);
                    mailMessage = MensagemEmail.ToString();

                    mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);

                    if (_clienteEntity.EmailsInfo != null || _clienteEntity.EmailsInfo != "")
                    {
                        EnviarEmailOSInfo(listOSPadrao, Id_Os);
                    }
                }



            }
            catch (Exception ex)
            {


            }
        }


        public void EnviarEmailOSInfo(OSPadraoEntity listOSPadrao, long? Id_Os)
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
                            _clienteEntity.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
                            _clienteEntity.EmailsInfo = dataTableReader["EmailsInfo"].ToString();
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

                    var pecasOS = new PecaOSData().ObterListaPecaOsEmail(listOSPadrao.ID_OS);

                    mailTo = _clienteEntity.EmailsInfo;

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
                    Conteudo += "Cliente: " + _clienteEntity.CD_CLIENTE + " - " + _clienteEntity.NM_CLIENTE + "<br/>";
                    Conteudo += "Cidade: " + _clienteEntity.EN_CIDADE + "<br/>";
                    Conteudo += "Técnico: " + tecnicoEntity.NM_TECNICO + " - " + tecnicoEntity.empresa.NM_Empresa + "<br/>";

                    if (Id_Os != null && Id_Os != 0)
                    {
                        Conteudo += "OS: <strong>" + Convert.ToInt64(Id_Os) + "</strong><br/>";
                    }
                    else if (listOSPadrao.ID_OS != 0)
                    {
                        Conteudo += "OS: <strong>" + Convert.ToInt64(listOSPadrao.ID_OS) + "</strong><br/>";
                    }

                    Conteudo += "Data: " + Convert.ToDateTime(listOSPadrao.DT_DATA_OS).ToString("dd/MM/yyyy") + "<br/>";
                    Conteudo += "Hora Inicio: " + listOSPadrao.HR_INICIO.ToString() + "<br/>";
                    Conteudo += "Hora de Finalização: " + listOSPadrao.HR_FIM.ToString() + "<br/>";
                    Conteudo += "Ativo Fixo: " + listOSPadrao.AtivoFixo.CD_ATIVO_FIXO + "-" + listOSPadrao.AtivoFixo.modelo.DS_MODELO + "<br/>";
                    Conteudo += "Linha: " + listOSPadrao.NOME_LINHA + "<br/>";
                    Conteudo += "Observação: " + listOSPadrao.DS_OBSERVACAO + "<br/>";
                    if (listOSPadrao.DS_RESPONSAVEL != null)
                    {
                        Conteudo += "Acompanhante: " + listOSPadrao.DS_RESPONSAVEL.ToString() + "<br/>";
                    }
                    else
                    {
                        Conteudo += "Acompanhante: " + "" + "<br/>";
                    }

                    var manutencao = "";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 1)
                        manutencao = "Preventiva";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 2)
                        manutencao = "Corretiva";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 3)
                        manutencao = "Instalação";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 4)
                        manutencao = "Outros";
                    Conteudo += "Tipo de Manutenção: " + manutencao + "<br/>";
                    Conteudo += "Status da OS: Finalizada<br/><br/>";

                    if (pecasOS?.Count > 0)
                    {
                        Conteudo += "<p><strong>Peças Utilizadas: </strong></p>";
                        int contaPc = 0;
                        foreach (var peca in pecasOS)
                        {
                            contaPc++;
                            var estoque = peca.CD_TP_ESTOQUE_CLI_TEC == 'C' ? "Cliente" : "Intermediario";
                            Conteudo += $" {contaPc}. {peca.DS_PECA} || Quantidade: {peca.QT_PECA} || Estoque: {estoque}<br/>";
                        }
                    }

                    //Conteudo += $"<a class='btn btn-danger m-1' href = '{URLSite}'>Avalie o Atendimento</a>";
                    MensagemEmail.Replace("#Conteudo", Conteudo);
                    mailMessage = MensagemEmail.ToString();

                    mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);

                }



            }
            catch (Exception ex)
            {


            }
        }

        [HttpGet]
        [Route("Obter")]
        public IHttpActionResult Obter(int ID_OS)
        {
            try
            {
                OSPadraoEntity osPadraoEntity = new OSPadraoEntity();
                osPadraoEntity.ID_OS = ID_OS;
                var listaOSPadrao = new OSPadraoData().ObterLista(osPadraoEntity);

                JObject JO = new JObject();
                JO.Add("OS", JArray.FromObject(listaOSPadrao));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }
 
        [HttpGet]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(OSPadraoEntity osPadraoEntity)
        {
            try
            {
                IList<OSPadraoEntity> listaOS = new List<OSPadraoEntity>();
                listaOS = new OSPadraoData().ObterLista(osPadraoEntity);

                JObject JO = new JObject();
                JO.Add("OS", JArray.FromObject(listaOS));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ObterListaOSSinc")]
        public IHttpActionResult ObterListaOSSinc(long idUsuario, int bloquearDataRetroativa = 0)
        {
            IList<OSPadraoEntity> listaOS = new List<OSPadraoEntity>();

            try
            {
                OSPadraoEntity osPadraoEntity = new OSPadraoEntity();
                osPadraoEntity.Tecnico.usuario.nidUsuario = idUsuario;
                osPadraoEntity.Tecnico.usuarioCoordenador.nidUsuario = idUsuario;
                listaOS = new OSPadraoData().ObterListaOSSinc(osPadraoEntity);

                DateTime dtInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-60);
                listaOS = listaOS.Where(x => x.DT_DATA_OS >= dtInicio).ToList();

                //if (bloquearDataRetroativa == 1)
                //{
                //    //DateTime dtInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //    DateTime dtInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-60);
                //    listaOS = listaOS.Where(x => x.DT_DATA_OS >= dtInicio).ToList();
                //}

                JObject JO = new JObject();
                JO.Add("SUCESSO", true);
                JO.Add("MENSAGEM", "");
                JO.Add("OS", JArray.FromObject(listaOS));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //return BadRequest(ex.Message);

                JObject JO = new JObject();
                JO.Add("SUCESSO", false);
                JO.Add("MENSAGEM", ex.Message);
                JO.Add("OS", JArray.FromObject(listaOS));
                return Ok(JO);
            }
        }

        [HttpPost]
        [Route("GravarOSSinc")]
        public IHttpActionResult GravarOSSinc(JObject JO, Int64 idUsuario)
        {
            var sincData = new SincronismoData();
            _idLogSincronismo = sincData.GravaLogSincronismo(idUsuario, Convert.ToString(JO));

            try
            {
                List<OSPadraoEntity> listOSPadrao = new List<OSPadraoEntity>();
                listOSPadrao = JsonConvert.DeserializeObject<List<OSPadraoEntity>>(JO["OSPADRAO"] is null ? "" : JO["OSPADRAO"].ToString());
                
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

                            foreach (var os in listOSPadrao)
                            {
                                if (os.TOKEN == OSValidar.TOKEN)
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


                List<RelatorioReclamacaoSincEntity> listRR = new List<RelatorioReclamacaoSincEntity>();
                listRR = JsonConvert.DeserializeObject<List<RelatorioReclamacaoSincEntity>>(JO["RR"] is null ? "" : JO["RR"].ToString());

                List<PecaOSSinc> listPecaOS = new List<PecaOSSinc>();
                listPecaOS = JsonConvert.DeserializeObject<List<PecaOSSinc>>(JO["PECA_OS"] is null ? "" : JO["PECA_OS"].ToString());

                List<PendenciaOSSinc> listPendenciaOS = new List<PendenciaOSSinc>();
                listPendenciaOS = JsonConvert.DeserializeObject<List<PendenciaOSSinc>>(JO["PENDENCIA_OS"] is null ? "" : JO["PENDENCIA_OS"].ToString());

                List<PendenciaOSSinc> listPendenciaOSoutros = new List<PendenciaOSSinc>();
                listPendenciaOSoutros = JsonConvert.DeserializeObject<List<PendenciaOSSinc>>(JO["PENDENCIA_OS_OUTROS"] is null ? "" : JO["PENDENCIA_OS_OUTROS"].ToString());

                sincData.GravarOSSinc(idUsuario, listOSPadrao, listPecaOS, listPendenciaOS, listRR, listPendenciaOSoutros);
                sincData.AtualizaLogSincronismo(_idLogSincronismo, SINCRONIZADO_SUCESSO);

                return Ok(SINCRONIZADO_SUCESSO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                sincData.AtualizaLogSincronismo(_idLogSincronismo, "ERRO:" + ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}