using _3M.Comodato.Utility;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Reports.Completos.dsReportCompletosTableAdapters;
using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using System.Text;
using static _3M.Comodato.Front.Reports.Completos.dsReportCompletos;
using System.Collections.Generic;
using _3M.Comodato.Front.Models;
using System.Linq;
using System.Globalization;

namespace _3M.Comodato.Front
{
    public partial class RelatorioCompletos : ReportBasePage
    {
        //private string IdKey => Request.QueryString["IdKey"];
        private DataSet RelatorioData { get; set; }

        protected override void ExibirRelatorio()
        {
            CompletosData completoData = new CompletosData();

            //string[] parametros = idKey.Split('|');

            CompletosEntity completosEntity = new CompletosEntity();

            if (parametros[0] != "" && parametros[0] != null)
                completosEntity.CD_CLIENTE = Convert.ToInt32(parametros[0]);

            if (parametros[1] != "" && parametros[1] != null)
                completosEntity.CD_VENDEDOR = Convert.ToInt32(parametros[1]);

            if (parametros[2] != "" && parametros[2] != null)
                completosEntity.CD_EXECUTIVO = Convert.ToInt32(parametros[2]);

            if (parametros[3] != "" && parametros[3] != null)
                completosEntity.CD_LINHA_PRODUTO = parametros[3].ToString();

            //DataTable completos = completoData.ObterLista(completosEntity);
            DataSet completos = completoData.ObterRelatorioVendasCompleto(completosEntity);

            //ViewState["dsCompletos"] = completos;
            RelatorioData = completos;

            var reportDS = new ReportDataSource();
            reportDS.Name = "DataSet1";
            reportDS.Value = completos.Tables[0];

            Double VL_PECAS = 0;
            Double VL_MAO_OBRA = 0;
            Double TOT_PECAS = 0;
            Double TOT_MAO_OBRA = 0;
            Int64 ID_OS = 0;
            Int64 ID_OS_CORRENTE = 0;
            int MaoDeObraAtualizado = 0;

            List<ManutencaoDetalhesCliente> manutencaoMes = new List<ManutencaoDetalhesCliente>();
            List<ManutencaoDetalhesCliente> manutencaoAno = new List<ManutencaoDetalhesCliente>();

            for (int i = 0; i < completos.Tables[2].Rows.Count; i++)
            {
                ManutencaoDetalhesCliente manutencaodetalhe = new ManutencaoDetalhesCliente();

                
                if (!Convert.IsDBNull(completos.Tables[2].Rows[i]["ID_OS"]))
                    manutencaodetalhe.ID_OS = Convert.ToInt64(completos.Tables[2].Rows[i]["ID_OS"]);
                else
                    completos.Tables[2].Rows[i]["ID_OS"] = "0";

                if (!Convert.IsDBNull(completos.Tables[2].Rows[i]["CD_CLIENTE"]))
                    manutencaodetalhe.CD_CLIENTE = Convert.ToInt64(completos.Tables[2].Rows[i]["CD_CLIENTE"]);
                else
                    completos.Tables[2].Rows[i]["CD_CLIENTE"] = "0";

                if (!Convert.IsDBNull(completos.Tables[2].Rows[i]["TOT_PECAS"]))
                    manutencaodetalhe.TOT_PECAS = Convert.ToDouble("0" + completos.Tables[2].Rows[i]["TOT_PECAS"]);
                else
                    completos.Tables[2].Rows[i]["TOT_PECAS"] = "0"; 

                if (!Convert.IsDBNull(completos.Tables[2].Rows[i]["TOT_MAO_OBRA"]))
                    manutencaodetalhe.TOT_MAO_OBRA = Convert.ToDouble("0" + completos.Tables[2].Rows[i]["TOT_MAO_OBRA"]);
                else
                    completos.Tables[2].Rows[i]["TOT_MAO_OBRA"] = "0";

                if (!Convert.IsDBNull(completos.Tables[2].Rows[i]["DS_LINHA_PRODUTO"]))
                    manutencaodetalhe.DS_LINHA_PRODUTO = completos.Tables[2].Rows[i]["DS_LINHA_PRODUTO"].ToString();
                else
                    completos.Tables[2].Rows[i]["DS_LINHA_PRODUTO"] = "0";  

                if (!Convert.IsDBNull(completos.Tables[2].Rows[i]["CD_LINHA_PRODUTO"]))
                    manutencaodetalhe.CD_LINHA_PRODUTO = Convert.ToInt64(completos.Tables[2].Rows[i]["CD_LINHA_PRODUTO"]);
                else
                    completos.Tables[2].Rows[i]["CD_LINHA_PRODUTO"] = "0";

                manutencaoAno.Add(manutencaodetalhe);
            }

            for (int i = 0; i < completos.Tables[3].Rows.Count; i++)
            {
                ManutencaoDetalhesCliente manutencaodetalhe = new ManutencaoDetalhesCliente();

                if (!Convert.IsDBNull(completos.Tables[3].Rows[i]["ID_OS"]))
                    manutencaodetalhe.ID_OS = Convert.ToInt64(completos.Tables[3].Rows[i]["ID_OS"]);
                else
                    completos.Tables[3].Rows[i]["ID_OS"] = "0";

                if (!Convert.IsDBNull(completos.Tables[3].Rows[i]["CD_CLIENTE"]))
                    manutencaodetalhe.CD_CLIENTE = Convert.ToInt64(completos.Tables[3].Rows[i]["CD_CLIENTE"]);
                else
                    completos.Tables[3].Rows[i]["CD_CLIENTE"] = "0";

                if (!Convert.IsDBNull(completos.Tables[3].Rows[i]["VL_PECAS"]))
                    manutencaodetalhe.TOT_PECAS = Convert.ToDouble("0" + completos.Tables[3].Rows[i]["VL_PECAS"]);
                else
                    completos.Tables[3].Rows[i]["VL_PECAS"] = "0";

                if (!Convert.IsDBNull(completos.Tables[3].Rows[i]["VL_MAO_OBRA"]))
                    manutencaodetalhe.TOT_MAO_OBRA = Convert.ToDouble("0" + completos.Tables[3].Rows[i]["VL_MAO_OBRA"]);
                else
                    completos.Tables[3].Rows[i]["VL_MAO_OBRA"] = "0";

                if (!Convert.IsDBNull(completos.Tables[3].Rows[i]["DS_LINHA_PRODUTO"]))
                    manutencaodetalhe.DS_LINHA_PRODUTO = completos.Tables[3].Rows[i]["DS_LINHA_PRODUTO"].ToString();
                else
                    completos.Tables[3].Rows[i]["DS_LINHA_PRODUTO"] = "0";

                if (!Convert.IsDBNull(completos.Tables[3].Rows[i]["CD_LINHA_PRODUTO"]))
                    manutencaodetalhe.CD_LINHA_PRODUTO = Convert.ToInt64(completos.Tables[3].Rows[i]["CD_LINHA_PRODUTO"]);
                else
                    completos.Tables[3].Rows[i]["CD_LINHA_PRODUTO"] = "0";

                manutencaoMes.Add(manutencaodetalhe);
            }


            //for (int i = 0; i < completos.Tables[2].Rows.Count; i++)
            //{
            //    TOT_PECAS += Convert.ToDouble("0" + completos.Tables[2].Rows[i]["TOT_PECAS"]);

            //    if (ID_OS_CORRENTE == 0)
            //        ID_OS_CORRENTE = Convert.ToInt64(completos.Tables[2].Rows[i]["ID_OS"].ToString());

            //    ID_OS = Convert.ToInt64(completos.Tables[2].Rows[i]["ID_OS"].ToString());

            //    if (ID_OS == ID_OS_CORRENTE && MaoDeObraAtualizado == 0) { 
            //        TOT_MAO_OBRA += Convert.ToDouble("0" + completos.Tables[2].Rows[i]["TOT_MAO_OBRA"]);
            //        MaoDeObraAtualizado++;
            //    }
            //    else if(ID_OS != ID_OS_CORRENTE && MaoDeObraAtualizado == 1)
            //    {
            //        MaoDeObraAtualizado = 0;
            //        ID_OS_CORRENTE = ID_OS;
            //    }
            //    if (ID_OS == ID_OS_CORRENTE && MaoDeObraAtualizado == 0)
            //    {
            //        TOT_MAO_OBRA += Convert.ToDouble("0" + completos.Tables[2].Rows[i]["TOT_MAO_OBRA"]);
            //        MaoDeObraAtualizado++;
            //    }
            //}

            ID_OS = 0;
            ID_OS_CORRENTE = 0;
            MaoDeObraAtualizado = 0;

            //for (int i = 0; i < completos.Tables[3].Rows.Count; i++)
            //{
            //    VL_PECAS += Convert.ToDouble("0" + completos.Tables[3].Rows[i]["VL_PECAS"]);

            //    if (ID_OS_CORRENTE == 0)
            //        ID_OS_CORRENTE = Convert.ToInt64(completos.Tables[3].Rows[i]["ID_OS"].ToString());

            //    ID_OS = Convert.ToInt64(completos.Tables[3].Rows[i]["ID_OS"].ToString());

            //    if (ID_OS == ID_OS_CORRENTE && MaoDeObraAtualizado == 0)
            //    {
            //        VL_MAO_OBRA += Convert.ToDouble("0" + completos.Tables[3].Rows[i]["VL_MAO_OBRA"]);
            //        MaoDeObraAtualizado++;
            //    }
            //    else if (ID_OS != ID_OS_CORRENTE && MaoDeObraAtualizado == 1)
            //    {
            //        MaoDeObraAtualizado = 0;
            //        ID_OS_CORRENTE = ID_OS;
            //    }

            //    if (ID_OS == ID_OS_CORRENTE && MaoDeObraAtualizado == 0)
            //    {
            //        VL_MAO_OBRA += Convert.ToDouble("0" + completos.Tables[3].Rows[i]["VL_MAO_OBRA"]);
            //        MaoDeObraAtualizado++;
            //    }

            //}

            completos.Tables[1].Columns.Add("VL_PECAS", typeof(System.Double));
            completos.Tables[1].Columns.Add("VL_MAO_OBRA", typeof(System.Double));
            completos.Tables[1].Columns.Add("TOT_PECAS", typeof(System.Double));
            completos.Tables[1].Columns.Add("TOT_MAO_OBRA", typeof(System.Double));

            Decimal ValorTotal = 0;

            for (int i = 0; i < completos.Tables[1].Rows.Count; i++)
            {
                ValorTotal = 0;
                var tipoProduto = completos.Tables[1].Rows[i]["CD_LINHA_PRODUTO"].ToString();
                var cliente = completos.Tables[1].Rows[i]["CD_CLIENTE"].ToString();
                VL_PECAS = 0;
                VL_MAO_OBRA = 0;
                TOT_PECAS = 0;
                TOT_MAO_OBRA = 0;

                if (tipoProduto != null && tipoProduto != "0" && cliente != null && cliente != "0")
                {
                    var listaManutencaoTipoProduto = manutencaoAno.Where(x => x.CD_LINHA_PRODUTO.ToString().ToUpper() == tipoProduto.ToUpper()
                                                                            && x.CD_CLIENTE.ToString().ToUpper() == cliente.ToUpper()).ToList();
                    if (listaManutencaoTipoProduto?.Count == 0)
                        ValorTotal = 0;
                    else
                    {
                        foreach (var manutencao in listaManutencaoTipoProduto)
                        {
                            TOT_PECAS += Convert.ToDouble(manutencao.TOT_PECAS);

                            if (ID_OS_CORRENTE == 0)
                                ID_OS_CORRENTE = Convert.ToInt64(manutencao.ID_OS);

                            ID_OS = Convert.ToInt64(manutencao.ID_OS);

                            if (ID_OS == ID_OS_CORRENTE && MaoDeObraAtualizado == 0)
                            {
                                TOT_MAO_OBRA += Convert.ToDouble(manutencao.TOT_MAO_OBRA);
                                MaoDeObraAtualizado++;
                            }
                            else if (ID_OS != ID_OS_CORRENTE && MaoDeObraAtualizado == 1)
                            {
                                MaoDeObraAtualizado = 0;
                                ID_OS_CORRENTE = ID_OS;
                            }
                            if (ID_OS == ID_OS_CORRENTE && MaoDeObraAtualizado == 0)
                            {
                                TOT_MAO_OBRA += Convert.ToDouble(manutencao.TOT_MAO_OBRA);
                                MaoDeObraAtualizado++;
                            }
                        }
                        ValorTotal = Convert.ToDecimal(TOT_MAO_OBRA + TOT_PECAS);
                    }
                }
                else
                {
                    ValorTotal = 0;
                }

                if (tipoProduto != null)
                {
                    var listaManutencaoTipoProduto = manutencaoMes.Where(x => x.CD_LINHA_PRODUTO.ToString().ToUpper() == tipoProduto.ToUpper()
                                                                            && x.CD_CLIENTE.ToString().ToUpper() == cliente.ToUpper()).ToList();
                    if (listaManutencaoTipoProduto?.Count == 0)
                        ValorTotal = 0;
                    else
                    {
                        foreach (var manutencao in listaManutencaoTipoProduto)
                        {
                            VL_PECAS += Convert.ToDouble(manutencao.TOT_PECAS);

                            if (ID_OS_CORRENTE == 0)
                                ID_OS_CORRENTE = Convert.ToInt64(manutencao.ID_OS);

                            ID_OS = Convert.ToInt64(manutencao.ID_OS);

                            if (ID_OS == ID_OS_CORRENTE && MaoDeObraAtualizado == 0)
                            {
                                VL_MAO_OBRA += Convert.ToDouble(manutencao.TOT_MAO_OBRA);
                                MaoDeObraAtualizado++;
                            }
                            else if (ID_OS != ID_OS_CORRENTE && MaoDeObraAtualizado == 1)
                            {
                                MaoDeObraAtualizado = 0;
                                ID_OS_CORRENTE = ID_OS;
                            }
                            if (ID_OS == ID_OS_CORRENTE && MaoDeObraAtualizado == 0)
                            {
                                VL_MAO_OBRA += Convert.ToDouble(manutencao.TOT_MAO_OBRA);
                                MaoDeObraAtualizado++;
                            }
                        }
                        ValorTotal = Convert.ToDecimal(VL_PECAS + VL_MAO_OBRA);
                    }
                }
                else
                {
                    ValorTotal = 0;
                }

                completos.Tables[1].Rows[i]["TOT_PECAS"] = TOT_PECAS;
                completos.Tables[1].Rows[i]["TOT_MAO_OBRA"] = TOT_MAO_OBRA;

                completos.Tables[1].Rows[i]["VL_PECAS"] = VL_PECAS;
                completos.Tables[1].Rows[i]["VL_MAO_OBRA"] = VL_MAO_OBRA;

                TOT_PECAS = 0;
                TOT_MAO_OBRA = 0;
                VL_PECAS = 0;
                VL_MAO_OBRA = 0;
            }

            //if (completos.Rows.Count == 0)
            if (completos.Tables[0].Rows.Count == 0)
            {
                pnlMensagem.Visible = true;
            }
            else
            {
                //ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Completos\ReportCompletos.rdlc";
            }
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.DataSources.Add(reportDS);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubrptProcessingEventHandler);
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubItensConsumidosProcessingEventHandler);

            ReportViewer1.LocalReport.Refresh();

        }

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        CompletosData completoData = new CompletosData();

        //        string[] parametros = IdKey.Split('|');

        //        CompletosEntity completosEntity = new CompletosEntity();

        //        if (parametros[0] != "" && parametros[0] != null)
        //        {
        //            completosEntity.CD_CLIENTE = Convert.ToInt32(parametros[0]);
        //        }
        //        if (parametros[1] != "" && parametros[1] != null)
        //        {
        //            completosEntity.CD_VENDEDOR = Convert.ToInt32(parametros[1]);
        //        }
        //        if (parametros[2] != "" && parametros[2] != null)
        //        {
        //            completosEntity.CD_EXECUTIVO = Convert.ToInt32(parametros[2]);
        //        }
        //        if (parametros[3] != "" && parametros[3] != null)
        //        {
        //            completosEntity.CD_LINHA_PRODUTO = parametros[3].ToString();
        //        }

        //        //DataTable completos = completoData.ObterLista(completosEntity);
        //        DataSet completos = completoData.ObterRelatorioVendasCompleto(completosEntity);

        //        //ViewState["dsCompletos"] = completos;
        //        RelatorioData = completos;

        //        var reportDS = new ReportDataSource();
        //        reportDS.Name = "DataSet1";
        //        reportDS.Value = completos.Tables[0];

        //        //if (completos.Rows.Count == 0)
        //        if (completos.Tables[0].Rows.Count == 0)
        //        {
        //            pnlMensagem.Visible = true;
        //        }
        //        else
        //        {
        //            //ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        //            ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\Completos\ReportCompletos.rdlc";
        //        }
        //        ReportViewer1.ProcessingMode = ProcessingMode.Local;
        //        ReportViewer1.LocalReport.DataSources.Add(reportDS);
        //        ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubrptProcessingEventHandler);
        //        ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubItensConsumidosProcessingEventHandler);

        //        ReportViewer1.LocalReport.Refresh();
        //    }
        //}

        private void SubItensConsumidosProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            //prcRelatorioCompletosSubItensConsumidosSelectDataTable dtItensConsumidos = new prcRelatorioCompletosSubItensConsumidosSelectDataTable();
            //using (prcRelatorioCompletosSubItensConsumidosSelectTableAdapter subItensAdapter = new prcRelatorioCompletosSubItensConsumidosSelectTableAdapter())
            //{
            //    long? codigoCliente = null;
            //    if (long.Parse(e.Parameters[0].Values[0]) > 0)
            //    {
            //        codigoCliente = long.Parse(e.Parameters[0].Values[0]);
            //    }

            //    subItensAdapter.Fill(dtItensConsumidos, int.Parse(e.Parameters[1].Values[0]), codigoCliente, null, null);
            //}

            StringBuilder sbFiltro = new StringBuilder();
            if (int.Parse(e.Parameters[0].Values[0]) > 0)
            {
                sbFiltro.Append($"CD_CLIENTE={e.Parameters[0].Values[0]} AND ");
            }
            sbFiltro.Append($"CD_LINHA_PRODUTO={e.Parameters[1].Values[0]}");

            DataTable source = RelatorioData.Tables[4].Clone();
            DataRow[] rows = RelatorioData.Tables[4].Select(sbFiltro.ToString());
            foreach (var row in rows)
            {
                source.ImportRow(row);
            }

            ReportDataSource dsItensConsumidos = new ReportDataSource("dsItensConsumidos", source);//RelatorioData.Tables[2]);//(DataTable)dtItensConsumidos);
            e.DataSources.Add(dsItensConsumidos);
        }

        private void SubrptProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            //CompletosData compData = new CompletosData();
            //CompletosEntity compEntity = new CompletosEntity();
            //if (int.Parse(e.Parameters[0].Values[0]) > 0)
            //{
            //    compEntity.CD_CLIENTE = int.Parse(e.Parameters[0].Values[0]);
            //}

            //compEntity.CD_LINHA_PRODUTO = e.Parameters[1].Values[0];
            //DataTable dtSub = compData.ObterListaSubReport(compEntity);

            StringBuilder sbFiltro = new StringBuilder();
            if (int.Parse(e.Parameters[0].Values[0]) > 0)
            {
                sbFiltro.Append($"CD_CLIENTE={e.Parameters[0].Values[0]} AND ");
            }
            sbFiltro.Append($"CD_LINHA_PRODUTO={e.Parameters[1].Values[0]}");

            DataTable source = RelatorioData.Tables[1].Clone();
            DataRow[] rows = RelatorioData.Tables[1].Select(sbFiltro.ToString());

            
            foreach (var row in rows)
            {
                source.ImportRow(row);
            }

            ReportDataSource ds = new ReportDataSource("DataSet1", source);//RelatorioData.Tables[1]); //dtSub);
            e.DataSources.Add(ds);
        }
    }
}