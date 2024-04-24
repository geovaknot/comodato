using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class RelatorioReclamacaoController : BaseController
    {
        #region Permissão de Acessos



        private bool PermissaoEditarDados => ControlesUtility.Enumeradores.Perfil.Administrador3M.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                    ControlesUtility.Enumeradores.Perfil.AnalistaTecnico3M.ToInt() == (int)CurrentUser.perfil.nidPerfil;


      
        private int PreencheGrupoUsuario(int ST_RR_STATUS)
        {
            
            string cdsGrupo = string.Empty;
            int idGrupo = 0;
            RelatorioReclamacaoData relatorioreclamacaoData = new RelatorioReclamacaoData();
            int currentUsuario = (int)CurrentUser.usuario.nidUsuario;

            if (ST_RR_STATUS == Convert.ToInt32(ControlesUtility.Enumeradores.AcompanamentoRR.TecRegional))
            {
                cdsGrupo = ControlesUtility.Dicionarios.Grupo().Where(c => c.Key == "TR3M").Select(c => c.Value).FirstOrDefault();

            }
            if (ST_RR_STATUS == Convert.ToInt32(ControlesUtility.Enumeradores.AcompanamentoRR.AnaliseTecnica))
            {
                cdsGrupo = ControlesUtility.Dicionarios.Grupo().Where(c => c.Key == "AT3M").Select(c => c.Value).FirstOrDefault();
            }
            if (ST_RR_STATUS == Convert.ToInt32(ControlesUtility.Enumeradores.AcompanamentoRR.EmCompras))
            {
                cdsGrupo = ControlesUtility.Dicionarios.Grupo().Where(c => c.Key == "AT3M").Select(c => c.Value).FirstOrDefault();
            }



            idGrupo = relatorioreclamacaoData.ObterCodigoGrupo(cdsGrupo, currentUsuario);
            return idGrupo;

        }



        private bool VerificaStatusAtual(int ST_RR_STATUS, int IdGrupo)
        {
            string cdsGrupo = string.Empty;
            Boolean habilita = false;
            if (IdGrupo > 0 || ControlesUtility.Enumeradores.Perfil.Administrador3M.ToInt() == (int)CurrentUser.perfil.nidPerfil)
                habilita = true;
            return habilita;
        }



        private bool HabilitarImprimirCompras(int ST_RR_STATUS)
        {
            string cdsGrupo = string.Empty;
            Boolean habilita = false;

            if (ST_RR_STATUS == Convert.ToInt32(ControlesUtility.Enumeradores.AcompanamentoRR.EmCompras))
            {
                habilita = true;
            }
          
            return habilita;
        }


        /// <summary>
        /// Retirao Valor hora dos tempo informado no banco
        /// </summary>
        /// <param name="Vl_Tempo_Atendimento"></param>
        /// <returns></returns>
        private string CalcularHora(int Vl_Tempo_Atendimento)
        {

            return TimeSpan.FromHours(Convert.ToInt32((Vl_Tempo_Atendimento / 60).ToString())).ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Vl_Tempo_Atendimento"></param>
        /// <returns></returns>
        private string CalcularMinuto(int Vl_Tempo_Atendimento, string Vl_Hora)
        {

            decimal hora;
            string minuto = Vl_Tempo_Atendimento.ToString();
            if (Convert.ToInt32(Vl_Hora.Substring(0, 2)) > 0)
            {
                hora = Math.Round(Convert.ToDecimal((Vl_Tempo_Atendimento / (double)60)), 2);
                

                if (hora.ToString().Length > 1)
                {
                    Int64 horaInteira= Convert.ToInt64(Math.Floor(Convert.ToDecimal((Vl_Tempo_Atendimento / (double)60))));
                    //string minuto = Math.Floor(Convert.ToDouble(hora)) ;
                    string minutoconvertido = Math.Round((hora - horaInteira) * 60).ToString();
                    minuto = minutoconvertido.PadRight(2, '0');
                    ////minuto = hora.ToString().Substring(2, 2);
                }
                else
                    minuto = "0";
            }
            return minuto.ToString();
        }

        #endregion

        #region Pesquisa de Pedidos de Equipamento

        [_3MAuthentication]
        public ActionResult Index()
        {
            DateTime periodo = DateTime.Today;

            RelatorioReclamacaoItemFiltro model = new RelatorioReclamacaoItemFiltro();
            model.DataInicio = periodo.AddMonths(-3).ToShortDateString();
            model.DataFim = periodo.ToShortDateString();

            model.ListaTecnico = ObterListaTecnicos();
            // model.ListaStatus = new SelectList(ControlesUtility.Dicionarios.TipoPedidoWorkflow(), "value", "key");
            model.ListaCliente = PopularClientes(null);
            //model.ListaAtivo = PopularAtivosFixo();
            model.ListaPeca = PopularPeca();
            model.ListaSolicitante = ObterUsuariosSolicitantes();
            model.ativos = new List<Models.Ativo>();

            //   model.ListaTipoSolicitacao = PopularTipoSolicitacao();
            //   model.ListaTipoSolicitacao = PopularTipoSolicitacao();
            model.PerfilAnalista = PermissaoEditarDados;
            //    model.ID_USUARIO_RESPONS = PreencheGrupoUsuario();

        

            return View(model);
        }

       
        /// <summary>
        /// pOPULA A GRID DA TELA DE Consulta
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [_3MAuthentication]
        // public JsonResult PopularGridReclamacao(RelatorioReclamacaoItemFiltro relatorioReclamacaoItemFiltro)
        //{
        //    Dictionary<string, object> jsonResult = new Dictionary<string, object>();
        //    IEnumerable<RelatorioReclamacaoItem> listaSolicitacao = null;

        //    try
        //    {
        //        if (string.IsNullOrEmpty(relatorioReclamacaoItemFiltro.DataFim))
        //        {
        //            relatorioReclamacaoItemFiltro.DataFim = DateTime.Today.ToShortDateString();
        //        }
        //        if (string.IsNullOrEmpty(relatorioReclamacaoItemFiltro.DataInicio))
        //        {
        //            relatorioReclamacaoItemFiltro.DataInicio = Convert.ToDateTime(relatorioReclamacaoItemFiltro.DataFim).AddMonths(-3).ToShortDateString();
        //        }

        //        RelatorioReclamacaoEntity filtroEntity = new RelatorioReclamacaoEntity();

        //        if (!string.IsNullOrEmpty(relatorioReclamacaoItemFiltro.Status))
        //        {
        //            filtroEntity.rrStatusEntity.ID_RR_STATUS = Convert.ToInt64(relatorioReclamacaoItemFiltro.Status);
        //        }

        //        if (!string.IsNullOrEmpty(relatorioReclamacaoItemFiltro.Ativo))
        //        {
        //            filtroEntity.ativoFixoEntity.CD_ATIVO_FIXO = relatorioReclamacaoItemFiltro.Ativo;
        //        }

        //        if (!string.IsNullOrEmpty(relatorioReclamacaoItemFiltro.Peca))
        //        {
        //            filtroEntity.pecaEntity.CD_PECA = relatorioReclamacaoItemFiltro.Peca;
        //        }


        //        RelatorioReclamacaoData data = new RelatorioReclamacaoData();
        //        var listaEntity = data.ObterListaEntity(filtroEntity, Convert.ToDateTime(relatorioReclamacaoItemFiltro.DataInicio), Convert.ToDateTime(relatorioReclamacaoItemFiltro.DataFim));
        //        listaSolicitacao = (from s in listaEntity
        //                            select ConverterParaPedidoEquipamentoPesquisa(s)).ToList();


        //        jsonResult.Add("Html", RenderRazorViewToString("~/Views/RelatorioReclamacao/_gridMvcRelatorioReclamacao.cshtml", listaSolicitacao));
        //        jsonResult.Add("Status", "Success");
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogarErro(ex);
        //        throw ex;
        //    }

        //    var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
        //    jsonList.MaxJsonLength = int.MaxValue;
        //    return jsonList;

        //}


        /// <summary>
        /// pOPULA A GRID DA TELA DE Consulta
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        //[_3MAuthentication]
        public JsonResult PopularGridReclamacao(int? Status, string DataFim, string DataInicio, string CD_PECA, string CD_ATIVO, string CD_TECNICO)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            IEnumerable<RelatorioReclamacaoItem> listaSolicitacao = null;

            try
            {
                if (string.IsNullOrEmpty(DataFim))
                {
                    DataFim = DateTime.Today.ToShortDateString();
                }
                if (string.IsNullOrEmpty(DataInicio))
                {
                    DataInicio = Convert.ToDateTime(DataFim).AddMonths(-3).ToShortDateString();
                }

                RelatorioReclamacaoEntity filtroEntity = new RelatorioReclamacaoEntity();

                if (Status >= 0)
                    filtroEntity.ST_STATUS_RR = Convert.ToInt32(Status);

                if (!string.IsNullOrEmpty(CD_PECA))
                    filtroEntity.pecaEntity.CD_PECA = CD_PECA;

                if (!string.IsNullOrEmpty(CD_ATIVO))
                    filtroEntity.ativoFixoEntity.CD_ATIVO_FIXO = CD_ATIVO;

                if (!string.IsNullOrEmpty(CD_TECNICO))
                    filtroEntity.tecnicoEntity.CD_TECNICO = CD_TECNICO;


                RelatorioReclamacaoData data = new RelatorioReclamacaoData();
                //  var listaEntity = data.ObterListaEntity(filtroEntity, Convert.ToDateTime(relatorioReclamacaoItemFiltro.DataInicio), Convert.ToDateTime(relatorioReclamacaoItemFiltro.DataFim));

                var listaEntity = data.ObterListaEntity(filtroEntity, Convert.ToDateTime(DataInicio), Convert.ToDateTime(DataFim));

                listaSolicitacao = (from s in listaEntity
                                    select ConverterParaPedidoEquipamentoPesquisa(s)).ToList();


                jsonResult.Add("Html", RenderRazorViewToString("~/Views/RelatorioReclamacao/_gridMvcRelatorioReclamacao.cshtml", listaSolicitacao));
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

        /// <summary>
        /// Obtem usuário logado
        /// </summary>
        /// <returns></returns>
        private SelectList ObterUsuariosSolicitantes()
        {
            SelectList selectListItems = null;
            try
            {
                UsuarioPerfilEntity perfilFiltro = new UsuarioPerfilEntity();
                perfilFiltro.bidAtivo = true;
                perfilFiltro.perfil = new PerfilEntity() { nidPerfil = (long)ControlesUtility.Enumeradores.Perfil.EquipeVendas };

                UsuarioPerfilData usuarioPerfilData = new UsuarioPerfilData();
                DataTable dtUsuario = usuarioPerfilData.ObterLista(perfilFiltro);
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
        /// <summary>
        /// Passa o entity da reclamacao para Model
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private RelatorioReclamacaoItem ConverterParaPedidoEquipamentoPesquisa(RelatorioReclamacaoEntity entity)
        {
            RelatorioReclamacaoItem model = new RelatorioReclamacaoItem();
            model.ID_RELATORIO_RECLAMACAO = entity.ID_RELATORIO_RECLAMACAO;
            model.idKey = ControlesUtility.Criptografia.Criptografar(entity.ID_RELATORIO_RECLAMACAO.ToString());

            if (entity.DataCadastro.ToString("dd/MM/yyyy") != "01/01/0001")
            model.DataCadastro = entity.DataCadastro.ToShortDateString();

            model.TipoAtendimento = ControlesUtility.Dicionarios.TipoAtendimento().Where(c => c.Key == entity.TipoAtendimento).Select(c => c.Value).FirstOrDefault();

            model.TipoReclamacaoRR = ControlesUtility.Dicionarios.TipoReclamacaoRR().Where(c => c.Key == entity.TipoReclamacaoRR).Select(c => c.Value).FirstOrDefault();

            model.DS_MOTIVO = entity.DS_MOTIVO;
            model.TecSolicitante = entity.TecnicoSolicitante;
            // model.TecRegional = entity.TecnicoRegional;
            model.Cliente = entity.clienteEntity.NM_CLIENTE;
            model.Status = entity.rrStatusEntity.DS_STATUS_NOME_REDUZ;
            model.ST_STATUS_RR = entity.ST_STATUS_RR;
            model.Ativo = entity.ativoFixoEntity.CD_ATIVO_FIXO;
            model.Peca =  entity.pecaEntity.DS_PECA;
            model.TEMPO_ATENDIMENTO = entity.TEMPO_ATENDIMENTO;

           


            //model.VL_Hora_Atendimento = CalcularHora(model.TEMPO_ATENDIMENTO).Substring(0,2);
            //model.VL_Minuto_Atendimento = CalcularMinuto(model.TEMPO_ATENDIMENTO, model.VL_Hora_Atendimento);

            //model.VL_Hora_Atendimento = model.VL_Hora_Atendimento + "," + model.VL_Minuto_Atendimento;
          
            model.CD_GRUPO_RESPONS = entity.CD_GRUPO_RESPONS;
            model.ID_OS = entity.osPadraoEntity.ID_OS;
            //  model.ID_USUARIO_RESPONS = entity.ID_USUARIO_RESPONS;
            model.IconeClass = "fas fa-pencil-alt fa-lg";
            //model.Acesso = this.ValidarAcessoRR(entity);
           // if (model.Acesso.HasFlag(AcessoReclamacao.Edicao) || model.Acesso.HasFlag(AcessoReclamacao.EdicaoStatus))
                model.IconeClass = "fas fa-pencil-alt fa-lg";

            //else if (model.Acesso.HasFlag(AcessoReclamacao.Visualizacao))
            //    model.IconeClass = "fas fa-search fa-lg";

            return model;
        }

        #endregion


        public ActionResult AcompanhamentoReclamacao(string idKey)
        {
            RRAcompanhamentoReclamacao model = new RRAcompanhamentoReclamacao();

           // model.usuarioAnalistaTecnico = VerificaAcessoAnalistaTecnico(CurrentUser.usuario.nidUsuario);
            if (CarregarRelatorioReclamacao(model, idKey) == false)
                return RedirectToAction("Index");

            if (CarregarAcompanhamentoReclamacao(model, idKey) == false)
                return RedirectToAction("Index");

            return View(model);
        }



        //Verificar se o grupo responsavel é o Analista Técnico para habilitar os campos
        protected bool VerificaAcessoAnalistaTecnico(Int64 nidUsuario, string grupoResponsavel, Int32 ID_USUARIO_RESPONS)
        {
            bool retorno = false;
            RRAcompanhamentoReclamacao model = new RRAcompanhamentoReclamacao();
            RelatorioReclamacaoItem rr = new RelatorioReclamacaoItem();

            WfGrupoUsuEntity wfGrupoUsuEntity = new WfGrupoUsuEntity();
            WfGrupoUsuData wfGrupoUsuData = new WfGrupoUsuData();
        
            wfGrupoUsuEntity.usuario.nidUsuario = Convert.ToInt32(nidUsuario);

            wfGrupoUsuEntity.grupoWf.ID_GRUPOWF = ID_USUARIO_RESPONS;
            var consulta = from grupos in wfGrupoUsuData.ObterLista(wfGrupoUsuEntity).AsEnumerable()
                           select  grupos;

            IEnumerable grupo = consulta.Where(c => c.Field<string>("CD_GRUPOWF").Contains(grupoResponsavel));


            foreach (DataRow grupos in grupo)
            {
                //lbDados.Items.Add(aluno.Field<string>("Nome"));
                rr.CD_GRUPO_RESPONS = grupos[0].ToString();
            }
            
            if (rr.CD_GRUPO_RESPONS == grupoResponsavel || ControlesUtility.Enumeradores.Perfil.Administrador3M.ToInt() == nidUsuario)
            {
                retorno = true;
            }
           
            return retorno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="idKey"></param>
        /// <returns></returns>
        protected bool CarregarAcompanhamentoReclamacao(RRAcompanhamentoReclamacao model, string idKey)
        {

            RRStatusEntity rrStatusRREntity = new RRStatusEntity();


            var listaEntity = new RRStatusData().ObterListaEntity(rrStatusRREntity);

            var listaItem = listaEntity.Where(x => x.ST_STATUS_RR == (int)ControlesUtility.Enumeradores.AcompanamentoRR.Novo).FirstOrDefault();
            model.acompanhamentoReclamacao.DS_STATUS_NOME_REDUZ_Novo = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_RR == (int)ControlesUtility.Enumeradores.AcompanamentoRR.TecRegional).FirstOrDefault();
            model.acompanhamentoReclamacao.DS_STATUS_NOME_REDUZ_TecRegional = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_RR == (int)ControlesUtility.Enumeradores.AcompanamentoRR.AnaliseTecnica).FirstOrDefault();
            model.acompanhamentoReclamacao.DS_STATUS_NOME_REDUZ_AnaliseTecnica = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_RR == (int)ControlesUtility.Enumeradores.AcompanamentoRR.EmCompras).FirstOrDefault();
            model.acompanhamentoReclamacao.DS_STATUS_NOME_REDUZ_EmCompras = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_RR == (int)ControlesUtility.Enumeradores.AcompanamentoRR.Finalizado).FirstOrDefault();
            model.acompanhamentoReclamacao.DS_STATUS_NOME_REDUZ_Finalizado = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_RR == (int)ControlesUtility.Enumeradores.AcompanamentoRR.EnviadoTecnicoCampo).FirstOrDefault();
            model.acompanhamentoReclamacao.DS_STATUS_NOME_REDUZ_EnviadoTecnicoCampo = listaItem.DS_STATUS_NOME_REDUZ;

            listaItem = listaEntity.Where(x => x.ST_STATUS_RR == (int)ControlesUtility.Enumeradores.AcompanamentoRR.Reprovado).FirstOrDefault();
            model.acompanhamentoReclamacao.DS_STATUS_NOME_REDUZ_Reprovado = listaItem.DS_STATUS_NOME_REDUZ;


            model.acompanhamentoReclamacao.wfGrupos = new List<WfGrupoEntity>();
            model.acompanhamentoReclamacao.wfGruposUsu = new List<WfGrupoUsuEntity>();
            model.acompanhamentoReclamacao.RRAllStatus = new List<RRStatusEntity>();
            //model.acompanhamentoPedidoEnvio.empresas = ObterListaEmpresa(ControlesUtility.Dicionarios.TipoEmpresa().ToArray()[2].Value, false);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="idKey"></param>
        /// <returns></returns>
        protected bool CarregarRelatorioReclamacao(RRAcompanhamentoReclamacao model, string idKey)
        {
            // AtribuirIdentificadores();

            ViewBag.Title = "Acompanhamento Relatório Reclamacao";

            model.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
            // model.NM_EMITENTE = CurrentUser.usuario.cnmNome.ToUpper();


            model = AlterarRelatorioReclamacao(idKey, model);



            return true;
        }

        private RRAcompanhamentoReclamacao AlterarRelatorioReclamacao(string idKey, RRAcompanhamentoReclamacao model)
        {

            if (PermissaoEditarDados)
                model.PerfilAnalista = true;
            else
                model.PerfilAnalista = false;

            RelatorioReclamacaoEntity rrEntity = new RelatorioReclamacaoEntity();
            rrEntity.ID_RELATORIO_RECLAMACAO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
            rrEntity = new RelatorioReclamacaoData().ObterListaEntity(rrEntity, null, null).FirstOrDefault();
            /// model.ModoAcesso = ValidarAcessoPedido(rrEntity);

            //PopularModelSolicitante(model, rrEntity);

            model.ID_RELATORIO_RECLAMACAO = rrEntity.ID_RELATORIO_RECLAMACAO;
            model.cdsAtivo = rrEntity.ativoFixoEntity.CD_ATIVO_FIXO;
            model.Peca =  rrEntity.pecaEntity.DS_PECA;
            model.DataCadastro = rrEntity.DataCadastro.ToShortDateString();
            model.ST_STATUS_RR = rrEntity.ST_STATUS_RR; //tbWfStatusPedidoEquip
            model.ListaPeca = PopularPeca(rrEntity.CD_PECA);
            model.Cliente = rrEntity.clienteEntity.NM_CLIENTE;
            model.TecSolicitante = rrEntity.TecnicoSolicitante;
            model.DS_MOTIVO = rrEntity.DS_MOTIVO;
            model.DS_Descricao = rrEntity.DS_DESCRICAO;
            model.NM_Fornecedor = rrEntity.NM_Fornecedor;
            model.ID_USUARIO_RESPONS = PreencheGrupoUsuario(Convert.ToInt32(model.ST_STATUS_RR));

            model.DS_ARQUIVO_FOTO = string.Empty;
            model.DS_ARQUIVO_FOTO = rrEntity.DS_ARQUIVO_FOTO;
            model.DS_TIPO_FOTO = rrEntity.DS_TIPO_FOTO;
   

            if (rrEntity.Custo_Peca > 0)
                model.Custo_Peca = Convert.ToDecimal(rrEntity.Custo_Peca);
            else
            {
                if (!string.IsNullOrEmpty(rrEntity.CD_PECA))
                {
                    model.Custo_Peca = Convert.ToDecimal(ListaPecaValor(rrEntity.CD_PECA));
                }
            }
            model.TEMPO_ATENDIMENTO = rrEntity.TEMPO_ATENDIMENTO;
            model.VL_Hora_Atendimento = CalcularHora(model.TEMPO_ATENDIMENTO);
            //TimeSpan.FromHours(Convert.ToInt32((model.TEMPO_ATENDIMENTO / 60).ToString())).ToString();
            // decimal hora = Math.Round(Convert.ToDecimal((model.TEMPO_ATENDIMENTO / (double)60)),2);
            // hora.ToString().Substring(hora.ToString().IndexOf(",") + 1, 2);
            model.VL_Minuto_Atendimento = CalcularMinuto(model.TEMPO_ATENDIMENTO, model.VL_Hora_Atendimento);
           

            model.VL_Minuto_Atendimento = TimeSpan.FromMinutes(Convert.ToDouble(model.VL_Minuto_Atendimento)).ToString().Substring(3,2);

           
            if (!string.IsNullOrEmpty(rrEntity.tecnicoEntity.CD_TECNICO))
            {
                //Recupera o valor hora do técnico para cáculo do Valor Mao de Obra
                string valorTecnico = ListaValorTecnico(rrEntity.tecnicoEntity.CD_TECNICO);

                //É o valor hora do Técnico mais o tempo de atendimento
                model.Vl_Mao_Obra = ((model.TEMPO_ATENDIMENTO / 60) * Convert.ToDecimal(valorTecnico));
            }

            if (!string.IsNullOrEmpty(model.Vl_Mao_Obra.ToString()))
            {
                //O valor mão de Obra mais o custo da Peça
                model.Custo_Total = model.Vl_Mao_Obra + model.Custo_Peca;
            }
            model.TipoAtendimento = ControlesUtility.Dicionarios.TipoAtendimento().Where(c => c.Key == rrEntity.TipoAtendimento).Select(c => c.Value).FirstOrDefault();
            model.TipoReclamacaoRR = ControlesUtility.Dicionarios.TipoReclamacaoRR().Where(c => c.Key == rrEntity.TipoReclamacaoRR).Select(c => c.Value).FirstOrDefault();

            model.CD_GRUPO_RESPONS = rrEntity.CD_GRUPO_RESPONS;
            //model.ID_USUARIO_RESPONS = rrEntity.ID_USUARIO_RESPONS;
            model.acompanhamentoReclamacao.RRStatus.ST_STATUS_RR = Convert.ToInt32(rrEntity.ST_STATUS_RR);


            model.usuarioAnalistaTecnico = VerificaAcessoAnalistaTecnico(CurrentUser.usuario.nidUsuario, model.CD_GRUPO_RESPONS, Convert.ToInt32(model.ID_USUARIO_RESPONS));


            model.HabilitaCampo = VerificaStatusAtual(Convert.ToInt32(model.ST_STATUS_RR), Convert.ToInt32(model.ID_USUARIO_RESPONS));
            model.HabilitaImprimir = HabilitarImprimirCompras(Convert.ToInt32(model.ST_STATUS_RR));
            
                
                return model;
        }


        [_3MAuthentication]
        public ActionResult AcoesRelatorioReclamacao(string idKey)
        {
            RRAcompanhamentoReclamacao model = new RRAcompanhamentoReclamacao();

            //  if (CarregarPedidoDevolucao(model, idKey) == false)
            //    return RedirectToAction("Index");
            return View(model);
        }

        private void PopularModelSolicitante(WfPedidoEquipamento model, WfPedidoEquipEntity rrEntity)
        {
            model.ID_USU_EMITENTE = CurrentUser.usuario.nidUsuario;
            model.NM_EMITENTE = CurrentUser.usuario.cnmNome.ToUpper();

            if (rrEntity != null)
            {
                UsuarioData usuarioData = new UsuarioData();
                DataTable dtUsuario = usuarioData.ObterLista(new UsuarioEntity() { nidUsuario = rrEntity.ID_USU_EMITENTE });
                if (dtUsuario.Rows.Count > 0)
                {
                    model.ID_USU_EMITENTE = Convert.ToInt64(dtUsuario.Rows[0]["nidUsuario"]);
                    model.NM_EMITENTE = dtUsuario.Rows[0]["cnmNome"].ToString().ToUpper();
                }
            }
        }

        ////private void PopularModelArquivo(string pastaConstante, WfPedidoEquipamento model, WfPedidoEquipEntity rrEntity)
        ////{
        ////    model.DS_ARQUIVO = string.Empty;

        ////    if (!string.IsNullOrEmpty(rrEntity.DS_ARQ_ANEXO))
        ////    {
        ////        if (FileExistsInServer(pastaConstante, rrEntity.DS_ARQ_ANEXO))
        ////        {
        ////            model.DS_ARQUIVO = rrEntity.DS_ARQ_ANEXO;
        ////        }
        ////    }
        ////}


        //private void AtribuirIdentificadores()
        //{
        //    //var codigoCategoriaFechador = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.WorkflowCategoriaFechador);
        //    var codigoCategoriaIdentificador = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.WorkflowCategoriaIdentificador);
        //    var codigoTipoSolicitacaoTroca = Convert.ToInt32(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.WorkflowSolicitacaoTroca));

        //    CategoriaData categoriaData = new CategoriaData();
        //   // CategoriaEntity categoriaFechador = categoriaData.ObterListaEntity(new CategoriaEntity() { CD_CATEGORIA = codigoCategoriaFechador }).FirstOrDefault();
        //    CategoriaEntity categoriaIdentificador = categoriaData.ObterListaEntity(new CategoriaEntity() { CD_CATEGORIA = codigoCategoriaIdentificador }).FirstOrDefault();
        //    WfTipoSolicitacaoEntity tipoSolicitacao = new WfTipoSolicitacaoData().ObterListaEntity(new WfTipoSolicitacaoEntity() { CD_TIPO_SOLICITACAO = codigoTipoSolicitacaoTroca }).FirstOrDefault();

        //   // ViewBag.CategoriaFechador = categoriaFechador.ID_CATEGORIA;
        //    ViewBag.CategoriaIdentificador = categoriaIdentificador.ID_CATEGORIA;
        //    ViewBag.TipoSolicitacaoTroca = tipoSolicitacao.ID_TIPO_SOLICITACAO;
        //}
        /// <summary>
        /// Carreaga a Div Observação
        /// </summary>
        /// <param name="ID_RELATORIO_RECLAMACAO"></param>
        /// <returns></returns>
        public JsonResult ObterListaMensagemJson(Int64 ID_RELATORIO_RECLAMACAO)
        {
            //ViewBag.Administrador3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M));
            //ViewBag.Tecnico3M = ChecarPerfilUsuario(Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Tecnico3M));

            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<RRComent> listaMensagens = ObterListaMensagem(ID_RELATORIO_RECLAMACAO);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/RelatorioReclamacao/_gridmvcMensagem.cshtml", listaMensagens));
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
        /// <summary>
        /// Carreaga a Div Observação
        /// </summary>
        /// <param name="ID_WF_PEDIDO_EQUIP"></param>
        /// <returns></returns>
        protected List<RRComent> ObterListaMensagem(Int64 ID_RELATORIO_RECLAMACAO)
        {
            List<RRComent> listaMensagens = new List<RRComent>();

            try
            {
                RRComentEntity mensagemEntity = new RRComentEntity();
                mensagemEntity.relatorioReclamacao.ID_RELATORIO_RECLAMACAO = ID_RELATORIO_RECLAMACAO;

                DataTableReader dataTableReader = new RRComentData().ObterLista(mensagemEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        RRComent mensagem = new RRComent
                        {
                            ID_RR_COMMENT = Convert.ToInt64(dataTableReader["ID_RR_COMMENT"]),
                            ID_RELATORIO_RECLAMACAO = Convert.ToInt64(dataTableReader["ID_RELATORIO_RECLAMACAO"]),
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

                //WfPedidoEquipLogEntity pedidoEquipLogEntity = new WfPedidoEquipLogEntity();
                //pedidoEquipLogEntity.pedidoEquip.ID_WF_PEDIDO_EQUIP = ID_WF_PEDIDO_EQUIP;

                //dataTableReader = new WfPedidoEquipLogData().ObterLista(pedidoEquipLogEntity).CreateDataReader();

                //if (dataTableReader.HasRows)
                //{
                //    while (dataTableReader.Read())
                //    {
                //        WfPedidoComent mensagem = new WfPedidoComent
                //        {
                //            ID_WF_PEDIDO_COMENT = Convert.ToInt64(dataTableReader["ID_WF_PEDIDO_EQUIP_LOG"]),
                //            ID_WF_PEDIDO_EQUIP = Convert.ToInt64(dataTableReader["ID_WF_PEDIDO_EQUIP"]),
                //            usuario = new UsuarioEntity()
                //            {
                //                nidUsuario = Convert.ToInt64(dataTableReader["ID_USUARIO"]),
                //                cnmNome = dataTableReader["cnmNome"].ToString()
                //            },
                //            DS_COMENT = Convert.ToDateTime(dataTableReader["DT_REGISTRO"]).ToString("dd/MM/yyyy HH:mm") + " - " + dataTableReader["cnmNome"].ToString() + ": " + dataTableReader["DS_COMENTARIO"].ToString(),
                //            DT_REGISTRO = Convert.ToDateTime(dataTableReader["DT_REGISTRO"])
                //        };

                //        listaMensagens.Add(mensagem);
                //    }
                //}

                //if (dataTableReader != null)
                //{
                //    dataTableReader.Dispose();
                //    dataTableReader = null;
                //}

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

        /// <summary>
        /// Busca os técnicos em campo que estão abaixo dos técnico logado através do campo ID_USUARIO_TECNICOREGIONAL
        /// Técnico 3m ou Analista Técnico
        /// </summary>
        /// <returns></returns>
        private SelectList ObterListaTecnicos()
        {
            SelectList selectListItems = null;
            try
            {
                TecnicoEntity tecnicoEntity = new TecnicoEntity();

                tecnicoEntity.usuarioSupervisorTecnico.nidUsuario = (int)CurrentUser.usuario.nidUsuario;
                tecnicoEntity.FL_ATIVO = "S";

                DataTable dataTable = new TecnicoData().ObterLista(tecnicoEntity);

                var lista = (from r in dataTable.Rows.Cast<DataRow>()

                             select new Tecnico() { CD_TECNICO = r["CD_TECNICO"].ToString(), NM_TECNICO = r["NM_TECNICO"].ToString() });

                return new SelectList(lista, "CD_TECNICO", "NM_TECNICO");
            }
            catch (Exception ex)
            {

                LogUtility.LogarErro(ex);
                throw ex;
            }


        }


        /// <summary>
        /// Busca o status RR
        /// Técnico 3m ou Analista Técnico
        /// </summary>
        /// <returns></returns>
        private SelectList ObterListaStatus()
        {
            SelectList selectListItems = null;
            try
            {
                RRStatusEntity rrStatusEntity = new RRStatusEntity();


                //DataTableReader dataTableReader = new RRStatusData().ObterLista(rrStatusEntity).CreateDataReader();


                var listaStatus = (from r in new RRStatusData().ObterLista(rrStatusEntity).Rows.Cast<DataRow>()
                                   select new RRStatus() { ID_RR_STATUS = Convert.ToInt32(r["ID_RR_STATUS"]), DS_STATUS_NOME_REDUZ = r["DS_STATUS_NOME_REDUZ"].ToString() });

                return new SelectList(listaStatus, "ID_RR_STATUS", "DS_STATUS_NOME_REDUZ");
            }
            catch (Exception ex)
            {

                LogUtility.LogarErro(ex);
                throw ex;
            }


        }

        /// <summary>
        /// Clientes combo Reclamacao
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Combo Ativos Reclamacao
        /// </summary>
        /// <returns></returns>
        private SelectList PopularAtivosFixo()
        {
            AtivoFixoEntity ativoFixoEntity = new AtivoFixoEntity();
            ativoFixoEntity.FL_STATUS = true;
            DataTable dataTable = new AtivoFixoData().ObterLista(ativoFixoEntity);

            var lista = (from r in dataTable.Rows.Cast<DataRow>()

                         select new Ativo() { CD_ATIVO_FIXO = r["CD_ATIVO_FIXO"].ToString(), DS_ATIVO_FIXO = r["DS_ATIVO_FIXO"].ToString() });

            return new SelectList(lista, "CD_ATIVO_FIXO", "DS_ATIVO_FIXO");
        }

        /// <summary>
        /// Peca Ativo Reclamação
        /// </summary>
        /// <returns></returns>
        private SelectList PopularPeca(string peca = null)
        {
            PecaEntity pecaEntity = new PecaEntity();
            pecaEntity.bidAtivo = true;
            DataTable dataTable = new PecaData().ObterListaNew(pecaEntity);

            var lista = (from r in dataTable.Rows.Cast<DataRow>()

                         select new Peca() { CD_PECA = r["CD_PECA"].ToString(), DS_PECA = r["DS_PECA"].ToString() });

            return new SelectList(lista, "CD_PECA", "DS_PECA");
        }


        public string ListaPecaValor(string CD_PECA)
        {
            string valor = "";
            List<PecaEntity> _lista = new List<PecaEntity>();
            PecaData m = new PecaData();
            foreach (var item in m.ObterPecas(CD_PECA))
            {
                _lista.Add(new PecaEntity()
                {
                    CD_PECA = item.CD_PECA.ToString(),
                    VL_PECA = Convert.ToDecimal(item.VL_PECA.ToString())
                });
            }

            //var retorno = from x in _lista
            //              select new { x.VL_PECA }.ToString();

            valor = _lista[0].VL_PECA.ToString();

            return valor;
        }

        public string ListaValorTecnico(string CD_Tecnico)
        {
            var valorCustoHora = "";
            List<TecnicoEntity> _lista = new List<TecnicoEntity>();
            TecnicoData m = new TecnicoData();
            foreach (var item in m.ObterTecnico(CD_Tecnico))
            {
                _lista.Add(new TecnicoEntity()
                {
                    CD_TECNICO = item.CD_TECNICO.ToString(),
                    VL_CUSTO_HORA = Convert.ToDecimal(item.VL_CUSTO_HORA.ToString())
                });
            }
            if (_lista.Count > 0)
                return valorCustoHora = _lista[0].VL_CUSTO_HORA.ToString();
            else
                return valorCustoHora;

        }

        #endregion
    }
}