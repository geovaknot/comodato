using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Linq;
using System.Web.Http;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/SolicitacaoAtendimentoAPI")]
    [Authorize]
    public class SolicitacaoAtendimentoAPIController : BaseAPIController
    {
        [HttpGet()]
        [Route("ObterCodigoSolicitacao")]
        public IHttpActionResult ObterCodigoSolicitacao()
        {
            try
            {
                SolicitacaoAtendimentoData data = new SolicitacaoAtendimentoData();
                string codigoSolicitacao = data.ObterNovoCodigo();

                JObject JO = new JObject();
                JO.Add("SOLIC_ATEND", JsonConvert.SerializeObject(codigoSolicitacao, Formatting.None));

                return Ok(JO);
            }
            catch (System.Exception ex)
            {

                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);

            }
        }

        [HttpPost]
        [Route("InserirSolicitacao")]
        public IHttpActionResult InserirSolicitacao(SolicitacaoAtendimentoEntity solicitacao)
        {
            try
            {
                solicitacao.DT_DATA_SOLICITACAO = DateTime.Today;
                solicitacao.StatusAtendimento.ID_STATUS_ATENDIMENTO = 1;//PENDENTE

                SolicitacaoAtendimentoData data = new SolicitacaoAtendimentoData();
                if (data.Inserir(solicitacao))
                {
                    JObject JO = new JObject();
                    JO.Add("SOLIC_ATEND", JsonConvert.SerializeObject(ControlesUtility.Constantes.MensagemGravacaoSucesso, Formatting.None));

                    EmailSolicitaAtendimento(solicitacao);


                    return Ok(JO);
                }
                else
                {
                    return StatusCode(System.Net.HttpStatusCode.NotModified);
                }
            }
            catch (System.Exception ex)
            {

                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("AlterarSolicitacao")]
        public IHttpActionResult AlterarSolicitacao(SolicitacaoAtendimentoEntity solicitacao)
        {
            try
            {

                SolicitacaoAtendimentoData data = new SolicitacaoAtendimentoData();
                if (data.Alterar(solicitacao))
                {
                    JObject JO = new JObject();
                    JO.Add("SOLIC_ATEND", JsonConvert.SerializeObject(ControlesUtility.Constantes.MensagemGravacaoSucesso, Formatting.None));

                    return Ok(JO);
                }
                else
                {
                    return StatusCode(System.Net.HttpStatusCode.NotModified);
                }
            }
            catch (System.Exception ex)
            {

                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("Obter")]
        public IHttpActionResult Obter(SolicitacaoAtendimentoEntity solicitacao)
        {
            try
            {
                if (null == solicitacao)
                {
                    solicitacao = new SolicitacaoAtendimentoEntity();
                }

                SolicitacaoAtendimentoData data = new SolicitacaoAtendimentoData();
                var listaSolicitacao = data.ObterListaEntity(solicitacao).ToList();


                JObject JO = new JObject();
                JO.Add("SOLIC_LIST", JsonConvert.SerializeObject(listaSolicitacao, Formatting.None));

                return Ok(JO);
            }
            catch (System.Exception ex)
            {

                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        internal bool EmailSolicitaAtendimento(SolicitacaoAtendimentoEntity solicitacao)
        {
            try
            {
                string emails3MSolicitacaoAtendimento = ControlesUtility.Parametro.ObterValorParametro("emails3MSolicitacaoAtendimento");

                if (emails3MSolicitacaoAtendimento == "false")
                {
                    return false;
                }

                TecnicoEntity tecnicoEntity = new TecnicoEntity();

                //Obter o código do Técnico Principal do cliente
                var tecnicoClienteEntity = new TecnicoClienteEntity();
                tecnicoClienteEntity.CD_ORDEM = 1;
                tecnicoClienteEntity.cliente.CD_CLIENTE = solicitacao.CLIENTE.CD_CLIENTE;
                DataTableReader dataTableReader = new TecnicoClienteData().ObterLista(tecnicoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        tecnicoEntity.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
                    }
                }
                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }



                //Obter o código os dados do Coord Lider do Técnico Principal do cliente
                dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();
                tecnicoEntity.NM_TECNICO = "Não cadastrado técnico principal para o cliente.";
                tecnicoEntity.usuarioCoordenador.cdsEmail = "Email não cadastrado.";
                tecnicoEntity.usuarioCoordenador.cnmNome = "Coordenador Lider não cadastrado.";
                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        tecnicoEntity.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                        if (dataTableReader["cdsEmail"] != DBNull.Value)
                        {
                            tecnicoEntity.usuario.cdsEmail = dataTableReader["cdsEmail"].ToString();
                        }
                        if (dataTableReader["cdsEmailCoordenador"] != DBNull.Value)
                        {
                            tecnicoEntity.usuarioCoordenador.cdsEmail = dataTableReader["cdsEmailCoordenador"].ToString();
                        }
                        if (dataTableReader["cnmNomeCoordenador"] != DBNull.Value)
                        {
                            tecnicoEntity.usuarioCoordenador.cnmNome = dataTableReader["cnmNomeCoordenador"].ToString();
                        }
                    }
                }
                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }


                MailSender mailSender = new MailSender();

                string mailTo = null;
                if (tecnicoEntity.usuarioCoordenador.cdsEmail != "Email não cadastrado.")
                {
                    mailTo = tecnicoEntity.usuarioCoordenador.cdsEmail;
                }
                string mailSubject = "3M.Comodato - Solicitação de Atendimento";
                string mailMessage = string.Empty;
                System.Net.Mail.Attachment Attachments = null;
                string mailCopy = null;
                if (!string.IsNullOrEmpty(emails3MSolicitacaoAtendimento))
                {
                    mailCopy = emails3MSolicitacaoAtendimento;
                }

                string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);
                var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                string Conteudo = "";//ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MensagemEnvioAvaliacao);

                Conteudo += "<p>Caro Coordenador Lider, </p><br />";
                Conteudo += "<p>Foi <strong>solicitado um Atendimento</strong> para: </p>";
                Conteudo += "<p><strong>Cliente:</strong> " + solicitacao.CLIENTE.NM_CLIENTE + "</p>";
                Conteudo += "<p><strong>Coordenador Lider:</strong> " + tecnicoEntity.usuarioCoordenador.cnmNome + " (" + tecnicoEntity.usuarioCoordenador.cdsEmail + ")" + ")</p>";
                Conteudo += "<p><strong>Técnico Principal:</strong> " + tecnicoEntity.NM_TECNICO + "</p>";
                Conteudo += "<p><strong>Data Solicitação:</strong> " + solicitacao.DT_DATA_SOLICITACAO.ToString("dd/MM/yyyy") + "</p>";
                Conteudo += "<p><strong>Contato:</strong> " + solicitacao.DS_CONTATO + "</p>";
                Conteudo += "<p><strong>Tipo de Atendimento:</strong> " + solicitacao.TipoAtendimento.DS_TIPO_ATENDIMENTO + "</p>";
                Conteudo += "<p><strong>Ativo Fixo:</strong> " + solicitacao.AtivoFixo.DS_ATIVO_FIXO + "</p>";
                Conteudo += "<p><strong>Observação:</strong> " + solicitacao.DS_OBSERVACAO + "</p></br>";

                Conteudo += "<p>Para acessar o sistema por favor, clique no link abaixo ou copie seu conteúdo para o Navegador WEB</p>";
                Conteudo += "<p><a href='" + URLSite + "'></a>" + URLSite + "</p>";


                MensagemEmail.Replace("#Conteudo", Conteudo);
                mailMessage = MensagemEmail.ToString();
                mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);

                return true;

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                LogUtility.LogarErro("Erro envio EmailSolicitaAtendimento para ID_SOLICITA_ATENDIMENTO=" + solicitacao.ID_SOLICITA_ATENDIMENTO);
                return false;
            }

        }


    }
}
