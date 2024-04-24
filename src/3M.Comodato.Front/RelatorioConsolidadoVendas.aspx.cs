using _3M.Comodato.Data;
using _3M.Comodato.Front.ReportDataSources;
using _3M.Comodato.Front.Reports.AnaliseConsumo.dsRptConsumoTableAdapters;
using _3M.Comodato.Front.Reports.ConsolidadoVendas;
using _3M.Comodato.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace _3M.Comodato.Front
{
    public partial class RelatorioConsolidadoVendas : ReportBasePage
    {

        protected override void ExibirRelatorio()
        {
            var param = idKey.Split('|');

            string codigoGrupo = null;
            if (!string.IsNullOrEmpty(param[1]))
                codigoGrupo = param[1];

            string codigoCliente = null;
            if (!string.IsNullOrEmpty(param[0]))
                codigoCliente = param[0];

            string linha = null;
            if (!string.IsNullOrEmpty(param[2]))
                linha = param[2];

            //DataSet dsRptKatPorTecnico = new ConsolidadoVendasData().ObterRelatorio(codigoGrupo, codigoCliente);

            DataTable KatPorTecnico = new KatTecnicoData().ObterRelatorioConsolidado(DateTime.Now.AddYears(-1), DateTime.Now.AddDays(1), "", codigoCliente, linha);
            DataTable analiseDeConsumo = new ConsolidadoVendasData().ObterRelatorioConsolidado(1, codigoGrupo, Convert.ToInt64(codigoCliente), linha);
            DataTable manutencaoGeral = new ManutencaoData().ObterRelatorioConsolidado(DateTime.Now.AddYears(-1), DateTime.Now.AddDays(1), codigoGrupo, Convert.ToInt64(codigoCliente), linha);
            DataTable analiseResumo = new ResumosData().ObterRelatorio(codigoGrupo, Convert.ToInt64(codigoCliente), linha);
            //var reportDS = new ReportDataSource();
            //reportDS.Name = "DataSet1";
            //reportDS.Value = KatPorTecnico.Tables[0];

            //if (KatPorTecnico.Tables[0].Rows.Count == 0)
            //{
            //    pnlMensagem.Visible = true;
            //    return;
            //}
            //else
            //    ReportViewer1.ProcessingMode = ProcessingMode.Local;
            List<ReportDataSources.KatPorTecnico> lstKatTecnico  = new List<ReportDataSources.KatPorTecnico>();
            List<ReportDataSources.AnaliseConsumo> lstAnaliseConsumo = new List<ReportDataSources.AnaliseConsumo>();
            List<ReportDataSources.Manutencao> lstManutencaoGeral = new List<ReportDataSources.Manutencao>();
            List<ReportDataSources.AnaliseVendaResumo> lstAnaliseResumo = new List<ReportDataSources.AnaliseVendaResumo>();
            foreach (DataRow row in KatPorTecnico.Rows)
            {
                ReportDataSources.KatPorTecnico obj = new ReportDataSources.KatPorTecnico();
                obj.NM_CLIENTE = row["NM_CLIENTE"].ToString();
                obj.CD_TECNICO = row["CD_TECNICO"].ToString();
                obj.CD_CLIENTE = row["CD_CLIENTE"].ToString();
                obj.KatRealizado = Convert.ToDouble(row["KatRealizado"]);
                obj.KatMesPorTecnico = Convert.ToDouble(row["KatMesPorTecnico"]);

                lstKatTecnico.Add(obj);
            }

            foreach (DataRow row in analiseDeConsumo.Rows)
            {
                ReportDataSources.AnaliseConsumo obj = new ReportDataSources.AnaliseConsumo();                
                obj.CD_CLIENTE = Convert.ToInt64(row["CD_CLIENTE"]);
                obj.AQT_VENDAS = Convert.ToDouble(row["AQT_VENDAS"]);
                obj.ATOT_VENDAS = Convert.ToDouble(row["ATOT_VENDAS"]);
                obj.QT_EQP = Convert.ToInt64(row["QT_EQP"]);
                obj.DS_LINHA_PRODUTO = row["DS_LINHA_PRODUTO"].ToString();
                obj.TOT_DEPRECIACAO = Convert.ToInt64(row["TOT_DEPRECIACAO"]);
                lstAnaliseConsumo.Add(obj);
            }

            foreach (DataRow row in manutencaoGeral.Rows)
            {
                ReportDataSources.Manutencao obj = new ReportDataSources.Manutencao();
                obj.CD_CLIENTE = row["CD_CLIENTE"].ToString();
                obj.NM_CLIENTE = row["NM_CLIENTE"].ToString();
                obj.QT_PECAS = Convert.ToInt64(row["QT_PECAS"]);
                obj.VL_PECAS = Convert.ToDouble(row["VL_PECAS"]);
                obj.QT_HORAS = Convert.ToDouble(row["QT_HORAS"]);
                obj.VL_HORAS = Convert.ToDouble(row["VL_HORAS"]);
                lstManutencaoGeral.Add(obj);
            }

            foreach (DataRow row in analiseResumo.Rows)
            {
                ReportDataSources.AnaliseVendaResumo obj = new ReportDataSources.AnaliseVendaResumo();
                obj.CD_CLIENTE = row["CD_CLIENTE"].ToString();
                obj.OIR = Convert.ToDouble(row["OIR"]);
                obj.FGM = Convert.ToDouble(row["fGM"]);
                lstAnaliseResumo.Add(obj);
            }

            
            
            ReportDataSources.ConsolidadoVendas objConsolidado = new ReportDataSources.ConsolidadoVendas();

            ReportDataSourceConsolidadoVendas lstConsolidado = new ReportDataSourceConsolidadoVendas();

            List<ReportDataSources.ConsolidadoVendas> lstConsolidadoSoma = new List<ReportDataSources.ConsolidadoVendas>();


            if (codigoGrupo != null)
            {
                lstKatTecnico = lstKatTecnico.Where(x => lstManutencaoGeral.Any(y => y.CD_CLIENTE == x.CD_CLIENTE)).ToList();
                
                if (lstKatTecnico?.Count > 0)
                {
                    
                    foreach (var k in lstKatTecnico)
                    {
                        ReportDataSources.ConsolidadoVendas objConsolidadoSoma = new ReportDataSources.ConsolidadoVendas();

                        objConsolidadoSoma.CD_CLIENTE = k.CD_CLIENTE;
                        objConsolidadoSoma.NM_CLIENTE = k.NM_CLIENTE;

                        var existenalista = lstConsolidadoSoma.Where(x => x.CD_CLIENTE == k.CD_CLIENTE).ToList();

                        if (existenalista?.Count == 0)
                        {
                            var lstKatTecnicoSoma = lstKatTecnico.Where(x => x.CD_CLIENTE == k.CD_CLIENTE).ToList();
                            double TotalKatMes = 0;
                            double TotalKatRealizado = 0;

                            if (lstKatTecnicoSoma?.Count > 0)
                            {
                                var CD_TECNICO_VALIDAR = "";
                                foreach (var kt in lstKatTecnicoSoma)
                                {
                                    
                                    if(!CD_TECNICO_VALIDAR.Contains(kt.CD_TECNICO))
                                        TotalKatMes += kt.KatMesPorTecnico;
                                    TotalKatRealizado += kt.KatRealizado;
                                    CD_TECNICO_VALIDAR += kt.CD_TECNICO + ",";
                                }

                            }

                            objConsolidadoSoma.KatRealizado = TotalKatRealizado;
                            objConsolidadoSoma.KatMesPorTecnico = TotalKatMes;

                            var listaSomaManutencao = lstManutencaoGeral.Where(x => x.CD_CLIENTE == k.CD_CLIENTE).ToList();

                            if (listaSomaManutencao?.Count > 0)
                            {
                                double VL_PECAS = 0;
                                double VL_HORAS = 0;
                                double QT_HORAS = 0;
                                Int64 QT_PECAS = 0;

                                foreach (var m in listaSomaManutencao)
                                {
                                    VL_HORAS += m.VL_HORAS;
                                    VL_PECAS += m.VL_PECAS;
                                    QT_HORAS += m.QT_HORAS;
                                    QT_PECAS += m.QT_PECAS;
                                }
                                objConsolidadoSoma.VL_PECAS = VL_PECAS;
                                objConsolidadoSoma.VL_HORAS = VL_HORAS;
                                objConsolidadoSoma.QT_PECAS = QT_PECAS;
                                objConsolidadoSoma.QT_HORAS = QT_HORAS;
                            }

                            var analiseResumoSoma = lstAnaliseResumo.Where(x => x.CD_CLIENTE == k.CD_CLIENTE).ToList();

                            if (analiseResumoSoma?.Count > 0)
                            {
                                double OIR = 0;
                                double FGM = 0;
                                foreach (var resumo in analiseResumoSoma)
                                {
                                    OIR += resumo.OIR;
                                    FGM += resumo.FGM;
                                }
                                objConsolidadoSoma.FGM = FGM;
                                objConsolidadoSoma.OIR = OIR;
                            }
                            else
                            {
                                objConsolidadoSoma.FGM = 0;
                                objConsolidadoSoma.OIR = 0;
                            }

                            var analiseConsumoSoma = lstAnaliseConsumo.Where(x => x.CD_CLIENTE.ToString() == k.CD_CLIENTE).ToList();

                            if (analiseConsumoSoma?.Count > 0)
                            {
                                int Contador = 0;
                                double AQT_VENDAS = 0;
                                double ATOT_VENDAS = 0;
                                double TOT_DEPRECIACAO = 0;
                                Int64 QT_EQP = 0;
                                string linhas = "";

                                foreach (var c in analiseConsumoSoma)
                                {
                                    Contador++;
                                    AQT_VENDAS += c.AQT_VENDAS;
                                    ATOT_VENDAS += c.ATOT_VENDAS;
                                    QT_EQP += c.QT_EQP;
                                    TOT_DEPRECIACAO += c.TOT_DEPRECIACAO;
                                    if (Contador < analiseConsumoSoma.Count)
                                    {
                                        linhas += c.DS_LINHA_PRODUTO + " = " + c.QT_EQP + "/ ";
                                    }
                                    else
                                    {
                                        linhas += c.DS_LINHA_PRODUTO + " = " + c.QT_EQP;
                                    }
                                }
                                objConsolidadoSoma.AQT_VENDAS = AQT_VENDAS;
                                objConsolidadoSoma.ATOT_VENDAS = ATOT_VENDAS;
                                objConsolidadoSoma.QT_EQP = QT_EQP;
                                objConsolidadoSoma.TOT_DEPRECIACAO = TOT_DEPRECIACAO;
                                objConsolidadoSoma.DS_LINHA_PRODUTO = linhas;
                            }

                            lstConsolidadoSoma.Add(objConsolidadoSoma);
                        }

                        
                    }
                     
                }
                
                

            }
            else
            {
                if (lstManutencaoGeral?.Count > 0)
                {
                    double VL_PECAS = 0;
                    double VL_HORAS = 0;
                    double QT_HORAS = 0;
                    Int64 QT_PECAS = 0;

                    foreach (var m in lstManutencaoGeral)
                    {
                        VL_HORAS += m.VL_HORAS;
                        VL_PECAS += m.VL_PECAS;
                        QT_HORAS += m.QT_HORAS;
                        QT_PECAS += m.QT_PECAS;
                    }
                    objConsolidado.VL_PECAS = VL_PECAS;
                    objConsolidado.VL_HORAS = VL_HORAS;
                    objConsolidado.QT_PECAS = QT_PECAS;
                    objConsolidado.QT_HORAS = QT_HORAS;
                }

                if (lstAnaliseResumo?.Count > 0)
                {
                    double OIR = 0;
                    double FGM = 0;
                    foreach (var resumo in lstAnaliseResumo)
                    {
                        OIR += resumo.OIR;
                        FGM += resumo.FGM;
                    }
                    objConsolidado.FGM = FGM;
                    objConsolidado.OIR = OIR;
                }



                if (lstAnaliseConsumo?.Count > 0)
                {
                    int Contador = 0;
                    double AQT_VENDAS = 0;
                    double ATOT_VENDAS = 0;
                    double TOT_DEPRECIACAO = 0;
                    Int64 QT_EQP = 0;
                    string linhas = "";

                    foreach (var c in lstAnaliseConsumo)
                    {
                        Contador++;
                        AQT_VENDAS += c.AQT_VENDAS;
                        ATOT_VENDAS += c.ATOT_VENDAS;
                        QT_EQP += c.QT_EQP;
                        TOT_DEPRECIACAO += c.TOT_DEPRECIACAO;
                        if (Contador < lstAnaliseConsumo.Count)
                        {
                            linhas += c.DS_LINHA_PRODUTO + " = " + c.QT_EQP + "/ ";
                        }
                        else
                        {
                            linhas += c.DS_LINHA_PRODUTO + " = " + c.QT_EQP;
                        }
                        
                    }
                    objConsolidado.AQT_VENDAS = AQT_VENDAS;
                    objConsolidado.ATOT_VENDAS = ATOT_VENDAS;
                    objConsolidado.QT_EQP = QT_EQP;
                    objConsolidado.DS_LINHA_PRODUTO = linhas;
                    objConsolidado.TOT_DEPRECIACAO = TOT_DEPRECIACAO;
                }

                if (lstKatTecnico?.Count > 0)
                {
                    string CD_CLIENTE = "";
                    string NM_CLIENTE = "";
                    double KatRealizado = 0;
                    double KatMesPorTecnico = 0;

                    var CD_TECNICO_VALIDAR = "";

                    foreach (var k in lstKatTecnico)
                    {
                        CD_CLIENTE = k.CD_CLIENTE;
                        NM_CLIENTE = k.NM_CLIENTE;
                        if(!CD_TECNICO_VALIDAR.Contains(k.CD_TECNICO))
                            KatMesPorTecnico += k.KatMesPorTecnico;
                        KatRealizado += k.KatRealizado;
                        
                        CD_TECNICO_VALIDAR += k.CD_TECNICO + ",";
                    }
                    objConsolidado.CD_CLIENTE = CD_CLIENTE;
                    objConsolidado.NM_CLIENTE = NM_CLIENTE;
                    objConsolidado.KatMesPorTecnico = KatMesPorTecnico;
                    objConsolidado.KatRealizado = KatRealizado;
                }

                lstConsolidado.Add(objConsolidado);
            }
            

            if (lstConsolidado?.Count > 0)
            {
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ConsolidadoVendas\ConsolidadoVendas.rdlc";
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("reportDS", lstConsolidado));
                //ReportViewer1.LocalReport.SetParameters(new ReportParameter("Periodo", "Período de " + DataInicialParam + " até " + DataFinalParam));
                //ReportViewer1.LocalReport.SetParameters(new ReportParameter("ExibirClientes", ExibirClientes));
                //ReportViewer1.LocalReport.SetParameters(new ReportParameter("AnoOuMes", AnoOuMes));
                ReportViewer1.LocalReport.Refresh();
            }
            else if (lstConsolidadoSoma?.Count > 0)
            {
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ConsolidadoVendas\ConsolidadoVendas.rdlc";
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("reportDS", lstConsolidadoSoma));
                //ReportViewer1.LocalReport.SetParameters(new ReportParameter("Periodo", "Período de " + DataInicialParam + " até " + DataFinalParam));
                //ReportViewer1.LocalReport.SetParameters(new ReportParameter("ExibirClientes", ExibirClientes));
                //ReportViewer1.LocalReport.SetParameters(new ReportParameter("AnoOuMes", AnoOuMes));
                ReportViewer1.LocalReport.Refresh();
            }else
            {
                pnlMensagem.Visible = true;
            }
        }
        //private string IdKey => ControlesUtility.Criptografia.Descriptografar(Request.QueryString["IdKey"]);

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        ConfigurarRelatorio();
        //        ReportViewer1.LocalReport.Refresh();
        //    }
        //}

        //private void ConfigurarRelatorio()
        //{
        //    var param = this.IdKey.Split('|');

        //    string tipoRelatorio = param[0];

        //    int codigoVisao = int.Parse(param[1]);
        //    string visaoRelatorio = param[2];
        //    //
        //    string codigoGrupo = null;
        //    if (!string.IsNullOrEmpty(param[3]))
        //        codigoGrupo = param[3];

        //    decimal? codigoCliente = null;
        //    if (!string.IsNullOrEmpty(param[4]))
        //        codigoCliente = Convert.ToDecimal(param[4]);

        //    decimal? codigoExecutivo = null;
        //    if (!string.IsNullOrEmpty(param[5]))
        //        codigoExecutivo = Convert.ToDecimal(param[5]);

        //    decimal? codigoVendedor = null;
        //    if (!string.IsNullOrEmpty(param[6]))
        //        codigoVendedor = Convert.ToDecimal(param[6]);

        //    int? codigoLinhaProduto = null;
        //    if (!string.IsNullOrEmpty(param[7]))
        //        codigoLinhaProduto = Convert.ToInt32(param[7]);

        //    DataTable dataTableRelatorio;
        //    string rdlc = string.Empty;
        //    string dataSetName = string.Empty;

        //    if (tipoRelatorio == "Mensal")
        //    {
        //        rdlc = "AnaliseConsumoMensal";
        //        using (var tableAdapter = new prcRptConsumoMensalTableAdapter())
        //        {
        //            using (var dsRptConsumo = new Reports.AnaliseConsumo.dsRptConsumo())
        //            {
        //                tableAdapter.Fill(dsRptConsumo.prcRptConsumoMensal, codigoVisao, codigoCliente, codigoGrupo, codigoVendedor, codigoExecutivo, codigoLinhaProduto);
        //                dataTableRelatorio = dsRptConsumo.prcRptConsumoMensal as DataTable;
        //            }
        //        }
        //        ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\AnaliseConsumo\AnaliseConsumoMensal.rdlc";
        //    }
        //    else
        //    {
        //        rdlc = "AnaliseConsumoCliente";
        //        using (var tableAdapter = new prcRptConsumoClienteTableAdapter())
        //        {
        //            using (var dsRptConsumo = new Reports.AnaliseConsumo.dsRptConsumo())
        //            {
        //                tableAdapter.Fill(dsRptConsumo.prcRptConsumoCliente, codigoVisao, codigoCliente, codigoGrupo, codigoVendedor, codigoExecutivo, codigoLinhaProduto);
        //                dataTableRelatorio = dsRptConsumo.prcRptConsumoCliente as DataTable;
        //            }
        //        }
        //        ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\AnaliseConsumo\AnaliseConsumoCliente.rdlc";
        //    }

        //    if (dataTableRelatorio.Rows.Count > 0)
        //    {
        //        //ReportViewer1.LocalReport.ReportPath = Path.Combine(Request.MapPath("~/"), $@"\Reports\AnaliseConsumo\{rdlc}.rdlc");
        //        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("ConsumoMensal", dataTableRelatorio));
        //        ReportViewer1.LocalReport.SetParameters(new ReportParameter("VisaoRelatorio", visaoRelatorio));
        //    }
        //    else
        //    {
        //        pnlMensagem.Visible = true;
        //    }
        //}
    }
}