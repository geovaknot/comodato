using System;
using System.Data;
using System.Linq;
using System.Web.Http;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/ContratoAPI")]
    [Authorize]
    public class ContratoAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("ObterListaStatus")]
        public IHttpActionResult ObterListaStatus()
        {
            try
            {
                StatusContratoEntity statusInfo = new StatusContratoEntity();
                StatusContratoData statusData = new StatusContratoData();
                var listaStatus = (from row in statusData.ObterLista(statusInfo).Rows.Cast<DataRow>()
                                   select new StatusContratoEntity()
                                   {
                                       CD_STATUS_CONTRATO = row["CD_STATUS_CONTRATO"].ToString(),
                                       DS_STATUS_CONTRATO = row["DS_STATUS_CONTRATO"].ToString()
                                   }).ToList();

                JObject JO = new JObject();
                JO.Add("STATUS", JsonConvert.SerializeObject(listaStatus, Formatting.None));
                return Ok(JO);

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ObterClausula")]
        public IHttpActionResult ObterClausula(string modeloContrato)
        {
            try
            {
                string clausulaContrato = string.Empty;

                ParametroEntity parametroFiltro = new ParametroEntity();
                parametroFiltro.ccdParametro = modeloContrato;

                ParametroData parametroData = new ParametroData();
                using (DataTable dtParametro = parametroData.ObterLista(parametroFiltro))
                {
                    if (dtParametro.Rows.Count > 0)
                    {
                        if (dtParametro.Rows[0]["cvlParametro"] != DBNull.Value)
                        {
                            clausulaContrato = dtParametro.Rows[0]["cvlParametro"].ToString();
                        }
                    }
                }

                JObject JO = new JObject();
                JO.Add("CLAUSULA", JsonConvert.SerializeObject(clausulaContrato, Formatting.None));
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
