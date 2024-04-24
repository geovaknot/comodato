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

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/AvaliacaoVisitaAPI")]
    [Authorize]
    public class AvaliacaoVisitaAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Incluir")]
        public HttpResponseMessage Incluir(AvaliacaoVisitaEntity avaliacaoVisitaEntity)
        {
            try
            {
                if (avaliacaoVisitaEntity.DT_AVALIACAO_VISITA == null)
                    avaliacaoVisitaEntity.DT_AVALIACAO_VISITA = DateTime.Now;

                new AvaliacaoVisitaData().Inserir(ref avaliacaoVisitaEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_AVALIACAO_VISITA = avaliacaoVisitaEntity.ID_AVALIACAO_VISITA });
        }

        [HttpPost]
        [Route("Alterar")]
        public HttpResponseMessage Alterar(AvaliacaoVisitaEntity avaliacaoVisitaEntity)
        {
            try
            {
                new AvaliacaoVisitaData().Alterar(avaliacaoVisitaEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_AVALIACAO_VISITA = avaliacaoVisitaEntity.ID_AVALIACAO_VISITA});
        }

        [HttpPost]
        [Route("DesfazerByVisita")]
        public HttpResponseMessage DesfazerByVisita(Int64 ID_VISITA, Int64 nidUsuarioAtualizacao)
        {
            try
            {
                new AvaliacaoVisitaData().Desfazer(ID_VISITA, nidUsuarioAtualizacao);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_VISITA = ID_VISITA });
        }

        /// <summary>
        /// Consulta a Avaliação Visita por ID_VISITA
        /// </summary>
        /// <param name="ID_VISITA"></param>
        /// <returns>AvaliacaoVisitaEntity avaliacaoVisitaEntity</returns>
        [HttpGet]
        [Route("ObterByVisita")]
        public HttpResponseMessage ObterByVisita(Int64 ID_VISITA)
        {
            AvaliacaoVisitaEntity avaliacaoVisitaEntity = new AvaliacaoVisitaEntity();

            try
            {
                avaliacaoVisitaEntity.visita.ID_VISITA = ID_VISITA;
                DataTableReader dataTableReader = new AvaliacaoVisitaData().ObterLista(avaliacaoVisitaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CarregarAvaliacaoVisitaEntity(dataTableReader, avaliacaoVisitaEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { avaliacaoVisita = avaliacaoVisitaEntity });
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para AgendaEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="avaliacaoVisitaEntity"></param>
        protected void CarregarAvaliacaoVisitaEntity(DataTableReader dataTableReader, AvaliacaoVisitaEntity avaliacaoVisitaEntity)
        {
            avaliacaoVisitaEntity.ID_AVALIACAO_VISITA = Convert.ToInt64(dataTableReader["ID_AVALIACAO_VISITA"]);
            avaliacaoVisitaEntity.usuario.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
            if(dataTableReader["nidUsuario"] != DBNull.Value)
                avaliacaoVisitaEntity.DT_AVALIACAO_VISITA = Convert.ToDateTime(dataTableReader["DT_AVALIACAO_VISITA"]);
            avaliacaoVisitaEntity.DS_AVALIACAO_VISITA = dataTableReader["DS_AVALIACAO_VISITA"].ToString();
            avaliacaoVisitaEntity.NR_GRAU_AVALIACAO = Convert.ToInt32(dataTableReader["NR_GRAU_AVALIACAO"]);
            avaliacaoVisitaEntity.visita.ID_VISITA = Convert.ToInt64(dataTableReader["ID_VISITA"]);
        }

    }
}