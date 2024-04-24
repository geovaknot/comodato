using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/GrupoModeloAPI")]
    [Authorize]
    public class GrupoModeloAPIController : BaseAPIController
    {
        /// <summary>
        /// Obtem lista completa de Modelos Ativos
        /// </summary>
        /// <param name="modeloEntity"></param>
        /// <returns>List<ModeloEntity> modelos</returns>
        //[HttpPost]
        //[Route("ObterLista")]
        //public IHttpActionResult ObterLista(GrupoModeloEntity grupoModeloEntity)
        [HttpGet]
        [Route("ObterLista")]
        //public HttpResponseMessage ObterLista(GrupoModeloEntity grupoModeloEntity)
        public HttpResponseMessage ObterLista()
        //public IHttpActionResult ObterLista() //string ccdGrupoModelo
        {
            List<GrupoModeloEntity> grupoModelos = new List<GrupoModeloEntity>();

            GrupoModeloEntity grupoModeloEntity = new GrupoModeloEntity();

            //if (ccdGrupoModelo == null)
            //    grupoModeloEntity = new GrupoModeloEntity();

            //grupoModeloEntity.CD_GRUPO_MODELO = ccdGrupoModelo;

            try
            {
                DataTableReader dataTableReader = new GrupoModeloData().ObterLista(grupoModeloEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        grupoModeloEntity = new GrupoModeloEntity();

                        CarregarGrupoModeloEntity(dataTableReader, grupoModeloEntity);

                        grupoModelos.Add(grupoModeloEntity);
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
                //return BadRequest(ex.Message);

                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);              
            }

            //JObject modelosJO = new JObject();
            //modelosJO.Add("GRUPOMODELO", JsonConvert.SerializeObject(grupoModelos, Formatting.None));
            //return Ok(modelosJO);

            //return Ok();

            return Request.CreateResponse(HttpStatusCode.OK, new { grupoModelos });
        }

        /// <summary>
        /// Obtem descricao de Grupo de Modelos
        /// </summary>
        /// <param name="idGrupoModelo"></param>
        /// <returns>List<ModeloEntity> modelos</returns>
        [HttpGet]
        [Route("ObterDados")]
        public HttpResponseMessage ObterDados(int idGrupoModelo)
        {
            List<GrupoModeloEntity> grupoModelos = new List<GrupoModeloEntity>();

            GrupoModeloEntity grupoModeloEntity = new GrupoModeloEntity();
            grupoModeloEntity.ID_GRUPO_MODELO = idGrupoModelo;

            try
            {
                DataTableReader dataTableReader = new GrupoModeloData().ObterLista(grupoModeloEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        grupoModeloEntity = new GrupoModeloEntity();

                        CarregarGrupoModeloEntity(dataTableReader, grupoModeloEntity);

                        grupoModelos.Add(grupoModeloEntity);
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
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { grupoModelos = grupoModelos });
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para ModeloEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="modeloEntity"></param>
        protected void CarregarGrupoModeloEntity(DataTableReader dataTableReader, GrupoModeloEntity grupoModeloEntity)
        {
            grupoModeloEntity.ID_GRUPO_MODELO = Convert.ToInt32(dataTableReader["ID_GRUPOMODELO"].ToString());
            grupoModeloEntity.CD_GRUPO_MODELO = Convert.ToString(dataTableReader["CD_GRUPOMODELO"]);
            grupoModeloEntity.DS_GRUPO_MODELO = Convert.ToString(dataTableReader["DS_GRUPOMODELO"]);
        }


        [HttpGet]
        [Route("ObterListaGrupoModeloSinc")]
        public IHttpActionResult ObterListaGrupoModeloSinc()
        {
            IList<GrupoModeloEntity> listaGrupoModelo = new List<GrupoModeloEntity>();
            try
            {
                //Int32 idUsuario = 60237;
                GrupoModeloData modeloData = new GrupoModeloData();
                listaGrupoModelo = modeloData.ObterListaGrupoModeloSinc();

                JObject JO = new JObject();
                //JO.Add("MODELO", JsonConvert.SerializeObject(listaModelo, Formatting.None));
                JO.Add("GRUPOMODELO", JArray.FromObject(listaGrupoModelo));
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
