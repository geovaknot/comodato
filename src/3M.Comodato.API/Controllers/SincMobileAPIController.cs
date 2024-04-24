using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json;
using System.IO;

namespace _3M.Comodato.API.Controllers
{
    [Authorize]
    public class SincMobileAPIController : BaseAPIController
    {
        /// <summary>
        /// Obtem lista completa de Modelos Ativos
        /// </summary>
        /// <param name="SincMobile"></param>
        /// <returns>List<ModeloEntity> modelos</returns>
        [HttpPost]
        public HttpResponseMessage Sincc(string teste)
        {
            return Request.CreateResponse(HttpStatusCode.OK, "123456" + teste);
        }

        [HttpGet]
        public IHttpActionResult  SincRegiaoLista() //  RegiaoEntity regiaoEntity = null
        {
            RegiaoEntity regiaoEntity = null;
            List<RegiaoEntity> regioes = new List<RegiaoEntity>();
            try
            {
                if (regiaoEntity == null) regiaoEntity = new RegiaoEntity();
                DataTableReader dataTableReader = new RegiaoData().ObterLista(regiaoEntity).CreateDataReader();
                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        regiaoEntity = new RegiaoEntity();
                        regiaoEntity.CD_REGIAO = dataTableReader["CD_REGIAO"].ToString();
                        regiaoEntity.DS_REGIAO = dataTableReader["DS_REGIAO"].ToString();
                        regioes.Add(regiaoEntity);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
                var jsonList = JsonConvert.SerializeObject(regioes, Formatting.None);
                return Ok(jsonList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
