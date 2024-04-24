using System;
using System.Collections;
//using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
//using System.Reflection;
using System.Web.Mvc;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using OfficeOpenXml;
//using OfficeOpenXml;

namespace _3M.Comodato.Front.Controllers
{
    public class PlanoZeroController : BaseController
    {
        public ActionResult Index()
        {
            EmptyResult e = new EmptyResult();
            return View(e);
        }
        
        public JsonResult ListarPlanoZero(string ccdGrupoModelo)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            try
            {
                int nSequencia = 0;
                Func<DataRow, PlanoZero> modelConverter = new Func<DataRow, PlanoZero>((dr) =>
                {
                    nSequencia++;
                    PlanoZero planoZero = new PlanoZero();
                    planoZero.nSequencia = nSequencia;

                    planoZero.nidPlanoZero = Convert.ToInt64(dr["ID_PLANO_ZERO"]);
                    planoZero.PecaModel.CD_PECA = planoZero.ccdPeca = dr["CD_PECA"].ToString();
                    planoZero.PecaModel.DS_PECA = dr["DS_PECA"].ToString();

                    planoZero.ModeloModel.CD_MODELO = planoZero.ccdModelo = dr["CD_MODELO"].ToString();
                    //planoZero.ModeloModel.DS_MODELO = dr["DS_MODELO"].ToString();

                    planoZero.grupoModelo.CD_GRUPO_MODELO = dr["CD_GRUPO_MODELO"].ToString();
                    //planoZero.grupoModelo.ID_GRUPO_MODELO = Convert.ToInt32(dr["ID_GRUPO_MODELO"].ToString());

                    planoZero.nqtPecaModelo = Convert.ToDecimal(dr["QT_PECA_MODELO"]).ToString("N0");

                    if (!String.IsNullOrEmpty(dr["QT_ESTOQUE_MINIMO"].ToString()))
                        planoZero.nqtEstoqueMinimo = Convert.ToDecimal(dr["QT_ESTOQUE_MINIMO"]).ToString("N2");

                    planoZero.ccdCriticidadeAbc = dr["CD_CRITICIDADE_ABC"].ToString();
                    planoZero.nidUsuarioAtualizacao = Convert.ToInt64(dr["ID_USU_RESPONSAVEL"]);
                    planoZero.dtmDataHoraAtualizacao = Convert.ToDateTime(dr["DT_CRIACAO"]);

                    planoZero.nqtEstoqueATec = Convert.ToInt32(dr["QT_PECA_3M1"]).ToString("N2");
                    planoZero.nqtEstoqueRec = Convert.ToInt32(dr["QT_PECA_3M2"]).ToString("N2");

                    return planoZero;
                });


                PlanoZeroEntity filtroPlanoZero = new PlanoZeroEntity();
                //filtroPlanoZero.ccdModelo = ccdModelo;
                filtroPlanoZero.grupoModelo.CD_GRUPO_MODELO = ccdGrupoModelo;

                PlanoZeroData planoZeroData = new PlanoZeroData();
                DataTable dtPlanoZero = planoZeroData.ObterLista(filtroPlanoZero);
                var listaPlanoZero = from dr in dtPlanoZero.Rows.Cast<DataRow>()
                                     select modelConverter(dr);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/PlanoZero/_gridPlanoZero.cshtml", listaPlanoZero.ToList()));
                jsonResult.Add("Status", "Success");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }
        
        public ActionResult DownloadExcel(string ccdGrupoModel)
        {
            
            int nSequencia = 0;
            Func<DataRow, PlanoZero> modelConverter = new Func<DataRow, PlanoZero>((dr) =>
            {
                nSequencia++;
                PlanoZero planoZero = new PlanoZero();
                planoZero.nSequencia = nSequencia;

                planoZero.nidPlanoZero = Convert.ToInt64(dr["ID_PLANO_ZERO"]);
                planoZero.PecaModel.CD_PECA = planoZero.ccdPeca = dr["CD_PECA"].ToString();
                planoZero.PecaModel.DS_PECA = dr["DS_PECA"].ToString();

                planoZero.ModeloModel.CD_MODELO = planoZero.ccdModelo = dr["CD_MODELO"].ToString();
                //planoZero.ModeloModel.DS_MODELO = dr["DS_MODELO"].ToString();

                planoZero.grupoModelo.CD_GRUPO_MODELO = dr["CD_GRUPO_MODELO"].ToString();
                //planoZero.grupoModelo.ID_GRUPO_MODELO = Convert.ToInt32(dr["ID_GRUPO_MODELO"].ToString());

                planoZero.nqtPecaModelo = Convert.ToDecimal(dr["QT_PECA_MODELO"]).ToString("N0");

                if (!String.IsNullOrEmpty(dr["QT_ESTOQUE_MINIMO"].ToString()))
                    planoZero.nqtEstoqueMinimo = Convert.ToDecimal(dr["QT_ESTOQUE_MINIMO"]).ToString("N2");

                planoZero.ccdCriticidadeAbc = dr["CD_CRITICIDADE_ABC"].ToString();
                planoZero.nidUsuarioAtualizacao = Convert.ToInt64(dr["ID_USU_RESPONSAVEL"]);
                planoZero.dtmDataHoraAtualizacao = Convert.ToDateTime(dr["DT_CRIACAO"]);

                planoZero.nqtEstoqueATec = Convert.ToInt32(dr["QT_PECA_3M1"]).ToString("N2");
                planoZero.nqtEstoqueRec = Convert.ToInt32(dr["QT_PECA_3M2"]).ToString("N2");

                return planoZero;
            });

            PlanoZeroEntity filtroPlanoZero = new PlanoZeroEntity();
            //filtroPlanoZero.ccdModelo = ccdModelo;
            filtroPlanoZero.grupoModelo.CD_GRUPO_MODELO = ccdGrupoModel;

            PlanoZeroData planoZeroData = new PlanoZeroData();
            DataTable dtPlanoZero = planoZeroData.ObterLista(filtroPlanoZero);
            var listaPlanoZero = from dr in dtPlanoZero.Rows.Cast<DataRow>()
                                 select modelConverter(dr);

            if (ccdGrupoModel != null && ccdGrupoModel != "" && ccdGrupoModel != " ")
                listaPlanoZero = listaPlanoZero.ToList();
            else
                listaPlanoZero = listaPlanoZero.OrderBy(x => x.grupoModelo.CD_GRUPO_MODELO).ToList();

            OfficeOpenXml.ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPackage = new OfficeOpenXml.ExcelPackage())
            {

                excelPackage.Workbook.Properties.Author = "3MComodato";
                excelPackage.Workbook.Properties.Title = "PlanoZero";

                //var name = listaPlanoZero.FirstOrDefault().grupoModelo.CD_GRUPO_MODELO;
                List<string> result = listaPlanoZero.Select(o => o.grupoModelo.CD_GRUPO_MODELO).Distinct().ToList();

                foreach (var grupo in result)
                {
                    var listaGrupo = listaPlanoZero.Where(x => x.grupoModelo.CD_GRUPO_MODELO == grupo).ToList();

                    var sheet = excelPackage.Workbook.Worksheets.Add(grupo);
                    sheet.Name = grupo;
                    
                    var lista = (IEnumerable)listaGrupo;
                    object teste = listaGrupo;
                    //var lista = (IEnumerable)listaGrupo.GetValue(teste, null);
                    if (lista == null) continue;
                    var fields = lista.GetType().GetGenericArguments()[0].GetFields(BindingFlags.Public | BindingFlags.Instance);
                    var titulos = new List<string>();
                    titulos.Add("ITEM");
                    titulos.Add("DESCRIÇÃO");
                    titulos.Add("CÓDIGO 3M");
                    titulos.Add("QTDE MAQUINA");
                    titulos.Add("QTDE ESTOQUE A.TÉC.");
                    titulos.Add("QTDE ESTOQUE REC");
                    titulos.Add("CRITICIDADE");

                    sheet.Columns[2].Width = 60;
                    sheet.Columns[3].Width = 14;
                    sheet.Columns[4].Width = 15;
                    sheet.Columns[5].Width = 21;
                    sheet.Columns[6].Width = 19;
                    sheet.Columns[7].Width = 13;

                    var i = 1;
                    foreach (var titulo in titulos)
                    {
                        sheet.Cells[1, i++].Value = titulo;
                    }

                    var rowIndex = 2;

                    foreach (var item in listaGrupo)
                    {
                        sheet.Cells[rowIndex, 1].Value = item.nSequencia.ToString() ?? "";
                        sheet.Cells[rowIndex, 2].Value = item.PecaModel.DS_PECA ?? "";
                        sheet.Cells[rowIndex, 3].Value = item.ccdPeca ?? "";
                        sheet.Cells[rowIndex, 4].Value = item.nqtPecaModelo ?? "";
                        sheet.Cells[rowIndex, 5].Value = item.nqtEstoqueATec ?? "";
                        sheet.Cells[rowIndex, 6].Value = item.nqtEstoqueRec ?? "";
                        sheet.Cells[rowIndex, 7].Value = item.ccdCriticidadeAbc ?? "";
                        

                        rowIndex++;
                    }

                    
                }

                

                return File(excelPackage.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "PlanoZero-" + DateTime.Now.ToString() + ".xlsx");
            }

        }
        
    }
}

