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
    public class ModeloController : BaseController
    {
        // GET: Funcao
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Modelo> modelos = new List<Models.Modelo>();

            try
            {
                ModeloEntity modeloEntity = new ModeloEntity();
                DataTableReader dataTableReader = new ModeloData().ObterLista(modeloEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Modelo Modelo = new Models.Modelo
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_MODELO"].ToString()),
                            CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                            DS_MODELO = dataTableReader["DS_MODELO"].ToString(),
                            
                            TP_EMPACOTAMENTO = dataTableReader["TP_EMPACOTAMENTO"].ToString(),
                            VL_COMP_MIN = Convert.ToDecimal("0" + dataTableReader["VL_COMP_MIN"]).ToString("N2"),
                            VL_COMP_MAX = Convert.ToDecimal("0" + dataTableReader["VL_COMP_MAX"]).ToString("N2"),
                            VL_LARG_MIN = Convert.ToDecimal("0" + dataTableReader["VL_LARG_MIN"]).ToString("N2"),
                            VL_LARG_MAX = Convert.ToDecimal("0" + dataTableReader["VL_LARG_MAX"]).ToString("N2"),
                            VL_ALTUR_MIN = Convert.ToDecimal("0" + dataTableReader["VL_ALTUR_MIN"]).ToString("N2"),

                            VL_ALTUR_MAX = Convert.ToDecimal("0" + dataTableReader["VL_ALTUR_MAX"]).ToString("N2"),
                            VL_LARG_CAIXA = Convert.ToDecimal("0" + dataTableReader["VL_LARG_CAIXA"]).ToString("N2"),
                            VL_ALTUR_CAIXA = Convert.ToDecimal("0" + dataTableReader["VL_ALTUR_CAIXA"]).ToString("N2"),
                            VL_COMP_CAIXA = Convert.ToDecimal("0" + dataTableReader["VL_COMP_CAIXA"]).ToString("N2"),
                            VL_PESO_CUBADO = Convert.ToDecimal("0" + dataTableReader["VL_PESO_CUBADO"]).ToString("N2"),

                            ID_CATEGORIA = dataTableReader["ID_CATEGORIA"].ToString(),
                            CD_LINHA_PRODUTO = dataTableReader["CD_LINHA_PRODUTO"].ToString(),
                            grupoModelo = new GrupoModeloEntity()
                            {
                                CD_GRUPO_MODELO = dataTableReader["CD_GRUPO_MODELO"].ToString(),
                                DS_GRUPO_MODELO = dataTableReader["ds_grupoModelo"].ToString(),
                            },
                            VL_COMPLEXIDADE_EQUIP = Convert.ToInt64("0" + dataTableReader["VL_COMPLEXIDADE_EQUIP"]),
                            VL_PROJETADO = Convert.ToInt64("0" + dataTableReader["VL_PROJETADO"]),
                            cdsFL_ATIVO = (dataTableReader["FL_ATIVO"].ToString() == "S" ? "Ativo" : "Inativo")
                        };

                        modelos.Add(Modelo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Modelo> iModelos = modelos;
            return View(iModelos);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Modelo modelo = new Models.Modelo
            {
                CancelarVerificarCodigo = false,
                gruposModelos = ObterListaGruposModelos(),
                categorias = ObterListaCategoria(),
                SimNao = ControlesUtility.Dicionarios.SimNao(),
                TipoEmpacotamento = ControlesUtility.Dicionarios.TipoEmpacotamento(),
                produtos = ObterListaLinhaProduto(),
            };

            return View(modelo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Modelo modelo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool Gravar = true;
                    ModeloEntity modeloEntity = new ModeloEntity();

                    if (!string.IsNullOrEmpty(modelo.CD_MODELO))
                    {
                        //Verifica se já existe esse registro no banco
                        modeloEntity.CD_MODELO = modelo.CD_MODELO;
                        DataTableReader dataTableReader = new ModeloData().ObterLista(modeloEntity).CreateDataReader();
                        if (dataTableReader.HasRows)//possui registro, então não inclui
                        {
                            if (dataTableReader.Read())
                            {
                                ViewBag.Mensagem = "Código de modelo já castrado por outro usuário!";
                                Gravar = false;
                            }
                        }

                        if (dataTableReader != null)
                        {
                            dataTableReader.Dispose();
                            dataTableReader = null;
                        }
                    }

                    if (Gravar == true)
                    {
                        modeloEntity.CD_MODELO = modelo.CD_MODELO;
                        modeloEntity.DS_MODELO = modelo.DS_MODELO;
                        modeloEntity.CD_MOD_NR12 = modelo.CD_MOD_NR12;
                        modeloEntity.CD_GRUPO_MODELO = modelo.grupoModelo.CD_GRUPO_MODELO;
                        modeloEntity.VL_COMPLEXIDADE_EQUIP = modelo.VL_COMPLEXIDADE_EQUIP;
                        modeloEntity.VL_PROJETADO = modelo.VL_PROJETADO;
                        modeloEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                        modeloEntity.TP_EMPACOTAMENTO = modelo.TP_EMPACOTAMENTO;
                        modeloEntity.VL_COMP_MIN = Convert.ToDecimal("0" + modelo.VL_COMP_MIN);
                        modeloEntity.VL_COMP_MAX = Convert.ToDecimal("0" + modelo.VL_COMP_MAX);
                        modeloEntity.VL_LARG_MIN = Convert.ToDecimal("0" + modelo.VL_LARG_MIN);
                        modeloEntity.VL_LARG_MAX = Convert.ToDecimal("0" + modelo.VL_LARG_MAX);
                        modeloEntity.VL_ALTUR_MIN = Convert.ToDecimal("0" + modelo.VL_ALTUR_MIN);
                        modeloEntity.VL_ALTUR_MAX = Convert.ToDecimal("0" + modelo.VL_ALTUR_MAX);
                        modeloEntity.VL_LARG_CAIXA = Convert.ToDecimal("0" + modelo.VL_LARG_CAIXA);
                        modeloEntity.VL_ALTUR_CAIXA = Convert.ToDecimal("0" + modelo.VL_ALTUR_CAIXA);
                        modeloEntity.VL_COMP_CAIXA = Convert.ToDecimal("0" + modelo.VL_COMP_CAIXA);
                        modeloEntity.VL_PESO_CUBADO = Convert.ToDecimal("0" + modelo.VL_PESO_CUBADO);
                        modeloEntity.FL_ATIVO = modelo.FL_ATIVO;
                        modeloEntity.CATEGORIA.ID_CATEGORIA = Convert.ToInt64("0" + modelo.ID_CATEGORIA);
                        modeloEntity.LINHA_PRODUTO.CD_LINHA_PRODUTO = Convert.ToInt32("0" + modelo.CD_LINHA_PRODUTO);

                        new ModeloData().Inserir(ref modeloEntity);

                        modelo.JavaScriptToRun = "MensagemSucesso()";
                        //return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            modelo.gruposModelos = ObterListaGruposModelos();
            modelo.categorias = ObterListaCategoria();
            modelo.SimNao = ControlesUtility.Dicionarios.SimNao();
            modelo.TipoEmpacotamento = ControlesUtility.Dicionarios.TipoEmpacotamento();
            modelo.produtos = ObterListaLinhaProduto();

            return View(modelo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Modelo modelo = null;

            try
            {
                ModeloEntity modeloEntity = new ModeloEntity();

                modeloEntity.CD_MODELO = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new ModeloData().ObterLista(modeloEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        modelo = new Models.Modelo
                        {
                            CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                            DS_MODELO = dataTableReader["DS_MODELO"].ToString(),
                            CD_MOD_NR12 = dataTableReader["CD_MOD_NR12"].ToString(),
                            grupoModelo = new GrupoModeloEntity()
                            {
                                CD_GRUPO_MODELO = dataTableReader["CD_GRUPO_MODELO"].ToString(),
                                DS_GRUPO_MODELO = dataTableReader["ds_grupoModelo"].ToString(),
                            },
                            VL_COMPLEXIDADE_EQUIP = Convert.ToInt64("0" + dataTableReader["VL_COMPLEXIDADE_EQUIP"]),
                            VL_PROJETADO = Convert.ToInt64("0" + dataTableReader["VL_PROJETADO"]),
                            SimNao = ControlesUtility.Dicionarios.SimNao(),
                            CancelarVerificarCodigo = true,

                            TP_EMPACOTAMENTO = dataTableReader["TP_EMPACOTAMENTO"].ToString(),
                            VL_COMP_MIN = Convert.ToDecimal("0" + dataTableReader["VL_COMP_MIN"]).ToString("N2"),
                            VL_COMP_MAX = Convert.ToDecimal("0" + dataTableReader["VL_COMP_MAX"]).ToString("N2"),
                            VL_LARG_MIN = Convert.ToDecimal("0" + dataTableReader["VL_LARG_MIN"]).ToString("N2"),
                            VL_LARG_MAX = Convert.ToDecimal("0" + dataTableReader["VL_LARG_MAX"]).ToString("N2"),
                            VL_ALTUR_MIN = Convert.ToDecimal("0" + dataTableReader["VL_ALTUR_MIN"]).ToString("N2"),

                            VL_ALTUR_MAX = Convert.ToDecimal("0" + dataTableReader["VL_ALTUR_MAX"]).ToString("N2"),
                            VL_LARG_CAIXA = Convert.ToDecimal("0" + dataTableReader["VL_LARG_CAIXA"]).ToString("N2"),
                            VL_ALTUR_CAIXA = Convert.ToDecimal("0" + dataTableReader["VL_ALTUR_CAIXA"]).ToString("N2"),
                            VL_COMP_CAIXA = Convert.ToDecimal("0" + dataTableReader["VL_COMP_CAIXA"]).ToString("N2"),
                            VL_PESO_CUBADO = Convert.ToDecimal("0" + dataTableReader["VL_PESO_CUBADO"]).ToString("N2"),

                            ID_CATEGORIA = dataTableReader["ID_CATEGORIA"].ToString(),
                            CD_LINHA_PRODUTO = dataTableReader["CD_LINHA_PRODUTO"].ToString()
                        };
                        if (dataTableReader["FL_ATIVO"] == DBNull.Value)
                            modelo.FL_ATIVO = "N";
                        else
                            modelo.FL_ATIVO = dataTableReader["FL_ATIVO"].ToString();
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                modelo.gruposModelos = ObterListaGruposModelos();
                modelo.categorias = ObterListaCategoria();
                modelo.SimNao = ControlesUtility.Dicionarios.SimNao();
                modelo.TipoEmpacotamento = ControlesUtility.Dicionarios.TipoEmpacotamento();
                modelo.produtos = ObterListaLinhaProduto();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (modelo == null)
                return HttpNotFound();
            else
                return View(modelo);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Modelo modelo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ModeloEntity modeloEntity = new ModeloEntity();

                    modeloEntity.CD_MODELO = modelo.CD_MODELO;
                    modeloEntity.DS_MODELO = modelo.DS_MODELO;
                    modeloEntity.CD_MOD_NR12 = modelo.CD_MOD_NR12;
                    modeloEntity.CD_GRUPO_MODELO = modelo.grupoModelo.CD_GRUPO_MODELO;
                    modeloEntity.FL_ATIVO = modelo.FL_ATIVO;
                    modeloEntity.VL_COMPLEXIDADE_EQUIP = modelo.VL_COMPLEXIDADE_EQUIP;
                    modeloEntity.VL_PROJETADO = modelo.VL_PROJETADO;
                    modeloEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    modeloEntity.TP_EMPACOTAMENTO = modelo.TP_EMPACOTAMENTO;
                    modeloEntity.VL_COMP_MIN = Convert.ToDecimal("0" + modelo.VL_COMP_MIN);
                    modeloEntity.VL_COMP_MAX = Convert.ToDecimal("0" + modelo.VL_COMP_MAX);
                    modeloEntity.VL_LARG_MIN = Convert.ToDecimal("0" + modelo.VL_LARG_MIN);
                    modeloEntity.VL_LARG_MAX = Convert.ToDecimal("0" + modelo.VL_LARG_MAX);
                    modeloEntity.VL_ALTUR_MIN = Convert.ToDecimal("0" + modelo.VL_ALTUR_MIN);
                    modeloEntity.VL_ALTUR_MAX = Convert.ToDecimal("0" + modelo.VL_ALTUR_MAX);
                    modeloEntity.VL_LARG_CAIXA = Convert.ToDecimal("0" + modelo.VL_LARG_CAIXA);
                    modeloEntity.VL_ALTUR_CAIXA = Convert.ToDecimal("0" + modelo.VL_ALTUR_CAIXA);
                    modeloEntity.VL_COMP_CAIXA = Convert.ToDecimal("0" + modelo.VL_COMP_CAIXA);
                    modeloEntity.VL_PESO_CUBADO = Convert.ToDecimal("0" + modelo.VL_PESO_CUBADO);
                    modeloEntity.FL_ATIVO = modelo.FL_ATIVO;
                    modeloEntity.CATEGORIA.ID_CATEGORIA = Convert.ToInt64("0" + modelo.ID_CATEGORIA);
                    modeloEntity.LINHA_PRODUTO.CD_LINHA_PRODUTO = Convert.ToInt32("0" + modelo.CD_LINHA_PRODUTO);

                    modelo.categorias = ObterListaCategoria();
                    modelo.produtos = ObterListaLinhaProduto();

                    modelo.gruposModelos = ObterListaGruposModelos();

                    new ModeloData().Alterar(modeloEntity);

                    modelo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            modelo.SimNao = ControlesUtility.Dicionarios.SimNao();
            modelo.gruposModelos = ObterListaGruposModelos();
            modelo.categorias = ObterListaCategoria();
            modelo.TipoEmpacotamento = ControlesUtility.Dicionarios.TipoEmpacotamento();
            modelo.produtos = ObterListaLinhaProduto();

            return View(modelo); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Modelo modelo = null;

            try
            {
                ModeloEntity modeloEntity = new ModeloEntity();

                modeloEntity.CD_MODELO = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new ModeloData().ObterLista(modeloEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        modelo = new Models.Modelo
                        {
                            CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                            DS_MODELO = dataTableReader["DS_MODELO"].ToString(),
                            grupoModelo = new GrupoModeloEntity()
                            {
                                CD_GRUPO_MODELO = dataTableReader["CD_GRUPO_MODELO"].ToString(),
                                DS_GRUPO_MODELO = dataTableReader["ds_grupoModelo"].ToString(),
                            },
                            VL_COMPLEXIDADE_EQUIP = Convert.ToInt64("0" + dataTableReader["VL_COMPLEXIDADE_EQUIP"]),
                            VL_PROJETADO = Convert.ToInt64("0" + dataTableReader["VL_PROJETO"]),
                        };
                        if (dataTableReader["FL_ATIVO"] == DBNull.Value)
                            modelo.FL_ATIVO = "N";
                        else
                            modelo.FL_ATIVO = dataTableReader["FL_ATIVO"].ToString();
                    }
                }

                modelo.gruposModelos = ObterListaGruposModelos();

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

            if (modelo == null)
                return HttpNotFound();
            else
                return View(modelo);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Modelo modelo = new Models.Modelo();
            try
            {
                if (ModelState.IsValid)
                {
                    ModeloEntity modeloEntity = new ModeloEntity();

                    modeloEntity.CD_MODELO = ControlesUtility.Criptografia.Descriptografar(idKey);
                    modeloEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new ModeloData().Excluir(modeloEntity);

                    modelo.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(modelo);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        public ActionResult VerificarCodigo(string CD_MODELO, bool CancelarVerificarCodigo)
        {
            bool Liberado = true;
            try
            {
                if (CancelarVerificarCodigo == false)
                {
                    ModeloEntity modeloEntity = new ModeloEntity();

                    modeloEntity.CD_MODELO = CD_MODELO;
                    DataTableReader dataTableReader = new ModeloData().ObterLista(modeloEntity).CreateDataReader();

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

    }
}