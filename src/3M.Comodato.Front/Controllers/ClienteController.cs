using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class ClienteController : BaseController
    {
        // GET: Cliente
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Cliente> clientes = new List<Models.Cliente>();

            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();
                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Cliente cliente = new Models.Cliente
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_CLIENTE"].ToString()),
                            CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                            grupo = new GrupoEntity
                            {
                                CD_GRUPO = dataTableReader["CD_GRUPO"].ToString(),
                                DS_GRUPO = dataTableReader["DS_GRUPO"].ToString()
                            },
                            CD_RAC = dataTableReader["CD_RAC"].ToString(),
                            vendedor = new VendedorEntity
                            {
                                CD_VENDEDOR = Convert.ToInt64("0" + dataTableReader["CD_VENDEDOR"]),
                                NM_VENDEDOR = dataTableReader["NM_VENDEDOR"].ToString(),
                                NM_APE_VENDEDOR = dataTableReader["NM_APE_VENDEDOR"].ToString()
                            },
                            NR_CNPJ = dataTableReader["NR_CNPJ"].ToString(),
                            NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + dataTableReader["CD_CLIENTE"].ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString(),
                            EN_ENDERECO = dataTableReader["EN_ENDERECO"].ToString(),
                            EN_BAIRRO = dataTableReader["EN_BAIRRO"].ToString(),
                            EN_CIDADE = dataTableReader["EN_CIDADE"].ToString(),
                            EN_ESTADO = dataTableReader["EN_ESTADO"].ToString(),
                            EN_CEP = dataTableReader["EN_CEP"].ToString(),
                            TX_EMAIL = dataTableReader["TX_EMAIL"].ToString(),
                            regiao = new RegiaoEntity
                            {
                                CD_REGIAO = dataTableReader["CD_REGIAO"].ToString(),
                                DS_REGIAO = dataTableReader["DS_REGIAO"].ToString()
                            },
                            CD_FILIAL = dataTableReader["CD_FILIAL"].ToString(),
                            CD_ABC = dataTableReader["CD_ABC"].ToString(),
                            CL_CLIENTE = dataTableReader["CL_CLIENTE"].ToString(),
                            TX_TELEFONE = dataTableReader["TX_TELEFONE"].ToString(),
                            TX_FAX = dataTableReader["TX_FAX"].ToString(),
                            executivo = new ExecutivoEntity
                            {
                                CD_EXECUTIVO = Convert.ToInt64("0" + dataTableReader["CD_VENDEDOR"]),
                                NM_EXECUTIVO = dataTableReader["NM_EXECUTIVO"].ToString()
                            },
                            QT_PERIODO = Convert.ToInt32("0" + dataTableReader["QT_PERIODO"]),
                            DS_CLASSIFICACAO_KAT = dataTableReader["DS_CLASSIFICACAO_KAT"].ToString(),
                        };
                        if (dataTableReader["DT_DESATIVACAO"] != DBNull.Value)
                        {
                            cliente.DT_DESATIVACAO = Convert.ToDateTime(dataTableReader["DT_DESATIVACAO"], new CultureInfo("pt-BR"));
                        }
                        //else
                        //cliente.DT_DESATIVACAO = "Ativo";

                        //if (dataTableReader["BPCS"] != DBNull.Value)
                        //    cliente.BPCS = Convert.ToBoolean(dataTableReader["BPCS"]);

                        if (dataTableReader["NM_TECNICO"] != DBNull.Value)
                        {
                            cliente.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                        }

                        if (dataTableReader["CD_ORDEM"] != DBNull.Value)
                        {
                            cliente.CD_ORDEM = Convert.ToInt32(dataTableReader["CD_ORDEM"]);
                        }

                        if (dataTableReader["nidUsuario"] != DBNull.Value)
                        {
                            cliente.usuario = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["nidUsuario"])
                            };
                        }

                        if (dataTableReader["FL_KAT_FIXO"] != DBNull.Value)
                        {
                            cliente.FL_KAT_FIXO = Convert.ToBoolean(dataTableReader["FL_KAT_FIXO"]);
                        }
                        clientes.Add(cliente);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Cliente> iClientes = clientes;
            return View(iClientes);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.ClienteBPCS cliente = new Models.ClienteBPCS
            {
                grupos = ObterListaGrupo(),
                vendedores = ObterListaVendedor(),
                executivos = ObterListaExecutivo(),
                regioes = ObterListaRegiao(),
                usuarios = ObterListaUsuario(),
                segmentos = ObterListaSegmento(),
                CancelarVerificarCodigo = false,
                CLASSIFICACOES_KAT = ControlesUtility.Dicionarios.ClassificacaoKAT(),
                SimNao = ControlesUtility.Dicionarios.SimNao(),
                //BEGIN - 14422 - Tela Cliente - Campo Tecnico Regional
                usuariosTecnicosRegionais = ObterListaSupervidorTecnico(),
                //BEGIN - 14422 - Tela Cliente - Campo Tecnico Regional
            };

            cliente.JavaScriptToRun = string.Empty;

            return View(cliente);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.ClienteBPCS cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ClienteEntity clienteEntity = new ClienteEntity();

                    clienteEntity.CD_CLIENTE = cliente.CD_CLIENTE;
                    clienteEntity.grupo.CD_GRUPO = cliente.grupo.CD_GRUPO;
                    clienteEntity.executivo.CD_EXECUTIVO = cliente.executivo.CD_EXECUTIVO;
                    clienteEntity.NM_CLIENTE = cliente.NM_CLIENTE;
                    clienteEntity.NR_CNPJ = cliente.NR_CNPJ;
                    clienteEntity.EN_ENDERECO = cliente.EN_ENDERECO;
                    clienteEntity.EN_BAIRRO = cliente.EN_BAIRRO;
                    clienteEntity.EN_CEP = cliente.EN_CEP;
                    clienteEntity.EN_CIDADE = cliente.EN_CIDADE;
                    clienteEntity.EN_ESTADO = cliente.EN_ESTADO;
                    clienteEntity.TX_TELEFONE = cliente.TX_TELEFONE;
                    clienteEntity.TX_FAX = cliente.TX_FAX;
                    clienteEntity.TX_EMAIL = cliente.TX_EMAIL;
                    clienteEntity.CD_FILIAL = cliente.CD_FILIAL;
                    clienteEntity.CD_ABC = cliente.CD_ABC;
                    clienteEntity.regiao.CD_REGIAO = cliente.regiao.CD_REGIAO;
                    clienteEntity.vendedor.CD_VENDEDOR = cliente.vendedor.CD_VENDEDOR;
                    clienteEntity.QT_PERIODO = cliente.QT_PERIODO;
                    clienteEntity.FL_PESQ_SATISF = cliente.FL_PESQ_SATISF;
                    clienteEntity.Segmento.ID_SEGMENTO = Convert.ToInt64(cliente.ID_SEGMENTO);
                    clienteEntity.usuario.nidUsuario = cliente.usuario.nidUsuario;

                    clienteEntity.FL_KAT_FIXO = (cliente.FL_KAT_FIXO_SimNao == "S" ? true : false);
                    clienteEntity.DS_CLASSIFICACAO_KAT = cliente.DS_CLASSIFICACAO_KAT;
                    clienteEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    clienteEntity.CD_BCPS = cliente.CD_BCPS;
                    clienteEntity.FL_AtivaPlanoZero = cliente.FL_AtivaPlanoZero;
                    clienteEntity.QTD_PeriodoPlanoZero = cliente.QTD_PeriodoPlanoZero;
                    clienteEntity.TX_NOMERESPONSAVELPECAS = cliente.TX_NOMERESPONSAVELPECAS;
                    clienteEntity.TX_TELEFONERESPONSAVELPECAS = cliente.TX_TELEFONERESPONSAVELPECAS;
                    //BEGIN - 14422 - Tela Cliente - Campo Tecnico Regional
                    clienteEntity.UsuarioTecnicoRegional.nidUsuario = cliente.UsuarioTecnicoRegional.nidUsuario;
                    //END - 14422 - Tela Cliente - Campo Tecnico Regional
                    if (cliente.EmailsInfo != null && cliente.EmailsInfo.Length > 500)
                        throw new Exception("Limite de Caracteres ultrapassado para o campo: E-mails Informativos!!");
                    clienteEntity.EmailsInfo = cliente.EmailsInfo;

                    new ClienteData().Inserir(ref clienteEntity);

                    cliente.JavaScriptToRun = "MensagemSucesso()";
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            cliente.grupos = ObterListaGrupo();
            cliente.vendedores = ObterListaVendedor();
            cliente.executivos = ObterListaExecutivo();
            cliente.regioes = ObterListaRegiao();
            cliente.usuarios = ObterListaUsuario();
            cliente.segmentos = ObterListaSegmento();
            cliente.usuariosTecnicosRegionais = ObterListaSupervidorTecnico();
            cliente.CancelarVerificarCodigo = false;
            cliente.CLASSIFICACOES_KAT = ControlesUtility.Dicionarios.ClassificacaoKAT();
            cliente.SimNao = ControlesUtility.Dicionarios.SimNao();

            return View(cliente); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Cliente cliente = null;
            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();

                clienteEntity.CD_CLIENTE = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        cliente = new Models.Cliente
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_CLIENTE"].ToString()),
                            CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                            grupo = new GrupoEntity
                            {
                                CD_GRUPO = dataTableReader["CD_GRUPO"].ToString(),
                                DS_GRUPO = dataTableReader["DS_GRUPO"].ToString()
                            },
                            CD_RAC = dataTableReader["CD_RAC"].ToString(),
                            vendedor = new VendedorEntity
                            {
                                CD_VENDEDOR = Convert.ToInt64("0" + dataTableReader["CD_VENDEDOR"]),
                                NM_VENDEDOR = dataTableReader["NM_VENDEDOR"].ToString()
                            },
                            NR_CNPJ = dataTableReader["NR_CNPJ"].ToString(),
                            NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString(),
                            EN_ENDERECO = dataTableReader["EN_ENDERECO"].ToString(),
                            EN_BAIRRO = dataTableReader["EN_BAIRRO"].ToString(),
                            EN_CIDADE = dataTableReader["EN_CIDADE"].ToString(),
                            EN_ESTADO = dataTableReader["EN_ESTADO"].ToString(),
                            EN_CEP = dataTableReader["EN_CEP"].ToString(),
                            CD_BCPS = dataTableReader["CD_BCPS"].ToString(),
                            regiao = new RegiaoEntity
                            {
                                CD_REGIAO = dataTableReader["CD_REGIAO"].ToString(),
                                DS_REGIAO = dataTableReader["DS_REGIAO"].ToString(),
                            },
                            CD_ABC = dataTableReader["CD_ABC"].ToString(),
                            CD_FILIAL = dataTableReader["CD_FILIAL"].ToString(),
                            CL_CLIENTE = dataTableReader["CL_CLIENTE"].ToString(),
                            TX_TELEFONE = dataTableReader["TX_TELEFONE"].ToString(),
                            TX_FAX = dataTableReader["TX_FAX"].ToString(),
                            TX_EMAIL = dataTableReader["TX_EMAIL"].ToString(),
                            executivo = new ExecutivoEntity
                            {
                                CD_EXECUTIVO = Convert.ToInt64("0" + dataTableReader["CD_EXECUTIVO"]),
                                NM_EXECUTIVO = dataTableReader["NM_EXECUTIVO"].ToString()
                            },
                            QT_PERIODO = Convert.ToInt32("0" + dataTableReader["QT_PERIODO"]),
                            DS_CLASSIFICACAO_KAT = dataTableReader["DS_CLASSIFICACAO_KAT"].ToString(),
                            grupos = ObterListaGrupo(),
                            vendedores = ObterListaVendedor(),
                            executivos = ObterListaExecutivo(),
                            regioes = ObterListaRegiao(),
                            listaAtivoCliente = ObterListaAtivoCliente(Convert.ToInt32(dataTableReader["CD_CLIENTE"])),
                            usuarios = ObterListaUsuario(),
                            segmentos = ObterListaSegmento(),
                            CancelarVerificarCodigo = true,
                            CLASSIFICACOES_KAT = ControlesUtility.Dicionarios.ClassificacaoKAT(),
                            SimNao = ControlesUtility.Dicionarios.SimNao(),
                            TX_NOMERESPONSAVELPECAS = dataTableReader["TX_NOMERESPONSAVELPECAS"].ToString(),
                            TX_TELEFONERESPONSAVELPECAS = dataTableReader["TX_TELEFONERESPONSAVELPECAS"].ToString(),

                            //BEGIN - 14422 - Tela Cliente - Campo Tecnico Regional
                            UsuarioTecnicoRegional = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO_TECNICOREGIONAL"]),
                                cnmNome = dataTableReader["cnmNomeTecRegional"].ToString() + " (" + dataTableReader["cdsLoginTecRegional"].ToString() + ")"

                            },
                            usuariosTecnicosRegionais = ObterListaSupervidorTecnico()
                            //END -14422 - Tela Cliente - Campo Tecnico Regional
                        };

                        //Chamado SL00033984
                        if (cliente.NR_CNPJ.Length >= 11 && cliente.NR_CNPJ.Length <= 15)
                        {
                            //completa com zeros a esquerda
                            cliente.NR_CNPJ = cliente.NR_CNPJ.PadLeft(16, '0');

                        }

                        if(dataTableReader["EmailsInfo"] != null)
                        {
                            cliente.EmailsInfo = dataTableReader["EmailsInfo"].ToString();
                        }
                        else
                        {
                            cliente.EmailsInfo = "";
                        }

                        if (dataTableReader["FL_KAT_FIXO"] != DBNull.Value)
                        {
                            cliente.FL_KAT_FIXO = Convert.ToBoolean(dataTableReader["FL_KAT_FIXO"]);
                            cliente.FL_KAT_FIXO_SimNao = (cliente.FL_KAT_FIXO == true ? "S" : "N");
                        }
                        else
                            cliente.FL_KAT_FIXO_SimNao = "N";

                        if (dataTableReader["FL_PESQ_SATISF"] != DBNull.Value)
                        {
                            cliente.FL_PESQ_SATISF = dataTableReader["FL_PESQ_SATISF"].ToString();
                        }

                        if (dataTableReader["ID_SEGMENTO"] != DBNull.Value)
                        {
                            cliente.ID_SEGMENTO = dataTableReader["ID_SEGMENTO"].ToString();
                        }

                        if (dataTableReader["DT_DESATIVACAO"] != DBNull.Value)
                        {
                            cliente.DT_DESATIVACAO = Convert.ToDateTime(dataTableReader["DT_DESATIVACAO"], new CultureInfo("pt-BR"));
                            cliente.DT_DESATIVACAO_Edit = Convert.ToDateTime(dataTableReader["DT_DESATIVACAO"]).ToString("dd/MM/yyyy");
                        }

                        if (dataTableReader["FL_AtivaPlanoZero"] != DBNull.Value)
                        {
                            cliente.FL_AtivaPlanoZero = dataTableReader["FL_AtivaPlanoZero"].ToString();
                        }

                        if (dataTableReader["QTD_PeriodoPlanoZero"] != DBNull.Value)
                        {
                            cliente.QTD_PeriodoPlanoZero = Convert.ToInt32(dataTableReader["QTD_PeriodoPlanoZero"]);
                        }
                        //if (dataTableReader["BPCS"] != DBNull.Value)
                        //    cliente.BPCS = Convert.ToBoolean(dataTableReader["BPCS"]);

                        if (dataTableReader["nidUsuario"] != DBNull.Value)
                        {
                            cliente.usuario = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["nidUsuario"])
                            };
                        }

                        if (string.IsNullOrEmpty(cliente.DS_CLASSIFICACAO_KAT))
                            cliente.DS_CLASSIFICACAO_KAT = "0";
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

            if (cliente == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(cliente);
            }
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Cliente cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ClienteEntity clienteEntity = new ClienteEntity();

                    clienteEntity.CD_CLIENTE = cliente.CD_CLIENTE;
                    clienteEntity.grupo.CD_GRUPO = cliente.grupo.CD_GRUPO;
                    DateTime date1 = new DateTime(0001, 01, 01, 00, 00, 00);
                    //if (cliente.DT_DESATIVACAO != null && cliente.DT_DESATIVACAO != date1)
                    //{
                    //    clienteEntity.DT_DESATIVACAO = Convert.ToDateTime(cliente.DT_DESATIVACAO);
                    //}
                    //SL00035945
                    if (cliente.DT_DESATIVACAO_Edit != null && cliente.DT_DESATIVACAO_Edit != "")
                    {
                        clienteEntity.DT_DESATIVACAO = Convert.ToDateTime(cliente.DT_DESATIVACAO_Edit);
                    }

                    clienteEntity.executivo.CD_EXECUTIVO = cliente.executivo.CD_EXECUTIVO;
                    clienteEntity.NM_CLIENTE = cliente.NM_CLIENTE;
                    clienteEntity.NR_CNPJ = cliente.NR_CNPJ;
                    clienteEntity.EN_ENDERECO = cliente.EN_ENDERECO;
                    clienteEntity.EN_BAIRRO = cliente.EN_BAIRRO;
                    clienteEntity.EN_CEP = cliente.EN_CEP;
                    clienteEntity.EN_CIDADE = cliente.EN_CIDADE;
                    clienteEntity.EN_ESTADO = cliente.EN_ESTADO;
                    clienteEntity.TX_TELEFONE = cliente.TX_TELEFONE;
                    clienteEntity.TX_FAX = cliente.TX_FAX;
                    clienteEntity.TX_EMAIL = cliente.TX_EMAIL;
                    clienteEntity.CD_FILIAL = cliente.CD_FILIAL;
                    clienteEntity.CD_ABC = cliente.CD_ABC;
                    clienteEntity.regiao.CD_REGIAO = cliente.regiao.CD_REGIAO;
                    clienteEntity.vendedor.CD_VENDEDOR = cliente.vendedor.CD_VENDEDOR;
                    clienteEntity.QT_PERIODO = cliente.QT_PERIODO;
                    clienteEntity.FL_PESQ_SATISF = cliente.FL_PESQ_SATISF;
                    clienteEntity.Segmento.ID_SEGMENTO = Convert.ToInt64(cliente.ID_SEGMENTO);
                    clienteEntity.usuario.nidUsuario = cliente.usuario.nidUsuario;
                    clienteEntity.FL_KAT_FIXO = (cliente.FL_KAT_FIXO_SimNao == "S" ? true : false);
                    clienteEntity.DS_CLASSIFICACAO_KAT = cliente.DS_CLASSIFICACAO_KAT;
                    clienteEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    clienteEntity.CD_BCPS = cliente.CD_BCPS;
                    clienteEntity.FL_AtivaPlanoZero = cliente.FL_AtivaPlanoZero;
                    clienteEntity.QTD_PeriodoPlanoZero = cliente.QTD_PeriodoPlanoZero;
                    clienteEntity.TX_NOMERESPONSAVELPECAS = cliente.TX_NOMERESPONSAVELPECAS;
                    clienteEntity.TX_TELEFONERESPONSAVELPECAS = cliente.TX_TELEFONERESPONSAVELPECAS;

                    clienteEntity.EmailsInfo = cliente.EmailsInfo;
                    if (cliente.EmailsInfo != null && cliente.EmailsInfo.Length > 500)
                        throw new Exception("Limite de Caracteres ultrapassado para o campo: E-mails Informativos!!");
                    //BEGIN - 14422 - Tela Cliente - Campo Tecnico Regional
                    clienteEntity.UsuarioTecnicoRegional.nidUsuario = cliente.UsuarioTecnicoRegional.nidUsuario;
                    //END - 14422 - Tela Cliente - Campo Tecnico Regional

                    new ClienteData().Alterar(clienteEntity);

                    cliente.JavaScriptToRun = "MensagemSucesso()";
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            cliente.grupos = ObterListaGrupo();
            cliente.vendedores = ObterListaVendedor();
            cliente.executivos = ObterListaExecutivo();
            cliente.listaAtivoCliente = ObterListaAtivoCliente(cliente.CD_CLIENTE);
            cliente.usuarios = ObterListaUsuario();
            cliente.regioes = ObterListaRegiao();
            cliente.segmentos = ObterListaSegmento();
            cliente.CancelarVerificarCodigo = true;
            cliente.CLASSIFICACOES_KAT = ControlesUtility.Dicionarios.ClassificacaoKAT();
            cliente.SimNao = ControlesUtility.Dicionarios.SimNao();
            cliente.usuariosTecnicosRegionais = ObterListaSupervidorTecnico();

            return View(cliente); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Cliente cliente = null;

            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();

                clienteEntity.CD_CLIENTE = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        cliente = new Models.Cliente
                        {
                            CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                            grupo = new GrupoEntity
                            {
                                CD_GRUPO = dataTableReader["CD_GRUPO"].ToString(),
                                DS_GRUPO = dataTableReader["DS_GRUPO"].ToString()
                            },
                            CD_RAC = dataTableReader["CD_RAC"].ToString(),
                            vendedor = new VendedorEntity
                            {
                                CD_VENDEDOR = Convert.ToInt64("0" + dataTableReader["CD_VENDEDOR"]),
                                NM_VENDEDOR = dataTableReader["NM_VENDEDOR"].ToString()
                            },
                            NR_CNPJ = dataTableReader["NR_CNPJ"].ToString(),
                            NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString(),
                            EN_ENDERECO = dataTableReader["EN_ENDERECO"].ToString(),
                            EN_BAIRRO = dataTableReader["EN_BAIRRO"].ToString(),
                            EN_CIDADE = dataTableReader["EN_CIDADE"].ToString(),
                            EN_ESTADO = dataTableReader["EN_ESTADO"].ToString(),
                            EN_CEP = dataTableReader["EN_CEP"].ToString(),
                            CD_BCPS = dataTableReader["CD_BCPS"].ToString(),
                            regiao = new RegiaoEntity
                            {
                                CD_REGIAO = dataTableReader["CD_REGIAO"].ToString(),
                                DS_REGIAO = dataTableReader["DS_REGIAO"].ToString()
                            },
                            CD_FILIAL = dataTableReader["CD_FILIAL"].ToString(),
                            CL_CLIENTE = dataTableReader["CL_CLIENTE"].ToString(),
                            TX_TELEFONE = dataTableReader["TX_TELEFONE"].ToString(),
                            TX_FAX = dataTableReader["TX_FAX"].ToString(),
                            TX_EMAIL = dataTableReader["TX_EMAIL"].ToString(),
                            executivo = new ExecutivoEntity
                            {
                                CD_EXECUTIVO = Convert.ToInt64("0" + dataTableReader["CD_EXECUTIVO"]),
                                NM_EXECUTIVO = dataTableReader["NM_EXECUTIVO"].ToString()
                            },
                            grupos = ObterListaGrupo(),
                            vendedores = ObterListaVendedor(),
                            executivos = ObterListaExecutivo(),
                            CancelarVerificarCodigo = true

                        };
                        if (dataTableReader["DT_DESATIVACAO"] != DBNull.Value)
                        {
                            cliente.DT_DESATIVACAO = Convert.ToDateTime(dataTableReader["DT_DESATIVACAO"], new CultureInfo("pt-BR"));
                        }

                        //if (dataTableReader["BPCS"] != DBNull.Value)
                        //    cliente.BPCS = Convert.ToBoolean(dataTableReader["BPCS"]);
                        if (dataTableReader["nidUsuario"] != DBNull.Value)
                        {
                            cliente.usuario = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["nidUsuario"])
                            };
                        }
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

            if (cliente == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(cliente);
            }
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Cliente cliente = new Models.Cliente();
            try
            {
                if (ModelState.IsValid)
                {
                    ClienteEntity clienteEntity = new ClienteEntity();

                    clienteEntity.CD_CLIENTE = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey));
                    clienteEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new ClienteData().Excluir(clienteEntity);


                    cliente.JavaScriptToRun = "MensagemSucesso()";

                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(cliente);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        public ActionResult DetalhesCliente(string idKey)
        {
            Models.Cliente cliente = null;

            try
            {
                if (Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey)) == 0)
                {
                    return HttpNotFound();
                }

                ClienteEntity clienteEntity = new ClienteEntity();

                clienteEntity.CD_CLIENTE = Convert.ToInt32(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        cliente = new Models.Cliente
                        {
                            CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                            NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString()
                        };
                    }

                    CarregarDetalhesCliente(clienteEntity.CD_CLIENTE);
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

            if (cliente == null)
            {
                return HttpNotFound();
            }

            ViewBag.idKey = idKey;
            ViewBag.CD_CLIENTE = cliente.CD_CLIENTE;
            ViewBag.NM_CLIENTE = cliente.NM_CLIENTE;
            return View();
        }

        protected void CarregarDetalhesCliente(Int64 CD_CLIENTE)
        {
            ClienteEntity clienteEntity = new ClienteEntity();

            clienteEntity.CD_CLIENTE = CD_CLIENTE;
            DataTableReader dataTableReader = new ClienteData().ObterListaDetalhes(clienteEntity).CreateDataReader();

            DataTableReader dataTableReaderManutencao = new ClienteData().ObterListaDetalhesManutencao(clienteEntity).CreateDataReader();

            List<ManutencaoDetalhesCliente> manutencaoDetalhes = new List<ManutencaoDetalhesCliente>();

            if (dataTableReaderManutencao.HasRows)
            {
                while (dataTableReaderManutencao.Read())
                {
                    ManutencaoDetalhesCliente manutencaodetalhe = new ManutencaoDetalhesCliente();

                    manutencaodetalhe.ID_OS = Convert.ToInt64(dataTableReaderManutencao["ID_OS"]);
                    manutencaodetalhe.CD_CLIENTE = Convert.ToInt64(dataTableReaderManutencao["CD_CLIENTE"]);
                    manutencaodetalhe.TOT_PECAS = Convert.ToDouble("0" + dataTableReaderManutencao["TOT_PECAS"]);
                    manutencaodetalhe.TOT_MAO_OBRA = Convert.ToDouble("0" + dataTableReaderManutencao["TOT_MAO_OBRA"]);
                    manutencaodetalhe.DS_LINHA_PRODUTO = dataTableReaderManutencao["DS_LINHA_PRODUTO"].ToString();

                    manutencaoDetalhes.Add(manutencaodetalhe);
                }
            }


            if (dataTableReaderManutencao != null)
            {
                dataTableReaderManutencao.Dispose();
                dataTableReaderManutencao = null;
            }

            string Thead = string.Empty;
            string tdNrEquipamentos = string.Empty;
            string tdVendasMES = string.Empty;
            string tdVolumeMES = string.Empty;
            string tdVendas3M = string.Empty;
            string tdVolume3M = string.Empty;
            string tdVendas12M = string.Empty;
            string tdVolume12M = string.Empty;
            string tdCusto12M = string.Empty;
            string tdDepreciacao12M = string.Empty;
            string tdManutencao12M = string.Empty;
            string tdDEPCOM12M = string.Empty;
            string tdLESAFO12M = string.Empty;
            string tdGM12M = string.Empty;
            string tdOI12M = string.Empty;

            // RIBBON e ETIQUETA serão unificados
            string DS_LINHA_PRODUTO = string.Empty;
            Int64 NREQP = 0;
            decimal QT_VENDAS = 0;
            decimal QT_VENDAS_3 = 0;
            decimal QT_VENDAS_12 = 0;
            decimal VL_VENDAS = 0;
            decimal VL_VENDAS_3 = 0;
            decimal VL_VENDAS_12 = 0;
            decimal DEPCOM = 0;
            decimal LESAFO = 0;
            decimal VL_MANUTENCAO = 0;
            decimal DEPRECIACAO = 0;
            decimal CUSTO = 0;
            Decimal ValorTotal = 0;

            Double VL_PECAS = 0;
            Double VL_MAO_OBRA = 0;
            Double TOT_PECAS = 0;
            Double TOT_MAO_OBRA = 0;
            Int64 ID_OS = 0;
            Int64 ID_OS_CORRENTE = 0;
            int MaoDeObraAtualizado = 0;

            
            if (dataTableReader.HasRows)
            {
                while (dataTableReader.Read())
                {
                    if (Convert.ToInt32(dataTableReader["NREQP"]) > 0 || Convert.ToDecimal("0" + dataTableReader["VL_VENDAS"]) != 0 || Convert.ToDecimal("0" + dataTableReader["QT_VENDAS"]) != 0 ||
                        Convert.ToDecimal("0" + dataTableReader["VL_VENDAS_3"]) != 0 || Convert.ToDecimal("0" + dataTableReader["QT_VENDAS_3"]) != 0 ||
                        Convert.ToDecimal("0" + dataTableReader["VL_VENDAS_12"]) != 0 || Convert.ToDecimal("0" + dataTableReader["QT_VENDAS_12"]) != 0 ||
                        Convert.ToDecimal("0" + dataTableReader["CUSTO"]) != 0 || Convert.ToDecimal("0" + dataTableReader["DEPRECIACAO"]) != 0 ||
                        Convert.ToDecimal("0" + dataTableReader["VL_MANUTENCAO"]) != 0 || Convert.ToDecimal("0" + dataTableReader["DEPCOM"]) != 0 || Convert.ToDecimal(dataTableReader["LESAFO"]) != 0) //Convert.ToDecimal("0" + dataTableReader["LESAFO"]) != 0)

                    {
                        if (dataTableReader["DS_LINHA_PRODUTO"].ToString().ToUpper().Trim() == "RIBBON" || dataTableReader["DS_LINHA_PRODUTO"].ToString().ToUpper().Trim() == "ETIQUETA")
                        {
                            ValorTotal = 0;
                            var tipoProduto = dataTableReader["DS_LINHA_PRODUTO"].ToString();
                            VL_PECAS = 0;
                            VL_MAO_OBRA = 0;
                            TOT_PECAS = 0;
                            TOT_MAO_OBRA = 0;
                            if (tipoProduto != null)
                            {
                                var listaManutencaoTipoProduto = manutencaoDetalhes.Where(x => x.DS_LINHA_PRODUTO.ToUpper() == tipoProduto.ToUpper()).ToList();
                                if (listaManutencaoTipoProduto?.Count == 0)
                                    ValorTotal = 0;
                                else
                                {
                                    foreach (var manutencao in listaManutencaoTipoProduto)
                                    {
                                        TOT_PECAS += Convert.ToDouble(manutencao.TOT_PECAS);

                                        if (ID_OS_CORRENTE == 0)
                                            ID_OS_CORRENTE = Convert.ToInt64(manutencao.ID_OS);

                                        ID_OS = Convert.ToInt64(manutencao.ID_OS);

                                        if (ID_OS == ID_OS_CORRENTE && MaoDeObraAtualizado == 0)
                                        {
                                            TOT_MAO_OBRA += Convert.ToDouble(manutencao.TOT_MAO_OBRA);
                                            MaoDeObraAtualizado++;
                                        }
                                        else if (ID_OS != ID_OS_CORRENTE && MaoDeObraAtualizado == 1)
                                        {
                                            MaoDeObraAtualizado = 0;
                                            ID_OS_CORRENTE = ID_OS;
                                        }
                                        if (ID_OS == ID_OS_CORRENTE && MaoDeObraAtualizado == 0)
                                        {
                                            TOT_MAO_OBRA += Convert.ToDouble(manutencao.TOT_MAO_OBRA);
                                            MaoDeObraAtualizado++;
                                        }
                                    }
                                    ValorTotal = Convert.ToDecimal(TOT_MAO_OBRA + TOT_PECAS);
                                }
                            }
                            else
                            {
                                ValorTotal = 0;
                            }
                            // RIBBON e ETIQUETA serão unificados
                            DS_LINHA_PRODUTO += (!string.IsNullOrEmpty(DS_LINHA_PRODUTO) ? "+" : string.Empty) + dataTableReader["DS_LINHA_PRODUTO"].ToString();


                            NREQP += Convert.ToInt64(dataTableReader["NREQP"]);
                            VL_VENDAS += Convert.ToDecimal("0" + dataTableReader["VL_VENDAS"]);
                            QT_VENDAS += Convert.ToDecimal("0" + dataTableReader["QT_VENDAS"]);
                            VL_VENDAS_3 += Convert.ToDecimal("0" + dataTableReader["VL_VENDAS_3"]);
                            QT_VENDAS_3 += Convert.ToDecimal("0" + dataTableReader["QT_VENDAS_3"]);
                            VL_VENDAS_12 += Convert.ToDecimal("0" + dataTableReader["VL_VENDAS_12"]);
                            QT_VENDAS_12 += Convert.ToDecimal("0" + dataTableReader["QT_VENDAS_12"]);
                            CUSTO += Convert.ToDecimal("0" + dataTableReader["CUSTO"]);
                            DEPRECIACAO += Convert.ToDecimal("0" + dataTableReader["DEPRECIACAO"]);
                            VL_MANUTENCAO += Convert.ToDecimal("0" + ValorTotal);
                            DEPCOM += Convert.ToDecimal("0" + dataTableReader["DEPCOM"]);


                            //BEGIN - Melhoria - IM8166298 - Ubirajara Lisboa - 23/03/2021
                            //LESAFO += Convert.ToDecimal("0" + dataTableReader["LESAFO"]);
                            LESAFO += Convert.ToDecimal(dataTableReader["LESAFO"]);
                            //BEGIN - Melhoria - IM8166298 - Ubirajara Lisboa - 23/03/2021
                            
                        }
                        else
                        {
                            var tipoProduto = dataTableReader["DS_LINHA_PRODUTO"].ToString();

                            VL_PECAS = 0;
                            VL_MAO_OBRA = 0;
                            TOT_PECAS = 0;
                            TOT_MAO_OBRA = 0;

                            if (tipoProduto != null)
                            {
                                var listaManutencaoTipoProduto = manutencaoDetalhes.Where(x => x.DS_LINHA_PRODUTO.ToUpper() == tipoProduto.ToUpper()).ToList();
                                if (listaManutencaoTipoProduto?.Count == 0)
                                    ValorTotal = 0;
                                else
                                {
                                    foreach(var manutencao in listaManutencaoTipoProduto)
                                    {
                                        TOT_PECAS += Convert.ToDouble(manutencao.TOT_PECAS);

                                        if (ID_OS_CORRENTE == 0)
                                            ID_OS_CORRENTE = Convert.ToInt64(manutencao.ID_OS);

                                        ID_OS = Convert.ToInt64(manutencao.ID_OS);

                                        if (ID_OS == ID_OS_CORRENTE && MaoDeObraAtualizado == 0)
                                        {
                                            TOT_MAO_OBRA += Convert.ToDouble(manutencao.TOT_MAO_OBRA);
                                            MaoDeObraAtualizado++;
                                        }
                                        else if (ID_OS != ID_OS_CORRENTE && MaoDeObraAtualizado == 1)
                                        {
                                            MaoDeObraAtualizado = 0;
                                            ID_OS_CORRENTE = ID_OS;
                                        }
                                        if (ID_OS == ID_OS_CORRENTE && MaoDeObraAtualizado == 0)
                                        {
                                            TOT_MAO_OBRA += Convert.ToDouble(manutencao.TOT_MAO_OBRA);
                                            MaoDeObraAtualizado++;
                                        }
                                    }
                                    ValorTotal = Convert.ToDecimal(TOT_MAO_OBRA + TOT_PECAS);
                                }
                            }
                            else
                            {
                                ValorTotal = 0;
                            }

                            Thead += "<th scope='col'>" + dataTableReader["DS_LINHA_PRODUTO"].ToString() + "</th>";

                            tdNrEquipamentos += "<td>" + dataTableReader["NREQP"].ToString() + "</td>";
                            tdVendasMES += "<td>" + Convert.ToDecimal("0" + dataTableReader["VL_VENDAS"]).ToString("C2") + "</td>";
                            tdVolumeMES += "<td>" + Convert.ToDecimal("0" + dataTableReader["QT_VENDAS"]).ToString("N0") + "</td>";
                            tdVendas3M += "<td>" + Convert.ToDecimal("0" + dataTableReader["VL_VENDAS_3"]).ToString("C2") + "</td>";
                            tdVolume3M += "<td>" + Convert.ToDecimal("0" + dataTableReader["QT_VENDAS_3"]).ToString("N0") + "</td>";
                            tdVendas12M += "<td>" + Convert.ToDecimal("0" + dataTableReader["VL_VENDAS_12"]).ToString("C2") + "</td>";
                            tdVolume12M += "<td>" + Convert.ToDecimal("0" + dataTableReader["QT_VENDAS_12"]).ToString("N0") + "</td>";
                            tdCusto12M += "<td>" + Convert.ToDecimal("0" + dataTableReader["CUSTO"]).ToString("C2") + "</td>";
                            tdDepreciacao12M += "<td>" + Convert.ToDecimal("0" + dataTableReader["DEPRECIACAO"]).ToString("C2") + "</td>";
                            tdManutencao12M += "<td>" + Convert.ToDecimal("0" + ValorTotal).ToString("C2") + "</td>";
                            tdDEPCOM12M += "<td>" + Convert.ToDecimal("0" + dataTableReader["DEPCOM"]).ToString("P2") + "</td>";

                            //BEGIN - Melhoria - IM8166298 - Ubirajara Lisboa - 23/03/2021
                            //tdLESAFO12M += "<td>" + Convert.ToDecimal("0" + dataTableReader["LESAFO"]).ToString("P2") + "</td>";
                            tdLESAFO12M += "<td>" + Convert.ToDecimal(dataTableReader["LESAFO"]).ToString("P2") + "</td>";
                            //BEGIN - Melhoria - IM8166298 - Ubirajara Lisboa - 23/03/2021



                            decimal GM12M = 0;
                            decimal OI12M = 0;

                            if (Convert.ToDecimal("0" + dataTableReader["VL_VENDAS_12"]) != 0)
                            {
                                GM12M = 1 - ((Convert.ToDecimal("0" + dataTableReader["VL_MANUTENCAO"]) + Convert.ToDecimal("0" + dataTableReader["DEPRECIACAO"]) + Convert.ToDecimal("0" + dataTableReader["CUSTO"])) / Convert.ToDecimal("0" + dataTableReader["VL_VENDAS_12"])) - Convert.ToDecimal("0" + dataTableReader["DEPCOM"]);
                                //BEGIN - Melhoria - IM8166298 - Ubirajara Lisboa - 23/03/2021
                                //OI12M = 1 - ((Convert.ToDecimal("0" + dataTableReader["VL_MANUTENCAO"]) + Convert.ToDecimal("0" + dataTableReader["DEPRECIACAO"]) + Convert.ToDecimal("0" + dataTableReader["CUSTO"])) / Convert.ToDecimal("0" + dataTableReader["VL_VENDAS_12"])) - Convert.ToDecimal("0" + dataTableReader["DEPCOM"]) - Convert.ToDecimal("0" + dataTableReader["LESAFO"]);
                                OI12M = 1 - ((Convert.ToDecimal("0" + dataTableReader["VL_MANUTENCAO"]) + Convert.ToDecimal("0" + dataTableReader["DEPRECIACAO"]) + Convert.ToDecimal("0" + dataTableReader["CUSTO"])) / Convert.ToDecimal("0" + dataTableReader["VL_VENDAS_12"])) - Convert.ToDecimal("0" + dataTableReader["DEPCOM"]) - Convert.ToDecimal(dataTableReader["LESAFO"]);

                                //BEGIN - Melhoria - IM8166298 - Ubirajara Lisboa - 23/03/2021
                            }

                            tdGM12M += "<td>" + Convert.ToDecimal(GM12M).ToString("P2") + "</td>";
                            tdOI12M += "<td>" + Convert.ToDecimal(OI12M).ToString("P2") + "</td>";

                            VL_PECAS = 0;
                            VL_MAO_OBRA = 0;
                            TOT_PECAS = 0;
                            TOT_MAO_OBRA = 0;
                            ID_OS = 0;
                            ID_OS_CORRENTE = 0;
                            MaoDeObraAtualizado = 0;
                            ValorTotal = 0;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(DS_LINHA_PRODUTO))
            {
                // RIBBON e ETIQUETA serão unificados
                Thead += "<th scope='col'>" + DS_LINHA_PRODUTO + "</th>";

                tdNrEquipamentos += "<td>" + NREQP.ToString() + "</td>";
                tdVendasMES += "<td>" + VL_VENDAS.ToString("C2") + "</td>";
                tdVolumeMES += "<td>" + QT_VENDAS.ToString("N0") + "</td>";
                tdVendas3M += "<td>" + VL_VENDAS_3.ToString("C2") + "</td>";
                tdVolume3M += "<td>" + QT_VENDAS_3.ToString("N0") + "</td>";
                tdVendas12M += "<td>" + VL_VENDAS_12.ToString("C2") + "</td>";
                tdVolume12M += "<td>" + QT_VENDAS_12.ToString("N0") + "</td>";
                tdCusto12M += "<td>" + CUSTO.ToString("C2") + "</td>";
                tdDepreciacao12M += "<td>" + DEPRECIACAO.ToString("C2") + "</td>";
                tdManutencao12M += "<td>" + VL_MANUTENCAO.ToString("C2") + "</td>";
                tdDEPCOM12M += "<td>" + DEPCOM.ToString("P2") + "</td>";
                tdLESAFO12M += "<td>" + LESAFO.ToString("P2") + "</td>";

                decimal GM12M = 0;
                decimal OI12M = 0;

                if (VL_VENDAS_12 != 0)
                {
                    GM12M = 1 - ((VL_MANUTENCAO + DEPRECIACAO + CUSTO) / VL_VENDAS_12) - DEPCOM;
                    OI12M = 1 - ((VL_MANUTENCAO + DEPRECIACAO + CUSTO) / VL_VENDAS_12) - DEPCOM - LESAFO;
                }

                tdGM12M += "<td>" + Convert.ToDecimal(GM12M).ToString("P2") + "</td>";
                tdOI12M += "<td>" + Convert.ToDecimal(OI12M).ToString("P2") + "</td>";
            }

            if (dataTableReader != null)
            {
                dataTableReader.Dispose();
                dataTableReader = null;
            }

            ViewBag.Thead = Thead;
            ViewBag.tdNrEquipamentos = tdNrEquipamentos;
            ViewBag.tdVendasMES = tdVendasMES;
            ViewBag.tdVolumeMES = tdVolumeMES;
            ViewBag.tdVendas3M = tdVendas3M;
            ViewBag.tdVolume3M = tdVolume3M;
            ViewBag.tdVendas12M = tdVendas12M;
            ViewBag.tdVolume12M = tdVolume12M;
            ViewBag.tdCusto12M = tdCusto12M;
            ViewBag.tdDepreciacao12M = tdDepreciacao12M;
            ViewBag.tdManutencao12M = tdManutencao12M;
            ViewBag.tdDEPCOM12M = tdDEPCOM12M;
            ViewBag.tdLESAFO12M = tdLESAFO12M;
            ViewBag.tdGM12M = tdGM12M;
            ViewBag.tdOI12M = tdOI12M;
        }

        public ActionResult VerificarCodigo(Int32 CD_CLIENTE, bool CancelarVerificarCodigo)
        {
            bool Liberado = true;

            try
            {
                if (CancelarVerificarCodigo == false && CD_CLIENTE > 0)
                {
                    ClienteEntity clienteEntity = new ClienteEntity();

                    clienteEntity.CD_CLIENTE = CD_CLIENTE;
                    DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            Liberado = false;
                        }
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

        public JsonResult VerificarCodigoJson(Int32 CD_CLIENTE)
        {
            bool Redirecionar = false;
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();

                clienteEntity.CD_CLIENTE = CD_CLIENTE;
                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        Redirecionar = true;
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (Redirecionar == true)
                {
                    jsonResult.Add("Status", "Redirecionar");
                    jsonResult.Add("idKey", Utility.ControlesUtility.Criptografia.Criptografar(clienteEntity.CD_CLIENTE.ToString()));
                }
                else
                {
                    // Busca na BPCS para importação
                    Models.Cliente cliente = new Models.Cliente();

                    dataTableReader = new ClienteData().ObterListaBPCS(clienteEntity).CreateDataReader();
                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            cliente.CD_CLIENTE = Convert.ToInt32(dataTableReader["CCUST"]);
                            cliente.EN_ENDERECO = dataTableReader["CAD1"].ToString();
                            cliente.EN_BAIRRO = dataTableReader["CAD2"].ToString();
                            cliente.EN_CIDADE = dataTableReader["CAD3"].ToString();
                            cliente.TX_FAX = dataTableReader["CMFAXN"].ToString();
                            cliente.TX_TELEFONE = dataTableReader["CPHON"].ToString();
                            cliente.NM_CLIENTE = dataTableReader["CNME"].ToString();
                            cliente.EN_ESTADO = dataTableReader["CSTE"].ToString();
                            cliente.EN_CEP = dataTableReader["CZIP"].ToString();
                            cliente.CD_ABC = dataTableReader["RVABC"].ToString();

                            if (!string.IsNullOrEmpty(dataTableReader["RVCREG"].ToString().Trim()))
                            {
                                bool encontrado = false;
                                DataTableReader dtRegiao = new RegiaoData().ObterLista(new RegiaoEntity() { CD_REGIAO = dataTableReader["RVCREG"].ToString() }).CreateDataReader();

                                if (dtRegiao.HasRows)
                                {
                                    if (dtRegiao.Read())
                                    {
                                        cliente.regiao.CD_REGIAO = dtRegiao["CD_REGIAO"].ToString();
                                        cliente.regiao.DS_REGIAO = dtRegiao["DS_REGIAO"].ToString();
                                        encontrado = true;
                                    }
                                }

                                if (dtRegiao != null)
                                {
                                    dtRegiao.Dispose();
                                    dtRegiao = null;
                                }

                                // Se não encontrar a região, utiliza o valor padrão para NÃO ENCONTRADO
                                if (encontrado == false)
                                {
                                    dtRegiao = new RegiaoData().ObterLista(new RegiaoEntity() { CD_REGIAO = "??" }).CreateDataReader();

                                    if (dtRegiao.HasRows)
                                    {
                                        if (dtRegiao.Read())
                                        {
                                            cliente.regiao.CD_REGIAO = dtRegiao["CD_REGIAO"].ToString();
                                            cliente.regiao.DS_REGIAO = dtRegiao["DS_REGIAO"].ToString();
                                        }
                                    }

                                    if (dtRegiao != null)
                                    {
                                        dtRegiao.Dispose();
                                        dtRegiao = null;
                                    }
                                }
                            }

                            cliente.CD_FILIAL = dataTableReader["RVREF5"].ToString();
                            cliente.CL_CLIENTE = dataTableReader["RVTYPE"].ToString();
                            cliente.CD_RAC = dataTableReader["RVREF3"].ToString();

                            if (dataTableReader["BCCCGC"] != DBNull.Value)
                                cliente.NR_CNPJ = Convert.ToInt32("0" + dataTableReader["BCCCGC"]).ToString("00000000") + "/" + Convert.ToInt32("0" + dataTableReader["BCFCGC"]).ToString("0000") + "-" + Convert.ToInt32("0" + dataTableReader["BCDCGC"]).ToString("00");
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    jsonResult.Add("Status", "Permanecer");
                    jsonResult.Add("cliente", cliente);
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

        public ActionResult ObterKatPorCliente(int clienteId)
        {
            return Json(new KAT().ObterKatPorCliente(clienteId), JsonRequestBehavior.AllowGet);
        }

        protected List<Segmento> ObterListaSegmento()
        {
            List<Segmento> listaSegmentos = new List<Segmento>();
            listaSegmentos.Add(new Segmento());

            SegmentoData data = new SegmentoData();
            listaSegmentos.AddRange((from s in data.ObterLista(new SegmentoEntity())
                                     select new Segmento()
                                     {
                                         idKey = ControlesUtility.Criptografia.Criptografar(s.ID_SEGMENTO.ToString()),
                                         id_segmento = s.ID_SEGMENTO,
                                         ds_segmentomin = s.DS_SEGMENTO_MIN,
                                         ds_segmento = s.DS_SEGMENTO,
                                         nm_criticidade = s.NM_CRITICIDADE,
                                         ds_descricao = string.IsNullOrEmpty(s.DS_DESCRICAO) ? "" : s.DS_DESCRICAO
                                     }));

            return listaSegmentos;
        }

    }
}