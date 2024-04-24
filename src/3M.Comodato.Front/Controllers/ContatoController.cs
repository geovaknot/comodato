using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using _3M.Comodato.Front.Models;

namespace _3M.Comodato.Front.Controllers
{
    public class ContatoController : BaseController
    {
        // GET: Contato
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Contato> contatos = new List<Models.Contato>();

            try
            {
                ContatoEntity tecnicoEntity = new ContatoEntity();
                DataTableReader dataTableReader = new ContatoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Contato contato = new Models.Contato
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["nidContato"].ToString()),
                            cnmContato = dataTableReader["cnmContato"].ToString(),
                            cnmApelido = dataTableReader["cnmApelido"].ToString(),
                            tipoContato = new TipoContatoEntity
                            {
                                nidTipoContato = Convert.ToInt64("0" + dataTableReader["nidTipoContato"].ToString()),
                                cdsTipoContato = dataTableReader["cdsTipoContato"].ToString()
                            },
                            cdsEmail = dataTableReader["cdsEmail"].ToString(),
                            empresa = new EmpresaEntity
                            {
                                CD_Empresa = Convert.ToInt64("0" + dataTableReader["nidEmpresa"].ToString()),
                                NM_Empresa = dataTableReader["NM_EMPRESA"].ToString()
                            },
                            cliente = new ClienteEntity
                            {
                                CD_CLIENTE = Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"].ToString()),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString()
                            },
                            bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]),
                            cdsAtivo = (Convert.ToBoolean(dataTableReader["bidAtivo"]) == true ? "Ativo" : "Inativo"),
                            cdsDDDTelefoneCel = dataTableReader["cdsDDDTelefoneCel"].ToString(),
                            cdsTelefoneCel = dataTableReader["cdsTelefoneCel"].ToString(),
                            cdsDDDTelefone2 = dataTableReader["cdsDDDTelefone2"].ToString(),
                            cdsTelefone2 = dataTableReader["cdsTelefone2"].ToString(),
                            cdsObservacoes = dataTableReader["cdsObservacoes"].ToString(),
                            cdsCargo = dataTableReader["cdsCargo"].ToString()
                        };
                        contatos.Add(contato);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Contato> iContatos = contatos;
            return View(iContatos);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Contato contato = new Models.Contato
            {
                tiposContatos = ObterListaTipoContato(),
                empresas = ObterListaEmpresa(),
                clientes = ObterListaCliente()
            };

            //contato.JavaScriptToRun = string.Empty;
            return View(contato);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Contato contato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ContatoEntity contatoEntity = new ContatoEntity();

                    contatoEntity.cnmContato = contato.cnmContato;
                    contatoEntity.cnmApelido = contato.cnmApelido;
                    contatoEntity.tipoContato.nidTipoContato = contato.tipoContato.nidTipoContato;
                    contatoEntity.cdsEmail = contato.cdsEmail;                    
                    //contatoEntity.bidAtivo = contato.bidAtivo;
                    contatoEntity.cdsDDDTelefoneCel = contato.cdsDDDTelefoneCel;
                    contatoEntity.cdsTelefoneCel = contato.cdsTelefoneCel;
                    contatoEntity.cdsDDDTelefone2 = contato.cdsDDDTelefone2;
                    contatoEntity.cdsTelefone2 = contato.cdsTelefone2;
                    contatoEntity.cdsObservacoes = contato.cdsObservacoes;
                    contatoEntity.cdsCargo = contato.cdsCargo;                    
                    contatoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    foreach (TipoContato tc in ObterListaTipoContato())
                    {
                        if (tc.cdsTipoContato.Trim() == "Cliente")
                        {
                            if (tc.nidTipoContato == contato.tipoContato.nidTipoContato)
                            {
                                contatoEntity.cliente.CD_CLIENTE = contato.cliente.CD_CLIENTE;
                                contatoEntity.empresa.CD_Empresa = 0;
                                break;
                            }
                            else
                            {
                                contatoEntity.empresa.CD_Empresa = contato.empresa.CD_Empresa;
                                contatoEntity.cliente.CD_CLIENTE = 0;
                                break;
                            }
                        }
                    }

                    new ContatoData().Inserir(ref contatoEntity);

                    contato.JavaScriptToRun = "MensagemSucesso()";
                    contato.tiposContatos = ObterListaTipoContato();
                    contato.empresas = ObterListaEmpresa();
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            contato.tiposContatos = ObterListaTipoContato();
            contato.empresas = ObterListaEmpresa();
            contato.clientes = ObterListaCliente();
            return View(contato); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Contato contato = null;

            try
            {
                ContatoEntity contatoEntity = new ContatoEntity();

                contatoEntity.nidContato = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new ContatoData().ObterLista(contatoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        contato = new Models.Contato
                        {
                            nidContato = Convert.ToInt64("0" + dataTableReader["nidContato"].ToString()),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["nidContato"].ToString()),
                            cnmContato = dataTableReader["cnmContato"].ToString(),
                            cnmApelido = dataTableReader["cnmApelido"].ToString(),
                            tipoContato = new TipoContatoEntity
                            {
                                nidTipoContato = Convert.ToInt64("0" + dataTableReader["nidTipoContato"].ToString()),
                                cdsTipoContato = dataTableReader["cdsTipoContato"].ToString()
                            },
                            cdsEmail = dataTableReader["cdsEmail"].ToString(),
                            empresa = new EmpresaEntity
                            {
                                CD_Empresa = Convert.ToInt64("0" + dataTableReader["nidEmpresa"].ToString()),
                                NM_Empresa = dataTableReader["NM_EMPRESA"].ToString()
                            },
                            cliente = new ClienteEntity
                            {
                                CD_CLIENTE = Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"].ToString()),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString()
                            },
                            bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]),
                            cdsAtivo = (Convert.ToBoolean(dataTableReader["bidAtivo"]) == true ? "Sim" : "Não"),
                            cdsDDDTelefoneCel = dataTableReader["cdsDDDTelefoneCel"].ToString(),
                            cdsTelefoneCel = dataTableReader["cdsTelefoneCel"].ToString(),
                            cdsDDDTelefone2 = dataTableReader["cdsDDDTelefone2"].ToString(),
                            cdsTelefone2 = dataTableReader["cdsTelefone2"].ToString(),
                            cdsObservacoes = dataTableReader["cdsObservacoes"].ToString(),
                            cdsCargo = dataTableReader["cdsCargo"].ToString(),

                            tiposContatos = ObterListaTipoContato(),
                            empresas = ObterListaEmpresa(),
                            clientes = ObterListaCliente()
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

            if (contato == null)
                return HttpNotFound();
            else
                return View(contato);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Contato contato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ContatoEntity contatoEntity = new ContatoEntity();

                    contatoEntity.nidContato = contato.nidContato;
                    contatoEntity.cnmContato = contato.cnmContato;
                    contatoEntity.cnmApelido = contato.cnmApelido;
                    contatoEntity.tipoContato.nidTipoContato = contato.tipoContato.nidTipoContato;
                    contatoEntity.cdsEmail = contato.cdsEmail;
                    contatoEntity.bidAtivo = contato.bidAtivo;
                    contatoEntity.cdsDDDTelefoneCel = contato.cdsDDDTelefoneCel;
                    contatoEntity.cdsTelefoneCel = contato.cdsTelefoneCel;
                    contatoEntity.cdsDDDTelefone2 = contato.cdsDDDTelefone2;
                    contatoEntity.cdsTelefone2 = contato.cdsTelefone2;
                    contatoEntity.cdsObservacoes = contato.cdsObservacoes;
                    contatoEntity.cdsCargo = contato.cdsCargo;
                    contatoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    foreach (TipoContato tc in ObterListaTipoContato())
                    {
                        if (tc.cdsTipoContato.Trim() == "Cliente")
                        {
                            if (tc.nidTipoContato == contato.tipoContato.nidTipoContato)
                            {
                                contatoEntity.cliente.CD_CLIENTE = contato.cliente.CD_CLIENTE;
                                contatoEntity.empresa.CD_Empresa = 0;
                                break;
                            }
                            else
                            {
                                contatoEntity.empresa.CD_Empresa = contato.empresa.CD_Empresa;
                                contatoEntity.cliente.CD_CLIENTE = 0;
                                break;
                            }
                        }
                    }

                    new ContatoData().Alterar(contatoEntity);

                    contato.JavaScriptToRun = "MensagemSucesso()";
                    contato.tiposContatos = ObterListaTipoContato();
                    contato.empresas = ObterListaEmpresa();
                    contato.clientes = ObterListaCliente();
                    //return RedirectToAction("Index");
                }

                //contato.tiposContatos = ObterListaTipoContato();
                //contato.empresas = ObterListaEmpresa();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            contato.tiposContatos = ObterListaTipoContato();
            contato.empresas = ObterListaEmpresa();
            contato.clientes = ObterListaCliente();
            return View(contato); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Contato contato = null;

            try
            {
                ContatoEntity contatoEntity = new ContatoEntity();

                contatoEntity.nidContato = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new ContatoData().ObterLista(contatoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        contato = new Models.Contato
                        {
                            nidContato = Convert.ToInt64("0" + dataTableReader["nidContato"].ToString()),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["nidContato"].ToString()),
                            cnmContato = dataTableReader["cnmContato"].ToString(),
                            cnmApelido = dataTableReader["cnmApelido"].ToString(),
                            tipoContato = new TipoContatoEntity
                            {
                                nidTipoContato = Convert.ToInt64("0" + dataTableReader["nidTipoContato"].ToString()),
                                cdsTipoContato = dataTableReader["cdsTipoContato"].ToString()
                            },
                            cdsEmail = dataTableReader["cdsEmail"].ToString(),
                            empresa = new EmpresaEntity
                            {
                                CD_Empresa = Convert.ToInt64("0" + dataTableReader["nidEmpresa"].ToString()),
                                NM_Empresa = dataTableReader["NM_EMPRESA"].ToString()
                            },
                            cliente = new ClienteEntity
                            {
                                CD_CLIENTE = Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"].ToString()),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString()
                            },
                            bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]),
                            cdsAtivo = (Convert.ToBoolean(dataTableReader["bidAtivo"]) == true ? "Sim" : "Não"),
                            cdsDDDTelefoneCel = dataTableReader["cdsDDDTelefoneCel"].ToString(),
                            cdsTelefoneCel = dataTableReader["cdsTelefoneCel"].ToString(),
                            cdsDDDTelefone2 = dataTableReader["cdsDDDTelefone2"].ToString(),
                            cdsTelefone2 = dataTableReader["cdsTelefone2"].ToString(),
                            cdsObservacoes = dataTableReader["cdsObservacoes"].ToString(),
                            cdsCargo = dataTableReader["cdsCargo"].ToString(),

                            tiposContatos = ObterListaTipoContato(),
                            empresas = ObterListaEmpresa(),
                            clientes = ObterListaCliente()
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

            if (contato == null)
                return HttpNotFound();
            else
                return View(contato);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Contato contato = new Models.Contato();
            try
            {
                if (ModelState.IsValid)
                {
                    ContatoEntity contatoEntity = new ContatoEntity();

                    contatoEntity.nidContato = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    contatoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new ContatoData().Excluir(contatoEntity);

                    
                    contato.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(contato); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }
    }
}