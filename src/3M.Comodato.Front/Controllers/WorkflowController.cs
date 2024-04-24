using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class WorkflowController : BaseController
    {
        #region Permissão de Acessos

        private bool PermissaoSolicitarEnvio => ControlesUtility.Enumeradores.Perfil.EquipeVendas.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                    ControlesUtility.Enumeradores.Perfil.GerenteRegionaldeVendas.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                    ControlesUtility.Enumeradores.Perfil.GerenteNacionaldeVendas.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                    ControlesUtility.Enumeradores.Perfil.Administrador3M.ToInt() == (int)CurrentUser.perfil.nidPerfil;

        private bool PermissaoSolicitarDevolucao =>
                    ControlesUtility.Enumeradores.Perfil.EquipeVendas.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                    ControlesUtility.Enumeradores.Perfil.GerenteRegionaldeVendas.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                    ControlesUtility.Enumeradores.Perfil.GerenteNacionaldeVendas.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                    ControlesUtility.Enumeradores.Perfil.AssistênciaTecnica3M.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                    ControlesUtility.Enumeradores.Perfil.Administrador3M.ToInt() == (int)CurrentUser.perfil.nidPerfil;

        private Acesso ValidarAcessoPedido(WfPedidoEquipEntity entity)
        {
            if ((CurrentUser.perfil.nidPerfil == (int)ControlesUtility.Enumeradores.Perfil.Administrador3M))
                return Acesso.Edicao | Acesso.EdicaoStatus;

            long nidUsuario = CurrentUser.usuario.nidUsuario;
            switch (entity.ST_STATUS_PEDIDO)
            {
                case (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.Rascunho:
                case (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.Rascunho:
                case (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.PendenteAnexar:
                case (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.PendenteAnexar:
                    if (entity.ID_USU_EMITENTE == nidUsuario || entity.ID_USU_SOLICITANTE == nidUsuario)
                        return Acesso.Edicao;
                    else
                        return Acesso.Visualizacao;

                default:
                    if (entity.ID_USUARIO_RESPONS == nidUsuario)
                        return Acesso.EdicaoStatus;

                    else if (!string.IsNullOrEmpty(entity.CD_GRUPO_RESPONS))
                    {
                        WfGrupoUsuData grupoUsuarioData = new WfGrupoUsuData();
                        DataTable dtGrupoUsuario = grupoUsuarioData.ObterLista(new WfGrupoUsuEntity() { usuario = CurrentUser.usuario });
                        DataRow[] rows = dtGrupoUsuario.Select($"CD_GRUPOWF='{entity.CD_GRUPO_RESPONS}' AND TP_GRUPOWF='{entity.TP_PEDIDO}'");
                        if (rows.Count() > 0)
                        {
                            return Acesso.EdicaoStatus;
                        }
                    }
                    return Acesso.Visualizacao;
            }
        }

        #endregion

        #region Pesquisa de Pedidos de Equipamento

        [_3MAuthentication]
        public ActionResult Index()
        {
            DateTime periodo = DateTime.Today;

            WfPedidoEquipamentoItemFiltro model = new WfPedidoEquipamentoItemFiltro();
            model.DataCadastroInicio = periodo.AddMonths(-3).ToShortDateString();
            model.DataCadastroFim = periodo.ToShortDateString();
            model.ListaSolicitante = ObterUsuariosSolicitantes();
            model.ListaStatus = new SelectList(ControlesUtility.Dicionarios.TipoPedidoWorkflow(), "value", "key");
            model.ListaTipoSolicitacao = PopularTipoSolicitacao();
            model.VisaoPedidos = 1;

            if (ControlesUtility.Enumeradores.Perfil.GerenteRegionaldeVendas.ToInt() == (int)CurrentUser.perfil.ccdPerfil)
            {
                model.VisaoPedidos = 2;
            }


            return View(model);
        }

        [_3MAuthentication]
        public JsonResult PopularGridSolicitacoes(WfPedidoEquipamentoItemFiltro filtro)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            IEnumerable<WfPedidoEquipamentoItem> listaSolicitacao = null;
            try
            {
                if (string.IsNullOrEmpty(filtro.DataCadastroFim))
                {
                    filtro.DataCadastroFim = DateTime.Today.ToShortDateString();
                }
                if (string.IsNullOrEmpty(filtro.DataCadastroInicio))
                {
                    filtro.DataCadastroInicio = Convert.ToDateTime(filtro.DataCadastroFim).AddMonths(-3).ToShortDateString();
                }

                WfPedidoEquipEntity filtroEntity = new WfPedidoEquipEntity();

                if (!string.IsNullOrEmpty(filtro.CodigoPedido))
                {
                    filtroEntity.CD_WF_PEDIDO_EQUIP = filtro.CodigoPedido;
                }

                if (!string.IsNullOrEmpty(filtro.TituloPedido))
                {
                    filtroEntity.DS_TITULO = filtro.TituloPedido;
                }

                if (!string.IsNullOrEmpty(filtro.TituloPedido))
                {
                    filtroEntity.DS_TITULO = filtro.TituloPedido;
                }

                //if (!string.IsNullOrEmpty(filtro.Solicitante))
                //{
                //    if(workflowCarregarGridMVC != "S")
                //        filtroEntity.ID_USU_SOLICITANTE = Convert.ToInt64(filtro.Solicitante);
                //}

                if (!string.IsNullOrEmpty(filtro.TipoPedido))
                {
                    filtroEntity.TP_PEDIDO = filtro.TipoPedido;
                }

                if (!string.IsNullOrEmpty(filtro.Status))
                {
                    filtroEntity.ST_STATUS_PEDIDO = Convert.ToInt32(filtro.Status);
                }

                if (!string.IsNullOrEmpty(filtro.TipoSolicitacao))
                {
                    filtroEntity.ID_TIPO_SOLICITACAO = Convert.ToInt32(filtro.TipoSolicitacao);
                }

                //filtroEntity.ID_USU_ULT_ATU = CurrentUser.usuario.nidUsuario;
                WfPedidoEquipData data = new WfPedidoEquipData();
                List<WfPedidoEquipEntity> listaEntityFinal = new List<WfPedidoEquipEntity>();
                var listaEntity = data.ObterListaEntity(filtroEntity, Convert.ToDateTime(filtro.DataCadastroInicio), Convert.ToDateTime(filtro.DataCadastroFim), filtro.Solicitante);
                //listaSolicitacao = (from s in listaEntity
                //                    select ConverterParaPedidoEquipamentoPesquisa(s)).ToList();
                WfGrupoUsuEntity WfGrupoUEntity = new WfGrupoUsuEntity();
                WfGrupoUEntity.usuario = CurrentUser.usuario;
                List<WfGrupoUsuEntity> WfGrupoUsu = new List<WfGrupoUsuEntity>();
                WfGrupoUsu = new WfGrupoUsuData().ObterListaEntity(WfGrupoUEntity);

                foreach (var item in listaEntity)
                {
                    //add todos qdo perfil ADM ou Ger. Nacional  e escolheu opção "Todos"
                    if ((CurrentUser.perfil.ccdPerfil == Convert.ToInt32(ControlesUtility.Enumeradores.Perfil.Administrador3M)
                        || CurrentUser.perfil.ccdPerfil == Convert.ToInt32(ControlesUtility.Enumeradores.Perfil.GerenteNacionaldeVendas)
                        || CurrentUser.perfil.ccdPerfil == Convert.ToInt32(ControlesUtility.Enumeradores.Perfil.EquipeMKT)
                        || CurrentUser.perfil.ccdPerfil == Convert.ToInt32(ControlesUtility.Enumeradores.Perfil.Tecnico3M)) 
                        && filtro.VisaoPedidos == 2)   
                    {
                        listaEntityFinal.Add(item);
                    }
                    
                    //add itens que estão em grupos de atuação do usuario se escolheu "Pendentes de minha Ação"
                    else if ( filtro.VisaoPedidos == 1 )
                    {
                        //Verifica quel grupo deste pedido
                        List<WfGrupoEntity> listaGrupos = new List<WfGrupoEntity>();
                        //string itemGrupo;
                        Utility.WorkflowUtility wfUtil = new WorkflowUtility();
                        string categoria = item.ID_CATEGORIA.ToString();

                        listaGrupos = wfUtil.IdentificaGrupo(item.ST_STATUS_PEDIDO, Convert.ToInt32(categoria), item.TP_LOCACAO, item.CD_LINHA, item.CD_MODELO);


                        if (listaGrupos.Count > 0)
                        {
                            string itemGrupo = listaGrupos.First().CD_GRUPOWF.ToString();
                            //Verificar qual grupos o usuario atua 
                            WfGrupoUsu.ForEach(i =>
                            {
                                if (( i.grupoWf.CD_GRUPOWF.Trim() == itemGrupo.Trim()) && !listaEntityFinal.Contains(item))
                                {
                                    listaEntityFinal.Add(item);
                                }
                            });

                        }

                    }



                    //add se onde o User  é emitente ou solicitante qdo perfil vend   EEE  Visão = "Meus Pedidos"
                    else if ((filtro.VisaoPedidos == 3 || filtro.VisaoPedidos == 2)
                         && (item.ID_USU_SOLICITANTE == CurrentUser.usuario.nidUsuario || item.ID_USU_EMITENTE == CurrentUser.usuario.nidUsuario))                        
                    {
                        listaEntityFinal.Add(item);
                    }


                    //add só WF onde o User  é emitente ou solicitante qdo perfil vend.
                    //else if (CurrentUser.perfil.ccdPerfil == Convert.ToInt32(ControlesUtility.Enumeradores.Perfil.EquipeVendas)
                    //    && (item.ID_USU_SOLICITANTE == CurrentUser.usuario.nidUsuario || item.ID_USU_EMITENTE == CurrentUser.usuario.nidUsuario))
                    //{
                    //    listaEntityFinal.Add(item);
                    //}

                    //add só WF onde a Ass. Tec. é emitente
                    //SL00033288
                    //else if (CurrentUser.perfil.ccdPerfil == Convert.ToInt32(ControlesUtility.Enumeradores.Perfil.AssistênciaTecnica3M)
                    //    && item.ID_USU_EMITENTE == CurrentUser.usuario.nidUsuario)
                    else if (CurrentUser.perfil.ccdPerfil == Convert.ToInt32(ControlesUtility.Enumeradores.Perfil.AssistênciaTecnica3M)
                        && (item.ID_USU_EMITENTE == CurrentUser.usuario.nidUsuario || item.ID_USUARIO_RESPONS == CurrentUser.usuario.nidUsuario))
                    {
                        listaEntityFinal.Add(item);
                    }

                    //add só WF onde o vend. é subordinado do GR
                    else if (CurrentUser.perfil.ccdPerfil == Convert.ToInt32(ControlesUtility.Enumeradores.Perfil.GerenteRegionaldeVendas))
                    {
                        DataTableReader dtr = new UsuarioData().ObterListaSubordinados(CurrentUser.usuario.nidUsuario).CreateDataReader();

                        if (dtr.HasRows)
                        {
                            bool adicionar = false;

                            while (dtr.Read())
                            {
                                if (item.ID_USU_EMITENTE == Convert.ToInt64(dtr["nidUsuario"].ToString().Trim())
                                    || item.ID_USU_SOLICITANTE == Convert.ToInt64(dtr["nidUsuario"].ToString().Trim()))
                                {
                                    adicionar = true;
                                }
                            }

                            if (adicionar == true)
                            {
                                listaEntityFinal.Add(item);
                            }
                        }
                    }
                }

                listaSolicitacao = (from s in listaEntityFinal
                                    select ConverterParaPedidoEquipamentoPesquisa(s)).ToList();

                if (filtro.VisaoPedidos == 3)//3, "Meus Pedidos"
                    listaSolicitacao = listaSolicitacao.Where(c => c.Acesso.HasFlag(Acesso.Edicao)).ToList();

                if (filtro.VisaoPedidos == 1)//1, "Pendentes de minha ação"
                    listaSolicitacao = listaSolicitacao.Where(c => c.Acesso.HasFlag(Acesso.EdicaoStatus)).ToList();

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Workflow/_gridMvcPedidos.cshtml", listaSolicitacao));
                jsonResult.Add("perfil", CurrentUser.perfil.ccdPerfil);
                jsonResult.Add("Status", "Success");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;

        }

        private WfPedidoEquipamentoItem ConverterParaPedidoEquipamentoPesquisa(WfPedidoEquipEntity entity)
        {
            WfPedidoEquipamentoItem model = new WfPedidoEquipamentoItem();
            model.ID_WF_PEDIDO_EQUIP = entity.ID_WF_PEDIDO_EQUIP;
            model.idKey = ControlesUtility.Criptografia.Criptografar(entity.ID_WF_PEDIDO_EQUIP.ToString());
            model.CodigoPedido = entity.CD_WF_PEDIDO_EQUIP;
            model.TituloPedido = entity.DS_TITULO;
            model.DataCadastro = entity.DT_PEDIDO;

            model.TipoPedido = ControlesUtility.Dicionarios.TipoPedidoWorkflow().Where(c => c.Value == entity.TP_PEDIDO).Select(c => c.Key).FirstOrDefault();

            if (entity.TP_PEDIDO == "E" && entity.ST_STATUS_PEDIDO != (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.Rascunho
                && entity.ST_STATUS_PEDIDO != (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.PendenteAnexar)
                model.ActionTipoPedido = "AcompanhamentoPedidoEnvio";
            else if (entity.TP_PEDIDO == "D" && entity.ST_STATUS_PEDIDO != (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.Rascunho
                && entity.ST_STATUS_PEDIDO != (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.PendenteAnexar)
                model.ActionTipoPedido = "AcompanhamentoPedidoDevolucao";
            else
                model.ActionTipoPedido = entity.TP_PEDIDO == "E" ? "PedidoEnvio" : "PedidoDevolucao";

            model.CodigoStatusPedido = entity.ST_STATUS_PEDIDO;
            model.Status = entity.DS_STATUS_PEDIDO;
            model.Solicitante = entity.NM_USU_SOLICITANTE;
            if (!string.IsNullOrEmpty(entity.NM_CLIENTE))
                model.Cliente = entity.NM_CLIENTE + "(" + entity.CD_CLIENTE.ToString() + ")" ?? "";//model.Cliente = entity.NM_CLIENTE ?? "";
            else
                model.Cliente = "";
            model.Modelo = entity.DS_MODELO ?? "";

            model.Acesso = this.ValidarAcessoPedido(entity);
            if (model.Acesso.HasFlag(Acesso.Edicao) || model.Acesso.HasFlag(Acesso.EdicaoStatus))
                model.IconeClass = "fas fa-pencil-alt fa-lg";

            else if (model.Acesso.HasFlag(Acesso.Visualizacao))
                model.IconeClass = "fas fa-search fa-lg";

            return model;
        }

        #endregion

        #region Pedido Envio

        [_3MAuthentication]
        public ActionResult PedidoEnvio(string idKey)
        {
            WfPedidoEnvioEquipamento model = new WfPedidoEnvioEquipamento();

            if (CarregarPedidoEnvio(model, idKey) == false)
                return RedirectToAction("Index");

            //ViewBag.Title = "Equipamento - Pedido de Envio";
            //AtribuirIdentificadores();

            //PedidoEnvioEquipamento model = new PedidoEnvioEquipamento(); ;

            //model.ID_USU_EMITENTE = CurrentUser.usuario.nidUsuario;
            //model.NM_EMITENTE = CurrentUser.usuario.cnmNome.ToUpper();

            //model.ListaVendedor = PopularVendedor();
            //model.ListaCategoria = PopularCategoria();
            //model.ListaFita = PopularFita();
            //model.ListaModeloFita = PopularTipoFita();
            //model.ListaLarguraFita = PopularLarguraFita();
            //model.ListaTipoSolicitacao = PopularTipoSolicitacao();
            //model.ListaEtiqueta = PopularEtiqueta();
            //model.ListaModelo = new SelectList(new List<string>() );
            //model.ListaAtivosCliente = new SelectList(new List<string>());
            //model.ListaModeloIdentificador = PopularModelos(Convert.ToInt64(ViewBag.CategoriaIdentificador),"");

            //if (string.IsNullOrEmpty(idKey))
            //{
            //    if (!PermissaoSolicitarEnvio)
            //        return RedirectToAction("Index");

            //    model = NovoPedidoEnvio(model);
            //}
            //else
            //{
            //    model = AlterarPedidoEnvio(idKey, model);
            //}

            return View(model);
        }

        private WfPedidoEnvioEquipamento NovoPedidoEnvio(WfPedidoEnvioEquipamento model)
        {
            WfPedidoEquipData data = new WfPedidoEquipData();
            model.DT_PEDIDO = DateTime.Today.ToShortDateString();
            model.ListaCliente = PopularClientes(null);

            PopularModelSolicitante(model, null);

            WfPedidoEquipEntity entity = new WfPedidoEquipEntity();
            entity.CD_WF_PEDIDO_EQUIP = model.CD_WF_PEDIDO_EQUIP = data.ObterNovoCodigoPedido();
            entity.DT_PEDIDO = Convert.ToDateTime(model.DT_PEDIDO);
            entity.ST_STATUS_PEDIDO = ControlesUtility.Enumeradores.WorkflowPedidoEnvio.Rascunho.ToInt();
            entity.ID_USU_EMITENTE = model.ID_USU_EMITENTE;
            entity.ID_USU_ULT_ATU = CurrentUser.usuario.nidUsuario;
            entity.TP_PEDIDO = "E";
            data.Inserir(ref entity);

            model.ID_WF_PEDIDO_EQUIP = entity.ID_WF_PEDIDO_EQUIP;
            model.ST_STATUS_PEDIDO = entity.ST_STATUS_PEDIDO;
            model.ModoAcesso = Acesso.Edicao;

            return model;
        }

        private WfPedidoEnvioEquipamento AlterarPedidoEnvio(string idKey, WfPedidoEnvioEquipamento model)
        {
            WfPedidoEquipEntity pedidoEntity = new WfPedidoEquipEntity();
            pedidoEntity.ID_WF_PEDIDO_EQUIP = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
            pedidoEntity = new WfPedidoEquipData().ObterListaEntity(pedidoEntity, null, null).FirstOrDefault();
            model.ModoAcesso = ValidarAcessoPedido(pedidoEntity);

            PopularModelSolicitante(model, pedidoEntity);

            model.ID_WF_PEDIDO_EQUIP = pedidoEntity.ID_WF_PEDIDO_EQUIP;
            model.CD_WF_PEDIDO_EQUIP = pedidoEntity.CD_WF_PEDIDO_EQUIP;
            model.DT_PEDIDO = pedidoEntity.DT_PEDIDO.ToShortDateString();
            model.ST_STATUS_PEDIDO = pedidoEntity.ST_STATUS_PEDIDO; //tbWfStatusPedidoEquip
            model.TipoSolicitacao = pedidoEntity.ID_TIPO_SOLICITACAO.ToString();
            model.ID_USU_SOLICITANTE = pedidoEntity.ID_USU_SOLICITANTE;//VENDEDOR
            model.DS_TITULO = pedidoEntity.DS_TITULO;

            #region Informações Cliente

            model.CD_CLIENTE = pedidoEntity.CD_CLIENTE.ToString();
            var clienteEntity = new ClienteEntity() { CD_CLIENTE = pedidoEntity.CD_CLIENTE };

            //Original
            //model.ListaCliente = PopularClientes(clienteEntity);
            //Chamado - SL00033995
            model.ListaCliente = PopularClientes(null);

            PopularModelCliente(model, pedidoEntity, clienteEntity);

            #endregion

            model.Categoria = pedidoEntity.ID_CATEGORIA.ToString();

            if (pedidoEntity.DS_CATEGORIA != null)
            {
                model.DescricaoCategoria = pedidoEntity.DS_CATEGORIA.ToString();
            }
            else
            {
                model.DescricaoCategoria = "";
            }

            model.LinhaProduto = pedidoEntity.DS_LINHA_PRODUTO.ToString();
            model.TipoLocacao = pedidoEntity.TP_LOCACAO;
            model.TipoSolicitacao = pedidoEntity.ID_TIPO_SOLICITACAO.ToString();
            model.Modelo = pedidoEntity.CD_MODELO;

            model.Linha = pedidoEntity.CD_LINHA;
            model.Tensao = pedidoEntity.CD_TENSAO.ToString();

            if (pedidoEntity.VL_VOLUME_ANO > 0)
                model.VolumeAno = pedidoEntity.VL_VOLUME_ANO.ToString("N0");

            model.UnidadeMedida = pedidoEntity.CD_UNIDADE_MEDIDA.ToString();
            model.QuantidadeEquipamento = pedidoEntity.QT_EQUIPAMENTO.ToString();

            if (pedidoEntity.CD_TROCA == "S")
            {
                model.NumeroAtivoTroca = pedidoEntity.CD_ATIVO_FIXO_TROCA;

                AtivoClienteEntity ativoClienteEntity = new AtivoClienteEntity();
                ativoClienteEntity.cliente.CD_CLIENTE = pedidoEntity.CD_CLIENTE;
                DataTable dataTable = new AtivoClienteData().ObterListaEquipamentoAlocado(ativoClienteEntity);
                DataRow[] row = dataTable.Select($"CD_ATIVO_FIXO='{pedidoEntity.CD_ATIVO_FIXO_TROCA}'");
                if (row.Length > 0)
                {
                    model.NumeroNFTroca = row[0]["NR_NOTAFISCAL"].ToString();
                    model.DataNFTroca = row[0]["DT_NOTAFISCAL"].DataString();
                    model.ModeloEquipamentoTroca = row[0]["DS_ATIVO_FIXO"].ToString();
                }
            }

            //Condições de Aplicação
            //if (pedidoEntity.CD_COND_LIMPEZA.HasValue)
            model.CondicaoLimpeza = pedidoEntity.CD_COND_LIMPEZA.ToString();

            //if (pedidoEntity.CD_COND_TEMPERATURA.HasValue)
            model.CondicaoTemperatura = pedidoEntity.CD_COND_TEMPERATURA.ToString();

            //if (pedidoEntity.CD_COND_UMIDADE.HasValue)
            model.CondicaoUmidade = pedidoEntity.CD_COND_UMIDADE.ToString();

            long codigoFechador = Convert.ToInt64(ViewBag.CategoriaFechador);
            if (pedidoEntity.ID_CATEGORIA == codigoFechador)
            {
                if (!string.IsNullOrEmpty(pedidoEntity.CD_MODELO))
                {
                    ModeloEntity modeloEntity = new ModeloEntity() { CD_MODELO = pedidoEntity.CD_MODELO };
                    DataTable dtModelo = new ModeloData().ObterLista(modeloEntity);
                    if (dtModelo.Rows.Count > 0)
                    {
                        DataRow row = dtModelo.Rows[0];
                        if (row["VL_ALTUR_MIN"] != DBNull.Value)
                            model.ModeloAlturaMinima = Convert.ToDecimal(row["VL_ALTUR_MIN"]).ToString("N0");

                        if (row["VL_ALTUR_MAX"] != DBNull.Value)
                            model.ModeloAlturaMaxima = Convert.ToDecimal(row["VL_ALTUR_MAX"]).ToString("N0");

                        if (row["VL_LARG_MIN"] != DBNull.Value)
                            model.ModeloLarguraMinima = Convert.ToDecimal(row["VL_LARG_MIN"]).ToString("N0");

                        if (row["VL_LARG_MAX"] != DBNull.Value)
                            model.ModeloLarguraMaxima = Convert.ToDecimal(row["VL_LARG_MAX"]).ToString("N0");

                        if (row["VL_COMP_MIN"] != DBNull.Value)
                            model.ModeloComprimentoMinimo = Convert.ToDecimal(row["VL_COMP_MIN"]).ToString("N0");

                        if (row["VL_COMP_MAX"] != DBNull.Value)
                            model.ModeloComprimentoMaximo = Convert.ToDecimal(row["VL_COMP_MAX"]).ToString("N0");
                    }
                }

                //FECHADOR - Dados da Caixa
                if (pedidoEntity.VL_ALTURA_MIN > 0)
                    model.ValorAlturaMinima = pedidoEntity.VL_ALTURA_MIN.ToString("N0");

                if (pedidoEntity.VL_ALTURA_MAX > 0)
                    model.ValorAlturaMaxima = pedidoEntity.VL_ALTURA_MAX.ToString("N0");

                if (pedidoEntity.VL_LARGURA_MIN > 0)
                    model.ValorLarguraMinima = pedidoEntity.VL_LARGURA_MIN.ToString("N0");

                if (pedidoEntity.VL_LARGURA_MAX > 0)
                    model.ValorLarguraMaxima = pedidoEntity.VL_LARGURA_MAX.ToString("N0");

                if (pedidoEntity.VL_COMPRIM_MIN > 0)
                    model.ComprimentoMinimo = pedidoEntity.VL_COMPRIM_MIN.ToString("N0");

                if (pedidoEntity.VL_COMPRIM_MAX > 0)
                    model.ComprimentoMaximo = pedidoEntity.VL_COMPRIM_MAX.ToString("N0");

                if (pedidoEntity.VL_PESO_MINIMO.HasValue)
                    model.PesoMinimo = pedidoEntity.VL_PESO_MINIMO.Value.ToString("N2");

                if (pedidoEntity.VL_PESO_MAXIMO.HasValue)
                    model.PesoMaximo = pedidoEntity.VL_PESO_MAXIMO.Value.ToString("N2");

                //FECHADOR - Dados da Fita
                model.Fita = pedidoEntity.CD_TIPO_FITA.ToString();
                model.ModeloFita = pedidoEntity.CD_MODELO_FITA.ToString();
                model.LarguraFita = pedidoEntity.CD_LARGURA_FITA.ToString();
            }

            long codigoIdentificador = Convert.ToInt64(ViewBag.CategoriaIdentificador);
            if (pedidoEntity.ID_CATEGORIA == codigoIdentificador)
            {
                //IDENTIFICADOR - Dados para Aplicadores e Impressoras
                model.TipoProduto = pedidoEntity.CD_TIPO_PRODUTO.ToString();
                model.LocalInstalacao = pedidoEntity.CD_LOCAL_INSTAL.ToString();
                model.VelocidadeLinha = pedidoEntity.CD_VELOCIDADE_LINHA.ToString();
                model.GuiaPosicionamento = pedidoEntity.CD_GUIA_POSICIONAMENTO.ToString();

                model.AplicadorDireito = pedidoEntity.FL_APLIC_DIREITO == "S";
                model.AplicadorEsquerdo = pedidoEntity.FL_APLIC_ESQUERDO == "S";
                model.AplicadorSuperior = pedidoEntity.FL_APLIC_SUPERIOR == "S";

                //IDENTIFICADOR - Dados da Etiqueta
                model.Etiqueta = pedidoEntity.ID_ETIQUETA.ToString();
                model.AlturaEtiqueta = pedidoEntity.VL_ALTURA_ETIQUETA;
                model.LarguraEtiqueta = pedidoEntity.VL_LARGURA_ETIQUETA;

                model.PvaGramaCaixaPVA = pedidoEntity.DS_KITPVA_GRAMACAIXA;
                model.StrechPesoPallet = pedidoEntity.VL_STRETCH_PESOPALLET;
                model.StrechAlturaPallet = pedidoEntity.VL_STRETCH_ALTURAPALLET;
                model.InkjetCaracteresCaixa = pedidoEntity.VL_INKJET_NUMCARACTER;
            }

            long codigoAcessorios = Convert.ToInt64(ViewBag.CategoriaAcessorios);

            model.AplicadorDireito = pedidoEntity.FL_APLIC_DIREITO == "S";
            model.AplicadorEsquerdo = pedidoEntity.FL_APLIC_ESQUERDO == "S";
            model.AplicadorSuperior = pedidoEntity.FL_APLIC_SUPERIOR == "S";

            PopularModelArquivo(ControlesUtility.Constantes.PastaWorkflowUploadEnvio, model, pedidoEntity);

            //Popular dropdown
            model.ListaModelo = PopularModelos(pedidoEntity.ID_CATEGORIA, model.Modelo);

            model.CD_GRUPO_RESPONS = pedidoEntity.CD_GRUPO_RESPONS;
            model.ID_USUARIO_RESPONS = pedidoEntity.ID_USUARIO_RESPONS;
            model.acompanhamentoPedidoEnvio.wfStatusPedidoEquip.ST_STATUS_PEDIDO = pedidoEntity.ST_STATUS_PEDIDO;
            //model.acompanhamentoPedidoEnvio.empresa.CD_Empresa = pedidoEntity.CD_EMPRESA;
            //if (pedidoEntity.DT_RETIRADA_AGENDADA != null)
            //    model.acompanhamentoPedidoEnvio.DT_RETIRADA_AGENDADA = Convert.ToDateTime(pedidoEntity.DT_RETIRADA_AGENDADA).ToString("dd/MM/yyyy");
            //if (pedidoEntity.DT_RETIRADA_REALIZADA != null)
            //    model.acompanhamentoPedidoEnvio.DT_RETIRADA_REALIZADA = Convert.ToDateTime(pedidoEntity.DT_RETIRADA_REALIZADA).ToString("dd/MM/yyyy");
            //if (pedidoEntity.DT_PROGRAMADA_TMS != null)
            //    model.acompanhamentoPedidoEnvio.DT_PROGRAMADA_TMS = Convert.ToDateTime(pedidoEntity.DT_PROGRAMADA_TMS).ToString("dd/MM/yyyy");
            //if (pedidoEntity.DT_DEVOLUCAO_3M != null)
            //    model.acompanhamentoPedidoEnvio.DT_DEVOLUCAO_3M = Convert.ToDateTime(pedidoEntity.DT_DEVOLUCAO_3M).ToString("dd/MM/yyyy");
            //if (pedidoEntity.DT_DEVOLUCAO_PLANEJAMENTO != null)
            //    model.acompanhamentoPedidoEnvio.DT_DEVOLUCAO_PLANEJAMENTO = Convert.ToDateTime(pedidoEntity.DT_DEVOLUCAO_PLANEJAMENTO).ToString("dd/MM/yyyy");

            model.DS_OBSERVACAO = pedidoEntity.DS_OBSERVACAO;

            return model;
        }

        [_3MAuthentication]
        public JsonResult PopularGridAcessorioModeloEnvio(long codigoWorkflow)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            IEnumerable<AcessorioPedidoEnvio> listaAcessorios = null;
            try
            {
                long codigoAcessorios = Convert.ToInt64(ViewBag.CategoriaAcessorios);
                Func<DataRow, AcessorioPedidoEnvio> ConverterParaAcessorioPedido = new Func<DataRow, AcessorioPedidoEnvio>((r) =>
                {
                    AcessorioPedidoEnvio model = new AcessorioPedidoEnvio();

                    model.CodigoAcessorio = Convert.ToInt64(r["ID_WF_ACESSORIO_EQUIP"]);
                    model.CodigoWorkflow = Convert.ToInt64(r["ID_WF_PEDIDO_EQUIP"]);
                    model.Modelo.CD_MODELO = r["CD_MODELO"].ToString();
                    model.Modelo.DS_MODELO = r["DS_MODELO"].ToString();
                    model.Quantidade = Convert.ToInt32(r["QTD_SOLICITADO"]);
                    model.ST_STATUS_PEDIDO = Convert.ToInt64(r["ST_STATUS_PEDIDO"]);

                    return model;
                });


                WfAcessorioPedidoEntity filter = new WfAcessorioPedidoEntity();
                filter.ID_WF_PEDIDO_EQUIP = codigoWorkflow;

                WfAcessorioPedidoData data = new WfAcessorioPedidoData();
                var listaData = data.ObterLista(filter);

                listaAcessorios = (from s in listaData.Rows.Cast<DataRow>()
                                   select ConverterParaAcessorioPedido(s)).ToList();

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Workflow/_gridAcessoriosModeloEnvio.cshtml", listaAcessorios));
                jsonResult.Add("Status", "Success");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;

        }

        public ActionResult AcompanhamentoPedidoEnvio(string idKey)
        {
            WfPedidoEnvioEquipamento model = new WfPedidoEnvioEquipamento();

            if (CarregarPedidoEnvio(model, idKey) == false)
                return RedirectToAction("Index");

            if (CarregarAcompanhamentoPedidoEnvio(model, idKey) == false)
                return RedirectToAction("Index");

            return View(model);
        }

        protected bool CarregarPedidoEnvio(WfPedidoEnvioEquipamento model, string idKey)
        {


            ViewBag.Title = "Equipamento - Pedido de Envio";
            AtribuirIdentificadores();

            model.ID_USU_EMITENTE = CurrentUser.usuario.nidUsuario;
            model.NM_EMITENTE = CurrentUser.usuario.cnmNome.ToUpper();

            model.ListaVendedor = PopularVendedor();
            model.ListaCategoria = PopularCategoria();
            model.ListaSegmento = PopularSegmento();
            model.ListaFita = PopularFita();
            model.ListaModeloFita = PopularTipoFita();
            model.ListaLarguraFita = PopularLarguraFita();
            model.ListaTipoSolicitacao = PopularTipoSolicitacao();
            model.ListaEtiqueta = PopularEtiqueta();
            model.ListaModelo = new SelectList(new List<string>());
            model.ListaAtivosCliente = new SelectList(new List<string>());
            model.ListaModeloIdentificador = PopularModelos(Convert.ToInt64(ViewBag.CategoriaIdentificador), "");

            if (string.IsNullOrEmpty(idKey))
            {
                if (!PermissaoSolicitarEnvio)
                    return false;

                model = NovoPedidoEnvio(model);
            }
            else
            {
                model = AlterarPedidoEnvio(idKey, model);
            }

            return true;
        }

        protected bool CarregarAcompanhamentoPedidoEnvio(WfPedidoEnvioEquipamento model, string idKey)
        {
            WfStatusPedidoEquipEntity tpStatusPedidoEntity = new WfStatusPedidoEquipEntity();

            tpStatusPedidoEntity.TP_PEDIDO = "E";
            var listaEntity = new WfStatusPedidoEquipData().ObterListaEntity(tpStatusPedidoEntity);

            var listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.Rascunho).FirstOrDefault();
            model.acompanhamentoPedidoEnvio.DS_STATUS_NOME_REDUZ_Rascunho = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.PendenteAnexar).FirstOrDefault();
            model.acompanhamentoPedidoEnvio.DS_STATUS_NOME_REDUZ_PendenteAnexar = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.AnaliseMarketing).FirstOrDefault();
            //model.acompanhamentoPedidoEnvio.DS_STATUS_NOME_REDUZ_AnaliseMarketing = listaItem.DS_STATUS_NOME_REDUZ;
            model.acompanhamentoPedidoEnvio.DS_STATUS_NOME_REDUZ_AnaliseMarketing = listaItem.DS_STATUS_DESCRICAO;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.AnaliseAreaTecnica).FirstOrDefault();
            model.acompanhamentoPedidoEnvio.DS_STATUS_NOME_REDUZ_AnaliseAreaTecnica = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.AnaliseEspecial).FirstOrDefault();
            model.acompanhamentoPedidoEnvio.DS_STATUS_NOME_REDUZ_AnaliseEspecial = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.EmCompras).FirstOrDefault();
            model.acompanhamentoPedidoEnvio.DS_STATUS_NOME_REDUZ_EmCompras = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.AnalisePlanejamento).FirstOrDefault();
            model.acompanhamentoPedidoEnvio.DS_STATUS_NOME_REDUZ_AnalisePlanejamento = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.EnviadoCliente).FirstOrDefault();
            model.acompanhamentoPedidoEnvio.DS_STATUS_NOME_REDUZ_EnviadoCliente = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.EntregueCliente).FirstOrDefault();
            model.acompanhamentoPedidoEnvio.DS_STATUS_NOME_REDUZ_EntregueCliente = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.Instalado).FirstOrDefault();
            model.acompanhamentoPedidoEnvio.DS_STATUS_NOME_REDUZ_Instalado = listaItem.DS_STATUS_NOME_REDUZ;

            //listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.DevolvidoPlanejam).FirstOrDefault();
            //model.acompanhamentoPedidoEnvio.DS_STATUS_NOME_REDUZ_DevolvidoPlanejam = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.Cancelado).FirstOrDefault();
            model.acompanhamentoPedidoEnvio.DS_STATUS_NOME_REDUZ_Cancelado = listaItem.DS_STATUS_NOME_REDUZ;

            model.acompanhamentoPedidoEnvio.wfGrupos = new List<WfGrupoEntity>();
            model.acompanhamentoPedidoEnvio.wfGruposUsu = new List<WfGrupoUsuEntity>();
            model.acompanhamentoPedidoEnvio.wfStatusPedidosEquip = new List<WfStatusPedidoEquipEntity>();
            //model.acompanhamentoPedidoEnvio.empresas = ObterListaEmpresa(ControlesUtility.Dicionarios.TipoEmpresa().ToArray()[2].Value, false);

            return true;
        }
        #endregion

        #region Pedido Devolucao

        [_3MAuthentication]
        public ActionResult PedidoDevolucao(string idKey)
        {
            WfPedidoDevolucaoEquipamento model = new WfPedidoDevolucaoEquipamento();

            if (CarregarPedidoDevolucao(model, idKey) == false)
                return RedirectToAction("Index");

            //AtribuirIdentificadores();

            //ViewBag.Title = "Equipamento - Pedido de Devolução";

            //PedidoDevolucaoEquipamento model = new PedidoDevolucaoEquipamento();

            //model.ID_USU_EMITENTE = CurrentUser.usuario.nidUsuario;
            //model.NM_EMITENTE = CurrentUser.usuario.cnmNome.ToUpper();

            //model.ListaVendedor = PopularVendedor();
            //model.ListaAtivosCliente = new SelectList(new List<string>());

            //if (string.IsNullOrEmpty(idKey))
            //{
            //    if (!PermissaoSolicitarDevolucao)
            //        return RedirectToAction("Index");

            //    model = NovoPedidoDevolucao(model);
            //}
            //else
            //{
            //    model = AlterarPedidoDevolucao(idKey, model);
            //}

            return View(model);
        }

        private WfPedidoDevolucaoEquipamento NovoPedidoDevolucao(WfPedidoDevolucaoEquipamento model)
        {
            WfPedidoEquipData data = new WfPedidoEquipData();
            model.DT_PEDIDO = DateTime.Today.ToShortDateString();
            model.ListaCliente = PopularClientes(null);
            PopularModelSolicitante(model, null);

            WfPedidoEquipEntity entity = new WfPedidoEquipEntity();
            entity.CD_WF_PEDIDO_EQUIP = model.CD_WF_PEDIDO_EQUIP = data.ObterNovoCodigoPedido();
            entity.DT_PEDIDO = Convert.ToDateTime(model.DT_PEDIDO);
            entity.ST_STATUS_PEDIDO = ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.Rascunho.ToInt();
            entity.ID_USU_EMITENTE = model.ID_USU_EMITENTE;
            entity.ID_USU_ULT_ATU = CurrentUser.usuario.nidUsuario;
            entity.TP_PEDIDO = "D";
            data.Inserir(ref entity);

            model.PossuiNotaFiscal3M = "N";

            model.ID_WF_PEDIDO_EQUIP = entity.ID_WF_PEDIDO_EQUIP;
            model.ST_STATUS_PEDIDO = entity.ST_STATUS_PEDIDO;
            model.ModoAcesso = Acesso.Edicao;

            return model;
        }

        private WfPedidoDevolucaoEquipamento AlterarPedidoDevolucao(string idKey, WfPedidoDevolucaoEquipamento model)
        {
            WfPedidoEquipEntity pedidoEntity = new WfPedidoEquipEntity();
            pedidoEntity.ID_WF_PEDIDO_EQUIP = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
            pedidoEntity = new WfPedidoEquipData().ObterListaEntity(pedidoEntity, null, null).FirstOrDefault();
            model.ModoAcesso = ValidarAcessoPedido(pedidoEntity);

            PopularModelSolicitante(model, pedidoEntity);

            model.ID_WF_PEDIDO_EQUIP = pedidoEntity.ID_WF_PEDIDO_EQUIP;
            model.CD_WF_PEDIDO_EQUIP = pedidoEntity.CD_WF_PEDIDO_EQUIP;
            model.DT_PEDIDO = pedidoEntity.DT_PEDIDO.ToShortDateString();
            model.ST_STATUS_PEDIDO = pedidoEntity.ST_STATUS_PEDIDO; //tbWfStatusPedidoEquip
            model.ID_USU_SOLICITANTE = pedidoEntity.ID_USU_SOLICITANTE;//VENDEDOR
            model.DS_TITULO = pedidoEntity.DS_TITULO;

            #region Informações Cliente

            model.CD_CLIENTE = pedidoEntity.CD_CLIENTE.ToString();
            var clienteEntity = new ClienteEntity() { CD_CLIENTE = pedidoEntity.CD_CLIENTE };
            model.ListaCliente = PopularClientes(clienteEntity);

            PopularModelCliente(model, pedidoEntity, clienteEntity);

            #endregion

            model.ListaAtivosCliente = PopularAtivosCliente(clienteEntity.CD_CLIENTE);

            model.PossuiNotaFiscal3M = pedidoEntity.FL_COPIA_NF3M;
            model.ValorNotaFiscal3M = pedidoEntity.VL_NOTA_FISCAL_3M > 0 ? pedidoEntity.VL_NOTA_FISCAL_3M.ToString("N2") : "";
            model.MotivoDevolucao = pedidoEntity.CD_MOTIVO_DEVOLUCAO;

            PopularModelArquivo(ControlesUtility.Constantes.PastaWorkflowUploadDevolucao, model, pedidoEntity);

            model.CD_GRUPO_RESPONS = pedidoEntity.CD_GRUPO_RESPONS;
            model.ID_USUARIO_RESPONS = pedidoEntity.ID_USUARIO_RESPONS;
            model.acompanhamentoPedidoDevolucao.wfStatusPedidoEquip.ST_STATUS_PEDIDO = pedidoEntity.ST_STATUS_PEDIDO;
            model.acompanhamentoPedidoDevolucao.empresa.CD_Empresa = pedidoEntity.CD_EMPRESA;
            if (pedidoEntity.DT_RETIRADA_AGENDADA != null)
                model.acompanhamentoPedidoDevolucao.DT_RETIRADA_AGENDADA = Convert.ToDateTime(pedidoEntity.DT_RETIRADA_AGENDADA).ToString("dd/MM/yyyy");
            if (pedidoEntity.DT_RETIRADA_REALIZADA != null)
                model.acompanhamentoPedidoDevolucao.DT_RETIRADA_REALIZADA = Convert.ToDateTime(pedidoEntity.DT_RETIRADA_REALIZADA).ToString("dd/MM/yyyy");
            if (pedidoEntity.DT_PROGRAMADA_TMS != null)
                model.acompanhamentoPedidoDevolucao.DT_PROGRAMADA_TMS = Convert.ToDateTime(pedidoEntity.DT_PROGRAMADA_TMS).ToString("dd/MM/yyyy");
            if (pedidoEntity.DT_DEVOLUCAO_3M != null)
                model.acompanhamentoPedidoDevolucao.DT_DEVOLUCAO_3M = Convert.ToDateTime(pedidoEntity.DT_DEVOLUCAO_3M).ToString("dd/MM/yyyy");
            if (pedidoEntity.DT_DEVOLUCAO_PLANEJAMENTO != null)
                model.acompanhamentoPedidoDevolucao.DT_DEVOLUCAO_PLANEJAMENTO = Convert.ToDateTime(pedidoEntity.DT_DEVOLUCAO_PLANEJAMENTO).ToString("dd/MM/yyyy");

            return model;
        }

        [_3MAuthentication]
        public JsonResult PopularGridEquipamentosDevolucao(long codigoWorkflow)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            IEnumerable<WfEquipamentoItemDevolucao> listaEquipamentoDevolucao = null;
            try
            {
                Func<DataRow, WfEquipamentoItemDevolucao> ConverterParaModel = new Func<DataRow, WfEquipamentoItemDevolucao>((r) =>
                {
                    WfEquipamentoItemDevolucao model = new WfEquipamentoItemDevolucao();

                    model.CodigoEquipamento = Convert.ToInt64(r["ID_WF_PEDIDO_EQUIP_ITEM"]);
                    model.NumeroAtivo = r["CD_ATIVO_FIXO"].ToString();
                    model.Envio = "";//r[""].ToString();
                    model.Modelo = r["DS_MODELO"] == DBNull.Value ? "" : r["DS_MODELO"].ToString();
                    model.CaixaLargura = r["VL_LARG_CAIXA"] == DBNull.Value ? "" : r["VL_LARG_CAIXA"].ToString();
                    model.CaixaAltura = r["VL_ALTUR_CAIXA"] == DBNull.Value ? "" : r["VL_ALTUR_CAIXA"].ToString();
                    model.CaixaComprimento = r["VL_COMP_CAIXA"] == DBNull.Value ? "" : r["VL_COMP_CAIXA"].ToString();
                    model.Peso = r["VL_PESO_CUBADO"] == DBNull.Value ? "" : r["VL_PESO_CUBADO"].ToString();
                    model.NotaFiscal = r["NR_NOTAFISCAL"] == DBNull.Value ? "" : r["NR_NOTAFISCAL"].ToString();
                    model.DS_ARQUIVO = r["DS_ARQUIVO_FOTO"] == DBNull.Value ? "" : r["DS_ARQUIVO_FOTO"].ToString();

                    if (!string.IsNullOrEmpty(model.DS_ARQUIVO))
                    {
                        if (!FileExistsInServer(ControlesUtility.Constantes.PastaWorkflowUploadDevolucao, model.DS_ARQUIVO))
                        {
                            model.DS_ARQUIVO = string.Empty;
                        }
                    }

                    model.ST_STATUS_PEDIDO = Convert.ToInt64(r["ST_STATUS_PEDIDO"]);
                    return model;
                });


                WfPedidoEquipItemEntity filter = new WfPedidoEquipItemEntity();
                filter.ID_WF_PEDIDO_EQUIP = codigoWorkflow;

                WfPedidoEquipItemData data = new WfPedidoEquipItemData();
                var listaData = data.ObterLista(filter);

                listaEquipamentoDevolucao = (from s in listaData.Rows.Cast<DataRow>()
                                             select ConverterParaModel(s)).ToList();

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Workflow/_gridEquipamentoItem.cshtml", listaEquipamentoDevolucao));
                jsonResult.Add("Status", "Success");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;

        }

        

        public ActionResult AcompanhamentoPedidoDevolucao(string idKey)
        {
            WfPedidoDevolucaoEquipamento model = new WfPedidoDevolucaoEquipamento();

            if (CarregarPedidoDevolucao(model, idKey) == false)
                return RedirectToAction("Index");

            if (CarregarAcompanhamentoPedidoDevolucao(model, idKey) == false)
                return RedirectToAction("Index");

            return View(model);
        }

        protected bool CarregarPedidoDevolucao(WfPedidoDevolucaoEquipamento model, string idKey)
        {
            AtribuirIdentificadores();

            ViewBag.Title = "Equipamento - Pedido de Devolução";

            model.ID_USU_EMITENTE = CurrentUser.usuario.nidUsuario;
            model.NM_EMITENTE = CurrentUser.usuario.cnmNome.ToUpper();

            model.ListaVendedor = PopularVendedor();
            model.ListaAtivosCliente = new SelectList(new List<string>());

            if (string.IsNullOrEmpty(idKey))
            {
                if (!PermissaoSolicitarDevolucao)
                    return false;

                model = NovoPedidoDevolucao(model);
            }
            else
            {
                model = AlterarPedidoDevolucao(idKey, model);
            }

            return true;
        }

        protected bool CarregarAcompanhamentoPedidoDevolucao(WfPedidoDevolucaoEquipamento model, string idKey)
        {
            WfStatusPedidoEquipEntity tpStatusPedidoEntity = new WfStatusPedidoEquipEntity();

            tpStatusPedidoEntity.TP_PEDIDO = "D";
            var listaEntity = new WfStatusPedidoEquipData().ObterListaEntity(tpStatusPedidoEntity);

            var listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.Rascunho).FirstOrDefault();
            model.acompanhamentoPedidoDevolucao.DS_STATUS_NOME_REDUZ_Rascunho = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.PendenteAnexar).FirstOrDefault();
            model.acompanhamentoPedidoDevolucao.DS_STATUS_NOME_REDUZ_PendenteAnexar = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.AnaliseLogistica).FirstOrDefault();
            model.acompanhamentoPedidoDevolucao.DS_STATUS_NOME_REDUZ_AnaliseLogistica = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.Solicitado).FirstOrDefault();
            model.acompanhamentoPedidoDevolucao.DS_STATUS_NOME_REDUZ_Solicitado = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.PendenciaCliente).FirstOrDefault();
            model.acompanhamentoPedidoDevolucao.DS_STATUS_NOME_REDUZ_PendenciaCliente = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.RetiradaAgendada).FirstOrDefault();
            model.acompanhamentoPedidoDevolucao.DS_STATUS_NOME_REDUZ_RetiradaAgendada = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.Coletado).FirstOrDefault();
            model.acompanhamentoPedidoDevolucao.DS_STATUS_NOME_REDUZ_Coletado = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.AguardandoProgTMS).FirstOrDefault();
            model.acompanhamentoPedidoDevolucao.DS_STATUS_NOME_REDUZ_AguardandoProgTMS = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.DevolucaoConcluida).FirstOrDefault();
            model.acompanhamentoPedidoDevolucao.DS_STATUS_NOME_REDUZ_DevolucaoConcluida = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.DevolvidoPlanejam).FirstOrDefault();
            model.acompanhamentoPedidoDevolucao.DS_STATUS_NOME_REDUZ_DevolvidoPlanejam = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.Cancelado).FirstOrDefault();
            model.acompanhamentoPedidoDevolucao.DS_STATUS_NOME_REDUZ_Cancelado = listaItem.DS_STATUS_NOME_REDUZ;

            model.acompanhamentoPedidoDevolucao.wfGrupos = new List<WfGrupoEntity>();
            model.acompanhamentoPedidoDevolucao.wfGruposUsu = new List<WfGrupoUsuEntity>();
            model.acompanhamentoPedidoDevolucao.wfStatusPedidosEquip = new List<WfStatusPedidoEquipEntity>();
            model.acompanhamentoPedidoDevolucao.empresas = ObterListaEmpresa(ControlesUtility.Dicionarios.TipoEmpresa().ToArray()[2].Value, false);

            return true;
        }
        #endregion

        private void PopularModelCliente(WfPedidoEquipamento model, WfPedidoEquipEntity pedidoEntity, ClienteEntity clienteEntity)
        {
            if (pedidoEntity.CD_CLIENTE == 0)
                return;

            DataTable dtCliente = new ClienteData().ObterLista(clienteEntity);
            if (dtCliente.Rows.Count > 0)
            {
                if (dtCliente.Rows[0]["NM_CLIENTE"] != DBNull.Value)
                    model.NM_CLIENTE = dtCliente.Rows[0]["NM_CLIENTE"].ToString();

                if (dtCliente.Rows[0]["NR_CNPJ"] != DBNull.Value)
                    model.NR_CNPJ = dtCliente.Rows[0]["NR_CNPJ"].ToString();

                if (dtCliente.Rows[0]["EN_ENDERECO"] != DBNull.Value)
                    model.EN_ENDERECO = dtCliente.Rows[0]["EN_ENDERECO"].ToString();

                if (dtCliente.Rows[0]["EN_BAIRRO"] != DBNull.Value)
                    model.EN_BAIRRO = dtCliente.Rows[0]["EN_BAIRRO"].ToString();

                if (dtCliente.Rows[0]["EN_CEP"] != DBNull.Value)
                    model.EN_CEP = dtCliente.Rows[0]["EN_CEP"].ToString();

                if (dtCliente.Rows[0]["EN_CIDADE"] != DBNull.Value)
                    model.EN_CIDADE = dtCliente.Rows[0]["EN_CIDADE"].ToString();

                if (dtCliente.Rows[0]["EN_ESTADO"] != DBNull.Value)
                    model.EN_ESTADO = dtCliente.Rows[0]["EN_ESTADO"].ToString();

                if (dtCliente.Rows[0]["TX_EMAIL"] != DBNull.Value)
                    model.TX_EMAIL = dtCliente.Rows[0]["TX_EMAIL"].ToString();

                if (dtCliente.Rows[0]["TX_TELEFONE"] != DBNull.Value)
                    model.TX_TELEFONE = dtCliente.Rows[0]["TX_TELEFONE"].ToString();

                if (dtCliente.Rows[0]["ID_SEGMENTO"] != DBNull.Value)
                    model.ID_SEGMENTO = Convert.ToInt64(dtCliente.Rows[0]["ID_SEGMENTO"]);

                if (dtCliente.Rows[0]["DS_SEGMENTO"] != DBNull.Value)
                    model.DS_SEGMENTO = dtCliente.Rows[0]["DS_SEGMENTO"].ToString();
            }

            model.DS_CONTATO_NOME = pedidoEntity.DS_CONTATO_NOME;

            if (!string.IsNullOrEmpty(pedidoEntity.DS_CONTATO_EMAIL))
                model.TX_EMAIL = pedidoEntity.DS_CONTATO_EMAIL;

            if (!string.IsNullOrEmpty(pedidoEntity.DS_CONTATO_TEL_NUM))
                model.TX_TELEFONE = pedidoEntity.DS_CONTATO_TEL_NUM;
        }

        private void PopularModelSolicitante(WfPedidoEquipamento model, WfPedidoEquipEntity pedidoEntity)
        {
            model.ID_USU_EMITENTE = CurrentUser.usuario.nidUsuario;
            model.NM_EMITENTE = CurrentUser.usuario.cnmNome.ToUpper();

            if (pedidoEntity != null)
            {
                UsuarioData usuarioData = new UsuarioData();
                DataTable dtUsuario = usuarioData.ObterLista(new UsuarioEntity() { nidUsuario = pedidoEntity.ID_USU_EMITENTE });
                if (dtUsuario.Rows.Count > 0)
                {
                    model.ID_USU_EMITENTE = Convert.ToInt64(dtUsuario.Rows[0]["nidUsuario"]);
                    model.NM_EMITENTE = dtUsuario.Rows[0]["cnmNome"].ToString().ToUpper();
                }
            }
        }

        private void PopularModelArquivo(string pastaConstante, WfPedidoEquipamento model, WfPedidoEquipEntity pedidoEntity)
        {
            model.DS_ARQUIVO = string.Empty;

            if (!string.IsNullOrEmpty(pedidoEntity.DS_ARQ_ANEXO))
            {
                if (FileExistsInServer(pastaConstante, pedidoEntity.DS_ARQ_ANEXO))
                {
                    model.DS_ARQUIVO = pedidoEntity.DS_ARQ_ANEXO;
                }
            }
        }


        private void AtribuirIdentificadores()
        {
            var codigoCategoriaFechador = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.WorkflowCategoriaFechador);
            var codigoCategoriaIdentificador = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.WorkflowCategoriaIdentificador);
            var codigoCategoriaAcessorios = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.WorkflowCategoriaAcessorios);
            var codigoTipoSolicitacaoTroca = Convert.ToInt32(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.WorkflowSolicitacaoTroca));

            CategoriaData categoriaData = new CategoriaData();
            CategoriaEntity categoriaFechador = categoriaData.ObterListaEntity(new CategoriaEntity() { CD_CATEGORIA = codigoCategoriaFechador }).FirstOrDefault();
            CategoriaEntity categoriaIdentificador = categoriaData.ObterListaEntity(new CategoriaEntity() { CD_CATEGORIA = codigoCategoriaIdentificador }).FirstOrDefault();
            CategoriaEntity categoriaAcessorios = categoriaData.ObterListaEntity(new CategoriaEntity() { CD_CATEGORIA = codigoCategoriaAcessorios }).FirstOrDefault();
            WfTipoSolicitacaoEntity tipoSolicitacao = new WfTipoSolicitacaoData().ObterListaEntity(new WfTipoSolicitacaoEntity() { CD_TIPO_SOLICITACAO = codigoTipoSolicitacaoTroca }).FirstOrDefault();

            ViewBag.CategoriaFechador = categoriaFechador.ID_CATEGORIA;
            ViewBag.CategoriaIdentificador = categoriaIdentificador.ID_CATEGORIA;
            ViewBag.CategoriaAcessorios = categoriaAcessorios.ID_CATEGORIA;
            ViewBag.TipoSolicitacaoTroca = tipoSolicitacao.ID_TIPO_SOLICITACAO;
        }

        public JsonResult ObterListaMensagemJson(Int64 ID_WF_PEDIDO_EQUIP)
        {
            //ViewBag.Administrador3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M));
            //ViewBag.Tecnico3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Tecnico3M));

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<WfPedidoComent> listaMensagens = ObterListaMensagem(ID_WF_PEDIDO_EQUIP);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Workflow/_gridmvcMensagem.cshtml", listaMensagens));
                jsonResult.Add("Status", "Success");
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

        protected List<WfPedidoComent> ObterListaMensagem(Int64 ID_WF_PEDIDO_EQUIP)
        {
            List<WfPedidoComent> listaMensagens = new List<WfPedidoComent>();

            try
            {
                WfPedidoComentEntity mensagemEntity = new WfPedidoComentEntity();
                mensagemEntity.pedidoEquip.ID_WF_PEDIDO_EQUIP = ID_WF_PEDIDO_EQUIP;

                DataTableReader dataTableReader = new WfPedidoComentData().ObterLista(mensagemEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        WfPedidoComent mensagem = new WfPedidoComent
                        {
                            ID_WF_PEDIDO_COMENT = Convert.ToInt64(dataTableReader["ID_WF_PEDIDO_Coment"]),
                            ID_WF_PEDIDO_EQUIP = Convert.ToInt64(dataTableReader["ID_WF_PEDIDO_EQUIP"]),
                            usuario = new UsuarioEntity()
                            {
                                nidUsuario = Convert.ToInt64(dataTableReader["ID_USUARIO"]),
                                cnmNome = dataTableReader["cnmNome"].ToString()
                            },
                            DS_COMENT = dataTableReader["DS_COMENT"].ToString(),
                            DT_REGISTRO = Convert.ToDateTime(dataTableReader["DT_REGISTRO"])
                        };

                        listaMensagens.Add(mensagem);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                WfPedidoEquipLogEntity pedidoEquipLogEntity = new WfPedidoEquipLogEntity();
                pedidoEquipLogEntity.pedidoEquip.ID_WF_PEDIDO_EQUIP = ID_WF_PEDIDO_EQUIP;

                dataTableReader = new WfPedidoEquipLogData().ObterLista(pedidoEquipLogEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        WfPedidoComent mensagem = new WfPedidoComent
                        {
                            ID_WF_PEDIDO_COMENT = Convert.ToInt64(dataTableReader["ID_WF_PEDIDO_EQUIP_LOG"]),
                            ID_WF_PEDIDO_EQUIP = Convert.ToInt64(dataTableReader["ID_WF_PEDIDO_EQUIP"]),
                            usuario = new UsuarioEntity()
                            {
                                nidUsuario = Convert.ToInt64(dataTableReader["ID_USUARIO"]),
                                cnmNome = dataTableReader["cnmNome"].ToString()
                            },
                            DS_COMENT = Convert.ToDateTime(dataTableReader["DT_REGISTRO"]).ToString("dd/MM/yyyy HH:mm") + " - " + dataTableReader["cnmNome"].ToString() + ": " + dataTableReader["DS_COMENTARIO"].ToString(),
                            DT_REGISTRO = Convert.ToDateTime(dataTableReader["DT_REGISTRO"])
                        };

                        listaMensagens.Add(mensagem);
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

            if (listaMensagens.Count() > 0)
                listaMensagens = listaMensagens.OrderByDescending(x => x.DT_REGISTRO).ToList();

            return listaMensagens;
        }

        #region SelectList

        private SelectList PopularTipoSolicitacao()
        {
            WfTipoSolicitacaoEntity entity = new WfTipoSolicitacaoEntity();
            entity.FL_ATIVO = "S";
            var lista = new WfTipoSolicitacaoData().ObterListaEntity(entity);
            return new SelectList(lista, "ID_TIPO_SOLICITACAO", "DS_TIPO_SOLICITACAO");
        }

        private SelectList ObterUsuariosSolicitantes()
        {
            SelectList selectListItems = null;
            try
            {
                UsuarioPerfilEntity perfilFiltro = new UsuarioPerfilEntity();
                perfilFiltro.bidAtivo = true;
                perfilFiltro.perfil = new PerfilEntity() { nidPerfil = (long)ControlesUtility.Enumeradores.Perfil.EquipeVendas };

                UsuarioEntity usuarioLogado = new UsuarioEntity();
                usuarioLogado.nidUsuario = CurrentUser.usuario.nidUsuario;

                UsuarioPerfilData usuarioPerfilData = new UsuarioPerfilData();
                DataTable dtUsuario = usuarioPerfilData.ObterLista(perfilFiltro, usuarioLogado);
                var listaUsuarios = (from r in dtUsuario.Rows.Cast<DataRow>()
                                     select new Usuario()
                                     {
                                         nidUsuario = Convert.ToInt64(r["nidUsuario"]),
                                         cnmNome = r["cnmNome"].ToString()
                                     }).ToList();

                selectListItems = new SelectList(listaUsuarios, "nidUsuario", "cnmNome");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return selectListItems;
        }

        private SelectList PopularVendedor()
        {
            VendedorEntity entity = new VendedorEntity();
            entity.bidAtivo = true;

            var listaVendedores = (from r in new VendedorData().ObterLista(entity).Rows.Cast<DataRow>()
                                   where r["ID_USUARIO"] != DBNull.Value
                                   select new Vendedor()
                                   {
                                       CD_VENDEDOR = Convert.ToInt64(r["CD_VENDEDOR"]),
                                       usuario = new UsuarioEntity() { nidUsuario = Convert.ToInt64(r["ID_USUARIO"]) },
                                       NM_VENDEDOR = r["cnmNome"].ToString()
                                   }).ToList();

            return new SelectList(listaVendedores, "usuario.nidUsuario", "NM_VENDEDOR");
        }

        private SelectList PopularClientes(ClienteEntity entity)
        {
            bool filtrarCliente = entity != null;
            if (entity == null)
            {
                entity = new ClienteEntity();
            }

            List<Cliente> listaClientes = base.ObterListaCliente(entity).Where(c => c.DT_DESATIVACAO == null).ToList();
            if (filtrarCliente)
            {
                return new SelectList(listaClientes, "CD_CLIENTE", "NM_CLIENTE", entity.CD_CLIENTE);
            }
            else
            {
                return new SelectList(listaClientes, "CD_CLIENTE", "NM_CLIENTE");
            }
        }

        private SelectList PopularCategoria()
        {
            var listaCategoria = new CategoriaData().ObterListaEntity();
            return new SelectList(listaCategoria, "ID_CATEGORIA", "DS_CATEGORIA");
        }

        private SelectList PopularTipoFita()
        {
            TipoFitaEntity entity = new TipoFitaEntity();
            var lista = new TipoFitaData().ObterListaEntity(entity);
            return new SelectList(lista, "ID_TIPO_FITA", "DS_CODIGO_TIPO_FITA");
        }

        private SelectList PopularFita()
        {
            FitaEntity entity = new FitaEntity();
            var lista = new FitaData().ObterListaEntity(entity);
            return new SelectList(lista, "ID_FITA", "DS_FITA");
        }

        private SelectList PopularLarguraFita()
        {
            LarguraFitaEntity entity = new LarguraFitaEntity();
            var lista = new LarguraFitaData().ObterListaEntity(entity);
            return new SelectList(lista, "ID_LARGURA_FITA", "DS_LARGURA_FITA");
        }

        private SelectList PopularEtiqueta()
        {
            EtiquetaEntity entity = new EtiquetaEntity();
            var lista = new EtiquetaData().ObterListaEntity(entity);
            return new SelectList(lista, "ID_ETIQUETA", "DS_ETIQUETA");
        }

        private SelectList PopularModelos(long nidCategoria, string selectedValue)
        {
            ModeloEntity entity = new ModeloEntity();
            entity.CATEGORIA.ID_CATEGORIA = nidCategoria;

            DataTable dataTable = new ModeloData().ObterLista(entity);

            var lista = (from r in dataTable.Rows.Cast<DataRow>()
                         select new Modelo() { CD_MODELO = r["CD_MODELO"].ToString(), DS_MODELO = r["DS_MODELO"].ToString() });

            return new SelectList(lista, "CD_MODELO", "DS_MODELO", selectedValue);
        }

        private SelectList PopularAtivosCliente(long codigoCliente)
        {
            AtivoClienteEntity ativoClienteEntity = new AtivoClienteEntity();
            ativoClienteEntity.cliente.CD_CLIENTE = codigoCliente;
            DataTable dataTable = new AtivoClienteData().ObterListaEquipamentoAlocado(ativoClienteEntity);

            var lista = (from r in dataTable.Rows.Cast<DataRow>()
                         where r["DT_DEVOLUCAO"] == DBNull.Value
                         select new ListaAtivoCliente() { CD_ATIVO_FIXO = r["CD_ATIVO_FIXO"].ToString(), DS_ATIVO_FIXO = r["DS_ATIVO_FIXO"].ToString() });

            return new SelectList(lista, "CD_ATIVO_FIXO", "DS_ATIVO_FIXO");
        }

        private SelectList PopularSegmento()
        {
            SegmentoEntity segmento = new SegmentoEntity();
            var listaSegmento = new SegmentoData().ObterLista(segmento);
            return new SelectList(listaSegmento, "id_segmento", "ds_segmento");
        }

        #endregion
    }
}