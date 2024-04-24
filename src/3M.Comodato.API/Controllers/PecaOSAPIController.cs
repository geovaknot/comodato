using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Data;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/PecaOSAPI")]
    [Authorize]
    public class PecaOSAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Incluir")]
        public HttpResponseMessage Incluir(PecaOSDetalhamentoEntity pecaOSDetalhamentoEntity)
        {
            if (pecaOSDetalhamentoEntity.TOKEN <= 0)
                throw new Exception($"Token para integração de registro de peça da OS código {pecaOSDetalhamentoEntity.OS.ID_OS} não informado.");

            string Mensagem = string.Empty;

            try
            {
                
                if (!string.IsNullOrEmpty(pecaOSDetalhamentoEntity.QT_PECA_Formatado) && pecaOSDetalhamentoEntity.QT_PECA == 0)
                {
                    pecaOSDetalhamentoEntity.QT_PECA = Convert.ToDecimal(pecaOSDetalhamentoEntity.QT_PECA_Formatado);
                    
                }

                new PecaOSData().Inserir(ref pecaOSDetalhamentoEntity, ref Mensagem);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, 
                new { pecaOSDetalhamentoEntity.ID_PECA_OS, MENSAGEM = Mensagem, pecaOSDetalhamentoEntity.TOKEN });
        }

        [HttpPost]
        [Route("Excluir")]
        public HttpResponseMessage Excluir(PecaOSDetalhamentoEntity pecaOSDetalhamentoEntity)
        {
            PecaOSEntity pecaOSEntity = new PecaOSEntity();
            bool encontrado = false;

            try
            {
                pecaOSEntity.ID_PECA_OS = pecaOSDetalhamentoEntity.ID_PECA_OS;
                DataTableReader dataTableReader = new PecaOSData().ObterLista(pecaOSEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        pecaOSDetalhamentoEntity.OS.ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]);
                        pecaOSDetalhamentoEntity.peca.CD_PECA = dataTableReader["CD_PECA"].ToString();
                        pecaOSDetalhamentoEntity.QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]);
                        pecaOSDetalhamentoEntity.CD_TP_ESTOQUE_CLI_TEC = dataTableReader["CD_TP_ESTOQUE_CLI_TEC"].ToString();
                        encontrado = true;
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (encontrado)
                    new PecaOSData().Excluir(pecaOSDetalhamentoEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("ObterListaPecaOsSinc")]
        public IHttpActionResult ObterListaPecaOsSinc(Int64 idUsuario, int bloquearDataRetroativa = 0)
        {
            IList<PecaOSSinc> listaPecaOs = new List<PecaOSSinc>();
            try
            {
                //Int32 idUsuario = 60237;
                PecaOSData pecaOSData = new PecaOSData();
                listaPecaOs = pecaOSData.ObterListaPecaOsSinc(idUsuario);

                DateTime dtInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-60);
                listaPecaOs = listaPecaOs.Where(x => x.DT_DATA_OS >= dtInicio).ToList();

                //if (bloquearDataRetroativa == 1)
                //{
                //    DateTime dtInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //    if (DateTime.Now.Day < 6) dtInicio = dtInicio.AddMonths(-1); // Até o 5o dia do mês, retornar registros do mês anterior
                //    listaPecaOs = listaPecaOs.Where(x => x.DT_DATA_OS >= dtInicio).ToList();
                //}

                JObject JO = new JObject();
                //JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(listaPecaOs)));
                JO.Add("PECAOS", JArray.FromObject(listaPecaOs));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }
    }
}