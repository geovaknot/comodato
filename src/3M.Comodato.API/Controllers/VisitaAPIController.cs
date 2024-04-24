using _3M.Comodato.Data;
//using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
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
    [RoutePrefix("api/VisitaAPI")]
    [Authorize]
    public class VisitaAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Incluir")]
        public HttpResponseMessage Incluir(VisitaEntity visitaEntity)
        {
            try
            {
                visitaEntity.DT_DATA_VISITA = DateTime.Now;
                new VisitaData().Inserir(ref visitaEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_VISITA = visitaEntity.ID_VISITA });
        }

        [HttpPost]
        [Route("Alterar")]
        public HttpResponseMessage Alterar(VisitaEntity visitaEntity)
        {
            string Mensagem = string.Empty;
            try
            {
                visitaEntity.DT_DATA_VISITA = DateTime.Now;

                // Só permitir Finalizar a Visita se todas as OS estiverem Finalizadas ou Canceladas
                if (visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Finalizada))
                {
                    if (new OSData().VerificarOSFechadaCancelada(ID_VISITA: visitaEntity.ID_VISITA) == false)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { MENSAGEM = "Visita só pode ser <strong>FINALIZADA</strong> se TODAS as OS's estiverem Finalizadas ou Canceladas!" });
                    }

                    // Se todas as OS estão canceladas, ao finalizar a Visita está visita fica Cancelada
                    if (new OSData().VerificarOSFechadaCancelada(ID_VISITA: visitaEntity.ID_VISITA, VerificarSoCanceladas: true) == true)
                    {
                        visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Cancelada);
                    }
                }
                // Só permitir Cancelar a Visita se todas as OS estiverem Canceladas
                else if (visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Cancelada))
                {
                    if (new OSData().VerificarOSFechadaCancelada(ID_VISITA: visitaEntity.ID_VISITA, VerificarSoCanceladas: true) == false)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { MENSAGEM = "Visita só pode ser <strong>CANCELADA</strong> se TODAS as OS's estiverem Canceladas!" });
                    }
                }

                AjustaRegrasVisitaOS(visitaEntity.ID_VISITA, visitaEntity.nidUsuarioAtualizacao, ref Mensagem);

                new AnaliseSatisfacaoAPIController().NotificarAvaliacaoDisponivel(visitaEntity);

                new VisitaData().Alterar(visitaEntity);
 
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_VISITA = visitaEntity.ID_VISITA, MENSAGEM = Mensagem });
        }

        /// <summary>
        /// Consulta a visita informada
        /// </summary>
        /// <param name="ID_VISITA"></param>
        /// <returns>VisitaEntity visitaEntity</returns>
        [HttpGet]
        [Route("Obter")]
        public HttpResponseMessage Obter(int ID_VISITA)
        {
            VisitaEntity visitaEntity = new VisitaEntity();
            Models.VisitaTecnica visitaTecnica = new Models.VisitaTecnica();

            try
            {
                visitaEntity.ID_VISITA = ID_VISITA;
                DataTableReader dataTableReader = new VisitaData().ObterLista(visitaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CarregarVisitaEntity(dataTableReader, visitaEntity);
                        CarregarVisitaModel(dataTableReader, visitaTecnica);

                        // Status Cancelada, Finalizada ou Confirmada transfere a Data/Hora do Log para os campos FIM
                        if (Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]) == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Cancelada) ||
                            Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]) == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Finalizada) ||
                            Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]) == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Confirmada))
                        {
                            //SL00036628
                            // Busca a Data/Hora do Log 
                            LogStatusVisitaEntity logStatusVisitaEntity2 = new LogStatusVisitaEntity();
                            logStatusVisitaEntity2.visita.ID_VISITA = ID_VISITA;
                            logStatusVisitaEntity2.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Finalizada);
                            var dataTableReader2 = new LogStatusVisitaData().ObterLista(logStatusVisitaEntity2).CreateDataReader();

                            if (dataTableReader2.HasRows)
                            {
                                if (dataTableReader2.Read())
                                {
                                    visitaTecnica.DT_DATA_VISITA_FIM = Convert.ToDateTime(dataTableReader2["DT_DATA_LOG_VISITA"]).ToString("dd/MM/yyyy");
                                    visitaTecnica.HR_FIM = Convert.ToDateTime(dataTableReader2["DT_DATA_LOG_VISITA"]).ToString("HH:mm");

                                    visitaTecnica.DT_DATA_VISITA = visitaTecnica.DT_DATA_VISITA_FIM;//Mais na frente no código troca pela hh:mm loge de Abertura de visita
                                    visitaTecnica.HR_INICIO = visitaTecnica.HR_FIM; 
                                }
                            }
                            else
                            {
                                visitaTecnica.DT_DATA_VISITA_FIM = visitaTecnica.DT_DATA_VISITA;
                                visitaTecnica.HR_FIM = visitaTecnica.HR_INICIO;

                            }

                            if (dataTableReader2 != null)
                            {
                                dataTableReader2.Dispose();
                                dataTableReader2 = null;
                            }


                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                // Busca a Data/Hora do Log NOVA
                LogStatusVisitaEntity logStatusVisitaEntity = new LogStatusVisitaEntity();
                logStatusVisitaEntity.visita.ID_VISITA = ID_VISITA;
                logStatusVisitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Aberta);
                dataTableReader = new LogStatusVisitaData().ObterLista(logStatusVisitaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        visitaEntity.DT_DATA_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_VISITA"]);
                        visitaTecnica.DT_DATA_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_VISITA"]).ToString("dd/MM/yyyy");
                        visitaTecnica.HR_INICIO = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_VISITA"]).ToString("HH:mm");
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

            return Request.CreateResponse(HttpStatusCode.OK, new { visita = visitaEntity, visitaTecnica = visitaTecnica });
        }

        [HttpPost]
        [Route("ObterLista")]
        //public IHttpActionResult ObterLista(VisitaEntity visitaEntity)
        public HttpResponseMessage ObterLista(VisitaEntity visitaEntity)
        {
            List<VisitaEntity> listaVisita = new List<VisitaEntity>();
            List<Models.VisitaTecnica> listaVisitaTecnica = new List<Models.VisitaTecnica>();

            try
            {
                DataTableReader dataTableReader = new VisitaData().ObterLista(visitaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        visitaEntity = new VisitaEntity();
                        Models.VisitaTecnica visitaTecnica = new Models.VisitaTecnica();

                        CarregarVisitaEntity(dataTableReader, visitaEntity);
                        CarregarVisitaModel(dataTableReader, visitaTecnica);

                        listaVisita.Add(visitaEntity);
                        listaVisitaTecnica.Add(visitaTecnica);
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
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            //JObject jObject = new JObject();
            //string teste = JsonConvert.SerializeObject(listaVisita, Formatting.None);            
            //jObject.Add("listaVisita", JToken.Parse(teste));
            //jObject.Add("listaVisita", JsonConvert.SerializeObject(listaVisita, Formatting.None));
            //jObject.Add("listaVisitaTecnica", JsonConvert.SerializeObject(listaVisitaTecnica, Formatting.None));

            //return Ok(jObject);
            return Request.CreateResponse(HttpStatusCode.OK, new { result = listaVisita });
        }

        [HttpGet]
        [Route("RemontarIdKey")]
        public HttpResponseMessage RemontarIdKey(Int64 ID_AGENDA, Int64 ID_VISITA, Int64 CD_CLIENTE, string CD_TECNICO, Int64 ID_OS, string tipoOrigemPagina)
        {
            string idKey = string.Empty;

            try
            {
                idKey = ControlesUtility.Criptografia.Criptografar(ID_AGENDA.ToString() + "|" + ID_VISITA.ToString() + "|" + CD_CLIENTE.ToString() + "|" + CD_TECNICO + "|" + ID_OS.ToString() + "|" + tipoOrigemPagina);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { idKey = idKey });
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para VisitaEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="visitaEntity"></param>
        protected void CarregarVisitaEntity(DataTableReader dataTableReader, VisitaEntity visitaEntity)
        {
            visitaEntity.ID_VISITA = Convert.ToInt64(dataTableReader["ID_VISITA"]);
            visitaEntity.DT_DATA_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_VISITA"]);
            visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]);
            visitaEntity.tpStatusVisitaOS.DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString();
            visitaEntity.cliente.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString();
            visitaEntity.cliente.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
            visitaEntity.cliente.EN_ESTADO = dataTableReader["EN_ESTADO"].ToString();
            visitaEntity.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
            visitaEntity.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
            visitaEntity.DS_OBSERVACAO = dataTableReader["DS_OBSERVACAO"].ToString();
            visitaEntity.DS_NOME_RESPONSAVEL = dataTableReader["DS_NOME_RESPONSAVEL"].ToString();

            if (dataTableReader["FL_ENVIO_EMAIL_PESQ"] != DBNull.Value)
            {
                visitaEntity.FL_ENVIO_EMAIL_PESQ = dataTableReader["FL_ENVIO_EMAIL_PESQ"].ToString();
            }
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para Models.VisitaTecnica
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="visitaTecnica"></param>
        protected void CarregarVisitaModel(DataTableReader dataTableReader, Models.VisitaTecnica visitaTecnica)
        {
            visitaTecnica.ID_VISITA = Convert.ToInt64(dataTableReader["ID_VISITA"]);
            visitaTecnica.DT_DATA_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_VISITA"]).ToString("dd/MM/yyyy");
            visitaTecnica.HR_INICIO = Convert.ToDateTime(dataTableReader["DT_DATA_VISITA"]).ToString("HH:mm");
            visitaTecnica.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]);
            visitaTecnica.tpStatusVisitaOS.DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString();
            visitaTecnica.cliente.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString();
            visitaTecnica.cliente.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
            visitaTecnica.cliente.EN_ESTADO = dataTableReader["EN_ESTADO"].ToString();
            visitaTecnica.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
            visitaTecnica.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
            visitaTecnica.DS_OBSERVACAO = dataTableReader["DS_OBSERVACAO"].ToString();
            visitaTecnica.DS_NOME_RESPONSAVEL = dataTableReader["DS_NOME_RESPONSAVEL"].ToString();

            if (dataTableReader["FL_ENVIO_EMAIL_PESQ"] != DBNull.Value)
            {
                visitaTecnica.FL_ENVIO_EMAIL_PESQ = dataTableReader["FL_ENVIO_EMAIL_PESQ"].ToString();
            }

        }


        [HttpGet]
        [Route("ObterListaVisitaSinc")]
        public IHttpActionResult ObterListaVisitaSinc(Int64 idUsuario)
        {
            IList<VisitaSinc> listaVisita = new List<VisitaSinc>();
            try
            {
                //Int32 idUsuario = 60237;
                VisitaData visitaData = new VisitaData();
                listaVisita = visitaData.ObterListaVisitaSinc(idUsuario);

                JObject JO = new JObject();
                //JO.Add("VISITA", JsonConvert.SerializeObject(listaVisita, Formatting.None));
                JO.Add("VISITA", JArray.FromObject(listaVisita));
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