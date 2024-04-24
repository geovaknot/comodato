using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class HistoricoVisitaController : BaseController
    {
        // GET: HistoricoVisita
        [_3MAuthentication]
        public ActionResult Index()
        {
            Models.HistoricoVisita historicoVisita = new Models.HistoricoVisita
            {
                DT_INICIO = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy"),
                DT_FIM = DateTime.Now.ToString("dd/MM/yyyy"),
                clientes = new List<Models.Cliente>(),
                tecnicos = new List<Models.Tecnico>(),
            };

            return View(historicoVisita);
        }

        public JsonResult ObterListaVisitaJson(int CD_CLIENTE, string CD_TECNICO, string DT_INICIO, string DT_FIM)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.ListaHistoricoVisitas> listaHistoricoVisitas = ObterListaHistoricoVisitas(CD_CLIENTE, CD_TECNICO, DT_INICIO, DT_FIM);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/HistoricoVisita/_gridMVC.cshtml", listaHistoricoVisitas));
                jsonResult.Add("Status", "Success");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            //return Json(jsonResult, JsonRequestBehavior.AllowGet);
            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }

        protected List<Models.ListaHistoricoVisitas> ObterListaHistoricoVisitas(int CD_CLIENTE, string CD_TECNICO, string DT_INICIO, string DT_FIM)
        {
            List<Models.ListaHistoricoVisitas> listaHistoricoVisitas = new List<Models.ListaHistoricoVisitas>();

            try
            {

                if (!String.IsNullOrEmpty(DT_FIM))
                {
                    DT_FIM = DT_FIM + " 23:59:59";
                }


                DataTableReader dataTableReader = new VisitaData().ObterListaHistoricoVisita(CD_CLIENTE, CD_TECNICO, DT_INICIO, DT_FIM).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaHistoricoVisitas historicoVisitas = new Models.ListaHistoricoVisitas
                        {
                            //idKey = ControlesUtility.Criptografia.Criptografar(Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]) + "|" + Convert.ToInt64("0" + dataTableReader["ID_VISITA"]).ToString() + "|" + Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"]).ToString() + "|" + Convert.ToInt64("0" + dataTableReader["CD_TECNICO"]).ToString() + "|" + Convert.ToInt64("0" + dataTableReader["ID_OS"]) + "|" + "HistoricoVisita"),
                            idKey = ControlesUtility.Criptografia.Criptografar(Convert.ToInt64("0" + dataTableReader["ID_AGENDA"]) + "|" + Convert.ToInt64("0" + dataTableReader["ID_VISITA"]).ToString() + "|" + Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"]).ToString() + "|" + Convert.ToInt64("0" + dataTableReader["CD_TECNICO"]).ToString() + "|" + Convert.ToInt64("0") + "|" + "HistoricoVisita"),
                            ID_VISITA = Convert.ToInt64("0" + dataTableReader["ID_VISITA"]),
                            DT_DATA_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_VISITA"]).ToString("dd/MM/yyyy"),
                            NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString(),
                            NM_TECNICO = dataTableReader["NM_REDUZIDO"].ToString(), //+ " (" + Convert.ToInt32(dataTableReader["CD_TECNICO"]).ToString() + ") " /*+ dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString()*/,
                            DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString(),
                            QT_PERIODO = Convert.ToInt32(dataTableReader["QT_PERIODO"] is DBNull ? 0 : dataTableReader["QT_PERIODO"])
                        };

                        historicoVisitas.logStatusVisitas = ObterListaLogStatusVisita(historicoVisitas.ID_VISITA);
                        historicoVisitas.TempoGastoTOTAL = CalcularTempoGastoVisita(historicoVisitas.logStatusVisitas);

                        try
                        {
                            bool contabilizarVisita = historicoVisitas.logStatusVisitas
                                .Where(c => c.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == ControlesUtility.Enumeradores.TpStatusVisita.Consultoria.ToInt()).Count() == 0;

                            if (contabilizarVisita)
                            {
                                historicoVisitas.QT_PERIODO_REALIZADO = Convert.ToDecimal(historicoVisitas.TempoGastoTOTAL.TotalHours) / 3;//new TimeSpan(historicoVisitas.TempoGastoTOTAL.Ticks / 3);
                                historicoVisitas.QT_PERIODO_REALIZADO_FORMATADO = historicoVisitas.QT_PERIODO_REALIZADO.ToString("N2"); //Convert.ToDateTime(historicoVisitas.QT_PERIODO_REALIZADO.ToString()).ToString("HH:mm");
                                if (historicoVisitas.QT_PERIODO > 0)                                                                                                       //TimeSpan QT_PERIODO = TimeSpan.FromHours(historicoVisitas.QT_PERIODO);
                                    historicoVisitas.PERCENTUAL = Convert.ToDecimal((historicoVisitas.QT_PERIODO_REALIZADO * 100) / (historicoVisitas.QT_PERIODO)).ToString("N2");
                                else
                                    historicoVisitas.PERCENTUAL = Convert.ToDecimal(0).ToString("N2");
                            }
                            else
                            {
                                historicoVisitas.QT_PERIODO_REALIZADO = 0;
                                historicoVisitas.QT_PERIODO_REALIZADO_FORMATADO = historicoVisitas.QT_PERIODO_REALIZADO.ToString("N2");
                                historicoVisitas.PERCENTUAL = Convert.ToDecimal(0).ToString("N2");
                            }
                        }
                        catch
                        {
                            historicoVisitas.PERCENTUAL = Convert.ToDecimal(0).ToString("N2");
                        }
                        historicoVisitas.HoraMinuto = (historicoVisitas.TempoGastoTOTAL.Hours < 10 ? "0"+ historicoVisitas.TempoGastoTOTAL.Hours.ToString() : historicoVisitas.TempoGastoTOTAL.Hours.ToString())+":"+(historicoVisitas.TempoGastoTOTAL.Minutes < 10 ? "0"+ historicoVisitas.TempoGastoTOTAL.Minutes.ToString() : historicoVisitas.TempoGastoTOTAL.Minutes.ToString());
                        listaHistoricoVisitas.Add(historicoVisitas);
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

            return listaHistoricoVisitas;
        }

        protected List<Models.LogStatusVisita> ObterListaLogStatusVisita(Int64 ID_VISITA)
        {
            List<Models.LogStatusVisita> listaLogStatusVisita = new List<Models.LogStatusVisita>();

            if (ID_VISITA == 0)
            {
                return listaLogStatusVisita;
            }

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
                            DT_DATA_LOG_VISITA_SHORT = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_VISITA"]).ToString("dd/MM/yyyy"), //.ToString("dd/MM/yyyy HH:mm"),
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
                        {
                            DT_DATA_LOG2 = logStatusVisita.DT_DATA_LOG_VISITA;
                        }

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
    }

}