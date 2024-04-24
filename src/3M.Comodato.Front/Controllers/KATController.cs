using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;

namespace _3M.Comodato.Front.Controllers
{
    public class KATController : Controller
    {
        // GET: KAT
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.RelatorioKAT> relatorioKAT = new List<Models.RelatorioKAT>();

            try
            {
                DataTableReader dataTableReader = new ClienteData().ObterListaKAT().CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.RelatorioKAT KATitem = new Models.RelatorioKAT
                        {
                            cliente = new ClienteEntity()
                            {
                                CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString()
                            },
                            VENDAS = Convert.ToDecimal(dataTableReader["VENDAS"]),
                            GM = Convert.ToDecimal(dataTableReader["GM"]),
                            GM_GRUPO_EMP = Convert.ToInt32(dataTableReader["GM_GRUPO_EMP"]),
                            CRITICIDADE_AMB = Convert.ToInt32(dataTableReader["CRITICIDADE_AMB"]),
                            QTD_ATIVOS = Convert.ToInt32(dataTableReader["QTD_ATIVOS"]),
                            NOTA_QTD_ATIVOS = Convert.ToInt32(dataTableReader["NOTA_QTD_ATIVOS"]),
                            COMPLEXIDADE_EQUIP = Convert.ToInt32(dataTableReader["COMPLEXIDADE_EQUIP"]),
                            SCORE = Convert.ToDecimal(dataTableReader["SCORE"]),
                            CLASSIFICACAO = dataTableReader["CLASSIFICACAO"].ToString(),
                            PERIODOS = Convert.ToInt32(dataTableReader["PERIODOS_KAT"]),
                        };
                        relatorioKAT.Add(KATitem);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.RelatorioKAT> irelatorioKAT = relatorioKAT;
            return View(irelatorioKAT);

        }
    }
}