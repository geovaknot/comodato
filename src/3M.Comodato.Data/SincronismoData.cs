using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace _3M.Comodato.Data
{
    public class SincronismoData
    {
        private const string CODIGO_GRUPO_RESPONSAVEL = "TR3M";
        private const int VALOR_ZERADO = 0;
        private const int PEDIDO_STATUS_SOLICITADO = 2;
        private const int PEDIDO_STATUS_RECEBIDO = 4;
        private const int PEDIDO_STATUS_RECEBIDO_PENDENCIA = 6;
        private const int OS_STATUS_FINALIZADA = 3;
        private const int OS_STATUS_CANCELADO = 4;
        private const int OPERACAO_ESTOQUE_ENTRADA = 1;
        private const int OPERACAO_ESTOQUE_SAIDA = 2;
        private const string ACTION_ALTERAR = "Alterar";

        readonly Database _db;
        DbCommand dbCommand;

        public SincronismoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }


        /// <summary>
        /// Obtem lista da tabela de Sincronismo para o sincronismo Mobile
        /// </summary>
        /// <param></param>
        /// <returns></returns>  
        public IList<LogSincronismoEntity> ObterListaSincronismoSinc(Int64 idUsuario)
        {
            try
            {
                IList<LogSincronismoEntity> listaSincronismo = new List<LogSincronismoEntity>();

                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         @"SELECT dt_data_sincronismo,
	                                              st_status_sincronismo
                                             FROM tbLogSincronismo
                                            WHERE id_usuario = @ID_USUARIO
                                              AND CAST(dt_data_sincronismo AS DATE) >= CAST(DATEADD(day, -5, GETDATE()) AS DATE)
                                            ORDER BY CAST(dt_data_sincronismo AS DATE) DESC, CAST(dt_data_sincronismo AS TIME)";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", SqlDbType.BigInt).Value = idUsuario;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            LogSincronismoEntity logsincronismo = new LogSincronismoEntity
                            {
                                dt_data_sincronismo = Convert.ToDateTime(SDR["dt_data_sincronismo"]),
                                st_status_sincronismo = Convert.ToString(SDR["st_status_sincronismo"] is DBNull ? " " : SDR["st_status_sincronismo"].ToString())
                            };

                            listaSincronismo.Add(logsincronismo);
                        }

                        cnx.Close();
                        return listaSincronismo;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LimparLogSincronismoUsuario(Int64 idUsuario)
        {
            SqlConnection conexao = new SqlConnection(_db.ConnectionString);
            conexao.Open();
            SqlTransaction transacao = conexao.BeginTransaction();

            try
            {
                using (SqlCommand comand = new SqlCommand())
                {
                    comand.Connection = conexao;
                    comand.Transaction = transacao;
                    comand.CommandType = CommandType.Text;

                    comand.CommandText =
                        @"DELETE FROM tbLogSincronismo
                           WHERE id_usuario = @ID_USUARIO
                             AND CAST(dt_data_sincronismo AS DATE) <= CAST(DATEADD(DAY, -30, GETDATE()) AS DATE)";

                    comand.Parameters.Add("@ID_USUARIO", SqlDbType.BigInt).Value = idUsuario;
                    comand.Connection = conexao;
                    comand.ExecuteNonQuery();
                }

                transacao.Commit();
            }
            catch (Exception ex)
            {
                transacao.Rollback();
                throw new Exception($"Falhar limpar log sincronismo do usuário. {ex.Message}");
            }
            finally
            {
                transacao.Dispose();
                conexao.Close();
                conexao.Dispose();
            }
        }

        public void GravarDiversosSinc(Int64 idUsuario, List<AgendaSinc> listaAgenda, List<VisitaSinc> listaVisita, List<VisitaPadraoEntity> listaVisitaPadrao, 
            List<LogStatusVisitaSinc> listaLogStatusVisita, List<OsSinc> listaOS, List<OSPadraoEntity> listaOSPadrao, List<PecaOSSinc> listaPecaOS, 
            List<PendenciaOSSinc> listaPendenciaOS, List<LogStatusOSSinc> listaLogStatusOS, List<EstoqueMoviSinc> listaEstoqueMovi, ref List<PedidoSinc> listaPedido, 
            List<PedidoPecaSinc> listaPedidoPeca, List<RelatorioReclamacaoSincEntity> listaRR, List<RRComentSincEntity> listRRComent, List<int> CodigosNotificacao,
            List<PendenciaOSSinc> listPendenciaOSOutros, List<PedidoPecaLogSinc> listaPedidoPecaLog, List<PedidoPecaSinc> listaPedidoPecaRecebimento)
        {
            try
            {
                if (listaAgenda != null)
                    atualizaAgenda(idUsuario, listaAgenda);

                if (listaVisitaPadrao != null)
                    atualizaVisita(idUsuario, listaVisita, listaVisitaPadrao, listaLogStatusVisita, listaOS, listaOSPadrao, listaPecaOS, 
                        listaPendenciaOS, listaLogStatusOS, listaEstoqueMovi, ref listaPedido, listaPedidoPeca, listaRR, listRRComent, 
                        CodigosNotificacao, listPendenciaOSOutros, listaPedidoPecaLog, listaPedidoPecaRecebimento);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GravarOSSinc(Int64 idUsuario, List<OSPadraoEntity> listaOSPadrao, List<PecaOSSinc> listaPecaOS,
            List<PendenciaOSSinc> listaPendenciaOS, List<RelatorioReclamacaoSincEntity> listaRR, List<PendenciaOSSinc> listaPendenciaOSOutros)
        {
            try
            {
                if (listaOSPadrao != null)
                    atualizaOS(idUsuario, listaOSPadrao, listaPecaOS, listaPendenciaOS, listaRR, listaPendenciaOSOutros);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GravarPedidoPecaSinc(Int64 idUsuario, ref List<PedidoSinc> listaPedido, List<PedidoPecaSinc> listaPedidoPeca, List<PedidoPecaLogSinc> listaPedidoPecaLog)
        {
            try
            {
                if (listaPedido != null)
                    atualizaPedidoPeca(idUsuario, ref listaPedido, listaPedidoPeca, listaPedidoPecaLog);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void atualizaVisita(long idUsuario, List<VisitaSinc> listaVisita, List<VisitaPadraoEntity> listaVisitaPadrao,
            List<LogStatusVisitaSinc> listaLogStatusVisita, List<OsSinc> listaOS, List<OSPadraoEntity> listaOSPadrao, List<PecaOSSinc> listaPecaOS,
            List<PendenciaOSSinc> listaPendenciaOS, List<LogStatusOSSinc> listaLogStatusOS, List<EstoqueMoviSinc> listaEstoqueMovi, ref List<PedidoSinc> listaPedido,
            List<PedidoPecaSinc> listaPedidoPeca, List<RelatorioReclamacaoSincEntity> listaRR, List<RRComentSincEntity> listaRRComent, List<int> CodigosNotificacao,
            List<PendenciaOSSinc> listPendenciaOSOutros, List<PedidoPecaLogSinc> listaPedidoPecaLog, List<PedidoPecaSinc> listaPedidoPecaRecebimento)
        {
            SqlConnection cnx = new SqlConnection(_db.ConnectionString);
            cnx.Open();
            SqlTransaction trans = cnx.BeginTransaction();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandType = CommandType.Text;
                    Int64? idVisita = null;

                    #region Visita

                    if (listaVisitaPadrao.Any(v => v.TOKEN == 0))
                        throw new Exception($"Existe visita sem token informado na sincronização enviada.");

                    foreach (VisitaPadraoEntity itemVisita in listaVisitaPadrao)
                    {
                        cmd.CommandText = @"BEGIN
                                            DECLARE @TB_TEMP_ID TABLE (ID BIGINT);
                                             UPDATE tbVISITAPADRAO WITH(UPDLOCK, SERIALIZABLE)
                                                SET ST_STATUS_VISITA = @ST_STATUS_VISITA,
                                                    CD_CLIENTE = @CD_CLIENTE, 
                                                    CD_TECNICO = @CD_TECNICO, 
                                                    DS_OBSERVACAO = @DS_OBSERVACAO, 
                                                    HR_INICIO = @HR_INICIO, 
                                                    HR_FIM = @HR_FIM, 
                                                    CD_MOTIVO_VISITA = @CD_MOTIVO_VISITA,
                                                    Email = @Email,
                                                    DS_RESPONSAVEL = @DS_RESPONSAVEL,
                                                    nidUsuarioAtualizacao = @nidUsuarioAtualizacao,
                                                    dtmDataHoraAtualizacao = @DataAtualizacaoVisita
                                                    OUTPUT inserted.ID_VISITA INTO @TB_TEMP_ID
                                              WHERE TOKEN = @TOKEN 
                                            IF @@ROWCOUNT = 0 
                                            BEGIN 
                                                INSERT INTO tbVISITAPADRAO 
                                                       ( DT_DATA_VISITA, ST_STATUS_VISITA, CD_CLIENTE, CD_TECNICO, DS_OBSERVACAO, HR_INICIO, HR_FIM, CD_MOTIVO_VISITA,
                                                         nidUsuarioAtualizacao, dtmDataHoraAtualizacao, TOKEN, Email, DS_RESPONSAVEL, Origem) 
                                                       OUTPUT inserted.ID_VISITA INTO @TB_TEMP_ID
                                                VALUES ( @DT_DATA_VISITA, @ST_STATUS_VISITA, @CD_CLIENTE, @CD_TECNICO, @DS_OBSERVACAO, @HR_INICIO, @HR_FIM, @CD_MOTIVO_VISITA, 
                                                         @nidUsuarioAtualizacao, getdate(), @TOKEN, @Email, @DS_RESPONSAVEL, @Origem );
                                            END;
                                            SELECT ID from @TB_TEMP_ID; 
                                            END";

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ST_STATUS_VISITA", SqlDbType.Int).Value = itemVisita.TpStatusVisita.ST_STATUS_VISITA;
                        cmd.Parameters.Add("@CD_CLIENTE", SqlDbType.Int).Value = itemVisita.Cliente.CD_CLIENTE;
                        cmd.Parameters.Add("@CD_TECNICO", SqlDbType.VarChar).Value = itemVisita.Tecnico.CD_TECNICO;
                        cmd.Parameters.Add("@DS_OBSERVACAO", SqlDbType.VarChar).Value = itemVisita.DS_OBSERVACAO ?? (object)DBNull.Value;

                        if (string.IsNullOrWhiteSpace(itemVisita.HR_INICIO))
                            cmd.Parameters.Add("@HR_INICIO", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            cmd.Parameters.Add("@HR_INICIO", SqlDbType.VarChar).Value = itemVisita.HR_INICIO;

                        if (string.IsNullOrWhiteSpace(itemVisita.HR_FIM))
                            cmd.Parameters.Add("@HR_FIM", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            cmd.Parameters.Add("@HR_FIM", SqlDbType.VarChar).Value = itemVisita.HR_FIM;

                        cmd.Parameters.Add("@CD_MOTIVO_VISITA", SqlDbType.Int).Value = itemVisita.TpMotivoVisita.CD_MOTIVO_VISITA;
                        cmd.Parameters.Add("@DataAtualizacaoVisita", SqlDbType.DateTime).Value = itemVisita.dtmDataHoraAtualizacao == DateTime.MinValue ? DateTime.Now : itemVisita.dtmDataHoraAtualizacao;
                        cmd.Parameters.Add("@nidUsuarioAtualizacao", SqlDbType.BigInt).Value = idUsuario;
                        cmd.Parameters.Add("@DT_DATA_VISITA", SqlDbType.DateTime).Value = itemVisita.DT_DATA_VISITA;
                        cmd.Parameters.Add("@TOKEN", SqlDbType.BigInt).Value = itemVisita.TOKEN;
                        cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = itemVisita.Email;
                        cmd.Parameters.Add("@DS_RESPONSAVEL", SqlDbType.VarChar).Value = itemVisita.DS_RESPONSAVEL;
                        cmd.Parameters.Add("@Origem", SqlDbType.VarChar).Value = "A";
                        cmd.Connection = cnx;
                        idVisita = Convert.ToInt64(cmd.ExecuteScalar());

                        if (itemVisita.TpStatusVisita.ST_STATUS_VISITA == 4)
                        {
                            EnviarEmailVisita(itemVisita, idVisita);
                        }
                        
                        if (listaLogStatusVisita != null)
                            grava_LogStatusVisita(ref trans, ref cnx, itemVisita, idUsuario, idVisita);
                    }

                    #endregion

                    #region Ordem de serviço (OS)

                    if (listaOSPadrao?.Count > 0)
                    {
                        if (listaOSPadrao.Any(v => v.TOKEN == 0))
                            throw new Exception($"Existe Ordem de Serviço (OS) sem token informado na sincronização enviada.");

                        grava_OS(ref trans, ref cnx, listaOSPadrao, idUsuario, listaPecaOS, listaPendenciaOS, listaLogStatusOS, listaEstoqueMovi,
                                 listaRR, listaRRComent, listPendenciaOSOutros);
                    }
                    
                    if (listPendenciaOSOutros?.Count > 0)
                    {
                        if (listPendenciaOSOutros.Any(v => v.TOKEN == 0))
                            throw new Exception($"Existe pendência sem token informado na sincronização enviada. Lista de Pedência outros.");

                        GravarPendenciaOS(ref trans, ref cnx, listPendenciaOSOutros, idUsuario);
                    }

                    #endregion

                    #region Solicitação de Peças (Pedido)

                    long? idpedido = null;
                    if (listaPedido != null)
                    {
                        if (listaPedido.Any(v => v.TOKEN == 0))
                            throw new Exception($"Existe Solicitação de Peças (Pedido) sem token informado na sincronização enviada.");

                        if (listaPedidoPeca.Any(v => v.TOKEN == 0))
                            throw new Exception($"Existe peça de solicitação sem token informado na sincronização enviada.");

                        int contadorPecaComItem = 0;
                        int contadorPecaSemItem = 0;

                        foreach (var peca in listaPedidoPeca)
                        {
                            if (peca.CD_PECA != "" && peca.CD_PECA != null)
                            {
                                contadorPecaComItem += 1;
                            }
                            else
                            {
                                contadorPecaSemItem += 1;
                            }
                        }

                        if (contadorPecaSemItem > 0 && contadorPecaComItem > 0)
                        {
                            throw new Exception($"Não pode ser adicionado Peça com item e Peça sem item ao mesmo pedido!");
                        }

                        grava_Pedido(ref trans, ref cnx, ref listaPedido, listaPedidoPeca, listaPedidoPecaLog, idUsuario, ref idpedido);
                    }

                    #region Recebimento de Peças 

                    if (listaPedidoPecaRecebimento?.Count > 0)
                        ProcessarRecebimentoPecaPedido(idUsuario, listaPedidoPecaRecebimento, ref cnx, ref trans);

                    #endregion

                    #endregion

                    #region Notificação

                    if (CodigosNotificacao != null && CodigosNotificacao.Count > 0)
                        ModificarNotificacaoParaVisualizada(CodigosNotificacao);

                    #endregion

                    cmd.Dispose();
                }
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                trans.Dispose();
                cnx.Close();
                cnx.Dispose();
            }
        }

        private void ProcessarRecebimentoPecaPedido(long idUsuario, List<PedidoPecaSinc> listaPedidoPecaRecebimento, ref SqlConnection cnx, ref SqlTransaction trans)
        {
            AtualizarEstoqueQuantidadeRecebidaSolicitacaoPeca(ref trans, ref cnx, idUsuario, listaPedidoPecaRecebimento);

            var pedidos = listaPedidoPecaRecebimento.Select(p => p.ID_PEDIDO).Distinct();

            foreach (var pedido in pedidos)
                AtualizarStatusPedido(ref trans, ref cnx, pedido.Value);
        }

        public void ProcessarRecebimentoPecaPedido(long idUsuario, List<PedidoPecaSinc> listaPedidoPecaRecebimento)
        {
            SqlConnection conexao = new SqlConnection(_db.ConnectionString);
            conexao.Open();

            SqlTransaction transacao = conexao.BeginTransaction();
            try
            {
                ProcessarRecebimentoPecaPedido(idUsuario, listaPedidoPecaRecebimento, ref conexao, ref transacao);
                transacao.Commit();
            }
            catch (Exception ex)
            {
                transacao.Rollback();
                throw ex;
            }
            finally
            {
                transacao.Dispose();
                conexao.Close();
                conexao.Dispose();
            }
        }

        private void AtualizarEstoqueQuantidadeRecebidaSolicitacaoPeca(ref SqlTransaction transacao, ref SqlConnection conexao, long idUsuario, List<PedidoPecaSinc> listaPedidoPecaRecebimento)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = conexao;
                    sqlCommand.Transaction = transacao;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "prcPedidoPecaRecebimentoEstoque";

                    foreach (var peca in listaPedidoPecaRecebimento)
                    {
                        var pecaTabela = new PedidoPecaData().ObterListaPedidoPecaPedidoTotal(Convert.ToInt64(peca.ID_PEDIDO)).FirstOrDefault(x => x.ID_ITEM_PEDIDO == peca.ID_ITEM_PEDIDO);

                        if(pecaTabela.QTD_RECEBIDA >= pecaTabela.QTD_APROVADA)
                        {
                            continue;
                        }
                        else
                        {
                            var quantidadepecarecebida = peca.QTD_RECEBIDA + pecaTabela.QTD_RECEBIDA;

                            if (quantidadepecarecebida > pecaTabela.QTD_APROVADA) { 
                                peca.QTD_RECEBIDA = pecaTabela.QTD_APROVADA - pecaTabela.QTD_RECEBIDA;
                                peca.ST_STATUS_ITEM = '5';
                            }
                            sqlCommand.Parameters.Clear();
                            sqlCommand.Parameters.Add(new SqlParameter("@p_ID_ITEM_PEDIDO", SqlDbType.BigInt) { Value = peca.ID_ITEM_PEDIDO });
                            sqlCommand.Parameters.Add(new SqlParameter("@p_ID_PEDIDO", SqlDbType.BigInt) { Value = peca.ID_PEDIDO });
                            sqlCommand.Parameters.Add(new SqlParameter("@p_QTD_RECEBIDA", SqlDbType.Decimal) { Value = peca.QTD_RECEBIDA });
                            sqlCommand.Parameters.Add(new SqlParameter("@p_nidUsuarioAtualizacao", SqlDbType.BigInt) { Value = idUsuario });
                            sqlCommand.Parameters.Add(new SqlParameter("@p_status_item", SqlDbType.Char) { Value = peca.ST_STATUS_ITEM });

                            var parametroMensagem = new SqlParameter("@p_Mensagem", SqlDbType.VarChar, 8000) { Direction = ParameterDirection.Output };
                            sqlCommand.Parameters.Add(parametroMensagem);
                            sqlCommand.ExecuteNonQuery();

                            var mensagemValidacaoMovimentaEstoque = parametroMensagem.Value.ToString();

                            if (!string.IsNullOrWhiteSpace(parametroMensagem.Value.ToString()))
                                throw new Exception(parametroMensagem.Value.ToString());
                        }

                        
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AtualizarStatusPedido(ref SqlTransaction transacao, ref SqlConnection conexao, long idPedido)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = conexao;
                    sqlCommand.Transaction = transacao;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "prcAtualizaStatusPedido";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_ID_PEDIDO", SqlDbType.BigInt) { Value = idPedido });
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void atualizaOS(long idUsuario, List<OSPadraoEntity> listaOSPadrao, List<PecaOSSinc> listaPecaOS,
            List<PendenciaOSSinc> listaPendenciaOS, List<RelatorioReclamacaoSincEntity> listaRR, List<PendenciaOSSinc> listaPendenciaOSOutros)
        {
            SqlConnection cnx = new SqlConnection(_db.ConnectionString);
            cnx.Open();
            SqlTransaction trans = cnx.BeginTransaction();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Transaction = trans;
                    cmd.CommandType = CommandType.Text;

                    if (listaOSPadrao != null)
                        grava_OS(ref trans, ref cnx, listaOSPadrao, idUsuario, listaPecaOS, listaPendenciaOS, null, null, listaRR, null, listaPendenciaOSOutros);

                    cmd.Dispose();
                }
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                trans.Dispose();
                cnx.Close();
                cnx.Dispose();
            }
        }

        private void atualizaPedidoPeca(long idUsuario, ref List<PedidoSinc> listaPedido, List<PedidoPecaSinc> listaPedidoPeca, List<PedidoPecaLogSinc> listaPedidoPecaLog)
        {
            SqlConnection cnx = new SqlConnection(_db.ConnectionString);
            cnx.Open();
            SqlTransaction trans = cnx.BeginTransaction();
            try
            {
                //============== Pedido ------------------------------------------------
                long? idpedido = null;

                if (listaPedido != null)
                    grava_Pedido(ref trans, ref cnx, ref listaPedido, listaPedidoPeca, listaPedidoPecaLog, idUsuario, ref idpedido);

                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                trans.Dispose();
                cnx.Close();
                cnx.Dispose();
            }
        }

        //TODO: Edgar - Verificar se deve retirar
        private Int64? verificaVisitaExistente(VisitaPadraoEntity itemVisita, ref SqlTransaction trans, ref SqlConnection cnx)
        {
            int statusVisita = 0;
            if (itemVisita.ID_VISITA > 0) return itemVisita.ID_VISITA;

            try
            {
                using (SqlCommand cmd22 = new SqlCommand())
                {
                    cmd22.Connection = cnx;
                    cmd22.Transaction = trans;
                    cmd22.CommandType = CommandType.Text;
                    Int64? idVisita = null;
                    cmd22.CommandText = @"  SELECT top 1 ID_VISITA,ST_STATUS_VISITA " +
                                       " from tbVISITAPADRAO (nolock) " +
                                       " WHERE CD_CLIENTE = @CD_CLIENTE AND " +
                                       "       CD_TECNICO = @CD_TECNICO AND " +
                                       "       Convert(Date, DT_DATA_VISITA,121) = Convert(Date, @DT_DATA_VISITA,121) ";
                    cmd22.Parameters.Clear();
                    cmd22.Parameters.Add("@CD_CLIENTE", System.Data.SqlDbType.Int).Value = itemVisita.Cliente.CD_CLIENTE;
                    cmd22.Parameters.Add("@CD_TECNICO", System.Data.SqlDbType.VarChar).Value = itemVisita.Tecnico.CD_TECNICO;
                    cmd22.Parameters.Add("@DT_DATA_VISITA", System.Data.SqlDbType.DateTime).Value = itemVisita.DT_DATA_VISITA;

                    SqlDataReader SDR = cmd22.ExecuteReader();
                    while (SDR.Read())
                    {

                        idVisita = Convert.ToInt64(SDR["ID_VISITA"].ToString());
                        statusVisita = Convert.ToInt16(SDR["ST_STATUS_VISITA"].ToString());
                    }

                    SDR.Close();
                    cmd22.Dispose();

                    if (idVisita > 0)
                    {
                        if (itemVisita.ID_VISITA > 0)
                            return itemVisita.ID_VISITA;
                        else if (statusVisita == 4 || statusVisita == 5) // 4 - Finalizada / 5 - Cancelada
                            return 1;
                        else
                            return -1;
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
            }

            return 0;
        }

        private void grava_PedidoPeca(ref SqlTransaction trans, ref SqlConnection cnx, List<PedidoPecaSinc> listaPedidoPeca, long idUsuario, long? idpedido,
                             String _IDENTIFICADOR_PK_ID_PEDIDO)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnx;
                    cmd.Transaction = trans;
                    cmd.CommandType = CommandType.Text;

                    foreach (PedidoPecaSinc itemPedidoPeca in listaPedidoPeca.Where(e => e.IDENTIFICADOR_FK_ID_PEDIDO == _IDENTIFICADOR_PK_ID_PEDIDO))
                    {
                        cmd.CommandText = @"UPDATE TB_PEDIDO_PECA
                                               SET DS_OBSERVACAO = @DS_OBSERVACAO, 
	                                               QTD_RECEBIDA = @QTD_RECEBIDA,
                                                   DS_DIR_FOTO = @DS_DIR_FOTO
                                             WHERE TOKEN = @TOKEN
                                            IF @@ROWCOUNT = 0 
                                            BEGIN
                                               INSERT INTO TB_PEDIDO_PECA 
                                                      ( ID_PEDIDO, CD_PECA, QTD_SOLICITADA, QTD_APROVADA, QTD_APROVADA_3M1, QTD_APROVADA_3M2, TX_APROVADO, NR_DOC_ORI,
                                                        QTD_RECEBIDA, ST_STATUS_ITEM, DS_OBSERVACAO, DS_DIR_FOTO, ID_ESTOQUE_DEBITO, VL_PECA,
                                                        TOKEN, TIPO_PECA, DESCRICAO_PECA
                                                      )
                                               VALUES ( @ID_PEDIDO, @CD_PECA, @QTD_SOLICITADA, @QTD_APROVADA, @QTD_APROVADA_3M1, @QTD_APROVADA_3M2, @TX_APROVADO, @NR_DOC_ORI,
                                                        @QTD_RECEBIDA, @ST_STATUS_ITEM, @DS_OBSERVACAO, @DS_DIR_FOTO, null, (SELECT top 1 VL_PECA FROM tb_PECA pe WHERE pe.CD_PECA = @CD_PECA),
                                                        @TOKEN, @TIPO_PECA, @DESCRICAO_PECA
                                                      )
                                            END";

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_PEDIDO", SqlDbType.Int).Value = idpedido is null ? itemPedidoPeca.ID_PEDIDO : idpedido;
                        cmd.Parameters.Add("@CD_PECA", SqlDbType.VarChar).Value = Convert.ToString(itemPedidoPeca.CD_PECA is null ? "0" : itemPedidoPeca.CD_PECA);
                        cmd.Parameters.Add("@QTD_SOLICITADA", SqlDbType.Int).Value = itemPedidoPeca.QTD_SOLICITADA ?? (object)DBNull.Value;
                        //cmd.Parameters.Add("@QTD_APROVADA", SqlDbType.Decimal).Value = itemPedidoPeca.QTD_SOLICITADA ?? (object)DBNull.Value;  Linha antiga recebendo Qtd_Aprovada = a QTD_Solicitada
                        cmd.Parameters.Add("@QTD_APROVADA", SqlDbType.Decimal).Value = 0;
                        cmd.Parameters.Add("@QTD_APROVADA_3M1", SqlDbType.Decimal).Value = (object)DBNull.Value;
                        cmd.Parameters.Add("@QTD_APROVADA_3M2", SqlDbType.Decimal).Value = (object)DBNull.Value;
                        cmd.Parameters.Add("@TX_APROVADO", SqlDbType.VarChar).Value = 'S';
                        cmd.Parameters.Add("@NR_DOC_ORI", SqlDbType.Int).Value = itemPedidoPeca.NR_DOC_ORI ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@QTD_RECEBIDA", SqlDbType.Int).Value = itemPedidoPeca.QTD_RECEBIDA ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@ST_STATUS_ITEM", SqlDbType.Char).Value = itemPedidoPeca.ST_STATUS_ITEM;
                        cmd.Parameters.Add("@DS_OBSERVACAO", SqlDbType.NVarChar).Value = itemPedidoPeca.DS_OBSERVACAO ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@DS_DIR_FOTO", SqlDbType.NVarChar).Value = itemPedidoPeca.DS_DIR_FOTO ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@ID_ESTOQUE_DEBITO", SqlDbType.BigInt).Value = itemPedidoPeca.ID_ESTOQUE_DEBITO ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@idUsuario", SqlDbType.BigInt).Value = idUsuario;
                        cmd.Parameters.Add("@TIPO_PECA", SqlDbType.TinyInt).Value = itemPedidoPeca.TIPO_PECA;
                        cmd.Parameters.Add("@DESCRICAO_PECA", SqlDbType.VarChar).Value = itemPedidoPeca.DESCRICAO_PECA;
                        cmd.Parameters.Add("@TOKEN", SqlDbType.BigInt).Value = itemPedidoPeca.TOKEN;
                        cmd.Connection = cnx;
                        int c = cmd.ExecuteNonQuery();

                        

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void grava_PedidoPecaLog(ref SqlTransaction trans, ref SqlConnection cnx, List<PedidoPecaLogSinc> listaPedidoPecaLog)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnx;
                    cmd.Transaction = trans;
                    cmd.CommandType = CommandType.Text;

                    foreach (PedidoPecaLogSinc itemPedidoPecaLog in listaPedidoPecaLog)
                    {
                        cmd.CommandText = @"BEGIN
                                               INSERT INTO TB_PEDIDO_PECA_LOG 
                                                      ( ID_ITEM_PEDIDO, DATA_RECEBIMENTO, QTD_PECA_RECEBIDA
                                                      )
                                               VALUES ( @ID_ITEM_PEDIDO, @DATA_RECEBIMENTO, @QTD_PECA_RECEBIDA
                                                      )
                                            END";

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_ITEM_PEDIDO", SqlDbType.Int).Value = itemPedidoPecaLog.ID_ITEM_PEDIDO;
                        cmd.Parameters.Add("@DATA_RECEBIMENTO", SqlDbType.DateTime).Value = itemPedidoPecaLog.DATA_RECEBIMENTO ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@QTD_PECA_RECEBIDA", SqlDbType.Int).Value = itemPedidoPecaLog.QTD_PECA_RECEBIDA ?? (object)DBNull.Value;
                        
                        cmd.Connection = cnx;
                        int c = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void grava_HistoricoOBS(ref SqlTransaction trans, ref SqlConnection cnx, long idUsuario, long? id_pedido, String obs)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnx;
                    cmd.Transaction = trans;
                    cmd.CommandType = CommandType.Text;
                    
                        if (!String.IsNullOrEmpty(obs))
                        {
                        cmd.CommandText = " INSERT INTO tbMensagem " +
                                          " ( ID_USUARIO, DS_Mensagem, ID_PEDIDO,dtmDataHoraAtualizacao,DT_OCORRENCIA " +
                                          " ) " +
                                          " VALUES (  @ID_USUARIO, @DS_Mensagem,@ID_PEDIDO,@dtmDataHoraAtualizacao,@DT_OCORRENCIA) ";
                                              
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.BigInt).Value = idUsuario;
                            cmd.Parameters.Add("@DS_Mensagem", System.Data.SqlDbType.VarChar).Value = obs;
                            cmd.Parameters.Add("@ID_PEDIDO", System.Data.SqlDbType.Int).Value = id_pedido;
                            cmd.Parameters.Add("@dtmDataHoraAtualizacao", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                        cmd.Parameters.Add("@DT_OCORRENCIA", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                        cmd.Connection = cnx;
                            int c = cmd.ExecuteNonQuery();
                        }
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void grava_Pedido(ref SqlTransaction trans, ref SqlConnection cnx, ref List<PedidoSinc> listaPedido, List<PedidoPecaSinc> listaPedidoPeca, List<PedidoPecaLogSinc> listaPedidoPecaLog, long idUsuario, ref long? idpedido)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnx;
                    cmd.Transaction = trans;
                    cmd.CommandType = CommandType.Text;

                    foreach (PedidoSinc itemPedido in listaPedido)
                    {
                        #region Pedido

                        cmd.CommandText = @"BEGIN
                                            DECLARE @TB_TEMP_ID TABLE (ID BIGINT);
                                            insert into @TB_TEMP_ID
                                            SELECT ID_PEDIDO
                                                FROM TB_PEDIDO WITH(UPDLOCK, SERIALIZABLE)
                                                WHERE TOKEN = @TOKEN 
                                            IF @@ROWCOUNT = 0 
                                            BEGIN
                                                INSERT INTO TB_PEDIDO
                                                        ( ID_PEDIDO, CD_TECNICO, NR_DOCUMENTO, DT_CRIACAO, DT_ENVIO, DT_RECEBIMENTO, TX_OBS,
                                                        PENDENTE, NR_DOC_ORI, ID_STATUS_PEDIDO, TP_TIPO_PEDIDO, CD_CLIENTE, Origem, FL_EMERGENCIA, CD_PEDIDO, TOKEN, nidUsuario )
                                                        OUTPUT inserted.ID_PEDIDO INTO @TB_TEMP_ID
                                                VALUES ( (SELECT ISNULL(MAX(ID_PEDIDO), 0) + 1 FROM TB_PEDIDO), @CD_TECNICO, @NR_DOCUMENTO, @DT_CRIACAO, @DT_ENVIO, @DT_RECEBIMENTO, @TX_OBS,
                                                        @PENDENTE, @NR_DOC_ORI, @ID_STATUS_PEDIDO, @TP_TIPO_PEDIDO, @CD_CLIENTE, @Origem, 'S', (SELECT ISNULL(MAX(CD_PEDIDO), 0) + 1 FROM TB_PEDIDO), @TOKEN, @nidUsuario
                                                        );
                                            END;
                                            SELECT ID from @TB_TEMP_ID ORDER BY ID DESC; 
                                            END";

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CD_TECNICO", SqlDbType.VarChar).Value = itemPedido.CD_TECNICO;
                        cmd.Parameters.Add("@NR_DOCUMENTO", SqlDbType.Int).Value = VALOR_ZERADO;
                        cmd.Parameters.Add("@DT_CRIACAO", SqlDbType.DateTime).Value = itemPedido.DT_CRIACAO;
                        cmd.Parameters.Add("@DT_ENVIO", SqlDbType.DateTime).Value = itemPedido.DT_ENVIO ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@DT_RECEBIMENTO", SqlDbType.DateTime).Value = itemPedido.DT_RECEBIMENTO ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@TX_OBS", SqlDbType.VarChar).Value = itemPedido.TX_OBS ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@PENDENTE", SqlDbType.VarChar).Value = itemPedido.PENDENTE ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@NR_DOC_ORI", SqlDbType.Int).Value = itemPedido.NR_DOC_ORI ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@ID_STATUS_PEDIDO", SqlDbType.BigInt).Value = PEDIDO_STATUS_SOLICITADO;
                        cmd.Parameters.Add("@TP_TIPO_PEDIDO", SqlDbType.Char).Value = itemPedido.TP_TIPO_PEDIDO;
                        cmd.Parameters.Add("@CD_CLIENTE", SqlDbType.Int).Value = itemPedido.CD_CLIENTE ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Origem", SqlDbType.VarChar).Value = "A";
                        cmd.Parameters.Add("@CD_PEDIDO", SqlDbType.BigInt).Value = itemPedido.CD_PEDIDO ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@TOKEN", SqlDbType.BigInt).Value = itemPedido.TOKEN;
                        cmd.Parameters.Add("@nidUsuario", SqlDbType.BigInt).Value = idUsuario;
                        cmd.Connection = cnx;
                        idpedido = Convert.ToInt64(cmd.ExecuteScalar());
                        itemPedido.ID_PEDIDO_INSERIDO = idpedido;

                        //TODO: Edgar - No final se o cliente não solicitar, retirar esse código.
                        //============== Hisórico observacao ------------------------------------------------
                        //if (idpedido != null) grava_HistoricoOBS(ref trans, ref cnx, idUsuario, idpedido, itemPedido.TX_OBS);

                        #endregion

                        #region Peças Pedido

                        if (listaPedidoPeca?.Count > 0)
                            grava_PedidoPeca(ref trans, ref cnx, listaPedidoPeca, idUsuario, idpedido, itemPedido.IDENTIFICADOR_PK_ID_PEDIDO);

                        #endregion
                        

                    }

                    #region Log Pedido Peca
                    if (listaPedidoPecaLog != null)
                        grava_PedidoPecaLog(ref trans, ref cnx, listaPedidoPecaLog);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IList<string> ObterPecasOSComMovimentacaoEstoque(ref SqlTransaction transacao, ref SqlConnection conexao, long idOs)
        {
            SqlDataReader SDR = null;

            try
            {
                IList<string> listaPecasOsEstoqueMovimentado = new List<string>();

                using (SqlCommand comand = new SqlCommand())
                {
                    comand.Connection = conexao;
                    comand.Transaction = transacao;
                    comand.CommandType = CommandType.Text;

                    comand.CommandText =
                                        @"select CD_PECA 
                                            from tbEstoqueMovi
                                           where ID_OS = @ID_OS";

                    comand.CommandType = CommandType.Text;

                    comand.Parameters.Clear();
                    comand.Parameters.Add("@ID_OS", SqlDbType.BigInt).Value = idOs;
                    comand.Connection = conexao;
                    SDR = comand.ExecuteReader();

                    while (SDR.Read())
                    {
                        listaPecasOsEstoqueMovimentado.Add(Convert.ToString(SDR["CD_PECA"]));
                    }

                    return listaPecasOsEstoqueMovimentado;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SDR.Close();
            }
        }


        //TODO: Edgar - Retirar 
        private int ObterCodigoEstoque(ref SqlTransaction transacao, ref SqlConnection conexao, char tipoEstoque, long idUsuario, long idCliente)
        {
            try
            {
                using (SqlCommand comand = new SqlCommand())
                {
                    comand.Connection = conexao;
                    comand.Transaction = transacao;
                    comand.CommandType = CommandType.Text;

                    switch (tipoEstoque)
                    {
                        case 'T':
                            {
                                comand.CommandText = "select top 1 id_Estoque from tbEstoque e where e.id_usu_responsavel = @idUsuario  and FL_ATIVO = 'S' ";
                                comand.Parameters.Clear();
                                comand.Parameters.Add("@idUsuario", SqlDbType.BigInt).Value = idUsuario;
                                break;
                            }
                        case 'C':
                            {
                                comand.CommandText = " select top 1 id_estoque from tbEstoque where CD_CLIENTE = @cd_CLIENTE and FL_ATIVO = 'S' ";
                                comand.Parameters.Clear();
                                comand.Parameters.Add("@CD_CLIENTE", SqlDbType.Int).Value = idCliente;
                                break;
                            }
                        default:
                            {
                                throw new Exception($"Tipo de estoque não informado ou estoque inexistente.");
                            }
                    }

                    comand.Connection = conexao;
                    return Convert.ToInt32(comand.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //TODO: Edgar - Retirar 
        private void grava_EstoqueMovi(ref SqlTransaction trans, ref SqlConnection cnx, List<EstoqueMoviSinc> listaEstoqueMovi, long? idUsuario,
                String _IDENTIFICADOR_PK_ID_OS, Int64? _id_os, long cd_cliente)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnx;
                    cmd.Transaction = trans;
                    cmd.CommandType = CommandType.Text;
                    foreach (EstoqueMoviSinc itemEstoqueMovi in listaEstoqueMovi.Where(e => e.IDENTIFICADOR_FK_ID_OS == _IDENTIFICADOR_PK_ID_OS))
                    {
                        if (itemEstoqueMovi.ID_ESTOQUE_MOVI <= 0 || itemEstoqueMovi.ID_ESTOQUE_MOVI == null)
                        {
                            int idEstoque;
                            if (itemEstoqueMovi.ID_ESTOQUE_ORIGEM == 1)
                            {
                                cmd.CommandText = "select top 1 id_Estoque from tbEstoque e where e.id_usu_responsavel = @idUsuario  and FL_ATIVO = 'S' ";
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@idUsuario", System.Data.SqlDbType.BigInt).Value = idUsuario;
                                cmd.Connection = cnx;
                                idEstoque = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                            else
                            {
                                cmd.CommandText = " select top 1 id_estoque from tbEstoque where CD_CLIENTE = @cd_CLIENTE and FL_ATIVO = 'S' ";
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@CD_CLIENTE", System.Data.SqlDbType.Int).Value = cd_cliente;
                                cmd.Connection = cnx;
                                idEstoque = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            if (idEstoque > 0)
                            {
                                cmd.CommandText = " INSERT INTO tbEstoqueMovi " +
                                                  " ( CD_TP_MOVIMENTACAO, ID_OS, DT_MOVIMENTACAO, ID_ESTOQUE, CD_PECA, QT_PECA, " +
                                                  "   ID_USU_MOVI, ID_ESTOQUE_ORIGEM, TP_ENTRADA_SAIDA, CD_CLIENTE, " +
                                                  "   nidUsuarioAtualizacao, dtmDataHoraAtualizacao) " +
                                                  " VALUES ( @CD_TP_MOVIMENTACAO, @ID_OS, @DT_MOVIMENTACAO, " +
                                                  "          (select top 1 id_Estoque from tbEstoque e where e.id_usu_responsavel = @idUsuario) " +
                                                  "         , @CD_PECA, @QT_PECA, " +
                                                  "          @ID_USU_MOVI, (select top 1 id_Estoque from tbEstoque e where e.id_usu_responsavel = @idUsuario) " +
                                                  "        , @TP_ENTRADA_SAIDA, @CD_CLIENTE, " +
                                                  "          @nidUsuarioAtualizacao, @dtmDataHoraAtualizacao ); ";
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@CD_TP_MOVIMENTACAO", System.Data.SqlDbType.Char).Value = itemEstoqueMovi.CD_TP_MOVIMENTACAO;
                                cmd.Parameters.Add("@ID_OS", System.Data.SqlDbType.BigInt).Value = _id_os;
                                cmd.Parameters.Add("@DT_MOVIMENTACAO", System.Data.SqlDbType.DateTime).Value = itemEstoqueMovi.DT_MOVIMENTACAO;
                                cmd.Parameters.Add("@ID_ESTOQUE", System.Data.SqlDbType.BigInt).Value = idEstoque;
                                cmd.Parameters.Add("@CD_PECA", System.Data.SqlDbType.VarChar).Value = itemEstoqueMovi.CD_PECA;
                                cmd.Parameters.Add("@QT_PECA", System.Data.SqlDbType.Decimal).Value = itemEstoqueMovi.QT_PECA;
                                cmd.Parameters.Add("@ID_USU_MOVI", System.Data.SqlDbType.BigInt).Value = itemEstoqueMovi.ID_USU_MOVI;
                                cmd.Parameters.Add("@ID_ESTOQUE_ORIGEM", System.Data.SqlDbType.BigInt).Value = itemEstoqueMovi.ID_ESTOQUE_ORIGEM;
                                cmd.Parameters.Add("@TP_ENTRADA_SAIDA", System.Data.SqlDbType.Char).Value = itemEstoqueMovi.TP_ENTRADA_SAIDA;
                                cmd.Parameters.Add("@CD_CLIENTE", System.Data.SqlDbType.Int).Value = cd_cliente ;
                                cmd.Parameters.Add("@dtmDataHoraAtualizacao", System.Data.SqlDbType.DateTime).Value = DateTime.UtcNow;
                                cmd.Parameters.Add("@nidUsuarioAtualizacao", System.Data.SqlDbType.BigInt).Value = idUsuario;
                                cmd.Parameters.Add("@idUsuario", System.Data.SqlDbType.BigInt).Value = idUsuario;
                                cmd.Connection = cnx;
                                int c = cmd.ExecuteNonQuery();
                                try
                                {
                                    if (itemEstoqueMovi.TP_ENTRADA_SAIDA == 'E')
                                    {
                                        cmd.CommandText = " UPDATE tbEstoquePeca  " +
                                        " SET QT_PECA_ATUAL = QT_PECA_ATUAL + ABS(@QT_PECA) " +
                                        " WHERE CD_PECA = @CD_PECA AND " +
                                        " ID_ESTOQUE =  @ID_ESTOQUE ";
                                    }
                                    else
                                    {
                                        cmd.CommandText = " UPDATE tbEstoquePeca  " +
                                        " SET QT_PECA_ATUAL = QT_PECA_ATUAL - ABS(@QT_PECA) " +
                                        " WHERE CD_PECA = @CD_PECA AND " +
                                        " ID_ESTOQUE =  @ID_ESTOQUE ";
                                    }
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("@ID_ESTOQUE", System.Data.SqlDbType.BigInt).Value = idEstoque;
                                    cmd.Parameters.Add("@QT_PECA", System.Data.SqlDbType.Decimal).Value = itemEstoqueMovi.QT_PECA;
                                    cmd.Parameters.Add("@CD_PECA", System.Data.SqlDbType.VarChar).Value = itemEstoqueMovi.CD_PECA;
                                    cmd.Connection = cnx;
                                    int c2 = cmd.ExecuteNonQuery();
                                    if (c2 == 0)
                                        throw new Exception("Estoque-Peça não encontrado");

                                }
                                catch (Exception ex)
                                {
                                    if (itemEstoqueMovi.ID_ESTOQUE != 0)
                                    {
                                        try
                                        {
                                            cmd.CommandText = " INSERT INTO tbEstoquePeca " +
                                            " ( CD_PECA, QT_PECA_ATUAL, QT_PECA_MIN, DT_ULT_MOVIM, ID_ESTOQUE ) " +
                                            " VALUES ( @CD_PECA,  @QT_PECA, 0, @DT_MOVIMENTACAO,  @ID_ESTOQUE ) ";
                                            cmd.Parameters.Clear();
                                            cmd.Parameters.Add("@ID_ESTOQUE", System.Data.SqlDbType.BigInt).Value = idEstoque;
                                            cmd.Parameters.Add("@QT_PECA", System.Data.SqlDbType.Decimal).Value = itemEstoqueMovi.QT_PECA * -1;
                                            cmd.Parameters.Add("@CD_PECA", System.Data.SqlDbType.VarChar).Value = itemEstoqueMovi.CD_PECA;
                                            cmd.Parameters.Add("@DT_MOVIMENTACAO", System.Data.SqlDbType.DateTime).Value = itemEstoqueMovi.DT_MOVIMENTACAO;
                                            cmd.Connection = cnx;
                                            int c2 = cmd.ExecuteNonQuery();
                                        }
                                        catch (Exception ex2)
                                        {
                                            throw ex2;
                                        }
                                    }
                                    else
                                    {
                                        throw ex;
                                    }
                                }
                            }
                            else
                            {
                                //enviar email comunicando que não tem estoque criado 
                                //registrar log que não tem estoque criado

                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void grava_LogStatusOs(ref SqlTransaction trans, ref SqlConnection cnx, List<LogStatusOSSinc> listaLogStatusOS, long? idUsuario,
                                       String _IDENTIFICADOR_PK_ID_OS, Int64? _id_os)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnx;
                    cmd.Transaction = trans;
                    cmd.CommandType = CommandType.Text;
                    foreach (LogStatusOSSinc itemLogStatusOS in listaLogStatusOS.Where(e => e.IDENTIFICADOR_FK_ID_OS == _IDENTIFICADOR_PK_ID_OS))
                    {
                        if (!(itemLogStatusOS.ID_LOG_STATUS_OS > 0))
                        {
                            cmd.CommandText = " INSERT INTO tbLogStatusOS " +
                                              " ( ID_OS, DT_DATA_LOG_OS, ST_TP_STATUS_VISITA_OS, nidUsuarioAtualizacao, dtmDataHoraAtualizacao) " +
                                              " VALUES ( @ID_OS, @DT_DATA_LOG_OS, @ST_TP_STATUS_VISITA_OS, " +
                                              "          @nidUsuarioAtualizacao, @DataAtualizacaoVisita ) ";
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@ID_OS", System.Data.SqlDbType.BigInt).Value = _id_os;
                            cmd.Parameters.Add("@DT_DATA_LOG_OS", System.Data.SqlDbType.DateTime).Value = itemLogStatusOS.DT_DATA_LOG_OS;
                            cmd.Parameters.Add("@ST_TP_STATUS_VISITA_OS", System.Data.SqlDbType.Char).Value = itemLogStatusOS.ST_TP_STATUS_VISITA_OS;
                            cmd.Parameters.Add("@DataAtualizacaoVisita", System.Data.SqlDbType.DateTime).Value = itemLogStatusOS.dtmDataHoraAtualizacao;
                            cmd.Parameters.Add("@nidUsuarioAtualizacao", System.Data.SqlDbType.BigInt).Value = idUsuario;

                            cmd.Connection = cnx;
                            int c = cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GravarPendenciaOS(ref SqlTransaction trans, ref SqlConnection cnx, List<PendenciaOSSinc> listaPendenciaOS, long? idUsuario, String _IDENTIFICADOR_PK_ID_OS, Int64? _id_os)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    ConfigurarSqlCommand(trans, cnx, sqlCommand);

                    foreach (PendenciaOSSinc itemPendenciaOS in listaPendenciaOS.Where(e => e.IDENTIFICADOR_FK_ID_OS == _IDENTIFICADOR_PK_ID_OS))
                    {
                        ObterTipoEstoquePendeciaOs(itemPendenciaOS);
                        ObterTipoPendenciaOs(itemPendenciaOS);

                        sqlCommand.CommandText = ObterSqlPendenciaOS();
                        ConfigurarParametrosSqlPendenciaOs(idUsuario, _id_os, sqlCommand, itemPendenciaOS);
                        sqlCommand.Connection = cnx;
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GravarPendenciaOutrosOS(ref SqlTransaction trans, ref SqlConnection cnx, List<PendenciaOSSinc> listaPendenciaOS, long? idUsuario)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    ConfigurarSqlCommand(trans, cnx, sqlCommand);

                    foreach (PendenciaOSSinc itemPendenciaOS in listaPendenciaOS)
                    {
                        ObterTipoEstoquePendeciaOs(itemPendenciaOS);
                        ObterTipoPendenciaOs(itemPendenciaOS);

                        sqlCommand.CommandText = ObterSqlPendenciaOS();
                        ConfigurarParametrosSqlPendenciaOs(idUsuario, itemPendenciaOS.ID_OS, sqlCommand, itemPendenciaOS);
                        sqlCommand.Connection = cnx;
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GravarPendenciaOS(ref SqlTransaction trans, ref SqlConnection cnx, List<PendenciaOSSinc> listaPendenciaOS, long? idUsuario)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    ConfigurarSqlCommand(trans, cnx, sqlCommand);

                    foreach (PendenciaOSSinc itemPendenciaOS in listaPendenciaOS)
                    {
                        ObterTipoEstoquePendeciaOs(itemPendenciaOS);
                        ObterTipoPendenciaOs(itemPendenciaOS);

                        sqlCommand.CommandText = ObterSqlPendenciaOS();
                        ConfigurarParametrosSqlPendenciaOs(idUsuario, null, sqlCommand, itemPendenciaOS);
                        sqlCommand.Connection = cnx;
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void ConfigurarSqlCommand(SqlTransaction transacao, SqlConnection conexao, SqlCommand sqlCommand)
        {
            sqlCommand.Connection = conexao;
            sqlCommand.Transaction = transacao;
            sqlCommand.CommandType = CommandType.Text;
        }

        private static void ConfigurarParametrosSqlPendenciaOs(long? idUsuario, long? _id_os, SqlCommand cmd, PendenciaOSSinc itemPendenciaOS)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@ID_OS", SqlDbType.BigInt).Value = (_id_os is null || _id_os == 0) ? itemPendenciaOS.ID_OS : _id_os;
            cmd.Parameters.Add("@DT_ABERTURA", SqlDbType.DateTime).Value = itemPendenciaOS.DT_ABERTURA;
            cmd.Parameters.Add("@DS_DESCRICAO", SqlDbType.VarChar).Value = itemPendenciaOS.DS_DESCRICAO;
            cmd.Parameters.Add("@CD_PECA", SqlDbType.VarChar).Value = itemPendenciaOS.CD_PECA ?? (object)DBNull.Value;
            cmd.Parameters.Add("@QT_PECA", SqlDbType.Decimal).Value = itemPendenciaOS.QT_PECA;
            cmd.Parameters.Add("@CD_TECNICO", SqlDbType.VarChar).Value = itemPendenciaOS.CD_TECNICO;
            cmd.Parameters.Add("@ST_STATUS_PENDENCIA", SqlDbType.Char).Value = itemPendenciaOS.ST_STATUS_PENDENCIA;
            cmd.Parameters.Add("@CD_TP_ESTOQUE_CLI_TEC", SqlDbType.Char).Value = itemPendenciaOS.CD_TP_ESTOQUE_CLI_TEC;
            cmd.Parameters.Add("@ST_TP_PENDENCIA", SqlDbType.Char).Value = itemPendenciaOS.ST_TP_PENDENCIA;
            cmd.Parameters.Add("@DataAtualizacaoVisita", SqlDbType.DateTime).Value = itemPendenciaOS.dtmDataHoraAtualizacao == DateTime.MinValue ? DateTime.Now : itemPendenciaOS.dtmDataHoraAtualizacao;
            cmd.Parameters.Add("@nidUsuarioAtualizacao", SqlDbType.BigInt).Value = idUsuario;
            cmd.Parameters.Add("@TOKEN", SqlDbType.BigInt).Value = itemPendenciaOS.TOKEN;
        }

        private void ObterTipoPendenciaOs(PendenciaOSSinc itemPendenciaOS)
        {
            if (itemPendenciaOS.ST_TP_PENDENCIA != 'P' && itemPendenciaOS.ST_TP_PENDENCIA != 'O')
                itemPendenciaOS.ST_TP_PENDENCIA = 'P';
        }

        private void ObterTipoEstoquePendeciaOs(PendenciaOSSinc itemPendenciaOS)
        {
            if (itemPendenciaOS.CD_TP_ESTOQUE_CLI_TEC != 'C' && itemPendenciaOS.CD_TP_ESTOQUE_CLI_TEC != 'T')
                itemPendenciaOS.CD_TP_ESTOQUE_CLI_TEC = 'T';
        }

        private string ObterSqlPendenciaOS()
        {
            return @"UPDATE tbPendenciaOS WITH(UPDLOCK, SERIALIZABLE)
                                               SET ID_OS = @ID_OS, 
                                                   DT_ABERTURA = @DT_ABERTURA, 
                                                   DS_DESCRICAO = @DS_DESCRICAO, 
                                                   CD_PECA = @CD_PECA, 
                                                   QT_PECA = @QT_PECA, 
                                                   CD_TECNICO = @CD_TECNICO, 
                                                   ST_STATUS_PENDENCIA = @ST_STATUS_PENDENCIA, 
                                                   CD_TP_ESTOQUE_CLI_TEC = @CD_TP_ESTOQUE_CLI_TEC, 
                                                   ST_TP_PENDENCIA = @ST_TP_PENDENCIA, 
                                                   nidUsuarioAtualizacao = @nidUsuarioAtualizacao, 
                                                   dtmDataHoraAtualizacao = @DataAtualizacaoVisita
                                             WHERE TOKEN = @TOKEN
                                            IF @@ROWCOUNT = 0                   
                                            BEGIN
                                               INSERT INTO tbPendenciaOS ( ID_OS, DT_ABERTURA, DS_DESCRICAO, CD_PECA, QT_PECA, CD_TECNICO, ST_STATUS_PENDENCIA, 
                                                                           CD_TP_ESTOQUE_CLI_TEC, ST_TP_PENDENCIA, nidUsuarioAtualizacao, dtmDataHoraAtualizacao, TOKEN )
                                                                  VALUES ( @ID_OS, @DT_ABERTURA, @DS_DESCRICAO, @CD_PECA, @QT_PECA, @CD_TECNICO, @ST_STATUS_PENDENCIA, 
                                                                           @CD_TP_ESTOQUE_CLI_TEC, @ST_TP_PENDENCIA, @nidUsuarioAtualizacao, @DataAtualizacaoVisita, @TOKEN )
                                            END";
        }

        private void grava_RR(ref SqlTransaction trans, ref SqlConnection cnx, List<RelatorioReclamacaoSincEntity> listaRR, long? idUsuario,
                                                                                String _IDENTIFICADOR_PK_ID_OS, Int64? _id_os, List<RRComentSincEntity> listaRRComent)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnx;
                    cmd.Transaction = trans;
                    cmd.CommandType = CommandType.Text;

                    foreach (RelatorioReclamacaoSincEntity itemRR in listaRR.Where(e => e.IDENTIFICADOR_FK_ID_OS == _IDENTIFICADOR_PK_ID_OS))
                    {
                        string _IDENTIFICADOR_PK_ID_RR = itemRR.IDENTIFICADOR_PK_ID_RR;
                        
                        cmd.CommandText = @"UPDATE tbRRRelatorioReclamacao WITH(UPDLOCK, SERIALIZABLE) 
                                               SET ST_STATUS_RR = @ST_STATUS_RR, 
                                                   CD_ATIVO_FIXO = @CD_ATIVO_FIXO, 
                                                   CD_PECA = @CD_PECA, 
                                                   CD_TIPO_ATENDIMENTO = @CD_TIPO_ATENDIMENTO, 
                                                   CD_TIPO_RECLAMACAO = @CD_TIPO_RECLAMACAO,
                                                   DS_MOTIVO = @DS_MOTIVO,
                                                   DS_DESCRICAO = @DS_DESCRICAO, 
                                                   VL_TEMPO_ATENDIMENTO = @VL_TEMPO_ATENDIMENTO 
                                             WHERE TOKEN = @TOKEN
                                            IF @@ROWCOUNT = 0
                                            BEGIN
                                               INSERT INTO tbRRRelatorioReclamacao 
                                                      ( ST_STATUS_RR, CD_TECNICO, CD_CLIENTE, CD_ATIVO_FIXO, CD_PECA, CD_TIPO_ATENDIMENTO,
                                                        CD_TIPO_RECLAMACAO, DS_MOTIVO, DS_DESCRICAO, VL_TEMPO_ATENDIMENTO, 
                                                        DS_ARQUIVO_FOTO, DS_TIPO_FOTO, VL_MAO_OBRA, VL_CUSTO_PECA, CD_GRUPO_RESPONS, ID_OS, 
                                                        nidUsuarioAtualizacao, dtmDataHoraAtualizacao, DT_CRIACAO, TOKEN ) 
                                               VALUES ( @ST_STATUS_RR, @CD_TECNICO, @CD_CLIENTE, @CD_ATIVO_FIXO, @CD_PECA, @CD_TIPO_ATENDIMENTO, 
                                                        @CD_TIPO_RECLAMACAO, @DS_MOTIVO, @DS_DESCRICAO, @VL_TEMPO_ATENDIMENTO, 
                                                        @DS_ARQUIVO_FOTO, @DS_TIPO_FOTO, (( @VL_TEMPO_ATENDIMENTO /60) * (select top 1 t.vl_custo_hora from tb_tecnico t where cd_tecnico = @CD_TECNICO)),
                                                        (select top 1 vl_peca 
                                                           from tb_Peca 
                                                          where cd_peca = @CD_PECA), 
                                                        @CD_GRUPO_RESPONS, @ID_OS, 
                                                        @nidUsuarioAtualizacao, @dtmDataHoraAtualizacao, @DT_CRIACAO, @TOKEN );   SELECT @@IDENTITY; 
                                            END";

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ST_STATUS_RR", SqlDbType.Int).Value = itemRR.ST_STATUS_RR;
                        cmd.Parameters.Add("@CD_ATIVO_FIXO", SqlDbType.VarChar).Value = itemRR.CD_ATIVO_FIXO;
                        cmd.Parameters.Add("@CD_PECA", SqlDbType.VarChar).Value = String.IsNullOrEmpty(itemRR.CD_PECA) ? (object)DBNull.Value : itemRR.CD_PECA;
                        cmd.Parameters.Add("@CD_TIPO_ATENDIMENTO", SqlDbType.BigInt).Value = itemRR.CD_TIPO_ATENDIMENTO;
                        cmd.Parameters.Add("@CD_TIPO_RECLAMACAO", SqlDbType.BigInt).Value = itemRR.CD_TIPO_RECLAMACAO;
                        cmd.Parameters.Add("@DS_MOTIVO", SqlDbType.VarChar).Value = itemRR.DS_MOTIVO;
                        cmd.Parameters.Add("@DS_DESCRICAO", SqlDbType.VarChar).Value = itemRR.DS_DESCRICAO ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@VL_TEMPO_ATENDIMENTO", SqlDbType.BigInt).Value = itemRR.VL_TEMPO_ATENDIMENTO;
                        cmd.Parameters.Add("@CD_TECNICO", SqlDbType.VarChar).Value = itemRR.CD_TECNICO;
                        cmd.Parameters.Add("@CD_CLIENTE", SqlDbType.VarChar).Value = itemRR.CD_CLIENTE;
                        cmd.Parameters.Add("@DS_ARQUIVO_FOTO", SqlDbType.Text).Value = (itemRR.DS_ARQUIVO_FOTO ?? (object)DBNull.Value);
                        cmd.Parameters.Add("@VL_MAO_OBRA", SqlDbType.Decimal).Value = VALOR_ZERADO;// calcular e gravar
                        cmd.Parameters.Add("@VL_CUSTO_PECA", SqlDbType.Decimal).Value = VALOR_ZERADO;// calcular e gravar
                        cmd.Parameters.Add("@CD_GRUPO_RESPONS", SqlDbType.Char).Value = CODIGO_GRUPO_RESPONSAVEL;
                        cmd.Parameters.Add("@DS_TIPO_FOTO", SqlDbType.Char).Value = (itemRR.DS_TIPO_FOTO ?? (object)DBNull.Value);
                        cmd.Parameters.Add("@ID_OS", SqlDbType.BigInt).Value = _id_os;
                        cmd.Parameters.Add("@dtmDataHoraAtualizacao", SqlDbType.DateTime).Value = itemRR.dtmDataHoraAtualizacao == DateTime.MinValue ? DateTime.Now : itemRR.dtmDataHoraAtualizacao;
                        cmd.Parameters.Add("@nidUsuarioAtualizacao", SqlDbType.BigInt).Value = idUsuario;
                        cmd.Parameters.Add("@DT_CRIACAO", SqlDbType.DateTime).Value = itemRR.dtmDataHoraAtualizacao == DateTime.MinValue ? DateTime.Now : itemRR.dtmDataHoraAtualizacao;
                        cmd.Parameters.Add("@TOKEN", SqlDbType.BigInt).Value = itemRR.TOKEN;

                        cmd.Connection = cnx;
                        int id_rr = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void grava_PecaOS(ref SqlTransaction trans, ref SqlConnection cnx, List<PecaOSSinc> listaPecaOS, long idUsuario, Int64? _id_os, OSPadraoEntity osPadrao)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    ConfigurarSqlCommand(trans, cnx, sqlCommand);

                    StringBuilder observacoesOs = new StringBuilder();

                    var pecasOsEstoqueMovimentado = ObterPecasOSComMovimentacaoEstoque(ref trans, ref cnx, osPadrao.ID_OS);

                    foreach (PecaOSSinc itemPecaOS in listaPecaOS.Where(e => e.IDENTIFICADOR_FK_ID_OS == osPadrao.IDENTIFICADOR_PK_ID_OS))
                    {
                        itemPecaOS.ID_OS = _id_os;
                        
                        PecaData pecaData = new PecaData();
                        PecaEntity pecaEntity = new PecaEntity();

                        pecaEntity = pecaData.ObterPecas(itemPecaOS.CD_PECA.ToUpper()).FirstOrDefault();

                        if (pecaEntity != null)
                        {
                            itemPecaOS.VL_VALOR_PECA = pecaEntity.VL_PECA;
                        }
                        
                        if (itemPecaOS.EXCLUIR_PECA && osPadrao.action == ACTION_ALTERAR)
                        {
                            if (itemPecaOS.ID_PECA_OS <= 0 || itemPecaOS.ID_PECA_OS == null)
                                throw new Exception($"Foi enviado instrução de exclusão de peça na O.S. {_id_os} sem código da peça {itemPecaOS.CD_PECA}.");

                            ExcluirPecaOs(sqlCommand, itemPecaOS);
                            continue;
                        }

                        if (PermitirMovimentarEstoque(osPadrao) && PecaNaoMovimentouEstoqueOs(pecasOsEstoqueMovimentado, itemPecaOS))
                        {
                            if (!RealizarAtualizacaoEstoque(ref trans, ref cnx, itemPecaOS, osPadrao, idUsuario))
                            {
                                if (itemPecaOS.ID_PECA_OS > 0)
                                    ExcluirPecaOs(sqlCommand, itemPecaOS);

                                CriarNotificacao(ref trans, ref cnx, $@"EXCLUSÃO DE PEÇA NA O.S. {_id_os}", $@"A O.S. {_id_os} teve a peça {itemPecaOS.CD_PECA} excluída, pois não havia estoque suficiente no momento da sincronização. Entre em contato com o Administrador.", idUsuario);

                                observacoesOs.AppendLine($"A peça {itemPecaOS.CD_PECA} foi excluída desta O.S. pois não havia estoque suficiente no momento da sincronização.");
                                continue;
                            }
                        }

                        AtualizarIncluirPecaOs(idUsuario, _id_os, sqlCommand, itemPecaOS);
                        ExecutarConversaoNR12(_id_os, sqlCommand, itemPecaOS);
                    }

                    if (observacoesOs.Length > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(osPadrao.DS_OBSERVACAO))
                            observacoesOs.AppendLine(osPadrao.DS_OBSERVACAO);

                        AtualizarObservacaoOs(sqlCommand, _id_os, observacoesOs.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool PecaNaoMovimentouEstoqueOs(IList<string> pecasOsEstoqueMovimentado, PecaOSSinc itemPecaOS)
        {
            if (pecasOsEstoqueMovimentado.Count > 0)
                return pecasOsEstoqueMovimentado.Any(x => x == itemPecaOS.CD_PECA) ? false : true;
            else
                return true;
        }

        private static void CriarNotificacao(ref SqlTransaction transacao, ref SqlConnection conexao, string titulo, string mensagem, long idUsuario)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = conexao;
                    sqlCommand.Transaction = transacao;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "prcNotificacaoInsert";

                    sqlCommand.Parameters.Add(new SqlParameter("@p_Titulo", SqlDbType.VarChar) { Value = titulo });
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Mensagem", SqlDbType.VarChar) { Value = mensagem });
                    sqlCommand.Parameters.Add(new SqlParameter("@p_Lida", SqlDbType.Bit) { Value = false });
                    sqlCommand.Parameters.Add(new SqlParameter("@p_IdUsuario", SqlDbType.BigInt) { Value = idUsuario });
                    sqlCommand.Parameters.Add(new SqlParameter("@p_IdNotificacao", SqlDbType.BigInt, 18) { Direction = ParameterDirection.Output });
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void AtualizarIncluirPecaOs(long idUsuario, long? _id_os, SqlCommand cmd, PecaOSSinc itemPecaOS)
        {
            cmd.CommandText = ObterSqlPecaOs();

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@ID_OS", SqlDbType.BigInt).Value = _id_os;
            cmd.Parameters.Add("@CD_PECA", SqlDbType.VarChar).Value = itemPecaOS.CD_PECA;
            cmd.Parameters.Add("@QT_PECA", SqlDbType.Decimal).Value = itemPecaOS.QT_PECA;
            cmd.Parameters.Add("@CD_TP_ESTOQUE_CLI_TEC", SqlDbType.Char).Value = itemPecaOS.CD_TP_ESTOQUE_CLI_TEC;
            cmd.Parameters.Add("@DataAtualizacaoVisita", SqlDbType.DateTime).Value = itemPecaOS.dtmDataHoraAtualizacao == DateTime.MinValue ? DateTime.Now : itemPecaOS.dtmDataHoraAtualizacao;
            cmd.Parameters.Add("@nidUsuarioAtualizacao", SqlDbType.BigInt).Value = idUsuario;
            cmd.Parameters.Add("@DS_OBSERVACAO", SqlDbType.VarChar).Value = itemPecaOS.DS_OBSERVACAO ?? (object)DBNull.Value;
            cmd.Parameters.Add("@TOKEN", SqlDbType.BigInt).Value = itemPecaOS.TOKEN;
            cmd.Parameters.Add("@VL_VALOR_PECA", SqlDbType.Decimal).Value = itemPecaOS.VL_VALOR_PECA;
            itemPecaOS.ID_OS = _id_os;

            cmd.ExecuteNonQuery();
        }

        private static string ObterSqlPecaOs()
        {
            return @"UPDATE tbPecaOS WITH(UPDLOCK, SERIALIZABLE)
                                               SET ID_OS = @ID_OS, 
                                                   CD_PECA = @CD_PECA, 
                                                   QT_PECA = @QT_PECA, 
                                                   CD_TP_ESTOQUE_CLI_TEC = @CD_TP_ESTOQUE_CLI_TEC, 
                                                   nidUsuarioAtualizacao = @nidUsuarioAtualizacao, 
                                                   dtmDataHoraAtualizacao = @DataAtualizacaoVisita, 
                                                   DS_OBSERVACAO = @DS_OBSERVACAO,
                                                   VL_VALOR_PECA = @VL_VALOR_PECA
                                             WHERE TOKEN = @TOKEN  
                                            IF @@ROWCOUNT = 0 
                                            BEGIN 
                                               INSERT INTO tbPecaOS 
                                                      ( ID_OS, CD_PECA, QT_PECA, CD_TP_ESTOQUE_CLI_TEC, 
                                                        nidUsuarioAtualizacao, dtmDataHoraAtualizacao, DS_OBSERVACAO, TOKEN, VL_VALOR_PECA ) 
                                               VALUES ( @ID_OS, @CD_PECA, @QT_PECA, @CD_TP_ESTOQUE_CLI_TEC,
                                                        @nidUsuarioAtualizacao, @DataAtualizacaoVisita, @DS_OBSERVACAO, @TOKEN, @VL_VALOR_PECA ) 
                                            END";
        }

        private static void ExcluirPecaOs(SqlCommand cmd, PecaOSSinc itemPecaOS)
        {
            cmd.CommandText = @"DELETE FROM tbPecaOS WITH(UPDLOCK, SERIALIZABLE)
                                                 WHERE ID_PECA_OS = @ID_PECA_OS
                                                   AND ID_OS = @ID_OS";

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@ID_PECA_OS", SqlDbType.BigInt).Value = itemPecaOS.ID_PECA_OS;
            cmd.Parameters.Add("@ID_OS", SqlDbType.BigInt).Value = itemPecaOS.ID_OS;
            cmd.ExecuteNonQuery();
        }

        private static void AtualizarObservacaoOs(SqlCommand cmd, long? idOs, string observacao)
        {
            cmd.CommandText = @"UPDATE tbOSPadrao 
                                   SET DS_OBSERVACAO = @DS_OBSERVACAO
                                 WHERE ID_OS = @ID_OS";

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@ID_OS", SqlDbType.BigInt).Value = idOs;
            cmd.Parameters.Add("@DS_OBSERVACAO", SqlDbType.VarChar).Value = observacao;
            cmd.ExecuteNonQuery();
        }

        private bool PermitirMovimentarEstoque(OSPadraoEntity osPadrao)
            => (osPadrao.TpStatusOS.ST_STATUS_OS == OS_STATUS_CANCELADO && osPadrao.action == ACTION_ALTERAR) 
               || (osPadrao.TpStatusOS.ST_STATUS_OS == OS_STATUS_FINALIZADA);

        private static void ExecutarConversaoNR12(long? _id_os, SqlCommand cmd, PecaOSSinc itemPecaOS)
        {
            //TODO: Edgar - Refatorar
            //--- Avalia necessidade de conversão NR12
            cmd.CommandText = " select top 1 m.CD_MOD_NR12 " +
                " from tb_Ativo_fixo a with (nolock) " +
                " INNER JOIN tbOs o with (nolock) ON o.id_OS = @ID_OS and o.CD_ATIVO_FIXO = a.CD_ATIVO_FIXO " +
                " INNER JOIN tb_MODELO m  with (nolock) ON m.CD_MODELO = a.CD_MODELO " +
                " INNER JOIN tbPecaOS po  with (nolock) ON po.ID_OS = o.ID_OS " +
                " INNER JOIN tb_PECA p  with (nolock) ON p.CD_PECA = po.CD_PECA " +
                " where m.CD_MOD_NR12 is not null " +
                " AND po.CD_PECA = @CD_PECA " +
                " AND (p.DS_PECA like '%NR12%' OR p.ds_peca like '%NR-12%' OR p.ds_peca like '%NR - 12%') " +
                " AND m.CD_MOD_NR12 is not null ";

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@CD_PECA", System.Data.SqlDbType.VarChar).Value = itemPecaOS.CD_PECA;
            cmd.Parameters.Add("@ID_OS", System.Data.SqlDbType.Int).Value = _id_os;
            string cdNR12 = Convert.ToString(cmd.ExecuteScalar());

            if (!(cdNR12 is null) && cdNR12 != "")
            {
                cmd.CommandText = " UPDATE tb_ativo_fixo set cd_MODELO = @CD_MODELO " +
                    " WHERE CD_ATIVO_FIXO = (select TOP 1 a.cd_ATIVO_FIXO FROM tb_ativo_fixo a " +
                    " INNER JOIN tbos o ON o.CD_ATIVO_FIXO = a.CD_ATIVO_FIXO WHERE o.ID_OS= @ID_OS) ";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CD_MODELO", System.Data.SqlDbType.VarChar).Value = cdNR12;
                cmd.Parameters.Add("@ID_OS", System.Data.SqlDbType.Int).Value = _id_os;
                int cont = cmd.ExecuteNonQuery();
            }
        }

        private bool RealizarAtualizacaoEstoque(ref SqlTransaction transacao, ref SqlConnection conexao, PecaOSSinc pecaOS, OSPadraoEntity osPadrao, long idUsuario)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = conexao;
                    sqlCommand.Transaction = transacao;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "prcPecaOSMovimentaEstoque";
                    sqlCommand.Parameters.Add(new SqlParameter("@p_ID_OS", SqlDbType.BigInt) { Value = pecaOS.ID_OS });
                    sqlCommand.Parameters.Add(new SqlParameter("@p_CD_PECA", SqlDbType.VarChar) { Value = pecaOS.CD_PECA });
                    sqlCommand.Parameters.Add(new SqlParameter("@p_QT_PECA", SqlDbType.Decimal) { Value = pecaOS.QT_PECA });
                    sqlCommand.Parameters.Add(new SqlParameter("@p_CD_TP_ESTOQUE_CLI_TEC", SqlDbType.VarChar) { Value = pecaOS.CD_TP_ESTOQUE_CLI_TEC });
                    sqlCommand.Parameters.Add(new SqlParameter("@p_CD_TECNICO", SqlDbType.VarChar) { Value = osPadrao.Tecnico.CD_TECNICO });
                    sqlCommand.Parameters.Add(new SqlParameter("@p_CD_CLIENTE", SqlDbType.BigInt) { Value = osPadrao.Cliente.CD_CLIENTE });
                    sqlCommand.Parameters.Add(new SqlParameter("@p_nidUsuarioAtualizacao", SqlDbType.BigInt) { Value = idUsuario });

                    sqlCommand.Parameters.Add(new SqlParameter("@p_Tipo_Movimentacao", SqlDbType.TinyInt)
                    {
                        Value = osPadrao.TpStatusOS.ST_STATUS_OS == OS_STATUS_CANCELADO ? OPERACAO_ESTOQUE_ENTRADA : OPERACAO_ESTOQUE_SAIDA
                    });

                    var parametroMensagem = new SqlParameter("@p_Mensagem", SqlDbType.VarChar, 8000) { Direction = ParameterDirection.Output };
                    sqlCommand.Parameters.Add(parametroMensagem);

                    var parametroPermiteMovimentarEstoque = new SqlParameter("@p_Permite_Movimentar_Estoque", SqlDbType.Bit, 1) { Direction = ParameterDirection.Output };
                    sqlCommand.Parameters.Add(parametroPermiteMovimentarEstoque);
                    sqlCommand.ExecuteNonQuery();

                    var mensagemValidacaoMovimentaEstoque = parametroMensagem.Value.ToString();

                    if (!string.IsNullOrWhiteSpace(parametroMensagem.Value.ToString()))
                        throw new Exception($"{parametroMensagem.Value} OS: {pecaOS.ID_OS} Peça: {pecaOS.CD_PECA}");

                    return Convert.ToBoolean(parametroPermiteMovimentarEstoque.Value);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region email
        public class Parametro
        {
            /// <summary>
            /// Busca o conteúdo de cdsParametro da tabela tbParametro
            /// </summary>
            /// <param name="ccdParametro">Código</param>
            /// <returns>Descrição</returns>
            public static string ObterValorParametro(string ccdParametro)
            {
                //if (ccdParametro.ToUpper() == Constantes.URLSite.ToUpper())
                //{
                //    return ConfigurationManager.AppSettings[Constantes.URLSite];
                //}
                //else if (ccdParametro.ToUpper() == Constantes.URLAPI.ToUpper())
                //{
                //    return ConfigurationManager.AppSettings[Constantes.URLAPI];
                //}

                string cvlParametro = string.Empty;
                ParametroEntity parametroEntity = new ParametroEntity();
                ParametroData parametroData = new ParametroData();

                parametroEntity.ccdParametro = ccdParametro;
                DataTableReader dataTableReader = parametroData.ObterLista(parametroEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        cvlParametro = dataTableReader["cvlParametro"].ToString();
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                return cvlParametro;
            }

        }
        public class MailSender
        {
            public bool Send(string mailTo, string mailSubject, string mailMessage, Attachment Attachments = null, string mailCopy = null)
            {
                MailMessage mail = new MailMessage();
                if (!string.IsNullOrEmpty(mailTo))
                {
                    string[] to = mailTo.Split(';');
                    foreach (string e in to)
                    {
                        if ((!string.IsNullOrEmpty(e)) && (e != "  ") && (e != " "))
                            mail.To.Add(e);
                    }
                }


                if (!string.IsNullOrEmpty(mailCopy))
                {
                    string[] cc = mailCopy.Split(';');
                    foreach (string e in cc)
                    {
                        if (!string.IsNullOrEmpty(e))
                            mail.To.Add(e);
                    }
                }

                //mail.From = new MailAddress(mailFrom);
                mail.Subject = mailSubject;
                mail.Body = mailMessage;
                mail.IsBodyHtml = true;

                if (Attachments != null)
                    mail.Attachments.Add(Attachments);

                SmtpClient smtp = new SmtpClient();
                smtp.Host = Parametro.ObterValorParametro(Constantes.MailHost);
                smtp.EnableSsl = Convert.ToBoolean(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MailEnableSSL));

                mail.From = new MailAddress(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MailCredentialsUserName));
                //smtp.Host = "smtp.office365.com";

                smtp.Port = Convert.ToInt16(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MailPort));//587;
                smtp.UseDefaultCredentials = Convert.ToBoolean(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MailUseDefaultCredentials));
                smtp.Credentials = new System.Net.NetworkCredential(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MailCredentialsUserName), ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MailCredentialsPassword)); // Enter seders User name and password
                                                                                                                                                                                                                                                                               //smtp.EnableSsl = true;

                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                //#if (DEBUG == true)
                //            smtp.Host = "smtp.gmail.com";
                //            smtp.Port = 587;
                //            smtp.Credentials = new System.Net.NetworkCredential("a84npzz@gmail.com", "3m@gsw123");
                //            //smtp.Timeout = 99999999;

                //            //smtp.Host = "in-v3.mailjet.com";
                //            //smtp.Port = 587;
                //            //smtp.Credentials = new System.Net.NetworkCredential("0d8b0ab5e42077c94ce7a3eca9896e7c", "d7ef8166521152e2677be6d05e37aa1f");
                //            //smtp.Timeout = 99999999;
                //#endif

                smtp.Send(mail);

                //await smtp.SendAsync(mail,null);

                return true;
            }
            public class Constantes
            {
                // Todas as constantes devem ser declaradas aqui
                public const string ADDomain = "ADDomain";
                public const string ADPassword = "ADPassword";
                public const string ADService = "ADService";
                public const string ADUser = "ADUser";
                public const string diasTrocaSenhaExterno = "diasTrocaSenhaExterno";
                public const string MailCredentialsPassword = "MailCredentialsPassword";
                public const string MailCredentialsUserName = "MailCredentialsUserName";
                public const string MailEnableSSL = "MailEnableSSL";
                public const string MailHost = "MailHost";
                public const string MailPort = "MailPort";
                public const string MailUseDefaultCredentials = "MailUseDefaultCredentials";

                public const string perfilExternoPadrao = "perfilExternoPadrao";
                public const string perfilLiderPadrao = "perfilLiderPadrao";
                public const string perfisMeusClientes = "perfisMeusClientes";
                public const string perfisTecnicos = "perfisTecnicos";
                public const string perfilEquipeVendasPadrao = "perfilEquipeVendasPadrao";
                public const string perfilGerenteRegionalVendasPadrao = "perfilGerenteRegionalVendasPadrao";
                public const string perfilGerenteNacionalVendasPadrao = "perfilGerenteNacionalVendasPadrao";

                public const string URLSite = "URLSite";
                public const string URLAPI = "URLAPI";
                public const string MensagemGravacaoSucesso = "Registro gravado com sucesso!";
                public const string MensagemExclusaoSucesso = "Registro excluído com sucesso!";
                public const string MensagemInativacaoSucesso = "Registro inativado com sucesso!";

                public const string vigenciaINICIAL = "vigenciaINICIAL";
                public const string vigenciaFINAL = "vigenciaFINAL";

                public const string MensagemEnvioAvaliacao = "PesquisaMailNotificacao";
                //public const string MensagemEnvioAvaliacao = "";
                public const string MargemDashVendas = "MargemDashVendas";
                public const string CodigoPecaAvulsa = "CodigoPecaAvulsa";

                public const string CodigoSegmentoRealocarExcluir = "SegmentoRealocarExcluir";
                public const string CodigoSegmentoDistribuidor = "SegmentoDistribuidor";

                public const string CaminhoUpload = "CaminhoArquivosUpload";
                public const string CaminhoUploadNF = "CaminhoArquivosUpload_NF";

                //public const string PastaNFLoteUpload = @"/NotaFiscal/Lote/";
                //public const string PastaFotosPecasSincronismo = @"/Sinc/";
                //public const string PastaWorkflowUploadEnvio = @"/Workflow/Envio/";
                //public const string PastaWorkflowUploadDevolucao = @"/Workflow/Devolucao/";
                //public const string PastaAtivoClienteNF = @"/NotaFiscal/AtivoCliente/";

                public const string PastaNFLoteUpload = @"\NotaFiscal\Lote\";
                public const string PastaFotosPecasSincronismo = @"\Sinc\";
                public const string PastaWorkflowUploadEnvio = @"\Workflow\Envio\";
                public const string PastaWorkflowUploadDevolucao = @"\Workflow\Devolucao\";
                public const string PastaAtivoClienteNF = @"\NotaFiscal\AtivoCliente\";

                public const string ValorEnvioMensalPecas = "ValorEnvioMensalPecas";
                public const string WorkflowCategoriaFechador = "WorkflowCategoriaFechador";
                public const string WorkflowCategoriaIdentificador = "WorkflowCategoriaIdentificador";
                public const string WorkflowCategoriaAcessorios = "WorkflowCategoriaAcessorios";
                public const string WorkflowSolicitacaoTroca = "WorkflowSolicitacaoTroca";

                public const string VideoEAD1 = "VideoEAD1";
                public const string DiasPendenciaAprovacao = "DiasPendenciaAprovacao";
            }


            /// <summary>
            /// Obter o conteúdo do arquivo HTML (modelo) para construção do corpo do e-mail
            /// </summary>
            /// <param name="ArquivoHTML">Arquivo HTML (modelo)</param>
            /// <returns>Conteúdo (HTML) do arquivo</returns>
            public StringBuilder GetConteudoHTML(string ArquivoHTML)
            {
                StringBuilder HTML = null;
                StreamReader sr = null;

                //Obtém o caminho da aplicação WEB
                string caminho = AppDomain.CurrentDomain.BaseDirectory;
                DirectoryInfo diretorio = new DirectoryInfo(caminho);

                //Percorrer os diretórios da aplicação para obter o arquivo identificado em ArquivoHTML
                //e extrair o seu conteúdo para um stream

                if (null != diretorio)
                {
                    DirectoryInfo[] subDiretorios = diretorio.GetDirectories();

                    foreach (DirectoryInfo dir in subDiretorios)
                    {
                        if (dir.Name.ToLower().Equals("htmlmail"))
                        {
                            FileInfo[] files = dir.GetFiles();

                            foreach (FileInfo file in files)
                            {
                                if (file.Name.ToLower().Equals(ArquivoHTML.ToLower()))
                                {
                                    string linha = null;
                                    HTML = new StringBuilder();

                                    sr = new StreamReader(file.OpenRead());

                                    while (null != (linha = sr.ReadLine()))
                                    {
                                        HTML.Append(linha);
                                    }
                                }
                            }
                        }
                    }
                }

                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
                return HTML;
            }

        }

        public class ControlesUtility
        {

            public class Constantes
            {
                // Todas as constantes devem ser declaradas aqui
                public const string ADDomain = "ADDomain";
                public const string ADPassword = "ADPassword";
                public const string ADService = "ADService";
                public const string ADUser = "ADUser";
                public const string diasTrocaSenhaExterno = "diasTrocaSenhaExterno";
                public const string MailCredentialsPassword = "MailCredentialsPassword";
                public const string MailCredentialsUserName = "MailCredentialsUserName";
                public const string MailEnableSSL = "MailEnableSSL";
                public const string MailHost = "MailHost";
                public const string MailPort = "MailPort";
                public const string MailUseDefaultCredentials = "MailUseDefaultCredentials";

                public const string perfilExternoPadrao = "perfilExternoPadrao";
                public const string perfilLiderPadrao = "perfilLiderPadrao";
                public const string perfisMeusClientes = "perfisMeusClientes";
                public const string perfisTecnicos = "perfisTecnicos";
                public const string perfilEquipeVendasPadrao = "perfilEquipeVendasPadrao";
                public const string perfilGerenteRegionalVendasPadrao = "perfilGerenteRegionalVendasPadrao";
                public const string perfilGerenteNacionalVendasPadrao = "perfilGerenteNacionalVendasPadrao";

                public const string URLSite = "URLSite";
                public const string URLAPI = "URLAPI";
                public const string MensagemGravacaoSucesso = "Registro gravado com sucesso!";
                public const string MensagemExclusaoSucesso = "Registro excluído com sucesso!";
                public const string MensagemInativacaoSucesso = "Registro inativado com sucesso!";

                public const string vigenciaINICIAL = "vigenciaINICIAL";
                public const string vigenciaFINAL = "vigenciaFINAL";

                public const string MensagemEnvioAvaliacao = "PesquisaMailNotificacao";
                //public const string MensagemEnvioAvaliacao = "";
                public const string MargemDashVendas = "MargemDashVendas";
                public const string CodigoPecaAvulsa = "CodigoPecaAvulsa";

                public const string CodigoSegmentoRealocarExcluir = "SegmentoRealocarExcluir";
                public const string CodigoSegmentoDistribuidor = "SegmentoDistribuidor";

                public const string CaminhoUpload = "CaminhoArquivosUpload";
                public const string CaminhoUploadNF = "CaminhoArquivosUpload_NF";

                //public const string PastaNFLoteUpload = @"/NotaFiscal/Lote/";
                //public const string PastaFotosPecasSincronismo = @"/Sinc/";
                //public const string PastaWorkflowUploadEnvio = @"/Workflow/Envio/";
                //public const string PastaWorkflowUploadDevolucao = @"/Workflow/Devolucao/";
                //public const string PastaAtivoClienteNF = @"/NotaFiscal/AtivoCliente/";

                public const string PastaNFLoteUpload = @"\NotaFiscal\Lote\";
                public const string PastaFotosPecasSincronismo = @"\Sinc\";
                public const string PastaWorkflowUploadEnvio = @"\Workflow\Envio\";
                public const string PastaWorkflowUploadDevolucao = @"\Workflow\Devolucao\";
                public const string PastaAtivoClienteNF = @"\NotaFiscal\AtivoCliente\";

                public const string ValorEnvioMensalPecas = "ValorEnvioMensalPecas";
                public const string WorkflowCategoriaFechador = "WorkflowCategoriaFechador";
                public const string WorkflowCategoriaIdentificador = "WorkflowCategoriaIdentificador";
                public const string WorkflowCategoriaAcessorios = "WorkflowCategoriaAcessorios";
                public const string WorkflowSolicitacaoTroca = "WorkflowSolicitacaoTroca";

                public const string VideoEAD1 = "VideoEAD1";
                public const string DiasPendenciaAprovacao = "DiasPendenciaAprovacao";
            }

            
            public class Parametro
            {
                /// <summary>
                /// Busca o conteúdo de cdsParametro da tabela tbParametro
                /// </summary>
                /// <param name="ccdParametro">Código</param>
                /// <returns>Descrição</returns>
                public static string ObterValorParametro(string ccdParametro)
                {
                    
                    string cvlParametro = string.Empty;
                    ParametroEntity parametroEntity = new ParametroEntity();
                    ParametroData parametroData = new ParametroData();

                    parametroEntity.ccdParametro = ccdParametro;
                    DataTableReader dataTableReader = parametroData.ObterLista(parametroEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            cvlParametro = dataTableReader["cvlParametro"].ToString();
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    return cvlParametro;
                }

            }

            
        }

        public void EnviarEmailOS(OSPadraoEntity listOSPadrao, long? Id_Os)
        {
            //Método Modelo envio de email
            try
            {
                //string emailsSolicitacaoPeca = ControlesUtility.Parametro.ObterValorParametro("emailsOSFinalizada");


                if (listOSPadrao.TpStatusOS.ST_STATUS_OS == 3)
                {
                    TecnicoEntity tecnicoEntity = new TecnicoEntity();
                    tecnicoEntity.CD_TECNICO = listOSPadrao.Tecnico.CD_TECNICO;
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

                    ClienteEntity _clienteEntity = new ClienteEntity();
                    _clienteEntity.CD_CLIENTE = listOSPadrao.Cliente.CD_CLIENTE;
                    dataTableReader = new ClienteData().ObterLista(_clienteEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            _clienteEntity.CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]);
                            _clienteEntity.TX_EMAIL = dataTableReader["TX_EMAIL"].ToString();
                            _clienteEntity.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString();
                            _clienteEntity.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
                            _clienteEntity.EmailsInfo = dataTableReader["EmailsInfo"].ToString();
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }


                    // Envia a requisição de troca de senha por e-mail

                    

                    MailSender mailSender = new MailSender();

                    string mailTo = "";

                    var pecasOS = new PecaOSData().ObterListaPecaOsEmail(listOSPadrao.ID_OS);

                    if (listOSPadrao.Email == null || listOSPadrao.Email == "")
                    {
                        //if (tecnicoEntity.usuario.cdsEmail != "")
                        //{
                        //    mailTo += tecnicoEntity.usuario.cdsEmail + ";";
                        //}
                        if (_clienteEntity.TX_EMAIL != "")
                        {
                            mailTo += _clienteEntity.TX_EMAIL + ";";
                        }
                    }
                    else if (listOSPadrao.Email != null && listOSPadrao.Email != "")
                    {
                        if (listOSPadrao.Email == tecnicoEntity.usuario.cdsEmail)
                            mailTo += _clienteEntity.TX_EMAIL + ";";
                        else
                            mailTo += listOSPadrao.Email + ";";
                    }



                    string mailSubject = "3M.Comodato - Finalização de OS";
                    string mailMessage = string.Empty;
                    System.Net.Mail.Attachment Attachments = null;
                    string mailCopy = null;

                    var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                    string Conteudo = string.Empty;
                    string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                    //URLSite += "/SolicitacaoPecas/Editar?idKey=" + ControlesUtility.Criptografia.Criptografar(tecnicoEntity.CD_TECNICO + "|" + pedidoEntity.ID_PEDIDO.ToString() + "|" + pedidoEntity.TP_TIPO_PEDIDO + "|Solicitação");
                    URLSite += "/OsPadrao/Pesquisa?ID_OS=" + Id_Os;
                    Conteudo += "<p>Uma OS acaba de ser <strong>Finalizada </strong>!</p>";
                    Conteudo += "<p>Segue dados da OS:</p>";
                    Conteudo += "Cliente: " + _clienteEntity.CD_CLIENTE + " - " + _clienteEntity.NM_CLIENTE + "<br/>";
                    Conteudo += "Cidade: " + _clienteEntity.EN_CIDADE + "<br/>";
                    Conteudo += "Técnico: " + tecnicoEntity.NM_TECNICO + " - " + tecnicoEntity.empresa.NM_Empresa + "<br/>";

                    if (Id_Os != null && Id_Os != 0)
                    {
                        Conteudo += "OS: <strong>" + Convert.ToInt64(Id_Os) + "</strong><br/>";
                    }else if (listOSPadrao.ID_OS != 0)
                    {
                        Conteudo += "OS: <strong>" + Convert.ToInt64(listOSPadrao.ID_OS) + "</strong><br/>";
                    }

                    Conteudo += "Data: " + Convert.ToDateTime(listOSPadrao.DT_DATA_OS).ToString("dd/MM/yyyy") + "<br/>";
                    Conteudo += "Hora Inicio: " + listOSPadrao.HR_INICIO.ToString() + "<br/>";
                    Conteudo += "Hora de Finalização: " + listOSPadrao.HR_FIM.ToString() + "<br/>";
                    Conteudo += "Ativo Fixo: " + listOSPadrao.AtivoFixo.CD_ATIVO_FIXO + "-" + listOSPadrao.AtivoFixo.modelo.DS_MODELO + "<br/>";
                    Conteudo += "Linha: " + listOSPadrao.NOME_LINHA + "<br/>";
                    Conteudo += "Observação: " + listOSPadrao.DS_OBSERVACAO + "<br/>";
                    if (listOSPadrao.DS_RESPONSAVEL != null)
                    {
                        Conteudo += "Acompanhante: " + listOSPadrao.DS_RESPONSAVEL.ToString() + "<br/>";
                    }
                    else
                    {
                        Conteudo += "Acompanhante: " + "" + "<br/>";
                    }

                    var manutencao = "";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 1)
                        manutencao = "Preventiva";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 2)
                        manutencao = "Corretiva";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 3)
                        manutencao = "Instalação";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 4)
                        manutencao = "Outros";
                    Conteudo += "Tipo de Manutenção: " + manutencao + "<br/>";
                    Conteudo += "Status da OS: Finalizada<br/><br/>";

                    if (pecasOS?.Count > 0)
                    {
                        Conteudo += "<p><strong>Peças Utilizadas: </strong></p>";
                        int contaPc = 0;
                        foreach(var peca in pecasOS)
                        {
                            contaPc++;
                            var estoque = peca.CD_TP_ESTOQUE_CLI_TEC == 'C' ? "Cliente" : "Intermediario";
                            Conteudo += $" {contaPc}. {peca.DS_PECA} || Quantidade: {peca.QT_PECA} || Estoque: {estoque}<br/>";
                        }
                    }


                    var button = mailSender.GetConteudoHTML("button.html");

                    button.Replace("TEXTO_URL", "Avalie o Atendimento");
                    button.Replace("URL", URLSite);

                    Conteudo += button;
                    //Conteudo += $"<a class='btn btn-danger m-1' href = '{URLSite}'>Avalie o Atendimento</a>";
                    MensagemEmail.Replace("#Conteudo", Conteudo);
                    mailMessage = MensagemEmail.ToString();

                    mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);

                    if (_clienteEntity.EmailsInfo != null || _clienteEntity.EmailsInfo != "")
                    {
                        EnviarEmailOSInfo(listOSPadrao, Id_Os);
                    }
                }



            }
            catch (Exception ex)
            {
                

            }
        }


        public void EnviarEmailOSInfo(OSPadraoEntity listOSPadrao, long? Id_Os)
        {
            //Método Modelo envio de email
            try
            {
                //string emailsSolicitacaoPeca = ControlesUtility.Parametro.ObterValorParametro("emailsOSFinalizada");


                if (listOSPadrao.TpStatusOS.ST_STATUS_OS == 3)
                {
                    TecnicoEntity tecnicoEntity = new TecnicoEntity();
                    tecnicoEntity.CD_TECNICO = listOSPadrao.Tecnico.CD_TECNICO;
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

                    ClienteEntity _clienteEntity = new ClienteEntity();
                    _clienteEntity.CD_CLIENTE = listOSPadrao.Cliente.CD_CLIENTE;
                    dataTableReader = new ClienteData().ObterLista(_clienteEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            _clienteEntity.CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]);
                            _clienteEntity.TX_EMAIL = dataTableReader["TX_EMAIL"].ToString();
                            _clienteEntity.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString();
                            _clienteEntity.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
                            _clienteEntity.EmailsInfo = dataTableReader["EmailsInfo"].ToString();
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }


                    // Envia a requisição de troca de senha por e-mail
                    
                    MailSender mailSender = new MailSender();

                    string mailTo = "";

                    var pecasOS = new PecaOSData().ObterListaPecaOsEmail(listOSPadrao.ID_OS);

                    mailTo = _clienteEntity.EmailsInfo;

                    string mailSubject = "3M.Comodato - Finalização de OS";
                    string mailMessage = string.Empty;
                    System.Net.Mail.Attachment Attachments = null;
                    string mailCopy = null;

                    var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                    string Conteudo = string.Empty;
                    string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                    //URLSite += "/SolicitacaoPecas/Editar?idKey=" + ControlesUtility.Criptografia.Criptografar(tecnicoEntity.CD_TECNICO + "|" + pedidoEntity.ID_PEDIDO.ToString() + "|" + pedidoEntity.TP_TIPO_PEDIDO + "|Solicitação");
                    
                    Conteudo += "<p>Uma OS acaba de ser <strong>Finalizada </strong>!</p>";
                    Conteudo += "<p>Segue dados da OS:</p>";
                    Conteudo += "Cliente: " + _clienteEntity.CD_CLIENTE + " - " + _clienteEntity.NM_CLIENTE + "<br/>";
                    Conteudo += "Cidade: " + _clienteEntity.EN_CIDADE + "<br/>";
                    Conteudo += "Técnico: " + tecnicoEntity.NM_TECNICO + " - " + tecnicoEntity.empresa.NM_Empresa + "<br/>";

                    if (Id_Os != null && Id_Os != 0)
                    {
                        Conteudo += "OS: <strong>" + Convert.ToInt64(Id_Os) + "</strong><br/>";
                    }
                    else if (listOSPadrao.ID_OS != 0)
                    {
                        Conteudo += "OS: <strong>" + Convert.ToInt64(listOSPadrao.ID_OS) + "</strong><br/>";
                    }

                    Conteudo += "Data: " + Convert.ToDateTime(listOSPadrao.DT_DATA_OS).ToString("dd/MM/yyyy") + "<br/>";
                    Conteudo += "Hora Inicio: " + listOSPadrao.HR_INICIO.ToString() + "<br/>";
                    Conteudo += "Hora de Finalização: " + listOSPadrao.HR_FIM.ToString() + "<br/>";
                    Conteudo += "Ativo Fixo: " + listOSPadrao.AtivoFixo.CD_ATIVO_FIXO + "-" + listOSPadrao.AtivoFixo.modelo.DS_MODELO + "<br/>";
                    Conteudo += "Linha: " + listOSPadrao.NOME_LINHA + "<br/>";
                    Conteudo += "Observação: " + listOSPadrao.DS_OBSERVACAO + "<br/>";
                    if (listOSPadrao.DS_RESPONSAVEL != null)
                    {
                        Conteudo += "Acompanhante: " + listOSPadrao.DS_RESPONSAVEL.ToString() + "<br/>";
                    }
                    else
                    {
                        Conteudo += "Acompanhante: " + "" + "<br/>";
                    }

                    var manutencao = "";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 1)
                        manutencao = "Preventiva";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 2)
                        manutencao = "Corretiva";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 3)
                        manutencao = "Instalação";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 4)
                        manutencao = "Outros";
                    Conteudo += "Tipo de Manutenção: " + manutencao + "<br/>";
                    Conteudo += "Status da OS: Finalizada<br/><br/>";

                    if (pecasOS?.Count > 0)
                    {
                        Conteudo += "<p><strong>Peças Utilizadas: </strong></p>";
                        int contaPc = 0;
                        foreach (var peca in pecasOS)
                        {
                            contaPc++;
                            var estoque = peca.CD_TP_ESTOQUE_CLI_TEC == 'C' ? "Cliente" : "Intermediario";
                            Conteudo += $" {contaPc}. {peca.DS_PECA} || Quantidade: {peca.QT_PECA} || Estoque: {estoque}<br/>";
                        }
                    }

                    //Conteudo += $"<a class='btn btn-danger m-1' href = '{URLSite}'>Avalie o Atendimento</a>";
                    MensagemEmail.Replace("#Conteudo", Conteudo);
                    mailMessage = MensagemEmail.ToString();

                    mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);

                }



            }
            catch (Exception ex)
            {


            }
        }

        public void EnviarEmailVisita(VisitaPadraoEntity visitaPadrao, long? Id_Visita)
        {
            //Método Modelo envio de email
            try
            {
                //string emailsSolicitacaoPeca = ControlesUtility.Parametro.ObterValorParametro("emailsOSFinalizada");


                if (visitaPadrao.TpStatusVisita.ST_STATUS_VISITA == 4)
                {
                    TecnicoEntity tecnicoEntity = new TecnicoEntity();
                    tecnicoEntity.CD_TECNICO = visitaPadrao.Tecnico.CD_TECNICO;
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

                    ClienteEntity _clienteEntity = new ClienteEntity();
                    _clienteEntity.CD_CLIENTE = visitaPadrao.Cliente.CD_CLIENTE;
                    dataTableReader = new ClienteData().ObterLista(_clienteEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            _clienteEntity.CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]);
                            _clienteEntity.TX_EMAIL = dataTableReader["TX_EMAIL"].ToString();
                            _clienteEntity.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString();
                            _clienteEntity.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
                            _clienteEntity.EmailsInfo = dataTableReader["EmailsInfo"].ToString();
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }


                    // Envia a requisição de troca de senha por e-mail

                    MailSender mailSender = new MailSender();

                    string mailTo = "";

                    if (visitaPadrao.Email == null || visitaPadrao.Email == "")
                    {
                        //if (tecnicoEntity.usuario.cdsEmail != "")
                        //{
                        //    mailTo += tecnicoEntity.usuario.cdsEmail + ";";
                        //}
                        if (_clienteEntity.TX_EMAIL != "")
                        {
                            mailTo += _clienteEntity.TX_EMAIL + ";";
                        }
                    }else if (visitaPadrao.Email != null && visitaPadrao.Email != "")
                    {
                        if (visitaPadrao.Email == tecnicoEntity.usuario.cdsEmail)
                            mailTo += _clienteEntity.TX_EMAIL + ";";
                        else
                            mailTo += visitaPadrao.Email + ";";
                    }
                    
                    string mailSubject = "3M.Comodato - Finalização de Visita";
                    string mailMessage = string.Empty;
                    System.Net.Mail.Attachment Attachments = null;
                    string mailCopy = null;

                    var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                    string Conteudo = string.Empty;
                    string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                    //URLSite += "/SolicitacaoPecas/Editar?idKey=" + ControlesUtility.Criptografia.Criptografar(tecnicoEntity.CD_TECNICO + "|" + pedidoEntity.ID_PEDIDO.ToString() + "|" + pedidoEntity.TP_TIPO_PEDIDO + "|Solicitação");
                    URLSite += "/VisitaPadrao/Pesquisa?ID_VISITA=" + Id_Visita;
                    Conteudo += "<p>Uma Visita acaba de ser <strong>Finalizada </strong>!</p>";
                    Conteudo += "<p>Segue dados da Visita:</p>";
                    Conteudo += "Cliente: " + _clienteEntity.CD_CLIENTE + " - " + _clienteEntity.NM_CLIENTE + "<br/>";
                    Conteudo += "Cidade: " + _clienteEntity.EN_CIDADE + "<br/>";
                    Conteudo += "Técnico: " + tecnicoEntity.NM_TECNICO + " - " + tecnicoEntity.empresa.NM_Empresa + "<br/>";
                    if (Id_Visita != 0 && Id_Visita != null)
                    {
                        Conteudo += "Visita: <strong>" + Convert.ToInt64(Id_Visita) + "</strong><br/>";
                    }
                    else if (visitaPadrao.ID_VISITA != 0)
                    {
                        Conteudo += "Visita: <strong>" + Convert.ToInt64(visitaPadrao.ID_VISITA) + "</strong><br/>";
                    }
                    Conteudo += "Data: " + Convert.ToDateTime(visitaPadrao.DT_DATA_VISITA).ToString("dd/MM/yyyy") + "<br/>";
                    Conteudo += "Hora de Inicio: " + visitaPadrao.HR_INICIO + "<br/>";
                    Conteudo += "Hora Fim: " + visitaPadrao.HR_FIM + "<br/>";
                    Conteudo += "Observação: " + visitaPadrao.DS_OBSERVACAO + "<br/>";


                    if (visitaPadrao.DS_RESPONSAVEL != null)
                    {
                        Conteudo += "Acompanhante: " + visitaPadrao.DS_RESPONSAVEL + "<br/>";
                    }
                    else
                    {
                        Conteudo += "Acompanhante: " + "" + "<br/>";
                    }
                    

                    var Motivo = "";
                    if (visitaPadrao.TpMotivoVisita.CD_MOTIVO_VISITA == 1)
                        Motivo = "Integração";
                    if (visitaPadrao.TpMotivoVisita.CD_MOTIVO_VISITA == 2)
                        Motivo = "Reunião";
                    if (visitaPadrao.TpMotivoVisita.CD_MOTIVO_VISITA == 3)
                        Motivo = "Treinamento";
                    if (visitaPadrao.TpMotivoVisita.CD_MOTIVO_VISITA == 4)
                        Motivo = "Carga de Dados";
                    Conteudo += "Motivo: " + Motivo + "<br/>";
                    Conteudo += "Status da Visita: Finalizada<br/>";

                    var button = mailSender.GetConteudoHTML("button.html");

                    button.Replace("TEXTO_URL", "Avalie o Atendimento");
                    button.Replace("URL", URLSite);

                    Conteudo += button;
                    //Conteudo += $"<a class='btn btn-danger m-1' href = '{URLSite}'>Avalie o Atendimento</a>";

                    MensagemEmail.Replace("#Conteudo", Conteudo);
                    mailMessage = MensagemEmail.ToString();

                    mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);

                    if (_clienteEntity.EmailsInfo != null || _clienteEntity.EmailsInfo != "")
                    {
                        EnviarEmailVisitaInfo(visitaPadrao, Id_Visita);
                    }
                }



            }
            catch (Exception ex)
            {
                

            }
        }

        public void EnviarEmailVisitaInfo(VisitaPadraoEntity visitaPadrao, long? Id_Visita)
        {
            //Método Modelo envio de email
            try
            {
                //string emailsSolicitacaoPeca = ControlesUtility.Parametro.ObterValorParametro("emailsOSFinalizada");


                if (visitaPadrao.TpStatusVisita.ST_STATUS_VISITA == 4)
                {
                    TecnicoEntity tecnicoEntity = new TecnicoEntity();
                    tecnicoEntity.CD_TECNICO = visitaPadrao.Tecnico.CD_TECNICO;
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

                    ClienteEntity _clienteEntity = new ClienteEntity();
                    _clienteEntity.CD_CLIENTE = visitaPadrao.Cliente.CD_CLIENTE;
                    dataTableReader = new ClienteData().ObterLista(_clienteEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            _clienteEntity.CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]);
                            _clienteEntity.TX_EMAIL = dataTableReader["TX_EMAIL"].ToString();
                            _clienteEntity.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString();
                            _clienteEntity.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
                            _clienteEntity.EmailsInfo = dataTableReader["EmailsInfo"].ToString();
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }


                    // Envia a requisição de troca de senha por e-mail

                    MailSender mailSender = new MailSender();

                    string mailTo = "";

                    mailTo = _clienteEntity.EmailsInfo;

                    string mailSubject = "3M.Comodato - Finalização de Visita";
                    string mailMessage = string.Empty;
                    System.Net.Mail.Attachment Attachments = null;
                    string mailCopy = null;

                    var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                    string Conteudo = string.Empty;
                    string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                    //URLSite += "/SolicitacaoPecas/Editar?idKey=" + ControlesUtility.Criptografia.Criptografar(tecnicoEntity.CD_TECNICO + "|" + pedidoEntity.ID_PEDIDO.ToString() + "|" + pedidoEntity.TP_TIPO_PEDIDO + "|Solicitação");
                    URLSite += "/VisitaPadrao/Pesquisa?ID_VISITA=" + Id_Visita;
                    Conteudo += "<p>Uma Visita acaba de ser <strong>Finalizada </strong>!</p>";
                    Conteudo += "<p>Segue dados da Visita:</p>";
                    Conteudo += "Cliente: " + _clienteEntity.CD_CLIENTE + " - " + _clienteEntity.NM_CLIENTE + "<br/>";
                    Conteudo += "Cidade: " + _clienteEntity.EN_CIDADE + "<br/>";
                    Conteudo += "Técnico: " + tecnicoEntity.NM_TECNICO + " - " + tecnicoEntity.empresa.NM_Empresa + "<br/>";
                    if (Id_Visita != 0 && Id_Visita != null)
                    {
                        Conteudo += "Visita: <strong>" + Convert.ToInt64(Id_Visita) + "</strong><br/>";
                    }
                    else if (visitaPadrao.ID_VISITA != 0)
                    {
                        Conteudo += "Visita: <strong>" + Convert.ToInt64(visitaPadrao.ID_VISITA) + "</strong><br/>";
                    }
                    Conteudo += "Data: " + Convert.ToDateTime(visitaPadrao.DT_DATA_VISITA).ToString("dd/MM/yyyy") + "<br/>";
                    Conteudo += "Hora de Inicio: " + visitaPadrao.HR_INICIO + "<br/>";
                    Conteudo += "Hora Fim: " + visitaPadrao.HR_FIM + "<br/>";
                    Conteudo += "Observação: " + visitaPadrao.DS_OBSERVACAO + "<br/>";


                    if (visitaPadrao.DS_RESPONSAVEL != null)
                    {
                        Conteudo += "Acompanhante: " + visitaPadrao.DS_RESPONSAVEL + "<br/>";
                    }
                    else
                    {
                        Conteudo += "Acompanhante: " + "" + "<br/>";
                    }


                    var Motivo = "";
                    if (visitaPadrao.TpMotivoVisita.CD_MOTIVO_VISITA == 1)
                        Motivo = "Integração";
                    if (visitaPadrao.TpMotivoVisita.CD_MOTIVO_VISITA == 2)
                        Motivo = "Reunião";
                    if (visitaPadrao.TpMotivoVisita.CD_MOTIVO_VISITA == 3)
                        Motivo = "Treinamento";
                    if (visitaPadrao.TpMotivoVisita.CD_MOTIVO_VISITA == 4)
                        Motivo = "Carga de Dados";
                    Conteudo += "Motivo: " + Motivo + "<br/>";
                    Conteudo += "Status da Visita: Finalizada<br/>";

                    
                    //Conteudo += $"<a class='btn btn-danger m-1' href = '{URLSite}'>Avalie o Atendimento</a>";

                    MensagemEmail.Replace("#Conteudo", Conteudo);
                    mailMessage = MensagemEmail.ToString();

                    mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);
                    
                }



            }
            catch (Exception ex)
            {


            }
        }

        #endregion
        private void grava_OS(ref SqlTransaction trans, ref SqlConnection cnx, List<OSPadraoEntity> listaOSPadrao, long idUsuario, List<PecaOSSinc> listaPecaOS, List<PendenciaOSSinc> listaPendenciaOS,
                             List<LogStatusOSSinc> listaLogStatusOS, List<EstoqueMoviSinc> listaEstoqueMovi, List<RelatorioReclamacaoSincEntity> listaRR, List<RRComentSincEntity> listaRRComent, List<PendenciaOSSinc> listaPendenciaOSOutros)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnx;
                    cmd.Transaction = trans;
                    cmd.CommandType = CommandType.Text;

                    #region Ordem de Serviço - OS

                    foreach (OSPadraoEntity itemOS in listaOSPadrao)
                    {
                        Int64? _id_os = null;

                        cmd.CommandText = @"BEGIN
                                            DECLARE @TB_TEMP_ID TABLE (ID BIGINT);
                                             UPDATE tbOSPadrao WITH(UPDLOCK, SERIALIZABLE)
                                                SET DT_DATA_OS = @DT_DATA_OS, 
                                                    ST_STATUS_OS = @ST_STATUS_OS, 
                                                    CD_TIPO_OS = @CD_TIPO_OS, 
                                                    CD_CLIENTE = @CD_CLIENTE, 
                                                    CD_TECNICO = @CD_TECNICO, 
                                                    HR_INICIO = @HR_INICIO, 
                                                    HR_FIM = @HR_FIM, 
                                                    CD_ATIVO_FIXO = @CD_ATIVO_FIXO,
                                                    DS_OBSERVACAO = @DS_OBSERVACAO,
                                                    Email = @Email,
                                                    NOME_LINHA = @NOME_LINHA,
                                                    DS_RESPONSAVEL = @DS_RESPONSAVEL,
                                                    nidUsuarioAtualizacao = @nidUsuarioAtualizacao, 
                                                    dtmDataHoraAtualizacao = @DataAtualizacaoVisita 
                                                    OUTPUT inserted.ID_OS INTO @TB_TEMP_ID
                                                WHERE TOKEN = @TOKEN 
                                            IF @@ROWCOUNT = 0
                                            BEGIN
                                                INSERT INTO tbOSPadrao 
                                                       ( DT_DATA_OS, ST_STATUS_OS, CD_TIPO_OS, CD_CLIENTE, CD_TECNICO, HR_INICIO, HR_FIM, CD_ATIVO_FIXO, 
                                                         DS_OBSERVACAO, nidUsuarioAtualizacao, dtmDataHoraAtualizacao, TOKEN, NOME_LINHA, Email, DS_RESPONSAVEL, Origem )
                                                       OUTPUT inserted.ID_OS INTO @TB_TEMP_ID
                                                VALUES ( @DT_DATA_OS, @ST_STATUS_OS, @CD_TIPO_OS, @CD_CLIENTE, @CD_TECNICO, @HR_INICIO, @HR_FIM, @CD_ATIVO_FIXO, 
                                                         @DS_OBSERVACAO, @nidUsuarioAtualizacao, @DataAtualizacaoVisita, @TOKEN, @NOME_LINHA, @Email, @DS_RESPONSAVEL, @Origem );
                                            END
                                            select ID from @TB_TEMP_ID;
                                            END";

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@DT_DATA_OS", SqlDbType.DateTime).Value = itemOS.DT_DATA_OS;
                        cmd.Parameters.Add("@ST_STATUS_OS", SqlDbType.Int).Value = itemOS.TpStatusOS.ST_STATUS_OS;
                        cmd.Parameters.Add("@CD_TIPO_OS", SqlDbType.Int).Value = itemOS.TpOS.CD_TIPO_OS;
                        cmd.Parameters.Add("@CD_CLIENTE", SqlDbType.Int).Value = itemOS.Cliente.CD_CLIENTE;
                        cmd.Parameters.Add("@CD_TECNICO", SqlDbType.VarChar).Value = itemOS.Tecnico.CD_TECNICO;

                        if (string.IsNullOrWhiteSpace(itemOS.HR_INICIO))
                            cmd.Parameters.Add("@HR_INICIO", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            cmd.Parameters.Add("@HR_INICIO", SqlDbType.VarChar).Value = itemOS.HR_INICIO;

                        if (string.IsNullOrWhiteSpace(itemOS.HR_FIM))
                            cmd.Parameters.Add("@HR_FIM", SqlDbType.VarChar).Value = DBNull.Value;
                        else
                            cmd.Parameters.Add("@HR_FIM", SqlDbType.VarChar).Value = itemOS.HR_FIM;

                        cmd.Parameters.Add("@CD_ATIVO_FIXO", SqlDbType.VarChar).Value = itemOS.AtivoFixo.CD_ATIVO_FIXO;
                        cmd.Parameters.Add("@DS_OBSERVACAO", SqlDbType.VarChar).Value = itemOS.DS_OBSERVACAO;
                        cmd.Parameters.Add("@nidUsuarioAtualizacao", SqlDbType.BigInt).Value = idUsuario;
                        cmd.Parameters.Add("@DataAtualizacaoVisita", SqlDbType.DateTime).Value = itemOS.dtmDataHoraAtualizacao == DateTime.MinValue ? DateTime.Now : itemOS.dtmDataHoraAtualizacao;
                        cmd.Parameters.Add("@TOKEN", SqlDbType.BigInt).Value = itemOS.TOKEN;
                        cmd.Parameters.Add("@NOME_LINHA", SqlDbType.VarChar).Value = itemOS.NOME_LINHA;
                        cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = itemOS.Email;
                        cmd.Parameters.Add("@DS_RESPONSAVEL", SqlDbType.VarChar).Value = itemOS.DS_RESPONSAVEL;
                        cmd.Parameters.Add("@Origem", SqlDbType.VarChar).Value = "A"; 
                        cmd.Connection = cnx;
                        _id_os = Convert.ToInt64(cmd.ExecuteScalar());
                        
                        
                        #region Grava Peca OS

                        if (listaPecaOS?.Count > 0)
                        {
                            if (listaPecaOS.Any(v => v.TOKEN == 0))
                                throw new Exception($"Existe peça sem token informado na OS {itemOS.ID_OS} na sincronização enviada.");

                            grava_PecaOS(ref trans, ref cnx, listaPecaOS, idUsuario, _id_os, itemOS);
                        }

                        if (listaPecaOS?.Count == 0 && itemOS.TpStatusOS.ST_STATUS_OS == OS_STATUS_FINALIZADA)
                        {
                            var listaPecasOs = ObterPecasOs(itemOS.ID_OS);

                            if (listaPecasOs?.Count > 0)
                            {
                                var pecasOsEstoqueMovimentado = ObterPecasOSComMovimentacaoEstoque(ref trans, ref cnx, itemOS.ID_OS);

                                foreach (var peca in listaPecasOs)
                                {
                                    if (PermitirMovimentarEstoque(itemOS) && PecaNaoMovimentouEstoqueOs(pecasOsEstoqueMovimentado, peca))
                                    {
                                        if (!RealizarAtualizacaoEstoque(ref trans, ref cnx, peca, itemOS, idUsuario))
                                        {
                                            if (peca.ID_PECA_OS > 0)
                                                ExcluirPecaOs(cmd, peca);

                                            CriarNotificacao(ref trans, ref cnx, $@"EXCLUSÃO DE PEÇA NA O.S. {_id_os}", $@"A O.S. {_id_os} teve a peça {peca.CD_PECA} excluída, pois não havia estoque suficiente no momento da sincronização. Entre em contato com o Administrador.", idUsuario);

                                            continue;
                                        }
                                    }
                                }
                                
                            }
                        }

                        #endregion

                        if (itemOS.TpStatusOS.ST_STATUS_OS == 3)
                        {
                            EnviarEmailOS(itemOS, _id_os);
                        }

                        #region Grava Pendencia OS

                        if (listaPendenciaOS?.Count > 0)
                        {
                            if (listaPendenciaOS.Any(v => v.TOKEN == 0))
                                throw new Exception($"Existe pendência sem token informado na OS {itemOS.ID_OS} na sincronização enviada.");

                            GravarPendenciaOS(ref trans, ref cnx, listaPendenciaOS, idUsuario, itemOS.IDENTIFICADOR_PK_ID_OS, _id_os);
                        }

                        #endregion

                        #region Gravar Reclamação

                        if (listaRR?.Count > 0)
                        {
                            if (listaRR.Any(v => v.TOKEN == 0))
                                throw new Exception($"Existe reclamação sem token informado na OS {itemOS.ID_OS} na sincronização enviada.");

                            grava_RR(ref trans, ref cnx, listaRR, idUsuario, itemOS.IDENTIFICADOR_PK_ID_OS, _id_os, listaRRComent);
                        }

                        #endregion

                        #region Grava LOG_STATUS_OS

                        if (listaLogStatusOS != null)
                            grava_LogStatusOs(ref trans, ref cnx, listaLogStatusOS, idUsuario, itemOS.IDENTIFICADOR_PK_ID_OS, _id_os);

                        #endregion

                        
                        
                        //============== Grava Estoque_Movi -------------------------------------
                        //TODO: Edgar - Retirar
                        if (listaEstoqueMovi != null)
                            grava_EstoqueMovi(ref trans, ref cnx, listaEstoqueMovi, idUsuario, itemOS.IDENTIFICADOR_PK_ID_OS, _id_os, itemOS.Cliente.CD_CLIENTE);
                    }

                    #region Grava Pendências OS OUTROS

                    if (listaPendenciaOSOutros?.Count > 0)
                    {
                        if (listaPendenciaOSOutros.Any(v => v.TOKEN == 0))
                            throw new Exception($"Existe pendência OUTROS sem token informado.");

                        GravarPendenciaOutrosOS(ref trans, ref cnx, listaPendenciaOSOutros, idUsuario);
                    }

                    #endregion

                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<PecaOSSinc> ObterPecasOs(Int64 ID_OS)
        {
            try
            {
                IList<PecaOSSinc> listaPecas = new List<PecaOSSinc>();

                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         @"SELECT *
                                             FROM tbPecaOS
                                            WHERE ID_OS = @ID_OS ";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_OS", SqlDbType.BigInt).Value = ID_OS;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            PecaOSSinc peca = new PecaOSSinc
                            {
                                ID_PECA_OS = Convert.ToInt64(SDR["ID_PECA_OS"]),
                                ID_OS = Convert.ToInt64(SDR["ID_OS"]),
                                CD_PECA = SDR["CD_PECA"].ToString(),
                                QT_PECA = Convert.ToInt64(SDR["QT_PECA"]),
                                CD_TP_ESTOQUE_CLI_TEC = SDR["CD_TP_ESTOQUE_CLI_TEC"].ToString().ToArray()[0]
                            };

                            listaPecas.Add(peca);
                        }

                        cnx.Close();
                        return listaPecas;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void grava_LogStatusVisita(ref SqlTransaction trans, ref SqlConnection cnx, VisitaPadraoEntity visitaPadrao, long idUsuario, Int64? _idVisita)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnx;
                    cmd.Transaction = trans;
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = " INSERT INTO tbLogStatusVisitaPadrao " +
                                        " ( ID_VISITA, DT_DATA_LOG_VISITA, ST_STATUS_VISITA, nidUsuarioAtualizacao, dtmDataHoraAtualizacao) " +
                                        " VALUES ( @ID_VISITA, @DT_DATA_LOG_VISITA, @ST_STATUS_VISITA, " +
                                        "          @nidUsuarioAtualizacao, @DataAtualizacaoVisita ) ";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@ID_VISITA", System.Data.SqlDbType.BigInt).Value = _idVisita;
                    cmd.Parameters.Add("@DT_DATA_LOG_VISITA", System.Data.SqlDbType.DateTime).Value = visitaPadrao.DT_DATA_VISITA;
                    cmd.Parameters.Add("@ST_STATUS_VISITA", System.Data.SqlDbType.Int).Value = visitaPadrao.TpStatusVisita.ST_STATUS_VISITA;
                    cmd.Parameters.Add("@dtmDataHoraAtualizacao", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@nidUsuarioAtualizacao", System.Data.SqlDbType.BigInt).Value = idUsuario;
                    cmd.Connection = cnx;
                    int c = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return;
        }

        /// <summary>
        /// Atualiza o Status do Log de Sincronismo
        /// </summary>
        /// <param name="id_log_sincronismo"></param>
        public void AtualizaLogSincronismo(long id_log_sincronismo, string resultadoSincronismo)
        {
            //SqlDataReader SDR = null;
            using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
            {
                cnx.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " UPDATE tbLogSincronismo " +
                                         " SET st_status_sincronismo = @resultadoSincronismo " +
                                         " WHERE id_log_sincronismo = @id_log_sincronismo ;";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@id_log_sincronismo", System.Data.SqlDbType.BigInt).Value = id_log_sincronismo;
                        cmd.Parameters.Add("@resultadoSincronismo", System.Data.SqlDbType.VarChar, -1).Value = resultadoSincronismo;

                        cmd.Connection = cnx;
                        Int64 c = Convert.ToInt64(cmd.ExecuteNonQuery());
                        cnx.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cnx.Close();
                    cnx.Dispose();
                }

            }
        }

        /// <summary>
        /// Grava uma ocorrencia de tentativa de Log e os dados enviados
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="JO"></param>
        /// <returns></returns>
        public Int64 GravaLogSincronismo(long idUsuario, string JO)
        {
            //SqlDataReader SDR = null;
            using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
            {
                cnx.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " INSERT INTO tbLogSincronismo " +
                                         " ( dt_data_sincronismo, id_usuario, ds_json ) " +
                                         " VALUES ( getdate(), @id_usuario, @ds_json ); " +
                                         " select @@IDENTITY ;";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@id_usuario", System.Data.SqlDbType.BigInt).Value = idUsuario;
                        cmd.Parameters.Add("@ds_json", System.Data.SqlDbType.NVarChar, -1).Value = JO;

                        cmd.Connection = cnx;
                        Int64 c = Convert.ToInt64(cmd.ExecuteScalar());
                        cnx.Close();
                        return c;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cnx.Close();
                    cnx.Dispose();
                }

            }
        }

        private void atualizaAgenda(Int64 idUsuario, IList<AgendaSinc> listaAgenda)
        {
            using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
            {
                cnx.Open();
                SqlTransaction trans = cnx.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Transaction = trans;
                        cmd.CommandText =
                                         " UPDATE tbAGENDA SET NR_ORDENACAO = @NR_ORDENACAO WHERE ID_AGENDA = @ID_AGENDA ";

                        cmd.CommandType = CommandType.Text;

                        foreach (AgendaSinc itemAgenda in listaAgenda.OrderByDescending(e => e.NR_ORDENACAO))
                        {
                            if (itemAgenda.ID_AGENDA > 0)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@ID_AGENDA", System.Data.SqlDbType.BigInt).Value = itemAgenda.ID_AGENDA;
                                cmd.Parameters.Add("@NR_ORDENACAO", System.Data.SqlDbType.BigInt).Value = itemAgenda.NR_ORDENACAO;

                                cmd.Connection = cnx;

                                int c = cmd.ExecuteNonQuery();
                            }
                        }
                        cmd.Dispose();
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    trans.Dispose();
                    cnx.Close();
                    cnx.Dispose();
                }
            }

        }



        private void ModificarNotificacaoParaVisualizada(IList<int> CodigosNotificacao)
        {
            using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
            {
                cnx.Open();
                SqlTransaction trans = cnx.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Transaction = trans;
                        cmd.CommandText =
                            $@"UPDATE tbNotificacao
                                   SET LIDA = 1
                                WHERE ID_NOTIFICACAO IN ({String.Join(",", CodigosNotificacao)})";

                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cnx;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    trans.Dispose();
                    cnx.Close();
                    cnx.Dispose();
                }
            }
        }

        

    }
}
