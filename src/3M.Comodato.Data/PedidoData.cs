using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace _3M.Comodato.Data
{
    public class PedidoData
    {
        private readonly Database _db;
        private DbConnection _connection;
        private DbCommand dbCommand;
        internal DbTransaction _transaction;

        public PedidoData(TransactionData transactionData)
        {
            _db = transactionData._db;
            _connection = transactionData._connection;
            _transaction = transactionData._transaction;
        }

        public PedidoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }


        /// <summary>
        /// Obtem lista para o sincronismo Mobile, traz todos os pedidos do usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public IList<PedidoSinc> ObterListaPedidoSinc(Int64 idUsuario)
        {
            try
            {
                IList<PedidoSinc> listaPedido = new List<PedidoSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         @"SELECT ped.* 
                                             FROM tb_pedido ped
                                            INNER JOIN tb_tecnico tec 
                                               ON tec.cd_tecnico = ped.cd_tecnico 
                                            WHERE (tec.ID_USUARIO = @ID_USUARIO OR tec.ID_USUARIO_COORDENADOR = @ID_USUARIO) 
                                              AND ped.ID_PEDIDO IN(SELECT p.ID_PEDIDO 
                                                                     FROM TB_PEDIDO p
                                                                    WHERE ((p.ID_STATUS_PEDIDO NOT IN(4, 7) AND CAST(p.DT_CRIACAO AS DATE) >= CAST(DATEADD(MONTH, -6, GETDATE()) AS DATE)) --Não seja Recebido ou Cancelado
      				                                                        OR (p.ID_STATUS_PEDIDO IN(4, 7) AND CAST(p.DT_CRIACAO AS DATE) >= CAST(DATEADD(day, -30, GETDATE()) AS DATE))) --Somente Recebido ou Cancelado
                                                                      AND p.CD_TECNICO = ped.CD_TECNICO
						                                          )";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", SqlDbType.BigInt).Value = idUsuario;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            PedidoSinc pedido = new PedidoSinc
                            {
                                ID_PEDIDO = Convert.ToInt64(SDR["ID_PEDIDO"].ToString()),
                                CD_TECNICO = Convert.ToString(SDR["CD_TECNICO"] is DBNull ? "" : SDR["CD_TECNICO"].ToString()),
                                NR_DOCUMENTO = Convert.ToInt32(SDR["NR_DOCUMENTO"] is DBNull ? 0 : SDR["NR_DOCUMENTO"]),
                                DT_CRIACAO = Convert.ToDateTime(SDR["DT_CRIACAO"] is DBNull ? "01/01/2000" : SDR["DT_CRIACAO"]),
                                DT_ENVIO = Convert.ToDateTime(SDR["DT_ENVIO"] is DBNull ? "01/01/2000" : SDR["DT_ENVIO"]),
                                DT_RECEBIMENTO = Convert.ToDateTime(SDR["DT_RECEBIMENTO"] is DBNull ? "01/01/2000" : SDR["DT_RECEBIMENTO"]),
                                TX_OBS = Convert.ToString(SDR["TX_OBS"] is DBNull ? "" : SDR["TX_OBS"].ToString()),
                                PENDENTE = Convert.ToString(SDR["PENDENTE"] is DBNull ? "" : SDR["PENDENTE"].ToString()),
                                NR_DOC_ORI = Convert.ToInt32(SDR["NR_DOC_ORI"] is DBNull ? 0 : SDR["NR_DOC_ORI"]),
                                ID_STATUS_PEDIDO = Convert.ToInt32(SDR["ID_STATUS_PEDIDO"] is DBNull ? 0 : SDR["ID_STATUS_PEDIDO"]),
                                TP_TIPO_PEDIDO = Convert.ToChar(SDR["TP_TIPO_PEDIDO"] is DBNull ? " " : SDR["TP_TIPO_PEDIDO"].ToString()),
                                CD_CLIENTE = Convert.ToInt64(SDR["CD_CLIENTE"] is DBNull ? null : SDR["CD_CLIENTE"].ToString()),
                                CD_PEDIDO = Convert.ToInt64(SDR["CD_PEDIDO"] is DBNull ? null : SDR["CD_PEDIDO"].ToString()),
                                TOKEN = Convert.ToInt64(SDR["TOKEN"].ToString())
                            };

                            listaPedido.Add(pedido);
                        }
                        cnx.Close();
                        return listaPedido;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Inserir(PedidoEntity pedidoEntity)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPedidoInsert");

                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, pedidoEntity.tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_NR_DOCUMENTO", DbType.Int64, pedidoEntity.NR_DOCUMENTO);
                _db.AddInParameter(dbCommand, "@p_DT_CRIACAO", DbType.DateTime, pedidoEntity.DT_CRIACAO);
                _db.AddInParameter(dbCommand, "@p_DT_ENVIO", DbType.DateTime, pedidoEntity.DT_ENVIO);
                _db.AddInParameter(dbCommand, "@p_DT_RECEBIMENTO", DbType.DateTime, pedidoEntity.DT_RECEBIMENTO);
                _db.AddInParameter(dbCommand, "@p_TX_OBS", DbType.String, pedidoEntity.TX_OBS);
                _db.AddInParameter(dbCommand, "@p_PENDENTE", DbType.String, pedidoEntity.PENDENTE);
                _db.AddInParameter(dbCommand, "@p_NR_DOC_ORI", DbType.Int64, pedidoEntity.NR_DOC_ORI);
                _db.AddInParameter(dbCommand, "@p_ID_STATUS_PEDIDO", DbType.Int64, pedidoEntity.statusPedido.ID_STATUS_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_TP_TIPO_PEDIDO", DbType.String, pedidoEntity.TP_TIPO_PEDIDO);

                if (pedidoEntity.cliente.CD_CLIENTE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, pedidoEntity.cliente.CD_CLIENTE);
                }

                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pedidoEntity.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_TOKEN", DbType.Int64, pedidoEntity.TOKEN);
                _db.AddInParameter(dbCommand, "@p_TP_Especial", DbType.String, pedidoEntity.TP_Especial);
                _db.AddInParameter(dbCommand, "@p_Responsavel", DbType.String, pedidoEntity.Responsavel);
                _db.AddInParameter(dbCommand, "@p_Telefone", DbType.String, pedidoEntity.Telefone);
                _db.AddInParameter(dbCommand, "@p_Origem", DbType.String, pedidoEntity.Origem);

                if (pedidoEntity.Origem == "A")
                {
                    _db.AddInParameter(dbCommand, "@p_FL_EMERGENCIA", DbType.String, "S");
                }

                _db.AddOutParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, 18);
                _db.AddOutParameter(dbCommand, "@p_TOKEN_GERADO", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                pedidoEntity.ID_PEDIDO = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_PEDIDO"));
                pedidoEntity.TOKEN = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_TOKEN_GERADO"));

                retorno = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        public void Alterar(PedidoEntity pedidoEntity)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoUpdate");

                if (pedidoEntity.ID_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, pedidoEntity.ID_PEDIDO);
                }

                if (!string.IsNullOrEmpty(pedidoEntity.tecnico.CD_TECNICO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, pedidoEntity.tecnico.CD_TECNICO);
                }

                if (pedidoEntity.NR_DOCUMENTO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_NR_DOCUMENTO", DbType.Int64, pedidoEntity.NR_DOCUMENTO);
                }

                if (pedidoEntity.DT_CRIACAO != null)
                {
                    _db.AddInParameter(dbCommand, "@p_DT_CRIACAO", DbType.DateTime, pedidoEntity.DT_CRIACAO);
                }

                if (pedidoEntity.DT_ENVIO != null)
                {
                    _db.AddInParameter(dbCommand, "@p_DT_ENVIO", DbType.DateTime, pedidoEntity.DT_ENVIO);
                }

                if (pedidoEntity.DT_RECEBIMENTO != null)
                {
                    _db.AddInParameter(dbCommand, "@p_DT_RECEBIMENTO", DbType.DateTime, pedidoEntity.DT_RECEBIMENTO);
                }

                if (!string.IsNullOrEmpty(pedidoEntity.TX_OBS))
                {
                    _db.AddInParameter(dbCommand, "@p_TX_OBS", DbType.String, pedidoEntity.TX_OBS);
                }

                if (!string.IsNullOrEmpty(pedidoEntity.PENDENTE))
                {
                    _db.AddInParameter(dbCommand, "@p_PENDENTE", DbType.String, pedidoEntity.PENDENTE);
                }

                if (pedidoEntity.NR_DOC_ORI > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_NR_DOC_ORI", DbType.Int64, pedidoEntity.NR_DOC_ORI);
                }

                if (pedidoEntity.statusPedido.ID_STATUS_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_STATUS_PEDIDO", DbType.Int64, pedidoEntity.statusPedido.ID_STATUS_PEDIDO);
                }

                if (pedidoEntity.cliente.CD_CLIENTE != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, pedidoEntity.cliente.CD_CLIENTE);
                }

                if (pedidoEntity.CD_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_PEDIDO", DbType.Int64, pedidoEntity.CD_PEDIDO);
                }

                if (!string.IsNullOrEmpty(pedidoEntity.FL_EMERGENCIA))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_EMERGENCIA", DbType.String, pedidoEntity.FL_EMERGENCIA);
                }

                if (!string.IsNullOrEmpty(pedidoEntity.Responsavel))
                {
                    _db.AddInParameter(dbCommand, "@p_Responsavel", DbType.String, pedidoEntity.Responsavel);
                }

                if (!string.IsNullOrEmpty(pedidoEntity.Telefone))
                {
                    _db.AddInParameter(dbCommand, "@p_Telefone", DbType.String, pedidoEntity.Telefone);
                }
                if (!string.IsNullOrEmpty(pedidoEntity.EnviaBPCS))
                {
                    _db.AddInParameter(dbCommand, "@p_EnviaBPCS", DbType.String, pedidoEntity.EnviaBPCS);
                }
                //if (!string.IsNullOrEmpty(pedidoEntity.pecasLote))
                //{
                //    _db.AddInParameter(dbCommand, "@p_PecasLote", DbType.String, pedidoEntity.pecasLote);
                //}


                if (pedidoEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pedidoEntity.nidUsuarioAtualizacao);
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

        public void AtualizarEnvioBPCS(PedidoEntity pedidoEntity)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoUpdateBPCS");

                if (pedidoEntity.ID_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, pedidoEntity.ID_PEDIDO);
                }
                
                if (!string.IsNullOrEmpty(pedidoEntity.Responsavel))
                {
                    _db.AddInParameter(dbCommand, "@p_Responsavel", DbType.String, pedidoEntity.Responsavel);
                }

                if (!string.IsNullOrEmpty(pedidoEntity.Telefone))
                {
                    _db.AddInParameter(dbCommand, "@p_Telefone", DbType.String, pedidoEntity.Telefone);
                }
                if (!string.IsNullOrEmpty(pedidoEntity.EnviaBPCS))
                {
                    _db.AddInParameter(dbCommand, "@p_EnviaBPCS", DbType.String, pedidoEntity.EnviaBPCS);
                }
                //if (!string.IsNullOrEmpty(pedidoEntity.pecasLote))
                //{
                //    _db.AddInParameter(dbCommand, "@p_PecasLote", DbType.String, pedidoEntity.pecasLote);
                //}


                if (pedidoEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pedidoEntity.nidUsuarioAtualizacao);
                }

                if (pedidoEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, pedidoEntity.nidUsuarioAtualizacao);
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

        public void Excluir(PedidoEntity pedidoEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPedidoDelete");

                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, pedidoEntity.ID_PEDIDO);

                if (pedidoEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pedidoEntity.nidUsuarioAtualizacao);
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

        public DataTable ObterListaSolicitacao(PedidoEntity pedidoEntity, Int64? ID_STATUS_PEDIDO_ADICIONAL, DateTime? DT_CRIACAO_INICIO, DateTime? DT_CRIACAO_FIM)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoSelectSolicitacao");

                if (pedidoEntity.cliente.CD_CLIENTE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, pedidoEntity.cliente.CD_CLIENTE);
                }

                if (!string.IsNullOrEmpty(pedidoEntity.tecnico.CD_TECNICO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, pedidoEntity.tecnico.CD_TECNICO);
                }

                if (pedidoEntity.CD_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_PEDIDO", DbType.Int64, pedidoEntity.CD_PEDIDO);
                }

                if (DT_CRIACAO_INICIO != null)
                {
                    _db.AddInParameter(dbCommand, "@p_DT_CRIACAO_INICIO", DbType.DateTime, DT_CRIACAO_INICIO);
                }

                if (DT_CRIACAO_FIM != null)
                {
                    _db.AddInParameter(dbCommand, "@p_DT_CRIACAO_FIM", DbType.DateTime, DT_CRIACAO_FIM);
                }

                if (pedidoEntity.statusPedido.ID_STATUS_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_STATUS_PEDIDO", DbType.Int64, pedidoEntity.statusPedido.ID_STATUS_PEDIDO);
                }

                _db.AddInParameter(dbCommand, "@p_ID_STATUS_PEDIDO_ADICIONAL", DbType.Int64, ID_STATUS_PEDIDO_ADICIONAL);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

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

        public DataTable ObterListaSolicitacaoPeca(Int64 ID_PEDIDO, string CD_TECNICO)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoSelectSolicitacaoPeca");

                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, CD_TECNICO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

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

        public DataTable ObterListaSolicitacaoPecaBPCS(Int64 ID_PEDIDO, string CD_TECNICO)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoSelectSolicitacaoPeca");

                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, CD_TECNICO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

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

        public DataTable ObterLista(PedidoEntity pedidoEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPedidoSelect");

                if (pedidoEntity.ID_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, pedidoEntity.ID_PEDIDO);
                }

                if (!string.IsNullOrEmpty(pedidoEntity.tecnico.CD_TECNICO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, pedidoEntity.tecnico.CD_TECNICO);
                }

                if (pedidoEntity.NR_DOCUMENTO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_NR_DOCUMENTO", DbType.Int64, pedidoEntity.NR_DOCUMENTO);
                }

                if (pedidoEntity.DT_CRIACAO != null)
                {
                    _db.AddInParameter(dbCommand, "@p_DT_CRIACAO", DbType.DateTime, pedidoEntity.DT_CRIACAO);
                }

                if (pedidoEntity.DT_ENVIO != null)
                {
                    _db.AddInParameter(dbCommand, "@p_DT_ENVIO", DbType.DateTime, pedidoEntity.DT_ENVIO);
                }

                if (pedidoEntity.DT_RECEBIMENTO != null)
                {
                    _db.AddInParameter(dbCommand, "@p_DT_RECEBIMENTO", DbType.DateTime, pedidoEntity.DT_RECEBIMENTO);
                }

                if (!string.IsNullOrEmpty(pedidoEntity.TX_OBS))
                {
                    _db.AddInParameter(dbCommand, "@p_TX_OBS", DbType.String, pedidoEntity.TX_OBS);
                }

                if (!string.IsNullOrEmpty(pedidoEntity.PENDENTE))
                {
                    _db.AddInParameter(dbCommand, "@p_PENDENTE", DbType.String, pedidoEntity.PENDENTE);
                }

                if (pedidoEntity.NR_DOC_ORI > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_NR_DOC_ORI", DbType.Int64, pedidoEntity.NR_DOC_ORI);
                }

                if (pedidoEntity.statusPedido.ID_STATUS_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_STATUS_PEDIDO", DbType.Int64, pedidoEntity.statusPedido.ID_STATUS_PEDIDO);
                }

                if (pedidoEntity.cliente.CD_CLIENTE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, pedidoEntity.cliente.CD_CLIENTE);
                }

                if (pedidoEntity.CD_PEDIDO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_PEDIDO", DbType.Int64, pedidoEntity.CD_PEDIDO);
                }

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

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

        public DataSet ObterRelatorio(Int64 ID_PEDIDO, Int64 NR_LOTE)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptSolicitacaoPecas");

                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);

                if (NR_LOTE > 0)
                    _db.AddInParameter(dbCommand, "@p_NR_LOTE", DbType.Int64, NR_LOTE);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                //dataTable = dataSet.Tables[0];
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
            return dataSet;
        }

    }
}
