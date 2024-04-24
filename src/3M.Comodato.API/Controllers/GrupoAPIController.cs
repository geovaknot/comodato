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
    [RoutePrefix("api/GrupoAPI")]
    [Authorize]
    public class GrupoAPIController : BaseAPIController
    {
        /// <summary>
        /// Consulta de grupos
        /// </summary>
        /// <param name="grupoEntity"></param>
        /// <returns>List<GrupoEntity> grupos</returns>
        [HttpGet]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(GrupoEntity grupoEntity)
        {
            List<GrupoEntity> grupos = new List<GrupoEntity>();

            try
            {
                if (grupoEntity == null)
                    grupoEntity = new GrupoEntity();

                DataTableReader dataTableReader = new GrupoData().ObterLista(grupoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        grupoEntity = new GrupoEntity();
                        CarregarGrupoEntity(dataTableReader, grupoEntity);
                        grupos.Add(grupoEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { grupos = grupos });
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para GrupoEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="grupoEntity"></param>
        protected void CarregarGrupoEntity(DataTableReader dataTableReader, GrupoEntity grupoEntity)
        {
            grupoEntity.CD_GRUPO = dataTableReader["CD_GRUPO"].ToString();
            grupoEntity.DS_GRUPO = dataTableReader["DS_GRUPO"].ToString();
        }
    }

    
}