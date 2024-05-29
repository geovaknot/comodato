using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/PedidoAPI")]
    [Authorize]
    public class PedidoAPIController : BaseAPIController
    {
        #region Constantes

        private const string TIPO_PEDIDO_AVULSO = "A";
        private const string TIPO_PEDIDO_CLIENTE = "C";
        private const string TIPO_PEDIDO_TECNICO = "T";
        private const string STATUS_ITEM_PEDIDO_CANCELADO = "4";

        #endregion


        /// <summary>
        /// Obter lista de tabela tb_PEDIDO para sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario">Id do usuario dono do celular</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObterListaPedidoSinc")]
        public IHttpActionResult ObterListaPedidoSinc(Int64 idUsuario)
        {
            IList<PedidoSinc> listaPedido = new List<PedidoSinc>();
            try
            {
                PedidoData pedidoData = new PedidoData();
                listaPedido = pedidoData.ObterListaPedidoSinc(idUsuario);

                JObject JO = new JObject
                {
                    { "PEDIDO", JArray.FromObject(listaPedido) }
                };

                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Incluir")]
        public HttpResponseMessage Incluir(PedidoEntity pedidoEntity)
        {
            try
            {
                pedidoEntity.DT_CRIACAO = DateTime.UtcNow.AddHours(-3);
                if (pedidoEntity.TP_Especial == "" || pedidoEntity.TP_Especial == null)
                    pedidoEntity.TP_Especial = "N";
                new PedidoData().Inserir(pedidoEntity);

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { pedidoEntity.ID_PEDIDO, pedidoEntity.TOKEN });
        }

        [HttpGet]
        [Route("GerarTodosPedidosTecnico")]
        public IHttpActionResult GerarTodosPedidosTecnico(Int64 idUsuario)
        {
            try
            {

                var statusNovoRascunho = Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.NovoRascunho);
                var statusSolicitado = Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Solicitado);

                TecnicoEntity tecnicoEntity = new TecnicoEntity();
                tecnicoEntity.FL_ATIVO = "S";
                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                PedidoData pedidoData = new PedidoData();
                PedidoPecaData pedidoPecaData = new PedidoPecaData();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        string CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();

                        PedidoEntity pedidoEntity = new PedidoEntity();
                        pedidoEntity.ID_PEDIDO = 0;
                        pedidoEntity.tecnico.CD_TECNICO = CD_TECNICO;
                        pedidoEntity.NR_DOCUMENTO = 0;
                        pedidoEntity.DT_CRIACAO = DateTime.UtcNow.AddHours(-3);
                        pedidoEntity.statusPedido.ID_STATUS_PEDIDO = statusNovoRascunho;
                        pedidoEntity.TP_TIPO_PEDIDO = "T";
                        pedidoEntity.nidUsuarioAtualizacao = 1;
                        pedidoEntity.FL_EMERGENCIA = "N";
                        pedidoEntity.TOKEN = ControlesUtility.Utilidade.ObterPrefixoTokenRegistro(idUsuario);

                        pedidoData.Inserir(pedidoEntity);
                        pedidoEntity.statusPedido.ID_STATUS_PEDIDO = statusSolicitado;
                        //Ao solicitar peças com qtd zero, estas se tornarão itens cancelados
                        pedidoPecaData.AlterarStatus(pedidoEntity.ID_PEDIDO, Convert.ToInt16(pedidoEntity.statusPedido.ID_STATUS_PEDIDO), pedidoEntity.pecasLote, pedidoEntity.nidUsuarioAtualizacao);

                        //Replicar qtds solicitadas para coluna aprovadas
                        pedidoPecaData.AlterarQtdeAprovada(pedidoEntity.ID_PEDIDO, pedidoEntity.nidUsuarioAtualizacao);

                        pedidoEntity.DT_ENVIO = DateTime.UtcNow.AddHours(-3);
                        pedidoData.Alterar(pedidoEntity);

                        //Atualiza Status do Pedido
                        pedidoPecaData.AtualizaStatusPedido(pedidoEntity.ID_PEDIDO);

                        //Pedidos solicitados enviam e-mail para grupo cadastrado em tbParametros
                        //string Mensagem = string.Empty;
                        //EnviarEmailPedido(pedidoEntity, "SOLICITADO", ref Mensagem);

                        //string msgLog = String.Format("GerarPedidosTecnico: CD_Tecnico({0}) gerado sucesso!)", CD_TECNICO);
                        //LogUtility.LogarErro(msgLog, TipoMensagem.Info);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                JObject JO = new JObject();
                JO.Add("STATUS", "Pedidos Técnico gerados com sucesso!");
                LogUtility.LogarErro("GerarPedidosTecnico executado com sucesso!", TipoMensagem.Info);
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("AlterarPedidoBPCS")]
        public HttpResponseMessage AlterarPedidoBPCS(PedidoEntity pedidoEntity)
        {
            string Mensagem = string.Empty;
            TransactionData transacao = new TransactionData();

            try
            {
                PedidoData pedidoData = new PedidoData(transacao);
                PedidoPecaData pedidoPecaData = new PedidoPecaData(transacao);
                PedidoEntity pedidoEntityDepoisAtualizacao = new PedidoEntity();
                
                var pecas = new PedidoPecaData().ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO).ToList();

                if (pedidoEntity.TP_Especial != "Especial")
                {
                    var pecasAP0 = pecas.Where(y => y.QTD_APROVADA_3M1 == 0 && (y.CD_PECA_REFERENCIA == "" || y.CD_PECA_REFERENCIA == null)).ToList();

                    if (pecasAP0?.Count > 0)
                    {
                        new PedidoPecaData().AtualizarEnvioBPCSPECAAprovado0(pedidoEntity.ID_PEDIDO);

                    }
                }
                

                var pecasDados = new PedidoPecaData().ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO).Where(x => x.ST_STATUS_ITEM != '4' && x.ST_STATUS_ITEM != '3').ToList();

                if (pecasDados?.Count > 0)
                {
                    foreach(var peca in pecasDados)
                    {
                        var dados = new PedidoPecaData().ObterListaDadosPedido(Convert.ToInt64(peca.ID_ITEM_PEDIDO)).FirstOrDefault();

                        if(dados == null)
                            throw new Exception ($"Realize o cadastro de dados do item: {peca.CD_PECA}!!");
                            
                    }

                    new PedidoPecaData().AtualizarEnvioBPCS(pedidoEntity.ID_PEDIDO);

                    new PedidoPecaData().AtualizarEnvioBPCSPECA(pedidoEntity.ID_PEDIDO);

                }
                
                transacao.Commit();
            }catch (Exception ex)
            {
                transacao.Rollback();
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            finally
            {
                transacao.Dispose();
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_PEDIDO = pedidoEntity.ID_PEDIDO, MENSAGEM = Mensagem });
        }

        [HttpPost]
        [Route("Alterar")]
        public HttpResponseMessage Alterar(PedidoEntity pedidoEntity)
        {
            /*STATUS PEDIDO:										|	STATUS ITEM:
            1	Novo/Rascunho			-							|	1-Novo/Rascunho
            2	Solicitado				Solicitar à 3M				|	2-Pendente
            3	Aprovado				Aprovar Itens				|	3-Aprovado
            4	Recebido				Confirmar Recebimento		|	4-Cancelado
            5	Pendente				Registrar Pendências		|	5-Recebido
            6	Recebido com Pendência	Registrar Peças Pendentes   |   6-Solicitado
            7	Cancelado				Cancelar Itens              |   7-Recebido com Pendência
            8   Aguardando Envio BPCS                               |   8-Aguardando envio BPCS
            9   Enviado para faturamento BPCS                       |   9-Enviado para faturamento BPCS
            */
            if(pedidoEntity.Telefone != null && pedidoEntity.Telefone != "")
                pedidoEntity.Telefone = pedidoEntity.Telefone.Replace("(", "").Replace(")", "").Replace("-", "");
            if (pedidoEntity.Origem == "Web")
            {
                pedidoEntity.Origem = "W";
            }
            else if (pedidoEntity.Origem == "App")
            {
                pedidoEntity.Origem = "A";
            }
            else
            {
                pedidoEntity.Origem = "";
            }
            string Mensagem = string.Empty;
            TransactionData transacao = new TransactionData();

            try
            {
                PedidoData pedidoData = new PedidoData(transacao);
                PedidoPecaData pedidoPecaData = new PedidoPecaData(transacao);
                PecaData pecaData = new PecaData(transacao);
                PedidoEntity pedidoEntityDepoisAtualizacao = new PedidoEntity();

                if (pedidoEntity.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.NovoRascunho))
                    pedidoData.Alterar(pedidoEntity);

                if (pedidoEntity.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Solicitado))
                {
                    var quantidadePecas = (from p in pedidoPecaData.ObterLista(new PedidoPecaEntity() { pedido = pedidoEntity }).Rows.Cast<DataRow>()
                                           select p.FieldOrDefault<decimal>("QTD_SOLICITADA")).Sum();

                    if (quantidadePecas <= 0)
                        return Request.CreateResponse(HttpStatusCode.OK, new { pedidoEntity.ID_PEDIDO, MENSAGEM = "Solicitação não efetuada, é necessário informar uma peça/quantidade para concluir a ação." });

                    //Ao solicitar peças com qtd zero, estas se tornarão itens cancelados
                    pedidoPecaData.AlterarStatus(pedidoEntity.ID_PEDIDO, Convert.ToInt16(ControlesUtility.Enumeradores.StatusPedido.Solicitado), pedidoEntity.pecasLote, pedidoEntity.nidUsuarioAtualizacao);

                    //Replicar qtds solicitadas para coluna aprovadas
                    //pedidoPecaData.AlterarQtdeAprovada(pedidoEntity.ID_PEDIDO, pedidoEntity.nidUsuarioAtualizacao);

                    pedidoEntity.DT_ENVIO = DateTime.Now;
                    pedidoEntity.DT_CRIACAO = DateTime.Now;
                    pedidoData.Alterar(pedidoEntity);

                    //Pedidos solicitados enviam e-mail para grupo cadastrado em tbParametros
                    EnviarEmailPedido(pedidoEntity, "SOLICITADO", ref Mensagem);
                }

                if (pedidoEntity.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Aprovado))
                {

                    //pedidoData.AtualizarEnvioBPCS(pedidoEntity);

                    IList<PedidoPecaSinc> pecasPedidoSemItem = pedidoPecaData.ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO).ToList();
                    if (pedidoEntity.TP_Especial == "Especial" && pedidoEntity.EnviaBPCS == "S")
                    {
                        var pecas = pedidoEntity.pecasLote.Split(',').ToList();
                        var pedido_peca = pedidoPecaData.ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO).ToList();
                        Int64 Qtd_Total_AP = 0;
                        foreach (var pc in pecas)
                        {
                            var pecaPedido = pedido_peca.FirstOrDefault(x => x.CD_PECA.ToUpper() == pc.ToUpper());
                            if (pecaPedido != null)
                            {
                                Qtd_Total_AP = Convert.ToInt64(pecaPedido.QTD_SOLICITADA);

                                if(pecaPedido.QTD_APROVADA == 0 && pecaPedido.QTD_APROVADA_3M2 == 0)
                                    pedidoPecaData.AprovarQTDPedidoPECA(pecaPedido.ID_ITEM_PEDIDO, Qtd_Total_AP, null, Qtd_Total_AP);

                                pedidoPecaData.AlterarStatusRecebimento(pecaPedido.ID_ITEM_PEDIDO, 8);

                                pedidoPecaData.CriarLoteCliente(pedidoEntity.ID_PEDIDO, pecaPedido.CD_PECA, pedidoEntity.pecasLote, ref Mensagem);
                            }
                        }
                    }
                    else if (pedidoEntity.TP_Especial == "Especial" && pedidoEntity.EnviaBPCS == "N")
                    {
                        Int64 Qtd_Total_AP = 0;
                        var pecas = pedidoEntity.pecasLote.Split(',').ToList();
                        var pedido_peca = pedidoPecaData.ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO).ToList();
                        foreach (var pc in pecas)
                        {
                            var pecaPedido = pedido_peca.FirstOrDefault(x => x.CD_PECA.ToUpper() == pc.ToUpper());
                            if (pecaPedido != null)
                            {
                                Qtd_Total_AP = Convert.ToInt64(pecaPedido.QTD_SOLICITADA);

                                if (pecaPedido.QTD_APROVADA == 0 && pecaPedido.QTD_APROVADA_3M2 == 0)
                                    pedidoPecaData.AprovarQTDPedidoPECA(pecaPedido.ID_ITEM_PEDIDO, Qtd_Total_AP, null, Qtd_Total_AP);

                                if (pedidoEntity.TP_TIPO_PEDIDO == "C")
                                    pedidoPecaData.AlterarStatusRecebimento(pecaPedido.ID_ITEM_PEDIDO, 5);
                                else
                                    pedidoPecaData.AlterarStatusRecebimento(pecaPedido.ID_ITEM_PEDIDO, 3);

                                pedidoPecaData.CriarLoteCliente(pedidoEntity.ID_PEDIDO, pecaPedido.CD_PECA, pedidoEntity.pecasLote, ref Mensagem);
                            }
                        }
                    }
                    else { 
                        foreach (var ppSemPeca in pecasPedidoSemItem)
                        {
                            if (ppSemPeca.CD_PECA == "0" || ppSemPeca.CD_PECA == null || ppSemPeca.CD_PECA == "")
                            {
                                pedidoPecaData.AprovarPedidoSemItem(ppSemPeca.ID_ITEM_PEDIDO);
                            }
                            if (pedidoEntity.TP_TIPO_PEDIDO == "C")
                            {
                                if (pedidoEntity.pecasLote.ToUpper().Contains(ppSemPeca.CD_PECA.ToUpper())){
                                    if (pedidoEntity.EnviaBPCS == "S")
                                    {
                                        if (ppSemPeca.QTD_APROVADA_3M2 > 0)
                                        {
                                            int contadortoken = 0;
                                            var pecaEntity = pecaData.ObterPecasRecuperadas(ppSemPeca.CD_PECA);
                                            if (pecaEntity.CD_PECA_RECUPERADA == null || pecaEntity.CD_PECA_RECUPERADA == "")
                                                throw new Exception($"Realize o cadastro da Peça recuperada a Peça: {pecaEntity.CD_PECA}!");

                                            else
                                            {
                                                var pecaEntityRec = pecaData.ObterPecasRecuperadas(pecaEntity.CD_PECA_RECUPERADA);
                                                if (pecaEntityRec.CD_PECA == null)
                                                    throw new Exception($"Realize o cadastro da Peça recuperada: {pecaEntity.CD_PECA_RECUPERADA}!");

                                                var pedidoCadastrado = pedidoPecaData.ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO).FirstOrDefault
                                                                       (x => x.CD_PECA_REFERENCIA == ppSemPeca.CD_PECA);

                                                if (pedidoCadastrado == null)
                                                {
                                                    PedidoPecaEntity pecaRecuperada = new PedidoPecaEntity();

                                                    pecaRecuperada.pedido.ID_PEDIDO = pedidoEntity.ID_PEDIDO;
                                                    pecaRecuperada.peca.CD_PECA = pecaEntity.CD_PECA_RECUPERADA;
                                                    pecaRecuperada.QTD_SOLICITADA = ppSemPeca.QTD_SOLICITADA;
                                                    pecaRecuperada.QTD_APROVADA = ppSemPeca.QTD_APROVADA;
                                                    pecaRecuperada.QTD_APROVADA_3M2 = ppSemPeca.QTD_APROVADA_3M2;
                                                    pecaRecuperada.QTD_RECEBIDA = ppSemPeca.QTD_RECEBIDA;
                                                    pecaRecuperada.QTD_APROVADA_3M1 = 0;
                                                    pecaRecuperada.TX_APROVADO = ppSemPeca.TX_APROVADO;
                                                    pecaRecuperada.ST_STATUS_ITEM = "8";
                                                    pecaRecuperada.DS_OBSERVACAO = ppSemPeca.DS_OBSERVACAO;
                                                    pecaRecuperada.DS_DIR_FOTO = ppSemPeca.DS_DIR_FOTO;
                                                    pecaRecuperada.estoque3M1.ID_ESTOQUE = 0;
                                                    pecaRecuperada.estoque3M2.ID_ESTOQUE = 2;
                                                    pecaRecuperada.VL_PECA = ppSemPeca.VL_PECA;
                                                    pecaRecuperada.TIPO_PECA = ppSemPeca.TIPO_PECA;
                                                    pecaRecuperada.DESCRICAO_PECA = ppSemPeca.DESCRICAO_PECA;
                                                    pecaRecuperada.Duplicado = "S";
                                                    pecaRecuperada.CD_PECA_REFERENCIA = ppSemPeca.CD_PECA;
                                                    var tokenn = ppSemPeca.TOKEN.ToString() + "1" + contadortoken.ToString() + "1";
                                                    pecaRecuperada.TOKEN = Convert.ToInt64(tokenn);
                                                    //new PedidoPecaData().DuplicarItem(ppSemPeca.ID_ITEM_PEDIDO);

                                                    pedidoPecaData.InserirPecaRecuperada(pecaRecuperada);
                                                    
                                                }

                                            }
                                        }
                                    }

                                    if (ppSemPeca.ST_STATUS_ITEM != '4' && ppSemPeca.ST_STATUS_ITEM != '5' && ppSemPeca.ST_STATUS_ITEM != '7' && ppSemPeca.ST_STATUS_ITEM != '1' && ppSemPeca.ST_STATUS_ITEM != '3')
                                    {
                                        if (pedidoEntity.pecasLote.ToUpper().Contains(ppSemPeca.CD_PECA.ToUpper()))
                                        {
                                            Int64 Qtd_Total_AP = 0;
                                            if (ppSemPeca.QTD_APROVADA_3M1 > 0 || ppSemPeca.QTD_APROVADA_3M2 > 0)
                                            {
                                                Qtd_Total_AP += Convert.ToInt64(ppSemPeca.QTD_APROVADA_3M1);
                                                Qtd_Total_AP += Convert.ToInt64(ppSemPeca.QTD_APROVADA_3M2);
                                                if (Qtd_Total_AP > 0 && ppSemPeca.ST_STATUS_ITEM != '4')
                                                {
                                                    pedidoPecaData.CriarLoteCliente(pedidoEntity.ID_PEDIDO, ppSemPeca.CD_PECA, pedidoEntity.pecasLote, ref Mensagem);
                                                    pedidoPecaData.ProcessarAprovacao(pedidoEntity.ID_PEDIDO, pedidoEntity.nidUsuarioAtualizacao, ppSemPeca.CD_PECA, ref Mensagem);

                                                    pedidoPecaData.Atualizar_Estoque_Tabela_Peca_CLIENTE(ppSemPeca.ID_ITEM_PEDIDO, ppSemPeca.CD_PECA, pedidoEntity.cliente.CD_CLIENTE);

                                                    ExibirMensagemvalidacao(Mensagem);

                                                    pedidoPecaData.AprovarQTDPedidoPECA(ppSemPeca.ID_ITEM_PEDIDO, Qtd_Total_AP, Convert.ToInt64(ppSemPeca.QTD_APROVADA_3M1), Convert.ToInt64(ppSemPeca.QTD_APROVADA_3M2));

                                                    string CdCli = pedidoEntity.cliente.CD_CLIENTE.ToString();

                                                    EstoqueData estoque = new EstoqueData();
                                                    EstoquePecaData estoquePeca = new EstoquePecaData();

                                                    EstoqueEntity est = new EstoqueEntity();

                                                    EstoqueSinc estoqueCli = estoque.ObterListaEstoqueSinc().Where(x => x.CD_CLIENTE == CdCli).FirstOrDefault();

                                                    if (estoqueCli == null)
                                                    {
                                                        Exception ex = new Exception("Não existe local de armazenamento criado para esse cliente");
                                                        throw ex;
                                                    }

                                                    EstoquePecaSinc estoquePecaEntity = estoquePeca.ObterListaEstoquePecaSincPorID(estoqueCli.ID_ESTOQUE).Where
                                                        (x => x.CD_PECA.ToUpper() == ppSemPeca.CD_PECA.ToUpper()).FirstOrDefault();

                                                    EstoquePecaSinc estoquePecaEntity3M = estoquePeca.ObterListaEstoquePecaSincPorID(1).Where
                                                        (x => x.CD_PECA.ToUpper() == ppSemPeca.CD_PECA.ToUpper()).FirstOrDefault();

                                                    EstoquePecaSinc estoquePecaEntity3M2 = estoquePeca.ObterListaEstoquePecaSincPorID(2).Where
                                                        (x => x.CD_PECA.ToUpper() == ppSemPeca.CD_PECA.ToUpper()).FirstOrDefault();

                                                    decimal app = Qtd_Total_AP;
                                                    pedidoPecaData.AprovarPedidoCliente(ppSemPeca.ID_ITEM_PEDIDO, Qtd_Total_AP);
                                                    //if (estoquePecaEntity != null)
                                                    //{
                                                    //    estoquePecaEntity.QT_PECA_ATUAL += app;

                                                    //    pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity.ID_ESTOQUE_PECA, estoquePecaEntity.QT_PECA_ATUAL);

                                                    //    if (estoquePecaEntity3M != null && ppSemPeca.QTD_APROVADA_3M1 > 0)
                                                    //    {
                                                    //        estoquePecaEntity3M.QT_PECA_ATUAL = estoquePecaEntity3M.QT_PECA_ATUAL - ppSemPeca.QTD_APROVADA_3M1;
                                                    //        if (estoquePecaEntity3M.QT_PECA_ATUAL < 0)
                                                    //        {
                                                    //            estoquePecaEntity3M.QT_PECA_ATUAL = 0;
                                                    //        }
                                                    //        pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity3M.ID_ESTOQUE_PECA, estoquePecaEntity3M.QT_PECA_ATUAL);
                                                    //    }
                                                    //    if (estoquePecaEntity3M2 != null && ppSemPeca.QTD_APROVADA_3M2 > 0)
                                                    //    {
                                                    //        estoquePecaEntity3M2.QT_PECA_ATUAL = estoquePecaEntity3M2.QT_PECA_ATUAL - ppSemPeca.QTD_APROVADA_3M2;
                                                    //        if (estoquePecaEntity3M2.QT_PECA_ATUAL < 0)
                                                    //        {
                                                    //            estoquePecaEntity3M2.QT_PECA_ATUAL = 0;
                                                    //        }
                                                    //        pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity3M2.ID_ESTOQUE_PECA, estoquePecaEntity3M2.QT_PECA_ATUAL);
                                                    //    }

                                                    //}
                                                    //else
                                                    //{
                                                    //    if (estoqueCli != null)
                                                    //    {
                                                    //        pedidoPecaData.InserirEstoquePecaCliente(app, ppSemPeca.CD_PECA, 0, estoqueCli.ID_ESTOQUE);
                                                    //        if (estoquePecaEntity3M != null && ppSemPeca.QTD_APROVADA_3M1 > 0)
                                                    //        {
                                                    //            estoquePecaEntity3M.QT_PECA_ATUAL = estoquePecaEntity3M.QT_PECA_ATUAL - ppSemPeca.QTD_APROVADA_3M1;
                                                    //            if (estoquePecaEntity3M.QT_PECA_ATUAL < 0)
                                                    //            {
                                                    //                estoquePecaEntity3M.QT_PECA_ATUAL = 0;
                                                    //            }
                                                    //            pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity3M.ID_ESTOQUE_PECA, estoquePecaEntity3M.QT_PECA_ATUAL);
                                                    //        }
                                                    //        if (estoquePecaEntity3M2 != null && ppSemPeca.QTD_APROVADA_3M2 > 0)
                                                    //        {
                                                    //            estoquePecaEntity3M2.QT_PECA_ATUAL = estoquePecaEntity3M2.QT_PECA_ATUAL - ppSemPeca.QTD_APROVADA_3M2;
                                                    //            if (estoquePecaEntity3M2.QT_PECA_ATUAL < 0)
                                                    //            {
                                                    //                estoquePecaEntity3M2.QT_PECA_ATUAL = 0;
                                                    //            }
                                                    //            pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity3M2.ID_ESTOQUE_PECA, estoquePecaEntity3M2.QT_PECA_ATUAL);
                                                    //        }
                                                    //    }

                                                    //}


                                                }
                                                else
                                                {
                                                    Qtd_Total_AP = Convert.ToInt64(ppSemPeca.QTD_SOLICITADA);
                                                    pedidoPecaData.CriarLoteCliente(pedidoEntity.ID_PEDIDO, ppSemPeca.CD_PECA, pedidoEntity.pecasLote, ref Mensagem);
                                                    pedidoPecaData.ProcessarAprovacao(pedidoEntity.ID_PEDIDO, pedidoEntity.nidUsuarioAtualizacao, ppSemPeca.CD_PECA, ref Mensagem);
                                                    pedidoPecaData.Atualizar_Estoque_Tabela_Peca_CLIENTE(ppSemPeca.ID_ITEM_PEDIDO, ppSemPeca.CD_PECA, pedidoEntity.cliente.CD_CLIENTE);

                                                    ExibirMensagemvalidacao(Mensagem);

                                                    pedidoPecaData.AprovarQTDPedidoPECA(ppSemPeca.ID_ITEM_PEDIDO, Qtd_Total_AP, Convert.ToInt64(ppSemPeca.QTD_APROVADA_3M1), Convert.ToInt64(ppSemPeca.QTD_APROVADA_3M2));

                                                    pedidoPecaData.AprovarPedidoCliente(ppSemPeca.ID_ITEM_PEDIDO, Qtd_Total_AP);
                                                }

                                            }
                                            else
                                            {
                                                if (ppSemPeca.ST_STATUS_ITEM != '4' && ppSemPeca.ST_STATUS_ITEM != '5' && ppSemPeca.ST_STATUS_ITEM != '7' && ppSemPeca.ST_STATUS_ITEM != '1' && ppSemPeca.ST_STATUS_ITEM != '3')
                                                {
                                                    string CdCli = pedidoEntity.cliente.CD_CLIENTE.ToString();

                                                    EstoqueData estoque = new EstoqueData();
                                                    EstoquePecaData estoquePeca = new EstoquePecaData();

                                                    EstoqueEntity est = new EstoqueEntity();

                                                    EstoqueSinc estoqueCli = estoque.ObterListaEstoqueSinc().Where(x => x.CD_CLIENTE == CdCli).FirstOrDefault();

                                                    if (estoqueCli == null)
                                                    {
                                                        Exception ex = new Exception("Não existe local de armazenamento criado para esse cliente");
                                                        throw ex;
                                                    }

                                                    EstoquePecaSinc estoquePecaEntity = estoquePeca.ObterListaEstoquePecaSincPorID(estoqueCli.ID_ESTOQUE).Where
                                                        (x => x.CD_PECA.ToUpper() == ppSemPeca.CD_PECA.ToUpper()).FirstOrDefault();

                                                    EstoquePecaSinc estoquePecaEntity3M = estoquePeca.ObterListaEstoquePecaSincPorID(1).Where
                                                        (x => x.CD_PECA.ToUpper() == ppSemPeca.CD_PECA.ToUpper()).FirstOrDefault();

                                                    if (estoquePecaEntity3M.QT_PECA_ATUAL < ppSemPeca.QTD_SOLICITADA)
                                                    {
                                                        Exception ex = new Exception($"Não há estoque suficiente para a aprovação da peça: {ppSemPeca.CD_PECA.ToUpper().ToString()}");
                                                        throw ex;
                                                    }

                                                    //pedidoPecaData.AprovarPedidoCliente(ppSemPeca.ID_ITEM_PEDIDO, Qtd_Total_AP);

                                                    Qtd_Total_AP = Convert.ToInt64(ppSemPeca.QTD_SOLICITADA);

                                                    pedidoPecaData.CriarLoteCliente(pedidoEntity.ID_PEDIDO, ppSemPeca.CD_PECA, pedidoEntity.pecasLote, ref Mensagem);
                                                    pedidoPecaData.AprovarQTDPedidoPECA(ppSemPeca.ID_ITEM_PEDIDO, Qtd_Total_AP, Convert.ToInt64(ppSemPeca.QTD_APROVADA_3M1), Convert.ToInt64(ppSemPeca.QTD_APROVADA_3M2));

                                                    pedidoPecaData.ProcessarAprovacao(pedidoEntity.ID_PEDIDO, pedidoEntity.nidUsuarioAtualizacao, ppSemPeca.CD_PECA, ref Mensagem);
                                                    pedidoPecaData.Atualizar_Estoque_Tabela_Peca_CLIENTE(ppSemPeca.ID_ITEM_PEDIDO, ppSemPeca.CD_PECA, pedidoEntity.cliente.CD_CLIENTE);

                                                    ExibirMensagemvalidacao(Mensagem);

                                                    pedidoPecaData.AprovarQTDPedidoPECA(ppSemPeca.ID_ITEM_PEDIDO, Qtd_Total_AP, Convert.ToInt64(Qtd_Total_AP), null);


                                                    pedidoPecaData.AprovarPedidoCliente(ppSemPeca.ID_ITEM_PEDIDO, Qtd_Total_AP);


                                                    decimal app = 0;
                                                    if (ppSemPeca.QTD_SOLICITADA > 0)
                                                    {
                                                        app = Convert.ToDecimal(ppSemPeca.QTD_SOLICITADA);
                                                    }

                                                    if (estoquePecaEntity != null)
                                                    {
                                                        estoquePecaEntity.QT_PECA_ATUAL += app;

                                                        pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity.ID_ESTOQUE_PECA, estoquePecaEntity.QT_PECA_ATUAL);

                                                        if (estoquePecaEntity3M != null)
                                                        {
                                                            estoquePecaEntity3M.QT_PECA_ATUAL = estoquePecaEntity3M.QT_PECA_ATUAL - app;
                                                            if (estoquePecaEntity3M.QT_PECA_ATUAL < 0)
                                                            {
                                                                estoquePecaEntity3M.QT_PECA_ATUAL = 0;
                                                            }
                                                            pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity3M.ID_ESTOQUE_PECA, estoquePecaEntity3M.QT_PECA_ATUAL);
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (estoqueCli != null)
                                                        {
                                                            pedidoPecaData.InserirEstoquePecaCliente(app, ppSemPeca.CD_PECA, 0, estoqueCli.ID_ESTOQUE);
                                                            if (estoquePecaEntity3M != null)
                                                            {
                                                                estoquePecaEntity3M.QT_PECA_ATUAL = estoquePecaEntity3M.QT_PECA_ATUAL - app;
                                                                if (estoquePecaEntity3M.QT_PECA_ATUAL < 0)
                                                                {
                                                                    estoquePecaEntity3M.QT_PECA_ATUAL = 0;
                                                                }
                                                                pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity3M.ID_ESTOQUE_PECA, estoquePecaEntity3M.QT_PECA_ATUAL);
                                                            }
                                                        }

                                                    }

                                                }


                                            }


                                        }

                                    }


                                    PedidoPecaSinc ppATT = pedidoPecaData.ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO).Where(x => x.ID_ITEM_PEDIDO == ppSemPeca.ID_ITEM_PEDIDO).FirstOrDefault();
                                    if (ppATT.ST_STATUS_ITEM == '3')
                                    {
                                        pedidoPecaData.AtualizarStatusPedidoCliente(ppATT.ID_ITEM_PEDIDO);
                                        if (pedidoEntity.EnviaBPCS == "S")
                                            pedidoPecaData.AlterarStatusRecebimento(ppSemPeca.ID_ITEM_PEDIDO, 8);
                                    }
                                }
                                
                            }
                            if (pedidoEntity.pecasLote.ToUpper().Contains(ppSemPeca.CD_PECA.ToUpper()) && pedidoEntity.TP_TIPO_PEDIDO != "C")
                            {
                                if(pedidoEntity.EnviaBPCS == "S")
                                {
                                    if (ppSemPeca.QTD_APROVADA_3M2 > 0)
                                    {
                                        int contadortoken = 0;
                                        var pecaEntity = pecaData.ObterPecasRecuperadas(ppSemPeca.CD_PECA);
                                        if (pecaEntity.CD_PECA_RECUPERADA == null || pecaEntity.CD_PECA_RECUPERADA == "")
                                            throw new Exception($"Realize o cadastro da Peça recuperada a Peça: {pecaEntity.CD_PECA}!");

                                        else
                                        {
                                            var pecaEntityRec = pecaData.ObterPecasRecuperadas(pecaEntity.CD_PECA_RECUPERADA);
                                            if (pecaEntityRec.CD_PECA == null)
                                                throw new Exception($"Realize o cadastro da Peça recuperada: {pecaEntity.CD_PECA_RECUPERADA}!");

                                            var pedidoCadastrado = pedidoPecaData.ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO).FirstOrDefault
                                                                   (x => x.CD_PECA_REFERENCIA == ppSemPeca.CD_PECA);

                                            if (pedidoCadastrado == null)
                                            {
                                                PedidoPecaEntity pecaRecuperada = new PedidoPecaEntity();

                                                pecaRecuperada.pedido.ID_PEDIDO = pedidoEntity.ID_PEDIDO;
                                                pecaRecuperada.peca.CD_PECA = pecaEntity.CD_PECA_RECUPERADA;
                                                pecaRecuperada.QTD_SOLICITADA = ppSemPeca.QTD_SOLICITADA;
                                                pecaRecuperada.QTD_APROVADA = ppSemPeca.QTD_APROVADA;
                                                pecaRecuperada.QTD_APROVADA_3M2 = ppSemPeca.QTD_APROVADA_3M2;
                                                pecaRecuperada.QTD_RECEBIDA = ppSemPeca.QTD_RECEBIDA;
                                                pecaRecuperada.QTD_APROVADA_3M1 = 0;
                                                pecaRecuperada.TX_APROVADO = ppSemPeca.TX_APROVADO;
                                                pecaRecuperada.ST_STATUS_ITEM = "8";
                                                pecaRecuperada.DS_OBSERVACAO = ppSemPeca.DS_OBSERVACAO;
                                                pecaRecuperada.DS_DIR_FOTO = ppSemPeca.DS_DIR_FOTO;
                                                pecaRecuperada.estoque3M1.ID_ESTOQUE = 0;
                                                pecaRecuperada.estoque3M2.ID_ESTOQUE = 2;
                                                pecaRecuperada.VL_PECA = ppSemPeca.VL_PECA;
                                                pecaRecuperada.TIPO_PECA = ppSemPeca.TIPO_PECA;
                                                pecaRecuperada.DESCRICAO_PECA = ppSemPeca.DESCRICAO_PECA;
                                                pecaRecuperada.Duplicado = "S";
                                                pecaRecuperada.CD_PECA_REFERENCIA = ppSemPeca.CD_PECA;
                                                var tokenn = ppSemPeca.TOKEN.ToString() + "1" + contadortoken.ToString() + "1";
                                                pecaRecuperada.TOKEN = Convert.ToInt64(tokenn);
                                                //new PedidoPecaData().DuplicarItem(ppSemPeca.ID_ITEM_PEDIDO);

                                                pedidoPecaData.InserirPecaRecuperada(pecaRecuperada);
                                            }

                                        }
                                    }
                                }
                                
                                pedidoPecaData.AprovarLote(pedidoEntity.ID_PEDIDO, ppSemPeca.CD_PECA, pedidoEntity.pecasLote, ref Mensagem);
                                if (!String.IsNullOrEmpty(Mensagem))
                                {
                                    Exception ex = new Exception(Mensagem);
                                    throw ex;
                                }

                                Int64 Qtd_Total_AP = 0;
                                if (ppSemPeca.QTD_APROVADA_3M1 > 0 || ppSemPeca.QTD_APROVADA_3M2 > 0)
                                {
                                    Qtd_Total_AP += Convert.ToInt64(ppSemPeca.QTD_APROVADA_3M1);
                                    Qtd_Total_AP += Convert.ToInt64(ppSemPeca.QTD_APROVADA_3M2);
                                    if (Qtd_Total_AP > 0 && ppSemPeca.ST_STATUS_ITEM != '4')
                                    {
                                        pedidoPecaData.ProcessarAprovacao(pedidoEntity.ID_PEDIDO, pedidoEntity.nidUsuarioAtualizacao, ppSemPeca.CD_PECA, ref Mensagem);
                                        pedidoPecaData.Atualizar_Estoque_Tabela_Peca(ppSemPeca.ID_ITEM_PEDIDO, ppSemPeca.CD_PECA, pedidoEntity.tecnico.CD_TECNICO);

                                        ExibirMensagemvalidacao(Mensagem);

                                        pedidoPecaData.AprovarQTDPedidoPECA(ppSemPeca.ID_ITEM_PEDIDO, Qtd_Total_AP);

                                        if (pedidoEntity.EnviaBPCS != "S")
                                        {
                                            pedidoPecaData.AlterarStatusRecebimento(ppSemPeca.ID_ITEM_PEDIDO, 3);
                                        }

                                    }
                                    else
                                    {
                                        Qtd_Total_AP = Convert.ToInt64(ppSemPeca.QTD_SOLICITADA);
                                        pedidoPecaData.ProcessarAprovacao(pedidoEntity.ID_PEDIDO, pedidoEntity.nidUsuarioAtualizacao, ppSemPeca.CD_PECA, ref Mensagem);

                                        pedidoPecaData.Atualizar_Estoque_Tabela_Peca(ppSemPeca.ID_ITEM_PEDIDO, ppSemPeca.CD_PECA, pedidoEntity.tecnico.CD_TECNICO);
                                        ExibirMensagemvalidacao(Mensagem);

                                        pedidoPecaData.AprovarQTDPedidoPECA(ppSemPeca.ID_ITEM_PEDIDO, Qtd_Total_AP);
                                    }

                                }
                                else
                                {
                                    if (ppSemPeca.ST_STATUS_ITEM != '4' && ppSemPeca.ST_STATUS_ITEM != '5' && ppSemPeca.ST_STATUS_ITEM != '7' && ppSemPeca.ST_STATUS_ITEM != '1' && ppSemPeca.ST_STATUS_ITEM != '3')
                                    {
                                        if (pedidoEntity.TP_TIPO_PEDIDO != "C")
                                        {
                                            Qtd_Total_AP = Convert.ToInt64(ppSemPeca.QTD_SOLICITADA);
                                            pedidoPecaData.ProcessarAprovacao(pedidoEntity.ID_PEDIDO, pedidoEntity.nidUsuarioAtualizacao, ppSemPeca.CD_PECA, ref Mensagem);

                                            pedidoPecaData.Atualizar_Estoque_Tabela_Peca(ppSemPeca.ID_ITEM_PEDIDO, ppSemPeca.CD_PECA, pedidoEntity.tecnico.CD_TECNICO);
                                            ExibirMensagemvalidacao(Mensagem);

                                            pedidoPecaData.AprovarQTDPedidoPECA(ppSemPeca.ID_ITEM_PEDIDO, Qtd_Total_AP);
                                            if (pedidoEntity.EnviaBPCS != "S")
                                            {
                                                pedidoPecaData.AlterarStatusRecebimento(ppSemPeca.ID_ITEM_PEDIDO, 3);
                                            }
                                        }

                                    }

                                }
                            }
                        }

                    }
                }

                if (pedidoEntity.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Recebido))
                {
                    if (pedidoEntity.TP_Especial == "Especial")
                    {
                        var pecas = pedidoEntity.pecasLote.Split(',').ToList();
                        var pedido_peca = pedidoPecaData.ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO).ToList();
                        foreach (var pc in pecas)
                        {
                            var pecaPedido = pedido_peca.FirstOrDefault(x => x.CD_PECA.ToUpper() == pc.ToUpper());
                            if (pecaPedido != null)
                            {
                                if (pecaPedido.QTD_APROVADA == pecaPedido.QTD_RECEBIDA)
                                {
                                    pedidoPecaData.AlterarStatusRecebimento(pecaPedido.ID_ITEM_PEDIDO, 5);
                                }
                                else
                                {
                                    pedidoPecaData.AlterarStatusRecebimento(pecaPedido.ID_ITEM_PEDIDO, 7);
                                }
                                
                            }
                        }
                    }
                    else
                    {

                        pedidoPecaData.AlterarStatus(pedidoEntity.ID_PEDIDO, Convert.ToInt16(ControlesUtility.Enumeradores.StatusPedido.Recebido), pedidoEntity.pecasLote, pedidoEntity.nidUsuarioAtualizacao);
                        pedidoPecaData.ProcessarRecebimento(pedidoEntity.ID_PEDIDO, Convert.ToInt16(ControlesUtility.Enumeradores.StatusPedido.Recebido), pedidoEntity.pecasLote, pedidoEntity.nidUsuarioAtualizacao);

                        IList<PedidoPecaSinc> pecasPedidoEstoque = pedidoPecaData.ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO);

                        pecasPedidoEstoque = pecasPedidoEstoque.Where(x => pedidoEntity.pecasLote.ToUpper().Contains(x.CD_PECA.ToUpper())).ToList();

                        if (pecasPedidoEstoque?.Count > 0)
                        {
                            foreach (var pedidos in pecasPedidoEstoque)
                            {
                                if (pedidos.atualizado != "S")
                                {
                                    if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_RECEBIDA && pedidos.ST_STATUS_ITEM == '5')
                                    {
                                        if (pedidos.QTD_APROVADA > 0)
                                        {

                                            string CdCli = pedidoEntity.tecnico.CD_TECNICO.ToString();

                                            EstoqueData estoque = new EstoqueData();
                                            EstoquePecaData estoquePeca = new EstoquePecaData();

                                            EstoqueEntity est = new EstoqueEntity();

                                            EstoqueSinc estoqueCli = estoque.ObterListaEstoqueSinc().Where(x => x.CD_TECNICO == CdCli).FirstOrDefault();

                                            if (estoqueCli == null)
                                            {
                                                Exception ex = new Exception("Não existe local de armazenamento criado para esse Tecnico");
                                                throw ex;
                                            }

                                            EstoquePecaSinc estoquePecaEntity = estoquePeca.ObterListaEstoquePecaSincPorID(estoqueCli.ID_ESTOQUE).Where
                                                (x => x.CD_PECA.ToUpper() == pedidos.CD_PECA.ToUpper()).FirstOrDefault();

                                            if (estoquePecaEntity == null)
                                            {
                                                if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_APROVADA || pedidos.QTD_RECEBIDA == pedidos.QTD_APROVADA)
                                                {
                                                    pedidoPecaData.AtualizarRecebimentoPedido(pedidos.ID_ITEM_PEDIDO);
                                                }
                                                pedidoPecaData.InserirEstoquePecaCliente(Convert.ToDecimal(pedidos.QTD_ULTIMO_RECEBIMENTO), pedidos.CD_PECA, 0, estoqueCli.ID_ESTOQUE);
                                            }
                                            else
                                            {
                                                decimal QTD_PECA_ATUAL = 0;
                                                decimal QTD_PECA_ATUAL_Estoque = 0;

                                                if (pedidos.QTD_ULTIMO_RECEBIMENTO != 0)
                                                {
                                                    QTD_PECA_ATUAL = Convert.ToDecimal(pedidos.QTD_ULTIMO_RECEBIMENTO);
                                                }

                                                QTD_PECA_ATUAL_Estoque = QTD_PECA_ATUAL + estoquePecaEntity.QT_PECA_ATUAL;

                                                pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity.ID_ESTOQUE_PECA, QTD_PECA_ATUAL_Estoque);
                                                if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_APROVADA || pedidos.QTD_RECEBIDA == pedidos.QTD_APROVADA)
                                                {
                                                    pedidoPecaData.AtualizarRecebimentoPedido(pedidos.ID_ITEM_PEDIDO);
                                                }
                                            }

                                        }
                                    }
                                    else if (pedidos.QTD_ULTIMO_RECEBIMENTO == 0 && pedidos.QTD_APROVADA == pedidos.QTD_RECEBIDA && pedidos.ST_STATUS_ITEM == '5')
                                    {
                                        if (pedidos.QTD_APROVADA > 0)
                                        {

                                            string CdCli = pedidoEntity.tecnico.CD_TECNICO.ToString();

                                            EstoqueData estoque = new EstoqueData();
                                            EstoquePecaData estoquePeca = new EstoquePecaData();

                                            EstoqueEntity est = new EstoqueEntity();

                                            EstoqueSinc estoqueCli = estoque.ObterListaEstoqueSinc().Where(x => x.CD_TECNICO == CdCli).FirstOrDefault();

                                            if (estoqueCli == null)
                                            {
                                                Exception ex = new Exception("Não existe local de armazenamento criado para esse Tecnico");
                                                throw ex;
                                            }

                                            EstoquePecaSinc estoquePecaEntity = estoquePeca.ObterListaEstoquePecaSincPorID(estoqueCli.ID_ESTOQUE).Where
                                                (x => x.CD_PECA.ToUpper() == pedidos.CD_PECA.ToUpper()).FirstOrDefault();

                                            if (estoquePecaEntity == null)
                                            {
                                                if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_APROVADA || pedidos.QTD_RECEBIDA == pedidos.QTD_APROVADA)
                                                {
                                                    pedidoPecaData.AtualizarRecebimentoPedido(pedidos.ID_ITEM_PEDIDO);
                                                }
                                                pedidoPecaData.InserirEstoquePecaCliente(Convert.ToDecimal(pedidos.QTD_RECEBIDA), pedidos.CD_PECA, 0, estoqueCli.ID_ESTOQUE);
                                            }
                                            else
                                            {
                                                decimal QTD_PECA_ATUAL = 0;
                                                decimal QTD_PECA_ATUAL_Estoque = 0;

                                                if (pedidos.QTD_RECEBIDA != 0)
                                                {
                                                    QTD_PECA_ATUAL = Convert.ToDecimal(pedidos.QTD_RECEBIDA);
                                                }

                                                QTD_PECA_ATUAL_Estoque = QTD_PECA_ATUAL + estoquePecaEntity.QT_PECA_ATUAL;

                                                pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity.ID_ESTOQUE_PECA, QTD_PECA_ATUAL_Estoque);
                                                if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_APROVADA || pedidos.QTD_RECEBIDA == pedidos.QTD_APROVADA)
                                                {
                                                    pedidoPecaData.AtualizarRecebimentoPedido(pedidos.ID_ITEM_PEDIDO);
                                                }
                                            }

                                        }
                                    }
                                    else if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_RECEBIDA && pedidos.ST_STATUS_ITEM == '7')
                                    {

                                        if (pedidos.QTD_APROVADA > 0)
                                        {
                                            if (pedidos.atualizado != "P")
                                            {
                                                string CdCli = pedidoEntity.tecnico.CD_TECNICO.ToString();

                                                EstoqueData estoque = new EstoqueData();
                                                EstoquePecaData estoquePeca = new EstoquePecaData();

                                                EstoqueEntity est = new EstoqueEntity();

                                                EstoqueSinc estoqueCli = estoque.ObterListaEstoqueSinc().Where(x => x.CD_TECNICO == CdCli).FirstOrDefault();

                                                if (estoqueCli == null)
                                                {
                                                    Exception ex = new Exception("Não existe local de armazenamento criado para esse Tecnico");
                                                    throw ex;
                                                }

                                                EstoquePecaSinc estoquePecaEntity = estoquePeca.ObterListaEstoquePecaSincPorID(estoqueCli.ID_ESTOQUE).Where
                                                    (x => x.CD_PECA.ToUpper() == pedidos.CD_PECA.ToUpper()).FirstOrDefault();

                                                if (estoquePecaEntity == null)
                                                {
                                                    if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_APROVADA || pedidos.QTD_RECEBIDA == pedidos.QTD_APROVADA)
                                                    {
                                                        pedidoPecaData.AtualizarRecebimentoPedido(pedidos.ID_ITEM_PEDIDO);
                                                    }
                                                    pedidoPecaData.InserirEstoquePecaCliente(Convert.ToDecimal(pedidos.QTD_ULTIMO_RECEBIMENTO), pedidos.CD_PECA, 0, estoqueCli.ID_ESTOQUE);
                                                    pedidoPecaData.AtualizarRecebimentoPedidoPendente(pedidos.ID_ITEM_PEDIDO);
                                                }
                                                else
                                                {
                                                    decimal QTD_PECA_ATUAL = 0;
                                                    decimal QTD_PECA_ATUAL_Estoque = 0;

                                                    if (pedidos.QTD_ULTIMO_RECEBIMENTO != 0)
                                                    {
                                                        QTD_PECA_ATUAL = Convert.ToDecimal(pedidos.QTD_ULTIMO_RECEBIMENTO);
                                                    }

                                                    QTD_PECA_ATUAL_Estoque = QTD_PECA_ATUAL + estoquePecaEntity.QT_PECA_ATUAL;

                                                    pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity.ID_ESTOQUE_PECA, QTD_PECA_ATUAL_Estoque);
                                                    if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_APROVADA || pedidos.QTD_RECEBIDA == pedidos.QTD_APROVADA)
                                                    {
                                                        pedidoPecaData.AtualizarRecebimentoPedido(pedidos.ID_ITEM_PEDIDO);
                                                    }
                                                    pedidoPecaData.AtualizarRecebimentoPedidoPendente(pedidos.ID_ITEM_PEDIDO);
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (pedidos.QTD_ULTIMO_RECEBIMENTO != pedidos.QTD_RECEBIDA && pedidos.ST_STATUS_ITEM == '7')
                                        {
                                            if (pedidos.QTD_APROVADA > 0)
                                            {
                                                if (pedidos.atualizado == "P" && pedidos.QTD_ULTIMO_RECEBIMENTO > 0)
                                                {
                                                    string CdCli = pedidoEntity.tecnico.CD_TECNICO.ToString();

                                                    EstoqueData estoque = new EstoqueData();
                                                    EstoquePecaData estoquePeca = new EstoquePecaData();

                                                    EstoqueEntity est = new EstoqueEntity();

                                                    EstoqueSinc estoqueCli = estoque.ObterListaEstoqueSinc().Where(x => x.CD_TECNICO == CdCli).FirstOrDefault();

                                                    if (estoqueCli == null)
                                                    {
                                                        Exception ex = new Exception("Não existe local de armazenamento criado para esse Tecnico");
                                                        throw ex;
                                                    }

                                                    EstoquePecaSinc estoquePecaEntity = estoquePeca.ObterListaEstoquePecaSincPorID(estoqueCli.ID_ESTOQUE).Where
                                                        (x => x.CD_PECA.ToUpper() == pedidos.CD_PECA.ToUpper()).FirstOrDefault();

                                                    if (estoquePecaEntity == null)
                                                    {
                                                        if (pedidos.QTD_ULTIMO_RECEBIMENTO != 0 && pedidos.QTD_ULTIMO_RECEBIMENTO != null)
                                                        {
                                                            pedidoPecaData.InserirEstoquePecaCliente(Convert.ToDecimal(pedidos.QTD_ULTIMO_RECEBIMENTO), pedidos.CD_PECA, 0, estoqueCli.ID_ESTOQUE);
                                                        }
                                                        else
                                                        {
                                                            pedidoPecaData.InserirEstoquePecaCliente(Convert.ToDecimal(pedidos.QTD_RECEBIDA), pedidos.CD_PECA, 0, estoqueCli.ID_ESTOQUE);
                                                        }
                                                        if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_APROVADA || pedidos.QTD_RECEBIDA == pedidos.QTD_APROVADA)
                                                        {
                                                            pedidoPecaData.AtualizarRecebimentoPedido(pedidos.ID_ITEM_PEDIDO);
                                                        }
                                                        pedidoPecaData.AtualizarRecebimentoPedidoPendente(pedidos.ID_ITEM_PEDIDO);
                                                    }
                                                    else
                                                    {
                                                        decimal QTD_PECA_ATUAL = 0;
                                                        decimal QTD_PECA_ATUAL_Estoque = 0;

                                                        if (pedidos.QTD_ULTIMO_RECEBIMENTO != 0 && pedidos.QTD_ULTIMO_RECEBIMENTO != null)
                                                        {
                                                            QTD_PECA_ATUAL = Convert.ToDecimal(pedidos.QTD_ULTIMO_RECEBIMENTO);
                                                        }
                                                        else
                                                        {
                                                            QTD_PECA_ATUAL = Convert.ToDecimal(pedidos.QTD_RECEBIDA);
                                                        }

                                                        QTD_PECA_ATUAL_Estoque = QTD_PECA_ATUAL + estoquePecaEntity.QT_PECA_ATUAL;

                                                        pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity.ID_ESTOQUE_PECA, QTD_PECA_ATUAL_Estoque);
                                                        if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_APROVADA || pedidos.QTD_RECEBIDA == pedidos.QTD_APROVADA)
                                                        {
                                                            pedidoPecaData.AtualizarRecebimentoPedido(pedidos.ID_ITEM_PEDIDO);
                                                        }
                                                        pedidoPecaData.AtualizarRecebimentoPedidoPendente(pedidos.ID_ITEM_PEDIDO);
                                                    }
                                                }
                                                else if (pedidos.atualizado != "P" && pedidos.QTD_ULTIMO_RECEBIMENTO == 0)
                                                {
                                                    string CdCli = pedidoEntity.tecnico.CD_TECNICO.ToString();

                                                    EstoqueData estoque = new EstoqueData();
                                                    EstoquePecaData estoquePeca = new EstoquePecaData();

                                                    EstoqueEntity est = new EstoqueEntity();

                                                    EstoqueSinc estoqueCli = estoque.ObterListaEstoqueSinc().Where(x => x.CD_TECNICO == CdCli).FirstOrDefault();

                                                    if (estoqueCli == null)
                                                    {
                                                        Exception ex = new Exception("Não existe local de armazenamento criado para esse Tecnico");
                                                        throw ex;
                                                    }

                                                    EstoquePecaSinc estoquePecaEntity = estoquePeca.ObterListaEstoquePecaSincPorID(estoqueCli.ID_ESTOQUE).Where
                                                        (x => x.CD_PECA.ToUpper() == pedidos.CD_PECA.ToUpper()).FirstOrDefault();

                                                    if (estoquePecaEntity == null)
                                                    {
                                                        if (pedidos.QTD_ULTIMO_RECEBIMENTO != 0 && pedidos.QTD_ULTIMO_RECEBIMENTO != null)
                                                        {
                                                            pedidoPecaData.InserirEstoquePecaCliente(Convert.ToDecimal(pedidos.QTD_ULTIMO_RECEBIMENTO), pedidos.CD_PECA, 0, estoqueCli.ID_ESTOQUE);
                                                        }
                                                        else
                                                        {
                                                            pedidoPecaData.InserirEstoquePecaCliente(Convert.ToDecimal(pedidos.QTD_RECEBIDA), pedidos.CD_PECA, 0, estoqueCli.ID_ESTOQUE);
                                                        }
                                                        if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_APROVADA || pedidos.QTD_RECEBIDA == pedidos.QTD_APROVADA)
                                                        {
                                                            pedidoPecaData.AtualizarRecebimentoPedido(pedidos.ID_ITEM_PEDIDO);
                                                        }
                                                        pedidoPecaData.AtualizarRecebimentoPedidoPendente(pedidos.ID_ITEM_PEDIDO);
                                                    }
                                                    else
                                                    {
                                                        decimal QTD_PECA_ATUAL = 0;
                                                        decimal QTD_PECA_ATUAL_Estoque = 0;

                                                        if (pedidos.QTD_ULTIMO_RECEBIMENTO != 0 && pedidos.QTD_ULTIMO_RECEBIMENTO != null)
                                                        {
                                                            QTD_PECA_ATUAL = Convert.ToDecimal(pedidos.QTD_ULTIMO_RECEBIMENTO);
                                                        }
                                                        else
                                                        {
                                                            QTD_PECA_ATUAL = Convert.ToDecimal(pedidos.QTD_RECEBIDA);
                                                        }

                                                        QTD_PECA_ATUAL_Estoque = QTD_PECA_ATUAL + estoquePecaEntity.QT_PECA_ATUAL;

                                                        pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity.ID_ESTOQUE_PECA, QTD_PECA_ATUAL_Estoque);
                                                        if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_APROVADA || pedidos.QTD_RECEBIDA == pedidos.QTD_APROVADA)
                                                        {
                                                            pedidoPecaData.AtualizarRecebimentoPedido(pedidos.ID_ITEM_PEDIDO);
                                                        }
                                                        pedidoPecaData.AtualizarRecebimentoPedidoPendente(pedidos.ID_ITEM_PEDIDO);

                                                    }
                                                }
                                            }
                                        }
                                        else if (pedidos.QTD_ULTIMO_RECEBIMENTO != pedidos.QTD_RECEBIDA && pedidos.ST_STATUS_ITEM == '5')
                                        {
                                            if (pedidos.QTD_APROVADA > 0)
                                            {

                                                string CdCli = pedidoEntity.tecnico.CD_TECNICO.ToString();

                                                EstoqueData estoque = new EstoqueData();
                                                EstoquePecaData estoquePeca = new EstoquePecaData();

                                                EstoqueEntity est = new EstoqueEntity();

                                                EstoqueSinc estoqueCli = estoque.ObterListaEstoqueSinc().Where(x => x.CD_TECNICO == CdCli).FirstOrDefault();

                                                if (estoqueCli == null)
                                                {
                                                    Exception ex = new Exception("Não existe local de armazenamento criado para esse Tecnico");
                                                    throw ex;
                                                }

                                                EstoquePecaSinc estoquePecaEntity = estoquePeca.ObterListaEstoquePecaSincPorID(estoqueCli.ID_ESTOQUE).Where
                                                    (x => x.CD_PECA.ToUpper() == pedidos.CD_PECA.ToUpper()).FirstOrDefault();

                                                if (estoquePecaEntity == null)
                                                {
                                                    if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_APROVADA || pedidos.QTD_RECEBIDA == pedidos.QTD_APROVADA)
                                                    {
                                                        pedidoPecaData.AtualizarRecebimentoPedido(pedidos.ID_ITEM_PEDIDO);
                                                    }
                                                    pedidoPecaData.InserirEstoquePecaCliente(Convert.ToDecimal(pedidos.QTD_ULTIMO_RECEBIMENTO), pedidos.CD_PECA, 0, estoqueCli.ID_ESTOQUE);
                                                }
                                                else
                                                {
                                                    decimal QTD_PECA_ATUAL = 0;
                                                    decimal QTD_PECA_ATUAL_Estoque = 0;

                                                    if (pedidos.QTD_ULTIMO_RECEBIMENTO != 0)
                                                    {
                                                        QTD_PECA_ATUAL = Convert.ToDecimal(pedidos.QTD_ULTIMO_RECEBIMENTO);
                                                    }

                                                    QTD_PECA_ATUAL_Estoque = QTD_PECA_ATUAL + estoquePecaEntity.QT_PECA_ATUAL;

                                                    pedidoPecaData.AtualizarEstoqueClientePeca(estoquePecaEntity.ID_ESTOQUE_PECA, QTD_PECA_ATUAL_Estoque);
                                                    if (pedidos.QTD_ULTIMO_RECEBIMENTO == pedidos.QTD_APROVADA || pedidos.QTD_RECEBIDA == pedidos.QTD_APROVADA)
                                                    {
                                                        pedidoPecaData.AtualizarRecebimentoPedido(pedidos.ID_ITEM_PEDIDO);
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }

                            }
                        }


                    }
                    //pedidoPecaData.AtualizarEstoqueQuantidadeRecebidaSolicitacaoPeca(pedidoEntity.ID_PEDIDO, pedidoEntity.nidUsuarioAtualizacao, ref Mensagem);

                    IList<PedidoPecaSinc> pecasPedidoTotal = pedidoPecaData.ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO);
                    IList<PedidoPecaSinc> pecasPedidoTotalRecebido = pecasPedidoTotal.Where(x => x.ST_STATUS_ITEM == '5').ToList();
                    IList<PedidoPecaSinc> pecasPedidoTotalRecebidoPendencia = pecasPedidoTotal.Where(x => x.ST_STATUS_ITEM == '7').ToList();
                    IList<PedidoPecaSinc> pecasPedidoTotalCancelado = pecasPedidoTotal.Where(x => x.ST_STATUS_ITEM == '4').ToList();
                    IList<PedidoPecaSinc> pecasPedidoTotalAprovado = pecasPedidoTotal.Where(x => x.ST_STATUS_ITEM == '3').ToList();
                    int TotaldePecas = pecasPedidoTotal.Count;
                    int TotaldePecasRecebido = pecasPedidoTotalRecebido.Count;
                    int TotaldePecasRecebidosPendencia = pecasPedidoTotalRecebidoPendencia.Count;
                    int TotaldePecasCanceladas = pecasPedidoTotalCancelado.Count;
                    int TotaldePecasAprovado = pecasPedidoTotalAprovado.Count;

                    if (TotaldePecasRecebido == TotaldePecas)
                    {
                        ExibirMensagemvalidacao(Mensagem);
                        pedidoEntity.DT_RECEBIMENTO = DateTime.Now;
                        pedidoData.Alterar(pedidoEntity);
                    }
                    else if (TotaldePecasRecebidosPendencia == TotaldePecas)
                    {
                        ExibirMensagemvalidacao(Mensagem);
                        pedidoEntity.statusPedido.ID_STATUS_PEDIDO = Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia);
                        pedidoEntity.DT_RECEBIMENTO = DateTime.Now;
                        pedidoData.Alterar(pedidoEntity);
                    }
                    else if (TotaldePecasRecebidosPendencia > 0)
                    {
                        ExibirMensagemvalidacao(Mensagem);
                        pedidoEntity.statusPedido.ID_STATUS_PEDIDO = Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia);
                        pedidoEntity.DT_RECEBIMENTO = DateTime.Now;
                        pedidoData.Alterar(pedidoEntity);
                    }
                    else if ((TotaldePecasCanceladas + TotaldePecasRecebido) == TotaldePecas)
                    {
                        ExibirMensagemvalidacao(Mensagem);
                        pedidoEntity.DT_RECEBIMENTO = DateTime.Now;
                        pedidoData.Alterar(pedidoEntity);
                    }
                    else if ((TotaldePecasAprovado + TotaldePecasRecebido) == TotaldePecas)
                    {
                        ExibirMensagemvalidacao(Mensagem);
                        pedidoEntity.statusPedido.ID_STATUS_PEDIDO = Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia);
                        pedidoEntity.DT_RECEBIMENTO = DateTime.Now;
                        pedidoData.Alterar(pedidoEntity);
                    }
                    else if ((TotaldePecasAprovado + TotaldePecasRecebido + TotaldePecasCanceladas) == TotaldePecas)
                    {
                        ExibirMensagemvalidacao(Mensagem);
                        pedidoEntity.statusPedido.ID_STATUS_PEDIDO = Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia);
                        pedidoEntity.DT_RECEBIMENTO = DateTime.Now;
                        pedidoData.Alterar(pedidoEntity);
                    }
                }



                //if (pedidoEntity.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia))
                //{
                //    pedidoPecaData.AlterarStatus(pedidoEntity.ID_PEDIDO, Convert.ToInt16(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia), pedidoEntity.pecasLote, pedidoEntity.nidUsuarioAtualizacao);
                //    pedidoPecaData.AtualizarEstoqueQuantidadeRecebidaSolicitacaoPeca(pedidoEntity.ID_PEDIDO, pedidoEntity.nidUsuarioAtualizacao, ref Mensagem);

                //    IList<PedidoPecaSinc> pecasPedidoTotal = pedidoPecaData.ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO);
                //    IList<PedidoPecaSinc> pecasPedidoTotalRecebido = pecasPedidoTotal.Where(x => x.ST_STATUS_ITEM == 5).ToList();
                //    IList<PedidoPecaSinc> pecasPedidoTotalRecebidoPendencia = pecasPedidoTotal.Where(x => x.ST_STATUS_ITEM == 7).ToList();
                //    IList<PedidoPecaSinc> pecasPedidoTotalCancelado = pecasPedidoTotal.Where(x => x.ST_STATUS_ITEM == 4).ToList();
                //    int TotaldePecas = pecasPedidoTotal.Count;
                //    int TotaldePecasRecebido = pecasPedidoTotalRecebido.Count;
                //    int TotaldePecasRecebidosPendencia = pecasPedidoTotalRecebidoPendencia.Count;
                //    int TotaldePecasCanceladas = pecasPedidoTotalCancelado.Count;

                //    if (TotaldePecasRecebido == TotaldePecas)
                //    {
                //        ExibirMensagemvalidacao(Mensagem);
                //        pedidoEntity.DT_RECEBIMENTO = DateTime.Now;
                //        pedidoData.Alterar(pedidoEntity);
                //    }
                //    else if (TotaldePecasRecebidosPendencia == TotaldePecas)
                //    {
                //        ExibirMensagemvalidacao(Mensagem);
                //        pedidoEntity.DT_RECEBIMENTO = DateTime.Now;
                //        pedidoData.Alterar(pedidoEntity);
                //    }
                //    else if (TotaldePecasRecebidosPendencia > 0)
                //    {
                //        ExibirMensagemvalidacao(Mensagem);
                //        pedidoEntity.statusPedido.ID_STATUS_PEDIDO = Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.RecebidoComPendencia);
                //        pedidoEntity.DT_RECEBIMENTO = DateTime.Now;
                //        pedidoData.Alterar(pedidoEntity);
                //    }
                //    else if ((TotaldePecasCanceladas + TotaldePecasRecebido) == TotaldePecas)
                //    {
                //        ExibirMensagemvalidacao(Mensagem);
                //        pedidoEntity.DT_RECEBIMENTO = DateTime.Now;
                //        pedidoData.Alterar(pedidoEntity);
                //    }
                //}

                if (pedidoEntity.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Cancelado))
                {
                    IList<PedidoPecaSinc> PedidoSemItem = pedidoPecaData.ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO);
                    if (PedidoSemItem?.Count == 1)
                    {
                        PedidoPecaSinc pedSemItem = PedidoSemItem.FirstOrDefault();
                        if (pedSemItem.CD_PECA == "" || pedSemItem.CD_PECA == null || pedSemItem.CD_PECA == "0")
                        {
                            pedidoPecaData.CancelarPedidoSemItem(pedidoEntity.ID_PEDIDO);
                        }
                        else
                            pedidoPecaData.AlterarStatus(pedidoEntity.ID_PEDIDO, Convert.ToInt16(ControlesUtility.Enumeradores.StatusPedido.Cancelado), pedidoEntity.pecasLote, pedidoEntity.nidUsuarioAtualizacao);
                    }
                    else
                        pedidoPecaData.AlterarStatus(pedidoEntity.ID_PEDIDO, Convert.ToInt16(ControlesUtility.Enumeradores.StatusPedido.Cancelado), pedidoEntity.pecasLote, pedidoEntity.nidUsuarioAtualizacao);

                    var itens_cancelados = pedidoEntity.pecasLote.Split(',');

                    if(pedidoEntity.TP_TIPO_PEDIDO == "C")
                    {
                        foreach (var item in itens_cancelados)
                        {
                            if (item != "")
                            {
                                var peca = PedidoSemItem.FirstOrDefault(x => x.CD_PECA.ToUpper() == item.ToUpper());
                                if (peca != null)
                                    pedidoPecaData.Atualizar_Estoque_Tabela_Peca_CLIENTE(peca.ID_ITEM_PEDIDO, peca.CD_PECA, pedidoEntity.cliente.CD_CLIENTE);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in itens_cancelados)
                        {
                            if (item != "")
                            {
                                var peca = PedidoSemItem.FirstOrDefault(x => x.CD_PECA.ToUpper() == item.ToUpper());
                                if (peca != null)
                                    pedidoPecaData.Atualizar_Estoque_Tabela_Peca(peca.ID_ITEM_PEDIDO, peca.CD_PECA, pedidoEntity.tecnico.CD_TECNICO);
                            }
                        }
                    }
                    
                }

                IList<PedidoPecaSinc> pecasPedido = pedidoPecaData.ObterListaPedidoPecaPedido(pedidoEntity.ID_PEDIDO);
                bool AtualizaPedido = false;
                if (pecasPedido != null)
                {
                    char ST_PECA = '0';
                    char ST_PECA_AUX = '0';
                    int StatusIguais = 0;
                    int Ap = 0;
                    int C = 0;
                    int Recebido = 0;
                    int RecebidoCPendencia = 0;
                    int bpcs = 0;
                    int somabcpsCancelado = 0;
                    int SomaApCancelado = 0;
                    int SomaRecebidoCancelado = 0;
                    if (pedidoEntity.TP_TIPO_PEDIDO != "C")
                    {
                        foreach (var peca in pecasPedido)
                        {
                            if (ST_PECA == '0' && ST_PECA_AUX == '0')
                            {
                                ST_PECA = peca.ST_STATUS_ITEM;

                            }
                            if (ST_PECA != '0' && ST_PECA_AUX == '0')
                            {
                                ST_PECA_AUX = ST_PECA;
                            }
                            if (ST_PECA != '0' && ST_PECA_AUX != '0')
                            {
                                if (peca.ST_STATUS_ITEM == ST_PECA_AUX)
                                {
                                    StatusIguais += 1;
                                }
                                if (peca.ST_STATUS_ITEM == '4')
                                {
                                    C += 1;
                                }
                                if (peca.ST_STATUS_ITEM == '3')
                                {
                                    Ap += 1;
                                }
                                if (peca.ST_STATUS_ITEM == '8')
                                {
                                    bpcs += 1;
                                }
                            }



                        }
                    }
                    else
                    {
                        foreach (var peca in pecasPedido)
                        {
                            if (ST_PECA == '0' && ST_PECA_AUX == '0')
                            {
                                ST_PECA = peca.ST_STATUS_ITEM;

                            }
                            if (ST_PECA != '0' && ST_PECA_AUX == '0')
                            {
                                ST_PECA_AUX = ST_PECA;
                            }
                            if (ST_PECA != '0' && ST_PECA_AUX != '0')
                            {
                                if (peca.ST_STATUS_ITEM == ST_PECA_AUX)
                                {
                                    StatusIguais += 1;
                                }
                                if (peca.ST_STATUS_ITEM == '4')
                                {
                                    C += 1;
                                }
                                if (peca.ST_STATUS_ITEM == '5')
                                {
                                    Recebido += 1;
                                }
                                if (peca.ST_STATUS_ITEM == '7')
                                {
                                    RecebidoCPendencia += 1;
                                }
                                if (peca.ST_STATUS_ITEM == '8')
                                {
                                    bpcs += 1;
                                }
                            }



                        }
                    }
                    somabcpsCancelado = bpcs + C;
                    SomaRecebidoCancelado = C + RecebidoCPendencia + Recebido;
                    SomaApCancelado = Ap + C;
                    if (StatusIguais == pecasPedido.Count || SomaApCancelado == pecasPedido.Count)
                    {
                        AtualizaPedido = true;
                    }
                    else if (StatusIguais == pecasPedido.Count || SomaRecebidoCancelado == pecasPedido.Count)
                    {
                        AtualizaPedido = true;
                    }
                    else if (StatusIguais == pecasPedido.Count || somabcpsCancelado == pecasPedido.Count)
                    {
                        AtualizaPedido = true;
                    }
                    else
                    {
                        AtualizaPedido = false;
                    }
                }
                if (AtualizaPedido == true)
                {
                    //Atualiza Status do Pedido
                    pedidoPecaData.AtualizaStatusPedido(pedidoEntity.ID_PEDIDO);
                }
                

                //SL00035666
                //Envia email pedidos Aprovado e Aprovado Parcial
                if (pedidoEntity.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Aprovado))
                {
                    pedidoEntityDepoisAtualizacao = ObterPedidoEntity(pedidoEntity.ID_PEDIDO);

                    if (pedidoEntityDepoisAtualizacao.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Aprovado))
                    {
                        PedidoPecaEntity pedidoPecaEntity = new PedidoPecaEntity();
                        pedidoPecaEntity.pedido.ID_PEDIDO = pedidoEntity.ID_PEDIDO;
                        pedidoPecaEntity.ST_STATUS_ITEM = STATUS_ITEM_PEDIDO_CANCELADO;
                        DataTableReader dataTableReader = pedidoPecaData.ObterLista(pedidoPecaEntity).CreateDataReader();

                        if (dataTableReader.HasRows) //Se possui peça cancelada é Parcial
                        {
                            CriarNotificacaoPedido(pedidoEntityDepoisAtualizacao, "Aprovado Parcialmente");
                            EnviarEmailPedido(pedidoEntity, "APROVADO PARCIALMENTE", ref Mensagem);
                        }
                        else
                        {
                            CriarNotificacaoPedido(pedidoEntityDepoisAtualizacao, "Aprovado");
                            EnviarEmailPedido(pedidoEntity, "APROVADO", ref Mensagem);
                        }
                    }

                    pedidoData.AtualizarEnvioBPCS(pedidoEntity);
                }

                //Envia email pedidos Cancelado e Cancelado Parcial
                if (pedidoEntity.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Cancelado))
                {
                    pedidoEntityDepoisAtualizacao = ObterPedidoEntity(pedidoEntity.ID_PEDIDO);

                    if (pedidoEntityDepoisAtualizacao.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Cancelado))
                        EnviarEmailPedido(pedidoEntity, "CANCELADO", ref Mensagem);

                    if (pedidoEntityDepoisAtualizacao.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Aprovado))
                        EnviarEmailPedido(pedidoEntity, "APROVADO PARCIALMENTE", ref Mensagem);
                }

                transacao.Commit();
            }
            catch (Exception ex)
            {
                transacao.Rollback();
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            finally
            {
                transacao.Dispose();
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { ID_PEDIDO = pedidoEntity.ID_PEDIDO, MENSAGEM = Mensagem });
        }

        private static void ExibirMensagemvalidacao(string Mensagem)
        {
            if (!String.IsNullOrEmpty(Mensagem))
            {
                Exception ex = new Exception(Mensagem);
                throw ex;
            }
        }

        private static void CriarNotificacaoPedido(PedidoEntity pedido, string statusPedido)
        {
            NotificacaoEntity notificacao = new NotificacaoEntity
            {
                TITULO = $"Pedido {pedido.CD_PEDIDO} {statusPedido}!",
                LIDA = false,
                ID_USUARIO = pedido.tecnico.usuario.nidUsuario
            };

            switch (pedido.TP_TIPO_PEDIDO)
            {
                case TIPO_PEDIDO_TECNICO:
                case TIPO_PEDIDO_AVULSO:
                    notificacao.MENSAGEM = "Faça a confirmação de recebimento das peças quando chegarem.";
                    break;

                case TIPO_PEDIDO_CLIENTE:
                    notificacao.MENSAGEM = "Se necessário, verifique com o Cliente seu recebimento.";
                    break;
            };

            new NotificacaoData().Inserir(ref notificacao);
        }

        [HttpPost]
        [Route("Excluir")]
        public HttpResponseMessage Excluir(PedidoEntity pedidoEntity)
        {
            try
            {
                new PedidoData().Excluir(pedidoEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("Obter")]
        public HttpResponseMessage Obter(Int64 ID_PEDIDO)
        {
            PedidoEntity pedidoEntity = new PedidoEntity();
            Models.ListaSolicitacaoPecas listaSolicitacaoPecas = new Models.ListaSolicitacaoPecas();

            try
            {
                pedidoEntity.ID_PEDIDO = ID_PEDIDO;
                DataTableReader dataTableReader = new PedidoData().ObterLista(pedidoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        
                        CarregarPedidoEntity(dataTableReader, pedidoEntity);
                        CarregarPedidoModel(dataTableReader, listaSolicitacaoPecas);
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
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { pedido = pedidoEntity, pedidoModel = listaSolicitacaoPecas });
        }

        [HttpPost]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(PedidoEntity pedidoEntity)
        {
            List<PedidoEntity> pedidos = new List<PedidoEntity>();
            List<Models.ListaSolicitacaoPecas> listasSolicitacaoPecas = new List<Models.ListaSolicitacaoPecas>();

            try
            {
                if (pedidoEntity == null)
                {
                    pedidoEntity = new PedidoEntity();
                }

                DataTableReader dataTableReader = new PedidoData().ObterLista(pedidoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        pedidoEntity = new PedidoEntity();
                        Models.ListaSolicitacaoPecas listaSolicitacaoPecas = new Models.ListaSolicitacaoPecas();



                        CarregarPedidoEntity(dataTableReader, pedidoEntity);
                        CarregarPedidoModel(dataTableReader, listaSolicitacaoPecas);

                        pedidos.Add(pedidoEntity);
                        listasSolicitacaoPecas.Add(listaSolicitacaoPecas);
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
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { pedidos = pedidos, pedidosModel = listasSolicitacaoPecas });
        }

        [HttpGet]
        [Route("ObterPedidoPeca")]
        public HttpResponseMessage ObterPedidoPeca(Int64 ID_PEDIDO, string CD_TECNICO, string CD_PECA)
        {
            PedidoEntity pedidoEntity = new PedidoEntity();
            Models.ListaPedidoPecas listaPedidoPecas = new Models.ListaPedidoPecas();

            try
            {
                pedidoEntity.ID_PEDIDO = ID_PEDIDO;
                DataTableReader dataTableReader = new PedidoData().ObterListaSolicitacaoPeca(ID_PEDIDO, CD_TECNICO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        string formatadorDecimais = string.Empty;

                        //if (dataTableReader["TX_UNIDADE"].ToString() == "MT")
                        //{
                        //    formatadorDecimais = "N3";
                        //}
                        //else
                        //{
                        formatadorDecimais = "N0";
                        //}

                        listaPedidoPecas = new Models.ListaPedidoPecas
                        {
                            peca = new Models.Peca()
                            {
                                CD_PECA = dataTableReader["CD_PECA"].ToString(),
                                DS_PECA = dataTableReader["DS_PECA"].ToString(),
                                TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                            },
                            planoZero = new Models.PlanoZero()
                            {
                                ccdCriticidadeAbc = string.Empty, //dataTableReader["ccdCriticidadeAbc"].ToString(),
                                nqtPecaModelo = "0",
                            },
                            estoqueMovimentacao = new Models.EstoqueMovimentacao(),
                            estoquePeca = new Models.EstoquePeca(),
                            estoquePeca3M = new Models.EstoquePeca(),
                            pedidoPeca = new Models.PedidoPeca()
                            {
                                QTD_SOLICITADA = Convert.ToDecimal("0" + dataTableReader["QTD_SOLICITADA"]).ToString(formatadorDecimais),
                                DS_ST_STATUS_ITEM = ControlesUtility.Dicionarios.StatusItem().Where(x => x.Value == dataTableReader["ST_STATUS_ITEM"].ToString()).ToArray()[0].Key,
                                ID_ITEM_PEDIDO = Convert.ToInt64(dataTableReader["ID_ITEM_PEDIDO"]),
                            },
                            statusPedido = new StatusPedidoEntity()
                            {
                                ID_STATUS_PEDIDO = Convert.ToInt64(dataTableReader["ID_STATUS_PEDIDO"]),
                            }//,
                            //tipoOrigemPagina = tipoOrigemPagina
                        };

                        if (dataTableReader["QT_PECA_ATUAL"] != DBNull.Value)
                        {
                            listaPedidoPecas.estoquePeca.QT_PECA_ATUAL = Convert.ToDecimal("0" + dataTableReader["QT_PECA_ATUAL"]).ToString(formatadorDecimais);
                        }

                        if (dataTableReader["QT_PECA_ATUAL_3M"] != DBNull.Value)
                        {
                            listaPedidoPecas.estoquePeca3M.QT_PECA_ATUAL = Convert.ToDecimal("0" + dataTableReader["QT_PECA_ATUAL_3M"]).ToString(formatadorDecimais);
                        }

                        if (dataTableReader["DT_MOVIMENTACAO"] != DBNull.Value)
                        {
                            listaPedidoPecas.estoqueMovimentacao.DT_MOVIMENTACAO = Convert.ToDateTime(dataTableReader["DT_MOVIMENTACAO"]).ToString("dd/MM/yyyy");
                        }

                        if (dataTableReader["QTD_APROVADA"] != DBNull.Value)
                        {
                            listaPedidoPecas.pedidoPeca.QTD_APROVADA = Convert.ToDecimal("0" + dataTableReader["QTD_APROVADA"]).ToString(formatadorDecimais);
                        }

                        if (dataTableReader["QTD_RECEBIDA"] != DBNull.Value)
                        {
                            listaPedidoPecas.pedidoPeca.QTD_RECEBIDA = Convert.ToDecimal("0" + dataTableReader["QTD_RECEBIDA"]).ToString(formatadorDecimais);
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
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { listaPedidoPecas = listaPedidoPecas });
        }

        public PedidoEntity ObterPedidoEntity(Int64 ID_PEDIDO)
        {
            PedidoEntity pedidoEntity = new PedidoEntity();
            Models.ListaSolicitacaoPecas listaSolicitacaoPecas = new Models.ListaSolicitacaoPecas();

            try
            {
                pedidoEntity.ID_PEDIDO = ID_PEDIDO;
                DataTableReader dataTableReader = new PedidoData().ObterLista(pedidoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CarregarPedidoEntity(dataTableReader, pedidoEntity);
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
                //throw ex;
            }

            return pedidoEntity;
        }

        [HttpGet]
        public IHttpActionResult CalcularValorPedidoEntity(Int64 CD_PEDIDO)
        {
            PedidoEntity pedidoEntity = new PedidoEntity();
            Models.ListaSolicitacaoPecas listaSolicitacaoPecas = new Models.ListaSolicitacaoPecas();
            JObject JO = new JObject();
            try
            {
                pedidoEntity.ID_PEDIDO = CD_PEDIDO;
                DataTableReader dataTableReader = new PedidoData().ObterLista(pedidoEntity).CreateDataReader();
                Decimal Valor = 0;
                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CarregarPedidoEntity(dataTableReader, pedidoEntity);

                        var pecas_pedido = new PedidoPecaData().ObterListaPedidoPecaPedidoTotal(pedidoEntity.ID_PEDIDO).Where(x => x.ST_STATUS_ITEM == '6').ToList();
                        if (pecas_pedido?.Count == 0)
                        {
                            Valor += 0;
                        }
                        else
                        {
                            foreach (var peca in pecas_pedido)
                            {
                                var valPeca = "0";
                                peca.QTD_APROVADA = Convert.ToInt32(peca.QTD_APROVADA_3M1) + Convert.ToInt32(peca.QTD_APROVADA_3M2);
                                if(peca.QTD_APROVADA > 0)
                                    valPeca = Convert.ToDecimal(Convert.ToDecimal(peca.QTD_APROVADA) * Convert.ToDecimal(peca.VL_PECA)).ToString("N2");
                                else
                                    valPeca = Convert.ToDecimal(Convert.ToDecimal(peca.QTD_SOLICITADA) * Convert.ToDecimal(peca.VL_PECA)).ToString("N2");

                                Valor += Convert.ToDecimal(valPeca);
                            }
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                JO.Add("Valor", JsonConvert.SerializeObject(Valor, Formatting.None));
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
            }

            return Ok(JO);
        }

        protected void CarregarPedidoEntity(DataTableReader dataTableReader, PedidoEntity pedidoEntity)
        {
            try
            {
                pedidoEntity.ID_PEDIDO = Convert.ToInt64("0" + dataTableReader["ID_PEDIDO"]);
                pedidoEntity.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
                pedidoEntity.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                pedidoEntity.tecnico.usuario.nidUsuario = Convert.ToInt64(dataTableReader["ID_USUARIO"]);
                pedidoEntity.tecnico.empresa.CD_Empresa = Convert.ToInt64("0" + dataTableReader["CD_Empresa"]);
                pedidoEntity.tecnico.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                pedidoEntity.NR_DOCUMENTO = Convert.ToInt64("0" + dataTableReader["NR_DOCUMENTO"]);
                pedidoEntity.DT_CRIACAO = Convert.ToDateTime(dataTableReader["DT_CRIACAO"]);
                if (dataTableReader["DT_ENVIO"] != DBNull.Value)
                {
                    pedidoEntity.DT_ENVIO = Convert.ToDateTime(dataTableReader["DT_ENVIO"]);
                }
                if (dataTableReader["DT_Aprovacao"] != DBNull.Value)
                {
                    pedidoEntity.DT_Aprovacao = Convert.ToDateTime(dataTableReader["DT_Aprovacao"]);
                }
                if (dataTableReader["nidUsuario"] != DBNull.Value)
                {
                    Int64 IdUser = Convert.ToInt64(dataTableReader["nidUsuario"].ToString());
                    if (IdUser == 0 || IdUser == null)
                        pedidoEntity.UsuarioSolicitante = null;
                    else
                    {
                        var user = new UsuarioData().ObterListaUsuarioSinc(IdUser).FirstOrDefault(x => x.nidUsuario == IdUser);
                        pedidoEntity.UsuarioSolicitante = user.cnmNome;
                    }
                }
                if (dataTableReader["nidUsuarioAprovador"] != DBNull.Value)
                {
                    Int64 IdUser = Convert.ToInt64(dataTableReader["nidUsuarioAprovador"].ToString());
                    if (IdUser == 0 || IdUser == null)
                        pedidoEntity.UsuarioAprovador = null;
                    else
                    {
                        var user = new UsuarioData().ObterListaUsuarioSinc(IdUser).FirstOrDefault(x => x.nidUsuario == IdUser);
                        pedidoEntity.UsuarioAprovador = user.cnmNome;
                    }
                }
                if (dataTableReader["DT_RECEBIMENTO"] != DBNull.Value)
                {
                    pedidoEntity.DT_RECEBIMENTO = Convert.ToDateTime(dataTableReader["DT_RECEBIMENTO"]);
                }

                pedidoEntity.TX_OBS = dataTableReader["TX_OBS"].ToString();
                pedidoEntity.PENDENTE = dataTableReader["PENDENTE"].ToString();
                pedidoEntity.FL_EMERGENCIA = dataTableReader["FL_EMERGENCIA"].ToString();
                pedidoEntity.NR_DOC_ORI = Convert.ToInt64("0" + dataTableReader["NR_DOC_ORI"]);
                pedidoEntity.statusPedido.ID_STATUS_PEDIDO = Convert.ToInt64("0" + dataTableReader["ID_STATUS_PEDIDO"]);
                pedidoEntity.statusPedido.DS_STATUS_PEDIDO = dataTableReader["DS_STATUS_PEDIDO"].ToString();
                pedidoEntity.TP_TIPO_PEDIDO = dataTableReader["TP_TIPO_PEDIDO"].ToString();
                pedidoEntity.cliente.CD_CLIENTE = Convert.ToInt32("0" + dataTableReader["CD_CLIENTE"]);
                pedidoEntity.cliente.TX_NOMERESPONSAVELPECAS = dataTableReader["TX_NOMERESPONSAVELPECAS"].ToString();
                pedidoEntity.cliente.TX_TELEFONERESPONSAVELPECAS = dataTableReader["TX_TELEFONERESPONSAVELPECAS"].ToString();
                pedidoEntity.CD_PEDIDO = Convert.ToInt64("0" + dataTableReader["CD_PEDIDO"]);
                if (dataTableReader["Responsavel"] != DBNull.Value)
                {
                    pedidoEntity.Responsavel = dataTableReader["Responsavel"].ToString();
                }
                else if (pedidoEntity.TP_TIPO_PEDIDO == "C" && dataTableReader["TX_NOMERESPONSAVELPECAS"] != DBNull.Value)
                {
                    pedidoEntity.Responsavel = dataTableReader["TX_NOMERESPONSAVELPECAS"].ToString();
                }
                else
                {
                    pedidoEntity.Responsavel = "";
                }

                if (dataTableReader["EnviaBPCS"] != DBNull.Value)
                {
                    pedidoEntity.EnviaBPCS = dataTableReader["EnviaBPCS"].ToString();
                }
                else
                {
                    pedidoEntity.EnviaBPCS = "";
                }

                if (dataTableReader["Telefone"] != DBNull.Value)
                {
                    pedidoEntity.Telefone = dataTableReader["Telefone"].ToString();
                }
                else
                {
                    if (pedidoEntity.TP_TIPO_PEDIDO == "A")
                    {
                        var tecnico = new TecnicoData().ObterTecnico(pedidoEntity.tecnico.CD_TECNICO).FirstOrDefault();

                        if (tecnico != null)
                            pedidoEntity.Telefone = tecnico.TX_TELEFONE;

                    }
                    else if (pedidoEntity.TP_TIPO_PEDIDO == "C" && dataTableReader["TX_TELEFONERESPONSAVELPECAS"] != DBNull.Value)
                    {
                        pedidoEntity.Telefone = dataTableReader["TX_TELEFONERESPONSAVELPECAS"].ToString();
                    }
                    else
                    {
                        pedidoEntity.Telefone = "";
                    }
                }

                if (dataTableReader["DT_Aprovacao"] != DBNull.Value)
                {
                    pedidoEntity.DT_Aprovacao = Convert.ToDateTime(dataTableReader["DT_Aprovacao"]);
                }
                if (dataTableReader["Origem"] != DBNull.Value)
                {
                    if (dataTableReader["Origem"].ToString() == "")
                    {
                        pedidoEntity.Origem = "";
                    }else if(dataTableReader["Origem"].ToString() == "W")
                    {
                        pedidoEntity.Origem = "Web";
                    }else if (dataTableReader["Origem"].ToString() == "A")
                    {
                        pedidoEntity.Origem = "App";
                    }
                     
                }

            }
            catch (Exception ex)
            {

            }
            
        }

        protected void CarregarPedidoModel(DataTableReader dataTableReader, Models.ListaSolicitacaoPecas listaSolicitacaoPecas)
        {
            try
            {
                listaSolicitacaoPecas.ID_PEDIDO = Convert.ToInt64("0" + dataTableReader["ID_PEDIDO"]);
                listaSolicitacaoPecas.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
                listaSolicitacaoPecas.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                listaSolicitacaoPecas.tecnico.empresa.CD_Empresa = Convert.ToInt64("0" + dataTableReader["CD_Empresa"]);
                listaSolicitacaoPecas.tecnico.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                listaSolicitacaoPecas.NR_DOCUMENTO = Convert.ToInt64("0" + dataTableReader["NR_DOCUMENTO"]);
                listaSolicitacaoPecas.DT_CRIACAO = Convert.ToDateTime(dataTableReader["DT_CRIACAO"]).ToString("dd/MM/yyyy");
                listaSolicitacaoPecas.TP_TIPO_PEDIDO = dataTableReader["TP_TIPO_PEDIDO"].ToString();
                if (dataTableReader["DT_ENVIO"] != DBNull.Value)
                {
                    listaSolicitacaoPecas.DT_ENVIO = Convert.ToDateTime(dataTableReader["DT_ENVIO"]).ToString("dd/MM/yyyy");
                }
                if (dataTableReader["DT_Aprovacao"] != DBNull.Value)
                {
                    listaSolicitacaoPecas.DT_Aprovacao = Convert.ToDateTime(dataTableReader["DT_Aprovacao"]).ToString("dd/MM/yyyy");
                }
                if (dataTableReader["nidUsuario"] != DBNull.Value)
                {
                    Int64 IdUser = Convert.ToInt64(dataTableReader["nidUsuario"].ToString());
                    if (IdUser == 0 || IdUser == null)
                        listaSolicitacaoPecas.UsuarioSolicitante = null;
                    else
                    {
                        var user = new UsuarioData().ObterListaUsuarioSinc(IdUser).FirstOrDefault(x => x.nidUsuario == IdUser);
                        listaSolicitacaoPecas.UsuarioSolicitante = user.cnmNome;
                    }
                }
                if (dataTableReader["nidUsuarioAprovador"] != DBNull.Value)
                {
                    Int64 IdUser = Convert.ToInt64(dataTableReader["nidUsuarioAprovador"].ToString());
                    if (IdUser == 0 || IdUser == null)
                        listaSolicitacaoPecas.UsuarioAprovador = null;
                    else
                    {
                        var user = new UsuarioData().ObterListaUsuarioSinc(IdUser).FirstOrDefault(x => x.nidUsuario == IdUser);
                        listaSolicitacaoPecas.UsuarioAprovador = user.cnmNome;
                    }
                }
                if (dataTableReader["DT_RECEBIMENTO"] != DBNull.Value)
                {
                    listaSolicitacaoPecas.DT_RECEBIMENTO = Convert.ToDateTime(dataTableReader["DT_RECEBIMENTO"]).ToString("dd/MM/yyyy");
                }

                if (dataTableReader["Responsavel"] != DBNull.Value)
                {
                    listaSolicitacaoPecas.Responsavel = dataTableReader["Responsavel"].ToString();
                }
                else if (listaSolicitacaoPecas.TP_TIPO_PEDIDO == "C" && dataTableReader["TX_NOMERESPONSAVELPECAS"] != DBNull.Value)
                {
                    listaSolicitacaoPecas.Responsavel = dataTableReader["TX_NOMERESPONSAVELPECAS"].ToString();
                }
                else
                {
                    listaSolicitacaoPecas.Responsavel = "";
                }

                if (dataTableReader["Telefone"] != DBNull.Value)
                {
                    listaSolicitacaoPecas.Telefone = dataTableReader["Telefone"].ToString();
                }
                else
                {
                    if (listaSolicitacaoPecas.TP_TIPO_PEDIDO == "A")
                    {
                        var tecnico = new TecnicoData().ObterTecnico(listaSolicitacaoPecas.tecnico.CD_TECNICO).FirstOrDefault();

                        if (tecnico != null)
                            listaSolicitacaoPecas.Telefone = tecnico.TX_TELEFONE;

                    }
                    else if (listaSolicitacaoPecas.TP_TIPO_PEDIDO == "C" && dataTableReader["TX_TELEFONERESPONSAVELPECAS"] != DBNull.Value)
                    {
                        listaSolicitacaoPecas.Telefone = dataTableReader["TX_TELEFONERESPONSAVELPECAS"].ToString();
                    }
                    else
                    {
                        listaSolicitacaoPecas.Telefone = "";
                    }
                }

                if (dataTableReader["EnviaBPCS"] != DBNull.Value)
                {
                    listaSolicitacaoPecas.EnviaBPCS = dataTableReader["EnviaBPCS"].ToString();
                }
                else
                {
                    listaSolicitacaoPecas.EnviaBPCS = "";
                }

                if (dataTableReader["Origem"] != DBNull.Value)
                {
                    if (dataTableReader["Origem"].ToString() == "")
                    {
                        listaSolicitacaoPecas.Origem = "";
                    }
                    else if (dataTableReader["Origem"].ToString() == "W")
                    {
                        listaSolicitacaoPecas.Origem = "Web";
                    }
                    else if (dataTableReader["Origem"].ToString() == "A")
                    {
                        listaSolicitacaoPecas.Origem = "App";
                    }

                }

                listaSolicitacaoPecas.TX_OBS = dataTableReader["TX_OBS"].ToString();
                listaSolicitacaoPecas.PENDENTE = dataTableReader["PENDENTE"].ToString();
                listaSolicitacaoPecas.FL_EMERGENCIA = dataTableReader["FL_EMERGENCIA"].ToString();
                listaSolicitacaoPecas.NR_DOC_ORI = Convert.ToInt64("0" + dataTableReader["NR_DOC_ORI"]);
                listaSolicitacaoPecas.statusPedido.ID_STATUS_PEDIDO = Convert.ToInt64("0" + dataTableReader["ID_STATUS_PEDIDO"]);
                listaSolicitacaoPecas.statusPedido.DS_STATUS_PEDIDO = dataTableReader["DS_STATUS_PEDIDO"].ToString();
                
                listaSolicitacaoPecas.DS_TP_TIPO_PEDIDO = ControlesUtility.Dicionarios.TipoPedido().Where(x => x.Value == dataTableReader["TP_TIPO_PEDIDO"].ToString()).ToArray()[0].Key;
                listaSolicitacaoPecas.cliente.CD_CLIENTE = Convert.ToInt32("0" + dataTableReader["CD_CLIENTE"]);
                listaSolicitacaoPecas.CD_PEDIDO = Convert.ToInt64("0" + dataTableReader["CD_PEDIDO"]);
                listaSolicitacaoPecas.CD_PEDIDO_Formatado = Convert.ToInt64("0" + dataTableReader["CD_PEDIDO"]).ToString("000000");
            }
            catch(Exception ex)
            {

            }
            

        }

        protected void EnviarEmailPedido(PedidoEntity pedidoEntity, string msgStatus, ref string Mensagem)
        {
            //Método Modelo envio de email
            try
            {
                string emailsSolicitacaoPeca = ControlesUtility.Parametro.ObterValorParametro("emailsSolicitacaoPeca");

                if (string.IsNullOrEmpty(emailsSolicitacaoPeca))
                {
                    return;
                }

                TecnicoEntity tecnicoEntity = new TecnicoEntity();
                tecnicoEntity.CD_TECNICO = pedidoEntity.tecnico.CD_TECNICO;
                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        tecnicoEntity.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                        tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                        tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                        tecnicoEntity.usuario.cdsEmail = "";
                        tecnicoEntity.usuarioCoordenador.cdsEmail = "";
                        if (dataTableReader["cdsEmail"] != DBNull.Value)
                        {
                            tecnicoEntity.usuario.cdsEmail = dataTableReader["cdsEmail"].ToString();
                        }
                        if (dataTableReader["cdsEmailCoordenador"] != DBNull.Value)
                        {
                            tecnicoEntity.usuarioCoordenador.cdsEmail = dataTableReader["cdsEmailCoordenador"].ToString();
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                PedidoEntity _pedidoEntity = new PedidoEntity();
                _pedidoEntity.ID_PEDIDO = pedidoEntity.ID_PEDIDO;
                dataTableReader = new PedidoData().ObterLista(_pedidoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        pedidoEntity.DT_CRIACAO = Convert.ToDateTime(dataTableReader["DT_CRIACAO"]);
                        pedidoEntity.CD_PEDIDO = Convert.ToInt64(dataTableReader["CD_PEDIDO"]);
                        pedidoEntity.FL_EMERGENCIA = dataTableReader["FL_EMERGENCIA"].ToString();
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                string msgEmergencia = "Não";
                if (pedidoEntity.FL_EMERGENCIA == "S")
                {
                    msgEmergencia = "<strong>SIM</strong>";
                }

                // Envia a requisição de troca de senha por e-mail
                MailSender mailSender = new MailSender();

                string mailTo = emailsSolicitacaoPeca;
                if (msgStatus != "SOLICITADO")
                {
                    mailTo = "";
                    if (tecnicoEntity.usuario.cdsEmail != "")
                    {
                        mailTo += tecnicoEntity.usuario.cdsEmail + ";";
                    }
                    if (tecnicoEntity.usuarioCoordenador.cdsEmail != "")
                    {
                        mailTo += tecnicoEntity.usuarioCoordenador.cdsEmail + ";";
                    }
                    //if (mailTo != "")
                    //{
                    //    mailTo = mailTo.Remove(mailTo.Length - 1);
                    //}
                }
                string mailSubject = "3M.Comodato - Pedido de Peça ( " + msgStatus + " )";
                string mailMessage = string.Empty;
                System.Net.Mail.Attachment Attachments = null;
                string mailCopy = null;

                var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                string Conteudo = string.Empty;
                string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                //URLSite += "/SolicitacaoPecas/Editar?idKey=" + ControlesUtility.Criptografia.Criptografar(tecnicoEntity.CD_TECNICO + "|" + pedidoEntity.ID_PEDIDO.ToString() + "|" + pedidoEntity.TP_TIPO_PEDIDO + "|Solicitação");

                Conteudo += "<p>Um Pedido de Peças acaba de ser <strong>" + msgStatus + "</strong>!</p>";
                Conteudo += "<p>Segue dados do pedido:</p>";
                Conteudo += "<p>Técnico: " + tecnicoEntity.NM_TECNICO + " - " + tecnicoEntity.empresa.NM_Empresa + "<br/>";
                Conteudo += "Pedido: <strong>" + Convert.ToInt64(pedidoEntity.CD_PEDIDO).ToString("000000") + "</strong><br/>";
                Conteudo += "Data: " + Convert.ToDateTime(pedidoEntity.DT_CRIACAO).ToString("dd/MM/yyyy") + "<br/>";
                Conteudo += "Status do pedido: " + msgStatus + "<br/>";
                Conteudo += "Tipo: <strong>";
                if (pedidoEntity.TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Value)
                {
                    Conteudo += ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Key + "</strong><br/>";
                }
                else if (pedidoEntity.TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[1].Value)
                {
                    Conteudo += ControlesUtility.Dicionarios.TipoPedido().ToArray()[1].Key + "</strong><br/>";
                }
                else if (pedidoEntity.TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[2].Value)
                {
                    Conteudo += ControlesUtility.Dicionarios.TipoPedido().ToArray()[2].Key + "</strong><br/>";
                }
                Conteudo += "Emergência: " + msgEmergencia + "<br/></p>";
                Conteudo += "<p>Por favor, clique no link abaixo ou copie seu conteúdo para o Navegador WEB:</p>";
                Conteudo += "<p><a href='" + URLSite + "'></a>" + URLSite + "</p>";

                MensagemEmail.Replace("#Conteudo", Conteudo);
                mailMessage = MensagemEmail.ToString();

                mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                Mensagem = "Não foi possível enviar e-mail para este pedido!";
            }
        }
    }
}
