using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/AnaliseSatisfacaoAPI")]
    [Authorize]
    public class AnaliseSatisfacaoAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("ObterCodigoPesquisaAtiva")]
        public IHttpActionResult ObterCodigoPesquisaAtiva(long codigoVisita)
        {
            SatisfacaoPesquisaEntity entity = new SatisfacaoPesquisaEntity();
            try
            {
                SatisfacaoPesquisaData data = new SatisfacaoPesquisaData();

                long? codigoPesquisa = data.ObterCodigoPesquisaSatisfacaoAtiva(0, codigoVisita);
                entity.ID_PESQUISA_SATISF = codigoPesquisa.HasValue ? codigoPesquisa.Value : 0;

                JObject JO = new JObject();
                JO.Add("PESQUISA_SATISF", JsonConvert.SerializeObject(entity, Formatting.None));

                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ObterCodigoPesquisaResposta")]
        public IHttpActionResult ObterCodigoPesquisaResposta(long codigoVisita)
        {
            SatisfacaoPesquisaEntity entity = new SatisfacaoPesquisaEntity();
            try
            {
                SatisfacaoPesquisaData data = new SatisfacaoPesquisaData();

                long? codigoPesquisa = data.ObterCodigoPesquisaResposta(codigoVisita);
                entity.ID_PESQUISA_SATISF = codigoPesquisa.HasValue ? codigoPesquisa.Value : 0;

                JObject JO = new JObject();
                JO.Add("PESQUISA_SATISF", JsonConvert.SerializeObject(entity, Formatting.None));

                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("Incluir")]
        public IHttpActionResult Incluir(SatisfacaoRespostaEntity respostaEntity)
        {
            try
            {
                SatisfacaoRespostaData data = new SatisfacaoRespostaData();
                data.Inserir(ref respostaEntity);

                JObject JO = new JObject();
                JO.Add("SATISFRESPOSTA", JsonConvert.SerializeObject(respostaEntity, Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("DesfazerByVisita")]
        public HttpResponseMessage DesfazerByVisita(Int64 ID_VISITA, Int64 nidUsuarioAtualizacao)
        {
            try
            {
                new SatisfacaoRespostaData().Desfazer(ID_VISITA, nidUsuarioAtualizacao);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_VISITA = ID_VISITA });
        }

        [HttpPost]
        [Route("ObterPesquisaSatisfacao")]
        public IHttpActionResult ObterPesquisaSatisfacao(SatisfacaoPesquisaEntity entity)
        {
            try
            {
                List<SatisfacaoPesquisaEntity> listaPesquisa = null;

                SatisfacaoPesquisaData data = new SatisfacaoPesquisaData();
                using (DataTable dataTable = data.ObterLista(entity))
                {
                    listaPesquisa = (from row in dataTable.Rows.Cast<DataRow>()
                                     select ToPesquisaEntity(row)).ToList();
                }
                JObject JO = new JObject();
                JO.Add("PESQUISA_SATISF", JsonConvert.SerializeObject(listaPesquisa, Formatting.None));

                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        internal bool NotificarAvaliacaoDisponivel(VisitaSinc visitaSinc)
        {
            VisitaEntity visitaEntity = new VisitaEntity();
            visitaEntity.ID_VISITA = visitaSinc.ID_VISITA.HasValue ? visitaSinc.ID_VISITA.Value : 0;
            visitaEntity.DT_DATA_VISITA = visitaSinc.DT_DATA_VISITA;
            visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = visitaSinc.ST_TP_STATUS_VISITA_OS;
            visitaEntity.cliente.CD_CLIENTE = visitaSinc.CD_CLIENTE;
            visitaEntity.tecnico.CD_TECNICO = visitaSinc.CD_TECNICO;
            visitaEntity.DS_OBSERVACAO = visitaSinc.DS_OBSERVACAO;
            visitaEntity.DS_NOME_RESPONSAVEL = visitaSinc.DS_NOME_RESPONSAVEL;
            visitaEntity.nidUsuarioAtualizacao = visitaSinc.nidUsuarioAtualizacao;
            visitaEntity.dtmDataHoraAtualizacao = visitaSinc.dtmDataHoraAtualizacao;
            visitaEntity.FL_ENVIO_EMAIL_PESQ = visitaSinc.FL_ENVIO_EMAIL_PESQ;

            return this.NotificarAvaliacaoDisponivel(visitaEntity);
        }

        internal bool NotificarAvaliacaoDisponivel(VisitaEntity visitaEntity)
        {
            bool enviarEmail = false;
            try
            {
                if (visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == ControlesUtility.Enumeradores.TpStatusVisita.Finalizada.ToInt())
                {

                    //SL00036327 ------------------------------------
                    // Enviar Email de Confirmação de Atendimento ao Cliente

                    ClienteData clienteData = new ClienteData();
                    using (DataTable dtCliente = clienteData.ObterLista(new ClienteEntity() { CD_CLIENTE = visitaEntity.cliente.CD_CLIENTE }, null))
                    {
                        if (dtCliente.Rows.Count > 0)
                        {
                            visitaEntity.cliente.NM_CLIENTE = dtCliente.Rows[0]["NM_CLIENTE"].ToString();
                            if (dtCliente.Rows[0]["FL_PESQ_SATISF"] != DBNull.Value)
                            {
                                visitaEntity.cliente.FL_PESQ_SATISF = dtCliente.Rows[0]["FL_PESQ_SATISF"].ToString();
                            }
                        }
                    }



                    UsuarioClienteData usuarioClienteData = new UsuarioClienteData();
                    UsuarioClienteEntity usuarioClienteEntity = new UsuarioClienteEntity();
                    usuarioClienteEntity.cliente.CD_CLIENTE = visitaEntity.cliente.CD_CLIENTE;
                    using (DataTable dtusuarioCliente = usuarioClienteData.ObterLista(usuarioClienteEntity))
                    {
                        if (dtusuarioCliente.Rows.Count > 0)
                        {
                            visitaEntity.cliente.TX_EMAIL = dtusuarioCliente.Rows[0]["cdsEmail"].ToString();
                        }
                    }

                    TecnicoData tecnicoData = new TecnicoData();
                    TecnicoEntity tecnicoEntity = new TecnicoEntity();
                    tecnicoEntity.CD_TECNICO = visitaEntity.tecnico.CD_TECNICO;
                    using (DataTable dtTecnico = tecnicoData.ObterLista(tecnicoEntity))
                    {
                        if (dtTecnico.Rows.Count > 0)
                        {
                            visitaEntity.tecnico.NM_TECNICO = dtTecnico.Rows[0]["NM_TECNICO"].ToString();
                        }
                    }


                    if (!string.IsNullOrEmpty(visitaEntity.cliente.TX_EMAIL))
                    {
                        //SL00036327 --- Descomentar linha abaixo para Enviar Emails de Conformação de Atendimento ao Cliente
                        var res = EmailConfirmaAtendimento(visitaEntity);
                    }

                    SatisfacaoPesquisaData pesquisaData = new SatisfacaoPesquisaData();
                    long? codigoPesquisa = pesquisaData.ObterCodigoPesquisaSatisfacaoAtiva(Convert.ToInt64(visitaEntity.cliente.CD_CLIENTE), visitaEntity.ID_VISITA);
                    if (codigoPesquisa.HasValue)
                    {
                        using (DataTable dtPesquisa = pesquisaData.ObterLista(new SatisfacaoPesquisaEntity() { ID_PESQUISA_SATISF = codigoPesquisa.Value }))
                        {
                            if (!string.IsNullOrEmpty(visitaEntity.cliente.FL_PESQ_SATISF) && !string.IsNullOrEmpty(visitaEntity.cliente.TX_EMAIL))
                            {
                                if ((dtPesquisa.Rows[0]["TP_PESQUISA"].ToString().Equals("1") && new string[] { "1", "2" }.Contains(visitaEntity.cliente.FL_PESQ_SATISF.ToString())) ||
                                    (dtPesquisa.Rows[0]["TP_PESQUISA"].ToString().Equals("2") && new string[] { "2" }.Contains(visitaEntity.cliente.FL_PESQ_SATISF.ToString())))
                                {
                                    enviarEmail = true;
                                }

                            }
                        }

                        if (enviarEmail)
                        {
                            MailSender mailSender = new MailSender();

                            string mailTo = visitaEntity.cliente.TX_EMAIL;
                            string mailSubject = "3M.Comodato - Pesquisa de Satisfação";
                            string mailMessage = string.Empty;
                            System.Net.Mail.Attachment Attachments = null;
                            string mailCopy = null;

                            string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);
                            var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                            string Conteudo = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MensagemEnvioAvaliacao);

                            Conteudo += "<p>Por favor, clique no link abaixo ou copie seu conteúdo para o Navegador WEB:</p>";
                            Conteudo += "<p><a href='" + URLSite + "'></a>" + URLSite + "</p>";

                            Conteudo = string.Format(Conteudo, visitaEntity.DT_DATA_VISITA.DataString());

                            MensagemEmail.Replace("#Conteudo", Conteudo);
                            mailMessage = MensagemEmail.ToString();
                            mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);

                            visitaEntity.FL_ENVIO_EMAIL_PESQ = "S";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                visitaEntity.FL_ENVIO_EMAIL_PESQ = "N";
                LogUtility.LogarErro(ex);
            }

            return enviarEmail;
        }

        //SL00036327
        internal bool EmailConfirmaAtendimento(VisitaEntity visitaEntity)
        {
            try
            {
                string emails3MConfirmaAtendimento = ControlesUtility.Parametro.ObterValorParametro("emails3MConfirmaAtendimento");

                if (string.IsNullOrEmpty(emails3MConfirmaAtendimento))
                {
                    return false;
                }
                if (emails3MConfirmaAtendimento == "false")
                {
                    return false;
                }


                MailSender mailSender = new MailSender();

                string mailTo = visitaEntity.cliente.TX_EMAIL;// + ";" + emails3MConfirmaAtendimento;
                string mailSubject = "3M.Comodato - Confirmação do Atendimento";
                string mailMessage = string.Empty;
                System.Net.Mail.Attachment Attachments = null;
                string mailCopy = emails3MConfirmaAtendimento;

                string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);
                var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                string Conteudo = "";//ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MensagemEnvioAvaliacao);

                Conteudo += "<p>Caro cliente " + visitaEntity.cliente.NM_CLIENTE + " (" + visitaEntity.cliente.CD_CLIENTE + "),</p>";
                
                Conteudo += "<p>Foi realizada um visita na data " + visitaEntity.DT_DATA_VISITA.Value.ToString("dd/MM/yyyy") + "</p>";
                Conteudo += "<p>pelo técnico " + visitaEntity.tecnico.NM_TECNICO + ".</p></br>";

                Conteudo += "<p>Por favor, clique no link abaixo ou copie seu conteúdo para o Navegador WEB</p>";
                Conteudo += "<p>para <strong>confirmar e atribuir uma nota</strong> para o atendimento.</p>";
                Conteudo += "<p><a href='" + URLSite + "'></a>" + URLSite + "</p>";

                Conteudo = string.Format(Conteudo, visitaEntity.DT_DATA_VISITA.DataString());

                MensagemEmail.Replace("#Conteudo", Conteudo);
                mailMessage = MensagemEmail.ToString();
                mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);

                return true;

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                LogUtility.LogarErro("Erro envio EmailConfirmaAtendimento para ID_VISITA=" + visitaEntity.ID_VISITA);
                return false;
            }

        }


        public static SatisfacaoPesquisaEntity ToPesquisaEntity(DataRow r)
        {
            SatisfacaoPesquisaEntity satisfacaoPesquisa = new SatisfacaoPesquisaEntity();

            long codigoPesquisa = Convert.ToInt64(r["ID_PESQUISA_SATISF"]);
            satisfacaoPesquisa.ID_PESQUISA_SATISF = codigoPesquisa;
            satisfacaoPesquisa.DS_TITULO = r["DS_TITULO"].ToString();
            satisfacaoPesquisa.DT_CRIACAO = Convert.ToDateTime(r["DT_CRIACAO"]);
            satisfacaoPesquisa.TP_PESQUISA = Convert.ToInt32(r["TP_PESQUISA"]);

            if (r["DT_FINALIZACAO"] != DBNull.Value)
            {
                satisfacaoPesquisa.DT_FINALIZACAO = Convert.ToDateTime(r["DT_FINALIZACAO"]);
            }

            satisfacaoPesquisa.ST_STATUS_PESQUISA = Convert.ToInt32(r["ST_STATUS_PESQUISA"]);

            satisfacaoPesquisa.USUARIO_RESPONSAVEL.nidUsuario = Convert.ToInt64(r["ID_USUARIO_RESPONSAVEL"]);

            satisfacaoPesquisa.DS_DESCRICAO = r["DS_DESCRICAO"].ToString();
            satisfacaoPesquisa.DS_PERGUNTA1 = r["DS_PERGUNTA1"].ToString();
            satisfacaoPesquisa.DS_PERGUNTA2 = r["DS_PERGUNTA2"].ToString();
            satisfacaoPesquisa.DS_PERGUNTA3 = r["DS_PERGUNTA3"].ToString();
            satisfacaoPesquisa.DS_PERGUNTA4 = r["DS_PERGUNTA4"].ToString();
            satisfacaoPesquisa.DS_PERGUNTA5 = r["DS_PERGUNTA5"].ToString();
            //satisfacaoPesquisa.qt = Convert.ToInt32(r["QT_VISITAS"]);

            return satisfacaoPesquisa;
        }

    }
}