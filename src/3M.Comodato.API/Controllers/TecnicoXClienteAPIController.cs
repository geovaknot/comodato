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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/TecnicoXClienteAPI")]
    [Authorize]
    public class TecnicoXClienteAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Incluir")]
        public HttpResponseMessage Incluir(TecnicoClienteEntity tecnicoClienteEntity)
        {
            try
            {
                new TecnicoClienteData().Inserir(tecnicoClienteEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("Excluir")]
        public HttpResponseMessage Excluir(TecnicoClienteEntity tecnicoClienteEntity)
        {
            try
            {
                new TecnicoClienteData().Excluir(tecnicoClienteEntity);
                new TecnicoClienteData().Reordenar(tecnicoClienteEntity, "R");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Executa reordenação de TecnicoCliente via CD_ORDEM
        /// </summary>
        /// <param name="tecnicoClienteReordernar"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Reordenar")]
        public HttpResponseMessage Reordenar(TecnicoClienteReordenar tecnicoClienteReordernar)
        {
            try
            {
                TecnicoClienteEntity tecnicoClienteEntity = new TecnicoClienteEntity();
                tecnicoClienteEntity.cliente.CD_CLIENTE = tecnicoClienteReordernar.cliente.CD_CLIENTE;
                tecnicoClienteEntity.CD_ORDEM = tecnicoClienteReordernar.CD_ORDEM;

                new TecnicoClienteData().Reordenar(tecnicoClienteEntity, tecnicoClienteReordernar.TP_ACAO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Consulta Técnicos vinculados ao Cliente
        /// </summary>
        /// <param name="tecnicoClienteEntity"></param>
        /// <returns>List<TecnicoClienteEntity> tecnicos</returns>
        [HttpPost]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(TecnicoClienteEntity tecnicoClienteEntity)
        {
            List<TecnicoClienteEntity> tecnicos = new List<TecnicoClienteEntity>();

            try
            {
                if (tecnicoClienteEntity == null)
                    tecnicoClienteEntity = new TecnicoClienteEntity();

                DataTableReader dataTableReader = new TecnicoClienteData().ObterLista(tecnicoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        tecnicoClienteEntity = new TecnicoClienteEntity();
                        tecnicoClienteEntity.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
                        tecnicoClienteEntity.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                        tecnicoClienteEntity.cliente.CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]);
                        tecnicoClienteEntity.cliente.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString();
                        tecnicoClienteEntity.CD_ORDEM = Convert.ToInt32("0" + dataTableReader["CD_ORDEM"]);
                        tecnicos.Add(tecnicoClienteEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { tecnicos = tecnicos });
        }

        /// <summary>
        /// Obtem lista de Clientes de um Tecnico - para compor agenda de atendimento
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns>Lista de Clientes de um tecnico</returns>
        [HttpGet]
        [Route("ObterListaTecnicoClienteSinc")]
        public IHttpActionResult ObterListaTecnicoClienteSinc(Int64 idUsuario)
        {
            IList<TecnicoClienteSinc> listaTecnicoCliente = new List<TecnicoClienteSinc>();
            try
            {
                //Int32 idUsuario = 60237;
                TecnicoClienteData tecnicoClienteData = new TecnicoClienteData();
                listaTecnicoCliente = tecnicoClienteData.ObterListaTecnicoClienteSinc(idUsuario);

                JObject JO = new JObject();
                //JO.Add("TECNICO_CLIENTE", JsonConvert.SerializeObject(listaTecnicoCliente, Formatting.None));
                JO.Add("TECNICO_CLIENTE", JArray.FromObject(listaTecnicoCliente));
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