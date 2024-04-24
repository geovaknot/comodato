using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/PecaAPI")]
    [Authorize]
    public class PecaAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("ObterListaAtivos")]
        public IHttpActionResult ObterListaAtivos(PecaEntity filtroPeca)
        {
            IList<PecaEntity> listaPecas = null;

            try
            {
                if (null == filtroPeca)
                {
                    filtroPeca = new PecaEntity();
                }

                filtroPeca.FL_ATIVO_PECA = "S";

                if (filtroPeca.TP_PECA == "N")
                {
                    filtroPeca.TP_PECA = null;
                }

                listaPecas = ObterListaPecas(filtroPeca);

                if (filtroPeca.TP_PECA == "E") {
                    listaPecas = listaPecas.Where(x => x.TP_PECA == "E").ToList();
                }
                else
                {
                    listaPecas = listaPecas.Where(x => x.TP_PECA != "E").ToList();
                }

                foreach (var peca in listaPecas)
                {
                    peca.CD_PECA = peca.CD_PECA.ToUpper();
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("PECA", JsonConvert.SerializeObject(listaPecas, Formatting.None));
            return Ok(pecasJO);
            //return Request.CreateResponse(HttpStatusCode.OK, new { result = listaPecas });
        }

        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(PecaEntity filtroPeca)
        {
            IList<PecaEntity> listaPecas = null;

            try
            {
                if (filtroPeca == null)
                {
                    filtroPeca = new PecaEntity();
                }

                listaPecas = ObterListaPecas(filtroPeca);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("PECA", JsonConvert.SerializeObject(listaPecas, Formatting.None));
            return Ok(pecasJO);
        }

        private IList<PecaEntity> ObterListaPecas(PecaEntity filtroPeca)
        {
            IList<PecaEntity> listaPecas;
            Func<DataRow, PecaEntity> entityConverter = new Func<DataRow, PecaEntity>((r) =>
            {
                PecaEntity pecaEntity = new PecaEntity();
                pecaEntity.CD_PECA = r["CD_PECA"].ToString();
                pecaEntity.DS_PECA = r["DS_PECA"].ToString();

                pecaEntity.TX_UNIDADE = r["TX_UNIDADE"] != DBNull.Value ? r["TX_UNIDADE"].ToString() : string.Empty;
                pecaEntity.QTD_ESTOQUE = r["QTD_ESTOQUE"] != DBNull.Value ? Convert.ToDecimal(r["QTD_ESTOQUE"]) : 0;
                pecaEntity.QTD_MINIMA = r["QTD_MINIMA"] != DBNull.Value ? Convert.ToDecimal(r["QTD_MINIMA"]) : 0;
                pecaEntity.VL_PECA = r["VL_PECA"] != DBNull.Value ? Convert.ToDecimal(r["VL_PECA"]) : 0;
                pecaEntity.TP_PECA = r["TP_PECA"] != DBNull.Value ? r["TP_PECA"].ToString() : string.Empty;

                pecaEntity.FL_ATIVO_PECA = r["FL_ATIVO_PECA"].ToString();

                return pecaEntity;
            });

            PecaData pecaData = new PecaData();
            using (DataTable dtPeca = pecaData.ObterListaNew(filtroPeca))
            {
                listaPecas = (from p in dtPeca.Rows.Cast<DataRow>()
                              select entityConverter(p)).ToList();

            }

            return listaPecas;
        }

        [HttpGet]
        [Route("Obter")]
        public IHttpActionResult Obter(string CD_PECA)
        {
            PecaEntity pecaEntity = new PecaEntity();

            try
            {
                pecaEntity.CD_PECA = CD_PECA;
                DataTableReader dataTableReader = new PecaData().ObterListaNew(pecaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        pecaEntity.CD_PECA = dataTableReader["CD_PECA"].ToString();
                        pecaEntity.DS_PECA = dataTableReader["DS_PECA"].ToString();

                        pecaEntity.TX_UNIDADE = dataTableReader["TX_UNIDADE"] != DBNull.Value ? dataTableReader["TX_UNIDADE"].ToString() : string.Empty;
                        pecaEntity.QTD_ESTOQUE = dataTableReader["QTD_ESTOQUE"] != DBNull.Value ? Convert.ToDecimal(dataTableReader["QTD_ESTOQUE"]) : 0;
                        pecaEntity.QTD_MINIMA = dataTableReader["QTD_MINIMA"] != DBNull.Value ? Convert.ToDecimal(dataTableReader["QTD_MINIMA"]) : 0;
                        pecaEntity.VL_PECA = dataTableReader["VL_PECA"] != DBNull.Value ? Convert.ToDecimal(dataTableReader["VL_PECA"]) : 0;
                        pecaEntity.TP_PECA = dataTableReader["TP_PECA"] != DBNull.Value ? dataTableReader["TP_PECA"].ToString() : string.Empty;

                        pecaEntity.FL_ATIVO_PECA = dataTableReader["FL_ATIVO_PECA"].ToString();
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
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("PECA", JsonConvert.SerializeObject(pecaEntity, Formatting.None));
            return Ok(pecasJO);
            //return Request.CreateResponse(HttpStatusCode.OK, new { result = listaPecas });
        }

        [HttpGet]
        [Route("ObterListaPecaSinc")]
        public IHttpActionResult ObterListaPecaSinc(string cd_grupo_modelo = "")
        {
            IList<PecaEntity> listaPeca = new List<PecaEntity>();
            try
            {
                PecaData pecaData = new PecaData();
                listaPeca = pecaData.ObterListaPecaSinc(cd_grupo_modelo);

                Int64 contador = 1;
                if (listaPeca != null)
                {
                    foreach (var itemLista in listaPeca)
                    {
                        itemLista.ID_PLANO_ZERO = contador;
                        contador++;
                    }
                }
                
                JObject JO = new JObject();
                //JO.Add("PECA", JsonConvert.SerializeObject(listaPeca, Formatting.None));
                JO.Add("PECA", JArray.FromObject(listaPeca));
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
