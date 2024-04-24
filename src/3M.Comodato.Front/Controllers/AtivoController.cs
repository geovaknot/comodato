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
    public class AtivoController : BaseController
    {

        public int qtdDiasGarantia { get; set; }
        public int qtdDiasGarantiaManutencao { get; set; }

        [_3MAuthentication]
        public ActionResult Index()
        {
            return View();
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Ativo ativo = new Models.Ativo
            {
                TX_ANO_MAQUINA = DateTime.Now.Year,
                DT_INCLUSAO = DateTime.Now.ToString("dd/MM/yyyy"),
                QTD_DIAS_GARANTIA = System.Convert.ToInt32(ControlesUtility.Parametro.ObterValorParametro("QtdDiasGarantia")),
                DT_FIM_GARANTIA = DateTime.Now.AddDays(System.Convert.ToInt32(ControlesUtility.Parametro.ObterValorParametro("QtdDiasGarantia"))).ToString("dd/MM/yyyy"),
                modelos = ObterListaModeloCombo(),
                statusAtivos = ObterListaStatusAtivo(),
                situacoesAtivos = ObterListaSituacaoAtivo(),
                linhasProdutos = ObterListaLinhaProduto(),
                CancelarVerificarCodigo = false
            };

            return View(ativo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Ativo ativo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AtivoFixoEntity ativoEntity = new AtivoFixoEntity();

                    ativoEntity.CD_ATIVO_FIXO = ativo.CD_ATIVO_FIXO;
                    ativoEntity.modelo.CD_MODELO = ativo.modelo.CD_MODELO;
                    if (!string.IsNullOrEmpty(ativo.DT_INCLUSAO))
                        ativoEntity.DT_INCLUSAO = Convert.ToDateTime(ativo.DT_INCLUSAO);
                    ativoEntity.TX_ANO_MAQUINA = ativo.TX_ANO_MAQUINA.ToString();
                    if (!string.IsNullOrEmpty(ativo.DT_INVENTARIO))
                        ativoEntity.DT_INVENTARIO = Convert.ToDateTime(ativo.DT_INVENTARIO);
                    ativoEntity.statusAtivo.CD_STATUS_ATIVO = ativo.statusAtivo.CD_STATUS_ATIVO;
                    ativoEntity.situacaoAtivo.CD_SITUACAO_ATIVO = ativo.situacaoAtivo.CD_SITUACAO_ATIVO;
                    ativoEntity.TX_TIPO = ativo.TX_TIPO;
                    ativoEntity.linhaProduto.CD_LINHA_PRODUTO = ativo.linhaProduto.CD_LINHA_PRODUTO;
                    //ativoEntity.FL_STATUS = true;
                    ativoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    if (!string.IsNullOrEmpty(ativo.DT_FIM_GARANTIA))
                        ativoEntity.DT_FIM_GARANTIA = Convert.ToDateTime(ativo.DT_FIM_GARANTIA);

                    if (!string.IsNullOrEmpty(ativo.DT_FIM_MANUTENCAO))
                        ativoEntity.DT_FIM_MANUTENCAO = Convert.ToDateTime(ativo.DT_FIM_MANUTENCAO);

                    if (!string.IsNullOrEmpty(ativo.DT_MANUTENCAO))
                        ativoEntity.DT_MANUTENCAO = Convert.ToDateTime(ativo.DT_MANUTENCAO);

                    if (!string.IsNullOrEmpty(ativo.DS_MOTIVO))
                        ativoEntity.DS_MOTIVO = ativo.DS_MOTIVO.ToString();

                    new AtivoFixoData().Inserir(ref ativoEntity);

                    /*
                    DepreciacaoEntity depreciacaoEntity = new DepreciacaoEntity();

                    depreciacaoEntity.CD_ATIVO_FIXO = ativo.CD_ATIVO_FIXO;
                    depreciacaoEntity.NR_MESES = ativo.depreciacao.NR_MESES;
                    if (!string.IsNullOrEmpty(ativo.depreciacao.DT_INICIO_DEPREC))
                        depreciacaoEntity.DT_INICIO_DEPREC = Convert.ToDateTime(ativo.depreciacao.DT_INICIO_DEPREC);
                    depreciacaoEntity.VL_CUSTO_ATIVO = Convert.ToDecimal(ativo.depreciacao.VL_CUSTO_ATIVO);
                    depreciacaoEntity.VL_DEPREC_TOTAL = Convert.ToDecimal(ativo.depreciacao.VL_DEPREC_TOTAL);
                    depreciacaoEntity.VL_DEPREC_ULT_MES = Convert.ToDecimal(ativo.depreciacao.VL_DEPREC_ULT_MES);
                    depreciacaoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new DepreciacaoData().Inserir(ref depreciacaoEntity);
                    */

                    if (ativo.TP_ACAO == "GravarSair")
                        ativo.JavaScriptToRun = "MensagemSucesso()";
                    else
                        ativo.JavaScriptToRun = "MensagemSucessoContinuar()";

                    //return RedirectToAction("Index");
                }

                ativo.modelos = ObterListaModeloCombo();
                ativo.statusAtivos = ObterListaStatusAtivo();
                ativo.situacoesAtivos = ObterListaSituacaoAtivo();
                ativo.linhasProdutos = ObterListaLinhaProduto();
                ativo.CancelarVerificarCodigo = false;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(ativo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Ativo ativo = null;

            try
            {
                AtivoFixoEntity ativoEntity = new AtivoFixoEntity();

                ativoEntity.CD_ATIVO_FIXO = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new AtivoFixoData().ObterLista(ativoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        ativo = new Models.Ativo
                        {
                            //idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_ATIVO_FIXO"].ToString()),
                            CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                            modelo = new ModeloEntity
                            {
                                CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                                DS_MODELO = dataTableReader["DS_MODELO"].ToString()
                            },
                            TX_TIPO = dataTableReader["TX_TIPO"].ToString(),
                            depreciacao = new Models.Depreciacao
                            {
                                NR_MESES = Convert.ToInt16("0" + dataTableReader["NR_MESES"])
                            }
                        };
                        try
                        {
                            if (dataTableReader["TX_ANO_MÁQUINA"] != DBNull.Value)
                                ativo.TX_ANO_MAQUINA = Convert.ToInt32("0" + dataTableReader["TX_ANO_MÁQUINA"]);
                        }
                        catch { };

                        if (dataTableReader["CD_STATUS_ATIVO"] != DBNull.Value)
                            ativo.statusAtivo.CD_STATUS_ATIVO = Convert.ToInt64("0" + dataTableReader["CD_STATUS_ATIVO"]);

                        ativo.statusAtivo.DS_STATUS_ATIVO = dataTableReader["DS_STATUS_ATIVO"].ToString();

                        if (dataTableReader["CD_SITUACAO_ATIVO"] != DBNull.Value)
                            ativo.situacaoAtivo.CD_SITUACAO_ATIVO = Convert.ToInt64("0" + dataTableReader["CD_SITUACAO_ATIVO"]);

                        ativo.situacaoAtivo.DS_SITUACAO_ATIVO = dataTableReader["DS_SITUACAO_ATIVO"].ToString();

                        if (dataTableReader["CD_LINHA_PRODUTO"] != DBNull.Value)
                            ativo.linhaProduto.CD_LINHA_PRODUTO = Convert.ToInt32("0" + dataTableReader["CD_LINHA_PRODUTO"]);

                        ativo.linhaProduto.DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString();

                        if (dataTableReader["VL_CUSTO_ATIVO"] != DBNull.Value)
                            ativo.depreciacao.VL_CUSTO_ATIVO = Convert.ToDecimal(dataTableReader["VL_CUSTO_ATIVO"]).ToString("N2");

                        if (dataTableReader["VL_RESIDUAL"] != DBNull.Value)
                            ativo.depreciacao.VL_DEPREC_TOTAL = Convert.ToDecimal(dataTableReader["VL_RESIDUAL"]).ToString("N2");

                        if (dataTableReader["VL_DEPREC_ULT_MES"] != DBNull.Value)
                            ativo.depreciacao.VL_DEPREC_ULT_MES = Convert.ToDecimal(dataTableReader["VL_DEPREC_ULT_MES"]).ToString("N2");

                        if (dataTableReader["DT_INCLUSÃO"] != DBNull.Value)
                            ativo.DT_INCLUSAO = Convert.ToDateTime(dataTableReader["DT_INCLUSÃO"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DT_FIM_GARANTIA"] != DBNull.Value)
                            ativo.DT_FIM_GARANTIA = Convert.ToDateTime(dataTableReader["DT_FIM_GARANTIA"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DT_MANUTENCAO"] != DBNull.Value)
                            ativo.DT_MANUTENCAO = Convert.ToDateTime(dataTableReader["DT_MANUTENCAO"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DT_FIM_MANUTENCAO"] != DBNull.Value)
                            ativo.DT_FIM_MANUTENCAO = Convert.ToDateTime(dataTableReader["DT_FIM_MANUTENCAO"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DS_MOTIVO"] != DBNull.Value)
                            ativo.DS_MOTIVO = dataTableReader["DS_MOTIVO"].ToString();

                        if (dataTableReader["DT_INVENTARIO"] != DBNull.Value)
                            ativo.DT_INVENTARIO = Convert.ToDateTime(dataTableReader["DT_INVENTARIO"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DT_INICIO_DEPREC"] != DBNull.Value)
                            ativo.depreciacao.DT_INICIO_DEPREC = Convert.ToDateTime(dataTableReader["DT_INICIO_DEPREC"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["FL_STATUS"] != DBNull.Value)
                            ativo.FL_STATUS = Convert.ToBoolean(dataTableReader["FL_STATUS"]);


                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                ativo.modelos = ObterListaModeloCombo();
                ativo.statusAtivos = ObterListaStatusAtivo();
                ativo.situacoesAtivos = ObterListaSituacaoAtivo();
                ativo.linhasProdutos = ObterListaLinhaProduto();
                ativo.CancelarVerificarCodigo = true;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (ativo == null)
                return HttpNotFound();
            else
                return View(ativo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Ativo ativo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AtivoFixoEntity ativoEntity = new AtivoFixoEntity();

                    ativoEntity.CD_ATIVO_FIXO = ativo.CD_ATIVO_FIXO;
                    ativoEntity.modelo.CD_MODELO = ativo.modelo.CD_MODELO;
                    if (!string.IsNullOrEmpty(ativo.DT_INCLUSAO))
                        ativoEntity.DT_INCLUSAO = Convert.ToDateTime(ativo.DT_INCLUSAO);
                    ativoEntity.TX_ANO_MAQUINA = ativo.TX_ANO_MAQUINA.ToString();
                    if (!string.IsNullOrEmpty(ativo.DT_INVENTARIO))
                        ativoEntity.DT_INVENTARIO = Convert.ToDateTime(ativo.DT_INVENTARIO);
                    ativoEntity.statusAtivo.CD_STATUS_ATIVO = ativo.statusAtivo.CD_STATUS_ATIVO;
                    ativoEntity.situacaoAtivo.CD_SITUACAO_ATIVO = ativo.situacaoAtivo.CD_SITUACAO_ATIVO;
                    ativoEntity.TX_TIPO = ativo.TX_TIPO;
                    ativoEntity.linhaProduto.CD_LINHA_PRODUTO = ativo.linhaProduto.CD_LINHA_PRODUTO;
                    ativoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    ativoEntity.FL_STATUS = Convert.ToBoolean(ativo.FL_STATUS);

                    if (!string.IsNullOrEmpty(ativo.DT_FIM_GARANTIA))
                        ativoEntity.DT_FIM_GARANTIA = Convert.ToDateTime(ativo.DT_FIM_GARANTIA);

                    if (!string.IsNullOrEmpty(ativo.DT_FIM_MANUTENCAO))
                        ativoEntity.DT_FIM_MANUTENCAO = Convert.ToDateTime(ativo.DT_FIM_MANUTENCAO);

                    if (!string.IsNullOrEmpty(ativo.DT_MANUTENCAO))
                        ativoEntity.DT_MANUTENCAO = Convert.ToDateTime(ativo.DT_MANUTENCAO);

                    if (!string.IsNullOrEmpty(ativo.DS_MOTIVO))
                        ativoEntity.DS_MOTIVO = ativo.DS_MOTIVO.ToString();


                    new AtivoFixoData().Alterar(ativoEntity);

                    /*
                    DepreciacaoEntity depreciacaoEntity = new DepreciacaoEntity();
                    bool Existe = false;

                    depreciacaoEntity.CD_ATIVO_FIXO = ativo.CD_ATIVO_FIXO;

                    //Verifica se existe um registro na Depreciação

                    DataTableReader dataTableReader = new DepreciacaoData().ObterLista(depreciacaoEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                            Existe = true;
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    depreciacaoEntity.NR_MESES = ativo.depreciacao.NR_MESES;
                    if (!string.IsNullOrEmpty(ativo.depreciacao.DT_INICIO_DEPREC))
                        depreciacaoEntity.DT_INICIO_DEPREC = Convert.ToDateTime(ativo.depreciacao.DT_INICIO_DEPREC);
                    depreciacaoEntity.VL_CUSTO_ATIVO = Convert.ToDecimal(ativo.depreciacao.VL_CUSTO_ATIVO);
                    depreciacaoEntity.VL_DEPREC_TOTAL = Convert.ToDecimal(ativo.depreciacao.VL_DEPREC_TOTAL);
                    depreciacaoEntity.VL_DEPREC_ULT_MES = Convert.ToDecimal(ativo.depreciacao.VL_DEPREC_ULT_MES);
                    depreciacaoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    if (Existe == true)
                        new DepreciacaoData().Alterar(depreciacaoEntity);
                    else
                        new DepreciacaoData().Inserir(ref depreciacaoEntity);
                    */

                    ativo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }

                ativo.modelos = ObterListaModeloCombo();
                ativo.statusAtivos = ObterListaStatusAtivo();
                ativo.situacoesAtivos = ObterListaSituacaoAtivo();
                ativo.linhasProdutos = ObterListaLinhaProduto();
                ativo.CancelarVerificarCodigo = false;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(ativo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Ativo ativo = null;
            try
            {
                AtivoFixoEntity ativoEntity = new AtivoFixoEntity();

                ativoEntity.CD_ATIVO_FIXO = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new AtivoFixoData().ObterLista(ativoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        ativo = new Models.Ativo
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_ATIVO_FIXO"].ToString()),
                            CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                            modelo = new ModeloEntity
                            {
                                CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                                DS_MODELO = dataTableReader["DS_MODELO"].ToString()
                            },
                            TX_TIPO = dataTableReader["TX_TIPO"].ToString(),
                            depreciacao = new Models.Depreciacao
                            {
                                NR_MESES = Convert.ToInt16("0" + dataTableReader["NR_MESES"])
                            }
                        };
                        try
                        {
                            if (dataTableReader["TX_ANO_MÁQUINA"] != DBNull.Value)
                                ativo.TX_ANO_MAQUINA = Convert.ToInt32("0" + dataTableReader["TX_ANO_MÁQUINA"]);
                        }
                        catch { };

                        if (dataTableReader["CD_STATUS_ATIVO"] != DBNull.Value)
                            ativo.statusAtivo.CD_STATUS_ATIVO = Convert.ToInt64("0" + dataTableReader["CD_STATUS_ATIVO"]);

                        ativo.statusAtivo.DS_STATUS_ATIVO = dataTableReader["DS_STATUS_ATIVO"].ToString();

                        if (dataTableReader["CD_SITUACAO_ATIVO"] != DBNull.Value)
                            ativo.situacaoAtivo.CD_SITUACAO_ATIVO = Convert.ToInt64("0" + dataTableReader["CD_SITUACAO_ATIVO"]);

                        ativo.situacaoAtivo.DS_SITUACAO_ATIVO = dataTableReader["DS_SITUACAO_ATIVO"].ToString();

                        if (dataTableReader["CD_LINHA_PRODUTO"] != DBNull.Value)
                            ativo.linhaProduto.CD_LINHA_PRODUTO = Convert.ToInt32("0" + dataTableReader["CD_LINHA_PRODUTO"]);

                        ativo.linhaProduto.DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString();

                        if (dataTableReader["VL_CUSTO_ATIVO"] != DBNull.Value)
                            ativo.depreciacao.VL_CUSTO_ATIVO = Convert.ToDecimal(dataTableReader["VL_CUSTO_ATIVO"]).ToString("N2");

                        if (dataTableReader["VL_RESIDUAL"] != DBNull.Value)
                            ativo.depreciacao.VL_DEPREC_TOTAL = Convert.ToDecimal(dataTableReader["VL_RESIDUAL"]).ToString("N2");

                        if (dataTableReader["VL_DEPREC_ULT_MES"] != DBNull.Value)
                            ativo.depreciacao.VL_DEPREC_ULT_MES = Convert.ToDecimal(dataTableReader["VL_DEPREC_ULT_MES"]).ToString("N2");

                        if (dataTableReader["DT_INCLUSÃO"] != DBNull.Value)
                            ativo.DT_INCLUSAO = Convert.ToDateTime(dataTableReader["DT_INCLUSÃO"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DT_FIM_GARANTIA"] != DBNull.Value)
                            ativo.DT_FIM_GARANTIA = Convert.ToDateTime(dataTableReader["DT_FIM_GARANTIA"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DT_MANUTENCAO"] != DBNull.Value)
                            ativo.DT_MANUTENCAO = Convert.ToDateTime(dataTableReader["DT_MANUTENCAO"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DT_FIM_MANUTENCAO"] != DBNull.Value)
                            ativo.DT_FIM_MANUTENCAO = Convert.ToDateTime(dataTableReader["DT_FIM_MANUTENCAO"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DS_MOTIVO"] != DBNull.Value)
                            ativo.DS_MOTIVO = dataTableReader["DS_MOTIVO"].ToString();

                        if (dataTableReader["DT_INVENTARIO"] != DBNull.Value)
                            ativo.DT_INVENTARIO = Convert.ToDateTime(dataTableReader["DT_INVENTARIO"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DT_INICIO_DEPREC"] != DBNull.Value)
                            ativo.depreciacao.DT_INICIO_DEPREC = Convert.ToDateTime(dataTableReader["DT_INICIO_DEPREC"]).ToString("dd/MM/yyyy");
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

            if (ativo == null)
                return HttpNotFound();
            else
                return View(ativo);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Ativo ativo = new Models.Ativo();
            try
            {
                if (ModelState.IsValid)
                {
                    AtivoFixoEntity ativoEntity = new AtivoFixoEntity();

                    ativoEntity.CD_ATIVO_FIXO = ControlesUtility.Criptografia.Descriptografar(idKey);
                    ativoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new AtivoFixoData().Excluir(ativoEntity);

                    //DepreciacaoEntity depreciacaoEntity = new DepreciacaoEntity();

                    //depreciacaoEntity.CD_ATIVO_FIXO = ControlesUtility.Criptografia.Descriptografar(idKey);
                    //depreciacaoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    //new DepreciacaoData().Excluir(depreciacaoEntity);

                    ativo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(ativo);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        public ActionResult VerificarCodigo(string CD_ATIVO_FIXO, bool CancelarVerificarCodigo)
        {
            bool Liberado = true;

            try
            {

                if (CancelarVerificarCodigo == false)
                {
                    AtivoFixoEntity ativoEntity = new AtivoFixoEntity();

                    ativoEntity.CD_ATIVO_FIXO = CD_ATIVO_FIXO;
                    DataTableReader dataTableReader = new AtivoFixoData().ObterLista(ativoEntity).CreateDataReader();

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

        public JsonResult VerificarCodigoJson(string CD_ATIVO_FIXO)
        {
            bool Redirecionar = false;
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                AtivoFixoEntity ativoEntity = new AtivoFixoEntity();

                ativoEntity.CD_ATIVO_FIXO = CD_ATIVO_FIXO;
                DataTableReader dataTableReader = new AtivoFixoData().ObterLista(ativoEntity).CreateDataReader();

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
                    jsonResult.Add("idKey", Utility.ControlesUtility.Criptografia.Criptografar(ativoEntity.CD_ATIVO_FIXO));
                }
                else
                    jsonResult.Add("Status", "Permanecer");
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

        public JsonResult AlterarData(string Data, int QtdDias)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            jsonResult.Add("DataFinal", (Convert.ToDateTime(Data).AddDays(QtdDias)).ToString("dd/MM/yyyy"));

            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }

        public JsonResult AlterarDias(string dataInclusao, string dataFim, string dataManutencao, string dataFimManutencao)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            TimeSpan date;
            int DiasG = 0;
            int DiasM = 0;

            if (dataFim != null && dataInclusao != null && dataFim != "" && dataInclusao != "")
            {            
                date = Convert.ToDateTime(dataFim) - Convert.ToDateTime(dataInclusao);
                DiasG = date.Days;
            }

            if (dataManutencao != null && dataFimManutencao != null && dataManutencao != "" && dataFimManutencao != "")
            {
                date = Convert.ToDateTime(dataFimManutencao) - Convert.ToDateTime(dataManutencao);
                DiasM = date.Days;
            }

            jsonResult.Add("DiasG", DiasG);
            jsonResult.Add("DiasM", DiasM);

            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }

        public JsonResult ObterListaAtivoJson(string CD_ATIVO_FIXO, long CD_CLIENTE)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.Ativo> ativos = new List<Models.Ativo>();

                AtivoFixoEntity ativoEntity = new AtivoFixoEntity();
                ativoEntity.CD_ATIVO_FIXO = CD_ATIVO_FIXO;
                ativoEntity.cliente.CD_CLIENTE = CD_CLIENTE;
                DataTableReader dataTableReader = new AtivoFixoData().ObterLista(ativoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Ativo ativo = new Models.Ativo
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_ATIVO_FIXO"].ToString()),
                            CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                            modelo = new ModeloEntity
                            {
                                CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                                DS_MODELO = dataTableReader["DS_MODELO"].ToString()
                            },
                            TX_TIPO = dataTableReader["TX_TIPO"].ToString(),
                            depreciacao = new Models.Depreciacao
                            {
                                NR_MESES = Convert.ToInt16("0" + dataTableReader["NR_MESES"])
                            },
                            NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString()
                        };
                        try
                        {
                            if (dataTableReader["TX_ANO_MÁQUINA"] != DBNull.Value)
                                ativo.TX_ANO_MAQUINA = Convert.ToInt32("0" + dataTableReader["TX_ANO_MÁQUINA"]);
                        }
                        catch { };

                        if (dataTableReader["CD_STATUS_ATIVO"] != DBNull.Value)
                            ativo.statusAtivo.CD_STATUS_ATIVO = Convert.ToInt64("0" + dataTableReader["CD_STATUS_ATIVO"]);

                        ativo.statusAtivo.DS_STATUS_ATIVO = dataTableReader["DS_STATUS_ATIVO"].ToString();

                        if (dataTableReader["CD_SITUACAO_ATIVO"] != DBNull.Value)
                            ativo.situacaoAtivo.CD_SITUACAO_ATIVO = Convert.ToInt64("0" + dataTableReader["CD_SITUACAO_ATIVO"]);

                        ativo.situacaoAtivo.DS_SITUACAO_ATIVO = dataTableReader["DS_SITUACAO_ATIVO"].ToString();

                        if (dataTableReader["CD_LINHA_PRODUTO"] != DBNull.Value)
                            ativo.linhaProduto.CD_LINHA_PRODUTO = Convert.ToInt32("0" + dataTableReader["CD_LINHA_PRODUTO"]);

                        ativo.linhaProduto.DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString();

                        if (dataTableReader["VL_CUSTO_ATIVO"] != DBNull.Value)
                            ativo.depreciacao.VL_CUSTO_ATIVO = Convert.ToDecimal(dataTableReader["VL_CUSTO_ATIVO"]).ToString("N2");

                        if (dataTableReader["VL_RESIDUAL"] != DBNull.Value)
                            ativo.depreciacao.VL_DEPREC_TOTAL = Convert.ToDecimal(dataTableReader["VL_RESIDUAL"]).ToString("N2");

                        if (dataTableReader["VL_DEPREC_ULT_MES"] != DBNull.Value)
                            ativo.depreciacao.VL_DEPREC_ULT_MES = Convert.ToDecimal(dataTableReader["VL_DEPREC_ULT_MES"]).ToString("N2");

                        if (dataTableReader["DT_INCLUSÃO"] != DBNull.Value)
                            ativo.DT_INCLUSAO = Convert.ToDateTime(dataTableReader["DT_INCLUSÃO"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DT_FIM_GARANTIA"] != DBNull.Value)
                            ativo.DT_FIM_GARANTIA = Convert.ToDateTime(dataTableReader["DT_FIM_GARANTIA"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DT_MANUTENCAO"] != DBNull.Value)
                            ativo.DT_MANUTENCAO = Convert.ToDateTime(dataTableReader["DT_MANUTENCAO"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DT_FIM_MANUTENCAO"] != DBNull.Value)
                            ativo.DT_FIM_MANUTENCAO = Convert.ToDateTime(dataTableReader["DT_FIM_MANUTENCAO"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DS_MOTIVO"] != DBNull.Value)
                            ativo.DS_MOTIVO = dataTableReader["DS_MOTIVO"].ToString();

                        if (dataTableReader["DT_INVENTARIO"] != DBNull.Value)
                            ativo.DT_INVENTARIO = Convert.ToDateTime(dataTableReader["DT_INVENTARIO"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["DT_INICIO_DEPREC"] != DBNull.Value)
                            ativo.depreciacao.DT_INICIO_DEPREC = Convert.ToDateTime(dataTableReader["DT_INICIO_DEPREC"]).ToString("dd/MM/yyyy");

                        if (dataTableReader["FL_STATUS"] != DBNull.Value)
                            ativo.cdsFL_STATUS = (Convert.ToBoolean(dataTableReader["FL_STATUS"]) == true ? "Ativo" : "Inativo");

                        if (dataTableReader["DT_DEVOLUCAO"] != DBNull.Value)
                        {
                            ativo.DT_DEVOLUCAO = Convert.ToDateTime(dataTableReader["DT_DEVOLUCAO"]).ToString("dd/MM/yyyy");
                            ativo.DT_DEVOLUCAO_GRID = Convert.ToDateTime(dataTableReader["DT_DEVOLUCAO"]);
                        }

                        ativos.Add(ativo);
                    }
                }

                //List<Models.ListaHistoricoVisitas> listaHistoricoVisitas = ObterListaHistoricoVisitas(CD_CLIENTE, CD_TECNICO, DT_INICIO, DT_FIM);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Ativo/_gridMVCAtivo.cshtml", ativos));
                jsonResult.Add("Status", "Success");
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


        //public JsonResult AdcionarData(string Data, int QtdDias)
        //{
        //    bool Redirecionar = false;
        //    Dictionary<string, object> jsonResult = new Dictionary<string, object>();


        //    string DataFinal = string.Empty;
        //    try
        //    {



        //        DataFinal = Convert.ToDateTime(Data).AddDays(QtdDias).ToString("dd/MM/yyyy");
        //        jsonResult.Add("DataFinal", DataFinal);
        //        jsonResult.Add("Status", "Success");
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogarErro(ex);
        //        throw ex;
        //    }

        //    //return Json(jsonResult, JsonRequestBehavior.AllowGet);
        //    var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
        //    jsonList.MaxJsonLength = int.MaxValue;
        //    return jsonList;

        //}



    }
}