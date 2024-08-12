using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class PedidoPecaData
    {
        private const string STATUS_ITEM_SOLICITACAO_CANCELADO = "4";
        private const char STATUS_ITEM_SOLICITACAO_SOLICITADO = '6';
        private readonly Database _db;
        private DbConnection _connection;
        private DbCommand dbCommand;
        internal DbTransaction _transaction;

        public PedidoPecaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }
        public PedidoPecaData(TransactionData transactionData)
        {
            _db = transactionData._db;
            _connection = transactionData._connection;
            _transaction = transactionData._transaction;
        }

        /// <summary>
        /// Obtem lista de pecas de pedidos para o sincronismo Mobile, de um usuario/tecnico
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>  
        public IList<PedidoPecaSinc> ObterListaPedidoPecaSinc(Int64 idUsuario)
        {
            try
            {
                IList<PedidoPecaSinc> listaPedidoPeca = new List<PedidoPecaSinc>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"select (ISNULL(pp.QTD_APROVADA, 0) - ISNULL(pp.QTD_RECEBIDA, 0)) QTD_PENDENTE, pp.* 
                                  from tb_pedido_peca pp 
                                 inner join tb_pedido ped 
                                    on ped.ID_PEDIDO = pp.ID_PEDIDO 
                                 inner join tb_tecnico t 
                                    on t.cd_tecnico = ped.cd_tecnico 
                                 where (t.ID_USUARIO = @ID_USUARIO OR t.ID_USUARIO_COORDENADOR = @ID_USUARIO) 
                                   and ped.ID_PEDIDO IN(select p.ID_PEDIDO 
                                                          from TB_PEDIDO p
                                                         where ((p.ID_STATUS_PEDIDO NOT IN(4, 7) AND CAST(p.DT_CRIACAO AS DATE) >= CAST(DATEADD(MONTH, -6, GETDATE()) AS DATE)) --Não seja Recebido ou Cancelado
      				                                             OR (p.ID_STATUS_PEDIDO IN(4, 7) AND CAST(p.DT_CRIACAO AS DATE) >= CAST(DATEADD(day, -30, GETDATE()) AS DATE))) --Somente Recebido ou Cancelado
						                                   and p.CD_TECNICO = ped.CD_TECNICO 
						                                )";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", SqlDbType.BigInt).Value = idUsuario;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            PedidoPecaSinc pedidoPeca = new PedidoPecaSinc
                            {
                                ID_ITEM_PEDIDO = Convert.ToInt64(SDR["ID_ITEM_PEDIDO"].ToString()),
                                ID_PEDIDO = Convert.ToInt64(SDR["ID_PEDIDO"].ToString()),
                                CD_PECA = Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString()),
                                QTD_SOLICITADA = Convert.ToInt64(SDR["QTD_SOLICITADA"] is DBNull ? 0 : SDR["QTD_SOLICITADA"]),
                                QTD_APROVADA = Convert.ToInt64(SDR["QTD_APROVADA"] is DBNull ? 0 : SDR["QTD_APROVADA"]),
                                TX_APROVADO = Convert.ToString(SDR["TX_APROVADO"] is DBNull ? "" : SDR["TX_APROVADO"].ToString()),
                                NR_DOC_ORI = Convert.ToInt64(SDR["NR_DOC_ORI"] is DBNull ? 0 : SDR["NR_DOC_ORI"]),
                                QTD_RECEBIDA = Convert.ToInt64(SDR["QTD_RECEBIDA"] is DBNull ? 0 : SDR["QTD_RECEBIDA"]),
                                ST_STATUS_ITEM = Convert.ToChar(SDR["ST_STATUS_ITEM"] is DBNull ? " " : SDR["ST_STATUS_ITEM"].ToString()),
                                DS_OBSERVACAO = Convert.ToString(SDR["DS_OBSERVACAO"] is DBNull ? "" : SDR["DS_OBSERVACAO"].ToString()),
                                DS_DIR_FOTO = Convert.ToString(SDR["DS_DIR_FOTO"] is DBNull ? "" : SDR["DS_DIR_FOTO"].ToString()),
                                ID_ESTOQUE_DEBITO = Convert.ToInt32(SDR["ID_ESTOQUE_DEBITO"] is DBNull ? null : SDR["ID_ESTOQUE_DEBITO"]),
                                TIPO_PECA = Convert.ToByte(SDR["TIPO_PECA"] is DBNull ? null : SDR["TIPO_PECA"]),
                                DESCRICAO_PECA = Convert.ToString(SDR["DESCRICAO_PECA"] is DBNull ? "" : SDR["DESCRICAO_PECA"].ToString()),
                                QTD_PENDENTE = Convert.ToInt64(SDR["QTD_PENDENTE"] is DBNull ? 0 : SDR["QTD_PENDENTE"]),
                                TOKEN = Convert.ToInt64(SDR["TOKEN"].ToString())
                            };

                            //TODO: Edgar - O código abaixo é provisório, e deve ser removido quando implementado os novos ajustes de solicitações de peça.
                            if (pedidoPeca.ST_STATUS_ITEM == STATUS_ITEM_SOLICITACAO_SOLICITADO)
                                pedidoPeca.QTD_APROVADA = 0;
                            if (pedidoPeca.QTD_PENDENTE < 0)
                            {
                                pedidoPeca.QTD_PENDENTE = 0;
                            }
                            pedidoPeca.QTD_RECEBIDA = 0;
                            listaPedidoPeca.Add(pedidoPeca);
                        }

                        cnx.Close();
                        return listaPedidoPeca;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<PedidoPecaSinc> ObterListaPedidoPecaPedido(Int64 id_Pedido)
        {
            try
            {
                IList<PedidoPecaSinc> listaPedidoPeca = new List<PedidoPecaSinc>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandTimeout = 500000;
                        cmd.CommandText =
                              @"select pp.* 
                                  from tb_pedido_peca pp with(nolock) 
                                 inner join tb_pedido ped with(nolock)
                                    on ped.ID_PEDIDO = pp.ID_PEDIDO 
                                 inner join tb_tecnico t with(nolock)
                                    on t.cd_tecnico = ped.cd_tecnico 
                                 where ped.ID_PEDIDO = @ID_PEDIDO";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_PEDIDO", SqlDbType.BigInt).Value = id_Pedido;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            PedidoPecaSinc pedidoPeca = new PedidoPecaSinc
                            {
                                ID_ITEM_PEDIDO = Convert.ToInt64(SDR["ID_ITEM_PEDIDO"].ToString()),
                                ID_PEDIDO = Convert.ToInt64(SDR["ID_PEDIDO"].ToString()),
                                CD_PECA = Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString()),
                                QTD_SOLICITADA = Convert.ToInt64(SDR["QTD_SOLICITADA"] is DBNull ? 0 : SDR["QTD_SOLICITADA"]),
                                QTD_APROVADA = Convert.ToInt64(SDR["QTD_APROVADA"] is DBNull ? 0 : SDR["QTD_APROVADA"]),
                                TX_APROVADO = Convert.ToString(SDR["TX_APROVADO"] is DBNull ? "" : SDR["TX_APROVADO"].ToString()),
                                NR_DOC_ORI = Convert.ToInt64(SDR["NR_DOC_ORI"] is DBNull ? 0 : SDR["NR_DOC_ORI"]),
                                QTD_RECEBIDA = Convert.ToInt64(SDR["QTD_RECEBIDA"] is DBNull ? 0 : SDR["QTD_RECEBIDA"]),
                                ST_STATUS_ITEM = Convert.ToChar(SDR["ST_STATUS_ITEM"] is DBNull ? " " : SDR["ST_STATUS_ITEM"].ToString()),
                                DS_OBSERVACAO = Convert.ToString(SDR["DS_OBSERVACAO"] is DBNull ? "" : SDR["DS_OBSERVACAO"].ToString()),
                                DS_DIR_FOTO = Convert.ToString(SDR["DS_DIR_FOTO"] is DBNull ? "" : SDR["DS_DIR_FOTO"].ToString()),
                                ID_ESTOQUE_DEBITO = Convert.ToInt32(SDR["ID_ESTOQUE_DEBITO"] is DBNull ? null : SDR["ID_ESTOQUE_DEBITO"]),
                                QTD_APROVADA_3M1 = Convert.ToInt64(SDR["QTD_APROVADA_3M1"] is DBNull ? 0 : SDR["QTD_APROVADA_3M1"]),
                                QTD_APROVADA_3M2 = Convert.ToInt64(SDR["QTD_APROVADA_3M2"] is DBNull ? 0 : SDR["QTD_APROVADA_3M2"]),
                                TIPO_PECA = Convert.ToByte(SDR["TIPO_PECA"] is DBNull ? null : SDR["TIPO_PECA"]),
                                DESCRICAO_PECA = Convert.ToString(SDR["DESCRICAO_PECA"] is DBNull ? "" : SDR["DESCRICAO_PECA"].ToString()),
                                TOKEN = Convert.ToInt64(SDR["TOKEN"].ToString()),
                                QTD_ULTIMO_RECEBIMENTO = Convert.ToInt64(SDR["QTD_ULTIMO_RECEBIMENTO"] is DBNull ? 0 : SDR["QTD_ULTIMO_RECEBIMENTO"]),
                                atualizado = Convert.ToString(SDR["atualizado"] is DBNull ? "" : SDR["atualizado"].ToString()),
                                CD_PECA_REFERENCIA = Convert.ToString(SDR["CD_PECA_REFERENCIA"] is DBNull ? "" : SDR["CD_PECA_REFERENCIA"].ToString()),
                                VL_PECA = Convert.ToDecimal(SDR["VL_PECA"] is DBNull ? 0 : SDR["VL_PECA"])
                            };

                            //TODO: Edgar - O código abaixo é provisório, e deve ser removido quando implementado os novos ajustes de solicitações de peça.
                            if (pedidoPeca.ST_STATUS_ITEM == STATUS_ITEM_SOLICITACAO_SOLICITADO)
                                pedidoPeca.QTD_APROVADA = 0;

                            listaPedidoPeca.Add(pedidoPeca);
                        }

                        cnx.Close();
                        return listaPedidoPeca;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PedidoPecaSinc> ObterListaPedidoPecaPedidoTotal(Int64 id_Pedido)
        {
            try
            {
                List<PedidoPecaSinc> listaPedidoPeca = new List<PedidoPecaSinc>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"select pp.* 
                                  from tb_pedido_peca pp with(nolock) 
                                 inner join tb_pedido ped with(nolock)
                                    on ped.ID_PEDIDO = pp.ID_PEDIDO 
                                 inner join tb_tecnico t with(nolock)
                                    on t.cd_tecnico = ped.cd_tecnico 
                                 where ped.ID_PEDIDO = @ID_PEDIDO";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_PEDIDO", SqlDbType.BigInt).Value = id_Pedido;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            PedidoPecaSinc pedidoPeca = new PedidoPecaSinc
                            {
                                ID_ITEM_PEDIDO = Convert.ToInt64(SDR["ID_ITEM_PEDIDO"].ToString()),
                                ID_PEDIDO = Convert.ToInt64(SDR["ID_PEDIDO"].ToString()),
                                CD_PECA = Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString()),
                                QTD_SOLICITADA = Convert.ToInt64(SDR["QTD_SOLICITADA"] is DBNull ? 0 : SDR["QTD_SOLICITADA"]),
                                QTD_APROVADA = Convert.ToInt64(SDR["QTD_APROVADA"] is DBNull ? 0 : SDR["QTD_APROVADA"]),
                                TX_APROVADO = Convert.ToString(SDR["TX_APROVADO"] is DBNull ? "" : SDR["TX_APROVADO"].ToString()),
                                NR_DOC_ORI = Convert.ToInt64(SDR["NR_DOC_ORI"] is DBNull ? 0 : SDR["NR_DOC_ORI"]),
                                QTD_RECEBIDA = Convert.ToInt64(SDR["QTD_RECEBIDA"] is DBNull ? 0 : SDR["QTD_RECEBIDA"]),
                                ST_STATUS_ITEM = Convert.ToChar(SDR["ST_STATUS_ITEM"] is DBNull ? " " : SDR["ST_STATUS_ITEM"].ToString()),
                                DS_OBSERVACAO = Convert.ToString(SDR["DS_OBSERVACAO"] is DBNull ? "" : SDR["DS_OBSERVACAO"].ToString()),
                                DS_DIR_FOTO = Convert.ToString(SDR["DS_DIR_FOTO"] is DBNull ? "" : SDR["DS_DIR_FOTO"].ToString()),
                                ID_ESTOQUE_DEBITO = Convert.ToInt32(SDR["ID_ESTOQUE_DEBITO"] is DBNull ? null : SDR["ID_ESTOQUE_DEBITO"]),
                                QTD_APROVADA_3M1 = Convert.ToInt64(SDR["QTD_APROVADA_3M1"] is DBNull ? 0 : SDR["QTD_APROVADA_3M1"]),
                                QTD_APROVADA_3M2 = Convert.ToInt64(SDR["QTD_APROVADA_3M2"] is DBNull ? 0 : SDR["QTD_APROVADA_3M2"]),
                                TIPO_PECA = Convert.ToByte(SDR["TIPO_PECA"] is DBNull ? null : SDR["TIPO_PECA"]),
                                DESCRICAO_PECA = Convert.ToString(SDR["DESCRICAO_PECA"] is DBNull ? "" : SDR["DESCRICAO_PECA"].ToString()),
                                TOKEN = Convert.ToInt64(SDR["TOKEN"].ToString()),
                                VL_PECA = Convert.ToDecimal(SDR["VL_PECA"] is DBNull ? 0 : SDR["VL_PECA"])
                            };

                            //TODO: Edgar - O código abaixo é provisório, e deve ser removido quando implementado os novos ajustes de solicitações de peça.
                            if (pedidoPeca.ST_STATUS_ITEM == STATUS_ITEM_SOLICITACAO_SOLICITADO)
                                pedidoPeca.QTD_APROVADA = 0;

                            listaPedidoPeca.Add(pedidoPeca);
                        }

                        cnx.Close();
                        return listaPedidoPeca;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Inserir(ref PedidoPecaEntity pedidoPecaEntity)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaInsert");
                dbCommand.CommandTimeout = 500000;
                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, pedidoPecaEntity.pedido.ID_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, pedidoPecaEntity.peca.CD_PECA);
                _db.AddInParameter(dbCommand, "@p_CD_PECA_REFERENCIA", DbType.String, pedidoPecaEntity.CD_PECA_REFERENCIA);
                _db.AddInParameter(dbCommand, "@p_QTD_SOLICITADA", DbType.Decimal, pedidoPecaEntity.QTD_SOLICITADA);
                _db.AddInParameter(dbCommand, "@p_QTD_APROVADA", DbType.Decimal, pedidoPecaEntity.QTD_APROVADA);
                _db.AddInParameter(dbCommand, "@p_QTD_RECEBIDA", DbType.Decimal, pedidoPecaEntity.QTD_RECEBIDA);
                _db.AddInParameter(dbCommand, "@p_TX_APROVADO", DbType.String, pedidoPecaEntity.TX_APROVADO);

                if (pedidoPecaEntity.NR_DOC_ORI > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_NR_DOC_ORI", DbType.Int64, pedidoPecaEntity.NR_DOC_ORI);
                }

                _db.AddInParameter(dbCommand, "@p_ST_STATUS_ITEM", DbType.String, pedidoPecaEntity.ST_STATUS_ITEM);
                _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, pedidoPecaEntity.DS_OBSERVACAO);
                _db.AddInParameter(dbCommand, "@p_DS_DIR_FOTO", DbType.String, pedidoPecaEntity.DS_DIR_FOTO);

                if (pedidoPecaEntity.estoque3M1.ID_ESTOQUE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE_DEBITO", DbType.Int64, pedidoPecaEntity.estoque3M1.ID_ESTOQUE);
                }

                if (pedidoPecaEntity.estoque3M2.ID_ESTOQUE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE_DEBITO_3M2", DbType.Int64, pedidoPecaEntity.estoque3M2.ID_ESTOQUE);
                }

                if (pedidoPecaEntity.QTD_APROVADA_3M1 > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_APROVADA_3M1", DbType.Decimal, pedidoPecaEntity.QTD_APROVADA_3M1);
                }

                if (pedidoPecaEntity.QTD_APROVADA_3M2 > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_APROVADA_3M2", DbType.Decimal, pedidoPecaEntity.QTD_APROVADA_3M2);
                }

                if (pedidoPecaEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pedidoPecaEntity.nidUsuarioAtualizacao);
                }

                _db.AddInParameter(dbCommand, "@p_VL_PECA", DbType.Decimal, pedidoPecaEntity.VL_PECA);
                _db.AddInParameter(dbCommand, "@p_TIPO_PECA", DbType.Byte, pedidoPecaEntity.TIPO_PECA);
                _db.AddInParameter(dbCommand, "@p_DESCRICAO_PECA", DbType.String, pedidoPecaEntity.DESCRICAO_PECA);
                _db.AddInParameter(dbCommand, "@p_TOKEN", DbType.Int64, pedidoPecaEntity.TOKEN);
                _db.AddOutParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, 18);
                _db.AddOutParameter(dbCommand, "@p_TOKEN_GERADO", DbType.Int64, 18);
                
                _db.ExecuteNonQuery(dbCommand);

                pedidoPecaEntity.ID_ITEM_PEDIDO = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_ITEM_PEDIDO"));
                pedidoPecaEntity.TOKEN = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_TOKEN_GERADO"));

                retorno = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;

        }

        public void InserirPecaRecuperada(PedidoPecaEntity itemPedidoPeca)
        {
            SqlConnection cnx = new SqlConnection(_db.ConnectionString);
            
            
            try
            {
                
                using (SqlCommand cmd = new SqlCommand())
                {
                    cnx.Open();
                    cmd.Connection = cnx;
                    SqlTransaction trans = cnx.BeginTransaction();
                    cmd.Transaction = trans;
                    cmd.CommandTimeout = 40;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"Begin
                                               INSERT INTO TB_PEDIDO_PECA 
                                                      ( ID_PEDIDO, CD_PECA, CD_PECA_REFERENCIA, QTD_SOLICITADA, QTD_APROVADA, QTD_APROVADA_3M1, QTD_APROVADA_3M2, TX_APROVADO, NR_DOC_ORI,
                                                        QTD_RECEBIDA, ST_STATUS_ITEM, ID_ESTOQUE_DEBITO, ID_ESTOQUE_DEBITO_3M2, VL_PECA,
                                                        TOKEN, TIPO_PECA, DESCRICAO_PECA
                                                      )
                                               VALUES ( @ID_PEDIDO, @CD_PECA, @CD_PECA_REFERENCIA, @QTD_SOLICITADA, @QTD_APROVADA, @QTD_APROVADA_3M1, @QTD_APROVADA_3M2, @TX_APROVADO, null,
                                                        @QTD_RECEBIDA, @ST_STATUS_ITEM, null, 2, @VL_PECA,
                                                        @TOKEN, @TIPO_PECA, @DESCRICAO_PECA
                                                      )
                                        END";

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@ID_PEDIDO", SqlDbType.Int).Value = itemPedidoPeca.pedido.ID_PEDIDO;
                    cmd.Parameters.Add("@CD_PECA", SqlDbType.VarChar).Value = Convert.ToString(itemPedidoPeca.peca.CD_PECA);
                    cmd.Parameters.Add("@CD_PECA_REFERENCIA", SqlDbType.VarChar).Value = Convert.ToString(itemPedidoPeca.CD_PECA_REFERENCIA);
                    cmd.Parameters.Add("@QTD_SOLICITADA", SqlDbType.Int).Value = itemPedidoPeca.QTD_SOLICITADA ?? (object)DBNull.Value;
                    //cmd.Parameters.Add("@QTD_APROVADA", SqlDbType.Decimal).Value = itemPedidoPeca.QTD_SOLICITADA ?? (object)DBNull.Value;  Linha antiga recebendo Qtd_Aprovada = a QTD_Solicitada
                    cmd.Parameters.Add("@QTD_APROVADA", SqlDbType.Decimal).Value = itemPedidoPeca.QTD_APROVADA;
                    cmd.Parameters.Add("@QTD_APROVADA_3M1", SqlDbType.Decimal).Value = itemPedidoPeca.QTD_APROVADA_3M1;
                    cmd.Parameters.Add("@QTD_APROVADA_3M2", SqlDbType.Decimal).Value = itemPedidoPeca.QTD_APROVADA_3M2;
                    cmd.Parameters.Add("@TX_APROVADO", SqlDbType.VarChar).Value = 'S';
                    cmd.Parameters.Add("@QTD_RECEBIDA", SqlDbType.Int).Value = itemPedidoPeca.QTD_RECEBIDA ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@ST_STATUS_ITEM", SqlDbType.Char).Value = '8';
                    cmd.Parameters.Add("@VL_PECA", SqlDbType.Decimal).Value = itemPedidoPeca.VL_PECA;
                    cmd.Parameters.Add("@TIPO_PECA", SqlDbType.TinyInt).Value = itemPedidoPeca.TIPO_PECA;
                    cmd.Parameters.Add("@DESCRICAO_PECA", SqlDbType.VarChar).Value = itemPedidoPeca.DESCRICAO_PECA;
                    cmd.Parameters.Add("@TOKEN", SqlDbType.BigInt).Value = itemPedidoPeca.TOKEN;
                    cmd.Connection = cnx;
                    int c = cmd.ExecuteNonQuery();
                    trans.Commit();
                    cnx.Close();
                    
                }
            }
            catch (Exception ex)
            {
                cnx.Close();
                throw ex;
            }
        }

        public bool InserirDuplicada(PedidoPecaEntity pedidoPecaEntity)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaInsertDuplicada");
                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, pedidoPecaEntity.pedido.ID_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, pedidoPecaEntity.peca.CD_PECA);
                _db.AddInParameter(dbCommand, "@p_CD_PECA_REFERENCIA", DbType.String, pedidoPecaEntity.CD_PECA_REFERENCIA);
                _db.AddInParameter(dbCommand, "@p_QTD_SOLICITADA", DbType.Decimal, pedidoPecaEntity.QTD_SOLICITADA);
                _db.AddInParameter(dbCommand, "@p_QTD_APROVADA", DbType.Decimal, pedidoPecaEntity.QTD_APROVADA);
                _db.AddInParameter(dbCommand, "@p_QTD_RECEBIDA", DbType.Decimal, pedidoPecaEntity.QTD_RECEBIDA);
                _db.AddInParameter(dbCommand, "@p_TX_APROVADO", DbType.String, pedidoPecaEntity.TX_APROVADO);

                if (pedidoPecaEntity.NR_DOC_ORI > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_NR_DOC_ORI", DbType.Int64, pedidoPecaEntity.NR_DOC_ORI);
                }

                _db.AddInParameter(dbCommand, "@p_ST_STATUS_ITEM", DbType.String, pedidoPecaEntity.ST_STATUS_ITEM);

                if (pedidoPecaEntity.estoque3M1.ID_ESTOQUE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE_DEBITO", DbType.Int64, pedidoPecaEntity.estoque3M1.ID_ESTOQUE);
                }

                if (pedidoPecaEntity.estoque3M2.ID_ESTOQUE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE_DEBITO_3M2", DbType.Int64, pedidoPecaEntity.estoque3M2.ID_ESTOQUE);
                }

                if (pedidoPecaEntity.QTD_APROVADA_3M1 > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_APROVADA_3M1", DbType.Decimal, pedidoPecaEntity.QTD_APROVADA_3M1);
                }

                if (pedidoPecaEntity.QTD_APROVADA_3M2 > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_APROVADA_3M2", DbType.Decimal, pedidoPecaEntity.QTD_APROVADA_3M2);
                }

                if (pedidoPecaEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pedidoPecaEntity.nidUsuarioAtualizacao);
                }

                _db.AddInParameter(dbCommand, "@p_VL_PECA", DbType.Decimal, pedidoPecaEntity.VL_PECA);
                _db.AddInParameter(dbCommand, "@p_TIPO_PECA", DbType.Byte, pedidoPecaEntity.TIPO_PECA);
                _db.AddInParameter(dbCommand, "@p_DESCRICAO_PECA", DbType.String, pedidoPecaEntity.DESCRICAO_PECA);
                _db.AddInParameter(dbCommand, "@p_TOKEN", DbType.Int64, pedidoPecaEntity.TOKEN);

                _db.ExecuteNonQuery(dbCommand);
                

                retorno = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;

        }

        public void Alterar(PedidoPecaEntity pedidoPecaEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaUpdate");

                if (pedidoPecaEntity.ID_ITEM_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, pedidoPecaEntity.ID_ITEM_PEDIDO);
                }

                if (pedidoPecaEntity.pedido.ID_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, pedidoPecaEntity.pedido.ID_PEDIDO);
                }

                if (!string.IsNullOrEmpty(pedidoPecaEntity.peca.CD_PECA))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, pedidoPecaEntity.peca.CD_PECA);
                }

                if (pedidoPecaEntity.QTD_SOLICITADA != null)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_SOLICITADA", DbType.Decimal, pedidoPecaEntity.QTD_SOLICITADA);
                }

                if (pedidoPecaEntity.QTD_RECEBIDA != null)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_RECEBIDA", DbType.Decimal, pedidoPecaEntity.QTD_RECEBIDA);
                }

                if (!string.IsNullOrEmpty(pedidoPecaEntity.TX_APROVADO))
                {
                    _db.AddInParameter(dbCommand, "@p_TX_APROVADO", DbType.String, pedidoPecaEntity.TX_APROVADO);
                }

                if (pedidoPecaEntity.NR_DOC_ORI > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_NR_DOC_ORI", DbType.Int64, pedidoPecaEntity.NR_DOC_ORI);
                }

                if (!string.IsNullOrEmpty(pedidoPecaEntity.ST_STATUS_ITEM))
                {
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_ITEM", DbType.String, pedidoPecaEntity.ST_STATUS_ITEM);

                    if(pedidoPecaEntity.ST_STATUS_ITEM == STATUS_ITEM_SOLICITACAO_CANCELADO)
                        pedidoPecaEntity.QTD_APROVADA = 0;
                }

                if (pedidoPecaEntity.QTD_APROVADA != null)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_APROVADA", DbType.Decimal, pedidoPecaEntity.QTD_APROVADA);
                }

                if (!string.IsNullOrEmpty(pedidoPecaEntity.DS_OBSERVACAO))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, pedidoPecaEntity.DS_OBSERVACAO);
                }

                if (!string.IsNullOrEmpty(pedidoPecaEntity.DS_DIR_FOTO))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_DIR_FOTO", DbType.String, pedidoPecaEntity.DS_DIR_FOTO);
                }

                if (pedidoPecaEntity.estoque3M1.ID_ESTOQUE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE_DEBITO", DbType.Int64, pedidoPecaEntity.estoque3M1.ID_ESTOQUE);
                }

                if (pedidoPecaEntity.estoque3M2.ID_ESTOQUE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE_DEBITO_3M2", DbType.Int64, pedidoPecaEntity.estoque3M2.ID_ESTOQUE);
                }

                if (pedidoPecaEntity.QTD_APROVADA_3M1 > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_APROVADA_3M1", DbType.Decimal, pedidoPecaEntity.QTD_APROVADA_3M1);
                }

                if (pedidoPecaEntity.pedido.TP_Especial == "Especial")
                {
                    _db.AddInParameter(dbCommand, "@p_VL_PECA", DbType.Decimal, pedidoPecaEntity.VL_PECA);
                }

                if (pedidoPecaEntity.QTD_APROVADA_3M2 > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_APROVADA_3M2", DbType.Decimal, pedidoPecaEntity.QTD_APROVADA_3M2);
                }

                if (pedidoPecaEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pedidoPecaEntity.nidUsuarioAtualizacao);
                }

                if (pedidoPecaEntity.TIPO_PECA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_TIPO_PECA", DbType.Byte, pedidoPecaEntity.TIPO_PECA);
                }

                if (! string.IsNullOrWhiteSpace(pedidoPecaEntity.DESCRICAO_PECA))
                {
                    _db.AddInParameter(dbCommand, "@p_DESCRICAO_PECA", DbType.String, pedidoPecaEntity.DESCRICAO_PECA);
                }

                if (pedidoPecaEntity.QTD_ULTIMO_RECEBIMENTO != null)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_ULTIMO_RECEBIMENTO", DbType.Decimal, pedidoPecaEntity.QTD_ULTIMO_RECEBIMENTO);
                }

                _db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Excluir(PedidoPecaEntity pedidoPecaEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaDelete");

                _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, pedidoPecaEntity.ID_ITEM_PEDIDO);

                if (pedidoPecaEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pedidoPecaEntity.nidUsuarioAtualizacao);
                }

                _db.ExecuteNonQuery(dbCommand);

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AlterarStatus(Int64 ID_PEDIDO, Int16 ST_STATUS_PEDIDO, string lote, Int64 nidUsuarioAtualizacao)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAtualizaStatusLote"); //Antigo: [prcPedidoPecaUpdateStatus]

                if (ID_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);
                }

                if (ST_STATUS_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS", DbType.Int16, ST_STATUS_PEDIDO);
                }

                if (nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, nidUsuarioAtualizacao);
                }

                if (!String.IsNullOrEmpty(lote))
                {
                    _db.AddInParameter(dbCommand, "@p_LotePecas", DbType.String, lote);
                }

                if (_transaction == null)
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand, _transaction);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AlterarQtdeAprovada(Int64 ID_PEDIDO, Int64 nidUsuarioAtualizacao)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaUpdateQtdeAprovada");

                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);

                if (nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, nidUsuarioAtualizacao);
                }

                if (_transaction == null)
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand, _transaction);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Atualizar_Estoque_Tabela_Peca(Int64? ID_ITEM_PEDIDO, string CD_PECA, string CD_TECNICO)
        {
            try
            {
                EstoquePecaData estoquePecaData = new EstoquePecaData();
                EstoqueData estoqueData = new EstoqueData();

                var estoque = estoqueData.ObterListaEstoqueSinc().FirstOrDefault(x => x.CD_TECNICO == CD_TECNICO);

                var estoquePeca = new EstoquePecaSinc();
                if(estoque != null)
                    estoquePeca = estoquePecaData.ObterListaEstoquePecaSincPorID(estoque.ID_ESTOQUE).FirstOrDefault(x => x.CD_PECA.ToUpper() == CD_PECA.ToUpper());
                if (estoquePeca != null)
                {
                    dbCommand = _db.GetStoredProcCommand("prcPedidoPecaUpdateEstTecnico");

                    _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, ID_ITEM_PEDIDO);

                    _db.AddInParameter(dbCommand, "@p_QTD_PECA", DbType.String, estoquePeca.QT_PECA_ATUAL);

                    if (_transaction == null)
                    {
                        _db.ExecuteNonQuery(dbCommand);
                    }
                    else
                    {
                        _db.ExecuteNonQuery(dbCommand, _transaction);
                    }
                }
                
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Atualizar_Estoque_Tabela_Peca_CLIENTE(Int64? ID_ITEM_PEDIDO, string CD_PECA, Int64 CD_Cliente)
        {
            try
            {
                EstoquePecaData estoquePecaData = new EstoquePecaData();
                EstoqueData estoqueData = new EstoqueData();

                var estoque = estoqueData.ObterListaEstoqueSinc().FirstOrDefault(x => x.CD_CLIENTE == CD_Cliente.ToString());
                var estoquePeca = new EstoquePecaSinc();
                if (estoque != null)
                    estoquePeca = estoquePecaData.ObterListaEstoquePecaSincPorID(estoque.ID_ESTOQUE).FirstOrDefault(x => x.CD_PECA.ToUpper() == CD_PECA.ToUpper());
                if (estoquePeca != null)
                {
                    dbCommand = _db.GetStoredProcCommand("prcPedidoPecaUpdateEstCliente");

                    _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, ID_ITEM_PEDIDO);

                    _db.AddInParameter(dbCommand, "@p_QTD_PECA", DbType.String, estoquePeca.QT_PECA_ATUAL);

                    if (_transaction == null)
                    {
                        _db.ExecuteNonQuery(dbCommand);
                    }
                    else
                    {
                        _db.ExecuteNonQuery(dbCommand, _transaction);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CancelarPedidoSemItem(Int64 ID_PEDIDO)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaCancelarSemItem");

                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);

                
                if (_transaction == null)
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand, _transaction);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AlterarQtdeSolicitada(Int64 ID_ITEM_PEDIDO, decimal QTD_SOLICITADA, Int64 nidUsuarioAtualizacao)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaUpdateQTD_SOLICITADA");

                _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, ID_ITEM_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_QTD_SOLICITADA", DbType.Decimal, QTD_SOLICITADA);

                if (nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, nidUsuarioAtualizacao);
                }

                _db.ExecuteNonQuery(dbCommand);

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ProcessarAprovacao(Int64 ID_PEDIDO, Int64 nidUsuarioAtualizacao, string lote, ref string Mensagem)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaProcessarAprovacao");

                if(ID_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);
                }

                if (!String.IsNullOrEmpty(lote))
                {
                    _db.AddInParameter(dbCommand, "@p_PecasLote", DbType.String, lote);
                }                

                if (nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, nidUsuarioAtualizacao);
                }

                _db.AddOutParameter(dbCommand, "@p_Mensagem", DbType.String, 8000);

                if (_transaction == null)
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand, _transaction);
                }


                Mensagem = _db.GetParameterValue(dbCommand, "@p_Mensagem").ToString();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ProcessarRecebimento(Int64 ID_PEDIDO, Int16 ST_STATUS_PEDIDO, string pecasLote, Int64 nidUsuarioAtualizacao)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcProcessarRecebimento");

                if (ID_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);
                }

                if (nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, nidUsuarioAtualizacao);
                }

                if (!String.IsNullOrEmpty(pecasLote))
                {
                    _db.AddInParameter(dbCommand, "@p_LotePecas", DbType.String, pecasLote);
                }

                if (_transaction == null)
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand, _transaction);
                }

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AlterarStatusRecebimento(Int64? ID_ITEM_PEDIDO, Int16 ST_STATUS_ITEM)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAtualizaStatusItemdePedido"); //Antigo: [prcPedidoPecaUpdateStatus]

                if (ID_ITEM_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, ID_ITEM_PEDIDO);
                }

                if (ST_STATUS_ITEM > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_ITEM", DbType.Int16, ST_STATUS_ITEM);
                }

                

                if (_transaction == null)
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand, _transaction);
                }

                
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AtualizarEstoqueQuantidadeRecebidaSolicitacaoPeca(Int64 ID_PEDIDO, Int64 nidUsuarioAtualizacao, ref string Mensagem)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaRecebimentoEstoque");
                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, nidUsuarioAtualizacao);
                _db.AddOutParameter(dbCommand, "@p_Mensagem", DbType.String, 8000);

                if (_transaction == null)
                    _db.ExecuteNonQuery(dbCommand);
                else
                    _db.ExecuteNonQuery(dbCommand, _transaction);

                Mensagem = _db.GetParameterValue(dbCommand, "@p_Mensagem").ToString();
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AtualizarEstoqueClientePeca(Int64 id_estoquePeca, decimal qtd_atual)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaRecebimentoEstoqueCliente");
                _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE_PECA", DbType.Int64, id_estoquePeca);
                _db.AddInParameter(dbCommand, "@p_QT_PECA_ATUAL", DbType.Decimal, qtd_atual);

                if (_transaction == null)
                    _db.ExecuteNonQuery(dbCommand);
                else
                    _db.ExecuteNonQuery(dbCommand, _transaction);

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InserirEstoquePecaCliente(decimal qtd_atual, string CD_PECA, decimal QT_PECA_MIN, Int64 id_estoque)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAdicionarPedidoPecaClienteAtt");
                _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE", DbType.Int64, id_estoque);
                _db.AddInParameter(dbCommand, "@p_QT_PECA_ATUAL", DbType.Decimal, qtd_atual);
                _db.AddInParameter(dbCommand, "@p_QT_PECA_MIN", DbType.Decimal, QT_PECA_MIN);
                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, CD_PECA);
                _db.AddInParameter(dbCommand, "@p_DT_ULT_MIN", DbType.DateTime, DateTime.Now);

                if (_transaction == null)
                    _db.ExecuteNonQuery(dbCommand);
                else
                    _db.ExecuteNonQuery(dbCommand, _transaction);

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ObterLista(PedidoPecaEntity pedidoPecaEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaSelect");

                if (pedidoPecaEntity.ID_ITEM_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, pedidoPecaEntity.ID_ITEM_PEDIDO);
                }

                if (pedidoPecaEntity.pedido.ID_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, pedidoPecaEntity.pedido.ID_PEDIDO);
                }

                if (!string.IsNullOrEmpty(pedidoPecaEntity.peca.CD_PECA))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, pedidoPecaEntity.peca.CD_PECA);
                }

                if (pedidoPecaEntity.QTD_SOLICITADA != null)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_SOLICITADA", DbType.Decimal, pedidoPecaEntity.QTD_SOLICITADA);
                }

                if (pedidoPecaEntity.QTD_APROVADA != null)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_APROVADA", DbType.Decimal, pedidoPecaEntity.QTD_APROVADA);
                }

                if (pedidoPecaEntity.QTD_RECEBIDA != null)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_RECEBIDA", DbType.Decimal, pedidoPecaEntity.QTD_RECEBIDA);
                }

                if (!string.IsNullOrEmpty(pedidoPecaEntity.TX_APROVADO))
                {
                    _db.AddInParameter(dbCommand, "@p_TX_APROVADO", DbType.String, pedidoPecaEntity.TX_APROVADO);
                }

                if (pedidoPecaEntity.NR_DOC_ORI > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_NR_DOC_ORI", DbType.Int64, pedidoPecaEntity.NR_DOC_ORI);
                }

                if (!string.IsNullOrEmpty(pedidoPecaEntity.ST_STATUS_ITEM))
                {
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_ITEM", DbType.String, pedidoPecaEntity.ST_STATUS_ITEM);
                }

                if (!string.IsNullOrEmpty(pedidoPecaEntity.DS_OBSERVACAO))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, pedidoPecaEntity.DS_OBSERVACAO);
                }

                if (!string.IsNullOrEmpty(pedidoPecaEntity.DS_DIR_FOTO))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_DIR_FOTO", DbType.String, pedidoPecaEntity.DS_DIR_FOTO);
                }

                if (pedidoPecaEntity.estoque3M1.ID_ESTOQUE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE_DEBITO", DbType.Int64, pedidoPecaEntity.estoque3M1.ID_ESTOQUE);
                }

                if (pedidoPecaEntity.estoque3M2.ID_ESTOQUE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE_DEBITO_3M2", DbType.Int64, pedidoPecaEntity.estoque3M2.ID_ESTOQUE);
                }

                if (pedidoPecaEntity.QTD_APROVADA_3M1 > 0)
                    _db.AddInParameter(dbCommand, "@p_QTD_APROVADA_3M1", DbType.Int64, pedidoPecaEntity.QTD_APROVADA_3M1);

                if (pedidoPecaEntity.QTD_APROVADA_3M2 > 0)
                    _db.AddInParameter(dbCommand, "@p_QTD_APROVADA_3M2", DbType.Int64, pedidoPecaEntity.QTD_APROVADA_3M2);

                if (pedidoPecaEntity.VL_PECA > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_PECA", DbType.Decimal, pedidoPecaEntity.VL_PECA);

                if (pedidoPecaEntity.ID_LOTE_APROVACAO > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_LOTE_APROVACAO", DbType.Int64, pedidoPecaEntity.ID_LOTE_APROVACAO);

                if (pedidoPecaEntity.TIPO_PECA > 0)
                    _db.AddInParameter(dbCommand, "@p_TIPO_PECA", DbType.Byte, pedidoPecaEntity.TIPO_PECA);

                if (_transaction == null)
                {
                    connection = _db.CreateConnection();
                    dbCommand.Connection = connection;
                    connection.Open();

                    dataSet = _db.ExecuteDataSet(dbCommand);
                }
                else
                {
                    dataSet = _db.ExecuteDataSet(dbCommand, _transaction);
                }

                dataTable = dataSet.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (!(connection is null))
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
                
            }
            return dataTable;
        }

        public DataTable BuscaPecaDuplicidadePedido(Int64 ID_PEDIDO, Int64 ID_ITEM_PEDIDO, string CD_PECA)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaSelectDuplicidadePedido");

                _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, ID_ITEM_PEDIDO);

                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);

                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, CD_PECA);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];
                //connection.Close();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public void AprovarPedidoSemItem(Int64? ID_ITEM_PEDIDO)
        {
            try
            {
                IList<PedidoPecaSinc> listaPedidoPeca = new List<PedidoPecaSinc>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"update TB_PEDIDO_PECA
                                    set ST_STATUS_ITEM = '3'
                                    where ID_ITEM_PEDIDO = @ID_ITEM_PEDIDO";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_ITEM_PEDIDO", SqlDbType.BigInt).Value = ID_ITEM_PEDIDO;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        

                        cnx.Close();
                        
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AtualizarEnvioBPCSPECA(Int64? ID_PEDIDO)
        {
            try
            {
                IList<PedidoPecaSinc> listaPedidoPeca = new List<PedidoPecaSinc>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"update TB_PEDIDO_PECA
                                    set ST_STATUS_ITEM = '9'
                                    where ID_PEDIDO = @ID_PEDIDO
                                    AND ST_STATUS_ITEM = '8'";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_PEDIDO", SqlDbType.BigInt).Value = ID_PEDIDO;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();



                        cnx.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AtualizarEnvioBPCSPECAAprovado0(Int64? ID_PEDIDO)
        {
            try
            {
                IList<PedidoPecaSinc> listaPedidoPeca = new List<PedidoPecaSinc>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"update TB_PEDIDO_PECA
                                    set ST_STATUS_ITEM = '3'
                                    where ID_PEDIDO = @ID_PEDIDO
                                    AND ST_STATUS_ITEM not in ('4')
                                    AND QTD_APROVADA_3M1 is null
                                    AND CD_PECA_REFERENCIA is null";
                                
                                    

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_PEDIDO", SqlDbType.BigInt).Value = ID_PEDIDO;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();



                        cnx.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DuplicarItem(Int64? ID_ITEM_PEDIDO)
        {
            
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaDuplicar");


                _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, ID_ITEM_PEDIDO);
                    
                if (_transaction == null)
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand, _transaction);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public void AtualizarEnvioRemessaBPCSPECA(Int64? ID_ITEM_PEDIDO)
        {
            try
            {
                IList<PedidoPecaSinc> listaPedidoPeca = new List<PedidoPecaSinc>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"update TB_PEDIDO_PECA
                                    set InformadoDados = 'S'
                                    where ID_ITEM_PEDIDO = @ID_ITEM_PEDIDO
                                    AND ST_STATUS_ITEM = '8'";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_ITEM_PEDIDO", SqlDbType.BigInt).Value = ID_ITEM_PEDIDO;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();



                        cnx.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AtualizarEnvioBPCS(Int64? ID_PEDIDO)
        {
            try
            {
                IList<PedidoPecaSinc> listaPedidoPeca = new List<PedidoPecaSinc>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"update TB_PEDIDO
                                    set ID_STATUS_PEDIDO = 9
                                    where ID_PEDIDO = @ID_PEDIDO
                                    AND ID_STATUS_PEDIDO = 8";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_PEDIDO", SqlDbType.BigInt).Value = ID_PEDIDO;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();



                        cnx.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AtualizarRecebimentoPedido(Int64? ID_ITEM_PEDIDO)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAtualizarRecebimentoPedidoDePeca");
                _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, ID_ITEM_PEDIDO);
                
                if (_transaction == null)
                    _db.ExecuteNonQuery(dbCommand);
                else
                    _db.ExecuteNonQuery(dbCommand, _transaction);

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AtualizarRecebimentoPedidoPendente(Int64? ID_ITEM_PEDIDO)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAtualizarRecebimentoPedidoDePecaPendente");
                _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, ID_ITEM_PEDIDO);

                if (_transaction == null)
                    _db.ExecuteNonQuery(dbCommand);
                else
                    _db.ExecuteNonQuery(dbCommand, _transaction);

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AprovarPedidoCliente(Int64? ID_ITEM_PEDIDO, Int64? QTD_RECEBIDA)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("AprovarPedidoCliente");


                _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, ID_ITEM_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_QTD_RECEBIDA", DbType.Int64, QTD_RECEBIDA);

                if (_transaction == null)
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand, _transaction);
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AtualizarStatusPedidoCliente(Int64? ID_ITEM_PEDIDO)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAttStatusPedidoCliente");


                _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, ID_ITEM_PEDIDO);
                

                if (_transaction == null)
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand, _transaction);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AprovarQTDPedidoPECA(Int64? ID_ITEM_PEDIDO, Int64 QTD_APROVADA, Int64? Qtd_3M1 = null, Int64? Qtd_3M2 = null)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("AprovarQTDPedidoPECA");


                _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, ID_ITEM_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_QTD_APROVADA", DbType.Int64, QTD_APROVADA);
                if (Qtd_3M1 != null && Qtd_3M1 > 0)
                    _db.AddInParameter(dbCommand, "@p_QTD_3M1", DbType.Int64, Qtd_3M1);
                if (Qtd_3M2 != null && Qtd_3M2 > 0)
                    _db.AddInParameter(dbCommand, "@p_QTD_3M2", DbType.Int64, Qtd_3M2);

                if (_transaction == null)
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand, _transaction);
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AprovarLote(Int64 ID_PEDIDO, String pecasLote, String pecasLoteAP, ref string Mensagem)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAprovaLoteDebitar");


                if (ID_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);
                }

                if (!String.IsNullOrEmpty(pecasLote))
                {
                    _db.AddInParameter(dbCommand, "@p_PecasLote", DbType.String, pecasLote);
                }

                if (!String.IsNullOrEmpty(pecasLote))
                {
                    _db.AddInParameter(dbCommand, "@p_PecasLoteAP", DbType.String, pecasLoteAP);
                }
                

                _db.AddOutParameter(dbCommand, "@p_Mensagem", DbType.String, 8000);


                if (_transaction == null)
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand, _transaction);
                }

                Mensagem = _db.GetParameterValue(dbCommand, "@p_Mensagem").ToString();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CriarLoteCliente(Int64 ID_PEDIDO, String pecasLote, String pecasLoteAP, ref string Mensagem)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcCriaLoteCliente");


                if (ID_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);
                }

                if (!String.IsNullOrEmpty(pecasLote))
                {
                    _db.AddInParameter(dbCommand, "@p_PecasLote", DbType.String, pecasLote);
                }

                if (!String.IsNullOrEmpty(pecasLote))
                {
                    _db.AddInParameter(dbCommand, "@p_PecasLoteAP", DbType.String, pecasLoteAP);
                }


                _db.AddOutParameter(dbCommand, "@p_Mensagem", DbType.String, 8000);


                if (_transaction == null)
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand, _transaction);
                }

                Mensagem = _db.GetParameterValue(dbCommand, "@p_Mensagem").ToString();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AtualizaStatusPedido(Int64 ID_PEDIDO)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAtualizaStatusPedido");

                if (ID_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);
                }

                if (_transaction == null)
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand, _transaction);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DadosPedidoEntity> ObterListaDadosPedido(Int64 id_Pedido_item)
        {
            try
            {
                List<DadosPedidoEntity> listaPedidoPeca = new List<DadosPedidoEntity>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"select * 
                                  from tb_pedido_ap_dados  
                                 where ID_ITEM_PEDIDO = @ID_ITEM_PEDIDO ";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_ITEM_PEDIDO", SqlDbType.BigInt).Value = id_Pedido_item;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            DadosPedidoEntity dados = new DadosPedidoEntity
                            {
                                ID_ITEM_PEDIDO = Convert.ToInt64(SDR["ID_ITEM_PEDIDO"].ToString()),
                                CD_PECA = Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString()),
                                QTD_SOLICITADA = Convert.ToInt64(SDR["QTD_SOLICITADA"] is DBNull ? 0 : SDR["QTD_SOLICITADA"]),
                                QTD_APROVADA = Convert.ToInt64(SDR["QTD_APROVADA"] is DBNull ? 0 : SDR["QTD_APROVADA"]),
                                ID_ESTOQUE_DEBITO = Convert.ToInt32(SDR["ID_ESTOQUE_DEBITO"] is DBNull ? null : SDR["ID_ESTOQUE_DEBITO"]),
                                QTD_APROVADA_3M1 = Convert.ToInt64(SDR["QTD_APROVADA_3M1"] is DBNull ? 0 : SDR["QTD_APROVADA_3M1"]),
                                QTD_APROVADA_3M2 = Convert.ToInt64(SDR["QTD_APROVADA_3M2"] is DBNull ? 0 : SDR["QTD_APROVADA_3M2"])
                                
                            };

                            //TODO: Edgar - O código abaixo é provisório, e deve ser removido quando implementado os novos ajustes de solicitações de peça.
                            
                            listaPedidoPeca.Add(dados);
                        }

                        cnx.Close();
                        return listaPedidoPeca;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DadosPedidoEntity> ObterListaDadosPedidoTotal(Int64 id_Pedido)
        {
            try
            {
                List<DadosPedidoEntity> listaPedidoPeca = new List<DadosPedidoEntity>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"select * 
                                  from tb_pedido_ap_dados  
                                 where ID_PEDIDO = @ID_PEDIDO ";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_PEDIDO", SqlDbType.BigInt).Value = id_Pedido;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            DadosPedidoEntity dados = new DadosPedidoEntity
                            {
                                ID_ITEM_PEDIDO = Convert.ToInt64(SDR["ID_ITEM_PEDIDO"].ToString()),
                                CD_PECA = Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString()),
                                QTD_SOLICITADA = Convert.ToInt64(SDR["QTD_SOLICITADA"] is DBNull ? 0 : SDR["QTD_SOLICITADA"]),
                                QTD_APROVADA = Convert.ToInt64(SDR["QTD_APROVADA"] is DBNull ? 0 : SDR["QTD_APROVADA"]),
                                ID_ESTOQUE_DEBITO = Convert.ToInt32(SDR["ID_ESTOQUE_DEBITO"] is DBNull ? null : SDR["ID_ESTOQUE_DEBITO"]),
                                QTD_APROVADA_3M1 = Convert.ToInt64(SDR["QTD_APROVADA_3M1"] is DBNull ? 0 : SDR["QTD_APROVADA_3M1"]),
                                QTD_APROVADA_3M2 = Convert.ToInt64(SDR["QTD_APROVADA_3M2"] is DBNull ? 0 : SDR["QTD_APROVADA_3M2"]),
                                ID_PEDIDO = Convert.ToInt64(SDR["ID_PEDIDO"].ToString()),
                                NR_REMESSA = Convert.ToInt64(SDR["NR_REMESSA"].ToString())
                            };

                            //TODO: Edgar - O código abaixo é provisório, e deve ser removido quando implementado os novos ajustes de solicitações de peça.

                            listaPedidoPeca.Add(dados);
                        }

                        cnx.Close();
                        return listaPedidoPeca;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool InserirDadosPedido(ref DadosPedidoEntity dadosEntity)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaInsertDados");

                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, dadosEntity.ID_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, dadosEntity.CD_PECA);
                _db.AddInParameter(dbCommand, "@p_QTD_SOLICITADA", DbType.Decimal, dadosEntity.QTD_SOLICITADA);
                _db.AddInParameter(dbCommand, "@p_QTD_APROVADA", DbType.Decimal, dadosEntity.QTD_APROVADA);

                _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE_DEBITO", DbType.Int64, dadosEntity.ID_ESTOQUE_DEBITO);
                _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE_DEBITO_3M2", DbType.Int64, dadosEntity.ID_ESTOQUE_DEBITO_3M2);
                _db.AddInParameter(dbCommand, "@p_QTD_APROVADA_3M1", DbType.Decimal, dadosEntity.QTD_APROVADA_3M1);
                _db.AddInParameter(dbCommand, "@p_QTD_APROVADA_3M2", DbType.Decimal, dadosEntity.QTD_APROVADA_3M2);

                _db.AddInParameter(dbCommand, "@p_VOLUME", DbType.Decimal, dadosEntity.VOLUME);
                _db.AddInParameter(dbCommand, "@p_RAMAL", DbType.Decimal, dadosEntity.RAMAL);
                _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Decimal, dadosEntity.ID_ITEM_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_PESOLIQUIDO", DbType.Decimal, dadosEntity.PesoLiquido);
                _db.AddInParameter(dbCommand, "@p_PESOBRUTO", DbType.Decimal, dadosEntity.PesoBruto);
                _db.AddInParameter(dbCommand, "@p_TELEFONE", DbType.String, dadosEntity.DS_TELEFONE);
                _db.AddInParameter(dbCommand, "@p_DS_APROVADOR", DbType.String, dadosEntity.DS_APROVADOR);
                _db.AddInParameter(dbCommand, "@p_DS_CLIENTE", DbType.String, dadosEntity.RESP_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_NR_REMESSA", DbType.Decimal, dadosEntity.NR_REMESSA);

                _db.ExecuteNonQuery(dbCommand);

                
                retorno = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;

        }

    }
}
