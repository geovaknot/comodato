using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.ReportDataSources;
using _3M.Comodato.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace _3M.Comodato.Front
{
    public partial class RelatorioCarteiraAtendimento : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            bool agruparTecnicos = false;
            string tecnicos = parametros[0];
            string status = parametros[1];
            string ordenarPor = parametros[2];
            DateTime? dataInicial = null;
            if (!string.IsNullOrEmpty(parametros[3]))
                dataInicial = Convert.ToDateTime(parametros[3]);

            DateTime? dataFinal = null;
            if (!string.IsNullOrEmpty(parametros[4]))
                dataFinal = Convert.ToDateTime(parametros[4]);

            List<String> listaTecnicos = new List<string>();
            if (tecnicos.Length > 0)
            {
                listaTecnicos = tecnicos.Split(',').ToList();
            }
            List<String> listaStatus = new List<string>();
            if (status.Length > 0)
            {
                listaStatus = status.Split(',').ToList();
            }

            AgendaData data = new AgendaData();
            var usuario = UsuarioAutenticado;
            DataTable dtAgenda = data.ObterListaAtendimentoRelatorio(listaTecnicos, listaStatus, UsuarioAutenticado.usuario.nidUsuario, dataInicial, dataFinal);

            ReportDataSourceCarteiraAtendimento listaAgenda = new ReportDataSourceCarteiraAtendimento();
            foreach (DataRow row in dtAgenda.Rows)
            {
                ReportDataSources.AgendaAtendimento agenda = new ReportDataSources.AgendaAtendimento();

                agenda.StatusOS = row["DS_STATUS_OS"].ToString();
                agenda.CodigoCliente = Convert.ToInt64(row["CD_CLIENTE"]);
                agenda.CD_TECNICO_PRINCIPAL = row["CD_TECNICO_PRINCIPAL"].ToString();
                agenda.NomeCliente = row["NM_CLIENTE"].ToString();
                agenda.Regiao = row["DS_REGIAO"].ToString();
                agenda.UltimaOS = row["DT_DATA_OS"].Data();
                agenda.TecnicoPrincipal = row["NM_TECNICO_PRINCIPAL"].ToString();
                //agenda.PeriodosAno = row["QT_PERIODO"] == DBNull.Value ? 0 : Convert.ToDecimal(row["QT_PERIODO"]);
                agenda.QT_PERIODO = row["QT_PERIODO"] == DBNull.Value ? 0 : Convert.ToInt32(row["QT_PERIODO"]);
                agenda.CodigoOrdem = row["CD_ORDEM"] == DBNull.Value ? 0 : Convert.ToInt32(row["CD_ORDEM"]);

                agenda.ID_OS = Convert.ToInt64("0" + row["ID_OS"].ToString());
                agenda.CD_ATIVO_FIXO = row["CD_ATIVO_FIXO"].ToString();
                agenda.CD_MODELO = row["CD_MODELO"].ToString();
                agenda.CD_GRUPO_MODELO = row["CD_GRUPO_MODELO"].ToString();

                switch (ordenarPor)
                {
                    case "R":
                        agenda.Ordenacao = agenda.PERCENTUAL;
                        break;
                    case "T":
                        agruparTecnicos = true;
                        agenda.Agrupamento = agenda.TecnicoPrincipal;
                        agenda.Ordenacao = agenda.TecnicoPrincipal;
                        break;
                    case "C":
                        agenda.Ordenacao = agenda.NomeCliente;
                        break;
                    case "A":
                        agenda.Ordenacao = agenda.UltimaVisita;
                        break;
                    default:
                        agenda.Ordenacao = agenda.CodigoOrdem;
                        break;
                }

                agenda.listaLogStatusOsPadrao = ObterListaLogStatusOs(agenda.ID_OS);
                agenda.TempoGastoTOTAL = CalcularTempoGastoOs(agenda.listaLogStatusOsPadrao);

                try
                {
                    //bool contabilizarVisita = agenda.listaLogStatusVisita
                    //    .Where(c => c.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == ControlesUtility.Enumeradores.TpStatusVisita.Consultoria.ToInt()).Count() == 0;

                    //if (contabilizarVisita)
                    //{
                    //    agenda.QT_PERIODO_REALIZADO = Convert.ToDecimal(agenda.TempoGastoTOTAL.TotalHours) / 3;//new TimeSpan(agenda.TempoGastoTOTAL.Ticks / 3);
                    //    agenda.QT_PERIODO_REALIZADO_FORMATADO = agenda.QT_PERIODO_REALIZADO.ToString("N2"); //Convert.ToDateTime(agenda.QT_PERIODO_REALIZADO.ToString()).ToString("HH:mm");
                    //    if (agenda.QT_PERIODO > 0)                                                                                                       //TimeSpan QT_PERIODO = TimeSpan.FromHours(agenda.QT_PERIODO);
                    //        agenda.PERCENTUAL_FORMATADO = Convert.ToDecimal((agenda.QT_PERIODO_REALIZADO * 100) / (agenda.QT_PERIODO * 3)).ToString("N2");
                    //    else
                    //        agenda.PERCENTUAL_FORMATADO = Convert.ToDecimal(0).ToString("N2");
                    //}
                    //else
                    //{
                    //agenda.QT_PERIODO_REALIZADO = 0;
                    //agenda.QT_PERIODO_REALIZADO_FORMATADO = agenda.QT_PERIODO_REALIZADO.ToString("N2");
                    //agenda.PERCENTUAL_FORMATADO = Convert.ToDecimal(0).ToString("N2");
                    //}
                    agenda.QT_PERIODO_REALIZADO = Convert.ToDecimal(agenda.TempoGastoTOTAL.TotalHours) / 3;//new TimeSpan(agenda.TempoGastoTOTAL.Ticks / 3);
                    agenda.QT_PERIODO_REALIZADO_FORMATADO = agenda.QT_PERIODO_REALIZADO.ToString("N2"); //Convert.ToDateTime(agenda.QT_PERIODO_REALIZADO.ToString()).ToString("HH:mm");
                    if (agenda.QT_PERIODO > 0)                                                                                                       //TimeSpan QT_PERIODO = TimeSpan.FromHours(agenda.QT_PERIODO);
                        agenda.PERCENTUAL_FORMATADO = Convert.ToDecimal((agenda.QT_PERIODO_REALIZADO * 100) / (agenda.QT_PERIODO * 3)).ToString("N2");
                    else
                        agenda.PERCENTUAL_FORMATADO = Convert.ToDecimal(0).ToString("N2");
                }
                catch
                {
                    agenda.PERCENTUAL_FORMATADO = Convert.ToDecimal(0).ToString("N2");
                }
                agenda.PERCENTUAL = Convert.ToDecimal(agenda.PERCENTUAL_FORMATADO);

                listaAgenda.Add(agenda);
            }
            if (listaAgenda.Count > 0)
            {
                //ReportViewer1.LocalReport.ReportPath = Path.Combine(Request.MapPath("~/"), @"\Reports\Agenda\CarteiraAtendimento.rdlc");
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Agenda\CarteiraAtendimento2.rdlc";
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("dsAgenda", listaAgenda));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("agruparTecnicos", agruparTecnicos ? "1" : "0"));
                ReportViewer1.LocalReport.Refresh();
            }
            else
            {
                pnlMensagem.Visible = true;
            }
        }

        protected List<Models.LogStatusOsPadrao> ObterListaLogStatusOs(Int64? ID_OS)
        {
            List<Models.LogStatusOsPadrao> listaLogStatusOsPadrao = new List<Models.LogStatusOsPadrao>();

            if (ID_OS == 0)
            {
                return listaLogStatusOsPadrao;
            }

            try
            {
                LogStatusOSPadraoEntity logStatusOsEntity = new LogStatusOSPadraoEntity();
                Int64 OS_ID = 0;

                if (ID_OS != null)
                {
                    logStatusOsEntity.OS.ID_OS = Convert.ToInt64(ID_OS);
                }
                
                DataTableReader dataTableReader = new LogStatusVisitaData().ObterListaOsPadrao(logStatusOsEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.LogStatusOsPadrao logStatusVisita = new Models.LogStatusOsPadrao
                        {
                            ID_LOG_STATUS_OS = Convert.ToInt64(dataTableReader["ID_LOG_STATUS_OS"]),
                            osPadrao = new OSPadraoEntity()
                            {
                                ID_OS = Convert.ToInt64(dataTableReader["ID_OS"])
                            },
                            DT_DATA_LOG_OS = dataTableReader["dtmDataHoraAtualizacao"].Data(), //.ToString("dd/MM/yyyy HH:mm"),
                            tpStatusOSPadrao = new TpStatusOSPadraoEntity()
                            {
                                ST_STATUS_OS = Convert.ToInt32(dataTableReader["ST_STATUS_OS"]),
                                DS_STATUS_OS = dataTableReader["DS_STATUS_OS"].ToString()
                            },
                            usuario = new UsuarioEntity()
                            {
                                cnmNome = dataTableReader["cnmNome"].ToString()
                            }
                        };

                        listaLogStatusOsPadrao.Add(logStatusVisita);
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

            return listaLogStatusOsPadrao;

        }

        //protected List<Models.LogStatusVisita> ObterListaLogStatusVisita(Int64 ID_VISITA)
        //{
        //    List<Models.LogStatusVisita> listaLogStatusVisita = new List<Models.LogStatusVisita>();

        //    if (ID_VISITA == 0)
        //    {
        //        return listaLogStatusVisita;
        //    }

        //    try
        //    {
        //        LogStatusVisitaEntity logStatusVisitaEntity = new LogStatusVisitaEntity();
        //        logStatusVisitaEntity.visita.ID_VISITA = ID_VISITA;
        //        DataTableReader dataTableReader = new LogStatusVisitaData().ObterLista(logStatusVisitaEntity).CreateDataReader();

        //        if (dataTableReader.HasRows)
        //        {
        //            while (dataTableReader.Read())
        //            {
        //                Models.LogStatusVisita logStatusVisita = new Models.LogStatusVisita
        //                {
        //                    ID_LOG_STATUS_VISITA = Convert.ToInt64(dataTableReader["ID_LOG_STATUS_VISITA"]),
        //                    visita = new VisitaEntity()
        //                    {
        //                        ID_VISITA = Convert.ToInt64(dataTableReader["ID_VISITA"])
        //                    },
        //                    DT_DATA_LOG_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_LOG_VISITA"]), //.ToString("dd/MM/yyyy HH:mm"),
        //                    tpStatusVisitaOS = new TpStatusVisitaOSEntity()
        //                    {
        //                        ST_TP_STATUS_VISITA_OS = Convert.ToInt32(dataTableReader["ST_TP_STATUS_VISITA_OS"]),
        //                        DS_TP_STATUS_VISITA_OS = dataTableReader["DS_TP_STATUS_VISITA_OS"].ToString()
        //                    },
        //                    usuario = new UsuarioEntity()
        //                    {
        //                        cnmNome = dataTableReader["cnmNome"].ToString()
        //                    }
        //                };

        //                listaLogStatusVisita.Add(logStatusVisita);
        //            }
        //        }

        //        if (dataTableReader != null)
        //        {
        //            dataTableReader.Dispose();
        //            dataTableReader = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogarErro(ex);
        //        throw ex;
        //    }

        //    return listaLogStatusVisita;

        //}

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

        protected TimeSpan CalcularTempoGastoOs(List<Models.LogStatusOsPadrao> listasLogStatusOsPadrao)
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
                foreach (Models.LogStatusOsPadrao logStatusOspadrao in listasLogStatusOsPadrao)
                {
                    if (logStatusOspadrao.tpStatusOSPadrao.ST_STATUS_OS != Convert.ToInt32(Utility.ControlesUtility.Enumeradores.TpStatusOSPadrao.AguardandoInicio))
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

                        if (logStatusOspadrao.tpStatusOSPadrao.ST_STATUS_OS == Convert.ToInt32(Utility.ControlesUtility.Enumeradores.TpStatusOSPadrao.Aberta))
                        {
                            DT_DATA_LOG1 = logStatusOspadrao.DT_DATA_LOG_OS;
                            DT_DATA_LOG2 = null;
                        }
                        else
                        {
                            DT_DATA_LOG2 = logStatusOspadrao.DT_DATA_LOG_OS;
                        }

                        if (DT_DATA_LOG1 != null && DT_DATA_LOG2 != null)
                        {
                            TempoGasto = (Convert.ToDateTime(DT_DATA_LOG2) - Convert.ToDateTime(DT_DATA_LOG1));
                            TempoGastoTOTALTECNICO += TempoGasto;
                            TempoGastoTOTAL += TempoGasto;

                            logStatusOspadrao.TempoGasto = TempoGasto;
                            logStatusOspadrao.TempoGastoTOTAL = TempoGastoTOTALTECNICO;

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