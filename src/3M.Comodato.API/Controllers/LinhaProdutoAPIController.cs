using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/LinhaProdutoAPI")]
    [Authorize]
    public class LinhaProdutoAPIController : BaseAPIController
    {
        /// <summary>
        /// Consulta de linhas de produto
        /// </summary>
        /// <param name="linhaProdutoEntity"></param>
        /// <returns>List<LinhaProdutoEntity>produtos</returns>
        [HttpGet]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(LinhaProdutoEntity linhaProdutoEntity)
        {
            List<LinhaProdutoEntity> produtos = new List<LinhaProdutoEntity>();

            try
            {
                if (linhaProdutoEntity == null)
                    linhaProdutoEntity = new LinhaProdutoEntity();

                DataTableReader dataTableReader = new LinhaProdutoData().ObterLista(linhaProdutoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        linhaProdutoEntity = new LinhaProdutoEntity();
                        linhaProdutoEntity.CD_LINHA_PRODUTO = Convert.ToInt32(dataTableReader["CD_LINHA_PRODUTO"]);
                        linhaProdutoEntity.DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString();
                        produtos.Add(linhaProdutoEntity);
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
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { produtos = produtos });
        }

        [HttpGet]
        [Route("ObterListaGp")]
        public HttpResponseMessage ObterListaGp()
        {   
            List<Front.Models.LinhaProdutoGp> produtos = new List<Front.Models.LinhaProdutoGp>();

            string[] NomeGrupos = { "Fechadores", "Identificação", "Unitização" };
            for (int i = 0; i < 3; i++)
            {
                Front.Models.LinhaProdutoGp Gp = new Front.Models.LinhaProdutoGp();
                Gp = new Front.Models.LinhaProdutoGp();
                Gp.CD_LINHA_PRODUTO = i;
                Gp.DS_LINHA_PRODUTO = NomeGrupos[i];
                produtos.Add(Gp);
            }
           

            return Request.CreateResponse(HttpStatusCode.OK, new { produtos = produtos });
        }

        [HttpGet]
        [Route("ObterListaLinhaProdutoSinc")]
        public IHttpActionResult ObterListaLinhaProdutoSinc()
        {
            IList<LinhaProdutoEntity> listaLinhaProduto = new List<LinhaProdutoEntity>();
            try
            {
                LinhaProdutoData linhaProdutoData = new LinhaProdutoData();
                listaLinhaProduto = linhaProdutoData.ObterListaLinhaProdutoSinc();

                JObject JO = new JObject();
                JO.Add("LINHA_PRODUTO", JArray.FromObject(listaLinhaProduto));
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
