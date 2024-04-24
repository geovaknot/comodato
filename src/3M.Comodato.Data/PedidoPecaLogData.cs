using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace _3M.Comodato.Data
{
    public class PedidoPecaLogData
    {
        private readonly Database _db;
        private DbConnection _connection;
        private DbCommand dbCommand;
        internal DbTransaction _transaction;

        public PedidoPecaLogData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }
        public PedidoPecaLogData(TransactionData transactionData)
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
        public IList<PedidoPecaLogSinc> ObterListaPedidoPecaLogSinc(Int64 id_item_pedido)
        {
            try
            {
                IList<PedidoPecaLogSinc> listaPedidoPecaLog = new List<PedidoPecaLogSinc>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"select * 
                                  from TB_PEDIDO_PECA_LOG  
                                 where ID_ITEM_PEDIDO = @ID_ITEM_PEDIDO";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_ITEM_PEDIDO", SqlDbType.BigInt).Value = id_item_pedido;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            PedidoPecaLogSinc pedidoPecaLog = new PedidoPecaLogSinc
                            {
                                ID_ITEM_PEDIDO_LOG = Convert.ToInt64(SDR["ID_ITEM_PEDIDO_LOG"].ToString()),
                                ID_ITEM_PEDIDO = Convert.ToInt64(SDR["ID_ITEM_PEDIDO"].ToString()),
                                DATA_RECEBIMENTO = Convert.ToDateTime(SDR["DATA_RECEBIMENTO"] is DBNull ? "01/01/2000" : SDR["DATA_RECEBIMENTO"]),
                                QTD_PECA_RECEBIDA = Convert.ToInt64(SDR["QTD_PECA_RECEBIDA"] is DBNull ? 0 : SDR["QTD_PECA_RECEBIDA"])
                            };

                            listaPedidoPecaLog.Add(pedidoPecaLog);
                        }

                        cnx.Close();
                        return listaPedidoPecaLog;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List <PedidoPecaLogSinc> ObterLista(Int64? ID_ITEM_PEDIDO)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            List<PedidoPecaLogSinc> listaPedidoPecaLog = new List<PedidoPecaLogSinc>();
            SqlDataReader SDR = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_db.ConnectionString))
                {
                    string cmdText = @"exec prcPedidoPecaLogSelect @p_ID_ITEM_PEDIDO";
                    SqlCommand sqlComm = new SqlCommand(cmdText, conn);

                    sqlComm.Parameters.Add("@p_ID_ITEM_PEDIDO", SqlDbType.BigInt);
                    sqlComm.Parameters["@p_ID_ITEM_PEDIDO"].Value = ID_ITEM_PEDIDO;

                    //sqlComm.CommandType = CommandType.StoredProcedure;
                    sqlComm.Connection = conn;
                    conn.Open();


                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = sqlComm;

                    da.SelectCommand.CommandTimeout = 1800;
                    SDR = sqlComm.ExecuteReader();

                    while (SDR.Read())
                    {
                        PedidoPecaLogSinc pedidoPecaLog = new PedidoPecaLogSinc
                        {
                            ID_ITEM_PEDIDO_LOG = Convert.ToInt64(SDR["ID_ITEM_PEDIDO_LOG"].ToString()),
                            ID_ITEM_PEDIDO = Convert.ToInt64(SDR["ID_ITEM_PEDIDO"].ToString()),
                            DATA_RECEBIMENTO = Convert.ToDateTime(SDR["DATA_RECEBIMENTO"] is DBNull ? "01/01/2000" : SDR["DATA_RECEBIMENTO"]),
                            QTD_PECA_RECEBIDA = Convert.ToInt64(SDR["QTD_PECA_RECEBIDA"] is DBNull ? 0 : SDR["QTD_PECA_RECEBIDA"])
                            
                        };

                        listaPedidoPecaLog.Add(pedidoPecaLog);
                    }


                }
                

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != connection)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            return listaPedidoPecaLog;
        }

        public IList<PedidoPecaLogSinc> ObterListaPedidoPecaLogPorId(Int64 idUsuario)
        {
            try
            {
                IList<PedidoPecaLogSinc> listaPedidoPecaLog = new List<PedidoPecaLogSinc>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"select ppl.* 
	                            from tb_pedido_peca_log ppl 
		                            inner join tb_pedido_peca pp 
			                            on pp.ID_ITEM_PEDIDO = ppl.ID_ITEM_PEDIDO 
		                            inner join tb_pedido ped 
			                            on ped.ID_PEDIDO = pp.ID_PEDIDO 
		                            inner join tb_tecnico t 
			                            on t.cd_tecnico = ped.cd_tecnico 
	                            where ped.ID_PEDIDO IN(select p.ID_PEDIDO 
                                                    from TB_PEDIDO p
                                                    where ((p.ID_STATUS_PEDIDO NOT IN(4, 7) AND CAST(p.DT_CRIACAO AS DATE) >= CAST(DATEADD(MONTH, -6, GETDATE()) AS DATE)) --Não seja Recebido ou Cancelado
      				                                        OR (p.ID_STATUS_PEDIDO IN(4, 7) AND CAST(p.DT_CRIACAO AS DATE) >= CAST(DATEADD(day, -30, GETDATE()) AS DATE))) --Somente Recebido ou Cancelado
						                            and p.CD_TECNICO = ped.CD_TECNICO 
					                            ) 
	                            and (t.ID_USUARIO = @ID_USUARIO OR t.ID_USUARIO_COORDENADOR = @ID_USUARIO)";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", SqlDbType.BigInt).Value = idUsuario;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            PedidoPecaLogSinc pedidoPecaLog = new PedidoPecaLogSinc
                            {
                                ID_ITEM_PEDIDO_LOG = Convert.ToInt64(SDR["ID_ITEM_PEDIDO_LOG"].ToString()),
                                ID_ITEM_PEDIDO = Convert.ToInt64(SDR["ID_ITEM_PEDIDO"].ToString()),
                                DATA_RECEBIMENTO = Convert.ToDateTime(SDR["DATA_RECEBIMENTO"] is DBNull ? "01/01/2000" : SDR["DATA_RECEBIMENTO"]),
                                QTD_PECA_RECEBIDA = Convert.ToInt64(SDR["QTD_PECA_RECEBIDA"] is DBNull ? 0 : SDR["QTD_PECA_RECEBIDA"])
                            };
                            
                            listaPedidoPecaLog.Add(pedidoPecaLog);
                        }

                        cnx.Close();
                        return listaPedidoPecaLog;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Inserir(ref PedidoPecaLogEntity pedidoPecaLogEntity)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoPecaLogInsert");

                _db.AddInParameter(dbCommand, "@p_ID_ITEM_PEDIDO", DbType.Int64, pedidoPecaLogEntity.ID_ITEM_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_QTD_PECA_RECEBIDA", DbType.Int64, pedidoPecaLogEntity.QTD_PECA_RECEBIDA);
                _db.AddInParameter(dbCommand, "@p_DATA_RECEBIMENTO", DbType.DateTime, pedidoPecaLogEntity.DATA_RECEBIMENTO);
                

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
