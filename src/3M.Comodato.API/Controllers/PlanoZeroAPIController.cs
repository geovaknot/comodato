using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/PlanoZeroAPI")]
    [Authorize]
    public class PlanoZeroAPIController : BaseAPIController
    {
        [HttpPost]
        [AcceptVerbs("POST")]
        [Route("Adicionar")]
        public IHttpActionResult Adicionar([FromBody]PlanoZeroEntity planoZero)
        {
            try
            {
                PlanoZeroData data = new PlanoZeroData();
                if (data.Inserir(ref planoZero))
                    return Ok(planoZero);
                else
                    return InternalServerError();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        //Novo Plano Zero
        [HttpPost]
        [AcceptVerbs("GET")]
        [Route("GerarPlanoZero")]
        public IHttpActionResult GerarPlanoZeroAsync(Int32 idUsuario)
        {
            string status = "";
            try
            {
                PlanoZeroData planozero = new PlanoZeroData();

                bool ponderacao = planozero.VerificaFatorPornderacao();

                if (planozero.VerificarParametro("Plano_Zero_em_Processamento") == "true")
                {
                    status = "existe";
                }
                else
                {
                    if (ponderacao)
                    {
                        if (planozero.AlteraStatusPlanoZero(true))
                        {
                            EnviarEmailPlanoZero("Inicio", "");
                            planozero.GerarPlanoZero(idUsuario);
                        }
                        
                    }
                    else
                    {
                        EnviarEmailPlanoZero("Ponderacao", "");
                        throw new Exception("Revise o Cadastro de Fator de Ponderação, pois existe lacuna de valores não contemplados entre as Faixas Iniciais e Finais de Qtde de Clientes!");
                    }
                }
                return Ok(status);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [AcceptVerbs("GET")]
        [Route("CancelarPlanoZero")]
        public IHttpActionResult CancelarPlanoZero(Int32 idUsuario)
        {
            string status = "";
            try
            {
                PlanoZeroData data = new PlanoZeroData();

                var existePZ = data.VerificaPlanoZeroExistente();
                if (existePZ)
                {
                    var pzId = data.RetornarPlanoZeroExistente();

                    if (pzId > 0)
                    {
                        data.CancelarPedidosPZ(pzId);
                        data.AtualizarStatusPlanoZero(pzId, idUsuario);
                    }
                }

                status = "Plano Zero cancelado com sucesso!";
                //if (data.Inserir(ref planoZero))
                return Ok(status);
                //else
                //    return InternalServerError();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [AcceptVerbs("POST")]
        [Route("Alterar")]
        public IHttpActionResult Alterar([FromBody]PlanoZeroEntity planoZero)
        {
            try
            {
                PlanoZeroData data = new PlanoZeroData();
                if (data.Alterar(planoZero))
                    return Ok(planoZero);
                else
                    return InternalServerError();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista([FromBody]PlanoZeroEntity planoZero)
        {
            try
            {
                Func<DataRow, Models.PlanoZero> entityConverter = new Func<DataRow, Models.PlanoZero>((r) =>
                {
                    Models.PlanoZero entity = new Models.PlanoZero();
                    entity.nidPlanoZero = Convert.ToInt64(r["ID_PLANO_ZERO"]);
                    entity.ccdPeca = r["CD_PECA"].ToString();
                    entity.ccdModelo = r["CD_MODELO"].ToString();
                    entity.ccdGrupoModelo = r["CD_GRUPO_MODELO"] is DBNull ? "0" : r["CD_GRUPO_MODELO"].ToString();
                    entity.nqtPecaModelo = r["QT_PECA_MODELO"] is DBNull ? "0" : Convert.ToDecimal(r["QT_PECA_MODELO"]).ToString("N0");
                    entity.nPonderacao = r["NM_PERC_PONDERACAO"] is DBNull ? "0" : Convert.ToDecimal(r["NM_PERC_PONDERACAO"]).ToString("N2");
                    entity.nqtEstoqueMinimo = r["QT_ESTOQUE_MINIMO"] is DBNull ? "0" : Convert.ToDecimal(r["QT_ESTOQUE_MINIMO"]).ToString("N2");
                    entity.ccdCriticidadeAbc = r["CD_CRITICIDADE_ABC"].ToString();
                    entity.nidUsuarioAtualizacao = Convert.ToInt64(r["ID_USU_RESPONSAVEL"]);
                    entity.dtmDataHoraAtualizacao = Convert.ToDateTime(r["DT_CRIACAO"]);

                    return entity;
                });

                PlanoZeroData data = new PlanoZeroData();
                DataTable dtPlanoZero = data.ObterLista(planoZero);

                var listaPlanoZero = (from p in dtPlanoZero.Rows.Cast<DataRow>()
                                      select entityConverter(p)).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { PlanoZero = listaPlanoZero });
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("ObterListaModelo")]
        public HttpResponseMessage ObterListaModelo(string CD_ATIVO_FIXO)
        {
            try
            {
                string CD_GRUPO_MODELO = string.Empty;
                List<Models.PlanoZero> listaPlanoZero = new List<Models.PlanoZero>();

                AtivoFixoEntity ativoFixoEntity = new AtivoFixoEntity();
                ativoFixoEntity.CD_ATIVO_FIXO = CD_ATIVO_FIXO;
                DataTableReader dataTableReader = new AtivoFixoData().ObterLista(ativoFixoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CD_GRUPO_MODELO = dataTableReader["CD_GRUPO_MODELO"].ToString();
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                PlanoZeroEntity planoZeroEntity = new PlanoZeroEntity();
                //planoZeroEntity.Modelo.CD_GRUPO_MODELO = CD_GRUPO_MODELO;
                planoZeroEntity.grupoModelo.CD_GRUPO_MODELO = CD_GRUPO_MODELO;
                dataTableReader = new PlanoZeroData().ObterLista(planoZeroEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.PlanoZero planoZero = new Models.PlanoZero();
                        //{
                            //nidPlanoZero = Convert.ToInt64(dataTableReader["ID_PLANO_ZERO"]),
                            //ccdPeca = dataTableReader["CD_PECA"].ToString(),
                            //ccdModelo = dataTableReader["CD_MODELO"].ToString(),
                            //ccdGrupoModelo = dataTableReader["CD_GRUPO_MODELO"].ToString(),
                            //nqtPecaModelo = Convert.ToDecimal(dataTableReader["QT_PECA_MODELO"]).ToString("N0"),
                            //nPonderacao = Convert.ToDecimal(dataTableReader["NM_PERC_PONDERACAO"]).ToString("N2"),
                            //nqtEstoqueMinimo = Convert.ToDecimal(dataTableReader["QT_ESTOQUE_MINIMO"]).ToString("N2"),
                            //ccdCriticidadeAbc = dataTableReader["CD_CRITICIDADE_ABC"].ToString(),
                            //PecaModel = new Models.Peca()
                            //{
                            //    CD_PECA = dataTableReader["CD_PECA"].ToString(),
                            //    DS_PECA = dataTableReader["DS_PECA"].ToString(),
                            //}
                        //};

                        if (dataTableReader["ID_PLANO_ZERO"] != DBNull.Value)
                        {
                            planoZero.nidPlanoZero = Convert.ToInt64(dataTableReader["ID_PLANO_ZERO"]);
                        }
                        if (dataTableReader["CD_PECA"] != DBNull.Value)
                        {
                            planoZero.ccdPeca = dataTableReader["CD_PECA"].ToString();
                        }
                        if (dataTableReader["CD_GRUPO_MODELO"] != DBNull.Value)
                        {
                            planoZero.ccdGrupoModelo = dataTableReader["CD_GRUPO_MODELO"].ToString();
                        }
                        if (dataTableReader["QT_PECA_MODELO"] != DBNull.Value)
                        {
                            planoZero.nqtPecaModelo = Convert.ToDecimal(dataTableReader["QT_PECA_MODELO"]).ToString("N0");
                        }
                        if (dataTableReader["NM_PERC_PONDERACAO"] != DBNull.Value)
                        {
                            planoZero.nPonderacao = Convert.ToDecimal(dataTableReader["NM_PERC_PONDERACAO"]).ToString("N2");
                        }
                        if (dataTableReader["QT_ESTOQUE_MINIMO"] != DBNull.Value)
                        {
                            planoZero.nqtEstoqueMinimo = Convert.ToDecimal(dataTableReader["QT_ESTOQUE_MINIMO"]).ToString("N2");
                        }
                        if (dataTableReader["CD_CRITICIDADE_ABC"] != DBNull.Value)
                        {
                            planoZero.ccdCriticidadeAbc = dataTableReader["CD_CRITICIDADE_ABC"].ToString();
                        }
                        planoZero.PecaModel = new Models.Peca();
                        if (dataTableReader["CD_PECA"] != DBNull.Value)
                        {
                            planoZero.PecaModel.CD_PECA = dataTableReader["CD_PECA"].ToString();
                        }
                        if (dataTableReader["DS_PECA"] != DBNull.Value)
                        {
                            planoZero.PecaModel.DS_PECA = dataTableReader["DS_PECA"].ToString();
                        }
                        
                        listaPlanoZero.Add(planoZero);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { PlanoZero = listaPlanoZero });
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("ObterQtSugerida")]
        public HttpResponseMessage ObterQtSugerida(string CD_TECNICO, string CD_PECA)
        {
            string QT_SUGERIDA_PZ = string.Empty;
            try
            {
                DataTableReader dataTableReader = new PlanoZeroData().ObterQtSugerida(CD_TECNICO, CD_PECA).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        QT_SUGERIDA_PZ = Convert.ToDecimal(dataTableReader["QT_SUGERIDA_PZ"]).ToString("N0");
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
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                //return Request.CreateResponse(HttpStatusCode.OK, new { QT_SUGERIDA_PZ = "-" });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { QT_SUGERIDA_PZ = QT_SUGERIDA_PZ });
        }

        [HttpPost]
        [Route("ObterCriticidadeCarteira")]
        public IHttpActionResult ObterCriticidadeCarteira(string ccdGrupoModelo)
        {
            try
            {
                object criticidade = null;
                PlanoZeroData planoZeroData = new PlanoZeroData();
                using (DataTable dtCriticidadeMaquina = planoZeroData.ObterCriticidadeCarteira(ccdGrupoModelo))
                {
                    int nqtMaquinaCarteira = 0, nqtCriticidadeA = 0, nqtCriticidadeB = 0, nqtCriticidadeC = 0, nqtCriticidadeTotal = 0;
                    if (dtCriticidadeMaquina.Rows.Count > 0)
                    {
                        nqtMaquinaCarteira = Convert.ToInt32(dtCriticidadeMaquina.Rows[0]["QT_CRITICIDADE_MAQ_CART"]);
                        nqtCriticidadeA = Convert.ToInt32(dtCriticidadeMaquina.Rows[0]["QT_CRITICIDADE_A"]);
                        nqtCriticidadeB = Convert.ToInt32(dtCriticidadeMaquina.Rows[0]["QT_CRITICIDADE_B"]);
                        nqtCriticidadeC = Convert.ToInt32(dtCriticidadeMaquina.Rows[0]["QT_CRITICIDADE_C"]);
                        nqtCriticidadeTotal = Convert.ToInt32(dtCriticidadeMaquina.Rows[0]["QT_CRITICIDADE_TOT"]);
                    }

                    criticidade = new
                    {
                        nqtMaquinaCarteira,
                        nqtCriticidadeA,
                        nqtCriticidadeB,
                        nqtCriticidadeC,
                        nqtCriticidadeTotal
                    };
                }

                JObject JO = new JObject();
                JO.Add("CriticidadeCarteira", JsonConvert.SerializeObject(criticidade, Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Remover")]
        public IHttpActionResult Remover(long nidPlanoZero, long nidUsuario)
        {
            try
            {
                PlanoZeroData planoZeroData = new PlanoZeroData();
                planoZeroData.Excluir(new PlanoZeroEntity() { nidPlanoZero = nidPlanoZero, nidUsuarioAtualizacao = nidUsuario });

                JObject JO = new JObject();
                JO.Add("Mensagem", JsonConvert.SerializeObject(ControlesUtility.Constantes.MensagemExclusaoSucesso, Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ObterListaPlanoZeroSinc")]
        public IHttpActionResult ObterListaPlanoZeroSinc()
        {
            IList<PlanoZeroSinc> listaPlanoZero = new List<PlanoZeroSinc>();
            try
            {
                PlanoZeroData planoZeroData = new PlanoZeroData();
                listaPlanoZero = planoZeroData.ObterListaPlanoZeroSinc();

                JObject JO = new JObject();
                //JO.Add("MODELO", JsonConvert.SerializeObject(listaModelo, Formatting.None));
                JO.Add("PLANOZERO", JArray.FromObject(listaPlanoZero));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// "status: Inicio/Fim/Erro"
        /// "erro: Msg de Erro"
        /// </summary>
 
        public void EnviarEmailPlanoZero(string status, string erro)
        {
            try
            {
                string mailTo = ControlesUtility.Parametro.ObterValorParametro("emailsProcessamentoPlanoZero");
                string Subject = string.Empty;
                string Conteudo = string.Empty;
                Attachment Attachments = null;
                string mailCopy = null;

                string ambienteParametro = ControlesUtility.Parametro.ObterValorParametro("Ambiente");

                string ambiente = "";
                if (ambienteParametro != null)
                    ambiente = ambienteParametro;
                //ConfigurationManager.AppSettings["Ambiente"] == "H" ? "Homologação" : "Produção";
                //
                switch (status)
                {
                    case "Inicio":
                        Subject = $"3M.Comodato - Inicio do Processamento do Plano Zero {ambiente}";
                        Conteudo = "<p>O processamento do Plano Zero foi iniciado!<br/>"; 
                        break;
                    case "Fim":
                        Subject = $"3M.Comodato - Finalização do Processamento do Plano Zero {ambiente}";
                        Conteudo = "<p>O processamento do Plano Zero foi finalizado com sucesso!<br/>";
                        break;
                    case "Erro":
                        Subject = $"3M.Comodato - Finalização do Processamento do Plano Zero {ambiente}";
                        Conteudo = "<p>O processamento do Plano Zero foi finalizado com erros!<br/>";
                        break;
                    case "Ponderacao":
                        Subject = $"3M.Comodato - Erro no Processamento do Plano Zero {ambiente}";
                        Conteudo = "<p>Revise o Cadastro de Fator de Ponderação, pois existe lacuna de valores não contemplados entre as Faixas Iniciais e Finais de Qtde de Clientes!<br>";
                        break;
                    default:
                        break;
                }
                MailSender mailSender = new MailSender();

                var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html").Replace("#Conteudo", Conteudo);

                MensagemEmail.Replace("#Conteudo", Conteudo);

                mailSender.Send(mailTo, Subject, MensagemEmail.ToString(), Attachments, mailCopy);

            }
            catch (Exception ex)
            {


            }
        }


    }
}
