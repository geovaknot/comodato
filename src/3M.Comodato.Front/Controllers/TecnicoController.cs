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
    public class TecnicoController : BaseController
    {
        // GET: Tecnico
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Tecnico> tecnicos = new List<Models.Tecnico>();

            try
            {
                TecnicoEntity tecnicoEntity = new TecnicoEntity();
                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Tecnico Tecnico = new Models.Tecnico
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_TECNICO"].ToString()),
                            CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                            NM_TECNICO = dataTableReader["NM_TECNICO"].ToString(),
                            NM_REDUZIDO = dataTableReader["NM_REDUZIDO"].ToString(),
                            EN_ENDERECO = dataTableReader["EN_ENDERECO"].ToString(),
                            EN_BAIRRO = dataTableReader["EN_BAIRRO"].ToString(),
                            EN_CIDADE = dataTableReader["EN_CIDADE"].ToString(),
                            EN_ESTADO = dataTableReader["EN_ESTADO"].ToString(),
                            EN_CEP = dataTableReader["EN_CEP"].ToString(),
                            TX_TELEFONE = dataTableReader["TX_TELEFONE"].ToString(),
                            TX_FAX = dataTableReader["TX_FAX"].ToString(),
                            TX_EMAIL = dataTableReader["TX_EMAIL"].ToString(),
                            TP_TECNICO = dataTableReader["TP_TECNICO"].ToString(),
                            VL_CUSTO_HORA = Convert.ToDecimal("0" + dataTableReader["VL_CUSTO_HORA"]).ToString("N2"),
                            FL_ATIVO = dataTableReader["FL_ATIVO"].ToString(),
                            FL_FERIAS = dataTableReader["FL_FERIAS"].ToString(),
                            cdsFL_ATIVO = ControlesUtility.Dicionarios.SimNao().Where(x => x.Value == dataTableReader["FL_ATIVO"].ToString()).ToArray()[0].Key,
                            cdsFL_FERIAS = ControlesUtility.Dicionarios.SimNao().Where(x => x.Value == dataTableReader["FL_FERIAS"].ToString()).ToArray()[0].Key,
                            usuarioCoordenador = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO_COORDENADOR"]),
                                cnmNome = dataTableReader["cnmNomeCoordenador"].ToString() + " (" + dataTableReader["cdsLoginCoordenador"].ToString() + ")"
                            },
                            empresa = new EmpresaEntity
                            {
                                CD_Empresa = Convert.ToInt64("0" + dataTableReader["CD_Empresa"]),
                                NM_Empresa = dataTableReader["NM_Empresa"].ToString()
                            }
                        };

                        if (dataTableReader["ID_USUARIO"] != DBNull.Value)
                        {
                            Tecnico.usuario = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO"]),
                                cnmNome = dataTableReader["cnmNome"].ToString() + " (" + dataTableReader["cdsLogin"].ToString() + ")"
                            };
                        }

                        tecnicos.Add(Tecnico);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Tecnico> iTecnicos = tecnicos;
            return View(iTecnicos);
        }

        [_3MAuthentication]
        public ActionResult EstoqueIntermediario(string idKey)
        {
            if (string.IsNullOrEmpty(idKey))
                return HttpNotFound();

            Models.Tecnico tecnico = null;
            List<Models.Peca> pecas = new List<Models.Peca>();

            try
            {
                TecnicoEntity tecnicoEntity = new TecnicoEntity();

                tecnicoEntity.CD_TECNICO = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        tecnico = new Models.Tecnico
                        {
                            CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                            NM_TECNICO = dataTableReader["NM_TECNICO"].ToString()
                        };
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (tecnico == null)
                    return HttpNotFound();

                PecaEntity pecaEntity = new PecaEntity();
                dataTableReader = new PecaData().ObterListaPecaByPedidoByTecnico(tecnicoEntity.CD_TECNICO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Peca Peca = new Models.Peca
                        {
                            CD_PECA = dataTableReader["CD_PECA"].ToString(),
                            DS_PECA = dataTableReader["DS_PECA"].ToString(),
                            TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                            QTD_ESTOQUE = Convert.ToDecimal("0" + dataTableReader["QTD_PECA"]).ToString("N3"),
                            QTD_ESTOQUE_GRID = Convert.ToDecimal("0" + dataTableReader["QTD_PECA"].ToString())
                            //QTD_ESTOQUE = Convert.ToDecimal("0" + dataTableReader["QTD_ALOCADA"]).ToString("N2"),
                            //QTD_MINIMA = Convert.ToDecimal("0" + dataTableReader["QTD_MINIMA"]).ToString("N2"),
                            //VL_PECA = Convert.ToDecimal("0" + dataTableReader["VL_PECA"]).ToString("N2"),
                            //TP_PECA = dataTableReader["TP_PECA"].ToString(),
                            //FL_ATIVO_PECA = dataTableReader["FL_ATIVO_PECA"].ToString()
                        };
                        pecas.Add(Peca);
                    }
                }

                ViewBag.idKey = idKey;
                ViewBag.NM_TECNICO = tecnico.NM_TECNICO;
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
            Models.Tecnico tecnico = new Models.Tecnico
            {
                tiposTecnicos = ControlesUtility.Dicionarios.TipoTecnico(),
                SimNao = ControlesUtility.Dicionarios.SimNao(),
                usuarios = ObterListaUsuario(),
                usuariosCoordenadores = ObterListaUsuarioCoordenador(),
                empresas = ObterListaEmpresa(),
                FL_FERIAS = ControlesUtility.Dicionarios.SimNao().ToArray()[1].Value,
                //tecnicosTransferenciaCarteira = ObterListaTecnico()
                usuariosSupervisoresTecnico = ObterListaSupervidorTecnico(),
            };

            return View(tecnico);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Tecnico tecnico)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool Validado = true;
                    TecnicoEntity tecnicoEntity = new TecnicoEntity();

                    tecnicoEntity.CD_TECNICO = tecnico.CD_TECNICO;

                    // Verifica se o código já existe na tabela
                    DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            Validado = false;
                            ViewBag.Mensagem = "Código já cadastrado!";
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    if (Validado == true)
                    {
                        tecnicoEntity.NM_TECNICO = tecnico.NM_TECNICO;
                        tecnicoEntity.NM_REDUZIDO = tecnico.NM_REDUZIDO;
                        tecnicoEntity.EN_ENDERECO = tecnico.EN_ENDERECO;
                        tecnicoEntity.EN_BAIRRO = tecnico.EN_BAIRRO;
                        tecnicoEntity.EN_CIDADE = tecnico.EN_CIDADE;
                        tecnicoEntity.EN_ESTADO = tecnico.EN_ESTADO;
                        tecnicoEntity.EN_CEP = tecnico.EN_CEP;
                        tecnicoEntity.TX_TELEFONE = tecnico.TX_TELEFONE;
                        tecnicoEntity.TX_FAX = tecnico.TX_FAX;
                        tecnicoEntity.TX_EMAIL = tecnico.TX_EMAIL;
                        tecnicoEntity.TP_TECNICO = tecnico.TP_TECNICO;
                        tecnicoEntity.VL_CUSTO_HORA = Convert.ToDecimal("0" + tecnico.VL_CUSTO_HORA);
                        tecnicoEntity.FL_ATIVO = tecnico.FL_ATIVO;
                        tecnicoEntity.FL_FERIAS = tecnico.FL_FERIAS;
                        tecnicoEntity.usuarioCoordenador.nidUsuario = tecnico.usuarioCoordenador.nidUsuario;
                        tecnicoEntity.usuario.nidUsuario = tecnico.usuario.nidUsuario;
                        tecnicoEntity.empresa.CD_Empresa = tecnico.empresa.CD_Empresa;
                        tecnicoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                        tecnicoEntity.usuarioSupervisorTecnico.nidUsuario = tecnico.usuariosSupervisorTecnico.nidUsuario;
                        tecnicoEntity.CD_BCPS = tecnico.CD_BCPS;

                        if (tecnico.FL_ATIVO == "" || tecnico.FL_ATIVO == null)
                            tecnico.JavaScriptToRun = "MensagemErroAtivo()";
                        else if (tecnico.FL_FERIAS == "" || tecnico.FL_FERIAS == null)
                            tecnico.JavaScriptToRun = "MensagemErroFerias()";
                        else
                        {
                            new TecnicoData().Inserir(ref tecnicoEntity);
                            tecnico.JavaScriptToRun = "MensagemSucesso()";
                        }
                        //return RedirectToAction("Index");
                    }
                }

                tecnico.tiposTecnicos = ControlesUtility.Dicionarios.TipoTecnico();
                tecnico.SimNao = ControlesUtility.Dicionarios.SimNao();
                tecnico.usuarios = ObterListaUsuario();
                tecnico.usuariosCoordenadores = ObterListaUsuarioCoordenador();
                tecnico.usuariosSupervisoresTecnico = ObterListaSupervidorTecnico();

                tecnico.empresas = ObterListaEmpresa();

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(tecnico); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Tecnico tecnico = null;

            try
            {
                TecnicoEntity tecnicoEntity = new TecnicoEntity();

                tecnicoEntity.CD_TECNICO = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        tecnico = new Models.Tecnico
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_TECNICO"].ToString()),
                            CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                            NM_TECNICO = dataTableReader["NM_TECNICO"].ToString(),
                            NM_REDUZIDO = dataTableReader["NM_REDUZIDO"].ToString(),
                            EN_ENDERECO = dataTableReader["EN_ENDERECO"].ToString(),
                            EN_BAIRRO = dataTableReader["EN_BAIRRO"].ToString(),
                            EN_CIDADE = dataTableReader["EN_CIDADE"].ToString(),
                            EN_ESTADO = dataTableReader["EN_ESTADO"].ToString(),
                            EN_CEP = dataTableReader["EN_CEP"].ToString(),
                            TX_TELEFONE = dataTableReader["TX_TELEFONE"].ToString(),
                            TX_FAX = dataTableReader["TX_FAX"].ToString(),
                            TX_EMAIL = dataTableReader["TX_EMAIL"].ToString(),
                            TP_TECNICO = dataTableReader["TP_TECNICO"].ToString(),
                            VL_CUSTO_HORA = Convert.ToDecimal("0" + dataTableReader["VL_CUSTO_HORA"]).ToString("N2"),
                            FL_ATIVO = dataTableReader["FL_ATIVO"].ToString(),
                            FL_FERIAS = dataTableReader["FL_FERIAS"].ToString(),
                            CD_BCPS = dataTableReader["CD_BCPS"].ToString(),
                            usuariosSupervisorTecnico = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO_TECNICOREGIONAL"]),
                                cnmNome = dataTableReader["cnmNomeTecRegional"].ToString() + " (" + dataTableReader["cdsLoginTecRegional"].ToString() + ")"

                            },

                            usuarioCoordenador = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO_COORDENADOR"]),
                                cnmNome = dataTableReader["cnmNomeCoordenador"].ToString() + " (" + dataTableReader["cdsLoginCoordenador"].ToString() + ")"
                            },
                            usuario = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO"]),
                                cnmNome = dataTableReader["cnmNome"].ToString() + " (" + dataTableReader["cdsLogin"].ToString() + ")"
                            },
                            empresa = new EmpresaEntity
                            {
                                CD_Empresa = Convert.ToInt64("0" + dataTableReader["CD_Empresa"]),
                                NM_Empresa = dataTableReader["NM_Empresa"].ToString()
                            },
                            FL_INATIVARESTOQUE = "N",

                            tiposTecnicos = ControlesUtility.Dicionarios.TipoTecnico(),
                            SimNao = ControlesUtility.Dicionarios.SimNao(),
                            usuarios = ObterListaUsuario(),
                            usuariosCoordenadores = ObterListaUsuarioCoordenador(),
                            usuariosSupervisoresTecnico = ObterListaSupervidorTecnico(),
                            empresas = ObterListaEmpresa(),
                            tecnicosTransferenciaCarteira = ObterListaTecnico()
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

            if (tecnico == null)
                return HttpNotFound();
            else
                return View(tecnico);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.Tecnico tecnico)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TecnicoEntity tecnicoEntity = new TecnicoEntity();

                    tecnicoEntity.CD_TECNICO = tecnico.CD_TECNICO;
                    tecnicoEntity.NM_TECNICO = tecnico.NM_TECNICO;
                    tecnicoEntity.NM_REDUZIDO = tecnico.NM_REDUZIDO;
                    tecnicoEntity.EN_ENDERECO = tecnico.EN_ENDERECO;
                    tecnicoEntity.EN_BAIRRO = tecnico.EN_BAIRRO;
                    tecnicoEntity.EN_CIDADE = tecnico.EN_CIDADE;
                    tecnicoEntity.EN_ESTADO = tecnico.EN_ESTADO;
                    tecnicoEntity.EN_CEP = tecnico.EN_CEP;
                    tecnicoEntity.TX_TELEFONE = tecnico.TX_TELEFONE;
                    tecnicoEntity.TX_FAX = tecnico.TX_FAX;
                    tecnicoEntity.TX_EMAIL = tecnico.TX_EMAIL;
                    tecnicoEntity.TP_TECNICO = tecnico.TP_TECNICO;
                    tecnicoEntity.VL_CUSTO_HORA = Convert.ToDecimal("0" + tecnico.VL_CUSTO_HORA);
                    tecnicoEntity.FL_ATIVO = tecnico.FL_ATIVO;
                    tecnicoEntity.FL_FERIAS = tecnico.FL_FERIAS;
                    tecnicoEntity.usuarioCoordenador.nidUsuario = tecnico.usuarioCoordenador.nidUsuario;
                    tecnicoEntity.usuario.nidUsuario = tecnico.usuario.nidUsuario;
                    tecnicoEntity.empresa.CD_Empresa = tecnico.empresa.CD_Empresa;
                    tecnicoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    tecnicoEntity.usuarioSupervisorTecnico.nidUsuario = tecnico.usuariosSupervisorTecnico.nidUsuario;
                    tecnicoEntity.CD_BCPS = tecnico.CD_BCPS;
                    new TecnicoData().Alterar(tecnicoEntity);

                    if (tecnico.FL_ATIVO == "N" && !string.IsNullOrEmpty(tecnico.tecnicoTransferenciaCarteira.CD_TECNICO))
                    {
                        new TecnicoData().TransferirCarteira(CD_TECNICO_ORIGEM: tecnico.CD_TECNICO, CD_TECNICO_DESTINO: tecnico.tecnicoTransferenciaCarteira.CD_TECNICO, nidUsuarioAtualizacao: CurrentUser.usuario.nidUsuario);
                    }

                    if(tecnico.FL_ATIVO == "N" && tecnico.FL_INATIVARESTOQUE == "S")
                    {
                        EstoqueEntity estoqueEntity = new EstoqueEntity();
                        estoqueEntity.tecnico.CD_TECNICO = tecnico.CD_TECNICO;
                        DataTableReader dataTableReader = new EstoqueData().ObterEstoqueTecnico(estoqueEntity).CreateDataReader();

                        if(dataTableReader.HasRows)
                        {
                            while(dataTableReader.Read())
                            {
                                estoqueEntity = new EstoqueEntity();
                                estoqueEntity.ID_ESTOQUE = Convert.ToInt64(dataTableReader["ID_ESTOQUE"]);
                                estoqueEntity.FL_ATIVO = "N";
                                new EstoqueData().Alterar(estoqueEntity);
                            }
                        }

                        if(dataTableReader!=null)
                        {
                            dataTableReader.Dispose();
                            dataTableReader = null;
                        }
                    }

                    if(tecnico.FL_ATIVO == "N" || tecnico.FL_FERIAS == "S")
                    {
                        // Tecnico Inativo ou Em Férias muda as Agendas onde ele e o primeiro e passá-lo para último
                    }

                    if (tecnico.FL_ATIVO == "N" && tecnico.TP_TECNICO == "T")
                    {
                        //Inativa o usuário técnico empresa terceira

                        var user = new UsuarioData().ObterListaUsuarioSinc(tecnico.usuario.nidUsuario).FirstOrDefault(x => x.nidUsuario == tecnico.usuario.nidUsuario);

                        if(user != null)
                        {
                            user.bidAtivo = false;

                            var usuario = ConvertUser(user);

                            new UsuarioData().Alterar(usuario);
                        }
                        
                    }

                    if (tecnico.FL_ATIVO == "N")
                    {
                        TecnicoClienteEntity tecnicoClienteEntity = new TecnicoClienteEntity();
                        new TecnicoClienteData().ReordenarTodos(tecnico.CD_TECNICO, CurrentUser.usuario.nidUsuario);
                    }

                    tecnico.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }

                tecnico.tiposTecnicos = ControlesUtility.Dicionarios.TipoTecnico();
                tecnico.SimNao = ControlesUtility.Dicionarios.SimNao();
                tecnico.usuarios = ObterListaUsuario();
                tecnico.usuariosCoordenadores = ObterListaUsuarioCoordenador();
                tecnico.usuariosSupervisoresTecnico = ObterListaSupervidorTecnico();
                tecnico.empresas = ObterListaEmpresa();
                tecnico.tecnicosTransferenciaCarteira = ObterListaTecnico();

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(tecnico); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        public UsuarioEntity ConvertUser(UsuarioSinc user)
        {
            UsuarioEntity usuario = new UsuarioEntity();

            usuario.nidUsuario = user.nidUsuario;
            usuario.cnmNome = user.cnmNome;
            usuario.bidAtivo = user.bidAtivo;
            usuario.cdsEmail = user.cdsEmail;
            usuario.cdsLogin = user.cdsLogin;
            usuario.cdsSenha = user.cdsSenha;
            usuario.ccdChaveAcessoTrocarSenha = user.ccdChaveAcessoTrocarSenha;
            usuario.dtmDataHoraAtualizacao = DateTime.Now;
            usuario.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

            return usuario;
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Tecnico tecnico = null;

            try
            {
                TecnicoEntity tecnicoEntity = new TecnicoEntity();

                tecnicoEntity.CD_TECNICO = ControlesUtility.Criptografia.Descriptografar(idKey);
                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        tecnico = new Models.Tecnico
                        {
                            CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                            NM_TECNICO = dataTableReader["NM_TECNICO"].ToString(),
                            NM_REDUZIDO = dataTableReader["NM_REDUZIDO"].ToString(),
                            EN_ENDERECO = dataTableReader["EN_ENDERECO"].ToString(),
                            EN_BAIRRO = dataTableReader["EN_BAIRRO"].ToString(),
                            EN_CIDADE = dataTableReader["EN_CIDADE"].ToString(),
                            EN_ESTADO = dataTableReader["EN_ESTADO"].ToString(),
                            EN_CEP = dataTableReader["EN_CEP"].ToString(),
                            TX_TELEFONE = dataTableReader["TX_TELEFONE"].ToString(),
                            TX_FAX = dataTableReader["TX_FAX"].ToString(),
                            TX_EMAIL = dataTableReader["TX_EMAIL"].ToString(),
                            TP_TECNICO = dataTableReader["TP_TECNICO"].ToString(),
                            cdsTP_TECNICO = ControlesUtility.Dicionarios.TipoTecnico().Where(x => x.Value == dataTableReader["TP_TECNICO"].ToString()).ToArray()[0].Key,
                            VL_CUSTO_HORA = Convert.ToDecimal("0" + dataTableReader["VL_CUSTO_HORA"]).ToString("N2"),
                            FL_ATIVO = dataTableReader["FL_ATIVO"].ToString(),
                            FL_FERIAS = dataTableReader["FL_FERIAS"].ToString(),
                            CD_BCPS = dataTableReader["CD_BCPS"].ToString(),
                            cdsFL_ATIVO = ControlesUtility.Dicionarios.SimNao().Where(x => x.Value == dataTableReader["FL_ATIVO"].ToString()).ToArray()[0].Key,
                            cdsFL_FERIAS = ControlesUtility.Dicionarios.SimNao().Where(x => x.Value == dataTableReader["FL_FERIAS"].ToString()).ToArray()[0].Key,
                            usuarioCoordenador = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO_COORDENADOR"]),
                                cnmNome = dataTableReader["cnmNomeCoordenador"].ToString() + " (" + dataTableReader["cdsLoginCoordenador"].ToString() + ")"
                            },
                            usuariosSupervisorTecnico = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO_TECNICOREGIONAL"]),
                                cnmNome = dataTableReader["cnmNomeTecRegional"].ToString() + " (" + dataTableReader["cdsLoginTecRegional"].ToString() + ")"
                            },
                            usuario = new UsuarioEntity
                            {
                                nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO"]),
                                cnmNome = dataTableReader["cnmNome"].ToString() + " (" + dataTableReader["cdsLogin"].ToString() + ")"
                            },
                            empresa = new EmpresaEntity
                            {
                                CD_Empresa = Convert.ToInt64("0" + dataTableReader["CD_Empresa"]),
                                NM_Empresa = dataTableReader["NM_Empresa"].ToString()
                            },
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

            if (tecnico == null)
                return HttpNotFound();
            else
                return View(tecnico);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Tecnico tecnico = new Models.Tecnico();
            try
            {
                if (ModelState.IsValid)
                {
                    TecnicoEntity tecnicoEntity = new TecnicoEntity();

                    tecnicoEntity.CD_TECNICO = ControlesUtility.Criptografia.Descriptografar(idKey);
                    tecnicoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new TecnicoData().Excluir(tecnicoEntity);

                    tecnico.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("reference constraint"))
                {
                    tecnico.JavaScriptToRun = "MensagemInativacao()";
                }
                else
                {
                    LogUtility.LogarErro(ex);
                    throw ex;
                }
            }
            return View(tecnico);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        protected List<TecnicoEntity> ObterListaTecnico()
        {
            List<TecnicoEntity> tecnicos = new List<TecnicoEntity>();
            tecnicos.Add(new TecnicoEntity { CD_TECNICO = string.Empty, NM_TECNICO = "Selecione..." });

            try
            {
                TecnicoEntity tecnicoEntity = new TecnicoEntity();
                tecnicoEntity.FL_ATIVO = "S";
                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        tecnicoEntity = new TecnicoEntity();
                        tecnicoEntity.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
                        tecnicoEntity.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                        tecnicos.Add(tecnicoEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return tecnicos;
        }

    }
}