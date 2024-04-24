using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using System.Data;
using Newtonsoft.Json;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/VisitaPadraoAPI")]
    [Authorize]
    public class VisitaPadraoAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Incluir")]
        public IHttpActionResult Incluir(VisitaPadraoEntity visitaPadraoEntity, Int64 idUsuario)
        {
            SincronismoData sincData = new SincronismoData();
            var id_log_sincronismo = sincData.GravaLogSincronismo(idUsuario, JsonConvert.SerializeObject((visitaPadraoEntity)));

            try
            {
                if (visitaPadraoEntity.TOKEN <= 0)
                    throw new Exception($"Token para integração de registro do cliente código {visitaPadraoEntity.Cliente.CD_CLIENTE} não informado.");

                if (ValidarPermiteIncluirVisitaSalvar(visitaPadraoEntity))
                    throw new Exception("Você já tem Visita ou Ordem de Serviço registrada neste período, corrija as informações deste formulário, se necessário");

                visitaPadraoEntity.Origem = "A";
                new VisitaPadraoData().Inserir(ref visitaPadraoEntity);

                sincData.AtualizaLogSincronismo(id_log_sincronismo, "sucesso");

                JObject JO = new JObject
                {
                    { "SUCESSO", true },
                    { "MENSAGEM", "" },
                    { "ID_VISITA", visitaPadraoEntity.ID_VISITA },
                    { "TOKEN", visitaPadraoEntity.TOKEN }
                };
                if (visitaPadraoEntity.TpStatusVisita.ST_STATUS_VISITA == 4)
                {
                    //EnviarEmailVisita(visitaPadraoEntity);

                    SincronismoData sincronismoData = new SincronismoData();
                    
                    sincronismoData.EnviarEmailVisita(visitaPadraoEntity, visitaPadraoEntity.ID_VISITA);
                }
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                sincData.AtualizaLogSincronismo(id_log_sincronismo, "ERRO:" + ex.Message);

                JObject JO = new JObject
                {
                    { "SUCESSO", false },
                    { "MENSAGEM", ex.Message }
                };
                return Ok(JO);
            }
        }

        
        [HttpPost]
        [Route("Alterar")]
        public IHttpActionResult Alterar(VisitaPadraoEntity visitaPadraoEntity, Int64 idUsuario)
        {
            SincronismoData sincData = new SincronismoData();
            var id_log_sincronismo = sincData.GravaLogSincronismo(idUsuario, JsonConvert.SerializeObject((visitaPadraoEntity)));

            try
            {
                if (ValidarPermiteIncluirVisitaSalvar(visitaPadraoEntity))
                    throw new Exception("Você já tem Visita ou Ordem de Serviço registrada neste período, corrija as informações deste formulário, se necessário");

                new VisitaPadraoData().Alterar(visitaPadraoEntity);

                sincData.AtualizaLogSincronismo(id_log_sincronismo, "sucesso");

                JObject JO = new JObject
                {
                    { "SUCESSO", true },
                    { "MENSAGEM", "" },
                    { "ID_VISITA", visitaPadraoEntity.ID_VISITA }
                };
                if (visitaPadraoEntity.TpStatusVisita.ST_STATUS_VISITA == 4)
                {
                    EnviarEmailVisita(visitaPadraoEntity);
                }
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                sincData.AtualizaLogSincronismo(id_log_sincronismo, "ERRO:" + ex.Message);

                JObject JO = new JObject
                {
                    { "SUCESSO", false },
                    { "MENSAGEM", ex.Message }
                };

                return Ok(JO);
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

        [HttpPost]
        [Route("Excluir")]
        public IHttpActionResult Excluir(VisitaPadraoEntity visitaPadraoEntity)
        {
            try
            {
                if (visitaPadraoEntity.TpStatusVisita.ST_STATUS_VISITA < 2 || visitaPadraoEntity.TpStatusVisita.ST_STATUS_VISITA > 4)
                    throw new Exception("Somente podem ser excluídas visitas com status Iniciar, Aberta ou Finalizada!");

                new VisitaPadraoData().Excluir(visitaPadraoEntity);

                JObject JO = new JObject();
                JO.Add("SUCESSO", true);
                JO.Add("MENSAGEM", "");
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                JObject JO = new JObject();
                JO.Add("SUCESSO", false);
                JO.Add("MENSAGEM", ex.Message);
                return Ok(JO);
            }

            //return Request.CreateResponse(HttpStatusCode.OK, new { ID_VISITA = visitaPadraoEntity.ID_VISITA, MENSAGEM = Mensagem });
        }

        [HttpGet]
        [Route("Obter")]
        public IHttpActionResult Obter(int ID_VISITA)
        {
            try
            {
                VisitaPadraoEntity visitaPadraoEntity = new VisitaPadraoEntity();
                visitaPadraoEntity.ID_VISITA = ID_VISITA;
                var listaVisitaPadrao = new VisitaPadraoData().ObterLista(visitaPadraoEntity, null, null);

                JObject JO = new JObject();
                JO.Add("VISITA", JArray.FromObject(listaVisitaPadrao));
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
        public IHttpActionResult ObterLista([FromUri] VisitaPadraoEntity visitaPadraoEntity, string orderby = "", string ordertype = "")
        {
            try
            {
                IList<VisitaPadraoEntity> listaVisita = new List<VisitaPadraoEntity>();
                listaVisita = new VisitaPadraoData().ObterListaVisita(visitaPadraoEntity);

                listaVisita = OrderList<VisitaPadraoEntity>(listaVisita, orderby, ordertype);

                JObject JO = new JObject();
                JO.Add("VISITA", JArray.FromObject(listaVisita));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ObterListaVisitaSinc")]
        public IHttpActionResult ObterListaVisitaSinc(long idUsuario, int bloquearDataRetroativa = 0)
        {
            IList<VisitaPadraoEntity> listaVisita = new List<VisitaPadraoEntity>();

            try
            {
                VisitaPadraoEntity visitaPadraoEntity = new VisitaPadraoEntity();
                visitaPadraoEntity.Tecnico.usuario.nidUsuario = idUsuario;
                visitaPadraoEntity.Tecnico.usuarioCoordenador.nidUsuario = idUsuario;
                listaVisita = new VisitaPadraoData().ObterListaVisitaSinc(visitaPadraoEntity);

                if (bloquearDataRetroativa == 1)
                {
                    DateTime dtInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    if (DateTime.Now.Day < 6) dtInicio = dtInicio.AddMonths(-1); // Até o 5o dia do mês, retornar registros do mês anterior
                    listaVisita = listaVisita.Where(x => x.DT_DATA_VISITA >= dtInicio).ToList();
                }

                JObject JO = new JObject();
                JO.Add("SUCESSO", true);
                JO.Add("MENSAGEM", "");
                JO.Add("VISITA", JArray.FromObject(listaVisita));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //return BadRequest(ex.Message);

                JObject JO = new JObject();
                JO.Add("SUCESSO", false);
                JO.Add("MENSAGEM", ex.Message);
                JO.Add("VISITA", JArray.FromObject(listaVisita));
                return Ok(JO);
            }
        }

        private string ObterCodigoTecnico(long idUsuario)
        {
            string CD_TECNICO = string.Empty;

            TecnicoEntity tecnicoEntity = new TecnicoEntity();
            tecnicoEntity.usuario.nidUsuario = idUsuario;

            DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

            if (dataTableReader.HasRows && dataTableReader.Read())
                CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();

            if (dataTableReader != null)
            {
                dataTableReader.Dispose();
                dataTableReader = null;
            }

            return CD_TECNICO;
        }

        [HttpGet]
        [Route("ObterListaClientePorIdTecnico")]
        public IHttpActionResult ObterListaClientePorIdTecnico(long idUsuario)
        {
            IList<ClienteEntity> listaCliente = new List<ClienteEntity>();

            try
            {
                TecnicoClienteEntity tecnicoClienteEntity = new TecnicoClienteEntity();
                tecnicoClienteEntity.tecnico.CD_TECNICO = ObterCodigoTecnico(idUsuario);

                DataTableReader dataTableReader = new TecnicoClienteData().ObterLista(tecnicoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        var clienteEntity = new ClienteEntity();

                        clienteEntity.CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]);
                        clienteEntity.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString(); //+ " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString();

                        listaCliente.Add(clienteEntity);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                JObject JO = new JObject();
                JO.Add("SUCESSO", true);
                JO.Add("MENSAGEM", "");
                JO.Add("CLIENTE", JArray.FromObject(listaCliente));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //return BadRequest(ex.Message);

                JObject JO = new JObject();
                JO.Add("SUCESSO", false);
                JO.Add("MENSAGEM", ex.Message);
                JO.Add("CLIENTE", JArray.FromObject(listaCliente));
                return Ok(JO);
            }
        }

        private VisitaPadraoEntity ObterVisita(VisitaPadraoEntity visitaPadraoEntity)
        {
            var lstVisita = new VisitaPadraoData().ObterListaVisita(visitaPadraoEntity);

            if (lstVisita.Where(v => v.DT_DATA_VISITA.ToString("yyyyMMdd") == visitaPadraoEntity.DT_DATA_VISITA.ToString("yyyyMMdd") && v.HR_INICIO == visitaPadraoEntity.HR_INICIO && v.HR_FIM == visitaPadraoEntity.HR_FIM).Any())
                return lstVisita.First();
            else
                return null;
        }
    }
}