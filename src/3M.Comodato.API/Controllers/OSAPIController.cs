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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/OSAPI")]
    [Authorize]
    public class OSAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("Incluir")]
        public HttpResponseMessage Incluir(OSEntity osEntity)
        {
            try
            {
                osEntity.DT_DATA_ABERTURA = DateTime.Now;
                new OSData().Inserir(ref osEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_OS = osEntity.ID_OS });
        }

        [HttpPost]
        [Route("Alterar")]
        public HttpResponseMessage Alterar(OSEntity osEntity)
        {
            string Mensagem = string.Empty;

            try
            {
                osEntity.DT_DATA_ABERTURA = DateTime.Now;


                // Caso este atendimento já possua uma OS ABERTA para o ativo selecionado, informar com mensagem de aviso
                OSEntity _osEntity = new OSEntity();

                _osEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOS.Aberta);
                _osEntity.ativoFixo.CD_ATIVO_FIXO = osEntity.ativoFixo.CD_ATIVO_FIXO;
                DataTableReader dataTableReader = new OSData().ObterLista(_osEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        if (Convert.ToInt64(dataTableReader["ID_OS"]) != osEntity.ID_OS)
                            Mensagem = "Este atendimento já <strong>possui uma OS</strong> para este Ativo em <strong>ABERTO</strong>!<br/>Favor alterar o atendimento aberto...";
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (!string.IsNullOrEmpty(Mensagem))
                    return Request.CreateResponse(HttpStatusCode.OK, new { MENSAGEM = Mensagem });

                AjustaRegrasVisitaOS(osEntity.visita.ID_VISITA, osEntity.nidUsuarioAtualizacao, ref Mensagem);

                // Caso seja escolhido o status da OS como 'Confirmada' mudar automaticamente para status 'Finalizada'
                if (osEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOS.Confirmada))
                    osEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOS.Finalizada);

                new OSData().Alterar(osEntity);

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_OS = osEntity.ID_OS, MENSAGEM = Mensagem });
        }

        [HttpPost]
        [Route("ExcluirSemAtivo")]
        public HttpResponseMessage ExcluirSemAtivo(OSEntity osEntity)
        {
            try
            {
                new OSData().ExcluirSemAtivo(osEntity);
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
        /// Consulta a visita informada
        /// </summary>
        /// <param name="ID_VISITA"></param>
        /// <returns>OSEntity osEntity</returns>
        [HttpGet]
        [Route("Obter")]
        public HttpResponseMessage Obter(int ID_OS)
        {
            OSEntity osEntity = new OSEntity();
            Models.PreenchimentoOS preenchimentoOS = new Models.PreenchimentoOS();

            try
            {
                osEntity.ID_OS = ID_OS;
                DataTableReader dataTableReader = new OSData().ObterLista(osEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CarregarOSEntity(dataTableReader, osEntity);
                        CarregarOSModel(dataTableReader, preenchimentoOS);

                        // Status Cancelada, Finalizada ou Confirmada transfere a Data/Hora do Log para os campos FIM
                        if (Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]) == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOS.Cancelada) ||
                            Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]) == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOS.Finalizada) ||
                            Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]) == Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOS.Confirmada))
                        {

                            //SL00036628
                            // Busca a Data/Hora do Log 
                            LogStatusOSEntity logStatusOSEntity2 = new LogStatusOSEntity();
                            logStatusOSEntity2.OS.ID_OS = ID_OS;
                            logStatusOSEntity2.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]);
                            var dataTableReader2 = new LogStatusOSData().ObterLista(logStatusOSEntity2).CreateDataReader();

                            if (dataTableReader2.HasRows)
                            {
                                if (dataTableReader2.Read())
                                {
                                    preenchimentoOS.DT_DATA_ABERTURA_FIM = Convert.ToDateTime(dataTableReader2["DT_DATA_LOG_OS"]).ToString("dd/MM/yyyy");
                                    preenchimentoOS.HR_FIM = Convert.ToDateTime(dataTableReader2["DT_DATA_LOG_OS"]).ToString("HH:mm");

                                    preenchimentoOS.DT_DATA_ABERTURA = preenchimentoOS.DT_DATA_ABERTURA_FIM;//Mais na frente no código troca pela hh:mm loge de Abertura de OS
                                    preenchimentoOS.HR_INICIO = preenchimentoOS.HR_FIM;
                                }
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
                LogStatusOSEntity logStatusOSEntity = new LogStatusOSEntity();
                logStatusOSEntity.OS.ID_OS = ID_OS;
                logStatusOSEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOS.Aberta);
                dataTableReader = new LogStatusOSData().ObterLista(logStatusOSEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        osEntity.DT_DATA_ABERTURA = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_OS"]);
                        preenchimentoOS.DT_DATA_ABERTURA = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_OS"]).ToString("dd/MM/yyyy");
                        preenchimentoOS.HR_INICIO = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_OS"]).ToString("HH:mm");
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

            return Request.CreateResponse(HttpStatusCode.OK, new { OS = osEntity, preenchimentoOS = preenchimentoOS });
        }

        [HttpPost]
        [Route("ObterListaVisita")]
        public IHttpActionResult ObterListaVisita(Int64 CD_CLIENTE, OSEntity osEntityFilter)//)
        {
            List<OSEntity> listaOS = new List<OSEntity>();
            List<Models.PreenchimentoOS> listaPreenchimentoOS = new List<Models.PreenchimentoOS>();

            try
            {
                //OSEntity _osEntity = new OSEntity();
                if (null == osEntityFilter)
                    osEntityFilter = new OSEntity();

                DataTableReader dataTableReader = new OSData().ObterListaVisita(osEntityFilter, null, CD_CLIENTE).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        OSEntity osEntity = new OSEntity();
                        Models.PreenchimentoOS preenchimentoOS = new Models.PreenchimentoOS();

                        CarregarOSEntity(dataTableReader, osEntity);
                        CarregarOSModel(dataTableReader, preenchimentoOS);

                        listaOS.Add(osEntity);
                        listaPreenchimentoOS.Add(preenchimentoOS);
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

            JObject jObject = new JObject();
            jObject.Add("OS", JsonConvert.SerializeObject(listaOS, Formatting.None));
            jObject.Add("preenchimentoOS", JsonConvert.SerializeObject(listaPreenchimentoOS, Formatting.None));

            return Ok(jObject);
            //return Request.CreateResponse(HttpStatusCode.OK, new { result = listaPecas });
        }


        [HttpPost]
        [Route("ObterListaComboOS")]
        public IHttpActionResult ObterListaComboOS(Int64 CD_CLIENTE, string CD_TECNICO, string DT_DATA_VISITA, Int64 ID_VISITA)//)
        {
            List<OSEntity> listaOS = new List<OSEntity>();
            List<Models.PreenchimentoOS> listaPreenchimentoOS = new List<Models.PreenchimentoOS>();

            try
            {
                var osEntityFilter = new OSEntity();
                if (CD_TECNICO == "0")
                    osEntityFilter.tecnico.CD_TECNICO = null;
                else
                    osEntityFilter.tecnico.CD_TECNICO = CD_TECNICO;

                osEntityFilter.visita.ID_VISITA = ID_VISITA;
                osEntityFilter.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = 4; //Finalizada

                DataTableReader dataTableReader = new OSData().ObterListaVisita(osEntityFilter, DT_DATA_VISITA, CD_CLIENTE).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        OSEntity osEntity = new OSEntity();
                        Models.PreenchimentoOS preenchimentoOS = new Models.PreenchimentoOS();

                        CarregarOSEntity(dataTableReader, osEntity);
                        CarregarOSModel(dataTableReader, preenchimentoOS);

                        listaOS.Add(osEntity);
                        listaPreenchimentoOS.Add(preenchimentoOS);
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

            JObject jObject = new JObject();
            jObject.Add("OS", JsonConvert.SerializeObject(listaOS, Formatting.None));
            jObject.Add("preenchimentoOS", JsonConvert.SerializeObject(listaPreenchimentoOS, Formatting.None));

            return Ok(jObject);
            //return Request.CreateResponse(HttpStatusCode.OK, new { result = listaPecas });
        }



        /// <summary>
        /// Transfere os dados do DataTableReader para OSEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="osEntity"></param>
        protected void CarregarOSEntity(DataTableReader dataTableReader, OSEntity osEntity)
        {
            osEntity.ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]);
            osEntity.DT_DATA_ABERTURA = Convert.ToDateTime(dataTableReader["DT_DATA_ABERTURA"]);
            osEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]);
            osEntity.tpStatusVisitaOS.DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString();
            osEntity.ativoFixo.CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString();
            osEntity.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
            osEntity.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
            osEntity.visita.ID_VISITA = Convert.ToInt64(dataTableReader["ID_VISITA"]);
            osEntity.TP_MANUTENCAO = dataTableReader["TP_MANUTENCAO"].ToString();
            osEntity.DS_OBSERVACAO = dataTableReader["DS_OBSERVACAO"].ToString();
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para Models.PreenchimentoOS
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="preenchimentoOS"></param>
        protected void CarregarOSModel(DataTableReader dataTableReader, Models.PreenchimentoOS preenchimentoOS)
        {
            preenchimentoOS.ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]);
            preenchimentoOS.ID_OS_Formatado = Convert.ToInt64(dataTableReader["ID_OS"]).ToString("000000");
            preenchimentoOS.DT_DATA_ABERTURA = Convert.ToDateTime(dataTableReader["DT_DATA_ABERTURA"]).ToString("dd/MM/yyyy");
            preenchimentoOS.HR_INICIO = Convert.ToDateTime(dataTableReader["DT_DATA_ABERTURA"]).ToString("HH:mm");
            preenchimentoOS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]);
            preenchimentoOS.tpStatusVisitaOS.DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString();
            preenchimentoOS.ativoFixo.CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString();
            preenchimentoOS.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
            preenchimentoOS.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
            preenchimentoOS.visita.ID_VISITA = Convert.ToInt64(dataTableReader["ID_VISITA"]);
            preenchimentoOS.TP_MANUTENCAO = dataTableReader["TP_MANUTENCAO"].ToString();
            preenchimentoOS.DS_OBSERVACAO = dataTableReader["DS_OBSERVACAO"].ToString();
        }

        [HttpGet]
        [Route("ObterListaOsSinc")]
        public IHttpActionResult ObterListaOsSinc(Int64 idUsuario)
        {
            IList<OsSinc> listaOs = new List<OsSinc>();
            try
            {
                //Int32 idUsuario = 60237;
                OSData osData = new OSData();
                listaOs = osData.ObterListaOsSinc(idUsuario);

                JObject JO = new JObject();
                //JO.Add("OS", JsonConvert.SerializeObject(listaOs, Formatting.None));
                JO.Add("OS", JArray.FromObject(listaOs));
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