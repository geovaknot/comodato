using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Data;
using System.Net;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/VendedorAPI")]
    [Authorize]
    public class VendedorAPIController : BaseAPIController
    {
        /// <summary>
        /// Consulta de vendedores
        /// </summary>
        /// <param name="vendedorEntity"></param>
        /// <returns>List<VendedorEntity> vendedores</returns>
        [HttpGet, HttpPost]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(VendedorEntity vendedorEntity)
        {
            List<VendedorEntity> vendedores = new List<VendedorEntity>();

            try
            {
                if (vendedorEntity == null)
                    vendedorEntity = new VendedorEntity();

                DataTableReader dataTableReader = new VendedorData().ObterLista(vendedorEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        vendedorEntity = new VendedorEntity();
                        CarregarVendedorEntity(dataTableReader, vendedorEntity);
                        vendedores.Add(vendedorEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { vendedores = vendedores });
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para VendedorEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="vendedorEntity"></param>
        protected void CarregarVendedorEntity(DataTableReader dataTableReader, VendedorEntity vendedorEntity)
        {
            vendedorEntity.CD_VENDEDOR = Convert.ToInt64(dataTableReader["CD_VENDEDOR"]);
            vendedorEntity.NM_VENDEDOR = dataTableReader["NM_VENDEDOR"].ToString();
            vendedorEntity.NM_APE_VENDEDOR = dataTableReader["NM_APE_VENDEDOR"].ToString();
            vendedorEntity.EN_ENDERECO = dataTableReader["EN_ENDERECO"].ToString();
            vendedorEntity.EN_BAIRRO = dataTableReader["EN_BAIRRO"].ToString();
            vendedorEntity.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
            vendedorEntity.EN_ESTADO = dataTableReader["EN_ESTADO"].ToString();
            vendedorEntity.EN_CEP = dataTableReader["EN_CEP"].ToString();
            vendedorEntity.EN_CX_POSTAL = dataTableReader["EN_CX_POSTAL"].ToString();
            vendedorEntity.TX_TELEFONE = dataTableReader["TX_TELEFONE"].ToString();
            vendedorEntity.TX_FAX = dataTableReader["TX_FAX"].ToString();
            vendedorEntity.TX_EMAIL = dataTableReader["TX_EMAIL"].ToString();
            vendedorEntity.FL_ATIVO = dataTableReader["FL_ATIVO"].ToString();
        }
    }


}