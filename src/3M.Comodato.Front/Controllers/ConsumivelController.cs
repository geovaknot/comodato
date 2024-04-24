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
    public class ConsumivelController : BaseController
    {
        // GET: Consumivel
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Consumivel> consumiveis = new List<Models.Consumivel>();

            try
            {
                ConsumivelEntity consumivelEntity = new ConsumivelEntity();
                DataTableReader dataTableReader = new ConsumivelData().ObterLista(consumivelEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Consumivel consumivel = new Models.Consumivel
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_CONSUMIVEL"].ToString()),
                            CD_CONSUMIVEL = dataTableReader["CD_CONSUMIVEL"].ToString(),
                            DS_CONSUMIVEL = dataTableReader["DS_CONSUMIVEL"].ToString(),
                            TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                            CD_COMMODITY = dataTableReader["CD_COMMODITY"].ToString(),
                            DS_COMMODITY = dataTableReader["DS_COMMODITY"].ToString(),
                            CD_MAJOR = dataTableReader["CD_MAJOR"].ToString(),
                            DS_MAJOR = dataTableReader["DS_MAJOR"].ToString(),
                            CD_FAMILY = dataTableReader["CD_FAMILY"].ToString(),
                            DS_FAMILY = dataTableReader["DS_FAMILY"].ToString(),
                            CD_SUB_FAMILY = dataTableReader["CD_SUB_FAMILY"].ToString(),
                            DS_SUB_FAMILY = dataTableReader["DS_SUB_FAMILY"].ToString(),
                            TX_UNIDADE_CONV = dataTableReader["TX_UNIDADE_CONV"].ToString(),
                            linhaProduto = new LinhaProdutoEntity
                            {
                                CD_LINHA_PRODUTO = Convert.ToInt32("0" + dataTableReader["CD_LINHA_PRODUTO"].ToString()),
                                DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString(),
                                VL_EXPECTATIVA_PADRAO = Convert.ToDecimal("0" + dataTableReader["VL_EXPECTATIVA_PADRAO"].ToString())
                            }
                        };
                        if (dataTableReader["ST_ATIVO"] != DBNull.Value)
                            consumivel.cdsST_ATIVO = (Convert.ToBoolean(dataTableReader["ST_ATIVO"]) == true ? "Ativo" : "Inativo");

                        //if (dataTableReader["BPCS"] != DBNull.Value)
                        //    consumivel.BPCS = Convert.ToBoolean(dataTableReader["BPCS"]);

                        consumiveis.Add(consumivel);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Consumivel> iConsumiveis = consumiveis;
            return View(iConsumiveis);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Consumivel consumivel = new Models.Consumivel
            {
                linhasProdutos = ObterListaLinhaProduto(),
                CancelarVerificarCodigo = false
            };

            return View(consumivel);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Consumivel consumivel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ConsumivelEntity consumivelEntity = new ConsumivelEntity();

                    consumivelEntity.CD_CONSUMIVEL = consumivel.CD_CONSUMIVEL;
                    consumivelEntity.linhaProduto.CD_LINHA_PRODUTO = consumivel.linhaProduto.CD_LINHA_PRODUTO;
                    consumivelEntity.DS_CONSUMIVEL = consumivel.DS_CONSUMIVEL;
                    consumivelEntity.TX_UNIDADE = consumivel.TX_UNIDADE;
                    consumivelEntity.CD_COMMODITY = consumivel.CD_COMMODITY;
                    consumivelEntity.DS_COMMODITY = consumivel.DS_COMMODITY;
                    consumivelEntity.CD_MAJOR = consumivel.CD_MAJOR;
                    consumivelEntity.DS_MAJOR = consumivel.DS_MAJOR;
                    consumivelEntity.CD_FAMILY = consumivel.CD_FAMILY;
                    consumivelEntity.DS_FAMILY = consumivel.DS_FAMILY;
                    consumivelEntity.CD_SUB_FAMILY = consumivel.CD_SUB_FAMILY;
                    consumivelEntity.DS_SUB_FAMILY = consumivel.DS_SUB_FAMILY;
                    consumivelEntity.TX_UNIDADE_CONV = consumivel.TX_UNIDADE_CONV;
                    consumivelEntity.BPCS = consumivel.BPCS;
                    consumivelEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new ConsumivelData().Inserir(ref consumivelEntity);

                    consumivel.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            consumivel.linhasProdutos = ObterListaLinhaProduto();
            consumivel.CancelarVerificarCodigo = false;

            return View(consumivel); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Consumivel consumivel = null;

            try
            {
                ConsumivelEntity consumivelEntity = new ConsumivelEntity();

                consumivelEntity.CD_CONSUMIVEL = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new ConsumivelData().ObterLista(consumivelEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        consumivel = new Models.Consumivel
                        {
                            CD_CONSUMIVEL = dataTableReader["CD_CONSUMIVEL"].ToString(),
                            DS_CONSUMIVEL = dataTableReader["DS_CONSUMIVEL"].ToString(),
                            TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                            CD_COMMODITY = dataTableReader["CD_COMMODITY"].ToString(),
                            DS_COMMODITY = dataTableReader["DS_COMMODITY"].ToString(),
                            CD_MAJOR = dataTableReader["CD_MAJOR"].ToString(),
                            DS_MAJOR = dataTableReader["DS_MAJOR"].ToString(),
                            CD_FAMILY = dataTableReader["CD_FAMILY"].ToString(),
                            DS_FAMILY = dataTableReader["DS_FAMILY"].ToString(),
                            CD_SUB_FAMILY = dataTableReader["CD_SUB_FAMILY"].ToString(),
                            DS_SUB_FAMILY = dataTableReader["DS_SUB_FAMILY"].ToString(),
                            TX_UNIDADE_CONV = dataTableReader["TX_UNIDADE_CONV"].ToString(),
                            CUSTO = Convert.ToDecimal("0" + dataTableReader["CUSTO"]).ToString("C2"),
                            linhaProduto = new LinhaProdutoEntity
                            {
                                CD_LINHA_PRODUTO = Convert.ToInt32("0" + dataTableReader["CD_LINHA_PRODUTO"].ToString()),
                                DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString(),
                                VL_EXPECTATIVA_PADRAO = Convert.ToDecimal("0" + dataTableReader["VL_EXPECTATIVA_PADRAO"].ToString())
                            },
                            linhasProdutos = ObterListaLinhaProduto(),
                            CancelarVerificarCodigo = true
                        };
                        if (dataTableReader["ST_ATIVO"] != DBNull.Value)
                            consumivel.ST_ATIVO = Convert.ToBoolean(dataTableReader["ST_ATIVO"]);
                        else
                            consumivel.ST_ATIVO = false;
                        //if (dataTableReader["BPCS"] != DBNull.Value)
                        //    consumivel.BPCS = Convert.ToBoolean(dataTableReader["BPCS"]);
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

            if (consumivel == null)
                return HttpNotFound();
            else
                return View(consumivel);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Consumivel consumivel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ConsumivelEntity consumivelEntity = new ConsumivelEntity();

                    consumivelEntity.CD_CONSUMIVEL = consumivel.CD_CONSUMIVEL;
                    consumivelEntity.linhaProduto.CD_LINHA_PRODUTO = consumivel.linhaProduto.CD_LINHA_PRODUTO;
                    consumivelEntity.DS_CONSUMIVEL = consumivel.DS_CONSUMIVEL;
                    consumivelEntity.TX_UNIDADE = consumivel.TX_UNIDADE;
                    consumivelEntity.CD_COMMODITY = consumivel.CD_COMMODITY;
                    consumivelEntity.DS_COMMODITY = consumivel.DS_COMMODITY;
                    consumivelEntity.CD_MAJOR = consumivel.CD_MAJOR;
                    consumivelEntity.DS_MAJOR = consumivel.DS_MAJOR;
                    consumivelEntity.CD_FAMILY = consumivel.CD_FAMILY;
                    consumivelEntity.DS_FAMILY = consumivel.DS_FAMILY;
                    consumivelEntity.CD_SUB_FAMILY = consumivel.CD_SUB_FAMILY;
                    consumivelEntity.DS_SUB_FAMILY = consumivel.DS_SUB_FAMILY;
                    consumivelEntity.TX_UNIDADE_CONV = consumivel.TX_UNIDADE_CONV;
                    consumivelEntity.BPCS = consumivel.BPCS;
                    consumivelEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    consumivelEntity.ST_ATIVO = Convert.ToBoolean(consumivel.ST_ATIVO);

                    new ConsumivelData().Alterar(consumivelEntity);

                    consumivel.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            consumivel.linhasProdutos = ObterListaLinhaProduto();
            consumivel.CancelarVerificarCodigo = false;

            return View(consumivel); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Consumivel consumivel = null;

            try
            {
                ConsumivelEntity consumivelEntity = new ConsumivelEntity();

                consumivelEntity.CD_CONSUMIVEL = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new ConsumivelData().ObterLista(consumivelEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        consumivel = new Models.Consumivel
                        {
                            CD_CONSUMIVEL = dataTableReader["CD_CONSUMIVEL"].ToString(),
                            DS_CONSUMIVEL = dataTableReader["DS_CONSUMIVEL"].ToString(),
                            TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                            CD_COMMODITY = dataTableReader["CD_COMMODITY"].ToString(),
                            DS_COMMODITY = dataTableReader["DS_COMMODITY"].ToString(),
                            CD_MAJOR = dataTableReader["CD_MAJOR"].ToString(),
                            DS_MAJOR = dataTableReader["DS_MAJOR"].ToString(),
                            CD_FAMILY = dataTableReader["CD_FAMILY"].ToString(),
                            DS_FAMILY = dataTableReader["DS_FAMILY"].ToString(),
                            CD_SUB_FAMILY = dataTableReader["CD_SUB_FAMILY"].ToString(),
                            DS_SUB_FAMILY = dataTableReader["DS_SUB_FAMILY"].ToString(),
                            TX_UNIDADE_CONV = dataTableReader["TX_UNIDADE_CONV"].ToString(),
                            CUSTO = Convert.ToDecimal("0" + dataTableReader["CUSTO"]).ToString("C2"),
                            linhaProduto = new LinhaProdutoEntity
                            {
                                CD_LINHA_PRODUTO = Convert.ToInt32("0" + dataTableReader["CD_LINHA_PRODUTO"].ToString()),
                                DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString(),
                                VL_EXPECTATIVA_PADRAO = Convert.ToDecimal("0" + dataTableReader["VL_EXPECTATIVA_PADRAO"].ToString())
                            },
                            //linhasProdutos = ObterListaLinhaProduto(),
                            //CancelarVerificarCodigo = true

                        };
                        //if (dataTableReader["BPCS"] != DBNull.Value)
                        //    consumivel.BPCS = Convert.ToBoolean(dataTableReader["BPCS"]);
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

            if (consumivel == null)
                return HttpNotFound();
            else
                return View(consumivel);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Consumivel consumivel = new Models.Consumivel();
            try
            {
                if (ModelState.IsValid)
                {
                    ConsumivelEntity consumivelEntity = new ConsumivelEntity();

                    consumivelEntity.CD_CONSUMIVEL = ControlesUtility.Criptografia.Descriptografar(idKey);
                    consumivelEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new ConsumivelData().Excluir(consumivelEntity);

                    consumivel.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(consumivel);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        public ActionResult VerificarCodigo(string CD_CONSUMIVEL, bool CancelarVerificarCodigo)
        {
            bool Liberado = true;

            try
            {
                if (CancelarVerificarCodigo == false)
                {
                    ConsumivelEntity consumivelEntity = new ConsumivelEntity();

                    consumivelEntity.CD_CONSUMIVEL = CD_CONSUMIVEL;
                    DataTableReader dataTableReader = new ConsumivelData().ObterLista(consumivelEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                            Liberado = false;
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return Json(Liberado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VerificarCodigoJson(string CD_CONSUMIVEL)
        {
            bool Redirecionar = false;
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                ConsumivelEntity consumivelEntity = new ConsumivelEntity();

                consumivelEntity.CD_CONSUMIVEL = CD_CONSUMIVEL;
                DataTableReader dataTableReader = new ConsumivelData().ObterLista(consumivelEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        Redirecionar = true;
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (Redirecionar == true)
                {
                    jsonResult.Add("Status", "Redirecionar");
                    jsonResult.Add("idKey", Utility.ControlesUtility.Criptografia.Criptografar(consumivelEntity.CD_CONSUMIVEL));
                }
                else
                {
                    // Busca na BPCS para importação
                    Models.Consumivel consumivel = new Models.Consumivel();

                    dataTableReader = new ConsumivelData().ObterListaBPCS(consumivelEntity).CreateDataReader();
                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            consumivel.CD_CONSUMIVEL = dataTableReader["COD_PRD_PRD"].ToString();
                            consumivel.linhaProduto.CD_LINHA_PRODUTO = 0;
                            consumivel.DS_CONSUMIVEL = dataTableReader["DSC_PRD_PRD"].ToString();
                            consumivel.TX_UNIDADE = dataTableReader["COD_UNIT_MEASURE"].ToString();
                            consumivel.CD_COMMODITY = dataTableReader["COD_PRD_COM"].ToString();
                            consumivel.DS_COMMODITY = dataTableReader["DSC_PRD_COM"].ToString();
                            consumivel.CD_MAJOR = dataTableReader["COD_PRD_MJR"].ToString();
                            consumivel.DS_MAJOR = dataTableReader["DSC_PRD_MJR"].ToString();
                            consumivel.CD_FAMILY = dataTableReader["COD_PRD_FAM"].ToString();
                            consumivel.DS_FAMILY = dataTableReader["DSC_PRD_FAM"].ToString();
                            consumivel.CD_SUB_FAMILY = dataTableReader["COD_PRD_SBF"].ToString();
                            consumivel.DS_SUB_FAMILY = dataTableReader["DSC_PRD_SBF"].ToString();
                            consumivel.TX_UNIDADE_CONV = dataTableReader["COD_UNIT_CONV"].ToString();
                            consumivel.CUSTO = string.Empty;
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    jsonResult.Add("Status", "Permanecer");
                    jsonResult.Add("consumivel", consumivel);
                }
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

    }
}