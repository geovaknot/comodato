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
    public partial class RelatorioCarteiraAtendimentoVisita : ReportBasePage
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
            DataTable dtAgenda = data.ObterListaAtendimentoVisitaRelatorio(listaTecnicos, listaStatus, UsuarioAutenticado.usuario.nidUsuario, dataInicial, dataFinal);

            ReportDataSourceCarteiraAtendimentoVisita listaAgenda = new ReportDataSourceCarteiraAtendimentoVisita();
            foreach (DataRow row in dtAgenda.Rows)
            {
                ReportDataSources.AgendaAtendimentoVisita agenda = new ReportDataSources.AgendaAtendimentoVisita();

                agenda.ID_VISITA = row["ID_VISITA"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID_VISITA"]);
                agenda.CD_CLIENTE = Convert.ToInt64(row["CD_CLIENTE"]);
                agenda.CD_TECNICO = row["CD_TECNICO"].ToString();
                agenda.NM_CLIENTE = row["NM_CLIENTE"].ToString();
                agenda.NM_TECNICO = row["NM_TECNICO"].ToString();
                agenda.DT_DATA_VISITA = Convert.ToDateTime(row["DT_DATA_VISITA"]);
                agenda.HR_INICIO = row["HR_INICIO"].ToString();
                
                agenda.HR_FIM = row["HR_FIM"].ToString();
                agenda.DS_OBSERVACAO = row["DS_OBSERVACAO"].ToString();
                agenda.DS_MOTIVO_VISITA = row["DS_MOTIVO_VISITA"].ToString();
                agenda.DS_STATUS_VISITA = row["DS_STATUS_VISITA"].ToString();
                agenda.NM_NOTA_PESQ = row["NM_NOTA_PESQ"].ToString();
                agenda.DS_JUSTIFICATIVA = row["DS_JUSTIFICATIVA"].ToString();

                listaAgenda.Add(agenda);
            }
            if (listaAgenda.Count > 0)
            {
                //ReportViewer1.LocalReport.ReportPath = Path.Combine(Request.MapPath("~/"), @"\Reports\Agenda\CarteiraAtendimento.rdlc");
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\AgendaVisita\CarteiraAtendimentoVisita2.rdlc";
                ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("dsAgendaVisita", listaAgenda));
                ReportViewer1.LocalReport.SetParameters(new ReportParameter("agruparTecnicos", agruparTecnicos ? "1" : "0"));
                ReportViewer1.LocalReport.Refresh();
            }
            else
            {
                pnlMensagem.Visible = true;
            }
        }

        

    }
}