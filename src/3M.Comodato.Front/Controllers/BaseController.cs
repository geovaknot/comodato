using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class BaseController : Controller
    {

        protected string idKey => ControlesUtility.Criptografia.Descriptografar(Request.QueryString["IdKey"]);
        protected string[] parametros => idKey.Split('|');

        private UsuarioPerfilEntity _CurrentUser = new UsuarioPerfilEntity();

        public UsuarioPerfilEntity CurrentUser
        {
            get => (_3M.Comodato.Entity.UsuarioPerfilEntity)Session["_CurrentUser"];
            set => Session.Add("_CurrentUser", value);

        }

        protected string RenderRazorViewToString(string viewName, object model)
        {
            if (model != null)
            {
                ViewData.Model = model;
            }
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        protected List<Models.Cliente> ObterListaCliente()
        {
            return this.ObterListaCliente(new ClienteEntity());
        }

        protected List<Models.Cliente> ObterListaCliente(ClienteEntity clienteEntity)
        {
            List<Models.Cliente> clientes = new List<Models.Cliente>();

            clientes.Add(new Models.Cliente { CD_CLIENTE = 0, NM_CLIENTE = "Selecione..." });

            try
            {
                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Cliente cliente = new Models.Cliente
                        {
                            CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                            NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString(),
                            ID_SEGMENTO = dataTableReader["ID_SEGMENTO"].ToString()
                        };
                        clientes.Add(cliente);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return clientes;
        }

        protected List<Models.Cliente> ObterListaClientePorCod(long CD_CLIENTE)
        {
            List<Models.Cliente> clientes = new List<Models.Cliente>();

            ClienteEntity clienteEntity = new ClienteEntity();

            clienteEntity.CD_CLIENTE = CD_CLIENTE;

            try
            {
                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Cliente cliente = new Models.Cliente
                        {
                            CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                            NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString(),
                            ID_SEGMENTO = dataTableReader["ID_SEGMENTO"].ToString()
                        };
                        clientes.Add(cliente);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return clientes;
        }

        protected List<Ativo> ObterListaAtivoFixo(bool? adicionarEmBranco = true)
        {
            List<Ativo> ativos = new List<Models.Ativo>();

            if (adicionarEmBranco == true)
            {
                ativos.Add(new Ativo { CD_ATIVO_FIXO = string.Empty, DS_ATIVO_FIXO = "Selecione..." });
            }

            try
            {
                AtivoFixoEntity ativoFixoEntity = new AtivoFixoEntity();
                DataTableReader dataTableReader = new AtivoFixoData().ObterLista(ativoFixoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Ativo ativo = new Ativo
                        {
                            CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                            modelo = new ModeloEntity
                            {
                                CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                                DS_MODELO = dataTableReader["CD_ATIVO_FIXO"].ToString() + " - " + dataTableReader["DS_MODELO"].ToString()
                            }
                        };
                        ativo.DS_ATIVO_FIXO = ativo.modelo.DS_MODELO + " - " + ativo.TX_ANO_MAQUINA.ToString();
                        ativos.Add(ativo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return ativos;
        }

        protected List<Models.Ativo> ObterListaAtivoFixoPorCod(string CD_ATIVO_FIXO)
        {
            List<Models.Ativo> ativos = new List<Models.Ativo>();


            try
            {
                AtivoFixoEntity ativoFixoEntity = new AtivoFixoEntity();
                ativoFixoEntity.CD_ATIVO_FIXO = CD_ATIVO_FIXO;

                DataTableReader dataTableReader = new AtivoFixoData().ObterLista(ativoFixoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Ativo ativo = new Models.Ativo
                        {
                            CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                            modelo = new ModeloEntity
                            {
                                CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                                DS_MODELO = dataTableReader["CD_ATIVO_FIXO"].ToString() + " - " + dataTableReader["DS_MODELO"].ToString()
                            }
                        };
                        ativo.DS_ATIVO_FIXO = ativo.modelo.DS_MODELO + " - " + ativo.TX_ANO_MAQUINA.ToString();
                        ativos.Add(ativo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return ativos;
        }


        protected List<Models.MotivoDevolucao> ObterListaMotivoDevolucao(bool? adicionarEmBranco = true)
        {
            List<Models.MotivoDevolucao> motivosDevolucoes = new List<Models.MotivoDevolucao>();

            if (adicionarEmBranco == true)
            {
                motivosDevolucoes.Add(new Models.MotivoDevolucao { CD_MOTIVO_DEVOLUCAO = string.Empty, DS_MOTIVO_DEVOLUCAO = "Selecione..." });
            }

            try
            {
                MotivoDevolucaoEntity motivoDevolucaoEntity = new MotivoDevolucaoEntity();
                DataTableReader dataTableReader = new MotivoDevolucaoData().ObterLista(motivoDevolucaoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.MotivoDevolucao motivoDevolucao = new Models.MotivoDevolucao
                        {
                            CD_MOTIVO_DEVOLUCAO = dataTableReader["CD_MOTIVO_DEVOLUCAO"].ToString(),
                            DS_MOTIVO_DEVOLUCAO = dataTableReader["DS_MOTIVO_DEVOLUCAO"].ToString()
                        };
                        motivosDevolucoes.Add(motivoDevolucao);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return motivosDevolucoes;
        }

        protected List<Models.RazaoComodato> ObterListaRazaoComodato()
        {
            List<Models.RazaoComodato> razoesComodatos = new List<Models.RazaoComodato>();

            try
            {
                RazaoComodatoEntity razaoComodatoEntity = new RazaoComodatoEntity();
                DataTableReader dataTableReader = new RazaoComodatoData().ObterLista(razaoComodatoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.RazaoComodato razaoComodato = new Models.RazaoComodato
                        {
                            CD_RAZAO_COMODATO = Convert.ToInt64(dataTableReader["CD_RAZAO"]),
                            DS_RAZAO_COMODATO = dataTableReader["DS_RAZAO"].ToString()
                        };
                        razoesComodatos.Add(razaoComodato);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return razoesComodatos;
        }

        protected List<Models.Tipo> ObterListaTipo()
        {
            List<Models.Tipo> tipos = new List<Models.Tipo>();


            try
            {
                TipoEntity tipoEntity = new TipoEntity();
                DataTableReader dataTableReader = new TipoData().ObterLista(tipoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Tipo tipo = new Models.Tipo
                        {
                            CD_TIPO = Convert.ToInt64(dataTableReader["CD_TIPO"]),
                            DS_TIPO = dataTableReader["DS_TIPO"].ToString()
                        };

                        tipo.FlagSegmentoDI = false;
                        if (dataTableReader["FL_SEGMENTO_DI"] != DBNull.Value)
                        {
                            tipo.FlagSegmentoDI = (bool)dataTableReader["FL_SEGMENTO_DI"];
                        }

                        tipos.Add(tipo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return tipos;
        }

        protected List<Models.Modelo> ObterListaModelo(bool? adicionarEmBranco = true)
        {
            List<Models.Modelo> modelos = new List<Models.Modelo>();

            if (adicionarEmBranco == true)
            {
                modelos.Add(new Models.Modelo { CD_MODELO = string.Empty, DS_MODELO = "Selecione..." });
            }

            try
            {
                ModeloEntity modeloEntity = new ModeloEntity();
                DataTableReader dataTableReader = new ModeloData().ObterLista(modeloEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Modelo modelo = new Models.Modelo
                        {
                            CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                            DS_MODELO = dataTableReader["DS_MODELO"].ToString()
                        };
                        modelos.Add(modelo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return modelos;
        }

        protected List<Models.Modelo> ObterListaModeloCombo(bool? adicionarEmBranco = true)
        {
            List<Models.Modelo> modelos = new List<Models.Modelo>();

            if (adicionarEmBranco == true)
            {
                modelos.Add(new Models.Modelo { CD_MODELO = string.Empty, DS_MODELO = "Selecione..." });
            }

            try
            {
                ModeloEntity modeloEntity = new ModeloEntity();
                DataTableReader dataTableReader = new ModeloData().ObterListaCombo(modeloEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Modelo modelo = new Models.Modelo
                        {
                            CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                            DS_MODELO = dataTableReader["DS_MODELO"].ToString()
                        };
                        modelos.Add(modelo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return modelos;
        }
        protected List<Models.StatusAtivo> ObterListaStatusAtivo()
        {
            List<Models.StatusAtivo> statusAtivos = new List<Models.StatusAtivo>();
            statusAtivos.Add(new Models.StatusAtivo { CD_STATUS_ATIVO = 0, DS_STATUS_ATIVO = "Selecione..." });

            try
            {
                StatusAtivoEntity statusAtivoEntity = new StatusAtivoEntity();
                DataTableReader dataTableReader = new StatusAtivoData().ObterLista(statusAtivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.StatusAtivo statusAtivo = new Models.StatusAtivo
                        {
                            CD_STATUS_ATIVO = Convert.ToInt64(dataTableReader["CD_STATUS_ATIVO"]),
                            DS_STATUS_ATIVO = dataTableReader["DS_STATUS_ATIVO"].ToString()
                        };
                        statusAtivos.Add(statusAtivo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return statusAtivos;
        }

        protected List<Models.SituacaoAtivo> ObterListaSituacaoAtivo()
        {
            List<Models.SituacaoAtivo> situacaoAtivos = new List<Models.SituacaoAtivo>();
            situacaoAtivos.Add(new Models.SituacaoAtivo { CD_SITUACAO_ATIVO = 0, DS_SITUACAO_ATIVO = "Selecione..." });

            try
            {
                SituacaoAtivoEntity situacaoAtivoEntity = new SituacaoAtivoEntity();
                DataTableReader dataTableReader = new SituacaoAtivoData().ObterLista(situacaoAtivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.SituacaoAtivo situacaoAtivo = new Models.SituacaoAtivo
                        {
                            CD_SITUACAO_ATIVO = Convert.ToInt64(dataTableReader["CD_SITUACAO_ATIVO"]),
                            DS_SITUACAO_ATIVO = dataTableReader["DS_SITUACAO_ATIVO"].ToString()
                        };
                        situacaoAtivos.Add(situacaoAtivo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return situacaoAtivos;
        }

        protected List<Models.LinhaProduto> ObterListaLinhaProduto()
        {
            List<Models.LinhaProduto> linhaProdutos = new List<Models.LinhaProduto>();
            linhaProdutos.Add(new Models.LinhaProduto { CD_LINHA_PRODUTO = 0, DS_LINHA_PRODUTO = "Selecione..." });

            try
            {
                LinhaProdutoEntity linhaProdutoEntity = new LinhaProdutoEntity();
                DataTableReader dataTableReader = new LinhaProdutoData().ObterLista(linhaProdutoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.LinhaProduto linhaProduto = new Models.LinhaProduto
                        {
                            CD_LINHA_PRODUTO = Convert.ToInt32(dataTableReader["CD_LINHA_PRODUTO"]),
                            DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString()
                        };
                        linhaProdutos.Add(linhaProduto);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return linhaProdutos;
        }

        protected List<Models.Grupo> ObterListaGrupo(bool? adicionarEmBranco = true)
        {
            List<Models.Grupo> grupos = new List<Models.Grupo>();

            if (adicionarEmBranco == true)
            {
                grupos.Add(new Models.Grupo { CD_GRUPO = string.Empty, DS_GRUPO = string.Empty });
            }

            try
            {
                GrupoEntity grupoEntity = new GrupoEntity();
                DataTableReader dataTableReader = new GrupoData().ObterLista(grupoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Grupo grupo = new Models.Grupo
                        {
                            CD_GRUPO = dataTableReader["CD_GRUPO"].ToString(),
                            DS_GRUPO = dataTableReader["DS_GRUPO"].ToString()
                        };
                        grupos.Add(grupo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return grupos;
        }

        protected List<Models.Vendedor> ObterListaVendedor()
        {
            List<Models.Vendedor> vendedores = new List<Models.Vendedor>();

            vendedores.Add(new Models.Vendedor { CD_VENDEDOR = 0, NM_VENDEDOR = string.Empty });

            try
            {
                VendedorEntity vendedorEntity = new VendedorEntity();
                DataTableReader dataTableReader = new VendedorData().ObterLista(vendedorEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Vendedor vendedor = new Models.Vendedor
                        {
                            CD_VENDEDOR = Convert.ToInt64(dataTableReader["CD_VENDEDOR"]),
                            NM_VENDEDOR = dataTableReader["NM_VENDEDOR"].ToString()
                        };
                        vendedores.Add(vendedor);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return vendedores;
        }

        protected List<Models.Executivo> ObterListaExecutivo()
        {
            List<Models.Executivo> executivos = new List<Models.Executivo>();

            executivos.Add(new Models.Executivo { CD_EXECUTIVO = 0, NM_EXECUTIVO = string.Empty });

            try
            {
                ExecutivoEntity executivoEntity = new ExecutivoEntity();
                DataTableReader dataTableReader = new ExecutivoData().ObterLista(executivoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Executivo executivo = new Models.Executivo
                        {
                            CD_EXECUTIVO = Convert.ToInt64(dataTableReader["CD_EXECUTIVO"]),
                            NM_EXECUTIVO = dataTableReader["NM_EXECUTIVO"].ToString()
                        };
                        executivos.Add(executivo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return executivos;
        }

        protected List<Models.Regiao> ObterListaRegiao()
        {
            List<Models.Regiao> regioes = new List<Models.Regiao>();

            regioes.Add(new Models.Regiao { CD_REGIAO = string.Empty, DS_REGIAO = string.Empty });

            try
            {
                RegiaoEntity regiaoEntity = new RegiaoEntity();
                DataTableReader dataTableReader = new RegiaoData().ObterLista(regiaoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Regiao regiao = new Models.Regiao
                        {
                            CD_REGIAO = dataTableReader["CD_REGIAO"].ToString(),
                            DS_REGIAO = dataTableReader["DS_REGIAO"].ToString()
                        };
                        regioes.Add(regiao);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return regioes;
        }

        protected List<TpStatusOSPadraoEntity> ObterStatusOsPadrao()
        {
            try
            {
                TpStatusOSPadraoEntity statusEntity = new TpStatusOSPadraoEntity();
                IList<TpStatusOSPadraoEntity> listaStatus = new TpStatusOSPadraoData().ObterLista(statusEntity);

                return new List<TpStatusOSPadraoEntity>(listaStatus);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        protected List<TpOSPadraoEntity> ObterTipoOsPadrao()
        {
            try
            {
                TpOSPadraoEntity statusEntity = new TpOSPadraoEntity();
                IList<TpOSPadraoEntity> listaTipos = new TpOSPadraoData().ObterLista(statusEntity);

                return new List<TpOSPadraoEntity>(listaTipos);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        protected List<TpStatusVisitaPadraoEntity> ObterStatusVisitaPadrao()
        {
            try
            {
                TpStatusVisitaPadraoEntity statusEntity = new TpStatusVisitaPadraoEntity();
                IList<TpStatusVisitaPadraoEntity> listaStatus = new TpStatusVisitaPadraoData().ObterLista(statusEntity);

                return new List<TpStatusVisitaPadraoEntity>(listaStatus);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        protected List<TpMotivoVisitaPadraoEntity> ObterMotivoVisitaPadrao()
        {
            try
            {
                TpMotivoVisitaPadraoEntity motivoEntity = new TpMotivoVisitaPadraoEntity();
                IList<TpMotivoVisitaPadraoEntity> listaMotivo = new TpMotivoVisitaPadraoData().ObterLista(motivoEntity);

                return new List<TpMotivoVisitaPadraoEntity>(listaMotivo);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        protected List<Tecnico> ObterListaTecnicosAtivos(long nidUsuario)
        {
            List<Tecnico> tecnicos = new List<Tecnico>();
            tecnicos.Add(new Tecnico { CD_TECNICO = string.Empty, NM_TECNICO = string.Empty });

            try
            {
                TecnicoEntity tecnicoEntity = new TecnicoEntity() { FL_ATIVO = "S" };
                tecnicoEntity.usuario.nidUsuario = nidUsuario;

                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Tecnico tecnico = new Tecnico
                        {
                            CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                            NM_TECNICO = dataTableReader["NM_TECNICO"].ToString(),

                            empresa = new EmpresaEntity
                            {
                                NM_Empresa = dataTableReader["Nm_Empresa"].ToString()
                            }
                        };

                        tecnicos.Add(tecnico);
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

        protected List<ListaAtivoCliente> ObterListaAtivoCliente(Int32 CD_Cliente, bool? SomenteATIVOSsemDTDEVOLUCAO = false)
        {
            List<ListaAtivoCliente> listaAtivosClientes = new List<Models.ListaAtivoCliente>();

            try
            {
                AtivoClienteEntity ativoClienteEntity = new AtivoClienteEntity();
                ativoClienteEntity.cliente.CD_CLIENTE = CD_Cliente;
                DataTableReader dataTableReader = new AtivoClienteData().ObterListaEquipamentoAlocado(ativoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        ListaAtivoCliente ativoCliente = new Models.ListaAtivoCliente
                        {
                            CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                            cdsPrograma = dataTableReader["DS_LINHA_PRODUTO"].ToString(),
                            cdsTipo = dataTableReader["DS_TIPO"].ToString() + " " + dataTableReader["TX_TIPO"].ToString(),
                            DS_ATIVO_FIXO = dataTableReader["DS_ATIVO_FIXO"].ToString(),
                            NR_NOTAFISCAL = dataTableReader["NR_NOTAFISCAL"].ToString(),
                            CD_MOTIVO_DEVOLUCAO = dataTableReader["CD_MOTIVO_DEVOLUCAO"].ToString(),
                            DS_MOTIVO_DEVOLUCAO = dataTableReader["DS_MOTIVO_DEVOLUCAO"].ToString()
                        };

                        if (dataTableReader["DT_INCLUSAO"] != DBNull.Value)
                        {
                            ativoCliente.DT_INCLUSAO = Convert.ToDateTime(dataTableReader["DT_INCLUSAO"]); //.ToString("dd/MM/yyyy");
                        }

                        if (dataTableReader["DT_NOTAFISCAL"] != DBNull.Value)
                        {
                            ativoCliente.DT_NOTAFISCAL = Convert.ToDateTime(dataTableReader["DT_NOTAFISCAL"]); //.ToString("dd/MM/yyyy");
                        }

                        if (dataTableReader["DT_DEVOLUCAO"] != DBNull.Value)
                        {
                            ativoCliente.DT_DEVOLUCAO = Convert.ToDateTime(dataTableReader["DT_DEVOLUCAO"]); //.ToString("dd/MM/yyyy");
                        }

                        if (dataTableReader["DT_ULTIMA_MANUTENCAO"] != DBNull.Value)
                        {
                            ativoCliente.DT_ULTIMA_MANUTENCAO = Convert.ToDateTime(dataTableReader["DT_ULTIMA_MANUTENCAO"]); //.ToString("dd/MM/yyyy");
                        }

                        if (SomenteATIVOSsemDTDEVOLUCAO == true)
                        {
                            if (dataTableReader["DT_DEVOLUCAO"] == DBNull.Value)
                            {
                                listaAtivosClientes.Add(ativoCliente);
                            }
                        }
                        else
                        {
                            listaAtivosClientes.Add(ativoCliente);
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

            return listaAtivosClientes;

        }

        protected List<Models.TipoContato> ObterListaTipoContato()
        {
            List<Models.TipoContato> tiposContatos = new List<Models.TipoContato>();

            try
            {
                TipoContatoEntity tipoContatoEntity = new TipoContatoEntity();
                DataTableReader dataTableReader = new TipoContatoData().ObterLista(tipoContatoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.TipoContato tipoContato = new Models.TipoContato
                        {
                            nidTipoContato = Convert.ToInt64(dataTableReader["nidTipoContato"]),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["nidTipoContato"].ToString()),
                            cdsTipoContato = dataTableReader["cdsTipoContato"].ToString()
                        };
                        tiposContatos.Add(tipoContato);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return tiposContatos;
        }

        protected List<Models.Empresa> ObterListaEmpresa(string FL_Tipo_Empresa = null, bool? adicionarEmBranco = true)
        {
            List<Models.Empresa> empresas = new List<Models.Empresa>();
            if (adicionarEmBranco == true)
            {
                empresas.Add(new Models.Empresa { CD_Empresa = 0, NM_Empresa = "Selecione..." });
            }

            try
            {
                EmpresaEntity empresaEntity = new EmpresaEntity();
                empresaEntity.FL_Tipo_Empresa = FL_Tipo_Empresa;
                DataTableReader dataTableReader = new EmpresaData().ObterLista(empresaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Empresa empresa = new Models.Empresa
                        {
                            CD_Empresa = Convert.ToInt64(dataTableReader["CD_Empresa"]),
                            NM_Empresa = dataTableReader["NM_Empresa"].ToString(),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_Empresa"].ToString()),
                            //IIComp = dataTableReader["IIComp"].ToString(),
                            NR_Cnpj = dataTableReader["NR_CNPJ"].ToString(),
                            EN_Endereco = dataTableReader["EN_ENDERECO"].ToString(),
                            EN_Bairro = dataTableReader["EN_BAIRRO"].ToString(),
                            EN_Cidade = dataTableReader["EN_CIDADE"].ToString(),
                            EN_Estado = dataTableReader["EN_ESTADO"].ToString(),
                            EN_Cep = dataTableReader["EN_CEP"].ToString(),
                            TX_Telefone = dataTableReader["TX_TELEFONE"].ToString(),
                            TX_Fax = dataTableReader["TX_FAX"].ToString(),
                            FL_Tipo_Empresa = dataTableReader["FL_TIPO_EMPRESA"].ToString()
                        };
                        empresas.Add(empresa);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return empresas;
        }

        protected List<Models.GrupoModelo> ObterListaGruposModelos()
        {
            List<Models.GrupoModelo> gruposModelos = new List<Models.GrupoModelo>();
            gruposModelos.Add(new Models.GrupoModelo { ID_GRUPO_MODELO = 0, CD_GRUPO_MODELO = string.Empty });

            try
            {
                GrupoModeloEntity gruposModelosEntity = new GrupoModeloEntity();
                DataTableReader dataTableReader = new GrupoModeloData().ObterLista(gruposModelosEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.GrupoModelo grupoModelo = new Models.GrupoModelo
                        {
                            ID_GRUPO_MODELO = Convert.ToInt32(dataTableReader["ID_GRUPOMODELO"].ToString()),
                            CD_GRUPO_MODELO = dataTableReader["CD_GRUPOMODELO"].ToString(),
                            DS_GRUPO_MODELO = dataTableReader["DS_GRUPOMODELO"].ToString()
                        };
                        gruposModelos.Add(grupoModelo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return gruposModelos;
        }

        protected List<Models.Perfil> ObterListaPerfil()
        {
            List<Models.Perfil> perfis = new List<Models.Perfil>();

            try
            {
                PerfilEntity perfilEntity = new PerfilEntity();
                DataTableReader dataTableReader = new PerfilData().ObterLista(perfilEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Perfil perfil = new Models.Perfil
                        {
                            nidPerfil = Convert.ToInt64(dataTableReader["nidPerfil"]),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["nidPerfil"].ToString()),
                            cdsPerfil = dataTableReader["cdsPerfil"].ToString(),
                            bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]),
                            cdsAtivo = (Convert.ToBoolean(dataTableReader["bidAtivo"]) == true ? "Sim" : "Não")
                        };
                        perfis.Add(perfil);
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

            return perfis;
        }

        protected List<Models.Funcao> ObterListaFuncao()
        {
            List<Models.Funcao> funcoes = new List<Models.Funcao>();

            try
            {
                FuncaoEntity funcaoEntity = new FuncaoEntity();
                DataTableReader dataTableReader = new FuncaoData().ObterLista(funcaoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Funcao funcao = new Models.Funcao
                        {
                            nidFuncao = Convert.ToInt64(dataTableReader["nidFuncao"]),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["nidFuncao"].ToString()),
                            cdsFuncao = dataTableReader["ccdFuncao"].ToString() + " - " + dataTableReader["cdsFuncao"].ToString(),
                            bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]),
                            cdsAtivo = (Convert.ToBoolean(dataTableReader["bidAtivo"]) == true ? "Sim" : "Não")
                        };
                        funcoes.Add(funcao);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return funcoes;
        }

        protected List<Models.Usuario> ObterListaUsuario()
        {
            List<Models.Usuario> usuarios = new List<Models.Usuario>();
            usuarios.Add(new Models.Usuario { nidUsuario = 0, cnmNome = string.Empty });

            try
            {
                UsuarioEntity usuarioEntity = new UsuarioEntity();
                DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Usuario usuario = new Models.Usuario
                        {
                            nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]),
                            cnmNome = dataTableReader["cnmNome"].ToString() + " (" + dataTableReader["cdsLogin"].ToString() + ")"
                        };
                        usuarios.Add(usuario);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return usuarios;
        }

        protected List<Models.Usuario> ObterListaUsuarioCoordenador()
        {
            List<Models.Usuario> usuarios = new List<Models.Usuario>();
            usuarios.Add(new Models.Usuario { nidUsuario = 0, cnmNome = string.Empty });

            try
            {
                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                usuarioPerfilEntity.perfil.ccdPerfil = Convert.ToInt32(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica);
                DataTableReader dataTableReader = new UsuarioPerfilData().ObterLista(usuarioPerfilEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Usuario usuario = new Models.Usuario
                        {
                            nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]),
                            cnmNome = dataTableReader["cnmNome"].ToString() + " (" + dataTableReader["cdsLogin"].ToString() + ")"
                        };
                        usuarios.Add(usuario);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return usuarios;
        }

        protected List<Models.Usuario> ObterListaGerenteRegional()
        {
            List<Models.Usuario> usuarios = new List<Models.Usuario>();
            usuarios.Add(new Models.Usuario { nidUsuario = 0, cnmNome = string.Empty });

            try
            {
                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                usuarioPerfilEntity.perfil.ccdPerfil = Convert.ToInt32(ControlesUtility.Enumeradores.Perfil.GerenteRegionaldeVendas);
                DataTableReader dataTableReader = new UsuarioPerfilData().ObterLista(usuarioPerfilEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Usuario usuario = new Models.Usuario
                        {
                            nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]),
                            cnmNome = dataTableReader["cnmNome"].ToString() + " (" + dataTableReader["cdsLogin"].ToString() + ")"
                        };
                        usuarios.Add(usuario);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return usuarios;
        }

        protected List<Models.Categoria> ObterListaCategoria()
        {
            List<Models.Categoria> categorias = new List<Models.Categoria>();
            categorias.Add(new Models.Categoria { ID_CATEGORIA = 0, DS_CATEGORIA = "Selecione..." });

            try
            {
                CategoriaEntity categoriaEntity = new CategoriaEntity();
                DataTableReader dataTableReader = new CategoriaData().ObterLista(categoriaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Categoria categoria = new Models.Categoria
                        {
                            ID_CATEGORIA = Convert.ToInt32(dataTableReader["ID_CATEGORIA"]),
                            DS_CATEGORIA = dataTableReader["DS_CATEGORIA"].ToString(),
                            FL_ATIVO = Convert.ToChar(dataTableReader["FL_ATIVO"]),
                            CD_CATEGORIA = dataTableReader["CD_CATEGORIA"].ToString()
                        };
                        categorias.Add(categoria);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return categorias;
        }


        protected List<Models.WfGrupo> ObterListaWFGrupo()
        {
            List<Models.WfGrupo> grupos = new List<Models.WfGrupo>();
            //grupos.Add(new Models.WFGrupo { ID_GRUPOWF = 0, CD_GRUPOWF = "Selecione..." });

            try
            {
                WfGrupoEntity WFGrupoEntity = new WfGrupoEntity();
                DataTableReader dataTableReader = new WfGrupoData().ObterLista(WFGrupoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.WfGrupo grupo = new Models.WfGrupo
                        {
                            ID_GRUPOWF = Convert.ToInt32(dataTableReader["ID_GRUPOWF"]),
                            CD_GRUPOWF = dataTableReader["CD_GRUPOWF"].ToString(),
                            DS_GRUPOWF = dataTableReader["DS_GRUPOWF"].ToString(),
                            TP_GRUPOWF = dataTableReader["TP_GRUPOWF"].ToString()

                        };
                        grupos.Add(grupo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return grupos;
        }


        protected string ChecarPerfilUsuario(Int64 ccdPerfil)
        {
            //return (CurrentUser.perfil.nidPerfil == ccdPerfil ? "S" : "N");
            //(ALEX) Somente para testes caso não queira que a regra de perfil seja aplicada para acesso aos botões de gravação (exemplo: Agenda Atendimento)
            return "S";
        }


        [HttpPost]
        public JsonResult Upload(string pastaConstante)
        {
            //ControlesUtility.Constantes.PastaWorkflowUploadDevolucao

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            List<string> lista = new List<string>();
            try
            {
                //string diretorio = Server.MapPath(string.Concat(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.CaminhoUpload),
                //    typeof(ControlesUtility.Constantes).GetField(pastaConstante).GetValue(null).ToString()));

                string diretorio = Server.MapPath("~") + string.Concat(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.CaminhoUpload),
                    typeof(ControlesUtility.Constantes).GetField(pastaConstante).GetValue(null).ToString());

                if (HttpContext.Request.Files.AllKeys.Any())
                {
                    if (!Directory.Exists(diretorio))
                        Directory.CreateDirectory(diretorio);

                    for (int i = 0; i < HttpContext.Request.Files.Count; i++)
                    {
                        var file = HttpContext.Request.Files["files" + i];
                        if (file != null)
                        {
                            //string arquivo = String.Concat(Guid.NewGuid().ToString(), GetDefaultExtension(file.ContentType));
                            string[] nome = file.FileName.ToString().Split('.');
                            string extensao = string.Empty;
                            if (nome.Length > 0)
                                extensao = "." + nome[nome.Length - 1];
                            string arquivo = String.Concat(Guid.NewGuid().ToString(), extensao);
                            file.SaveAs(Path.Combine(diretorio, arquivo));
                            lista.Add(arquivo);
                        }
                    }
                }
                jsonResult.Add("file", lista);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                jsonResult.Add("file", ex.Message);
            }
            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }

        protected bool FileExistsInServer(string pastaConstante, string fileName)
        {
            try
            {
                string diretorioBase = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.CaminhoUpload);
                return System.IO.File.Exists(Path.Combine(Server.MapPath(diretorioBase + pastaConstante), fileName));
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        public ActionResult DownloadFile(string pastaConstante, string fileName)
        {
            try
            {
                string diretorio = Server.MapPath("~") + string.Concat(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.CaminhoUpload),
                            typeof(ControlesUtility.Constantes).GetField(pastaConstante).GetValue(null).ToString());

                var filepath = Path.Combine(diretorio, fileName);

                if (!System.IO.File.Exists(filepath))
                    return RedirectToAction("Index", "Error");

                return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        public JsonResult clearFile(string pastaConstante, string fileName, string idAtivo, string arquivo)
        {
            bool result = false;
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            try
            {
                string diretorio = Server.MapPath("~") + string.Concat(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.CaminhoUpload),
                            typeof(ControlesUtility.Constantes).GetField(pastaConstante).GetValue(null).ToString());

                var filepath = Path.Combine(diretorio, fileName);

                if (System.IO.File.Exists(filepath))
                {
                    result = true;
                    if (!string.IsNullOrEmpty(idAtivo))
                    {
                        result = new AtivoClienteData().ExcluirArquivoFoto(idAtivo, arquivo);
                    }

                    System.IO.File.Delete(filepath);
                }
            }
            catch (Exception ex)
            {
                //jsonResult.Add("file", ex.Message);
                LogUtility.LogarErro(ex);
                throw ex;
            }
            jsonResult.Add("confirmacao", result);
            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }


        private string GetDefaultExtension(string mimeType)
        {
            string[] result;
            //RegistryKey key;
            //object value;

            //key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
            //value = key != null ? key.GetValue("Extension", null) : null;
            //result = value != null ? value.ToString() : string.Empty;

            result = mimeType.Split('/');

            return "." + result[1];
        }

        public JsonResult CriptografarChaveJson(string Conteudo)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                string idKey = ControlesUtility.Criptografia.Criptografar(Conteudo);
                jsonResult.Add("idKey", idKey);
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



        /// <summary>
        /// Carrega os Tecnicos  responsaveis pelos Técnico 3M
        /// Alteração para projeto RR
        /// </summary>
        /// <returns></returns>
        protected List<Models.Usuario> ObterListaSupervidorTecnico()
        {
            List<Models.Usuario> usuarios = new List<Models.Usuario>();
            usuarios.Add(new Models.Usuario { nidUsuario = 0, cnmNome = string.Empty });

            try
            {
                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                usuarioPerfilEntity.perfil.ccdPerfil = Convert.ToInt32(ControlesUtility.Enumeradores.Perfil.Tecnico3M);
                usuarioPerfilEntity.usuario.bidAtivo = true;
                DataTableReader dataTableReader = new UsuarioPerfilData().ObterLista(usuarioPerfilEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Usuario usuario = new Models.Usuario
                        {
                            nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]),
                            cnmNome = dataTableReader["cnmNome"].ToString() + " (" + dataTableReader["cdsLogin"].ToString() + ")"
                        };
                        usuarios.Add(usuario);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return usuarios;
        }

        private string RenderViewToString(ControllerContext context, string viewPath, object model = null, bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                            context.Controller.ViewData,
                                            context.Controller.TempData,
                                            sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        protected IEnumerable<VisitaPadrao> OrdernarListaVisita(IEnumerable<VisitaPadrao> lst, string orderby, string ordertype)
        {
            if (string.IsNullOrWhiteSpace(orderby))
                return lst;
            else
            {
                var propertyInfo = typeof(VisitaPadrao).GetProperty(orderby);

                if (string.IsNullOrWhiteSpace(ordertype) || ordertype.ToLower() == "asc")
                    return lst.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
                else
                    return lst.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
            }
        }

        protected IEnumerable<OsPadrao> OrdernarListaOs(IEnumerable<OsPadrao> lst, string orderby, string ordertype)
        {
            if (string.IsNullOrWhiteSpace(orderby))
                return lst;
            else
            {
                var propertyInfo = typeof(OsPadrao).GetProperty(orderby);

                if (string.IsNullOrWhiteSpace(ordertype) || ordertype.ToLower() == "asc")
                    return lst.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
                else
                    return lst.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
            }
        }

        protected List<Cliente> ObterListaClientePorUsuarioPerfil(Int64 nidUsuario, bool? SomenteAtivos = false)
        {
            List<Cliente> clientes = new List<Cliente>();

            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();
                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity, nidUsuario).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        if (SomenteAtivos == true)
                        {
                            if (dataTableReader["DT_DESATIVACAO"] == DBNull.Value)
                            {
                                Models.Cliente cliente = new Models.Cliente
                                {
                                    CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                                    NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString(),
                                    CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                                };
                                clientes.Add(cliente);
                            }
                        }
                        else
                        {
                            Models.Cliente cliente = new Models.Cliente
                            {
                                CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString(),
                                CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                            };
                            clientes.Add(cliente);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return clientes;
        }

    }
}