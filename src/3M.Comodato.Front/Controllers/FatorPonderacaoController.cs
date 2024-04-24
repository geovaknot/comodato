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
    public class FatorPonderacaoController : BaseController
    {
        // GET: FatorPonderacao
        public ActionResult Index()
        {
            List<Models.TB_PONDERACAO_pz> fator_ponderacoes = new List<Models.TB_PONDERACAO_pz>();

            try
            {
                TB_PONDERACAO_pzEntity tB_PONDERACAO_pzEntity = new TB_PONDERACAO_pzEntity();
                DataTableReader dataTableReader = new FatorPonderacaoData().ObterLista().CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.TB_PONDERACAO_pz FatorPonderacao = new Models.TB_PONDERACAO_pz
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["Id"].ToString()),
                            ID = Convert.ToInt32(dataTableReader["ID"]),
                            //IIComp = dataTableReader["IIComp"].ToString(),
                            MIN_CLIENTES = Convert.ToInt64(dataTableReader["MIN_CLIENTES"].ToString()),
                            MAX_CLIENTES = Convert.ToInt64(dataTableReader["MAX_CLIENTES"].ToString()),
                            FATOR = Convert.ToInt32(dataTableReader["FATOR"].ToString()),
                            dtmDataHoraAtualizacao = Convert.ToDateTime(dataTableReader["DataInclusao"].ToString()),
                            nidUsuarioAtualizacao = Convert.ToInt32(dataTableReader["nidUsuario"].ToString()),
                            Ativo = Convert.ToBoolean(dataTableReader["Ativo"].ToString())
                        };
                        fator_ponderacoes.Add(FatorPonderacao);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.TB_PONDERACAO_pz> iFatorPonderacaos = fator_ponderacoes;
            return View(iFatorPonderacaos);
        }
        
        public ActionResult Incluir()
        {
            Models.TB_PONDERACAO_pz empresa = new Models.TB_PONDERACAO_pz();

            int PonderacaoInicial = 0;
            int PonderacaoFinal = 100;

            List<int> ponderacoes = new List<int>();
            
            DataTableReader dataTableReader = new FatorPonderacaoData().ObterPonderacao().CreateDataReader();

            if (dataTableReader.HasRows)
            {
                int ponderacao = 0;
                while (dataTableReader.Read())
                {
                    ponderacao = Convert.ToInt32(dataTableReader["Ponderacao"].ToString());

                    ponderacoes.Add(ponderacao);
                }
            }

            if (ponderacoes?.Count > 0) { 
                PonderacaoInicial = ponderacoes[0];
                PonderacaoFinal = ponderacoes[1];
            }

            ViewBag.PonderacaoInicial = PonderacaoInicial;
            ViewBag.PonderacaoFinal = PonderacaoFinal;

            return View(empresa);
        }

        [HttpPost]
        public ActionResult Incluir(Models.TB_PONDERACAO_pz empresa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool Gravar = true;
                    //verifica se já existe esse registro no banco
                    TB_PONDERACAO_pzEntity tB_PONDERACAO_pzEntity = new TB_PONDERACAO_pzEntity();

                    int PonderacaoInicial = 0;
                    int PonderacaoFinal = 0;

                    List<int> ponderacoes = new List<int>();

                    DataTableReader dataTableReader = new FatorPonderacaoData().ObterPonderacao().CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        int ponderacao = 0;
                        while (dataTableReader.Read())
                        {
                            ponderacao = Convert.ToInt32(dataTableReader["Ponderacao"].ToString());

                            ponderacoes.Add(ponderacao);
                        }
                    }

                    if (ponderacoes?.Count > 0)
                    {
                        PonderacaoInicial = ponderacoes[0];
                        PonderacaoFinal = ponderacoes[1];
                    }

                    List<TB_PONDERACAO_pzEntity> listadePonderacoes = new List<TB_PONDERACAO_pzEntity>();

                    listadePonderacoes = new FatorPonderacaoData().ObterListaEntity().ToList();

                    //var ponderacaoExistente = listadePonderacoes.FirstOrDefault(x => x.Ativo && x.FATOR == empresa.FATOR);

                    //if (ponderacaoExistente != null)
                    //{
                    //    Gravar = false;
                    //    empresa.JavaScriptToRun = "MensagemRangePonderacao()"; 
                    //}

                    bool RangePreenchido = false;

                    foreach(var item in listadePonderacoes)
                    {
                        if((empresa.MIN_CLIENTES >= item.MIN_CLIENTES && empresa.MIN_CLIENTES <= item.MAX_CLIENTES) || 
                           (empresa.MAX_CLIENTES >= item.MIN_CLIENTES && empresa.MAX_CLIENTES <= item.MAX_CLIENTES))
                        {
                            Gravar = false;
                            RangePreenchido = true;
                        }
                    }

                    if(RangePreenchido)
                        empresa.JavaScriptToRun = "MensagemRangePonderacao()";

                    if(empresa.MIN_CLIENTES > empresa.MAX_CLIENTES)
                    {
                        Gravar = false;
                        empresa.JavaScriptToRun = "MensagemValoresPonderacaoIncorretosMinMaiorMax()"; 
                    }                   

                    if (empresa.MIN_CLIENTES <= 0 && empresa.MAX_CLIENTES <= 0)
                    {
                        Gravar = false;
                        empresa.JavaScriptToRun = "MensagemValoresPonderacaoIncorretos()";
                    }
                    else
                    {
                        if (empresa.MIN_CLIENTES <= 0)
                        {
                            Gravar = false;
                            empresa.JavaScriptToRun = "MensagemValoresPonderacaoIncorretosMin()";
                        }
                        else
                        {
                            if (empresa.MAX_CLIENTES <= 0)
                            {
                                Gravar = false;
                                empresa.JavaScriptToRun = "MensagemValoresPonderacaoIncorretosMax()";
                            }
                            else
                            {
                                if (empresa.FATOR < PonderacaoInicial || empresa.FATOR > PonderacaoFinal || PonderacaoFinal == 0)
                                {
                                    Gravar = false;
                                    empresa.JavaScriptToRun = "MensagemValoresFatorPonderacaoIncorretos()";
                                }
                            }
                        }  
                    }

                    if (Gravar == true)
                    {
                        tB_PONDERACAO_pzEntity.MIN_CLIENTES = empresa.MIN_CLIENTES;
                        tB_PONDERACAO_pzEntity.MAX_CLIENTES = empresa.MAX_CLIENTES;
                        tB_PONDERACAO_pzEntity.FATOR = empresa.FATOR;
                        tB_PONDERACAO_pzEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                        new FatorPonderacaoData().Inserir(ref tB_PONDERACAO_pzEntity);

                        empresa.JavaScriptToRun = "MensagemSucesso()";
                        //return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(empresa); 
        }
        
        public ActionResult Editar(string idKey)
        {
            Models.TB_PONDERACAO_pz empresa = null;

            try
            {
                TB_PONDERACAO_pzEntity tB_PONDERACAO_pzEntity = new TB_PONDERACAO_pzEntity();

                tB_PONDERACAO_pzEntity.ID = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new FatorPonderacaoData().ObterPonderacaoPorId(tB_PONDERACAO_pzEntity.ID).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        empresa = new Models.TB_PONDERACAO_pz
                        {
                            ID = Convert.ToInt32("0" + dataTableReader["ID"].ToString()),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["Id"].ToString()),
                            //IIComp = dataTableReader["IIComp"].ToString(),
                            MIN_CLIENTES = Convert.ToInt64("0" + dataTableReader["MIN_CLIENTES"].ToString()),
                            MAX_CLIENTES = Convert.ToInt64("0" + dataTableReader["MAX_CLIENTES"].ToString()),
                            FATOR = Convert.ToInt32("0" + dataTableReader["FATOR"].ToString()),
                            dtmDataHoraAtualizacao = Convert.ToDateTime(dataTableReader["DataInclusao"].ToString()),
                            nidUsuarioAtualizacao = Convert.ToInt32("0" + dataTableReader["nidUsuario"].ToString()),
                            Ativo = Convert.ToBoolean(dataTableReader["Ativo"].ToString())
                        };
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

            if (empresa == null)
                return HttpNotFound();
            else
                return View(empresa);
        }

        [HttpPost]
        public ActionResult Editar(Models.TB_PONDERACAO_pz empresa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TB_PONDERACAO_pzEntity tB_PONDERACAO_pzEntity = new TB_PONDERACAO_pzEntity();

                    bool Gravar = true;

                    int PonderacaoInicial = 0;
                    int PonderacaoFinal = 0;

                    List<int> ponderacoes = new List<int>();

                    DataTableReader dataTableReader = new FatorPonderacaoData().ObterPonderacao().CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        int ponderacao = 0;
                        while (dataTableReader.Read())
                        {
                            ponderacao = Convert.ToInt32(dataTableReader["Ponderacao"].ToString());

                            ponderacoes.Add(ponderacao);
                        }
                    }

                    if (ponderacoes?.Count > 0)
                    {
                        PonderacaoInicial = ponderacoes[0];
                        PonderacaoFinal = ponderacoes[1];
                    }

                    List<TB_PONDERACAO_pzEntity> listadePonderacoes = new List<TB_PONDERACAO_pzEntity>();

                    listadePonderacoes = new FatorPonderacaoData().ObterListaEntity().ToList();

                    //var ponderacaoExistente = listadePonderacoes.FirstOrDefault(x => x.Ativo && x.FATOR == empresa.FATOR && x.ID != empresa.ID);

                    //if (ponderacaoExistente != null)
                    //{
                    //    Gravar = false;
                    //    empresa.JavaScriptToRun = "MensagemRangePonderacao()";
                    //}

                    bool RangePreenchido = false;

                    foreach (var item in listadePonderacoes)
                    {
                        if ((empresa.MIN_CLIENTES >= item.MIN_CLIENTES && empresa.MIN_CLIENTES <= item.MAX_CLIENTES) ||
                           (empresa.MAX_CLIENTES >= item.MIN_CLIENTES && empresa.MAX_CLIENTES <= item.MAX_CLIENTES))
                        {
                            if(item.ID != empresa.ID)
                            {
                                Gravar = false;
                                RangePreenchido = true;
                            }
                            
                        }
                    }

                    if (RangePreenchido)
                        empresa.JavaScriptToRun = "MensagemRangePonderacao()";

                    if (empresa.MIN_CLIENTES > empresa.MAX_CLIENTES)
                    {
                        Gravar = false;
                        empresa.JavaScriptToRun = "MensagemValoresPonderacaoIncorretosMinMaiorMax()";
                    }

                    if (empresa.MIN_CLIENTES <= 0 && empresa.MAX_CLIENTES <= 0)
                    {
                        Gravar = false;
                        empresa.JavaScriptToRun = "MensagemValoresPonderacaoIncorretos()";
                    }
                    else
                    {
                        if (empresa.MIN_CLIENTES <= 0)
                        {
                            Gravar = false;
                            empresa.JavaScriptToRun = "MensagemValoresPonderacaoIncorretosMin()";
                        }
                        else
                        {
                            if (empresa.MAX_CLIENTES <= 0)
                            {
                                Gravar = false;
                                empresa.JavaScriptToRun = "MensagemValoresPonderacaoIncorretosMax()";
                            }
                            else
                            {
                                if (empresa.FATOR < PonderacaoInicial || empresa.FATOR > PonderacaoFinal || PonderacaoFinal == 0)
                                {
                                    Gravar = false;
                                    empresa.JavaScriptToRun = "MensagemValoresFatorPonderacaoIncorretos()";
                                }
                            }
                        }
                    }


                    if (Gravar) { 

                        tB_PONDERACAO_pzEntity.ID = empresa.ID;
                        tB_PONDERACAO_pzEntity.MIN_CLIENTES = empresa.MIN_CLIENTES;
                        tB_PONDERACAO_pzEntity.MAX_CLIENTES = empresa.MAX_CLIENTES;
                        tB_PONDERACAO_pzEntity.FATOR = empresa.FATOR;
                        tB_PONDERACAO_pzEntity.Ativo = empresa.Ativo;

                        new FatorPonderacaoData().Alterar(tB_PONDERACAO_pzEntity);

                        empresa.JavaScriptToRun = "MensagemSucesso()";
                            //return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(empresa); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        public ActionResult Excluir(string idKey)
        {
            Models.TB_PONDERACAO_pz empresa = null;

            try
            {
                TB_PONDERACAO_pzEntity tB_PONDERACAO_pzEntity = new TB_PONDERACAO_pzEntity();

                tB_PONDERACAO_pzEntity.ID = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new FatorPonderacaoData().ObterPonderacaoPorId(tB_PONDERACAO_pzEntity.ID).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        empresa = new Models.TB_PONDERACAO_pz
                        {
                            ID = Convert.ToInt32("0" + dataTableReader["ID"].ToString()),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["Id"].ToString()),
                            //IIComp = dataTableReader["IIComp"].ToString(),
                            MIN_CLIENTES = Convert.ToInt64("0" + dataTableReader["MIN_CLIENTES"].ToString()),
                            MAX_CLIENTES = Convert.ToInt64("0" + dataTableReader["MAX_CLIENTES"].ToString()),
                            FATOR = Convert.ToInt32("0" + dataTableReader["FATOR"].ToString()),
                            dtmDataHoraAtualizacao = Convert.ToDateTime(dataTableReader["DataInclusao"].ToString()),
                            nidUsuarioAtualizacao = Convert.ToInt32("0" + dataTableReader["nidUsuario"].ToString()),
                            Ativo = Convert.ToBoolean(dataTableReader["Ativo"].ToString())
                        };
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

            if (empresa == null)
                return HttpNotFound();
            else
                return View(empresa);
        }

        [HttpPost, ActionName("Excluir")]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.TB_PONDERACAO_pz empresa = new Models.TB_PONDERACAO_pz();
            try
            {
                if (ModelState.IsValid)
                {
                    TB_PONDERACAO_pzEntity empresaEntity = new TB_PONDERACAO_pzEntity();

                    empresaEntity.ID = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey));

                    new FatorPonderacaoData().Excluir(empresaEntity);

                    empresa.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(empresa);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

    }
}