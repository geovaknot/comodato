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
    public class RazaoComodatoController : BaseController
    {
        // GET: Funcao
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.RazaoComodato> razaoComodatos = new List<Models.RazaoComodato>();

            try
            {
                RazaoComodatoEntity razaoComodatoEntity = new RazaoComodatoEntity();
                DataTableReader dataTableReader = new RazaoComodatoData().ObterLista(razaoComodatoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.RazaoComodato RazaoComodato = new Models.RazaoComodato
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_RAZAO"].ToString()),
                            CD_RAZAO_COMODATO = Convert.ToInt64(dataTableReader["CD_RAZAO"].ToString()),
                            DS_RAZAO_COMODATO = dataTableReader["DS_RAZAO"].ToString(),

                        };
                        razaoComodatos.Add(RazaoComodato);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.RazaoComodato> iRazaoComodatos = razaoComodatos;
            return View(iRazaoComodatos);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.RazaoComodato razaoComodato = new Models.RazaoComodato();
            return View(razaoComodato);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.RazaoComodato razaoComodato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RazaoComodatoEntity razaoComodatoEntity = new RazaoComodatoEntity();

                    // razaoComodatoEntity.CD_RAZAO_COMODATO = razaoComodato.CD_RAZAO_COMODATO;
                    razaoComodatoEntity.DS_RAZAO_COMODATO = razaoComodato.DS_RAZAO_COMODATO;
                    razaoComodatoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new RazaoComodatoData().Inserir(ref razaoComodatoEntity);

                    razaoComodato.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(razaoComodato); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.RazaoComodato razaoComodato = null;

            try
            {
                RazaoComodatoEntity razaoComodatoEntity = new RazaoComodatoEntity();

                razaoComodatoEntity.CD_RAZAO_COMODATO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new RazaoComodatoData().ObterLista(razaoComodatoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        razaoComodato = new Models.RazaoComodato
                        {
                            CD_RAZAO_COMODATO = Convert.ToInt64(dataTableReader["CD_RAZAO"].ToString()),
                            DS_RAZAO_COMODATO = dataTableReader["DS_RAZAO"].ToString()
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

            if (razaoComodato == null)
                return HttpNotFound();
            else
                return View(razaoComodato);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.RazaoComodato razaoComodato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RazaoComodatoEntity razaoComodatoEntity = new RazaoComodatoEntity();

                    razaoComodatoEntity.CD_RAZAO_COMODATO = razaoComodato.CD_RAZAO_COMODATO;
                    razaoComodatoEntity.DS_RAZAO_COMODATO = razaoComodato.DS_RAZAO_COMODATO;
                    razaoComodatoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new RazaoComodatoData().Alterar(razaoComodatoEntity);

                    razaoComodato.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(razaoComodato); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.RazaoComodato razaoComodato = null;

            try
            {
                RazaoComodatoEntity razaoComodatoEntity = new RazaoComodatoEntity();

                razaoComodatoEntity.CD_RAZAO_COMODATO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new RazaoComodatoData().ObterLista(razaoComodatoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        razaoComodato = new Models.RazaoComodato
                        {
                            CD_RAZAO_COMODATO = Convert.ToInt64(dataTableReader["CD_RAZAO"].ToString()),
                            DS_RAZAO_COMODATO = dataTableReader["DS_RAZAO"].ToString()
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

            if (razaoComodato == null)
                return HttpNotFound();
            else
                return View(razaoComodato);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.RazaoComodato razao = new Models.RazaoComodato();
            try
            {
                if (ModelState.IsValid)
                {
                    RazaoComodatoEntity razaoComodatoEntity = new RazaoComodatoEntity();

                    razaoComodatoEntity.CD_RAZAO_COMODATO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    razaoComodatoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new RazaoComodatoData().Excluir(razaoComodatoEntity);

                    razao.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(razao); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }
    }
}