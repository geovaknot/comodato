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
    public class PecaController : BaseController
    {
        // GET: Peca
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Peca> pecas = new List<Models.Peca>();

            try
            {
                PecaEntity pecaEntity = new PecaEntity();
                DataTableReader dataTableReader = new PecaData().ObterListaNew(pecaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Peca peca = new Models.Peca
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_PECA"].ToString()),
                            CD_PECA = dataTableReader["CD_PECA"].ToString(),
                            DS_PECA = dataTableReader["DS_PECA"].ToString(),
                            TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                            QTD_ESTOQUE = Convert.ToDecimal("0" + dataTableReader["QTD_ESTOQUE"]).ToString("N3"),
                            QTD_ESTOQUE_GRID = Convert.ToDecimal("0" + dataTableReader["QTD_ESTOQUE"].ToString()),
                            QTD_MINIMA = Convert.ToDecimal(dataTableReader["QTD_MINIMA"]).ToString("N3"),
                            VL_PECA = Convert.ToDecimal(dataTableReader["VL_PECA"]).ToString("N2"),
                            TP_PECA = dataTableReader["TP_PECA"].ToString(),
                            FL_ATIVO_PECA = dataTableReader["FL_ATIVO_PECA"].ToString(),
                            cdsFL_ATIVO_PECA = (dataTableReader["FL_ATIVO_PECA"].ToString() == "S" ? "Ativo" : "Inativo"),
                            QTD_PlanoZero = Convert.ToDecimal(dataTableReader["QTD_PlanoZero"]).ToString("N3")
                        };
                        if (peca.TP_PECA == "E")
                            peca.cdsTP_PECA = "Especial";
                        if (peca.TP_PECA == "N")
                            peca.cdsTP_PECA = "Normal";
                        //if (peca.TP_PECA == "R")
                        //    peca.cdsTP_PECA = "Recuperada";
                        if (peca.TP_PECA == "A")
                            peca.cdsTP_PECA = "Automatica";

                        pecas.Add(peca);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Peca> iPecas = pecas;
            return View(iPecas);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Peca peca = new Models.Peca
            {
                tiposPecas = ControlesUtility.Dicionarios.TipoPeca(),
                SimNao = ControlesUtility.Dicionarios.SimNao(),
                CancelarVerificarCodigo = false
            };

            return View(peca);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Peca peca)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PecaEntity pecaEntity = new PecaEntity();

                    pecaEntity.CD_PECA = peca.CD_PECA;
                    if (peca.TP_PECA == "" || peca.TP_PECA == null)
                        peca.TP_PECA = "A";
                    pecaEntity.TP_PECA = peca.TP_PECA;
                    pecaEntity.DS_PECA = peca.DS_PECA;
                    pecaEntity.TX_UNIDADE = peca.TX_UNIDADE;
                    pecaEntity.VL_PECA = Convert.ToDecimal(peca.VL_PECA);
                    //pecaEntity.FL_ATIVO_PECA = "S";
                    pecaEntity.QTD_MINIMA = Convert.ToDecimal(peca.QTD_MINIMA);
                    pecaEntity.QTD_ESTOQUE = Convert.ToDecimal(peca.QTD_ESTOQUE);
                    pecaEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    pecaEntity.QTD_PlanoZero = Convert.ToDecimal(peca.QTD_PlanoZero);
                    
                    PecaEntity pecaEspecial = new PecaEntity();
                    pecaEspecial.TP_PECA = "E";
                    pecaEspecial.CD_PECA = peca.CD_PECA_RECUPERADA;
                    pecaEspecial.FL_ATIVO_PECA = "S";

                    pecaEntity.CD_PECA_RECUPERADA = peca.CD_PECA_RECUPERADA;

                    if (peca.CD_PECA_RECUPERADA != null && peca.CD_PECA_RECUPERADA != "")
                    {
                        List<Models.Peca> pecas = new List<Models.Peca>();
                        
                        List<Models.Peca> pecas2 = new List<Models.Peca>();

                        DataTableReader dataTableReader2 = new PecaData().ObterListaNew(pecaEspecial).CreateDataReader();

                        if (dataTableReader2.HasRows)
                        {
                            if (dataTableReader2.Read())
                            {
                                peca = new Models.Peca
                                {
                                    CD_PECA = dataTableReader2["CD_PECA"].ToString(),
                                    DS_PECA = dataTableReader2["DS_PECA"].ToString(),
                                    TX_UNIDADE = dataTableReader2["TX_UNIDADE"].ToString(),
                                    QTD_ESTOQUE = Convert.ToDecimal("0" + dataTableReader2["QTD_ESTOQUE"]).ToString("N3"),
                                    QTD_ESTOQUE_GRID = Convert.ToDecimal("0" + dataTableReader2["QTD_ESTOQUE"].ToString()),
                                    QTD_MINIMA = Convert.ToDecimal("0" + dataTableReader2["QTD_MINIMA"]).ToString("N3"),
                                    VL_PECA = Convert.ToDecimal("0" + dataTableReader2["VL_PECA"]).ToString("N2"),
                                    TP_PECA = dataTableReader2["TP_PECA"].ToString(),
                                    cdsTP_PECA = ControlesUtility.Dicionarios.TipoPeca().Where(x => x.Value == dataTableReader2["TP_PECA"].ToString()).ToArray()[0].Key,
                                    FL_ATIVO_PECA = dataTableReader2["FL_ATIVO_PECA"].ToString(),

                                    tiposPecas = ControlesUtility.Dicionarios.TipoPeca(),
                                    SimNao = ControlesUtility.Dicionarios.SimNao(),
                                    CancelarVerificarCodigo = true,
                                    CD_PECA_RECUPERADA = dataTableReader2["CD_PECA_RECUPERADA"].ToString(),
                                    QTD_PlanoZero = Convert.ToDecimal("0" + dataTableReader2["QTD_PlanoZero"]).ToString("N3")
                                };
                                pecas2.Add(peca);
                            }
                        }

                        if (dataTableReader2 != null)
                        {
                            dataTableReader2.Dispose();
                            dataTableReader2 = null;
                        }

                        pecas2 = pecas2.Where(x => x.CD_PECA_RECUPERADA.ToUpper() == peca.CD_PECA_RECUPERADA.ToUpper()).ToList();

                        if (pecas2?.Count == 0)
                        {
                            pecaEspecial.DS_PECA = pecaEntity.DS_PECA;
                            pecaEspecial.VL_PECA = pecaEntity.VL_PECA;
                            pecaEspecial.TX_UNIDADE = pecaEntity.TX_UNIDADE;
                            //pecaEntity.FL_ATIVO_PECA = "S";
                            pecaEspecial.QTD_MINIMA = pecaEntity.QTD_MINIMA;
                            pecaEspecial.QTD_ESTOQUE = pecaEntity.QTD_ESTOQUE;
                            pecaEspecial.nidUsuarioAtualizacao = pecaEntity.nidUsuarioAtualizacao;
                            pecaEspecial.QTD_PlanoZero = pecaEntity.QTD_PlanoZero;
                            new PecaData().Inserir(ref pecaEspecial);

                            new PecaData().grava_PecaRecuperada(pecaEntity);

                            peca.JavaScriptToRun = "MensagemSucesso()";
                        }
                        else
                        {
                            new PecaData().grava_PecaRecuperada(pecaEntity);

                            new PecaData().Inserir(ref pecaEntity);

                            peca.JavaScriptToRun = "MensagemSucesso()";
                        }
                    }
                    else
                    {
                        new PecaData().Inserir(ref pecaEntity);

                        peca.JavaScriptToRun = "MensagemSucesso()";


                    }
                    
                        
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            peca.tiposPecas = ControlesUtility.Dicionarios.TipoPeca();
            peca.SimNao = ControlesUtility.Dicionarios.SimNao();
            peca.CancelarVerificarCodigo = false;

            return View(peca); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Peca peca = null;

            try
            {
                PecaEntity pecaEntity = new PecaEntity();

                pecaEntity.CD_PECA = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new PecaData().ObterListaNew(pecaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        peca = new Models.Peca
                        {
                            CD_PECA = dataTableReader["CD_PECA"].ToString(),
                            DS_PECA = dataTableReader["DS_PECA"].ToString(),
                            TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                            QTD_ESTOQUE = Convert.ToDecimal("0" + dataTableReader["QTD_ESTOQUE"]).ToString("N3"),
                            QTD_ESTOQUE_GRID = Convert.ToDecimal("0" + dataTableReader["QTD_ESTOQUE"].ToString()),
                            QTD_MINIMA = Convert.ToDecimal("0" + dataTableReader["QTD_MINIMA"]).ToString("N3"),
                            VL_PECA = Convert.ToDecimal("0" + dataTableReader["VL_PECA"]).ToString("N2"),
                            TP_PECA = dataTableReader["TP_PECA"].ToString(),
                            cdsTP_PECA = ControlesUtility.Dicionarios.TipoPeca().Where(x => x.Value == dataTableReader["TP_PECA"].ToString()).ToArray()[0].Key,
                            FL_ATIVO_PECA = dataTableReader["FL_ATIVO_PECA"].ToString(),

                            tiposPecas = ControlesUtility.Dicionarios.TipoPeca(),
                            SimNao = ControlesUtility.Dicionarios.SimNao(),
                            CancelarVerificarCodigo = true,
                            CD_PECA_RECUPERADA = dataTableReader["CD_PECA_RECUPERADA"].ToString(),
                            QTD_PlanoZero = Convert.ToDecimal("0" + dataTableReader["QTD_PlanoZero"]).ToString("N3")
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

            if (peca == null)
                return HttpNotFound();
            else
                return View(peca);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Peca peca)
        {
            try
            {
                if (peca.TP_PECA == "E")
                {
                    if (peca.VL_PECA == null)
                        peca.VL_PECA = "0";
                    if (peca.QTD_MINIMA == null)
                        peca.QTD_MINIMA = "0";
                    if (peca.QTD_ESTOQUE == null)
                        peca.QTD_ESTOQUE = "0";
                }
                Models.Peca peca2 = null;
                Models.Peca peca3 = null;
                Models.Peca pecaInativa = null;
                Models.Peca pecaInativa1 = null;
                if (ModelState.IsValid)
                {
                    PecaEntity pecaEntity = new PecaEntity();

                    pecaEntity.CD_PECA = peca.CD_PECA;
                    pecaEntity.TP_PECA = peca.TP_PECA;
                    pecaEntity.DS_PECA = peca.DS_PECA;
                    pecaEntity.TX_UNIDADE = peca.TX_UNIDADE;
                    pecaEntity.VL_PECA = Convert.ToDecimal(peca.VL_PECA);
                    pecaEntity.FL_ATIVO_PECA = peca.FL_ATIVO_PECA;
                    pecaEntity.QTD_MINIMA = Convert.ToDecimal(peca.QTD_MINIMA);
                    pecaEntity.QTD_ESTOQUE = Convert.ToDecimal(peca.QTD_ESTOQUE);
                    pecaEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    pecaEntity.QTD_PlanoZero = Convert.ToDecimal(peca.QTD_PlanoZero);

                    PecaEntity pecaRecuperada = new PecaEntity();
                    pecaRecuperada.TP_PECA = "E";
                    pecaRecuperada.CD_PECA = peca.CD_PECA_RECUPERADA;
                    pecaRecuperada.FL_ATIVO_PECA = "S";
                    pecaEntity.CD_PECA_RECUPERADA = peca.CD_PECA_RECUPERADA;
                    if (peca.CD_PECA_RECUPERADA != null && peca.CD_PECA_RECUPERADA != "")
                    {
                        List<Models.Peca> pecas = new List<Models.Peca>();

                        DataTableReader dataTableReader = new PecaData().ObterListaNew(pecaRecuperada).CreateDataReader();

                        if (dataTableReader.HasRows)
                        {
                            if (dataTableReader.Read())
                            {
                                peca2 = new Models.Peca
                                {
                                    CD_PECA = dataTableReader["CD_PECA"].ToString(),
                                    DS_PECA = dataTableReader["DS_PECA"].ToString(),
                                    TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                                    QTD_ESTOQUE = Convert.ToDecimal("0" + dataTableReader["QTD_ESTOQUE"]).ToString("N3"),
                                    QTD_ESTOQUE_GRID = Convert.ToDecimal("0" + dataTableReader["QTD_ESTOQUE"].ToString()),
                                    QTD_MINIMA = Convert.ToDecimal("0" + dataTableReader["QTD_MINIMA"]).ToString("N3"),
                                    VL_PECA = Convert.ToDecimal("0" + dataTableReader["VL_PECA"]).ToString("N2"),
                                    TP_PECA = dataTableReader["TP_PECA"].ToString(),
                                    cdsTP_PECA = ControlesUtility.Dicionarios.TipoPeca().Where(x => x.Value == dataTableReader["TP_PECA"].ToString()).ToArray()[0].Key,
                                    FL_ATIVO_PECA = dataTableReader["FL_ATIVO_PECA"].ToString(),

                                    tiposPecas = ControlesUtility.Dicionarios.TipoPeca(),
                                    SimNao = ControlesUtility.Dicionarios.SimNao(),
                                    CancelarVerificarCodigo = true,
                                    CD_PECA_RECUPERADA = dataTableReader["CD_PECA_RECUPERADA"].ToString(),
                                    QTD_PlanoZero = Convert.ToDecimal("0" + dataTableReader["QTD_PlanoZero"]).ToString("N3")
                                };
                                pecas.Add(peca2);
                            }
                        }

                        if (dataTableReader != null)
                        {
                            dataTableReader.Dispose();
                            dataTableReader = null;
                        }

                        pecas = pecas.Where(x => x.CD_PECA.ToUpper() == peca.CD_PECA_RECUPERADA.ToUpper()).ToList();

                        if (pecas?.Count == 0)
                        {
                            pecaRecuperada.DS_PECA = pecaEntity.DS_PECA;
                            pecaRecuperada.VL_PECA = pecaEntity.VL_PECA;
                            pecaRecuperada.TX_UNIDADE = pecaEntity.TX_UNIDADE;
                            //pecaEntity.FL_ATIVO_PECA = "S";
                            pecaRecuperada.QTD_MINIMA = pecaEntity.QTD_MINIMA;
                            pecaRecuperada.QTD_ESTOQUE = pecaEntity.QTD_ESTOQUE;
                            pecaRecuperada.nidUsuarioAtualizacao = pecaEntity.nidUsuarioAtualizacao;
                            pecaRecuperada.QTD_PlanoZero = Convert.ToDecimal(peca.QTD_PlanoZero);

                            List<Models.Peca> pecasInativas = new List<Models.Peca>();

                            DataTableReader dataTableReaderInativo1 = new PecaData().ObterListaNewInativa(pecaRecuperada).CreateDataReader();

                            if (dataTableReaderInativo1.HasRows)
                            {
                                if (dataTableReaderInativo1.Read())
                                {
                                    pecaInativa1 = new Models.Peca
                                    {
                                        CD_PECA = dataTableReaderInativo1["CD_PECA"].ToString(),
                                        DS_PECA = dataTableReaderInativo1["DS_PECA"].ToString(),
                                        TX_UNIDADE = dataTableReaderInativo1["TX_UNIDADE"].ToString(),
                                        QTD_ESTOQUE = Convert.ToDecimal("0" + dataTableReaderInativo1["QTD_ESTOQUE"]).ToString("N3"),
                                        QTD_ESTOQUE_GRID = Convert.ToDecimal("0" + dataTableReaderInativo1["QTD_ESTOQUE"].ToString()),
                                        QTD_MINIMA = Convert.ToDecimal("0" + dataTableReaderInativo1["QTD_MINIMA"]).ToString("N3"),
                                        VL_PECA = Convert.ToDecimal("0" + dataTableReaderInativo1["VL_PECA"]).ToString("N2"),
                                        TP_PECA = dataTableReaderInativo1["TP_PECA"].ToString(),
                                        cdsTP_PECA = ControlesUtility.Dicionarios.TipoPeca().Where(x => x.Value == dataTableReaderInativo1["TP_PECA"].ToString()).ToArray()[0].Key,
                                        FL_ATIVO_PECA = dataTableReaderInativo1["FL_ATIVO_PECA"].ToString(),

                                        tiposPecas = ControlesUtility.Dicionarios.TipoPeca(),
                                        SimNao = ControlesUtility.Dicionarios.SimNao(),
                                        CancelarVerificarCodigo = true,
                                        CD_PECA_RECUPERADA = dataTableReaderInativo1["CD_PECA_RECUPERADA"].ToString(),
                                        QTD_PlanoZero = Convert.ToDecimal("0" + dataTableReaderInativo1["QTD_PlanoZero"]).ToString("N3")
                                    };
                                    pecasInativas.Add(pecaInativa1);
                                }
                            }

                            if (dataTableReaderInativo1 != null)
                            {
                                dataTableReaderInativo1.Dispose();
                                dataTableReaderInativo1 = null;
                            }

                            if (pecasInativas.Count == 0)
                            {
                                new PecaData().Inserir(ref pecaRecuperada);

                                new PecaData().grava_PecaRecuperada(pecaRecuperada);

                                peca.JavaScriptToRun = "MensagemSucesso()";
                            }
                            else
                                peca.JavaScriptToRun = "MensagemPecaInativa()";
                        }
                        else
                        {
                            new PecaData().grava_PecaRecuperada(pecaEntity);

                            new PecaData().Alterar(pecaEntity);

                            peca.JavaScriptToRun = "MensagemSucesso()";
                        }
                    }
                    else
                    {
                        new PecaData().RemoverRecuperada(pecaEntity.CD_PECA);

                        new PecaData().Alterar(pecaEntity);

                        peca.JavaScriptToRun = "MensagemSucesso()";
                    }
                    
                    
                    //return RedirectToAction("Index");
                }
                else
                {
                    if (peca.TP_PECA == "A")
                    {
                        PecaEntity pecaEntity = new PecaEntity();

                        pecaEntity.CD_PECA = peca.CD_PECA;
                        
                        Models.Peca pecass = new Models.Peca();

                        DataTableReader dataTableReader = new PecaData().ObterListaNew(pecaEntity).CreateDataReader();

                        if (dataTableReader.HasRows)
                        {
                            if (dataTableReader.Read())
                            {
                                pecass = new Models.Peca
                                {
                                    CD_PECA = dataTableReader["CD_PECA"].ToString(),
                                    DS_PECA = dataTableReader["DS_PECA"].ToString(),
                                    TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                                    QTD_ESTOQUE = Convert.ToDecimal("0" + dataTableReader["QTD_ESTOQUE"]).ToString("N3"),
                                    QTD_ESTOQUE_GRID = Convert.ToDecimal("0" + dataTableReader["QTD_ESTOQUE"].ToString()),
                                    QTD_MINIMA = Convert.ToDecimal("0" + dataTableReader["QTD_MINIMA"]).ToString("N3"),
                                    VL_PECA = Convert.ToDecimal("0" + dataTableReader["VL_PECA"]).ToString("N2"),
                                    TP_PECA = dataTableReader["TP_PECA"].ToString(),
                                    cdsTP_PECA = ControlesUtility.Dicionarios.TipoPeca().Where(x => x.Value == dataTableReader["TP_PECA"].ToString()).ToArray()[0].Key,
                                    FL_ATIVO_PECA = dataTableReader["FL_ATIVO_PECA"].ToString(),

                                    tiposPecas = ControlesUtility.Dicionarios.TipoPeca(),
                                    SimNao = ControlesUtility.Dicionarios.SimNao(),
                                    CancelarVerificarCodigo = true,
                                    CD_PECA_RECUPERADA = dataTableReader["CD_PECA_RECUPERADA"].ToString(),
                                    QTD_PlanoZero = Convert.ToDecimal("0" + dataTableReader["QTD_PlanoZero"]).ToString("N3")
                                };
                                
                            }
                        }

                        if (dataTableReader != null)
                        {
                            dataTableReader.Dispose();
                            dataTableReader = null;
                        }

                        pecaEntity.TP_PECA = pecass.TP_PECA;
                        pecaEntity.DS_PECA = pecass.DS_PECA;
                        pecaEntity.TX_UNIDADE = pecass.TX_UNIDADE;
                        pecaEntity.VL_PECA = Convert.ToDecimal(pecass.VL_PECA);
                        pecaEntity.FL_ATIVO_PECA = peca.FL_ATIVO_PECA;
                        pecaEntity.QTD_MINIMA = Convert.ToDecimal(peca.QTD_MINIMA);
                        pecaEntity.QTD_ESTOQUE = Convert.ToDecimal(peca.QTD_ESTOQUE);
                        pecaEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                        pecaEntity.QTD_PlanoZero = Convert.ToDecimal(peca.QTD_PlanoZero);
                        peca.DS_PECA = pecass.DS_PECA;
                        peca.VL_PECA = Convert.ToString(pecass.VL_PECA);
                        peca.TX_UNIDADE = pecass.TX_UNIDADE;
                        peca.cdsTP_PECA = pecass.cdsTP_PECA;
                        
                        PecaEntity pecaEspecial = new PecaEntity();
                        pecaEspecial.TP_PECA = "E";
                        pecaEspecial.FL_ATIVO_PECA = "S";
                        pecaEspecial.CD_PECA = peca.CD_PECA_RECUPERADA;

                        pecaEntity.CD_PECA_RECUPERADA = peca.CD_PECA_RECUPERADA;
                        if (peca.CD_PECA_RECUPERADA != null && peca.CD_PECA_RECUPERADA != "")
                        {
                            
                            List<Models.Peca> pecas2 = new List<Models.Peca>();

                            DataTableReader dataTableReader3 = new PecaData().ObterListaNew(pecaEspecial).CreateDataReader();

                            if (dataTableReader3.HasRows)
                            {
                                if (dataTableReader3.Read())
                                {
                                    peca3 = new Models.Peca
                                    {
                                        CD_PECA = dataTableReader3["CD_PECA"].ToString(),
                                        DS_PECA = dataTableReader3["DS_PECA"].ToString(),
                                        TX_UNIDADE = dataTableReader3["TX_UNIDADE"].ToString(),
                                        QTD_ESTOQUE = Convert.ToDecimal("0" + dataTableReader3["QTD_ESTOQUE"]).ToString("N3"),
                                        QTD_ESTOQUE_GRID = Convert.ToDecimal("0" + dataTableReader3["QTD_ESTOQUE"].ToString()),
                                        QTD_MINIMA = Convert.ToDecimal("0" + dataTableReader3["QTD_MINIMA"]).ToString("N3"),
                                        VL_PECA = Convert.ToDecimal("0" + dataTableReader3["VL_PECA"]).ToString("N2"),
                                        TP_PECA = dataTableReader3["TP_PECA"].ToString(),
                                        cdsTP_PECA = ControlesUtility.Dicionarios.TipoPeca().Where(x => x.Value == dataTableReader3["TP_PECA"].ToString()).ToArray()[0].Key,
                                        FL_ATIVO_PECA = dataTableReader3["FL_ATIVO_PECA"].ToString(),

                                        tiposPecas = ControlesUtility.Dicionarios.TipoPeca(),
                                        SimNao = ControlesUtility.Dicionarios.SimNao(),
                                        CancelarVerificarCodigo = true,
                                        CD_PECA_RECUPERADA = dataTableReader3["CD_PECA_RECUPERADA"].ToString(),
                                        QTD_PlanoZero = Convert.ToDecimal("0" + dataTableReader3["QTD_PlanoZero"]).ToString("N3")
                                    };
                                    pecas2.Add(peca3);
                                }
                            }

                            if (dataTableReader3 != null)
                            {
                                dataTableReader3.Dispose();
                                dataTableReader3 = null;
                            }

                            //pecas = pecas.Where(x => x.CD_PECA_RECUPERADA.ToUpper() == peca.CD_PECA_RECUPERADA.ToUpper()).ToList();

                            if (pecas2?.Count == 0)
                            {
                                pecaEspecial.DS_PECA = pecaEntity.DS_PECA;
                                pecaEspecial.VL_PECA = pecaEntity.VL_PECA;
                                pecaEspecial.TX_UNIDADE = pecaEntity.TX_UNIDADE;
                                //pecaEntity.FL_ATIVO_PECA = "S";
                                pecaEspecial.QTD_MINIMA = pecaEntity.QTD_MINIMA;
                                pecaEspecial.QTD_ESTOQUE = pecaEntity.QTD_ESTOQUE;
                                pecaEspecial.nidUsuarioAtualizacao = pecaEntity.nidUsuarioAtualizacao;
                                pecaEspecial.QTD_PlanoZero = pecaEntity.QTD_PlanoZero;

                                List<Models.Peca> pecasInativas = new List<Models.Peca>();

                                DataTableReader dataTableReaderInativo = new PecaData().ObterListaNewInativa(pecaEspecial).CreateDataReader();

                                if (dataTableReaderInativo.HasRows)
                                {
                                    if (dataTableReaderInativo.Read())
                                    {
                                        pecaInativa = new Models.Peca
                                        {
                                            CD_PECA = dataTableReaderInativo["CD_PECA"].ToString(),
                                            DS_PECA = dataTableReaderInativo["DS_PECA"].ToString(),
                                            TX_UNIDADE = dataTableReaderInativo["TX_UNIDADE"].ToString(),
                                            QTD_ESTOQUE = Convert.ToDecimal("0" + dataTableReaderInativo["QTD_ESTOQUE"]).ToString("N3"),
                                            QTD_ESTOQUE_GRID = Convert.ToDecimal("0" + dataTableReaderInativo["QTD_ESTOQUE"].ToString()),
                                            QTD_MINIMA = Convert.ToDecimal("0" + dataTableReaderInativo["QTD_MINIMA"]).ToString("N3"),
                                            VL_PECA = Convert.ToDecimal("0" + dataTableReaderInativo["VL_PECA"]).ToString("N2"),
                                            TP_PECA = dataTableReaderInativo["TP_PECA"].ToString(),
                                            cdsTP_PECA = ControlesUtility.Dicionarios.TipoPeca().Where(x => x.Value == dataTableReaderInativo["TP_PECA"].ToString()).ToArray()[0].Key,
                                            FL_ATIVO_PECA = dataTableReaderInativo["FL_ATIVO_PECA"].ToString(),

                                            tiposPecas = ControlesUtility.Dicionarios.TipoPeca(),
                                            SimNao = ControlesUtility.Dicionarios.SimNao(),
                                            CancelarVerificarCodigo = true,
                                            CD_PECA_RECUPERADA = dataTableReaderInativo["CD_PECA_RECUPERADA"].ToString(),
                                            QTD_PlanoZero = Convert.ToDecimal("0" + dataTableReaderInativo["QTD_PlanoZero"]).ToString("N3")
                                        };
                                        pecasInativas.Add(pecaInativa);
                                    }
                                }

                                if (dataTableReaderInativo != null)
                                {
                                    dataTableReaderInativo.Dispose();
                                    dataTableReaderInativo = null;
                                }

                                if (pecasInativas.Count == 0)
                                {
                                    new PecaData().Inserir(ref pecaEspecial);

                                    new PecaData().grava_PecaRecuperada(pecaEntity);

                                    peca.JavaScriptToRun = "MensagemSucesso()";
                                }
                                else
                                    peca.JavaScriptToRun = "MensagemPecaInativa()";

                            }
                            else
                            {
                                new PecaData().grava_PecaRecuperada(pecaEntity);

                                new PecaData().Alterar(pecaEntity);

                                peca.JavaScriptToRun = "MensagemSucesso()";
                            }
                        }
                        else
                        {
                            new PecaData().RemoverRecuperada(pecaEntity.CD_PECA);

                            new PecaData().Alterar(pecaEntity);

                            peca.JavaScriptToRun = "MensagemSucesso()";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            peca.tiposPecas = ControlesUtility.Dicionarios.TipoPeca();
            peca.SimNao = ControlesUtility.Dicionarios.SimNao();
            peca.CancelarVerificarCodigo = true;

            return View(peca); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Peca peca = null;

            try
            {
                PecaEntity pecaEntity = new PecaEntity();

                pecaEntity.CD_PECA = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new PecaData().ObterListaNew(pecaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        peca = new Models.Peca
                        {
                            CD_PECA = dataTableReader["CD_PECA"].ToString(),
                            DS_PECA = dataTableReader["DS_PECA"].ToString(),
                            TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                            QTD_ESTOQUE = Convert.ToDecimal("0" + dataTableReader["QTD_ESTOQUE"]).ToString("N3"),
                            QTD_ESTOQUE_GRID = Convert.ToDecimal("0" + dataTableReader["QTD_ESTOQUE"].ToString()),
                            QTD_MINIMA = Convert.ToDecimal("0" + dataTableReader["QTD_MINIMA"]).ToString("N3"),
                            VL_PECA = Convert.ToDecimal("0" + dataTableReader["VL_PECA"]).ToString("N2"),
                            TP_PECA = dataTableReader["TP_PECA"].ToString(),
                            cdsTP_PECA = ControlesUtility.Dicionarios.TipoPeca().Where(x => x.Value == dataTableReader["TP_PECA"].ToString()).ToArray()[0].Key,
                            FL_ATIVO_PECA = dataTableReader["FL_ATIVO_PECA"].ToString(),
                            cdsFL_ATIVO_PECA = ControlesUtility.Dicionarios.SimNao().Where(x => x.Value == dataTableReader["FL_ATIVO_PECA"].ToString()).ToArray()[0].Key
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

            if (peca == null)
                return HttpNotFound();
            else
                return View(peca);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Peca peca = new Models.Peca();
            try
            {
                if (ModelState.IsValid)
                {
                    PecaEntity pecaEntity = new PecaEntity();

                    pecaEntity.CD_PECA = ControlesUtility.Criptografia.Descriptografar(idKey);
                    pecaEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new PecaData().Excluir(pecaEntity);

                    peca.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(peca);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        public ActionResult VerificarCodigo(string CD_PECA, bool CancelarVerificarCodigo)
        {
            bool Liberado = true;

            try
            {
                if (CancelarVerificarCodigo == false)
                {
                    PecaEntity pecaEntity = new PecaEntity();

                    pecaEntity.CD_PECA = CD_PECA;
                    DataTableReader dataTableReader = new PecaData().ObterListaNew(pecaEntity).CreateDataReader();

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

        public JsonResult VerificarCodigoJson(string CD_PECA)
        {
            bool Redirecionar = false;
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                PecaEntity pecaEntity = new PecaEntity();

                pecaEntity.CD_PECA = CD_PECA;
                DataTableReader dataTableReader = new PecaData().ObterListaNew(pecaEntity).CreateDataReader();

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
                    jsonResult.Add("idKey", Utility.ControlesUtility.Criptografia.Criptografar(pecaEntity.CD_PECA));
                }
                else
                {
                    // Busca na BPCS para importação
                    Models.Peca peca = new Models.Peca();

                    dataTableReader = new PecaData().ObterListaBPCS(pecaEntity).CreateDataReader();
                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            peca.CD_PECA = CD_PECA; //dataTableReader["COD_PRD_PRD"].ToString();
                            peca.DS_PECA = dataTableReader["DSC_PRD_PRD"].ToString();
                            peca.TX_UNIDADE = dataTableReader["COD_UNIT_MEASURE"].ToString();
                            peca.VL_PECA = Convert.ToDecimal("0" + dataTableReader["FUCOS_VAL_CUSTO_UNITARIO_TOT"]).ToString("N2");
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    peca.TP_PECA = "A";
                    peca.FL_ATIVO_PECA = "S";

                    jsonResult.Add("Status", "Permanecer");
                    jsonResult.Add("peca", peca);
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