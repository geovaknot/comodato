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
    [RoutePrefix("api/AgendaAPI")]
    [Authorize]
    public class AgendaAPIController : BaseAPIController
    {
        /// <summary>
        /// Executa reordenação de Agenda do Técnico via NR_ORDENACAO
        /// </summary>
        /// <param name="CD_TECNICO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Reordenar")]
        public HttpResponseMessage Reordenar(AgendaReordenar agendaReordenar)
        {
            try
            {
                AgendaEntity agendaEntity = new AgendaEntity();
                agendaEntity.tecnico.CD_TECNICO = agendaReordenar.tecnico.CD_TECNICO;
                agendaEntity.NR_ORDENACAO = agendaReordenar.NR_ORDENACAO;
                new AgendaData().Reordenar(agendaEntity, agendaReordenar.TP_ACAO);
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
        /// Executa reordenação de Agenda do Técnico via NR_ORDENACAO
        /// </summary>
        /// <param name="CD_TECNICO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ReordenarTotal")]
        public HttpResponseMessage ReordenarTotal(AgendaReordenar agendaReordenar)
        {
            try
            {
                List<Models.ListaAgendaAtendimento> listasAgendaAtendimentos = ObterListaAgendaAtendimento(agendaReordenar);

                // Reordenar pela QT_PERIODO_REALIZADO
                listasAgendaAtendimentos = listasAgendaAtendimentos.OrderBy(a => a.QT_PERIODO_REALIZADO).ThenBy(a => a.QT_PERIODO).ToList();

                int NR_ORDENACAO = 1;
                foreach (Models.ListaAgendaAtendimento listaAgendaAtendimento in listasAgendaAtendimentos)
                {
                    AgendaEntity agendaEntity = new AgendaEntity();
                    agendaEntity.ID_AGENDA = listaAgendaAtendimento.ID_AGENDA;

                    agendaEntity.NR_ORDENACAO = NR_ORDENACAO;
                    agendaEntity.nidUsuarioAtualizacao = agendaReordenar.nidUsuarioAtualizacao;

                    new AgendaData().Alterar(agendaEntity);

                    NR_ORDENACAO++;
                }
                // Busca a lista agora reordenada
                listasAgendaAtendimentos = ObterListaAgendaAtendimento(agendaReordenar);

                // Se exisitr visitas com status 1 (Novo) ou 2 (Aberto) são reordenados para a primeira posição
                foreach (Models.ListaAgendaAtendimento listaAgendaAtendimento in listasAgendaAtendimentos.Where(a => a.ST_TP_STATUS_VISITA_OS >= 1 && a.ST_TP_STATUS_VISITA_OS <= 2))
                {
                    AgendaEntity agendaEntity = new AgendaEntity();
                    agendaEntity.tecnico.CD_TECNICO = listaAgendaAtendimento.CD_TECNICO_PRINCIPAL;
                    agendaEntity.NR_ORDENACAO = listaAgendaAtendimento.NR_ORDENACAO;
                    new AgendaData().Reordenar(agendaEntity, "P");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { mensagem = "Agenda reordenada com sucesso!" });
        }

        /// <summary>
        /// Consulta a agenda informada
        /// </summary>
        /// <param name="ID_AGENDA"></param>
        /// <returns>AgendaEntity agendaEntity</returns>
        [HttpGet]
        [Route("Obter")]
        public HttpResponseMessage Obter(int ID_AGENDA)
        {
            AgendaEntity agendaEntity = new AgendaEntity();

            try
            {
                agendaEntity.ID_AGENDA = ID_AGENDA;
                DataTableReader dataTableReader = new AgendaData().ObterLista(agendaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CarregarAgendaEntity(dataTableReader, agendaEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { agenda = agendaEntity });
        }

        protected List<Models.ListaAgendaAtendimento> ObterListaAgendaAtendimento(AgendaReordenar agendaReordenar)
        {
            List<Models.ListaAgendaAtendimento> listaAgendaAtendimentos = new List<Models.ListaAgendaAtendimento>();

            try
            {
                AgendaEntity AgendaEntity = new AgendaEntity();
                AgendaEntity.tecnico.CD_TECNICO = agendaReordenar.tecnico.CD_TECNICO;
                DataTableReader dataTableReader = new AgendaData().ObterListaAtendimento(AgendaEntity, null, null, agendaReordenar.nidUsuarioAtualizacao, null).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaAgendaAtendimento listaAgendaAtendimento = new Models.ListaAgendaAtendimento
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]).ToString()),
                            ID_VISITA = Convert.ToInt64("0" + dataTableReader["ID_VISITA"]),
                            DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString(),
                            ID_AGENDA = Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]),
                            CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]),
                            NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString(),
                            EN_CIDADE = dataTableReader["EN_CIDADE"].ToString(),
                            EN_ESTADO = dataTableReader["EN_ESTADO"].ToString(),
                            CD_REGIAO = dataTableReader["CD_REGIAO"].ToString(),
                            DS_REGIAO = dataTableReader["DS_REGIAO"].ToString(),
                            CD_TECNICO_PRINCIPAL = dataTableReader["CD_TECNICO_PRINCIPAL"].ToString(),
                            NM_TECNICO_PRINCIPAL = dataTableReader["NM_TECNICO_PRINCIPAL"].ToString(),
                            QT_PERIODO = Convert.ToInt32("0" + dataTableReader["QT_PERIODO"]),
                            NR_ORDENACAO = Convert.ToInt32(dataTableReader["NR_ORDENACAO"]),
                        };

                        //if (dataTableReader["ID_OS"] != DBNull.Value)
                        //    listaAgendaAtendimento.ID_OS = Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]);

                        if (dataTableReader["ST_TP_STATUS_VISITA_OS"] != DBNull.Value)
                            listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS = Convert.ToInt32("0" + dataTableReader["ST_TP_STATUS_VISITA_OS"]);

                        if (dataTableReader["DT_DATA_VISITA"] != DBNull.Value)
                            listaAgendaAtendimento.DT_DATA_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_VISITA"]).ToString("dd/MM/yyyy");

                        if (listaAgendaAtendimento.ST_TP_STATUS_VISITA_OS == 0)
                            listaAgendaAtendimento.DS_TP_STATUS_VISITA_OS = "Iniciar";

                        //listaAgendaAtendimento.listaLogStatusOs = ObterListaLogStatusOS(ID_VISITA: null, CD_CLIENTE: listaAgendaAtendimento.CD_CLIENTE);
                        //listaAgendaAtendimento.TempoGastoTOTAL = CalcularTempoGastoOS(listaAgendaAtendimento.listaLogStatusOs);
                        listaAgendaAtendimento.listaLogStatusVisita = ObterListaLogStatusVisita(listaAgendaAtendimento.ID_VISITA);
                        listaAgendaAtendimento.TempoGastoTOTAL = CalcularTempoGastoVisita(listaAgendaAtendimento.listaLogStatusVisita);

                        try
                        {
                            bool contabilizarVisita = listaAgendaAtendimento.listaLogStatusVisita
                                .Where(c => c.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == ControlesUtility.Enumeradores.TpStatusVisita.Consultoria.ToInt()).Count() == 0;
                            if (contabilizarVisita)
                            {

                                listaAgendaAtendimento.QT_PERIODO_REALIZADO = Convert.ToDecimal(listaAgendaAtendimento.TempoGastoTOTAL.TotalHours) / 3;//new TimeSpan(listaAgendaAtendimento.TempoGastoTOTAL.Ticks / 3);
                                listaAgendaAtendimento.QT_PERIODO_REALIZADO_FORMATADO = listaAgendaAtendimento.QT_PERIODO_REALIZADO.ToString("N2"); //Convert.ToDateTime(listaAgendaAtendimento.QT_PERIODO_REALIZADO.ToString()).ToString("HH:mm");
                                                                                                                                                    //TimeSpan QT_PERIODO = TimeSpan.FromHours(listaAgendaAtendimento.QT_PERIODO);
                                                                                                                                                    //listaAgendaAtendimento.PERCENTUAL = Convert.ToDecimal((listaAgendaAtendimento.QT_PERIODO_REALIZADO.Ticks * 100) / QT_PERIODO.Ticks).ToString("N2");
                                listaAgendaAtendimento.PERCENTUAL = Convert.ToDecimal((listaAgendaAtendimento.QT_PERIODO_REALIZADO * 100) / (listaAgendaAtendimento.QT_PERIODO * 3)).ToString("N2");
                            }
                            else
                            {
                                listaAgendaAtendimento.QT_PERIODO_REALIZADO = 0;
                                listaAgendaAtendimento.QT_PERIODO_REALIZADO_FORMATADO = listaAgendaAtendimento.QT_PERIODO_REALIZADO.ToString("N2");
                                listaAgendaAtendimento.PERCENTUAL = Convert.ToDecimal(0).ToString("N2");

                            }
                        }
                        catch
                        {
                            listaAgendaAtendimento.PERCENTUAL = Convert.ToDecimal(0).ToString("N2");
                        }

                        listaAgendaAtendimentos.Add(listaAgendaAtendimento);
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
                throw ex;
            }

            return listaAgendaAtendimentos;
        }

        /// <summary>
        /// Consulta o histórico de LOG_STATUS_OS
        /// </summary>
        /// <param name="ID_VISITA"></param>
        /// <param name="CD_CLIENTE"></param>
        /// <returns></returns>
        protected List<Models.ListaLogStatusOS> ObterListaLogStatusOS(Int64? ID_VISITA = null, Int64? CD_CLIENTE = null)
        {
            List<Models.ListaLogStatusOS> listasLogStatusOs = new List<Models.ListaLogStatusOS>();

            try
            {
                DataTableReader dataTableReader = new LogStatusOSData().ObterListaAgendaVisita(ID_VISITA: ID_VISITA, CD_CLIENTE: CD_CLIENTE).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaLogStatusOS listaLogStatusOS = new Models.ListaLogStatusOS
                        {
                            OS = new OSEntity()
                            {
                                ID_OS = Convert.ToInt64(dataTableReader["ID_OS"])
                            },
                            //ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]),
                            tecnico = new TecnicoEntity()
                            {
                                CD_TECNICO = dataTableReader["CD_TECNICO"].ToString()
                            },
                            //CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                            DT_DATA_LOG_OS = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_OS"]),
                            tpStatusVisitaOS = new TpStatusVisitaOSEntity()
                            {
                                ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]),
                                DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString()
                            }
                            //ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]),
                            //DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString()
                        };
                        listasLogStatusOs.Add(listaLogStatusOS);
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
                throw ex;
            }

            //CalcularTempoGasto(listasLogStatusOs);

            return listasLogStatusOs;
        }

        /// <summary>
        /// Calcula o tempo gasto no histórico de LOG_STATUS_OS
        /// </summary>
        /// <param name="listasLogStatusOs"></param>
        /// <returns></returns>
        protected TimeSpan CalcularTempoGastoOS(List<Models.ListaLogStatusOS> listasLogStatusOs)
        {
            string CD_TECNICO = string.Empty;
            TimeSpan TempoGasto = new TimeSpan();
            TimeSpan TempoGastoTOTALTECNICO = new TimeSpan();
            TimeSpan TempoGastoTOTAL = new TimeSpan();
            string TempoTOTAL = string.Empty;
            DateTime? DT_DATA_LOG1 = null;
            DateTime? DT_DATA_LOG2 = null;

            try
            {
                foreach (Models.ListaLogStatusOS listaLogStatusOs in listasLogStatusOs)
                {
                    if (listaLogStatusOs.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS != Convert.ToInt32(Utility.ControlesUtility.Enumeradores.TpStatusOS.Nova))
                    {
                        if (string.IsNullOrEmpty(CD_TECNICO))
                            CD_TECNICO = listaLogStatusOs.tecnico.CD_TECNICO;

                        if (CD_TECNICO != listaLogStatusOs.tecnico.CD_TECNICO)
                        {
                            CD_TECNICO = listaLogStatusOs.tecnico.CD_TECNICO;
                            TempoGastoTOTALTECNICO = new TimeSpan();
                            DT_DATA_LOG1 = null;
                            DT_DATA_LOG2 = null;
                        }

                        if (listaLogStatusOs.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(Utility.ControlesUtility.Enumeradores.TpStatusOS.Aberta))
                        {
                            DT_DATA_LOG1 = listaLogStatusOs.DT_DATA_LOG_OS;
                            DT_DATA_LOG2 = null;
                        }
                        else
                            DT_DATA_LOG2 = listaLogStatusOs.DT_DATA_LOG_OS;

                        if (DT_DATA_LOG1 != null && DT_DATA_LOG2 != null)
                        {
                            TempoGasto = (Convert.ToDateTime(DT_DATA_LOG2) - Convert.ToDateTime(DT_DATA_LOG1));
                            TempoGastoTOTALTECNICO += TempoGasto;
                            TempoGastoTOTAL += TempoGasto;

                            listaLogStatusOs.TempoGasto = TempoGasto;
                            listaLogStatusOs.TempoGastoTOTAL = TempoGastoTOTALTECNICO;

                            DT_DATA_LOG1 = null;
                            DT_DATA_LOG2 = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return TempoGastoTOTAL;
        }

        protected List<Models.LogStatusVisita> ObterListaLogStatusVisita(Int64 ID_VISITA)
        {
            List<Models.LogStatusVisita> listaLogStatusVisita = new List<Models.LogStatusVisita>();

            if (ID_VISITA == 0)
                return listaLogStatusVisita;

            try
            {
                LogStatusVisitaEntity logStatusVisitaEntity = new LogStatusVisitaEntity();
                logStatusVisitaEntity.visita.ID_VISITA = ID_VISITA;
                DataTableReader dataTableReader = new LogStatusVisitaData().ObterLista(logStatusVisitaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.LogStatusVisita logStatusVisita = new Models.LogStatusVisita
                        {
                            ID_LOG_STATUS_VISITA = Convert.ToInt64(dataTableReader["ID_LOG_STATUS_VISITA"]),
                            visita = new VisitaEntity()
                            {
                                ID_VISITA = Convert.ToInt64(dataTableReader["ID_VISITA"])
                            },
                            DT_DATA_LOG_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_VISITA"]), //.ToString("dd/MM/yyyy HH:mm"),
                            tpStatusVisitaOS = new TpStatusVisitaOSEntity()
                            {
                                ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]),
                                DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString()
                            },
                            usuario = new UsuarioEntity()
                            {
                                cnmNome = dataTableReader["cnmNome"].ToString()
                            }
                        };

                        listaLogStatusVisita.Add(logStatusVisita);
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
                throw ex;
            }

            return listaLogStatusVisita;

        }

        /// <summary>
        /// Calcula o tempo gasto no histórico de LOG_STATUS_VISITA
        /// </summary>
        /// <param name="listasLogStatusVisita"></param>
        /// <returns></returns>
        protected TimeSpan CalcularTempoGastoVisita(List<Models.LogStatusVisita> listasLogStatusVisita)
        {
            //string CD_TECNICO = string.Empty;
            TimeSpan TempoGasto = new TimeSpan();
            TimeSpan TempoGastoTOTALTECNICO = new TimeSpan();
            TimeSpan TempoGastoTOTAL = new TimeSpan();
            string TempoTOTAL = string.Empty;
            DateTime? DT_DATA_LOG1 = null;
            DateTime? DT_DATA_LOG2 = null;

            try
            {
                foreach (Models.LogStatusVisita logStatusVisita in listasLogStatusVisita)
                {
                    if (logStatusVisita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS != Convert.ToInt32(Utility.ControlesUtility.Enumeradores.TpStatusVisita.Nova))
                    {
                        //if (string.IsNullOrEmpty(CD_TECNICO))
                        //    CD_TECNICO = logStatusVisita.CD_TECNICO;

                        //if (CD_TECNICO != logStatusVisita.CD_TECNICO)
                        //{
                        //    CD_TECNICO = logStatusVisita.CD_TECNICO;
                        //    TempoGastoTOTALTECNICO = new TimeSpan();
                        //    DT_DATA_LOG1 = null;
                        //    DT_DATA_LOG2 = null;
                        //}

                        if (logStatusVisita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == Convert.ToInt32(Utility.ControlesUtility.Enumeradores.TpStatusVisita.Aberta))
                        {
                            DT_DATA_LOG1 = logStatusVisita.DT_DATA_LOG_VISITA;
                            DT_DATA_LOG2 = null;
                        }
                        else
                            DT_DATA_LOG2 = logStatusVisita.DT_DATA_LOG_VISITA;

                        if (DT_DATA_LOG1 != null && DT_DATA_LOG2 != null)
                        {
                            TempoGasto = (Convert.ToDateTime(DT_DATA_LOG2) - Convert.ToDateTime(DT_DATA_LOG1));
                            TempoGastoTOTALTECNICO += TempoGasto;
                            TempoGastoTOTAL += TempoGasto;

                            logStatusVisita.TempoGasto = TempoGasto;
                            logStatusVisita.TempoGastoTOTAL = TempoGastoTOTALTECNICO;

                            DT_DATA_LOG1 = null;
                            DT_DATA_LOG2 = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return TempoGastoTOTAL;
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para AgendaEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="agendaEntity"></param>
        protected void CarregarAgendaEntity(DataTableReader dataTableReader, AgendaEntity agendaEntity)
        {
            agendaEntity.ID_AGENDA = Convert.ToInt64(dataTableReader["ID_AGENDA"]);
            agendaEntity.cliente.CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]);
            //agendaEntity.cliente.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString();
            agendaEntity.cliente.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString();
            agendaEntity.cliente.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
            agendaEntity.cliente.EN_ESTADO = dataTableReader["EN_ESTADO"].ToString();
            agendaEntity.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
            agendaEntity.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
            agendaEntity.tecnico.empresa.CD_Empresa = Convert.ToInt64("0" + dataTableReader["CD_Empresa"]);
            agendaEntity.tecnico.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
            agendaEntity.NR_ORDENACAO = Convert.ToInt32("0" + dataTableReader["NR_ORDENACAO"]);
        }

        /// <summary>
        /// Obter a listagem de Agenda de um tecnico (usuario)
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObterListaAgendaSinc")]
        public IHttpActionResult ObterListaAgendaSinc(Int64 idUsuario)
        {
            IList<AgendaSinc> listaAgenda = new List<AgendaSinc>();
            try
            {
                AgendaData agendaData = new AgendaData();
                listaAgenda = agendaData.ObterListaAgendaSinc(idUsuario);

                JObject JO = new JObject();
                JO.Add("AGENDA", JArray.FromObject(listaAgenda));
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