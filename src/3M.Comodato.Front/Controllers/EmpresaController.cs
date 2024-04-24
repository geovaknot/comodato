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
    public class EmpresaController : BaseController
    {
        // GET: Empresa
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Empresa> empresas = new List<Models.Empresa>();

            try
            {
                EmpresaEntity empresaEntity = new EmpresaEntity();
                DataTableReader dataTableReader = new EmpresaData().ObterLista(empresaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Empresa Empresa = new Models.Empresa
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_Empresa"].ToString()),
                            CD_Empresa = Convert.ToInt64(dataTableReader["CD_Empresa"]),
                            //IIComp = dataTableReader["IIComp"].ToString(),
                            NM_Empresa = dataTableReader["NM_Empresa"].ToString(),
                            NR_Cnpj = dataTableReader["NR_CNPJ"].ToString(),
                            EN_Endereco = dataTableReader["EN_ENDERECO"].ToString(),
                            EN_Bairro = dataTableReader["EN_BAIRRO"].ToString(),
                            EN_Cidade = dataTableReader["EN_CIDADE"].ToString(),
                            EN_Estado = dataTableReader["EN_ESTADO"].ToString(),
                            EN_Cep = dataTableReader["EN_CEP"].ToString(),
                            TX_Telefone = dataTableReader["TX_TELEFONE"].ToString(),
                            TX_Fax = dataTableReader["TX_FAX"].ToString(),
                            NM_Contato = dataTableReader["NM_CONTATO"].ToString(),
                            TX_Email = dataTableReader["TX_EMAIL"].ToString(),
                            FL_Tipo_Empresa = dataTableReader["FL_TIPO_EMPRESA"].ToString(),
                            cdsFL_Tipo_Empresa = ControlesUtility.Dicionarios.TipoEmpresa().Where(x => x.Value == dataTableReader["FL_TIPO_EMPRESA"].ToString()).ToArray()[0].Key
                        };
                        empresas.Add(Empresa);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Empresa> iEmpresas = empresas;
            return View(iEmpresas);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Empresa empresa = new Models.Empresa();
            empresa.TipoEmpresa = ControlesUtility.Dicionarios.TipoEmpresa();
            empresa.CancelarVerificarCodigo = false;

            return View(empresa);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Empresa empresa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool Gravar = true;
                    //verifica se já existe esse registro no banco
                    EmpresaEntity empresaEntity = new EmpresaEntity();

                    empresaEntity.CD_Empresa = empresa.CD_Empresa;
                    DataTableReader dataTableReader = new EmpresaData().ObterLista(empresaEntity).CreateDataReader();
                    if (dataTableReader.HasRows)//possui registro, então não inclui
                    {
                        if (dataTableReader.Read())
                        {
                            ViewBag.Mensagem = "Código já castrado!";
                            Gravar = false;
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    if (Gravar == true)
                    {
                        //empresaEntity.IIComp = empresa.IIComp;
                        empresaEntity.NM_Empresa = empresa.NM_Empresa;
                        empresaEntity.NR_Cnpj = empresa.NR_Cnpj;
                        empresaEntity.EN_Endereco = empresa.EN_Endereco;
                        empresaEntity.EN_Bairro = empresa.EN_Bairro;
                        empresaEntity.EN_Cidade = empresa.EN_Cidade;
                        empresaEntity.EN_Estado = empresa.EN_Estado;
                        empresaEntity.EN_Cep = empresa.EN_Cep;
                        empresaEntity.TX_Telefone = empresa.TX_Telefone;
                        empresaEntity.TX_Fax = empresa.TX_Fax;
                        empresaEntity.NM_Contato = empresa.NM_Contato;
                        empresaEntity.TX_Email = empresa.TX_Email;
                        empresaEntity.FL_Tipo_Empresa = empresa.FL_Tipo_Empresa;
                        empresaEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                        new EmpresaData().Inserir(ref empresaEntity);

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
            empresa.TipoEmpresa = ControlesUtility.Dicionarios.TipoEmpresa();
            return View(empresa); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Empresa empresa = null;

            try
            {
                EmpresaEntity empresaEntity = new EmpresaEntity();

                empresaEntity.CD_Empresa = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new EmpresaData().ObterLista(empresaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        empresa = new Models.Empresa
                        {
                            CD_Empresa = Convert.ToInt64("0" + dataTableReader["CD_Empresa"].ToString()),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_Empresa"].ToString()),
                            //IIComp = dataTableReader["IIComp"].ToString(),
                            NM_Empresa = dataTableReader["NM_Empresa"].ToString(),
                            NR_Cnpj = dataTableReader["NR_CNPJ"].ToString(),
                            EN_Endereco = dataTableReader["EN_ENDERECO"].ToString(),
                            EN_Bairro = dataTableReader["EN_BAIRRO"].ToString(),
                            EN_Cidade = dataTableReader["EN_CIDADE"].ToString(),
                            EN_Estado = dataTableReader["EN_ESTADO"].ToString(),
                            EN_Cep = dataTableReader["EN_CEP"].ToString(),
                            TX_Telefone = dataTableReader["TX_TELEFONE"].ToString(),
                            TX_Fax = dataTableReader["TX_FAX"].ToString(),
                            NM_Contato = dataTableReader["NM_CONTATO"].ToString(),
                            TX_Email = dataTableReader["TX_EMAIL"].ToString(),
                            FL_Tipo_Empresa = dataTableReader["FL_TIPO_EMPRESA"].ToString(),
                            CancelarVerificarCodigo = true,
                            TipoEmpresa = ControlesUtility.Dicionarios.TipoEmpresa()
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
        [_3MAuthentication]
        public ActionResult Editar(Models.Empresa empresa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EmpresaEntity empresaEntity = new EmpresaEntity();

                    empresaEntity.CD_Empresa = empresa.CD_Empresa;
                    //empresaEntity.IIComp = empresa.IIComp;
                    empresaEntity.NM_Empresa = empresa.NM_Empresa;
                    empresaEntity.NR_Cnpj = empresa.NR_Cnpj;
                    empresaEntity.EN_Endereco = empresa.EN_Endereco;
                    empresaEntity.EN_Bairro = empresa.EN_Bairro;
                    empresaEntity.EN_Cidade = empresa.EN_Cidade;
                    empresaEntity.EN_Estado = empresa.EN_Estado;
                    empresaEntity.EN_Cep = empresa.EN_Cep;
                    empresaEntity.TX_Telefone = empresa.TX_Telefone;
                    empresaEntity.TX_Fax = empresa.TX_Fax;
                    empresaEntity.NM_Contato = empresa.NM_Contato;
                    empresaEntity.TX_Email = empresa.TX_Email;
                    empresaEntity.FL_Tipo_Empresa = empresa.FL_Tipo_Empresa;
                    empresaEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new EmpresaData().Alterar(empresaEntity);

                    empresa.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            empresa.TipoEmpresa = ControlesUtility.Dicionarios.TipoEmpresa();
            return View(empresa); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Empresa empresa = null;

            try
            {
                EmpresaEntity empresaEntity = new EmpresaEntity();

                empresaEntity.CD_Empresa = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new EmpresaData().ObterLista(empresaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        empresa = new Models.Empresa
                        {
                            CD_Empresa = Convert.ToInt64("0" + dataTableReader["CD_Empresa"].ToString()),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_Empresa"].ToString()),
                            //IIComp = dataTableReader["IIComp"].ToString(),
                            NM_Empresa = dataTableReader["NM_Empresa"].ToString(),
                            NR_Cnpj = dataTableReader["NR_CNPJ"].ToString(),
                            EN_Endereco = dataTableReader["EN_ENDERECO"].ToString(),
                            EN_Bairro = dataTableReader["EN_BAIRRO"].ToString(),
                            EN_Cidade = dataTableReader["EN_CIDADE"].ToString(),
                            EN_Estado = dataTableReader["EN_ESTADO"].ToString(),
                            EN_Cep = dataTableReader["EN_CEP"].ToString(),
                            TX_Telefone = dataTableReader["TX_TELEFONE"].ToString(),
                            TX_Fax = dataTableReader["TX_FAX"].ToString(),
                            NM_Contato = dataTableReader["NM_CONTATO"].ToString(),
                            TX_Email = dataTableReader["TX_EMAIL"].ToString(),
                            FL_Tipo_Empresa = dataTableReader["FL_TIPO_EMPRESA"].ToString()
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
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Empresa empresa = new Models.Empresa();
            try
            {
                if (ModelState.IsValid)
                {
                    EmpresaEntity empresaEntity = new EmpresaEntity();

                    empresaEntity.CD_Empresa = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    empresaEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new EmpresaData().Excluir(empresaEntity);

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

        public ActionResult VerificarCodigo(Int64 CD_Empresa, bool CancelarVerificarCodigo)
        {
            bool Liberado = true;

            try
            {
                if (CancelarVerificarCodigo == false)
                {
                    EmpresaEntity empresaEntity = new EmpresaEntity();

                    empresaEntity.CD_Empresa = CD_Empresa;
                    DataTableReader dataTableReader = new EmpresaData().ObterLista(empresaEntity).CreateDataReader();

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