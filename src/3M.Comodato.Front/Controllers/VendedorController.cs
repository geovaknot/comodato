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
    public class VendedorController : BaseController
    {
        

        // GET: Funcao
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Vendedor> vendedores = new List<Models.Vendedor>();

            try
            {

                VendedorEntity vendedorEntity = new VendedorEntity();
                DataTableReader dataTableReader = new VendedorData().ObterLista(vendedorEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Vendedor Vendedor = new Models.Vendedor
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_VENDEDOR"].ToString()),
                            CD_VENDEDOR = Convert.ToInt64(dataTableReader["CD_VENDEDOR"].ToString()),
                            NM_VENDEDOR = dataTableReader["NM_VENDEDOR"].ToString(),
                            NM_APE_VENDEDOR = dataTableReader["NM_APE_VENDEDOR"].ToString(),
                            EN_ENDERECO = dataTableReader["EN_ENDERECO"].ToString(),
                            EN_BAIRRO = dataTableReader["EN_BAIRRO"].ToString(),
                            EN_CIDADE = dataTableReader["EN_CIDADE"].ToString(),
                            EN_ESTADO = dataTableReader["EN_ESTADO"].ToString(),
                            EN_CEP = dataTableReader["EN_CEP"].ToString(),
                            EN_CX_POSTAL = dataTableReader["EN_CX_POSTAL"].ToString(),
                            TX_TELEFONE = dataTableReader["TX_TELEFONE"].ToString(),
                            TX_FAX = dataTableReader["TX_FAX"].ToString(),
                            TX_EMAIL = dataTableReader["TX_EMAIL"].ToString()
                        };

                        if (dataTableReader["ID_USUARIO_REGIONAL"] != DBNull.Value)
                        {
                            Vendedor.usuarioGerenteRegional = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO_REGIONAL"]),
                                cnmNome = dataTableReader["cnmNomeGerente"].ToString() + " (" + dataTableReader["cdsLoginGerente"].ToString() + ")"
                            };
                        }

                        if (dataTableReader["ID_USUARIO"] != DBNull.Value)
                        {
                            Vendedor.usuario = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO"]),
                                cnmNome = dataTableReader["cnmNome"].ToString() + " (" + dataTableReader["cdsLogin"].ToString() + ")"
                            };
                        }

                        vendedores.Add(Vendedor);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Vendedor> iVendedores = vendedores;
            return View(iVendedores);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Vendedor vendedor = new Models.Vendedor();
            vendedor.JavaScriptToRun = string.Empty;
            vendedor.usuarios = ObterListaUsuario();
            vendedor.usuariosGerentesRegionais = ObterListaGerenteRegional();
            vendedor.SimNao = ControlesUtility.Dicionarios.SimNao();

            return View(vendedor);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Vendedor vendedor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    VendedorEntity vendedorEntity = new VendedorEntity();

                    vendedorEntity.NM_VENDEDOR = vendedor.NM_VENDEDOR;
                    vendedorEntity.NM_APE_VENDEDOR = vendedor.NM_APE_VENDEDOR;
                    vendedorEntity.EN_ENDERECO = vendedor.EN_ENDERECO;
                    vendedorEntity.EN_BAIRRO = vendedor.EN_BAIRRO;
                    vendedorEntity.EN_CIDADE = vendedor.EN_CIDADE;
                    vendedorEntity.EN_ESTADO = vendedor.EN_ESTADO;
                    vendedorEntity.EN_CEP = vendedor.EN_CEP;
                    vendedorEntity.EN_CX_POSTAL = vendedor.EN_CX_POSTAL;
                    vendedorEntity.TX_TELEFONE = vendedor.TX_TELEFONE;
                    vendedorEntity.TX_FAX = vendedor.TX_FAX;
                    vendedorEntity.TX_EMAIL = vendedor.TX_EMAIL;
                    vendedorEntity.ID_USUARIO = vendedor.usuario.nidUsuario;
                    vendedorEntity.ID_USUARIO_REGIONAL = vendedor.usuarioGerenteRegional.nidUsuario;
                    vendedorEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    vendedorEntity.FL_ATIVO = vendedor.FL_ATIVO;

                    new VendedorData().Inserir(ref vendedorEntity);

                    vendedor.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }

                vendedor.SimNao = ControlesUtility.Dicionarios.SimNao();
                vendedor.usuarios = ObterListaUsuario();
                vendedor.usuariosGerentesRegionais = ObterListaGerenteRegional();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(vendedor); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Vendedor vendedor = null;

            try
            {
                VendedorEntity vendedorEntity = new VendedorEntity();

                vendedorEntity.CD_VENDEDOR = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new VendedorData().ObterLista(vendedorEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        vendedor = new Models.Vendedor
                        {
                            CD_VENDEDOR = Convert.ToInt64(dataTableReader["CD_VENDEDOR"].ToString()),
                            NM_VENDEDOR = dataTableReader["NM_VENDEDOR"].ToString(),
                            NM_APE_VENDEDOR = dataTableReader["NM_APE_VENDEDOR"].ToString(),
                            EN_ENDERECO = dataTableReader["EN_ENDERECO"].ToString(),
                            EN_BAIRRO = dataTableReader["EN_BAIRRO"].ToString(),
                            EN_CIDADE = dataTableReader["EN_CIDADE"].ToString(),
                            EN_ESTADO = dataTableReader["EN_ESTADO"].ToString(),
                            EN_CEP = dataTableReader["EN_CEP"].ToString(),
                            EN_CX_POSTAL = dataTableReader["EN_CX_POSTAL"].ToString(),
                            TX_TELEFONE = dataTableReader["TX_TELEFONE"].ToString(),
                            TX_FAX = dataTableReader["TX_FAX"].ToString(),
                            TX_EMAIL = dataTableReader["TX_EMAIL"].ToString(),
                            usuarioGerenteRegional = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO_REGIONAL"]),
                                cnmNome = dataTableReader["cnmNomeGerente"].ToString() + " (" + dataTableReader["cdsLoginGerente"].ToString() + ")"
                            },
                            usuario = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO"]),
                                cnmNome = dataTableReader["cnmNome"].ToString() + " (" + dataTableReader["cdsLogin"].ToString() + ")"
                            },
                            FL_ATIVO = dataTableReader["FL_ATIVO"].ToString(),

                            SimNao = ControlesUtility.Dicionarios.SimNao(),
                            usuarios = ObterListaUsuario(),
                            usuariosGerentesRegionais = ObterListaGerenteRegional()
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

            if (vendedor == null)
                return HttpNotFound();
            else
                return View(vendedor);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Vendedor vendedor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    VendedorEntity vendedorEntity = new VendedorEntity();

                    vendedorEntity.CD_VENDEDOR = vendedor.CD_VENDEDOR;
                    vendedorEntity.NM_VENDEDOR = vendedor.NM_VENDEDOR;
                    vendedorEntity.NM_APE_VENDEDOR = vendedor.NM_APE_VENDEDOR;
                    vendedorEntity.EN_ENDERECO = vendedor.EN_ENDERECO;
                    vendedorEntity.EN_BAIRRO = vendedor.EN_BAIRRO;
                    vendedorEntity.EN_CIDADE = vendedor.EN_CIDADE;
                    vendedorEntity.EN_ESTADO = vendedor.EN_ESTADO;
                    vendedorEntity.EN_CEP = vendedor.EN_CEP;
                    vendedorEntity.EN_CX_POSTAL = vendedor.EN_CX_POSTAL;
                    vendedorEntity.TX_TELEFONE = vendedor.TX_TELEFONE;
                    vendedorEntity.TX_FAX = vendedor.TX_FAX;
                    vendedorEntity.TX_EMAIL = vendedor.TX_EMAIL;
                    vendedorEntity.usuarioGerenteRegional.nidUsuario = vendedor.usuarioGerenteRegional.nidUsuario;
                    vendedorEntity.usuario.nidUsuario = vendedor.usuario.nidUsuario;
                    vendedorEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    vendedorEntity.FL_ATIVO = vendedor.FL_ATIVO;
                    vendedorEntity.SimNao = vendedor.SimNao;

                    new VendedorData().Alterar(vendedorEntity);

                    vendedor.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
                vendedor.SimNao = ControlesUtility.Dicionarios.SimNao();
                vendedor.usuarios = ObterListaUsuario();
                vendedor.usuariosGerentesRegionais = ObterListaGerenteRegional();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(vendedor); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Vendedor vendedor = null;

            try
            {
                VendedorEntity vendedorEntity = new VendedorEntity();

                vendedorEntity.CD_VENDEDOR = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new VendedorData().ObterLista(vendedorEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        vendedor = new Models.Vendedor
                        {
                            CD_VENDEDOR = Convert.ToInt64(dataTableReader["CD_VENDEDOR"].ToString()),
                            NM_VENDEDOR = dataTableReader["NM_VENDEDOR"].ToString(),
                            NM_APE_VENDEDOR = dataTableReader["NM_APE_VENDEDOR"].ToString(),
                            EN_ENDERECO = dataTableReader["EN_ENDERECO"].ToString(),
                            EN_BAIRRO = dataTableReader["EN_BAIRRO"].ToString(),
                            EN_CIDADE = dataTableReader["EN_CIDADE"].ToString(),
                            EN_ESTADO = dataTableReader["EN_ESTADO"].ToString(),
                            EN_CEP = dataTableReader["EN_CEP"].ToString(),
                            EN_CX_POSTAL = dataTableReader["EN_CX_POSTAL"].ToString(),
                            TX_TELEFONE = dataTableReader["TX_TELEFONE"].ToString(),
                            TX_FAX = dataTableReader["TX_FAX"].ToString(),
                            TX_EMAIL = dataTableReader["TX_EMAIL"].ToString(),
                            usuarioGerenteRegional = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO_REGIONAL"]),
                                cnmNome = dataTableReader["cnmNomeGerente"].ToString() + " (" + dataTableReader["cdsLoginGerente"].ToString() + ")"
                            },
                            usuario = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO"]),
                                cnmNome = dataTableReader["cnmNome"].ToString() + " (" + dataTableReader["cdsLogin"].ToString() + ")"
                            }
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

            if (vendedor == null)
                return HttpNotFound();
            else
                return View(vendedor);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Vendedor vendedor = new Models.Vendedor();
            try
            {
                if (ModelState.IsValid)
                {
                    VendedorEntity vendedorEntity = new VendedorEntity();

                    vendedorEntity.CD_VENDEDOR = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    vendedorEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new VendedorData().Excluir(vendedorEntity);
                    
                    vendedor.JavaScriptToRun = "MensagemSucesso()";
                    return View(vendedor);
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(vendedor);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }
    }
}