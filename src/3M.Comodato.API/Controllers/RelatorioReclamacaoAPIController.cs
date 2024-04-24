using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Data;
using System.Net;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/RelatorioReclamacaoAPI")]
    [Authorize]
    public class RelatorioReclamacaoAPIController :BaseAPIController
    {
        [HttpPost]
        [Route("AtualizarRelatorioReclamacao")]
        public IHttpActionResult AtualizarRelatorioReclamacao(RelatorioReclamacaoEntity relatorioReclamacaoEntity)
        {
            try
            {
                RelatorioReclamacaoData data = new RelatorioReclamacaoData();

                if (!string.IsNullOrEmpty(relatorioReclamacaoEntity.VL_Hora_Atendimento) )
                {

                    ///Calcular o Tempo Atendimento passado
                    relatorioReclamacaoEntity.VL_Minuto_Atendimento = relatorioReclamacaoEntity.VL_Minuto_Atendimento;

                    if (Convert.ToInt32(relatorioReclamacaoEntity.VL_Hora_Atendimento) > 0)
                    {
                        if (Convert.ToInt32(relatorioReclamacaoEntity.VL_Hora_Atendimento.Substring(0, 2)) > 0)
                        {
                            relatorioReclamacaoEntity.VL_Minuto_Atendimento = relatorioReclamacaoEntity.VL_Minuto_Atendimento.Substring(relatorioReclamacaoEntity.VL_Minuto_Atendimento.IndexOf(":") + 1, 2);

                            //relatorioReclamacaoEntity.VL_Hora_Atendimento = relatorioReclamacaoEntity.VL_Hora_Atendimento + "," + relatorioReclamacaoEntity.VL_Minuto_Atendimento;
                            relatorioReclamacaoEntity.TEMPO_ATENDIMENTO = Convert.ToInt32(Math.Round(Convert.ToDouble(relatorioReclamacaoEntity.VL_Hora_Atendimento) * 60)) + Convert.ToInt32(relatorioReclamacaoEntity.VL_Minuto_Atendimento);

                        }

                        else
                        {

                            relatorioReclamacaoEntity.TEMPO_ATENDIMENTO = Convert.ToInt32(relatorioReclamacaoEntity.VL_Minuto_Atendimento.Substring(relatorioReclamacaoEntity.VL_Minuto_Atendimento.IndexOf(":") + 1, 2));

                        }
                    }
                }



                data.Alterar(relatorioReclamacaoEntity);
                InserirComentario(relatorioReclamacaoEntity.ID_RELATORIO_RECLAMACAO, Convert.ToInt32(relatorioReclamacaoEntity.ST_STATUS_RR), relatorioReclamacaoEntity.ID_USUARIO_RESPONS);
                JObject JO = new JObject();
                JO.Add("Mensagem", JsonConvert.SerializeObject(ControlesUtility.Constantes.MensagemGravacaoSucesso, Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ObterListaRelatorioReclamacaoSinc")]
        public IHttpActionResult ObterListaRelatorioReclamacaoSinc(Int64? idUsuario)
        {
            IList<RelatorioReclamacaoSincEntity> listaRelatorioReclamacao = new List<RelatorioReclamacaoSincEntity>();
            try
            {
                RelatorioReclamacaoData relatorioReclamacaoData = new RelatorioReclamacaoData();
                listaRelatorioReclamacao = relatorioReclamacaoData.ObterListaRelatorioReclamacaoSinc(idUsuario);

                JObject JO = new JObject
                {
                    { "RR", JArray.FromObject(listaRelatorioReclamacao) }
                };
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        public void InserirComentario(Int64 ID_RELATORIO_RECLAMACAO, int ST_STATUS_RR, long nidUsuario)
        {
            string cnmNome = string.Empty;


            // Busca informações
            RRStatusEntity rrStatusEntity = new RRStatusEntity();
            rrStatusEntity.ST_STATUS_RR = ST_STATUS_RR;
            DataTableReader dataTableReader = new RRStatusData().ObterLista(rrStatusEntity).CreateDataReader();

            if (dataTableReader.HasRows)
            {
                if (dataTableReader.Read())
                    rrStatusEntity.DS_COMENTARIO = dataTableReader["DS_COMENTARIO"].ToString();
            }

            if (dataTableReader != null)
            {
                dataTableReader.Dispose();
                dataTableReader = null;
            }


            if (nidUsuario > 0)
            {
                UsuarioEntity usuarioEntity = new UsuarioEntity();
                usuarioEntity.nidUsuario = nidUsuario;
                 dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        cnmNome = dataTableReader["cnmNome"].ToString();
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }

            // Cria registro em Histórico
            RRComentEntity rrComentEntity = new RRComentEntity();

            rrComentEntity.relatorioReclamacao.ID_RELATORIO_RECLAMACAO = ID_RELATORIO_RECLAMACAO;
            rrComentEntity.DS_COMENT = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy hh:mm") + " - " + cnmNome + ": " + rrStatusEntity.DS_COMENTARIO;
            rrComentEntity.usuario.nidUsuario = nidUsuario;

            new RRComentData().Inserir(ref rrComentEntity);

        }

        [HttpPost]
        [Route("Incluir")]
        public IHttpActionResult Incluir(RelatorioReclamacaoEntity relatorioReclamacaoEntity)
        {
            try
            {
                
                if (relatorioReclamacaoEntity.TEMPO_ATENDIMENTO_FORMATADO != null && relatorioReclamacaoEntity.TEMPO_ATENDIMENTO_FORMATADO != "" && relatorioReclamacaoEntity.TEMPO_ATENDIMENTO_FORMATADO != "00:00")
                {
                    var tempo = relatorioReclamacaoEntity.TEMPO_ATENDIMENTO_FORMATADO.Split(':');
                    Int32 horas = Convert.ToInt32(tempo[0]) * 60;
                    relatorioReclamacaoEntity.TEMPO_ATENDIMENTO = horas + Convert.ToInt32(tempo[1]);
                }


                new RelatorioReclamacaoData().Inserir(relatorioReclamacaoEntity);

                JObject JO = new JObject
                {
                    { "SUCESSO", true },
                    { "MENSAGEM", "" },
                    { "ID_RELATORIO_RECLAMACAO", relatorioReclamacaoEntity.ID_RELATORIO_RECLAMACAO },
                    { "TOKEN", relatorioReclamacaoEntity.TOKEN }
                };
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
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
        public IHttpActionResult Alterar(RelatorioReclamacaoEntity relatorioReclamacaoEntity)
        {
            try
            {
                new RelatorioReclamacaoData().Alterar(relatorioReclamacaoEntity);

                JObject JO = new JObject();
                JO.Add("SUCESSO", true);
                JO.Add("MENSAGEM", "");
                JO.Add("ID_RELATORIO_RECLAMACAO", relatorioReclamacaoEntity.ID_RELATORIO_RECLAMACAO);
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                JObject JO = new JObject();
                JO.Add("SUCESSO", false);
                JO.Add("MENSAGEM", ex.Message);
                return Ok(JO);
            }
        }

        [HttpPost]
        [Route("Excluir")]
        public IHttpActionResult Excluir(RelatorioReclamacaoEntity relatorioReclamacaoEntity)
        {
            try
            {
                new RelatorioReclamacaoData().Excluir(relatorioReclamacaoEntity);

                JObject JO = new JObject();
                JO.Add("SUCESSO", true);
                JO.Add("MENSAGEM", "");
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                JObject JO = new JObject();
                JO.Add("SUCESSO", false);
                JO.Add("MENSAGEM", ex.Message);
                return Ok(JO);
            }
        }

    }
}