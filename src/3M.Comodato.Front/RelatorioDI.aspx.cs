using _3M.Comodato.Data;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Front.ReportDataSources;
using _3M.Comodato.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace _3M.Comodato.Front
{
    public partial class RelatorioDI : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            //long codigoPesquisa = long.Parse(idKey);
            AtivoClienteData ativoData = new AtivoClienteData();
            ReportDataSourceDI listaResultado = new ReportDataSourceDI();
            DataTable dataTable = ativoData.ObterRelatorioAtivosClientesDI();

            listaResultado.AddRange((from r in dataTable.Rows.Cast<DataRow>()
                        select new Distribuidor()
                        {
                            NomeDI = r["NM_CLIENTE"].ToString(),
                            Modelo = r["DS_MODELO"].ToString(),
                            MesesPrevistos = r.FieldOrDefault<int>("QTD_MESES_LOCACAO_PRV"),
                            MesesAlocado = r.FieldOrDefault<int>("QTD_MESES_LOCACAO_ALC"),
                            ValorAtivo = Convert.ToDecimal(r["VL_ATIVO"]),
                            ValorAluguel = Convert.ToDecimal(r["VL_ALUGUEL"]),
                            Status = r["STATUS"].ToString(),
                            NR_NOTA = r["NR_NOTA"].ToString(),
                            VL_NOTA = r["VL_NOTA"].ToString()
                        }).ToList());

            if (listaResultado.Count > 0)
            {
                ReportViewer1.LocalReport.ReportPath = Path.Combine(Request.MapPath("~/"), @"Reports\Distribuidor\AtivosDI.rdlc");
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("dsAtivosDI", listaResultado));
                ReportViewer1.LocalReport.Refresh();
            }
            else
            {
                pnlMensagem.Visible = true;
            }
        }
    }
}