using _3M.Comodato.Data;
using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using System.Linq;

namespace _3M.Comodato.Front
{
    public partial class RelatorioRecolhidos : ReportBasePage
    {
        protected override void ExibirRelatorio()
        {
            RecolhidosData recolhidoData = new RecolhidosData();

            //string[] parameters = parametros;
            //string dtInicial = parameters[2];
            //string dtFinal = parameters[3];
            //string cdAtivo = parameters[1];
            //if (parameters[0] == "" || parameters[0] == null)
            //    parameters[0] = "0";
            //Int64 cdCliente = Convert.ToInt32(parameters[0]);

            //if (string.IsNullOrEmpty(dtInicial))
            //    dtInicial = DateTime.Now.AddYears(-5).ToString("dd/MM/yyyy");
            //if (string.IsNullOrEmpty(dtFinal))
            //    dtFinal = DateTime.Now.ToString("dd/MM/yyyy");

            //RecolhidosEntity recolhidosEntity = new RecolhidosEntity();
            //recolhidosEntity.DT_DEV_INICIAL = Convert.ToDateTime(dtInicial + " 00:00:00");
            //recolhidosEntity.DT_DEV_FINAL = Convert.ToDateTime(dtFinal + " 23:59:59");
            //if (Convert.ToInt32("0" + cdAtivo) > 0)
            //    recolhidosEntity.CD_ATIVO_FIXO = cdAtivo;
            //if(cdCliente > 0)
            //    recolhidosEntity.CD_CLIENTE = cdCliente;


            string arrayFiltros = parametros[0];
            DateTime dtInicial = DateTime.Today.AddYears(-5);
            DateTime dtFinal = DateTime.Today;
            string tipoRelatorio = parametros[3];

            if (!string.IsNullOrEmpty(parametros[1]))
            {
                dtInicial = Convert.ToDateTime(parametros[1]);
            }

            if (!string.IsNullOrEmpty(parametros[2]))
            {
                dtFinal = Convert.ToDateTime(parametros[2] + " 23:59:59");
            }

            DataSet recolhidos = recolhidoData.ObterLista(arrayFiltros, dtInicial, dtFinal, tipoRelatorio);

            //Ordenação por data devolução
            DataView Dw = new DataView(recolhidos.Tables[0]);
            Dw.Sort = "DT_DEVOLUCAO";
            recolhidos.Tables.Clear();
            recolhidos.Tables.Add(Dw.Table);

            var reportDS = new ReportDataSource();
            reportDS.Name = "DataSet1";
            reportDS.Value = recolhidos.Tables[0];

            if (recolhidos.Tables[0].Rows.Count == 0)
            {
                pnlMensagem.Visible = true;
            }
            else
            {
                ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
            }

            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Recolhidos\ReportRecolhidos.rdlc";
            //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
            ReportViewer1.LocalReport.DataSources.Add(reportDS);
            ReportViewer1.LocalReport.Refresh();

        }

        //private string IdKey => Request.QueryString["IdKey"];

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        RecolhidosData recolhidoData = new RecolhidosData();

        //        string[] parametros = IdKey.Split('|');
        //        string dtInicial = parametros[2];
        //        string dtFinal = parametros[3];
        //        string cdAtivo = parametros[1];
        //        if (parametros[0] == "" || parametros[0] == null)
        //            parametros[0] = "0";
        //        Int64 cdCliente = Convert.ToInt32(parametros[0]);

        //        if (string.IsNullOrEmpty(dtInicial))
        //            dtInicial = DateTime.Now.AddYears(-5).ToString("dd/MM/yyyy");
        //        if (string.IsNullOrEmpty(dtFinal))
        //            dtFinal = DateTime.Now.ToString("dd/MM/yyyy");

        //        RecolhidosEntity recolhidosEntity = new RecolhidosEntity();
        //        recolhidosEntity.DT_DEV_INICIAL = Convert.ToDateTime(dtInicial + " 00:00:00");
        //        recolhidosEntity.DT_DEV_FINAL = Convert.ToDateTime(dtFinal + " 23:59:59");
        //        recolhidosEntity.CD_ATIVO_FIXO = cdAtivo;
        //        recolhidosEntity.CD_CLIENTE = cdCliente;

        //        DataSet recolhidos = recolhidoData.ObterLista(recolhidosEntity);

        //        //Ordenação por data devolução
        //        DataView Dw = new DataView(recolhidos.Tables[0]);
        //        Dw.Sort = "DT_DEVOLUCAO";
        //        recolhidos.Tables.Clear();
        //        recolhidos.Tables.Add(Dw.Table);

        //        var reportDS = new ReportDataSource();
        //        reportDS.Name = "DataSet1";
        //        reportDS.Value = recolhidos.Tables[0];

        //        if (recolhidos.Tables[0].Rows.Count == 0)
        //            pnlMensagem.Visible = true;
        //        else
        //            ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        //            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Recolhidos\ReportRecolhidos.rdlc";
        //            //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("recolhidos", (System.Data.DataTable)recolhidos.Tables[0]));
        //            ReportViewer1.LocalReport.DataSources.Add(reportDS);
        //            ReportViewer1.LocalReport.Refresh();
        //    }
        //}

        private void ReportViewer1_Load(object sender, EventArgs e)
        {
            SetVisibleExportOption("PDF, Excel");
        }

        /// <summary>
        /// Atribuir visibilidade false para opção de exportar
        /// </summary>
        /// <param name="exportOption">Excel, EXCELOPENXML, IMAGE, PDF, WORD ou WORDOPENXML</param>
        private void SetVisibleExportOption(string exportOption)
        {
            Action<RenderingExtension, bool> actionVisible = new Action<RenderingExtension, bool>((extension, visible) =>
            {
                if (extension != null)
                {
                    System.Reflection.FieldInfo fieldInfo = extension.GetType().GetField("m_isVisible", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    fieldInfo.SetValue(extension, visible);
                }
            });

            var listExtensions = ReportViewer1.LocalReport.ListRenderingExtensions().ToList();
            listExtensions.ForEach(e => actionVisible(e, false));

            RenderingExtension extensionToShow = listExtensions.Find(x => x.Name.Equals(exportOption, StringComparison.CurrentCultureIgnoreCase));
            actionVisible(extensionToShow, true);
        }
    }
}