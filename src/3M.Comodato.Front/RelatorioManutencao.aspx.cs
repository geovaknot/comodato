using _3M.Comodato.Utility;
using _3M.Comodato.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace _3M.Comodato.Front
{
    public partial class RelatorioManutencao : ReportBasePage
    {
        private enum ParametroManutencao
        {
            DataInicial = 0,
            DataFinal = 1,
            CodigoGrupo = 2,
            CodigoCliente = 3,
            CodigoTecnico = 4,
            CodigoEquipamento = 5,
            CodigoVisita = 6,
            CodigoOS = 7,
            ModeloRelatorio = 8
        }

        protected override void ExibirRelatorio()
        {

            var param = this.idKey.Split('|');
            DateTime? dataInicial = null;
            if (!string.IsNullOrEmpty(param[ParametroManutencao.DataInicial.ToInt()]))
                dataInicial = Convert.ToDateTime(param[ParametroManutencao.DataInicial.ToInt()]);

            DateTime? dataFinal = null;
            if (!string.IsNullOrEmpty(param[ParametroManutencao.DataInicial.ToInt()]))
                dataFinal = Convert.ToDateTime(param[ParametroManutencao.DataFinal.ToInt()]);

            string codigoGrupo = null;
            if (!string.IsNullOrEmpty(param[ParametroManutencao.CodigoGrupo.ToInt()]))
                codigoGrupo = param[ParametroManutencao.CodigoGrupo.ToInt()];

            decimal? codigoCliente = null;
            if (!string.IsNullOrEmpty(param[ParametroManutencao.CodigoCliente.ToInt()]))
                codigoCliente = Convert.ToDecimal(param[ParametroManutencao.CodigoCliente.ToInt()]);

            string codigoTecnico = null;
            if (!string.IsNullOrEmpty(param[ParametroManutencao.CodigoTecnico.ToInt()]))
                codigoTecnico = param[ParametroManutencao.CodigoTecnico.ToInt()];

            long? codigoVisita = null;
            if (!string.IsNullOrEmpty(param[ParametroManutencao.CodigoVisita.ToInt()]))
                codigoVisita = Convert.ToInt64(param[ParametroManutencao.CodigoVisita.ToInt()]);

            long? codigoOS = null;
            if (!string.IsNullOrEmpty(param[ParametroManutencao.CodigoOS.ToInt()]))
                codigoOS = Convert.ToInt64(param[ParametroManutencao.CodigoOS.ToInt()]);

            string ModeloRelatorio = null;
            if (!string.IsNullOrEmpty(param[ParametroManutencao.ModeloRelatorio.ToInt()]))
                ModeloRelatorio = param[ParametroManutencao.ModeloRelatorio.ToInt()];


            ManutencaoData manutencaoData = new ManutencaoData();

            DataSet manutencao = manutencaoData.ObterRelatorio(dataInicial, dataFinal, codigoGrupo, codigoCliente, codigoTecnico, codigoVisita, codigoOS);

            double horasTrabalhadas = 0;
            double valorManutencao = 0;
            double quantidadePecas = 0;
            double valorTotalPecas = 0;
            string horaTrabConv = "";

            if (ModeloRelatorio == "1")
            {
                for (int i = 0; i < manutencao.Tables[0].Rows.Count; i++)
                {
                    if (manutencao.Tables[0].Rows[i]["HR_TRAB_CONV"] == DBNull.Value || string.IsNullOrWhiteSpace(manutencao.Tables[0].Rows[i]["HR_TRAB_CONV"].ToString()))
                        manutencao.Tables[0].Rows[i]["HR_TRAB_CONV"] = "0:00:00";
                    else
                        manutencao.Tables[0].Rows[i]["HR_TRAB_CONV"] = manutencao.Tables[0].Rows[i]["HR_TRAB_CONV"] + ":00";

                    if (manutencao.Tables[0].Rows[i]["HR_TRABALHADAS"] != DBNull.Value)
                        horasTrabalhadas += Convert.ToDouble(manutencao.Tables[0].Rows[i]["HR_TRABALHADAS"]);

                    if (manutencao.Tables[0].Rows[i]["VL_MANUTENCAO"] != DBNull.Value)
                        valorManutencao += Convert.ToDouble(manutencao.Tables[0].Rows[i]["VL_MANUTENCAO"]);

                    if (manutencao.Tables[0].Rows[i]["QT_PECA"] != DBNull.Value)
                        quantidadePecas += Convert.ToDouble(manutencao.Tables[0].Rows[i]["QT_PECA"]);

                    if (manutencao.Tables[0].Rows[i]["VL_TOT_PECA"] != DBNull.Value)
                        valorTotalPecas += Convert.ToDouble(manutencao.Tables[0].Rows[i]["VL_TOT_PECA"]);
                }

                horaTrabConv = Math.Truncate(horasTrabalhadas).ToString() + ":" + ((horasTrabalhadas % 1) * 60).ToString("00") + ":00";
            }

            var reportDS = new ReportDataSource();
            reportDS.Name = "dsRptManutencao";
            reportDS.Value = manutencao.Tables[0];

            if (manutencao.Tables[0].Rows.Count == 0)
                pnlMensagem.Visible = true;
            else
            {
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Manutencao\Manutencao.rdlc";
                if (ModeloRelatorio == "1") // Modelo de Tabela Simplificado (Excel)
                {
                    reportDS.Name = "DataSet1";
                    ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Manutencao\ManutencaoTab.rdlc";

                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("horaTrabConv", horaTrabConv));
                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("valorManutencao", valorManutencao.ToString()));
                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("quantidadePecas", quantidadePecas.ToString("0.##")));
                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("valorTotalPecas", valorTotalPecas.ToString()));
                }
                if (ModeloRelatorio == "2") // Modelo de Tabela Completo
                {
                    reportDS.Name = "DataSet1";
                    ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Manutencao\ManutencaoTabCompleto.rdlc";

                }
                ReportViewer1.LocalReport.DataSources.Add(reportDS);
                ReportViewer1.LocalReport.Refresh();
            }

        }

    }
}